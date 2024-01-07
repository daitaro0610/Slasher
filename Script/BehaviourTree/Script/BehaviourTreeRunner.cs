using UnityEngine;

public class BehaviourTreeRunner : MonoBehaviour
{
    public BehaviourTree m_Tree;
    private EnemyContext m_Context;

    private bool m_IsActive = true;

    /// <summary>
    /// BehaviourTree�𓮍삳���邩�����Ȃ���
    /// </summary>
    /// <param name="isbool"></param>
    public void SetActive(bool isbool) => m_IsActive = isbool;

    private void Awake()
    {
        //������
        m_IsActive = true;
    }

    void Start()
    {
        m_Context = CreateBehaviourTreeContext();
        m_Tree = m_Tree.Clone();
        //���g�̏���BehaviourTree�ɑ���
        m_Tree.Bind(m_Context);
    }

    void Update()
    {
        if (m_IsActive)
            m_Tree.Update();
    }

    EnemyContext CreateBehaviourTreeContext()
    {
        return EnemyContext.CreateFromGameObject(gameObject);
    }
}
