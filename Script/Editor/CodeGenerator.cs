using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System.Reflection;

public class CodeGenerator : EditorWindow
{
    private string FileName = "";


    //�X�N���v�g�̃T���v��---------------------

    private string m_DefaultCodeTemplete =
        "//MadeVariablesCreator\n" +
            "#USING#\n" +
            "using UnityEngine;\n" +
        "public class #CLASS_NAME#\n" +
            "{";

    private string m_Cord = "public #STATIC# #TYPE# #NAME# = #VALUE#;";

    //�܂����g�p
    //private string m_VariablesCord = "public";
    private string m_StaticCord = "static ";

    private string m_ReadOnlyCord = "readonly ";

    private string m_TypeNameCord = "string";

    private readonly string m_EndScriptCord = "}";

    private readonly string m_UsingTemplateCord = @"
using System.Collections;
using System.Collections.Generic;";

    private string m_MonoBehaviourCord = " : MonoBehaviour";

    private string m_MonoSampleCord = @"
    void Start()
    {

    }

    void Update()
    {

    }
";

    //-----------------------

    private string m_FilePath = "Assets/Script";

    //�X�N���[���ʒu
    private Vector2 _scrollPositionLeft = Vector2.zero;

    private Vector2 _scrollPositionRight = Vector2.zero;

    //�E�B���h�E�̕��@�Жʂ̃T�C�Y
    private float Width = 500;

    UnityEngine.Object target;

    private enum State
    {
        [InspectorName("�X�N���v�g�쐬���[�h")]
        Create,
        [InspectorName("�X�N���v�g�ҏW���[�h")]
        OverWrite
    }
    private State m_State;

    private enum Type
    {
        String,
        Int,
        Float,
        Animator,
        [InspectorName("")]
        None    //�m��Ȃ��ϐ���������
    }
    private Type m_Type;

    private Type[] m_Types;


    //�ϐ���
    public string[] VariableNames;
    //�t�B�[���h�ɋL�����ꂽ�l��ۑ�����̂Ɏg�p����
    public string[] stringField;
    public int[] intField;
    public float[] floatField;

    private int[] m_MemoryInt;
    private float[] m_MemoryFloat;


    //�A�^�b�`�����X�N���v�g�̃p�X�̕ۑ�
    private string scriptPath = "";

    //�A�^�b�`���ꂽ�N���X�̖��O���L�����āADrawParam����x�����ĂԂ悤�ɂ��Ă���
    private string memoryClassName = "";


    //override����X�N���v�g�̏���ۑ�����
    private List<string> scriptCodes = new List<string>();
    private List<string> variableNameList = new List<string>();
    private List<string> variableCodeList = new List<string>();
    private List<string> newNameList = new List<string>();
    private List<bool> is_Static = new List<bool>();
    private List<bool> is_ReadOnly = new List<bool>();
    private List<Type> m_CurrentTypes = new List<Type>();
    private bool isThisMade = false;

    //�ŏ��Ƀp�����[�^�[��ǂݍ��񂾍s
    private int ParameterReadFirstLine = 0;
    //�ϐ��̌�(Create�p)
    private int size = 0;
    //�ϐ��̌�(OverWrite�p)
    private int ParameterCount = 0;

    //MonoBehaviour�ɂ��邩�ǂ����̔���
    private bool is_MonoBehaviour;

    //�ǂݎ���p���ǂ����̔���
    private bool is_ReadOnlyCreate;

    /// <summary>
    /// �R�[�h���L������e���v���[�g
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    private string CordSetting(string name, string value)
    {
        m_Cord = "\tpublic #STATIC##READONLY##TYPE# #NAME# = #VALUE#;";
        m_Cord = m_Cord.Replace("#STATIC#", m_StaticCord);
        m_Cord = m_Cord.Replace("#READONLY#", m_ReadOnlyCord);
        m_Cord = m_Cord.Replace("#TYPE#", m_TypeNameCord);
        m_Cord = m_Cord.Replace("#NAME#", name);
        m_Cord = m_Cord.Replace("#VALUE#", value);
        return m_Cord;
    }

    /// <summary>
    /// Type�ɍ��킹�ĕϐ��̌^�̕������ύX����
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private string TypeSetting(Type type)
    {
        return type switch
        {
            Type.String => "string",
            Type.Int => "int",
            Type.Float => "float",
            Type.Animator => "int",
            _ => "None",
        };
    }

