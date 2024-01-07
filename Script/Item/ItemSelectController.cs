using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class ItemSelectController : MonoBehaviour
{
    private PlayerGame m_PlayerInput;

    [SerializeField]
    private Image[] m_ItemBarIcons;
    [SerializeField]
    private TextMeshProUGUI m_UsageCountText;

    private ItemBase[] m_Items; //�A�C�e����z��ɂ��đI�����ɐ؂�ւ���
    private int m_CurrentIndex; //���݂̗v�f��

    private ItemBase m_CurrentItem;

    [SerializeField]
    private Image m_CoolDownImage;

    [SerializeField]
    private float m_ItemUsageInterval = 2f;
    private float m_ItemIntervalTimer;

    private void Awake()
    {
        m_Items = FindObjectsByType<ItemBase>(FindObjectsSortMode.InstanceID);

        m_CurrentIndex = 0;
        m_CurrentItem = m_Items[m_CurrentIndex];

        m_ItemIntervalTimer = 0;

        //�A�C�R����ύX����
        ChangeItemBarIcon();
    }

    private void Update()
    {
        if (m_ItemIntervalTimer > 0)
        {
            m_ItemIntervalTimer -= Time.deltaTime;
        }
        else
        {
            m_ItemIntervalTimer = 0;
        }

        float normalizeValue = m_ItemIntervalTimer / m_ItemUsageInterval;
        m_CoolDownImage.fillAmount = normalizeValue;
    }

    public void OnItemSelect(InputAction.CallbackContext callbackContext)
    {
        //Positive������������
        if (callbackContext.ReadValue<float>() > 0)
        {
            m_CurrentIndex++;
        }
        //Negative������������
        else
        {
            m_CurrentIndex--;
        }

        m_CurrentIndex = (int)Mathf.Repeat(m_CurrentIndex, m_Items.Length);

        //�A�C�e����؂�ւ���
        m_CurrentItem = m_Items[m_CurrentIndex];
        ChangeItemBarIcon();
    }

    private void ChangeItemBarIcon()
    {
        m_ItemBarIcons[0].sprite = m_CurrentItem.GetItemIcon();//���S�̃A�C�R����ύX
        m_ItemBarIcons[1].sprite = m_Items[(int)Mathf.Repeat(m_CurrentIndex + 1, m_Items.Length)].GetItemIcon(); //�E���̃A�C�R����ύX
        m_ItemBarIcons[2].sprite = m_Items[(int)Mathf.Repeat(m_CurrentIndex - 1, m_Items.Length)].GetItemIcon(); //�����̃A�C�R����ύX

        //�c��g�p�񐔂�\��
        m_UsageCountText.text = $"{m_CurrentItem.GetRemainingUsageCount()}";
    }


    /// <summary>
    /// Item���g�p����
    /// </summary>
    public bool UseItem()
    {
        //�N�[���^�C�����I����Ă��Ȃ��ꍇ��False
        if (!CheckItemUsageInterval()) return false;

        bool result = m_CurrentItem.UseItem();

        //�A�C�e�����g�p���邱�Ƃ��ł����ꍇ�A�A�C�e���̐��̍X�V���s��
        if (result)
        {
            ResetInterval();
            m_UsageCountText.text = $"{m_CurrentItem.GetRemainingUsageCount()}";
        }

        return result;
    }

    /// <summary>
    /// �A�C�e���ŗL�̃A�j���[�V�����̃n�b�V�����擾����
    /// </summary>
    /// <returns></returns>
    public int GetUseItemAnimationHash()
        => m_CurrentItem.UseItemAnimationHash();

    /// <summary>
    /// �A�C�e�����g�����ԂȂ�True
    /// </summary>
    /// <returns></returns>
    private bool CheckItemUsageInterval() => m_ItemIntervalTimer == 0;

    /// <summary>
    /// �C���^�[�o���̏�����
    /// </summary>
    private void ResetInterval() => m_ItemIntervalTimer = m_ItemUsageInterval;
}
