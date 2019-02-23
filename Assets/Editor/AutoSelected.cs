using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
public class AutoSelectObject
{

    [MenuItem("MyTools/AutoSelect", false, 11)]
    static void Start()
    {

        GameObject go = GameObject.Find("mjtable");

        EditorGUIUtility.PingObject(go);
        Selection.activeGameObject = go;

        Vector3 pos = go.transform.position;

        go.transform.position = new Vector3(pos.x + 2, pos.y, pos.z);

        //也可以选择Project下的Object
        //Selection.activeObject  = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Cube.prefab");

    }
}
