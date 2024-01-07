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
    /// BehaviourTreeRunner�N���X��Update����Ă΂��
    /// RootNode�����s��Ԃł����RootNode��Update���Ăё�����
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
    /// �r�w�C�r�A�c���[�G�f�B�^��Ńm�[�h���쐬����ۂɌĂ΂�鏈��
    /// </summary>
    /// <param name="type">�I������Node�̎��</param>
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
    /// �r�w�C�r�A�c���[�G�f�B�^���Node���폜����ۂɌĂ΂�鏈��
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
    /// �r�w�C�r�A�c���[�G�f�B�^��ŃR�����g�G���A���쐬����ۂɌĂ΂�鏈��
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
    /// �r�w�C�r�A�c���[�G�f�B�^��ŃR�����g�G���A���폜����ۂɌĂ΂�鏈��
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
    /// �m�[�h���m���q�����Ƃ��ɌĂ΂�鏈��
    /// </summary>
    /// <param name="parent">�q�����ƂȂ�Node</param>
    /// <param name="child">�q����Node</param>
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
    /// �q����Node���m�̊֌W��؂鎞�ɌĂ΂�鏈��
    /// </summary>
    /// <param name="parent">�e��Node</param>
    /// <param name="child">�q��Node</param>
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
    /// �q��Node�B���擾����
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
    /// ���s���Ƀr�w�C�r�A�c���[�𕡐����A�f�[�^���Փ˂��邱�Ƃ�h������
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
    /// �G�̏��𖄂ߍ��ޏ���
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
