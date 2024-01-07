using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SelectionUi : Selectable, IEventSystemHandler
{
    [SerializeField]
    private UnityEvent m_SelectEvent;
    [SerializeField]
    private UnityEvent m_DeselectEvent;

    private MaskableGraphic m_Graphic;


    protected override void Awake()
    {
        m_Graphic = GetComponent<MaskableGraphic>();
    }

    /// <summary>
    /// �I�����̃C�x���g��ǉ�����
    /// </summary>
    /// <param name="action"></param>
    public void SetSelectEvent(UnityEvent action){
        m_SelectEvent = action;
    }

    /// <summary>
    /// �I������O�ꂽ�Ƃ��̃C�x���g��ǉ�����
    /// </summary>
    /// <param name="action"></param>
    public void SetDeselectEvent(UnityEvent action)
    {
        m_DeselectEvent = action;
    }

    /// <summary>
    /// �I�����ꂽ�Ƃ��ɌĂ΂�鏈��
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        if(m_SelectEvent != null)
        {
            m_SelectEvent.Invoke();
        }
    }

    /// <summary>
    /// �I�����O�ꂽ�Ƃ��ɌĂ΂�鏈��
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);
        if(m_DeselectEvent != null)
        {
            m_DeselectEvent.Invoke();
        }
    }
}
