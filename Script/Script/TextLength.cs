using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextLength : MonoBehaviour
{
    [SerializeField]
    private Text m_OperationText;

    [SerializeField]
    private RectTransform m_ImgRt;

    private float m_ImgSize;

    private float TEXT_OFFSET = 100f;

    // Start is called before the first frame update
    void Start()
    {
        TEXT_OFFSET = m_OperationText.fontSize;
        this.m_ImgSize = this.m_ImgRt.sizeDelta.x;
        m_ImgRt.sizeDelta = new Vector2(this.m_ImgSize + (m_OperationText.text.Length * this.TEXT_OFFSET), this.m_ImgRt.sizeDelta.y);
    }

    private void Update()
    {
        //ílÇ™ìØÇ∂Ç»ÇÁìØÇ∂èàóùÇÇµÇ»Ç¢
        if (m_ImgRt.sizeDelta == new Vector2(this.m_ImgSize + (m_OperationText.text.Length * this.TEXT_OFFSET), this.m_ImgRt.sizeDelta.y)) return;
        m_ImgRt.sizeDelta = new Vector2(this.m_ImgSize + (m_OperationText.text.Length * this.TEXT_OFFSET), this.m_ImgRt.sizeDelta.y);
    }

    public void SizeChange()
    {
        //Debug.Log(m_AnswerText.text.Length);
        m_ImgRt.sizeDelta = new Vector2(this.m_ImgSize + (m_OperationText.text.Length * this.TEXT_OFFSET), this.m_ImgRt.sizeDelta.y);
    }
}
