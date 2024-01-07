using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

public class ResizableManipulator : MouseManipulator
{
    private bool m_Active;
    private Vector2 m_Start;
    private Vector2 m_InitialSize;

    public ResizableManipulator()
    {
        activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse });
    }

    protected override void RegisterCallbacksOnTarget()
    {
        target.RegisterCallback<MouseDownEvent>(OnMouseDown);
        target.RegisterCallback<MouseUpEvent>(OnMouseUp);
        target.RegisterCallback<MouseMoveEvent>(OnMouseMove);
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
        target.UnregisterCallback<MouseUpEvent>(OnMouseUp);
        target.UnregisterCallback<MouseMoveEvent>(OnMouseMove);
    }

    private void OnMouseDown(MouseDownEvent e)
    {
        if (m_Active)
            return;

        if (CanStartManipulation(e))
        {
            m_Active = true;
            m_Start = e.localMousePosition;
            m_InitialSize = target.layout.size;
            target.CaptureMouse();
            e.StopPropagation();
        }
    }

    private void OnMouseUp(MouseUpEvent e)
    {
        if (!m_Active || !target.HasMouseCapture())
            return;

        target.ReleaseMouse();
        m_Active = false;
        e.StopPropagation();
    }

    private void OnMouseMove(MouseMoveEvent e)
    {
        if (!m_Active || !target.HasMouseCapture())
            return;

        Vector2 diff = e.localMousePosition - m_Start;
        target.style.width = m_InitialSize.x + diff.x;
        target.style.height = m_InitialSize.y + diff.y;
        e.StopPropagation();
    }
}