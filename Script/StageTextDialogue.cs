using UnityEngine;
using TMPro;
using KD;

//Text�����X�ɕ\�����Ă����N���X
public class StageTextDialogue : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI m_Text;

    [SerializeField]
    private string m_StageText; //���̕�����\������̂�

    [SerializeField]
    private float m_ReadSpeed = 0.1f;�@//�\�����Ă����X�s�[�h

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
