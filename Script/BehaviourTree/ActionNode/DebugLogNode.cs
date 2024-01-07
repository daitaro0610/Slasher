using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugLogNode : ActionNode
{
    public enum DisplayLog
    {
        Start,
        Update,
        Stop,
        All,
    }
    [SerializeField]
    private DisplayLog m_DisplayLog = DisplayLog.All;


    public string m_Message;
    protected override void OnStart()
    {
        if (m_DisplayLog == DisplayLog.Start || m_DisplayLog == DisplayLog.All)
            Debugger.Log($"OnStart{m_Message}");
    }

    protected override void OnStop()
    {
        if ( m_DisplayLog == DisplayLog.Stop || m_DisplayLog == DisplayLog.All)
        Debugger.Log($"OnStop{m_Message}");
    }

    protected override State OnUpdate()
    {
        if (m_DisplayLog == DisplayLog.Update || m_DisplayLog == DisplayLog.All)
            Debugger.Log($"OnUpdate{m_Message}");

        return State.Success;
    }
}
