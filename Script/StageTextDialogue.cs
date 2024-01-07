using UnityEngine;
using TMPro;
using KD;

//Textを徐々に表示していくクラス
public class StageTextDialogue : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI m_Text;

    [SerializeField]
    private string m_StageText; //何の文字を表示するのか

    [SerializeField]
    private float m_ReadSpeed = 0.1f;　//表示していくスピード

    private bool m_IsCompleteText;
    public bool IsCompleteText => m_IsCompleteText;

    private void Awake()
    {
        m_IsCompleteText = false;
    }

    private void Start()
    {
        m_Text.text = "";
        TextMeshProManager.instance.LoadText(m_Text, m_StageText,m_ReadSpeed)
            .OnCompleted(()=>m_IsCompleteText = true);
    }
}
