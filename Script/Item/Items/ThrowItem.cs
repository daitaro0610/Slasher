using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowItem : ItemBase
{
    [SerializeField]
    private GameObject m_ThrowItem;

    [SerializeField]
    private Transform m_ThrowPos;

    [SerializeField]
    private float m_ThrowPower;

    public override bool UseItem()
    {
        if (CheckRemainingItems())
        {
            m_RemainingUsageCount--;
            //•¨‚ğ“Š‚°‚é
            Debugger.Log("•¨‚ğ“Š‚°‚é");

            GameObject throwObj = Instantiate(m_ThrowItem, m_ThrowPos.position, Quaternion.identity);
            
            //“Š‚°‚éˆ—‚ğs‚¤
            Rigidbody rig = throwObj.GetComponent<Rigidbody>();
            rig.AddForce(m_ThrowPos.forward * m_ThrowPower,ForceMode.Impulse);

            return true;
        }
        return false;
    }

    public override int UseItemAnimationHash()
    {
        return AnimationHash.Throw;
    }
}
