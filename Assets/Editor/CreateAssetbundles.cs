using UnityEditor;
using System.IO;
using UnityEngine;

public class CreateAssetbundles
{

    [MenuItem("MyTools/Build AssetBundles")]
    static void BuildAllAssetBundles()//进行打包
    {
        // string dir = Application.dataPath + "/StreamingAssets/" ;
        // string dir = Application.streamingAssetsPath;
        //  string dir = "jar:file://" + Application.dataPath + "!/assets/";
        //  string dir = "AssetBundles";
        //判断该目录是否存在
        //if (Directory.Exists(dir) == false)
        //{
        //    Directory.CreateDirectory(dir);//在工程下创建AssetBundles目录
        //}
        //参数一为打包到哪个路径，参数二压缩选项  参数三 平台的目标
        string streamPath = Application.streamingAssetsPath;

        BuildPipeline.BuildAssetBundles(streamPath, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);

        //BuildPipeline.BuildAssetBundles(streamPath, BuildAssetBundleOptions.None, BuildTarget.Android);
        AssetDatabase.Refresh();
    }
}