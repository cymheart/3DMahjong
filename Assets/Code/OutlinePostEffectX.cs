using UnityEngine;
using System.Collections;

public class OutlinePostEffectX : PostEffectBase
{

    private Camera mainCam = null;
    public Camera additionalCam = null;
    private RenderTexture renderTexture = null;

    public Shader outlineShader = null;
    //采样率  
    public float samplerScale = 0.01f;
    public int downSample = 0;
    public int iteration = 0;
    public Color outlineColor = Color.green;
    public GameObject[] targets;
    private MeshFilter[] meshFilters;

    private Material occupiedMaterial = null;
    public Material OccupiedMaterial { get { return CheckShaderAndCreateMaterial(outlineShader, ref occupiedMaterial); } }
    void Awake()
    {
        //创建一个和当前相机一致的相机  
        InitAdditionalCam();

    }

    private void InitAdditionalCam()
    {
        mainCam = GetComponent<Camera>();
        if (mainCam == null)
            return;

        //Transform addCamTransform = transform.Find("additionalCam");
        //if (addCamTransform != null)
        //    DestroyImmediate(addCamTransform.gameObject);

        //GameObject additionalCamObj = new GameObject("additionalCam");
        //additionalCam = additionalCamObj.AddComponent<Camera>();

      

        SetAdditionalCam();
    }

    private void SetAdditionalCam()
    {
        //if (additionalCam)
        //{

            additionalCam.CopyFrom(mainCam);
            additionalCam.clearFlags = CameraClearFlags.Color;
            additionalCam.backgroundColor = Color.black;
            additionalCam.cullingMask = 1 << LayerMask.NameToLayer("PostEffect");

            //additionalCam.transform.parent = mainCam.transform;
            //additionalCam.transform.localPosition = Vector3.zero;
            //additionalCam.transform.localRotation = Quaternion.identity;
            //additionalCam.transform.localScale = Vector3.one;
            //additionalCam.farClipPlane = mainCam.farClipPlane;
            //additionalCam.nearClipPlane = mainCam.nearClipPlane;
            //additionalCam.fieldOfView = mainCam.fieldOfView;
            //additionalCam.backgroundColor = Color.clear;
            //additionalCam.clearFlags = CameraClearFlags.Color;
            //additionalCam.cullingMask = 1 << LayerMask.NameToLayer("PostEffect");
            //additionalCam.depth = -999;


           // if (renderTexture == null)
           //     renderTexture = RenderTexture.GetTemporary(additionalCam.pixelWidth >> downSample, additionalCam.pixelHeight >> downSample, 0);
       // }
    }

    //void OnEnable()
    //{
    //    SetAdditionalCam();
    //    additionalCam.enabled = true;
    //}

    //void OnDisable()
    //{
    //    additionalCam.enabled = false;
    //}

    //void OnDestroy()
    //{
    //    if (renderTexture)
    //    {
    //        RenderTexture.ReleaseTemporary(renderTexture);
    //    }
    //  //  DestroyImmediate(additionalCam.gameObject);
    //}

    //unity提供的在渲染之前的接口，在这一步渲染描边到RT  
    //void OnPreRender()
    //{
    //    //使用OutlinePrepass进行渲染，得到RT  
    //    if (additionalCam && additionalCam.enabled)
    //    {
    //        //渲染到RT上  
    //        //首先检查是否需要重设RT，比如屏幕分辨率变化了  
    //        if (renderTexture != null && (renderTexture.width != Screen.width >> downSample || renderTexture.height != Screen.height >> downSample))
    //        {
                
    //        }

    //        additionalCam.targetTexture = renderTexture;
    //        additionalCam.RenderWithShader(outlineShader, "");

    //        //additionalCam.targetTexture = renderTexture;

    //        //for (int i = 0; i < targets.Length; i++)
    //        //{
    //        //    if (targets[i] == null)
    //        //        continue;
    //        //    meshFilters = targets[i].GetComponentsInChildren<MeshFilter>();
    //        //    for (int j = 0; j < meshFilters.Length; j++)
    //        //        if ((mainCam.cullingMask & (1 << meshFilters[j].gameObject.layer)) != 0) // 把主相机没渲染的也不加入渲染队列
    //        //            for (int k = 0; k < meshFilters[j].sharedMesh.subMeshCount; k++)
    //        //                Graphics.DrawMesh(meshFilters[j].sharedMesh, meshFilters[j].transform.localToWorldMatrix, OccupiedMaterial, LayerMask.NameToLayer("PostEffect"), additionalCam, k); // 描绘选中物体的所占面积
    //        //}
    //        //additionalCam.Render();  // 需要调用渲染函数，才能及时把描绘物体渲染到纹理中
    //    }
    //}

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (_Material && outlineShader)
        {
            SetAdditionalCam();

            renderTexture = RenderTexture.GetTemporary(source.width, source.height, 0);
            additionalCam.targetTexture = renderTexture;
           // additionalCam.RenderWithShader(outlineShader, "");

            for (int i = 0; i < targets.Length; i++)
            {
                if (targets[i] == null)
                    continue;
                meshFilters = targets[i].GetComponentsInChildren<MeshFilter>();
                for (int j = 0; j < meshFilters.Length; j++)
                    if ((mainCam.cullingMask & (1 << meshFilters[j].gameObject.layer)) != 0) // 把主相机没渲染的也不加入渲染队列
                        for (int k = 0; k < meshFilters[j].sharedMesh.subMeshCount; k++)
                            Graphics.DrawMesh(meshFilters[j].sharedMesh, meshFilters[j].transform.localToWorldMatrix, OccupiedMaterial, LayerMask.NameToLayer("PostEffect"), additionalCam, k); // 描绘选中物体的所占面积
            }
            additionalCam.Render();


            //renderTexture.width = 111;  
            //对RT进行Blur处理  
            RenderTexture temp1 = RenderTexture.GetTemporary(source.width, source.height, 0);
            RenderTexture temp2 = RenderTexture.GetTemporary(source.width, source.height, 0);

            //高斯模糊，两次模糊，横向纵向，使用pass0进行高斯模糊  
            _Material.SetVector("_offsets", new Vector4(0, samplerScale, 0, 0));
            Graphics.Blit(renderTexture, temp1, _Material, 0);
            _Material.SetVector("_offsets", new Vector4(samplerScale, 0, 0, 0));
            Graphics.Blit(temp1, temp2, _Material, 0);

            //如果有叠加再进行迭代模糊处理  
            for (int i = 0; i < iteration; i++)
            {
                _Material.SetVector("_offsets", new Vector4(0, samplerScale, 0, 0));
                Graphics.Blit(temp2, temp1, _Material, 0);
                _Material.SetVector("_offsets", new Vector4(samplerScale, 0, 0, 0));
                Graphics.Blit(temp1, temp2, _Material, 0);
            }

            //用模糊图和原始图计算出轮廓图,并和场景图叠加,节省一个Pass  
            _Material.SetTexture("_OriTex", renderTexture);
            _Material.SetTexture("_BlurTex", temp2);
            _Material.SetColor("_OutlineColor", outlineColor);
            Graphics.Blit(source, destination, _Material, 1);

            renderTexture.Release();
            temp1.Release();
            temp2.Release();

            //RenderTexture.ReleaseTemporary(renderTexture);
            //RenderTexture.ReleaseTemporary(temp1);
            //RenderTexture.ReleaseTemporary(temp2);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }
} 