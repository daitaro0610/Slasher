using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu()]
public class BehaviourTree : ScriptableObject
{
    public Node m_RootNode;
    public Node.State m_TreeState = Node.State.Running;
    public List<Node> nodes = new();
    public List<NodeComment> comments = new();
    public Blackboard blackboard = new();

    /// <summary>
    /// BehaviourTreeRunnerクラスのUpdateから呼ばれる
    /// RootNodeが実行状態であればRootNodeのUpdateを呼び続ける
    /// </summary>
    /// <returns></returns>
    public Node.State Update()
    {
        if (m_RootNode.state == Node.State.Running)
        {
            m_TreeState = m_RootNode.Update();
        }
        return m_TreeState;
    }

#if UNITY_EDITOR

    /// <summary>
    /// ビヘイビアツリーエディタ上でノードを作成する際に呼ばれる処理
    /// </summary>
    /// <param name="type">選択したNodeの種類</param>
    /// <returns></returns>
    public Node CreateNode(System.Type type)
    {
        Node node = ScriptableObject.CreateInstance(type) as Node;
        node.name = type.Name;
        node.guid = GUID.Generate().ToString();

        Undo.RecordObject(this, "Behaviour Tree(CreateNode)");
        nodes.Add(node);

        if (!Application.isPlaying)
        {
            AssetDatabase.AddObjectToAsset(node, this);
        }
        Undo.RegisterCreatedObjectUndo(node, "Behaviour Tree(CreateNode)");
        AssetDatabase.SaveAssets();
        return node;
    }

    /// <summary>
    /// ビヘイビアツリーエディタ上でNodeを削除する際に呼ばれる処理
    /// </summary>
    /// <param name="node"></param>
    public void DeleteNode(Node node)
    {
        Undo.RecordObject(this, "Behaviour Tree(DeleteNode)");

        nodes.Remove(node);


        //AssetDatabase.RemoveObjectFromAsset(node);
        Undo.DestroyObjectImmediate(node);
        AssetDatabase.SaveAssets();
    }

    /// <summary>
    /// ビヘイビアツリーエディタ上でコメントエリアを作成する際に呼ばれる処理
    /// </summary>
    /// <returns></returns>
    public NodeComment CreateComment()
    {
        NodeComment comment = ScriptableObject.CreateInstance(typeof(NodeComment)) as NodeComment;
        comment.name = typeof(NodeComment).Name;
        comment.guid = GUID.Generate().ToString();

        Undo.RecordObject(this, "Behaviour Tree(CreateComment)");
        comments.Add(comment);

        if (!Application.isPlaying)
        {
            AssetDatabase.AddObjectToAsset(comment, this);
        }
        Undo.RegisterCreatedObjectUndo(comment, "Behaviour Tree(CreateComment)");
        AssetDatabase.SaveAssets();
        return comment;
    }

    /// <summary>
    /// ビヘイビアツリーエディタ上でコメントエリアを削除する際に呼ばれる処理
    /// </summary>
    /// <param name="comment"></param>
    public void DeleteComment(NodeComment comment)
    {
        Undo.RecordObject(this, "Behaviour Tree(DeleteComment)");

        comments.Remove(comment);

        Undo.DestroyObjectImmediate(comment);
        AssetDatabase.SaveAssets();
    }

    /// <summary>
    /// ノード同士を繋げたときに呼ばれる処理
    /// </summary>
    /// <param name="parent">繋げ元となるNode</param>
    /// <param name="child">繋げたNode</param>
    public void AddChild(Node parent, Node child)
    {
        DecoratorNode decorator = parent as DecoratorNode;
        if (decorator)
        {
            Undo.RecordObject(decorator, "Behaviour Tree(AddChild)");
            decorator.m_Child = child;
            EditorUtility.SetDirty(decorator);
        }

        RootNode rootNode = parent as RootNode;
        if (rootNode)
        {
            Undo.RecordObject(rootNode, "Behaviour Tree(AddChild)");
            rootNode.m_Child = child;
            EditorUtility.SetDirty(rootNode);
        }

        CompositeNode composite = parent as CompositeNode;
        if (composite)
        {
            Undo.RecordObject(composite, "Behaviour Tree(AddChild)");
            composite.m_Children.Add(child);
            EditorUtility.SetDirty(composite);
        }
    }

    /// <summary>
    /// 繋げたNode同士の関係を切る時に呼ばれる処理
    /// </summary>
    /// <param name="parent">親のNode</param>
    /// <param name="child">子のNode</param>
    public void RemoveChild(Node parent, Node child)
    {
        DecoratorNode decorator = parent as DecoratorNode;
        if (decorator)
        {
            Undo.RecordObject(decorator, "Behaviour Tree(RemoveChild)");
            decorator.m_Child = null;
            EditorUtility.SetDirty(decorator);
        }

        RootNode rootNode = parent as RootNode;
        if (rootNode)
        {
            Undo.RecordObject(rootNode, "Behaviour Tree(RemoveChild)");
            rootNode.m_Child = null;
            EditorUtility.SetDirty(rootNode);
        }

        CompositeNode composite = parent as CompositeNode;
        if (composite)
        {
            Undo.RecordObject(composite, "Behaviour Tree(RemoveChild)");
            composite.m_Children.Remove(child);
            EditorUtility.SetDirty(composite);
        }
    }
#endif

    /// <summary>
    /// 子のNode達を取得する
    /// </summary>
    /// <param name="parent"></param>
    /// <returns></returns>
    public static List<Node> GetChildren(Node parent)
    {
        List<Node> children = new();

        if (parent is DecoratorNode decorator && decorator.m_Child != null)
        {
            children.Add(decorator.m_Child);
        }

        if (parent is RootNode rootNode && rootNode.m_Child != null)
        {
            children.Add(rootNode.m_Child);
        }

        if (parent is CompositeNode composite)
        {
            return composite.m_Children;
        }

        return children;
    }

    public static void Traverse(Node node, System.Action<Node> visiter)
    {
        if (node)
        {
            visiter.Invoke(node);
            var children = GetChildren(node);
            children.ForEach((n) => Traverse(n, visiter));
        }
    }

    /// <summary>
    /// 実行時にビヘイビアツリーを複製し、データが衝突することを防ぐ処理
    /// </summary>
    /// <returns></returns>
    public BehaviourTree Clone()
    {
        BehaviourTree tree = Instantiate(this);
        tree.m_RootNode = tree.m_RootNode.Clone();
        tree.nodes = new List<Node>();
        Traverse(tree.m_RootNode, (n) =>
        {
            tree.nodes.Add(n);
        });

        return tree;
    }

    /// <summary>
    /// 敵の情報を埋め込む処理
    /// </summary>
    /// <param name="context"></param>
    public void Bind(EnemyContext context)
    {
        Traverse(m_RootNode, node =>
         {
             node.blackboard = blackboard;
             node.context = context;
         });
    }
}
