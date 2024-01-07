using UnityEngine;
using UnityEngine.InputSystem;
using KD;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private InputActionAsset m_InputActions;

    private PlayerGame m_PlayerGame;

    private CharacterMovement m_Movement;
    private CharacterAttack m_Attack;
    private PlayerHealth m_Health;
    private AnimationController m_Animation;
    private PlayerVoiceManager m_PlayerVoice;
    private KatanaSeManager m_KatanaSe;

    private StaminaSystem m_Stamina;

    private bool m_IsStop;
    private bool m_IsDash;

    [SerializeField]
    private float m_AvoidanceInterval = 2f;
    private float m_AvoidanceCoolTime;

    private Vector2 m_ForwardVelocity;

    private float m_JumpMaxHeight;  //�W�����v�����Ƃ��̍ō��n�_�ƒn�ʂƂ̋���

    [SerializeField]
    private Vector3 m_GroundRayOffset;�@//���g����Ray���΂��Ƃ��Ɉʒu��␳���邽�߂�Offset

    [SerializeField]
    private float m_FallMinHeight = 0.3f;�@//����������s���Œ�l
    [SerializeField]
    private float m_LandingHeight = 0.1f;  //�n�ʂɐڂ��Ă���Ɣ��肷��ō��l

    private CapsuleCollider m_GroundCastCollider; //�n�ʂɐڂ��Ă��邩�ǂ������肷�邽�߂̃R���C�_�[�@���Radius���g�p����

    private HitStopSystem m_HitStop; //�U�����Ɏ��Ԃ������x�点�鏈��

    private ItemSelectController m_ItemSelectController;

    public enum PlayerBaseState
    {
        Normal,
        Battle,
    }
    private PlayerBaseState m_BaseState;

    public enum State
    {
        Idle,
        Counter,
        Attack,
        Jump,
        Avoidance,
        Fall,
        Death,
    }
    [SerializeField, ReadOnly]
    private State m_State;

    public State GetState() => m_State;

    private void Awake()
    {
        m_Movement = GetComponent<CharacterMovement>();
        m_Attack = GetComponent<CharacterAttack>();
        m_Health = GetComponent<PlayerHealth>();
        m_Animation = new(gameObject);
        m_PlayerVoice = new();
        m_KatanaSe = new();

        m_GroundCastCollider = GetComponent<CapsuleCollider>();

        m_Stamina = FindAnyObjectByType<StaminaSystem>();

        //�q�b�g�X�g�b�v����
        var hitStopObj = GameObject.FindGameObjectWithTag(TagName.HitStop);
        if (hitStopObj != null)
            m_HitStop = hitStopObj.GetComponent<HitStopSystem>();

        m_ItemSelectController = FindAnyObjectByType<ItemSelectController>();

        RebindSaveSystem saveSystem = new();
        m_PlayerGame = new();
        saveSystem.Load(m_PlayerGame.asset);

        //Action�C�x���g�o�^
        m_PlayerGame.Player.Move.started += OnMove;
        m_PlayerGame.Player.Move.performed += OnMove;
        m_PlayerGame.Player.Move.canceled += OnMove;

        m_PlayerGame.Player.Dash.started += OnDash;
        m_PlayerGame.Player.Dash.canceled += OnDashEnd;

        //�W�����v����
        m_PlayerGame.Player.Jump.started += OnAvoidance;
        //�U������
        m_PlayerGame.Player.Attack.started += OnAttack;
        m_PlayerGame.Player.SubAttack.started += OnSubAttack;
        //�J�E���^�[
        m_PlayerGame.Player.Counter.started += OnCounter;
        m_PlayerGame.Player.Counter.canceled += OnCounterEnd;

        //�����g�p����A�N�V�����p
        m_PlayerGame.Player.Use.started += OnUse;
        m_PlayerGame.Player.ItemSelect.started += m_ItemSelectController.OnItemSelect;

        m_PlayerGame.Enable();
    }

    private void Start()
    {
        m_State = State.Idle;
        m_BaseState = PlayerBaseState.Normal;
        m_IsDash = false;
        m_IsStop = false;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        //�d�͂̏���
        m_Movement.OnGravity();

        switch (m_State)
        {
            case State.Idle: MoveUpdate(); break;
            case State.Fall: FallUpdate(); break;
        }
    }
    private void Update()
    {
        //����̃N�[���^�C�������炷
        m_AvoidanceCoolTime -= Time.deltaTime;
    }


    private void MoveInput()
    {
        if (m_IsStop) return;

        if (m_ForwardVelocity != Vector2.zero)
        {
            m_Movement.OnCharacterMove(m_ForwardVelocity);

            //�_�b�V�����̏ꍇ�A�X�^�~�i�����炷
            if (m_IsDash)
            {
                if (!m_Stamina.SetUse(true))
                {
                    DashEnd();
                }
            }
        }
    }


    private void MoveUpdate()
    {
        MoveInput();
        m_Animation.SetMove(m_ForwardVelocity.magnitude);

        //���̍����ȏゾ�����痎����Ԃɐ؂�ւ���
        if (!IsCheckGround())
        {
            m_State = State.Fall;
        }
    }

    private void JumpUpdate()
    {
        MoveInput();
        CheckHeight();
    }

    private void FallUpdate()
    {
        MoveInput();
        CheckIfGrounded();
    }

    private void DashEnd()
    {
        m_IsDash = false;
        m_Animation.IsDash(false);
        m_Stamina.SetUse(false);
        m_Movement.SetSpeed(0);
    }

    public void Counter()
    {
        m_Animation.Counter();
        m_HitStop.HitStop(0.3f);
    }

    public void Damage()
    {
        m_BaseState = PlayerBaseState.Battle;
        m_State = State.Idle;
        //�_���[�W�̏���
        m_Animation.SetDamageNumber(1);
        m_Animation.Damage();
        m_Movement.SetSpeed(1);
    }

    public void Death()
    {
        m_State = State.Death;
        m_Animation.Death();
        GameManager.Instance.PlayerDeath();
    }

    public void Clear()
    {
        m_IsStop = true;
        m_Animation.Clear();
    }

    #region InputSystem�̃R�[���o�b�N

    public void OnMove(InputAction.CallbackContext callback)
    {
        m_ForwardVelocity = callback.ReadValue<Vector2>();
    }

    public void OnDash(InputAction.CallbackContext callback)
    {
        if (m_BaseState == PlayerBaseState.Battle) return;
        m_IsDash = true;
        m_Animation.IsDash(true);
        m_Movement.SetSpeed(2);
    }
    public void OnDashEnd(InputAction.CallbackContext callback)
    {
        if (m_BaseState == PlayerBaseState.Battle) return;
        DashEnd();
    }

    public void OnAttack(InputAction.CallbackContext callback)
    {
        //�������߂Ă����ԂȂ�Δ����U�����s��
        if (m_BaseState != PlayerBaseState.Battle)
        {
            InformationTextController.Instance.AttackStart();
            //�o�g����ԂɂȂ��ł̏���������
            m_BaseState = PlayerBaseState.Battle;
            m_IsDash = false;
            m_Stamina.SetUse(false);
            m_State = State.Attack;
            m_Animation.Attack();
            m_Movement.SetSpeed(1);
            return;
        }

        if (m_State == State.Idle)
        {
            m_State = State.Attack;
            m_Animation.Attack();
        }
        else if (m_State == State.Attack)
        {
            m_Animation.Attack();
        }
        else if (m_State == State.Counter)
        {
            m_Animation.Attack();
        }
    }

    public void OnSubAttack(InputAction.CallbackContext callback)
    {
        if (m_BaseState != PlayerBaseState.Battle && m_State != State.Attack) return;

        //�T�u�U�����s���@
        //���[�v�ɂȂ���悤�ȍU��
        m_Animation.SubAttack();
    }

    /// <summary>
    /// ���̔�����[��������Ƃ��Ɏg�p����
    /// </summary>
    /// <param name="callback"></param>
    public void OnUse(InputAction.CallbackContext callback)
    {
        if (m_BaseState == PlayerBaseState.Battle)
        {
            SwordAction();
        }
        else if(m_BaseState == PlayerBaseState.Normal)
        {
            if (m_ItemSelectController.UseItem())
            {
                //�A�C�e�����g�p���邱�Ƃ��ł����ꍇ�̓A�j���[�V�������s��
                m_ItemSelectController.GetUseItemAnimationHash();
            }
        }
    }

    private void SwordAction()
    {
        InformationTextController.Instance.IdleStart();
        m_BaseState = PlayerBaseState.Normal;
        m_Animation.ReturnSword();
        m_Movement.SetSpeed(0);
    }

    public void OnAvoidance(InputAction.CallbackContext callback)
    {
        if (m_BaseState != PlayerBaseState.Battle || m_IsStop) return;

        if (m_AvoidanceCoolTime > 0) return;

        if (m_Stamina.UseStamina(m_Stamina.StaminaMaxValue / 5))
        {
            if (m_State == State.Idle)
            {
                m_Animation.AvoidanceVelocity(0);
            }
            else if (m_State == State.Attack)
            {
                m_Animation.AvoidanceVelocity(m_ForwardVelocity.x);
            }
            else return;

            m_State = State.Avoidance;
            //���p�̃N�[���^�C���ݒ�
            m_AvoidanceCoolTime = m_AvoidanceInterval;
            m_Animation.Avoidance();
        }
    }

    private void OnCounter(InputAction.CallbackContext callback)
    {
        if (m_State != State.Idle || m_BaseState != PlayerBaseState.Battle) return;

        m_State = State.Counter;
        m_Animation.Counter(true);
    }

    private void OnCounterEnd(InputAction.CallbackContext callback)
    {
        if (m_State != State.Counter || m_BaseState != PlayerBaseState.Battle) return;

        m_State = State.Idle;
        m_Animation.Counter(false);
    }

    #endregion

    #region �A�j���[�V�����̃R�[���o�b�N

    private void CounterEnd()
    {
        m_State = State.Idle;
        Debugger.Log("�J�E���^�[�I��");
        m_Health.SetIsInvincible(false);
    }

    private void Stopped()
    {
        m_IsStop = true;
    }

    private void StopEnd()
    {
        m_IsStop = false;
    }

    /// <summary>
    /// ���i�̍U���≡�����̍U���͂���
    /// </summary>
    /// <param name="damageValueIndex"></param>
    private void AnimAttack(int damageValueIndex)
    {
        //�_���[�W��^�����ꍇ�ASE��炷
        if (AttackCheck(0, damageValueIndex))
        {
            m_KatanaSe.PlayNormalAttack();
        }
    }

    /// <summary>
    /// �c�ɑ傫���R���C�_�[���g�p����ꍇ�͂���
    /// </summary>
    /// <param name="damageValueIndex"></param>
    private void AnimAttackVertical(int damageValueIndex)
    {
        //�_���[�W��^�����ꍇ�ASE��炷
        if (AttackCheck(1, damageValueIndex))
        {
            Debugger.Log("�_���[�W");
            m_KatanaSe.PlayNormalAttack();
        }
    }

    private void AnimAttackLong(int damageValueIndex)
    {
        //�_���[�W��^�����ꍇ�ASE��炷
        if (AttackCheck(2, damageValueIndex))
        {
            Debugger.Log("�_���[�W");
            m_KatanaSe.PlayCounterAttack();
        }
    }

    private void AnimCounterAttack(int damageValueIndex)
    {
        //�_���[�W��^�����ꍇ�ASE��炷
        if (AttackCheck(3, damageValueIndex))
        {
            Debugger.Log("�_���[�W");
            m_KatanaSe.PlayCounterAttack();
        }
    }

    private bool AttackCheck(int colliderIndex,int damageValueIndex)
    {
        m_Attack.SetCollider(colliderIndex);
        m_Attack.SetDamageValue(damageValueIndex);
        return m_Attack.Attack();
    }

    private void AttackEnd()
    {
        m_State = State.Idle;
    }

    /// <summary>
    /// �U����Ԃ̎��Ɏg�p������p
    /// </summary>
    /// <param name="index"></param>
    private void AnimAvoidance(int index)
    {
        m_State = State.Avoidance;

        AudioManager.instance.PlaySE(SoundName.Avoidance,1);

        //AddForce���g�p���ĉ������Ɉړ�����
        switch (index)
        {
            case 0: //���ʂɉ��
                m_Movement.Avoidance(transform.forward);

                break;
            case 1: //�E�����ɉ��
                m_Movement.Avoidance(transform.right);

                break;
            case 2: //�������ɉ��
                m_Movement.Avoidance(-transform.right);

                break;
        }
    }


    private void AvoidanceEnd()
    {
        m_State = State.Idle;
    }

    private void AttackVoice(int index)
    {
        m_PlayerVoice.AttackVoice(index, transform);
    }


    /// <summary>
    /// �[�����̌��ʉ�
    /// </summary>
    private void SheathingSwordAnim()
    {
        m_KatanaSe.PlaySheathingSword();
    }

    /// <summary>
    /// �������̌��ʉ�
    /// </summary>
    private void DrawingSwordAnim()
    {
        m_KatanaSe.PlayDrawingSword();
    }

    #endregion

    #region �n�ʊ֌W�̏���

    /// <summary>
    /// �n�ʂ̏�ɂ��邩�ǂ����m�F����
    /// </summary>
    private bool IsCheckGround()
    {
        if (CastRayDownward(out RaycastHit hit))
        {
            if (hit.distance > m_FallMinHeight)
            {
                return false;
            }
            else return true;
        }

        //�n�ʂƓ�����Ȃ������ꍇ��False
        Debugger.LogWarning("�n�ʂƏՓ˂��܂���ł���");
        return false;
    }

    /// <summary>
    ///�����̍ō��n�_���v�����A����ɍ��킹�Ē��n���̃A�j���[�V������ύX����
    /// </summary>
    private void CheckHeight()
    {
        if (CastRayDownward(out RaycastHit hit))
        {
            //�l�̑傫�������ő�l�Ƃ���
            m_JumpMaxHeight = Mathf.Max(m_JumpMaxHeight, hit.distance);

            //hit.distance�̒l�����������ꍇ�͗���State�ɕς���
            if (m_JumpMaxHeight > hit.distance)
            {
                m_State = State.Fall;
                // m_Animation.SetFall(true);
            }
        }
    }


    /// <summary>
    /// �n�ʂƂ̋��������C�L���X�g�Ŕ��肵�Ĉ��̋����ɂȂ����璅�n����
    /// </summary>
    private void CheckIfGrounded()
    {
        if (CastRayDownward(out RaycastHit hit))
        {
            //�n�ʂƈ��̋����܂ŋ߂Â����璅�n�������s��
            if (hit.distance <= m_LandingHeight)
            {
                OnGround();
            }
        }
    }

    /// <summary>
    /// �������Ƀ��C�L���X�g���΂��Ĕ��肷��
    /// </summary>
    /// <param name="hit"></param>
    /// <returns>Ray���R���C�_�[�ƏՓ˂�����True�@���Ȃ����False</returns>
    private bool CastRayDownward(out RaycastHit hit)
    {
        Ray ray = new(transform.position + m_GroundRayOffset, Vector3.down);
        if (Physics.SphereCast(ray, m_GroundCastCollider.radius, out hit))
        {
            return true;
        }

        return false;
    }

    //�n�ʂƂ̋������߂��Ȃ�����W�����v�X�e�[�g���I��������
    private void OnGround()
    {
        m_State = State.Idle;
        //m_Animation.SetFall(false);

        //�����̍ō��_�Ƃ̋����ɉ����Ē��n�A�j���[�V������ω�������
        if (m_JumpMaxHeight > 5.0f)
        {
            Debugger.Log("High");
        }
        else if (m_JumpMaxHeight > 2.0f)
        {
            Debugger.Log("Low");
        }
    }

    #endregion

    private void OnDestroy()
    {
        m_PlayerGame?.Dispose();
    }
}
