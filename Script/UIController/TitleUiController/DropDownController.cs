using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using KD.Tweening;
using TMPro;
using System.Linq;

public class DropDownController : Selectable, IPointerClickHandler, IEventSystemHandler, ISubmitHandler
{
    private List<DropDownChild> m_DropDownList;

    [SerializeField]
    private TextMeshProUGUI m_Text;

    [SerializeField, Min(0.1f)]
    private float m_ContentsHeight = 50f;

    [SerializeField]
    private int m_FirstSelectContentIndex = 0;

    [SerializeField]
    private RectTransform m_BackGround;


    /// <summary>
    /// コールバック用のイベントを設定する構造体
    /// インスペクターの視認性を挙げるための設計
    /// </summary>
    [System.Serializable]
    public struct EventData
    {
        public UnityEvent OnClick;

        public UnityEvent OnPointerClick;

        public UnityEvent OnSelect;

        public UnityEvent OnDeselect;
    }

    [SerializeField]
    private EventData m_Event;

    private bool m_IsOpen;
    private const float DURATION = 0.5f;
    private Vector2 m_BackGroundSizeDelta;

    [ReadOnly]
    public GameObject m_ContentPrefab;

    protected override void Awake()
    {
        m_IsOpen = false;
        m_BackGround.sizeDelta = Vector2.zero;

        m_Text = GetComponentInChildren<TextMeshProUGUI>();

        GetDropDownList();

        foreach (DropDownChild child in m_DropDownList)
        {
            child.Initialize(this);
        }

        m_BackGroundSizeDelta = new Vector2(0, m_DropDownList.Count * m_ContentsHeight);
    }

    public void SetDropDown(int index)
    {
        m_Text.text = m_DropDownList[index].GetText().text;
        m_DropDownList[index].Submit();
    }

    /// <summary>
    /// ドロップダウン内にあるDropDownChildをすべて取得する
    /// </summary>
    private void GetDropDownList()
    {
        m_DropDownList = new();
        m_DropDownList = GetComponentsInChildren<DropDownChild>().ToList();
    }

    void OnValidate()
    {
        if (m_DropDownList == null)
            GetDropDownList();

        foreach (DropDownChild child in m_DropDownList)
        {
            child.SetHeight(m_ContentsHeight);
        }
    }

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);

        OnCallBack(m_Event.OnSelect);
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);
        OnCallBack(m_Event.OnDeselect);
        Debugger.Log("Deselect");

        if (m_IsOpen)
        {
            StartCoroutine(WaitOneFrame()); 
        }
    }

    private IEnumerator WaitOneFrame()
    {
        yield return null;

        //子を選択している場合は処理しない
        foreach (DropDownChild child in m_DropDownList)
        {
            //Debugger.Log($"Select {EventSystem.current.currentSelectedGameObject.name}  Child {child.gameObject.name}");
            if (child.IsCurrentSelect())
            {
                yield break;
            }
        }

        DropDownClose();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnCallBack(m_Event.OnPointerClick);

        SelectOpenOrClose();
    }

    public void OnSubmit(BaseEventData eventData)
    {
        OnCallBack(m_Event.OnClick);

        SelectOpenOrClose();
    }

    public void OnSubmit(DropDownChild child)
    {
        m_Text.text = child.GetText().text;
        DropDownClose();
    }

    public void DropDownOpen()
    {
        m_IsOpen = true;
        m_BackGround.TweenSizeDelta(m_BackGroundSizeDelta, DURATION).SetEase(Ease.OutCirc);
        EventSystem.current.SetSelectedGameObject(m_DropDownList[0].gameObject);
        Debugger.Log("Open");
    }

    public void DropDownClose()
    {
        m_IsOpen = false;
        m_BackGround.TweenSizeDelta(Vector2.zero, DURATION).SetEase(Ease.OutCirc);
        EventSystem.current.SetSelectedGameObject(gameObject);
        Debugger.Log("Close");
    }

    /// <summary>
    /// 既に開かれている場合は閉じ、閉じている場合は開く
    /// </summary>
    private void SelectOpenOrClose()
    {
        if (m_IsOpen)
        {
            DropDownClose();
        }
        else
        {
            DropDownOpen();
        }
    }

    /// <summary>
    /// コールバック処理をまとめたもの。選択時や押されたときのイベントをここで呼ぶ
    /// </summary>
    /// <param name="callback"></param>
    private void OnCallBack(UnityEvent callback)
    {
        if (callback != null)
            callback.Invoke();
    }


    public void CreateContent()
    {
        GameObject content = Instantiate(m_ContentPrefab, m_BackGround.transform);
        DropDownChild child = content.GetComponent<DropDownChild>();
        m_DropDownList.Add(child);
    }



}
