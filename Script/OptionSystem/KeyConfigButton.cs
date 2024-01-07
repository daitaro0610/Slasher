using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class KeyConfigButton : Selectable, IPointerClickHandler, IEventSystemHandler, ISubmitHandler
{
    private TextMeshProUGUI m_Text;
    [SerializeField, Space(10)]
    private InputActionReference m_InputActionReference; //変更したいボタン入力

    //このスクリプトでどのデバイスに対して変更を行いたいのか
    private enum Device
    {
        Keyboard,
        Gamepad,
        Mouse
    }
    [SerializeField]
    private Device m_Device;

    private RebindSystem m_Rebind;


    protected override void Awake()
    {
        m_Text = GetComponentInChildren<TextMeshProUGUI>();

        SetInputAction();

        DisplayBoundInputText();
    }

    /// <summary>
    /// InputActionを設定する
    /// </summary>
    private void SetInputAction()
    {
        m_Rebind = new(m_InputActionReference);
        switch (m_Device)
        {
            case Device.Gamepad:
                m_Rebind.SetDevice<Gamepad>();
                break;

            case Device.Keyboard:
                m_Rebind.SetDevice<Keyboard>();
                break;

            case Device.Mouse:
                m_Rebind.SetDevice<Mouse>();
                break;
        }
    }

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Rebind();
    }

    public void OnSubmit(BaseEventData eventData)
    {
        Rebind();
    }

    private void DisplayBoundInputText()
    {
        m_Text.text = m_Rebind.GetBindingDisplayString();
    }

    private void Rebind()
    {
        m_Rebind.StartRebind()
            .OnStart(() =>
            {
                m_Text.text = "Binding...";
            })
            .OnComplete(() =>
            {
                DisplayBoundInputText();
            });
    }
}
