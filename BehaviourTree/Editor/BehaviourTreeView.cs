using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using System;
using System.Linq;

public class BehaviourTreeView : GraphView
{
    public Action<NodeView> OnNodeSelected;
    public Action<NodeCommentView> OnCommentSelected;
    public new class UxmlFactory : UxmlFactory<BehaviourTreeView, GraphView.UxmlTraits> { }
    BehaviourTree tree;
    NodeTemplateSettings settings;

    public struct ScriptTemplate
    {
        public TextAsset TemplateFile;
        public string DefaultFileName;
        public string SubFolder;
    }

    public ScriptTemplate[] scriptFileAssets =
    {
         new ScriptTemplate{ TemplateFile=NodeTemplateSettings.GetOrCreateSettings().m_ActionNodeTemplate, DefaultFileName="NewActionNode.cs", SubFolder="Actions" },
            new ScriptTemplate{ TemplateFile=NodeTemplateSettings.GetOrCreateSettings().m_CompositeNodeTemplate, DefaultFileName="NewCompositeNode.cs", SubFolder="Composites" },
            new ScriptTemplate{ TemplateFile=NodeTemplateSettings.GetOrCreateSettings().m_DecoratorNodeTemplate, DefaultFileName="NewDecoratorNode.cs", SubFolder="Decorators" },
    };



    public BehaviourTreeView()
    {
        settings = NodeTemplateSettings.GetOrCreateSettings();

        Insert(0, new GridBackground());

        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/BehaviourTree/Data/BehaviourTreeEditor.uss");
        styleSheets.Add(styleSheet);

        Undo.undoRedoPerformed += OnUndoRedo;
    }

    private void OnUndoRedo()
    {
        PopulateView(tree);
        AssetDatabase.SaveAssets();
    }

    NodeView FindNodeView(Node node)
    {
        return GetNodeByGuid(node.guid) as NodeView;
    }

    internal void PopulateView(BehaviourTree t)
    {
        tree = t;

        graphViewChanged -= OnGraphViewChanged;
        DeleteElements(graphElements);
        graphViewChanged += OnGraphViewChanged;

        if (tree.m_RootNode == null)
        {
            tree.m_RootNode = tree.CreateNode(typeof(RootNode)) as RootNode;
            EditorUtility.SetDirty(tree);
            AssetDatabase.SaveAssets();
        }

        //CommentViewの作成
        tree.comments.ForEach(c => CreateCommentView(c));

        //NodeViewの作成
        tree.nodes.ForEach(n => CreateNodeView(n));

        //Edgeの作成
        this.tree.nodes.ForEach(n =>
        {
            var children = BehaviourTree.GetChildren(n);
            children.ForEach(c =>
            {
                NodeView parentView = FindNodeView(n);
                NodeView childView = FindNodeView(c);

                Edge edge = parentView.m_Output.ConnectTo(childView.m_Input);
                AddElement(edge);
            });
        });
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        return ports.ToList().Where(endPort =>
        endPort.direction != startPort.direction &&
        endPort.node != startPort.node).ToList();
    }

    private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
    {
        if (graphViewChange.elementsToRemove != null)
        {
            graphViewChange.elementsToRemove.ForEach(elem =>
            {
                if (elem is NodeView nodeView)
                {
                    tree.DeleteNode(nodeView.m_Node);
                }

                if (elem is NodeCommentView commentView)
                {
                    tree.DeleteComment(commentView.m_Comment);
                }

                if (elem is Edge edge)
                {
                    NodeView parentView = edge.output.node as NodeView;
                    NodeView childView = edge.input.node as NodeView;
                    tree.RemoveChild(parentView.m_Node, childView.m_Node);
                }
            });
        }

        if (graphViewChange.edgesToCreate != null)
        {
            graphViewChange.edgesToCreate.ForEach(edge =>
            {
                NodeView parentView = edge.output.node as NodeView;
                NodeView childView = edge.input.node as NodeView;
                tree.AddChild(parentView.m_Node, childView.m_Node);
            });
        }

        if (graphViewChange.movedElements != null)
        {
            nodes.ForEach((n) =>
            {
                NodeView nodeView = n as NodeView;
                if (nodeView != null)
                    nodeView.SortChildren();
            });
        }

        return graphViewChange;
    }

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        //base.BuildContextualMenu(evt);


        //新しいスクリプトを作成する
        evt.menu.AppendAction($"Create Script /New Action Node", (a) => CreateNewScript(scriptFileAssets[0]));
        evt.menu.AppendAction($"Create Script /New Composite Node", (a) => CreateNewScript(scriptFileAssets[1]));
        evt.menu.AppendAction($"Create Script /New Decorator Node", (a) => CreateNewScript(scriptFileAssets[2]));
        evt.menu.AppendSeparator();

        //ツリーウィンドウ内で右クリックを押した際にノードを作成するメニューを表示する
        Vector2 nodePosition = this.ChangeCoordinatesTo(contentViewContainer, evt.localMousePosition);
        {
            var types = TypeCache.GetTypesDerivedFrom<ActionNode>();
            foreach (var type in types)
            {
                evt.menu.AppendAction($"[Action]/{type.Name}", (a) => CreateNode(type, nodePosition));
            }
        }
        {
            var types = TypeCache.GetTypesDerivedFrom<CompositeNode>();
            foreach (var type in types)
            {
                evt.menu.AppendAction($"[Composite]/{type.Name}", (a) => CreateNode(type, nodePosition));
            }
        }
        {
            var types = TypeCache.GetTypesDerivedFrom<DecoratorNode>();
            foreach (var type in types)
            {
                evt.menu.AppendAction($"[Decorator]/{type.Name}", (a) => CreateNode(type, nodePosition));
            }
        }

        evt.menu.AppendSeparator();
        {
            evt.menu.AppendAction($"CreateNodeComment", (a) => CreateComment(nodePosition));
        }


    }

    void SelectFolder(string path)
    {
        if (path[path.Length - 1] == '/')
            path = path.Substring(0, path.Length - 1);

        UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(UnityEngine.Object));

        Selection.activeObject = obj;

        EditorGUIUtility.PingObject(obj);
    }

    void CreateNewScript(ScriptTemplate template)
    {
        SelectFolder($"{settings.m_NewNodeBasePath}/{template.SubFolder}");
        var templatePath = AssetDatabase.GetAssetPath(template.TemplateFile);
        ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templatePath, template.DefaultFileName);
    }

    void CreateNode(System.Type type, Vector2 position)
    {
        Node node = tree.CreateNode(type);
        node.position = position;
        CreateNodeView(node);
    }

    void CreateComment(Vector2 position)
    {
        NodeComment comment = tree.CreateComment();
        comment.position = position;
        CreateCommentView(comment);
    }

    void CreateCommentView(NodeComment comment)
    {
        NodeCommentView commentView = new(comment);
        commentView.OnCommentSelected = OnCommentSelected;
        AddElement(commentView);
    }

    void CreateNodeView(Node node)
    {
        NodeView nodeView = new(node);
        nodeView.OnNodeSelected = OnNodeSelected;
        AddElement(nodeView);
    }

    public void UpdateNodeStates()
    {
        nodes.ForEach(n =>
        {
            NodeView nodeView = n as NodeView;
            if (nodeView != null)
                nodeView.UpdateState();
        });
    }
}
