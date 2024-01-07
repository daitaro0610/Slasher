using UnityEngine;

public class BehaviourTreeRunner : MonoBehaviour
{
    public BehaviourTree m_Tree;
    private EnemyContext m_Context;

    private bool m_IsActive = true;

    /// <summary>
    /// BehaviourTree‚ğ“®ì‚³‚¹‚é‚©‚³‚¹‚È‚¢‚©
    /// </summary>
    /// <param name="isbool"></param>
    public void SetActive(bool isbool) => m_IsActive = isbool;

    private void Awake()
    {
        //‰Šú‰»
        m_IsActive = true;
    }

    void Start()
    {
        m_Context = CreateBehaviourTreeContext();
        m_Tree = m_Tree.Clone();
        //©g‚Ìî•ñ‚ğBehaviourTree‚É‘—‚é
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
