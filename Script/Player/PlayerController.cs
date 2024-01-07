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

    private float m_JumpMaxHeight;  //ジャンプしたときの最高地点と地面との距離

    [SerializeField]
    private Vector3 m_GroundRayOffset;　//自身からRayを飛ばすときに位置を補正するためのOffset

    [SerializeField]
    private float m_FallMinHeight = 0.3f;　//落下判定を行う最低値
    [SerializeField]
    private float m_LandingHeight = 0.1f;  //地面に接していると判定する最高値

    private CapsuleCollider m_GroundCastCollider; //地面に接しているかどうか判定するためのコライダー　主にRadiusを使用する

    private HitStopSystem m_HitStop; //攻撃時に時間を少し遅らせる処理

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

        //ヒットストップ実装
        var hitStopObj = GameObject.FindGameObjectWithTag(TagName.HitStop);
        if (hitStopObj != null)
            m_HitStop = hitStopObj.GetComponent<HitStopSystem>();

        m_ItemSelectController = FindAnyObjectByType<ItemSelectController>();

        RebindSaveSystem saveSystem = new();
        m_PlayerGame = new();
        saveSystem.Load(m_PlayerGame.asset);

        //Actionイベント登録
        m_PlayerGame.Player.Move.started += OnMove;
        m_PlayerGame.Player.Move.performed += OnMove;
        m_PlayerGame.Player.Move.canceled += OnMove;

        m_PlayerGame.Player.Dash.started += OnDash;
        m_PlayerGame.Player.Dash.canceled += OnDashEnd;

        //ジャンプ処理
        m_PlayerGame.Player.Jump.started += OnAvoidance;
        //攻撃処理
        m_PlayerGame.Player.Attack.started += OnAttack;
        m_PlayerGame.Player.SubAttack.started += OnSubAttack;
        //カウンター
        m_PlayerGame.Player.Counter.started += OnCounter;
        m_PlayerGame.Player.Counter.canceled += OnCounterEnd;

        //何か使用するアクション用
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
        //重力の処理
        m_Movement.OnGravity();

        switch (m_State)
        {
            case State.Idle: MoveUpdate(); break;
            case State.Fall: FallUpdate(); break;
        }
    }
    private void Update()
    {
        //回避のクールタイムを減らす
        m_AvoidanceCoolTime -= Time.deltaTime;
    }


    private void MoveInput()
    {
        if (m_IsStop) return;

        if (m_ForwardVelocity != Vector2.zero)
        {
            m_Movement.OnCharacterMove(m_ForwardVelocity);

            //ダッシュ中の場合、スタミナを減らす
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

        //一定の高さ以上だったら落下状態に切り替える
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
        //ダメージの処理
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

    #region InputSystemのコールバック

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
        //剣を収めている状態ならば抜刀攻撃を行う
        if (m_BaseState != PlayerBaseState.Battle)
        {
            InformationTextController.Instance.AttackStart();
            //バトル状態になる上での初期化処理
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

        //サブ攻撃を行う　
        //ループにつながるような攻撃
        m_Animation.SubAttack();
    }

    /// <summary>
    /// 剣の抜刀や納刀をするときに使用する
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
                //アイテムを使用することができた場合はアニメーションを行う
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
            //回避用のクールタイム設定
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

    #region アニメーションのコールバック

    private void CounterEnd()
    {
        m_State = State.Idle;
        Debugger.Log("カウンター終了");
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
    /// 普段の攻撃や横向きの攻撃はこれ
    /// </summary>
    /// <param name="damageValueIndex"></param>
    private void AnimAttack(int damageValueIndex)
    {
        //ダメージを与えた場合、SEを鳴らす
        if (AttackCheck(0, damageValueIndex))
        {
            m_KatanaSe.PlayNormalAttack();
        }
    }

    /// <summary>
    /// 縦に大きいコライダーを使用する場合はこれ
    /// </summary>
    /// <param name="damageValueIndex"></param>
    private void AnimAttackVertical(int damageValueIndex)
    {
        //ダメージを与えた場合、SEを鳴らす
        if (AttackCheck(1, damageValueIndex))
        {
            Debugger.Log("ダメージ");
            m_KatanaSe.PlayNormalAttack();
        }
    }

    private void AnimAttackLong(int damageValueIndex)
    {
        //ダメージを与えた場合、SEを鳴らす
        if (AttackCheck(2, damageValueIndex))
        {
            Debugger.Log("ダメージ");
            m_KatanaSe.PlayCounterAttack();
        }
    }

    private void AnimCounterAttack(int damageValueIndex)
    {
        //ダメージを与えた場合、SEを鳴らす
        if (AttackCheck(3, damageValueIndex))
        {
            Debugger.Log("ダメージ");
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
    /// 攻撃状態の時に使用する回避用
    /// </summary>
    /// <param name="index"></param>
    private void AnimAvoidance(int index)
    {
        m_State = State.Avoidance;

        AudioManager.instance.PlaySE(SoundName.Avoidance,1);

        //AddForceを使用して回避方向に移動する
        switch (index)
        {
            case 0: //正面に回避
                m_Movement.Avoidance(transform.forward);

                break;
            case 1: //右方向に回避
                m_Movement.Avoidance(transform.right);

                break;
            case 2: //左方向に回避
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
    /// 納刀時の効果音
    /// </summary>
    private void SheathingSwordAnim()
    {
        m_KatanaSe.PlaySheathingSword();
    }

    /// <summary>
    /// 抜刀時の効果音
    /// </summary>
    private void DrawingSwordAnim()
    {
        m_KatanaSe.PlayDrawingSword();
    }

    #endregion

    #region 地面関係の処理

    /// <summary>
    /// 地面の上にいるかどうか確認する
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

        //地面と当たらなかった場合はFalse
        Debugger.LogWarning("地面と衝突しませんでした");
        return false;
    }

    /// <summary>
    ///高さの最高地点を計測し、それに合わせて着地時のアニメーションを変更する
    /// </summary>
    private void CheckHeight()
    {
        if (CastRayDownward(out RaycastHit hit))
        {
            //値の大きい方を最大値とする
            m_JumpMaxHeight = Mathf.Max(m_JumpMaxHeight, hit.distance);

            //hit.distanceの値が下がりつつある場合は落下Stateに変える
            if (m_JumpMaxHeight > hit.distance)
            {
                m_State = State.Fall;
                // m_Animation.SetFall(true);
            }
        }
    }


    /// <summary>
    /// 地面との距離をレイキャストで判定して一定の距離になったら着地する
    /// </summary>
    private void CheckIfGrounded()
    {
        if (CastRayDownward(out RaycastHit hit))
        {
            //地面と一定の距離まで近づいたら着地処理を行う
            if (hit.distance <= m_LandingHeight)
            {
                OnGround();
            }
        }
    }

    /// <summary>
    /// 下方向にレイキャストを飛ばして判定する
    /// </summary>
    /// <param name="hit"></param>
    /// <returns>Rayがコライダーと衝突したらTrue　しなければFalse</returns>
    private bool CastRayDownward(out RaycastHit hit)
    {
        Ray ray = new(transform.position + m_GroundRayOffset, Vector3.down);
        if (Physics.SphereCast(ray, m_GroundCastCollider.radius, out hit))
        {
            return true;
        }

        return false;
    }

    //地面との距離が近くなったらジャンプステートを終了させる
    private void OnGround()
    {
        m_State = State.Idle;
        //m_Animation.SetFall(false);

        //高さの最高点との距離に応じて着地アニメーションを変化させる
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
