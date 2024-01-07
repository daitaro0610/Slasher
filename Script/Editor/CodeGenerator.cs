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


    //スクリプトのサンプル---------------------

    private string m_DefaultCodeTemplete =
        "//MadeVariablesCreator\n" +
            "#USING#\n" +
            "using UnityEngine;\n" +
        "public class #CLASS_NAME#\n" +
            "{";

    private string m_Cord = "public #STATIC# #TYPE# #NAME# = #VALUE#;";

    //まだ未使用
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

    //スクロール位置
    private Vector2 _scrollPositionLeft = Vector2.zero;

    private Vector2 _scrollPositionRight = Vector2.zero;

    //ウィンドウの幅　片面のサイズ
    private float Width = 500;

    UnityEngine.Object target;

    private enum State
    {
        [InspectorName("スクリプト作成モード")]
        Create,
        [InspectorName("スクリプト編集モード")]
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
        None    //知らない変数だった時
    }
    private Type m_Type;

    private Type[] m_Types;


    //変数名
    public string[] VariableNames;
    //フィールドに記入された値を保存するのに使用する
    public string[] stringField;
    public int[] intField;
    public float[] floatField;

    private int[] m_MemoryInt;
    private float[] m_MemoryFloat;


    //アタッチしたスクリプトのパスの保存
    private string scriptPath = "";

    //アタッチされたクラスの名前を記憶して、DrawParamを一度だけ呼ぶようにしている
    private string memoryClassName = "";


    //overrideするスクリプトの情報を保存する
    private List<string> scriptCodes = new List<string>();
    private List<string> variableNameList = new List<string>();
    private List<string> variableCodeList = new List<string>();
    private List<string> newNameList = new List<string>();
    private List<bool> is_Static = new List<bool>();
    private List<bool> is_ReadOnly = new List<bool>();
    private List<Type> m_CurrentTypes = new List<Type>();
    private bool isThisMade = false;

    //最初にパラメーターを読み込んだ行
    private int ParameterReadFirstLine = 0;
    //変数の個数(Create用)
    private int size = 0;
    //変数の個数(OverWrite用)
    private int ParameterCount = 0;

    //MonoBehaviourにするかどうかの判定
    private bool is_MonoBehaviour;

    //読み取り専用かどうかの判定
    private bool is_ReadOnlyCreate;

    /// <summary>
    /// コードを記入するテンプレート
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
    /// Typeに合わせて変数の型の文字列を変更する
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
    /// 変数の型によって表示する方法を変更する
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
    /// ウィンドウを表示する
    /// </summary>
    [MenuItem("Window/VariablesCreator")]
    private static void CreateWindow()
    {
        GetWindow<CodeGenerator>("CodeGenerator");
    }
    /// <summary>
    /// ウィンドウの描画や、ステートに応じて表示を変更する
    /// </summary>
    private void OnGUI()
    {
        minSize = new Vector2(700, 200);
        maxSize = new Vector2(900, 1000);

        EditorGUILayout.LabelField("Mode");
        m_State = (State)EditorGUILayout.EnumPopup(m_State);

        EditorGUILayout.Space(10);

        //一つ目のフィールド
        EditorGUILayout.BeginHorizontal(GUI.skin.box);
        {
            //Stateの状態に応じて表示を変更する
            if (m_State == State.Create)
                CreateWindowMenu();
            else
                OverWriteWindowMenu();
        }
        EditorGUILayout.EndHorizontal();

    }
    /// <summary>
    /// StateがCreateの時に表示するGUI
    /// </summary>
    private void CreateWindowMenu()
    {
        //一つ目のフィールド
        EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(Width));
        {
            EditorGUILayout.LabelField("変数作成");

            EditorGUILayout.BeginHorizontal(GUI.skin.box);
            {
                EditorGUILayout.LabelField("Size", GUILayout.Width(100));
                size = EditorGUILayout.IntField(size, GUILayout.Width(100));
            }
            EditorGUILayout.EndHorizontal();


            //配列のサイズを変更
            Resize();


            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                //描画範囲が足りなければスクロール出来るように
                _scrollPositionLeft = EditorGUILayout.BeginScrollView(_scrollPositionLeft);

                EditorGUILayout.BeginHorizontal(GUI.skin.box);
                {
                    EditorGUILayout.LabelField("変数名", GUILayout.Width(100));
                    EditorGUILayout.LabelField("型", GUILayout.Width(100));
                    EditorGUILayout.LabelField("値", GUILayout.Width(100));
                }
                EditorGUILayout.EndHorizontal();


                //配列の数分表示する
                for (int i = 0; i < VariableNames.Length; i++)
                {
                    EditorGUILayout.BeginHorizontal(GUI.skin.box);
                    {
                        VariableNames[i] = EditorGUILayout.TextField(VariableNames[i], GUILayout.Width(100));


                        m_Types[i] = (Type)EditorGUILayout.EnumPopup(m_Types[i], GUILayout.Width(100));

                        //表示をTypeに応じて変更する
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

            //スクリプト作成ボタン
            GUILayout.Space(10);
            if (GUILayout.Button("作成"))
                Create();
        }
        EditorGUILayout.EndVertical();


        //二つ目のフィールド
        EditorGUILayout.BeginVertical(GUI.skin.box);
        {

            EditorGUILayout.LabelField("設定");

            m_FilePath = EditorGUILayout.TextField("FilePath", m_FilePath);

            FileName = EditorGUILayout.TextField("ScriptName", FileName);

            //Monobehaviourにするかどうかの判定
            EditorGUILayout.BeginHorizontal(GUI.skin.box, GUILayout.Width(300));
            {
                EditorGUILayout.LabelField("MonoBehaviour");

                is_MonoBehaviour = EditorGUILayout.Toggle(is_MonoBehaviour);
            }
            EditorGUILayout.EndHorizontal();

            //ReadOnlyにするかどうかの判定
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
    ///  StateがOverWriteの時に表示するGUI
    /// </summary>
    private void OverWriteWindowMenu()
    {
        //一つ目のフィールド
        EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(Width));
        {
            EditorGUILayout.LabelField("変数編集");


            if (variableNameList.Count > 0)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                {
                    //描画範囲が足りなければスクロール出来るように
                    _scrollPositionLeft = EditorGUILayout.BeginScrollView(_scrollPositionLeft);

                    EditorGUILayout.BeginHorizontal(GUI.skin.box);
                    {
                        EditorGUILayout.LabelField("変数名", GUILayout.Width(100));
                        EditorGUILayout.LabelField("型", GUILayout.Width(100));
                        EditorGUILayout.LabelField("値", GUILayout.Width(100));
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

                                //状態に応じて表示する
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

                                if (GUILayout.Button("削除"))
                                    RemoveParam(i);
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                    }

                    EditorGUILayout.EndScrollView();

                    GUILayout.Space(10);
                    if (GUILayout.Button("追加"))
                        AddParam();

                }
                EditorGUILayout.EndVertical();
            }

            GUILayout.Space(10);
            if (target != null && isThisMade)
            {
                if (GUILayout.Button("上書きする"))
                    OverWrite();
            }
        }
        EditorGUILayout.EndVertical();

        //二つ目のフィールド
        EditorGUILayout.BeginVertical(GUI.skin.box);
        {
            EditorGUILayout.LabelField("パラメータ");

            //MonoBehaviourを継承したクラスをアタッチできるようにする
            target = EditorGUILayout.ObjectField(
                   "Script",
                   target,
                   typeof(MonoScript), false);

            if (target != null)
            {
                scriptPath = AssetDatabase.GetAssetPath(target);

                //アタッチしたオブジェクトが変化したら再表示する
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

                //描画範囲が足りなければスクロール出来るように
                _scrollPositionRight = EditorGUILayout.BeginScrollView(_scrollPositionRight);
                //インスペクタのように表示する
                Editor.CreateEditor(target).OnInspectorGUI();

                //スクロール箇所終了
                EditorGUILayout.EndScrollView();
            }
        }
        EditorGUILayout.EndVertical();
    }
    /// <summary>
    /// .csファイルを作成する
    /// </summary>
    private void Create()
    {
        if (FileName == "")
        {
            Debug.LogError("名前が見つかりません");
            return;
        }

        if (!FileName.All(IsLetter))
        {
            Debug.LogError("アルファベット以外が含まれています");
            return;
        }


        //頭文字を大文字にする
        var className = ToUpper(FileName, 0);
        if (CheckIfClassExists(className))
        {
            Debug.LogError($"既にそのクラスが存在します。　クラス名{className}");
            return;
        }
        var filePath = m_FilePath + "/" + className + ".cs";

        var folderPath = Path.GetDirectoryName(filePath);
        CreateFolder(folderPath);

        //アセットのパスを作成
        var assetPath = AssetDatabase.GenerateUniqueAssetPath(filePath);



        if (is_MonoBehaviour) m_StaticCord = "";
        else m_StaticCord = "static ";

        if (is_ReadOnlyCreate) m_ReadOnlyCord = "readonly ";
        else m_ReadOnlyCord = "";

        //変数の作成
        string[] variableNamesCode = new string[VariableNames.Length];
        for (int i = 0; i < VariableNames.Length; i++)
        {
            if (!VariableNames[i].All(IsLetter))
                Debug.LogError("アルファベット以外が含まれています : " + VariableNames[i]);

            m_TypeNameCord = TypeSetting(m_Types[i]);

            //変数を型に合わせて書き換える
            variableNamesCode[i] = CordSetting(ToUpper(VariableNames[i], 0), TypeStringVariablesSetting(m_Types[i], stringField[i]));
        }

        //ファイルに書き込む
        StreamWriter streamWriter = new StreamWriter(assetPath);

        //コードテンプレートを置換
        var codeHead = m_DefaultCodeTemplete.Replace("#CLASS_NAME#", className);

        if (is_MonoBehaviour)
            codeHead = codeHead.Replace("#USING#", m_UsingTemplateCord);
        else codeHead = codeHead.Replace("#USING#", "");

        streamWriter.WriteLine(codeHead);

        for (int i = 0; i < variableNamesCode.Length; i++)
        {
            streamWriter.WriteLine(variableNamesCode[i]);
        }

        //MonoBehaviourならStartとUpdateのサンプルも表示する
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
    /// ファイルの内容を更新する
    /// </summary>
    private void OverWrite()
    {
        for (int i = 0; i < newNameList.Count; i++)
        {
            if (newNameList[i] == "")
            {
                Debug.LogError("文字列が見つかりません");
                return;
            }
        }

        for (int i = 0; i < variableNameList.Count; i++)
        {
            if (!variableNameList[i].All(IsLetter))
            {
                Debug.LogWarning("変数名にアルファベット以外が含まれています : " + variableNameList[i]);
            }
        }

        //ファイルに書き込む
        StreamWriter streamWriter = new StreamWriter(scriptPath);

        //コードテンプレートを置換
        var codeHead = m_DefaultCodeTemplete.Replace("#CLASS_NAME#", ToUpper(target.name, 0));

        if (is_MonoBehaviour)
        {
            codeHead = codeHead.Replace("#USING#", m_UsingTemplateCord);
        }
        else codeHead = codeHead.Replace("#USING#", "");

        streamWriter.WriteLine(codeHead);

        //ここから変数
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

        //StartとかUpdateとか
        if (is_MonoBehaviour)
            streamWriter.WriteLine(m_MonoSampleCord);

        //最後の}を入力する
        streamWriter.WriteLine(m_EndScriptCord);
        streamWriter.Close();

        ParameterCount = 0;
        DrawParam();

        AssetDatabase.Refresh();
    }
    /// <summary>
    /// スクリプトファイルをアタッチしたときにウィンドウに変数のパラメータを表示する値をここで代入する
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

        //ファイルを一行ずつ表示する
        foreach (string line in File.ReadLines(scriptPath))
        {
            scriptCodes.Add(line);
        }

        if (scriptCodes.Count == 0) Debug.LogError("スクリプトにコードが見つかりません");

        //文字を分割する
        for (int i = 0; i < scriptCodes.Count; i++)
        {
            if (scriptCodes[i].Contains("//MadeVariablesCreator"))
                isThisMade = true;
            //これが変数かどうかの判定
            if (scriptCodes[i].Contains("public ") && scriptCodes[i].Contains("="))
            {
                if (!isThisMade) continue;

                if (scriptCodes[i].Contains("StringToHash")) m_CurrentTypes.Add(Type.Animator);
                else if (scriptCodes[i].Contains("string")) m_CurrentTypes.Add(Type.String);
                else if (scriptCodes[i].Contains("int")) m_CurrentTypes.Add(Type.Int);
                else if (scriptCodes[i].Contains("float")) m_CurrentTypes.Add(Type.Float);
                //例外なら処理しない
                else continue;

                //変数を記入していく行数を記憶する
                if (ParameterReadFirstLine == 0)
                    ParameterReadFirstLine = i;

                //パラメーターがいくつあるのか記憶する
                ParameterCount++;

                //変数を空白で分ける
                string[] code = scriptCodes[i].Split(' ');


                int staticCount = 0;
                //staticがあるかどうかで表示を変更する
                if (scriptCodes[i].Contains("static"))
                {
                    is_Static.Add(true);
                    staticCount = 1;
                }
                else
                {
                    is_Static.Add(false);
                }

                //readonlyがあるかどうかで表示を変更する
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

                //変数リストに追加する
                variableCodeList.Add(scriptCodes[i]);
            }
        }

        //最後の}をリストから削除する処理　(これだとMonoBehaviourにしたとき、Startなどがあるためバグる)
        scriptCodes.RemoveAt(scriptCodes.Count - 1);

        if (!isThisMade) Debug.Log("VariablesCreatorで作られたScriptではありません");
    }
    /// <summary>
    /// パラメータを追加したときに呼び出す
    /// </summary>
    private void AddParam()
    {
        ParameterCount++;
        variableNameList.Add("変数名");
        m_CurrentTypes.Add(Type.String);
        newNameList.Add("name");
        is_Static.Add(false);
        is_ReadOnly.Add(false);
        variableCodeList.Add(CordSetting(variableCodeList[variableCodeList.Count - 1], newNameList[newNameList.Count - 1]));

        //Linesがバグの原因？
    }
    /// <summary>
    /// パラメータを削除したときに呼びだす
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
    /// 文字列がアルファベットかどうかを判定　アルファベット以外が含まれていたらfalse
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    private bool IsLetter(char c)
    {
        return (c >= 'A' && c <= 'z') ? true : false;
    }
    /// <summary>
    /// 配列のサイズを変更する
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
    /// 指定した文字列の場所を大文字にする
    /// </summary>
    private string ToUpper(string self, int no)
    {
        if (no > self.Length) return self;

        var array = self.ToCharArray();
        var up = char.ToUpper(array[no]);
        array[no] = up;
        return new string(array);
    }
    /// 指定されたパスのフォルダを生成する
    /// </summary>
    /// <param name="path">フォルダパス（例: Assets/Sample/FolderName）</param>
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
