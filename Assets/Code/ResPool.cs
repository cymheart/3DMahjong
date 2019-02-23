using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Assets;
using Net;
using System.IO;
using MahjongMachineNS;
public class ResPool : MonoBehaviour
{
    static public App app;
    static public MahjongAssets mjAssets;
    public static int ass;
    BaseServer baseserver;
    Image imageLogin;
    InputField account;
    InputField password;

    int resLoadFinishedCount = 0;

   

    // Use this for initialization  
    void Start()
    {
        Application.targetFrameRate = 60;
        app = new App();
        app.resPool = this;

        mjAssets = new MahjongAssets();
        mjAssets.LoadMahjongAssetsCompleted = LoadMjResCompleted;


        GameObject[] netUpdate = GameObject.FindGameObjectsWithTag("NetUpdate");
        netUpdate[0].AddComponent<UnityUpdate>();
      //  UnityTaskProcesser taskProcesser = new UnityTaskProcesser(netUpdate[0].GetComponent<UnityUpdate>());
      //  baseserver = new BaseServer(app, taskProcesser);
     //   baseserver.ConnectServer("192.168.1.105", 9001);

        StartCoroutine(mjAssets.LoadMahjongRes());
        StartCoroutine("loadres");


      //  StartCoroutine(LoadHotFixAssembly());
    }


    private IEnumerator loadres()
    {
       // string path = "file://" + Application.dataPath + "/StreamingAssets" + "/dd/gamebtn.res";

       string path = "jar:file://" + Application.dataPath + "!/assets/dd/gamebtn.res";
        WWW bundle = new WWW(path);
        yield return bundle;

       

        app.CreateRes(bundle.assetBundle);
        app.CreateDefaultUI();

        if (bundle != null)
        {
            Text info = transform.Find("Info").GetComponent<Text>();
            info.text += "载入gamebtn.res成功!!\n";
        }

        resLoadFinishedCount++;
        LoadResFinished();
    }

    void LoadMjResCompleted()
    {
        resLoadFinishedCount++;
        LoadResFinished();
    }

    void LoadResFinished()
    {
        Text info = transform.Find("Info").GetComponent<Text>();
        info.text += resLoadFinishedCount;
        info.gameObject.SetActive(true);

        if (resLoadFinishedCount < 2)
            return;

        

        //
        imageLogin = transform.Find("ImageLogin").GetComponent<Image>();
        imageLogin.gameObject.SetActive(true);

        account = imageLogin.transform.Find("ImageAccount").Find("InputField").GetComponent<InputField>();
        password = imageLogin.transform.Find("ImagePassword").Find("InputField").GetComponent<InputField>();

        // Button btnLogin = imageLogin.transform.Find("ButtonLogin").GetComponent<Button>();
        // EventTriggerListener.Get(btnLogin.gameObject).onClick = OnButtonClick;


        LoadScene();
        // SceneManager.LoadScene("GameScene",LoadSceneMode.Additive);
    }


    private void OnButtonClick(GameObject go)
    {
        baseserver.ReqLogin(account.text, password.text);
    }

    public void LoadScene()
    {
        SceneManager.LoadSceneAsync("GameScene");
    }

    private IEnumerator loadres2()
    {
        //string path = "file://" + Application.dataPath + "/StreamingAssets" + "/dd/hall_music.mp3";
        string path = "jar:file://" + Application.dataPath + "!/assets/dd/hall_music.mp3";
        WWW bundle = new WWW(path);
        yield return bundle;



        AudioClip myaudioclip = bundle.GetAudioClip();
        AudioSource.PlayClipAtPoint(myaudioclip, transform.position, 1.0f);
    }


    IEnumerator LoadHotFixAssembly()
    {
        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        //这个DLL文件是直接编译HotFix_Project.sln生成的，已经在项目中设置好输出目录为StreamingAssets，在VS里直接编译即可生成到对应目录，无需手动拷贝
//#if UNITY_ANDROID
        WWW www = new WWW(Application.streamingAssetsPath + "/HotFix_Project.dll");
//#else
 //       WWW www = new WWW("file:///" + Application.streamingAssetsPath + "/HotFix_Project.dll");
//#endif
        while (!www.isDone)
            yield return null;
        if (!string.IsNullOrEmpty(www.error))
            UnityEngine.Debug.LogError(www.error);
        byte[] dll = www.bytes;
        www.Dispose();

        //PDB文件是调试数据库，如需要在日志中显示报错的行号，则必须提供PDB文件，不过由于会额外耗用内存，正式发布时请将PDB去掉，下面LoadAssembly的时候pdb传null即可
//#if UNITY_ANDROID
        www = new WWW(Application.streamingAssetsPath + "/HotFix_Project.pdb");
//#else
     //   www = new WWW("file:///" + Application.streamingAssetsPath + "/HotFix_Project.pdb");
//#endif
        while (!www.isDone)
            yield return null;
        if (!string.IsNullOrEmpty(www.error))
            UnityEngine.Debug.LogError(www.error);
        byte[] pdb = www.bytes;

        app.LoadHotFixAssembly(dll, pdb);

        resLoadFinishedCount++;
        LoadResFinished();
    }
}


