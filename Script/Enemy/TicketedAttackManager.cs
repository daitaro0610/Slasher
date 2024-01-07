using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TicketedAttackManager
{
    private static TicketedAttackManager m_Instance;
    public static TicketedAttackManager Instance => m_Instance;

    private const int DEFAULT_TICKET_COUNT = 3;
    private int m_TicketCount;
    
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public TicketedAttackManager()
    {
        if (m_Instance != null) return;

        m_Instance = this;
        m_TicketCount = DEFAULT_TICKET_COUNT;
    }

    /// <summary>
    /// チケットを消費する
    /// </summary>
    /// <returns>チケットがある場合はTrue 無い場合はFalse</returns>
    public bool UseTicket()
    {
        if (m_TicketCount <= 0)
            return false;

        m_TicketCount--;
        return true;
    }

    /// <summary>
    /// チケットを返却する
    /// </summary>
    public void ReturnTicket()
    {
        if (m_TicketCount == DEFAULT_TICKET_COUNT) return;

        m_TicketCount++;
    }

}