    /// <summary>
    /// �ϐ��̌^�ɂ���ĕ\��������@��ύX����
    /// </summary>
    /// <returns></returns>
    private string TypeStringVariablesSetting(Type type, string variablesName)
    {
        return type switch
        {
            Type.String => "\"" + variablesName + "\"",
            Type.Int => variablesName,
            Type.Float => variablesName + "f",
            Type.Animator => "Animator.StringToHash(\"" + variablesName + "\")",
            _ => "None",
        };
    }


    /// <summary>
    /// �E�B���h�E��\������
    /// </summary>
    [MenuItem("Window/VariablesCreator")]
    private static void CreateWindow()
    {
        GetWindow<CodeGenerator>("CodeGenerator");
    }
    /// <summary>
    /// �E�B���h�E�̕`���A�X�e�[�g�ɉ����ĕ\����ύX����
    /// </summary>
    private void OnGUI()
    {
        minSize = new Vector2(700, 200);
        maxSize = new Vector2(900, 1000);

        EditorGUILayout.LabelField("Mode");
        m_State = (State)EditorGUILayout.EnumPopup(m_State);

        EditorGUILayout.Space(10);

        //��ڂ̃t�B�[���h
        EditorGUILayout.BeginHorizontal(GUI.skin.box);
        {
            //State�̏�Ԃɉ����ĕ\����ύX����
            if (m_State == State.Create)
                CreateWindowMenu();
            else
                OverWriteWindowMenu();
        }
        EditorGUILayout.EndHorizontal();

    }
    /// <summary>
    /// State��Create�̎��ɕ\������GUI
    /// </summary>
    private void CreateWindowMenu()
    {
        //��ڂ̃t�B�[���h
        EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(Width));
        {
            EditorGUILayout.LabelField("�ϐ��쐬");

            EditorGUILayout.BeginHorizontal(GUI.skin.box);
            {
                EditorGUILayout.LabelField("Size", GUILayout.Width(100));
                size = EditorGUILayout.IntField(size, GUILayout.Width(100));
            }
            EditorGUILayout.EndHorizontal();


            //�z��̃T�C�Y��ύX
            Resize();


            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                //�`��͈͂�����Ȃ���΃X�N���[���o����悤��
                _scrollPositionLeft = EditorGUILayout.BeginScrollView(_scrollPositionLeft);

                EditorGUILayout.BeginHorizontal(GUI.skin.box);
                {
                    EditorGUILayout.LabelField("�ϐ���", GUILayout.Width(100));
                    EditorGUILayout.LabelField("�^", GUILayout.Width(100));
                    EditorGUILayout.LabelField("�l", GUILayout.Width(100));
                }
                EditorGUILayout.EndHorizontal();


                //�z��̐����\������
                for (int i = 0; i < VariableNames.Length; i++)
                {
                    EditorGUILayout.BeginHorizontal(GUI.skin.box);
                    {
                        VariableNames[i] = EditorGUILayout.TextField(VariableNames[i], GUILayout.Width(100));


                        m_Types[i] = (Type)EditorGUILayout.EnumPopup(m_Types[i], GUILayout.Width(100));

                        //�\����Type�ɉ����ĕύX����
                        switch (m_Types[i])
                        {
                            case Type.String: stringField[i] = EditorGUILayout.TextField(stringField[i], GUILayout.Width(100)); break;

                            case Type.Int:
                                intField[i] = EditorGUILayout.IntField(intField[i], GUILayout.Width(100));
                                stringField[i] = intField[i].ToString(); break;

                            case Type.Float:
                                floatField[i] = EditorGUILayout.FloatField(floatField[i], GUILayout.Width(100));
                                stringField[i] = floatField[i].ToString(); break;

                            case Type.Animator: stringField[i] = EditorGUILayout.TextField(stringField[i], GUILayout.Width(100)); break;
                        }

                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndScrollView();
            }
            EditorGUILayout.EndVertical();

            //�X�N���v�g�쐬�{�^��
            GUILayout.Space(10);
            if (GUILayout.Button("�쐬"))
                Create();
        }
        EditorGUILayout.EndVertical();


        //��ڂ̃t�B�[���h
        EditorGUILayout.BeginVertical(GUI.skin.box);
        {

            EditorGUILayout.LabelField("�ݒ�");

            m_FilePath = EditorGUILayout.TextField("FilePath", m_FilePath);

            FileName = EditorGUILayout.TextField("ScriptName", FileName);

            //Monobehaviour�ɂ��邩�ǂ����̔���
            EditorGUILayout.BeginHorizontal(GUI.skin.box, GUILayout.Width(300));
            {
                EditorGUILayout.LabelField("MonoBehaviour");

                is_MonoBehaviour = EditorGUILayout.Toggle(is_MonoBehaviour);
            }
            EditorGUILayout.EndHorizontal();

            //ReadOnly�ɂ��邩�ǂ����̔���
            EditorGUILayout.BeginHorizontal(GUI.skin.box, GUILayout.Width(300));
            {
                EditorGUILayout.LabelField("ReadOnly");

                is_ReadOnlyCreate = EditorGUILayout.Toggle(is_ReadOnlyCreate);
            }
            EditorGUILayout.EndHorizontal();

        }
        EditorGUILayout.EndVertical();
    }
    /// <summary>
    ///  State��OverWrite�̎��ɕ\������GUI
    /// </summary>
    private void OverWriteWindowMenu()
    {
        //��ڂ̃t�B�[���h
        EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(Width));
        {
            EditorGUILayout.LabelField("�ϐ��ҏW");


            if (variableNameList.Count > 0)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                {
                    //�`��͈͂�����Ȃ���΃X�N���[���o����悤��
                    _scrollPositionLeft = EditorGUILayout.BeginScrollView(_scrollPositionLeft);

                    EditorGUILayout.BeginHorizontal(GUI.skin.box);
                    {
                        EditorGUILayout.LabelField("�ϐ���", GUILayout.Width(100));
                        EditorGUILayout.LabelField("�^", GUILayout.Width(100));
                        EditorGUILayout.LabelField("�l", GUILayout.Width(100));
                        EditorGUILayout.LabelField("static", GUILayout.Width(50));
                        EditorGUILayout.LabelField("readonly", GUILayout.Width(50));
                    }
                    EditorGUILayout.EndHorizontal();

                    if (target != null)
                    {
                        Array.Resize(ref m_MemoryInt, variableNameList.Count);
                        Array.Resize(ref m_MemoryFloat, variableNameList.Count);

                        for (int i = 0; i < variableNameList.Count; i++)
                        {
                            EditorGUILayout.BeginHorizontal(GUI.skin.box);
                            {
                                variableNameList[i] = EditorGUILayout.TextField(variableNameList[i], GUILayout.Width(100));

                                //��Ԃɉ����ĕ\������
                                m_CurrentTypes[i] = (Type)EditorGUILayout.EnumPopup(m_CurrentTypes[i], GUILayout.Width(100));

                                switch (m_CurrentTypes[i])
                                {
                                    case Type.String: newNameList[i] = EditorGUILayout.TextField(newNameList[i], GUILayout.Width(100)); break;
                                    case Type.Int: newNameList[i] = EditorGUILayout.IntField(m_MemoryInt[i], GUILayout.Width(100)).ToString(); break;
                                    case Type.Float: newNameList[i] = EditorGUILayout.FloatField(m_MemoryFloat[i], GUILayout.Width(100)).ToString(); break;
                                    case Type.Animator: newNameList[i] = EditorGUILayout.TextField(newNameList[i], GUILayout.Width(100)); break;
                                }

                                is_Static[i] = EditorGUILayout.Toggle(is_Static[i], GUILayout.Width(50));
                                is_ReadOnly[i] = EditorGUILayout.Toggle(is_ReadOnly[i], GUILayout.Width(50));

                                if (GUILayout.Button("�폜"))
                                    RemoveParam(i);
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                    }

                    EditorGUILayout.EndScrollView();

                    GUILayout.Space(10);
                    if (GUILayout.Button("�ǉ�"))
                        AddParam();

                }
                EditorGUILayout.EndVertical();
            }

            GUILayout.Space(10);
            if (target != null && isThisMade)
            {
                if (GUILayout.Button("�㏑������"))
                    OverWrite();
            }
        }
        EditorGUILayout.EndVertical();

        //��ڂ̃t�B�[���h
        EditorGUILayout.BeginVertical(GUI.skin.box);
        {
            EditorGUILayout.LabelField("�p�����[�^");

            //MonoBehaviour���p�������N���X���A�^�b�`�ł���悤�ɂ���
            target = EditorGUILayout.ObjectField(
                   "Script",
                   target,
                   typeof(MonoScript), false);

            if (target != null)
            {
                scriptPath = AssetDatabase.GetAssetPath(target);

                //�A�^�b�`�����I�u�W�F�N�g���ω�������ĕ\������
                if (target.name != memoryClassName)
                {
                    DrawParam();
                    memoryClassName = target.name;
                }

                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("MonoBehaviour");
                    is_MonoBehaviour = EditorGUILayout.Toggle(is_MonoBehaviour);
                }
                EditorGUILayout.EndHorizontal();

                //�`��͈͂�����Ȃ���΃X�N���[���o����悤��
                _scrollPositionRight = EditorGUILayout.BeginScrollView(_scrollPositionRight);
                //�C���X�y�N�^�̂悤�ɕ\������
                Editor.CreateEditor(target).OnInspectorGUI();

                //�X�N���[���ӏ��I��
                EditorGUILayout.EndScrollView();
            }
        }
        EditorGUILayout.EndVertical();
    }
    /// <summary>
    /// .cs�t�@�C�����쐬����
    /// </summary>
    private void Create()
    {
        if (FileName == "")
        {
            Debug.LogError("���O��������܂���");
            return;
        }

        if (!FileName.All(IsLetter))
        {
            Debug.LogError("�A���t�@�x�b�g�ȊO���܂܂�Ă��܂�");
            return;
        }


        //��������啶���ɂ���
        var className = ToUpper(FileName, 0);
        if (CheckIfClassExists(className))
        {
            Debug.LogError($"���ɂ��̃N���X�����݂��܂��B�@�N���X��{className}");
            return;
        }
        var filePath = m_FilePath + "/" + className + ".cs";

        var folderPath = Path.GetDirectoryName(filePath);
        CreateFolder(folderPath);

        //�A�Z�b�g�̃p�X���쐬
        var assetPath = AssetDatabase.GenerateUniqueAssetPath(filePath);



        if (is_MonoBehaviour) m_StaticCord = "";
        else m_StaticCord = "static ";

        if (is_ReadOnlyCreate) m_ReadOnlyCord = "readonly ";
        else m_ReadOnlyCord = "";

        //�ϐ��̍쐬
        string[] variableNamesCode = new string[VariableNames.Length];
        for (int i = 0; i < VariableNames.Length; i++)
        {
            if (!VariableNames[i].All(IsLetter))
                Debug.LogError("�A���t�@�x�b�g�ȊO���܂܂�Ă��܂� : " + VariableNames[i]);

            m_TypeNameCord = TypeSetting(m_Types[i]);

            //�ϐ����^�ɍ��킹�ď���������
            variableNamesCode[i] = CordSetting(ToUpper(VariableNames[i], 0), TypeStringVariablesSetting(m_Types[i], stringField[i]));
        }

        //�t�@�C���ɏ�������
        StreamWriter streamWriter = new StreamWriter(assetPath);

        //�R�[�h�e���v���[�g��u��
        var codeHead = m_DefaultCodeTemplete.Replace("#CLASS_NAME#", className);

        if (is_MonoBehaviour)
            codeHead = codeHead.Replace("#USING#", m_UsingTemplateCord);
        else codeHead = codeHead.Replace("#USING#", "");

        streamWriter.WriteLine(codeHead);

        for (int i = 0; i < variableNamesCode.Length; i++)
        {
            streamWriter.WriteLine(variableNamesCode[i]);
        }

        //MonoBehaviour�Ȃ�Start��Update�̃T���v�����\������
        if (is_MonoBehaviour)
            streamWriter.WriteLine(m_MonoSampleCord);

        streamWriter.WriteLine(m_EndScriptCord);
        streamWriter.Close();

        AssetDatabase.Refresh();

    }
    private bool CheckIfClassExists(string className)
    {
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (Assembly assembly in assemblies)
        {
            System.Type type = assembly.GetType(className);
            if (type != null)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// �t�@�C���̓��e���X�V����
    /// </summary>
    private void OverWrite()
    {
        for (int i = 0; i < newNameList.Count; i++)
        {
            if (newNameList[i] == "")
            {
                Debug.LogError("�����񂪌�����܂���");
                return;
            }
        }

        for (int i = 0; i < variableNameList.Count; i++)
        {
            if (!variableNameList[i].All(IsLetter))
            {
                Debug.LogWarning("�ϐ����ɃA���t�@�x�b�g�ȊO���܂܂�Ă��܂� : " + variableNameList[i]);
            }
        }

        //�t�@�C���ɏ�������
        StreamWriter streamWriter = new StreamWriter(scriptPath);

        //�R�[�h�e���v���[�g��u��
        var codeHead = m_DefaultCodeTemplete.Replace("#CLASS_NAME#", ToUpper(target.name, 0));

        if (is_MonoBehaviour)
        {
            codeHead = codeHead.Replace("#USING#", m_UsingTemplateCord);
        }
        else codeHead = codeHead.Replace("#USING#", "");

        streamWriter.WriteLine(codeHead);

        //��������ϐ�
        for (int i = 0; i < variableCodeList.Count; i++)
        {
            if (is_Static[i]) m_StaticCord = "static ";
            else m_StaticCord = "";

            if (is_ReadOnly[i]) m_ReadOnlyCord = "readonly ";
            else m_ReadOnlyCord = "";

            m_TypeNameCord = TypeSetting(m_CurrentTypes[i]);
            newNameList[i] = TypeStringVariablesSetting(m_CurrentTypes[i], newNameList[i]);
            variableCodeList[i] = CordSetting(ToUpper(variableNameList[i], 0), newNameList[i]);
            Debugger.Log(variableCodeList[i]);
            streamWriter.WriteLine(variableCodeList[i]);
        }

        //Start�Ƃ�Update�Ƃ�
        if (is_MonoBehaviour)
            streamWriter.WriteLine(m_MonoSampleCord);

        //�Ō��}����͂���
        streamWriter.WriteLine(m_EndScriptCord);
        streamWriter.Close();

        ParameterCount = 0;
        DrawParam();

        AssetDatabase.Refresh();
    }
    /// <summary>
    /// �X�N���v�g�t�@�C�����A�^�b�`�����Ƃ��ɃE�B���h�E�ɕϐ��̃p�����[�^��\������l�������ő������
    /// </summary>
    private void DrawParam()
    {
        scriptCodes.Clear();
        variableNameList.Clear();
        variableCodeList.Clear();
        newNameList.Clear();
        m_CurrentTypes.Clear();
        is_Static.Clear();
        is_ReadOnly.Clear();
        ParameterReadFirstLine = 0;

        //�t�@�C������s���\������
        foreach (string line in File.ReadLines(scriptPath))
        {
            scriptCodes.Add(line);
        }

        if (scriptCodes.Count == 0) Debug.LogError("�X�N���v�g�ɃR�[�h��������܂���");

        //�����𕪊�����
        for (int i = 0; i < scriptCodes.Count; i++)
        {
            if (scriptCodes[i].Contains("//MadeVariablesCreator"))
                isThisMade = true;
            //���ꂪ�ϐ����ǂ����̔���
            if (scriptCodes[i].Contains("public ") && scriptCodes[i].Contains("="))
            {
                if (!isThisMade) continue;

                if (scriptCodes[i].Contains("StringToHash")) m_CurrentTypes.Add(Type.Animator);
                else if (scriptCodes[i].Contains("string")) m_CurrentTypes.Add(Type.String);
                else if (scriptCodes[i].Contains("int")) m_CurrentTypes.Add(Type.Int);
                else if (scriptCodes[i].Contains("float")) m_CurrentTypes.Add(Type.Float);
                //��O�Ȃ珈�����Ȃ�
                else continue;

                //�ϐ����L�����Ă����s�����L������
                if (ParameterReadFirstLine == 0)
                    ParameterReadFirstLine = i;

                //�p�����[�^�[����������̂��L������
                ParameterCount++;

                //�ϐ����󔒂ŕ�����
                string[] code = scriptCodes[i].Split(' ');


                int staticCount = 0;
                //static�����邩�ǂ����ŕ\����ύX����
                if (scriptCodes[i].Contains("static"))
                {
                    is_Static.Add(true);
                    staticCount = 1;
                }
                else
                {
                    is_Static.Add(false);
                }

                //readonly�����邩�ǂ����ŕ\����ύX����
                if (scriptCodes[i].Contains("readonly"))
                {
                    is_ReadOnly.Add(true);
                    variableNameList.Add(code[3 + staticCount]);
                }
                else
                {
                    is_ReadOnly.Add(false);
                    variableNameList.Add(code[2 + staticCount]);
                }

                if (m_CurrentTypes[m_CurrentTypes.Count - 1] == Type.String || m_CurrentTypes[m_CurrentTypes.Count - 1] == Type.Animator)
                    newNameList.Add(code[code.Length - 1].Split('"')[1]);
                else
                    newNameList.Add(code[code.Length - 1]);

                //�ϐ����X�g�ɒǉ�����
                variableCodeList.Add(scriptCodes[i]);
            }
        }

        //�Ō��}�����X�g����폜���鏈���@(���ꂾ��MonoBehaviour�ɂ����Ƃ��AStart�Ȃǂ����邽�߃o�O��)
        scriptCodes.RemoveAt(scriptCodes.Count - 1);

        if (!isThisMade) Debug.Log("VariablesCreator�ō��ꂽScript�ł͂���܂���");
    }
    /// <summary>
    /// �p�����[�^��ǉ������Ƃ��ɌĂяo��
    /// </summary>
    private void AddParam()
    {
        ParameterCount++;
        variableNameList.Add("�ϐ���");
        m_CurrentTypes.Add(Type.String);
        newNameList.Add("name");
        is_Static.Add(false);
        is_ReadOnly.Add(false);
        variableCodeList.Add(CordSetting(variableCodeList[variableCodeList.Count - 1], newNameList[newNameList.Count - 1]));

        //Lines���o�O�̌����H
    }
    /// <summary>
    /// �p�����[�^���폜�����Ƃ��ɌĂт���
    /// </summary>
    /// <param name="num"></param>
    private void RemoveParam(int num)
    {
        ParameterCount--;
        variableNameList.RemoveAt(num);
        m_CurrentTypes.RemoveAt(num);
        newNameList.RemoveAt(num);
        is_Static.RemoveAt(num);
        is_ReadOnly.RemoveAt(num);
        variableCodeList.RemoveAt(num);

        var intList = m_MemoryInt.ToList();
        intList.RemoveAt(num);
        m_MemoryInt = intList.ToArray();

        var floatList = m_MemoryFloat.ToList();
        floatList.RemoveAt(num);
        m_MemoryFloat = floatList.ToArray();
    }
    /// <summary>
    /// �����񂪃A���t�@�x�b�g���ǂ����𔻒�@�A���t�@�x�b�g�ȊO���܂܂�Ă�����false
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    private bool IsLetter(char c)
    {
        return (c >= 'A' && c <= 'z') ? true : false;
    }
    /// <summary>
    /// �z��̃T�C�Y��ύX����
    /// </summary>
    private void Resize()
    {
        Array.Resize(ref VariableNames, size);
        Array.Resize(ref stringField, size);
        Array.Resize(ref intField, size);
        Array.Resize(ref floatField, size);
        Array.Resize(ref m_Types, size);
    }
    /// <summary>
    /// �w�肵��������̏ꏊ��啶���ɂ���
    /// </summary>
    private string ToUpper(string self, int no)
    {
        if (no > self.Length) return self;

        var array = self.ToCharArray();
        var up = char.ToUpper(array[no]);
        array[no] = up;
        return new string(array);
    }
    /// �w�肳�ꂽ�p�X�̃t�H���_�𐶐�����
    /// </summary>
    /// <param name="path">�t�H���_�p�X�i��: Assets/Sample/FolderName�j</param>
    private void CreateFolder(string path)
    {
        var target = "";
        var splitChars = new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar };
        foreach (var dir in path.Split(splitChars))
        {
            var parent = target;
            target = Path.Combine(target, dir);
            if (!AssetDatabase.IsValidFolder(target))
            {
                AssetDatabase.CreateFolder(parent, dir);
            }
        }
    }
}
