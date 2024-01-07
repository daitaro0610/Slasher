using UnityEngine;

public abstract class ItemBase : MonoBehaviour
{
    [SerializeField]
    private Sprite m_ItemIcon;

    [SerializeField]
    protected int m_RemainingUsageCount;

    public abstract int UseItemAnimationHash();

    public abstract bool UseItem();

    public Sprite GetItemIcon() => m_ItemIcon;
    public bool CheckRemainingItems() => m_RemainingUsageCount > 0;
    public int GetRemainingUsageCount() => m_RemainingUsageCount;
}
