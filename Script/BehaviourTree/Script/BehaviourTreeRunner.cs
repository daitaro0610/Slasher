using UnityEngine;

public class BehaviourTreeRunner : MonoBehaviour
{
    public BehaviourTree m_Tree;
    private EnemyContext m_Context;

    private bool m_IsActive = true;

    /// <summary>
    /// BehaviourTreeを動作させるかさせないか
    /// </summary>
    /// <param name="isbool"></param>
    public void SetActive(bool isbool) => m_IsActive = isbool;

    private void Awake()
    {
        //初期化
        m_IsActive = true;
    }

    void Start()
    {
        m_Context = CreateBehaviourTreeContext();
        m_Tree = m_Tree.Clone();
        //自身の情報をBehaviourTreeに送る
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
