using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using KD.Tweening;

/// <summary>
/// 自身含め、子のMaskableGraphicを継承したクラスの表示非表示を制御するクラス
/// </summary>
public class GraphicVisibilityController : MonoBehaviour
{
    [SerializeField, Header("同じLayerのみ表示非表示行う")]
    private LayerMask m_CurrentVisibilityLayer;

    [SerializeField]
    private float m_FadeDuration = 0.5f;

    [SerializeField, Header("初回の表示非表示")]
    private bool m_IsAwakeActive = true;
    [SerializeField, Header("透明状態から始める場合はTrue")]
    private bool m_IsAwakeColorClear = false;

    private bool m_IsActive = false;
    public bool IsActive => m_IsActive;

    private MaskableGraphic[] m_Graphics;　//ImageやTextなどのGraphic関連
    private Color[] m_DefaultAlphaValue;
    [SerializeField,Header("TiimeScaleが0でもFadeするかどうか")]
    private bool m_IsUnscaled = false;

    [Header("表示した際にオブジェクトを選択するかどうか")]
    public bool IsSelectObjectOnActivate = false;



    [HideInInspector]
    public GameObject SelectObject;

    private void Awake()
    {
        m_Graphics = GetComponentsInChildren<MaskableGraphic>();
        m_Graphics = m_Graphics.Where(x => CompareLayer(m_CurrentVisibilityLayer, x.gameObject.layer)).ToArray();
        Debugger.Log($"要素数{m_Graphics.Length}");

        GetDafaultAlphaValue();
        SetActive(m_IsAwakeActive);

        if (m_IsAwakeColorClear)
            SetColorClear();
    }

    /// <summary>
    /// アルファ値のデフォルトの値を取得する
    /// </summary>
    private void GetDafaultAlphaValue()
    {
        m_DefaultAlphaValue = new Color[m_Graphics.Length];

        for (int i = 0; i < m_Graphics.Length; i++)
        {
            m_DefaultAlphaValue[i] = m_Graphics[i].color;
        }
    }

    /// <summary>
    /// すべての色を透明にする
    /// </summary>
    private void SetColorClear()
    {
        for (int i = 0; i < m_Graphics.Length; i++)
        {
            m_Graphics[i].color = Color.clear;
        }
    }

    /// <summary>
    /// そのまま一瞬で消す
    /// </summary>
    /// <param name="isbool"></param>
    public void SetActive(bool isbool)
    {
        Debugger.Log("表示切替 " + gameObject.name +" : " + isbool);
        m_IsActive = isbool;

        for (int i = 0; i < m_Graphics.Length; i++)
        {
            m_Graphics[i].enabled = isbool;
        }

        if (isbool)
            SetSelectObject();
    }

    /// <summary>
    /// Fadeしながら消える場合の処理
    /// </summary>
    /// <param name="isbool"></param>
    public void SetFadeActive(bool isbool)
    {
        //Trueの場合は表示させるのでフェードインする
        if (isbool)
        {
            FadeOut();
            SetSelectObject();
        }
        else//Falseの場合は非表示にする
        {
            FadeIn();
        }
    }

    public void SetDuration(float changeDuration)
    {
        m_FadeDuration = changeDuration;
    }

    private void FadeIn()
    {
        m_IsActive = false;
        for (int i = 0; i < m_Graphics.Length; i++)
        {
           var tween = m_Graphics[i].TweenColor(Color.clear, m_FadeDuration);

            if (m_IsUnscaled)
                tween.SetUnscaled();
        }
    }

    private void FadeOut()
    {
        m_IsActive = true;
        for (int i = 0; i < m_Graphics.Length; i++)
        {
            m_Graphics[i].enabled = true;
          var tween =  m_Graphics[i].TweenColor(m_DefaultAlphaValue[i], m_FadeDuration);

            if (m_IsUnscaled)
                tween.SetUnscaled();
        }
    }

    private void SetSelectObject()
    {
        if (IsSelectObjectOnActivate && SelectObject != null)
        {
            EventSystem.current.SetSelectedGameObject(SelectObject);
        }
    }

    // LayerMaskに対象のLayerが含まれているかチェックする
    private bool CompareLayer(LayerMask layerMask, int layer)
    {
        return ((1 << layer) & layerMask) != 0;
    }
}
