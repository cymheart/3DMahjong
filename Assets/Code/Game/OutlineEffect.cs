using UnityEngine;

[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
public class OutlineEffect : MonoBehaviour
{
    private const string NODE = "Outline Camera";

    [Range(0.1f, 1.0f)]
    public float sample = 0.25f;
    [Range(0.0f, 1.0f)]
    public float spread = 0.5f;
    [Range(0, 4)]
    public int iteration = 2;
    [Range(0.0f, 10.0f)]
    public float intensity = 1.0f;
    public Color outlineColor = Color.white;
    public LayerMask cullingMask;

    private Camera outlineCamera
    {
        get
        {
            if (null == m_OutlineCamera)
            {
                Transform node = transform.Find(NODE);
                if (null == node)
                {
                    node = new GameObject(NODE).transform;
                    node.parent = transform;
                    node.localPosition = Vector3.zero;
                    node.localRotation = Quaternion.identity;
                    node.localScale = Vector3.one;
                }

                m_OutlineCamera = node.GetComponent<Camera>();
                if (null == m_OutlineCamera)
                {
                    m_OutlineCamera = node.gameObject.AddComponent<Camera>();
                }

                m_OutlineCamera.enabled = false;
                m_OutlineCamera.clearFlags = CameraClearFlags.SolidColor;
                m_OutlineCamera.backgroundColor = new Color(0, 0, 0, 0);
                m_OutlineCamera.renderingPath = RenderingPath.VertexLit;
                m_OutlineCamera.allowHDR = false;
                m_OutlineCamera.useOcclusionCulling = false;
                m_OutlineCamera.gameObject.hideFlags = HideFlags.HideAndDontSave;
            }

            return m_OutlineCamera;
        }
    }
    private Camera m_OutlineCamera;

    private Material outlineMaterial
    {
        get
        {
            if (m_OutlineMaterial == null)
            {
                m_OutlineMaterial = new Material(Shader.Find("Yogi/ImageEffect/Outline"));
                m_OutlineMaterial.hideFlags = HideFlags.HideAndDontSave;
            }

            return m_OutlineMaterial;
        }
    }
    private Material m_OutlineMaterial = null;

    private Material blurMaterial
    {
        get
        { 
            if (m_BlurMaterial == null)
            {
                m_BlurMaterial = new Material(Shader.Find("Yogi/ImageEffect/Blur"));
                m_BlurMaterial.hideFlags = HideFlags.HideAndDontSave;
            }

            return m_BlurMaterial;
        }
    }
    private Material m_BlurMaterial = null;

    private RenderTexture stencilMap;

    private void OnPreRender()
    {
        stencilMap = RenderTexture.GetTemporary(Screen.width, Screen.height, 0);

        Camera cam = GetComponent<Camera>();
        outlineCamera.CopyFrom(cam);
        outlineCamera.cullingMask = cullingMask;
        outlineCamera.targetTexture = stencilMap;
        outlineCamera.Render();
    }

    private void OnPostRender()
    {
        RenderTexture.ReleaseTemporary(stencilMap);
    }

    private void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
    {
        int width = (int)(sourceTexture.width * sample);
        int height = (int)(sourceTexture.height * sample);
        RenderTexture blurMap = RenderTexture.GetTemporary(width, height, 0);
        Graphics.Blit(stencilMap, blurMap);

        for (int i = 0; i < iteration; ++i)
        {
           

            if (blurMaterial != null)
            {
                RenderTexture buffer = RenderTexture.GetTemporary(width, height, 0);
                float offset = spread * i + 0.5f;

                blurMaterial.SetVector("_Offset", new Vector4(offset / width, offset / height));
                Graphics.Blit(blurMap, buffer, blurMaterial);
                RenderTexture.ReleaseTemporary(blurMap);
                blurMap = buffer;
            }
        }

        outlineMaterial.SetTexture("_StencilMap", stencilMap);
        outlineMaterial.SetTexture("_BlurMap", blurMap);
        outlineMaterial.SetColor("_OutlineColor", outlineColor);
        outlineMaterial.SetFloat("_Intensity", intensity);
        Graphics.Blit(sourceTexture, destTexture, outlineMaterial);


      //  Graphics.Blit(sourceTexture, destTexture);

        RenderTexture.ReleaseTemporary(blurMap);
    }

    private void OnDisable()
    {
        OnDestroy();
    }

    private void OnDestroy()
    {
        if (null != m_OutlineCamera)
        {
            if (Application.isPlaying)
            {
                Destroy(m_OutlineCamera.gameObject);
            }
            else
            {
                DestroyImmediate(m_OutlineCamera.gameObject);
            }
        }
    }
}
