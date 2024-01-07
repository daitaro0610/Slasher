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

    private ItemBase[] m_Items; //アイテムを配列にして選択時に切り替える
    private int m_CurrentIndex; //現在の要素番

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

        //アイコンを変更する
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
        //Positive側を押したら
        if (callbackContext.ReadValue<float>() > 0)
        {
            m_CurrentIndex++;
        }
        //Negative側を押したら
        else
        {
            m_CurrentIndex--;
        }

        m_CurrentIndex = (int)Mathf.Repeat(m_CurrentIndex, m_Items.Length);

        //アイテムを切り替える
        m_CurrentItem = m_Items[m_CurrentIndex];
        ChangeItemBarIcon();
    }

    private void ChangeItemBarIcon()
    {
        m_ItemBarIcons[0].sprite = m_CurrentItem.GetItemIcon();//中心のアイコンを変更
        m_ItemBarIcons[1].sprite = m_Items[(int)Mathf.Repeat(m_CurrentIndex + 1, m_Items.Length)].GetItemIcon(); //右側のアイコンを変更
        m_ItemBarIcons[2].sprite = m_Items[(int)Mathf.Repeat(m_CurrentIndex - 1, m_Items.Length)].GetItemIcon(); //左側のアイコンを変更

        //残り使用回数を表示
        m_UsageCountText.text = $"{m_CurrentItem.GetRemainingUsageCount()}";
    }


    /// <summary>
    /// Itemを使用する
    /// </summary>
    public bool UseItem()
    {
        //クールタイムが終わっていない場合はFalse
        if (!CheckItemUsageInterval()) return false;

        bool result = m_CurrentItem.UseItem();

        //アイテムを使用することができた場合、アイテムの数の更新を行う
        if (result)
        {
            ResetInterval();
            m_UsageCountText.text = $"{m_CurrentItem.GetRemainingUsageCount()}";
        }

        return result;
    }

    /// <summary>
    /// アイテム固有のアニメーションのハッシュを取得する
    /// </summary>
    /// <returns></returns>
    public int GetUseItemAnimationHash()
        => m_CurrentItem.UseItemAnimationHash();

    /// <summary>
    /// アイテムを使える状態ならTrue
    /// </summary>
    /// <returns></returns>
    private bool CheckItemUsageInterval() => m_ItemIntervalTimer == 0;

    /// <summary>
    /// インターバルの初期化
    /// </summary>
    private void ResetInterval() => m_ItemIntervalTimer = m_ItemUsageInterval;
}
