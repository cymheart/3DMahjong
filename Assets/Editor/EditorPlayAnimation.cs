using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.Collections.Generic;

[CanEditMultipleObjects]
[CustomEditor(typeof(Animation), true)]
public class EditorPlayAnimation : Editor
{
    public Rect windowRect = new Rect(20, 20, 350, 410);
    private GameObject select;
    private Animation anim;
    Dictionary<string, AnimationClip> animClipDict = new Dictionary<string, AnimationClip>();
    List<string> animClipNameList = new List<string>();
    static int index = 0;

    public static float curValue = 0f;
    private GameObject distGo = null;
    static bool isOpenAnim = true;

    GameObject shadowPlane;
    GameObject axisParent;
    int shaderTexIdx = 0;
    string axisName;

    string[] shaderTexNames = { "_MainTex", "_MainTex1", "_MainTex2" };
    string[] shaderAngNames = { "_Angle", "_Angle1", "_Angle2" };

    void OnSceneGUI()
    {
        windowRect = GUI.Window(0, windowRect, ToolWindow, "动画预览");
    }
   
    void ToolWindow(int windowID)
    {
        isOpenAnim = GUILayout.Toggle(isOpenAnim, "开启动作设置");
        if (isOpenAnim == true)
        {
            SelectInit();
            if (animClipNameList.Count == 0)
                return;

            index = EditorGUILayout.Popup(index, animClipNameList.ToArray());
            curValue = EditorGUILayout.Slider(curValue, 0, 1);
            Example(index);

            distGo = (GameObject)EditorGUILayout.ObjectField("设置测距物件", distGo, typeof(GameObject), true, GUILayout.MinWidth(100f));

            Vector3 dist = new Vector3(0, 0, 0);
            if (select != null && distGo != null)
            {
                dist = select.transform.position - distGo.transform.position;
            }

            EditorGUILayout.Vector3Field("偏移距离:", dist);

            if (GUILayout.Button("复制偏移距离值"))
            {
                TextEditor t = new TextEditor();
                t.text = dist.x + "f, " + dist.y + "f, " + dist.z + "f";
                t.OnFocus();
                t.Copy();
            }

            if (GUILayout.Button("复制测距物件的位移值"))
            {
                if (distGo)
                {
                    TextEditor t = new TextEditor();
                    t.text = distGo.transform.localPosition.x + "f, " + distGo.transform.localPosition.y + "f, " + distGo.transform.localPosition.z + "f";
                    t.OnFocus();
                    t.Copy();
                }
            }

            if (GUILayout.Button("复制测距物件的欧拉角"))
            {
                if (distGo)
                {
                    TextEditor t = new TextEditor();
                    t.text = distGo.transform.localEulerAngles.x + "f, " + distGo.transform.localEulerAngles.y + "f, " + distGo.transform.localEulerAngles.z + "f";
                    t.OnFocus();
                    t.Copy();
                }
            }
        }

        shadowPlane = (GameObject)EditorGUILayout.ObjectField("阴影接受平面", shadowPlane, typeof(GameObject), true, GUILayout.MinWidth(100f));
        axisParent = (GameObject)EditorGUILayout.ObjectField("轴放置父物件", axisParent, typeof(GameObject), true, GUILayout.MinWidth(100f));
        shaderTexIdx = EditorGUILayout.IntField("对应Shader纹理号:", shaderTexIdx);
        axisName = EditorGUILayout.TextField("生成轴名称:", axisName);
      

        if (GUILayout.Button("生成轴物体到目标"))
        {
            if (shadowPlane && axisParent)
            {
                GameObject axis = CreateShadowAxis(shadowPlane, shaderTexIdx, axisName);
                axis.transform.SetParent(axisParent.transform,true);
            }
        }

        GUI.DragWindow();
    }

    GameObject GetNodeTransformByNodePathNames(GameObject parentNode, string[] nodePathNames)
    {
        Transform cparent = parentNode.transform;
        foreach (string nodename in nodePathNames)
        {
            cparent = cparent.Find(nodename);
        }
        return cparent.gameObject;
    }

    void Example(int index)
    {
        AnimationState animState = anim[animClipNameList[index]];
        animState.enabled = true;
        animState.speed = 1f;
        animState.weight = 1;
        if (curValue > 1f)
        {
            curValue = 0f;
        }
        animState.normalizedTime = curValue;
        anim.Sample();
        animState.enabled = false;
    }


    //选中状态下的初始化
    private void SelectInit()
    {
        animClipNameList.Clear();
        animClipDict.Clear();
        select = Selection.activeGameObject;

        try
        {
            if (select == null)
                return;

            anim = select.GetComponent<Animation>();

            if (anim == null)
                return;

            foreach (AnimationState state in anim)
            {
                animClipNameList.Add(state.name);
                animClipDict.Add(state.name, anim.GetClip(state.name));
            }
        }
        catch
        {
            //EditorUtility.DisplayDialog("错误", "未知错误", "确定");
            EditorApplication.Beep();
        }
    }


    GameObject CreateShadowAxis(GameObject plane, int shaderTexIdx, string name)
    {
        GameObject shadowAxis = new GameObject(name);

        Renderer planeRenderer = plane.transform.GetComponent<Renderer>();
        Vector3 planeSize = planeRenderer.bounds.size;
        Material mat = planeRenderer.sharedMaterial;

        float ang = mat.GetFloat(shaderAngNames[shaderTexIdx]);
        Vector2 tiling = mat.GetTextureScale(shaderTexNames[shaderTexIdx]);
        Vector2 offset = mat.GetTextureOffset(shaderTexNames[shaderTexIdx]);

        float uCellSize = planeSize.x / tiling.x;
        float vCellSize = planeSize.z / tiling.y;

        float xpos = plane.transform.position.x + planeSize.x / 2 - uCellSize / 2 + offset.x / tiling.x * planeSize.x;
        float zpos = plane.transform.position.z + planeSize.z / 2 - vCellSize / 2 + offset.y / tiling.y * planeSize.z;
        shadowAxis.transform.localPosition = new Vector3(xpos, plane.transform.localPosition.y, zpos);

        Vector3 eulerAngs = shadowAxis.transform.localEulerAngles;
        shadowAxis.transform.eulerAngles = new Vector3(eulerAngs.x, -ang, eulerAngs.z);

        //HandShadowShaderInfo shaderInfo = shadowAxis.AddComponent<HandShadowShaderInfo>();
        //shaderInfo.tiling = tiling;
        //shaderInfo.offset = offset;
        //shaderInfo.shadowPos = new Vector2(xpos, zpos);
        //shaderInfo.ang = ang;

        return shadowAxis;

    }

}
