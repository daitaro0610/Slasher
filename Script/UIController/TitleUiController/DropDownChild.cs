using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class DropDownChild : Selectable, IPointerClickHandler, IEventSystemHandler, ISubmitHandler
{
    private TextMeshProUGUI m_OptionText;

    private DropDownController m_DropDownController;
    private Image m_BackGround;
    [SerializeField]
    private UnityEvent m_OnSubmitEvent;

    private bool m_IsSelect;
    public bool IsCurrentSelect() => m_IsSelect;

    public void Initialize(DropDownController dropDownController)
    {
        GetBackGround();
        m_OptionText = GetComponentInChildren<TextMeshProUGUI>();
        m_DropDownController = dropDownController;
    }

    public void GetBackGround()
       => m_BackGround = GetComponent<Image>();

    public TextMeshProUGUI GetText()
       => m_OptionText;

    public void SetHeight(float height)
    {
        if (m_BackGround == null)
            GetBackGround();

        m_BackGround.rectTransform.sizeDelta = new Vector2(m_BackGround.rectTransform.sizeDelta.x, height);
    }

    #region EventSystemのコールバック関数

    public void OnPointerClick(PointerEventData eventData)
    {
        m_DropDownController.OnSubmit(this);
        OnEvent(m_OnSubmitEvent);
    }

    public void OnSubmit(BaseEventData eventData)
    {
        m_DropDownController.OnSubmit(this);
        OnEvent(m_OnSubmitEvent);
    }

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        m_IsSelect = true;
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);
        m_IsSelect = false;
    }

    #endregion

    public void Submit()
    {
        OnEvent(m_OnSubmitEvent);
    }

    /// <summary>
    /// UnityEventを実行する
    /// </summary>
    /// <param name="unityEvent"></param>
    private void OnEvent(UnityEvent unityEvent)
    {
        if (unityEvent != null)
            unityEvent.Invoke();
    }

}
