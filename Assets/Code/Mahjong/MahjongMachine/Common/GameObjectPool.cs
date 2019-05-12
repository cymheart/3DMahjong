using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class GameObjectPool
{
    public delegate void ProcessGO(GameObject go, object data);

    public List<GameObject> goList = new List<GameObject>();
    int ogListIdx = 0;
    GameObject orgGo;
    Transform parent = null;
    ProcessGO processGo;
    object data;


    public void CreatePool(GameObject orgGo, int Count, Transform parent = null, ProcessGO processGo = null, object data = null)
    {
        this.orgGo = orgGo;
        GameObject go;
        this.parent = parent;
        this.processGo = processGo;
        this.data = data;

        for (int i = 0; i < Count; i++)
        {
            if (parent == null)
                go = Object.Instantiate(orgGo);
            else
                go = Object.Instantiate(orgGo, parent);

            if (processGo != null)
                processGo(go, data);

            go.SetActive(false);
            goList.Add(go);
        }
    }

    public GameObject PopGameObject()
    {
        for (int i = 0; i < goList.Count; ++i)
        {
            int temI = (ogListIdx + i) % goList.Count;
            if (!goList[temI].activeInHierarchy)
            {
                ogListIdx = (temI + 1) % goList.Count;
                return goList[temI];
            }
        }

        GameObject go;

        if (parent == null)
            go = Object.Instantiate(orgGo);
        else
            go = Object.Instantiate(orgGo, parent);

        if (processGo != null)
            processGo(go, data);

        goList.Add(go);
        return go;
    }

    public void PushGameObject(GameObject go)
    {
        go.SetActive(false);

        if (parent != null)
            go.transform.SetParent(parent);
    }

    public void RecoverGameObjectsForParticles()
    {
        ParticleSystem ps;
        for (int i = 0; i < goList.Count; i++)
        {
            ps = goList[i].GetComponent<ParticleSystem>();
            if (goList[i].activeSelf && ps.isStopped)
                goList[i].SetActive(false);
        }
    }

    public List<GameObject> GetGameObjectList()
    {
        return goList;
    }

    public void Destory()
    {
        for (int i = 0; i < goList.Count; i++)
        {
            Object.Destroy(goList[i]);
        }

        goList.Clear();
        orgGo = null;
    }
}
