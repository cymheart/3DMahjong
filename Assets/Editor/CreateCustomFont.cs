using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class CreateCustomFont : EditorWindow
{
    [MenuItem("MyTools/Create Custom Font")]
    static void AddWindow()
    {
        //创建窗口
        Rect wr = new Rect(0, 0, 300, 300);
        CreateCustomFont window = (CreateCustomFont)EditorWindow.GetWindowWithRect(typeof(CreateCustomFont), wr, true, "Create Custom Font");
        window.Show();

    }

    //输入文字的内容
    private string text;
    //选择贴图的对象
    private Texture texture;

    private Font font;
    TextureImporter textureImporter;
    Texture2D texture2D;
    //  GameObject buggyGameobject;
    public void Awake()
    {
        //在资源中读取一张贴图
       // texture = Resources.Load("1") as Texture;
    }

    //绘制窗口时调用
    void OnGUI()
    {
        //输入框控件
       // text = EditorGUILayout.TextField("输入文字:", text);
       // string selectionPathx = AssetDatabase.GetAssetPath(Selection.activeObject);
       // font = AssetDatabase.LoadAssetAtPath<Font>("Assets\\Resources\\New Font.fontsettings");

        //object
        //buggyGameobject = (GameObject)EditorGUILayout.ObjectField("Buggy Gameobject", buggyGameobject, typeof(GameObject), true);

        //font
        font = (Font)EditorGUILayout.ObjectField("自定义字体文件",font, typeof(Font), true, GUILayout.MinWidth(100f));
        texture2D = (Texture2D)EditorGUILayout.ObjectField("自定义文字图片:",texture2D, typeof(Texture2D), true);


        if (GUILayout.Button("生成", GUILayout.Height(25)))
        {
            string selectionPath = AssetDatabase.GetAssetPath(texture2D);
            TextureImporter textureImporter = AssetImporter.GetAtPath(selectionPath) as TextureImporter;

            List<CharacterInfo> charList = new List<CharacterInfo>();
            Rect rect;
            int i = 46;
            foreach (var spmeta in textureImporter.spritesheet)
            {
                rect = spmeta.rect;

                CharacterInfo info = new CharacterInfo();
                info.index = i++;

                float uvx = rect.x / texture2D.width;
                float uvy = rect.y / texture2D.height;
                float uvw = rect.width / texture2D.width;
                float uvh = rect.height / texture2D.height;

                //float uvx = 1f * rect.x / texture2D.width;
                //float uvy = 1 - (1f * rect.y / texture2D.height);
                //float uvw = 1f * rect.width / texture2D.width;
                //float uvh = -1f * rect.height / texture2D.height;


                info.uvBottomLeft = new Vector2(uvx, uvy);
                info.uvBottomRight = new Vector2(uvx + uvw, uvy);
                info.uvTopLeft = new Vector2(uvx, uvy + uvh);
                info.uvTopRight = new Vector2(uvx + uvw, uvy + uvh);


                //info.vert.x = 0;
                //info.vert.y = -rect.y + rect.height;
                //info.glyphWidth = (int)rect.width;
                //info.glyphHeight = (int)rect.height;

                info.minX = 0;
                info.minY = (int)rect.y;
                info.glyphWidth = (int)rect.width;
                info.glyphHeight = (int)rect.height;  // 同上，不知道为什么要用负的，可能跟unity纹理uv有关  

                //info.minX = 0;
                //info.minY = 0; //(int)rect.y;   // 这样调出来的效果是ok的，原理未知  
                //info.glyphWidth = (int)rect.width;
                //info.glyphHeight = -(int)rect.height; // 同上，不知道为什么要用负的，可能跟unity纹理uv有关  
                info.advance = info.glyphWidth + 2;

                charList.Add(info);
            }

            font.characterInfo = charList.ToArray();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    //更新
    void Update()
    {

    }

    void OnFocus()
    {
        Debug.Log("当窗口获得焦点时调用一次");
    }

    void OnLostFocus()
    {
        Debug.Log("当窗口丢失焦点时调用一次");
    }

    void OnHierarchyChange()
    {
        Debug.Log("当Hierarchy视图中的任何对象发生改变时调用一次");
    }

    void OnProjectChange()
    {
        Debug.Log("当Project视图中的资源发生改变时调用一次");
    }

    void OnInspectorUpdate()
    {
        //Debug.Log("窗口面板的更新");
        //这里开启窗口的重绘，不然窗口信息不会刷新
        this.Repaint();
    }

    void OnSelectionChange()
    {
        //当窗口出去开启状态，并且在Hierarchy视图中选择某游戏对象时调用
        foreach (Transform t in Selection.transforms)
        {
            //有可能是多选，这里开启一个循环打印选中游戏对象的名称
            Debug.Log("OnSelectionChange" + t.name);
        }
    }

    void OnDestroy()
    {
        Debug.Log("当窗口关闭时调用");
    }
}


//public class CustomFont
//{
//    // Use this for initialization
//    [MenuItem("MyTools/Create Custom Font")]
//    static void CreateCustomFont()
//    {
//        string selectionPathx = AssetDatabase.GetAssetPath(Selection.activeObject);
//        Font customFont = AssetDatabase.LoadAssetAtPath<Font>(selectionPathx);
//        //  customFont.characterInfo = charList.ToArray();
//        AssetDatabase.SaveAssets();
//        AssetDatabase.Refresh();



//        string selectionPath = AssetDatabase.GetAssetPath(Selection.activeObject);
//        Texture2D selTex = Selection.activeObject as Texture2D;
//        string rootPath = Path.GetDirectoryName(selectionPath) + "/" + Selection.activeObject.name;
//        TextureImporter textureImporter = AssetImporter.GetAtPath(selectionPath) as TextureImporter;
//        //textureImporter.textureType = TextureImporterType.Advanced;
//        //textureImporter.isReadable = true;



//        foreach (var spmeta in textureImporter.spritesheet)
//        {
//            string path = rootPath + "/" + spmeta.name + ".png";
//            string subDir = Path.GetDirectoryName(path);
//            if (!Directory.Exists(subDir))
//            {
//                Directory.CreateDirectory(subDir);
//            }
//            Debug.Log("output :" + path);
//        }
//        AssetDatabase.Refresh();
//    }
//}