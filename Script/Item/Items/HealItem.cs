using UnityEngine;
using KD;

public class HealItem : ItemBase
{
    [SerializeField]
    private int m_HealValue;

    private PlayerHealth m_Health;


    [SerializeField]
    private Vector3 m_HealEffectOffset;

    private void Awake()
    {
        m_Health = FindAnyObjectByType<PlayerHealth>();
    }

    public override bool UseItem()
    {
        if (CheckRemainingItems())
        {
            m_RemainingUsageCount--;
            //�񕜂�����
            m_Health.Heal(m_HealValue);

            EffectManager.instance.Play(EffectName.Heal,m_Health.gameObject,OffSet: m_HealEffectOffset);

            Debugger.Log("��");
            return true;
        }
        return false;
    }

    public override int UseItemAnimationHash()
    {
        return AnimationHash.Heal;
    }
}
