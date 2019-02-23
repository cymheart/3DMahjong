using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotShadow : MonoBehaviour {

    // Use this for initialization

    public GameObject plane;
    Renderer planeRenderer;
    Vector3 orgPos;
    Vector3 planeSize;

    void Start ()
    {
        planeRenderer = plane.transform.GetComponent<Renderer>();
        planeSize = planeRenderer.bounds.size;

        float xCellSize = planeSize.x / 3;
        float yCellSize = planeSize.z / 2;

        float xpos = plane.transform.position.x + xCellSize;
        float zpos = plane.transform.position.z + yCellSize/2;
        orgPos = new Vector3(xpos, transform.position.y, zpos);
        transform.position = orgPos;
    }
	
	// Update is called once per frame
	void Update ()
    {
        float ang = -transform.localEulerAngles.y;
        Vector3 posOffset = transform.position - orgPos;

        float matUOffset = (posOffset.x / planeSize.x)*3;
        float matVOffset = (posOffset.z / planeSize.z)*2;

        Vector2 matOffset = new Vector2(matUOffset, matVOffset);
        Material mat = planeRenderer.material;

        mat.SetFloat("_Angle", ang);
        mat.SetTextureOffset("_MainTex", matOffset);
		
	}
}
