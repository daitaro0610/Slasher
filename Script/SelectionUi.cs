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
    /// 選択時のイベントを追加する
    /// </summary>
    /// <param name="action"></param>
    public void SetSelectEvent(UnityEvent action){
        m_SelectEvent = action;
    }

    /// <summary>
    /// 選択から外れたときのイベントを追加する
    /// </summary>
    /// <param name="action"></param>
    public void SetDeselectEvent(UnityEvent action)
    {
        m_DeselectEvent = action;
    }

    /// <summary>
    /// 選択されたときに呼ばれる処理
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
    /// 選択が外れたときに呼ばれる処理
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
