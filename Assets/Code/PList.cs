using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.IO;
using UnityEngine;

public class TPFrameData
{
    public string name;
    public Rect frame;
    public Vector2 offset;
    //public string rotated;
    public Rect sourceColorRect;
    public Vector2 sourceSize;
    public Sprite sprite;
    Texture2D texture;

    public TPFrameData(Texture2D texture)
    {
        this.texture = texture;
    }

    public void LoadX(string sname, PList plist)
    {
        name = sname;
        //frame = TPAtlas.StrToRect(plist["frame"] as string);
        //offset = TPAtlas.StrToVec2(plist["offset"] as string);
        //sourceColorRect = TPAtlas.StrToRect(plist["sourceColorRect"] as string);
        //sourceSize = TPAtlas.StrToVec2(plist["sourceSize"] as string);
        object varCheck;
        if (plist.TryGetValue("frame", out varCheck))
        {
            frame = TPAtlas.StrToRect(plist["frame"] as string);
            offset = TPAtlas.StrToVec2(plist["offset"] as string);
            sourceColorRect = TPAtlas.StrToRect(plist["sourceColorRect"] as string);
            sourceSize = TPAtlas.StrToVec2(plist["sourceSize"] as string);
        }
        else
        {
            int x = (int)plist["x"];
            int y = (int)plist["y"];
            int w = (int)plist["width"];
            int h = (int)plist["height"];
            frame = new Rect(x, y, w, h);

            float offsetx = (float)plist["offsetX"];
            float offsety = (float)plist["offsetY"];
            offset = new Vector2(offsetx, offsety);

            int originalWidth = (int)plist["originalWidth"];
            int originalHeight = (int)plist["originalHeight"];
            sourceSize = new Vector2(originalWidth, originalHeight);
        }
        //else
        //{
        //    frame = TPAtlas.StrToRect(plist["textureRect"] as string);
        //    offset = TPAtlas.StrToVec2(plist["spriteOffset"] as string);
        //    sourceColorRect = TPAtlas.StrToRect(plist["sourceColorRect"] as string);
        //    sourceSize = TPAtlas.StrToVec2(plist["spriteSourceSize"] as string);
        //}

        Rect rect = new Rect(frame.x, texture.height - frame.y - frame.height, frame.width, frame.height);//这里原点在左下角，y相反
        sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));
    }
}

public class TPAtlas
{
    public string realTextureFileName;
    public Vector2 size;
    public List<TPFrameData> sheets = new List<TPFrameData>();
    Texture2D texture;
    //string plistFilePath;
    string textureFilePath;

    public void CreateSpriteFrame(string fileContent, Texture2D tex)
    {
        texture = tex;
        //去掉<!DOCTYPE>,不然异常
        int delStart = fileContent.IndexOf("<!DOCTYPE");

        if (delStart >= 0)
        {
            int delEnd = fileContent.IndexOf("\n", delStart);
            fileContent = fileContent.Remove(delStart, delEnd - delStart);
            Debug.Log(fileContent);
        }

        //解析文件
        PList plist = new PList();
        plist.LoadText(fileContent);//Load(selectionPath);
        LoadX(plist);
    }

    //public void CreateSpriteFrame(string plistFilePath)
    //{
    //    this.plistFilePath = plistFilePath;

    //    if (!plistFilePath.EndsWith(".plist"))
    //    {
    //        Debug.Log("Error");
    //        return;
    //    }

    //    Debug.LogWarning("#PLisToSprites start:" + plistFilePath);
    //    string fileContent = string.Empty;
    //    using (FileStream file = new FileStream(plistFilePath, FileMode.Open))
    //    {
    //        byte[] str = new byte[(int)file.Length];
    //        file.Read(str, 0, str.Length);
    //        fileContent = GetUTF8String(str);
    //        Debug.Log(fileContent);
    //        file.Close();
    //        file.Dispose();
    //    }
    //    //去掉<!DOCTYPE>,不然异常
    //    int delStart = fileContent.IndexOf("<!DOCTYPE");

    //    if (delStart >= 0)
    //    {
    //        int delEnd = fileContent.IndexOf("\n", delStart);
    //        fileContent = fileContent.Remove(delStart, delEnd - delStart);
    //        Debug.Log(fileContent);
    //    }

    //    //解析文件
    //    PList plist = new PList();
    //    plist.LoadText(fileContent);//Load(selectionPath);
    //    LoadX(plist);
    //}

    public static string GetUTF8String(byte[] bt)
    {
        string val = System.Text.Encoding.UTF8.GetString(bt);
        return val;
    }

    public void LoadX(PList plist)
    {
        //read metadata
        PList meta = plist["metadata"] as PList;
        object varCheck;
        if (meta.TryGetValue("realTextureFileName", out varCheck))
        {
            realTextureFileName = meta["realTextureFileName"] as string;
        }
        else
        {
            PList ptarget = meta["target"] as PList;
            realTextureFileName = ptarget["name"] as string;
        }

        size = StrToVec2(meta["size"] as string);

       // int lastidx = plistFilePath.LastIndexOf('\\');
       //textureFilePath = plistFilePath.Substring(0, lastidx + 1) + realTextureFileName;
       // CreateTexture(textureFilePath, (int)size.x, (int)size.y);

        //read frames
        PList frames = plist["frames"] as PList;
        foreach (var kv in frames)
        {
            string name = kv.Key;
            PList framedata = kv.Value as PList;
            TPFrameData frame = new TPFrameData(texture);
            frame.LoadX(name, framedata);
            sheets.Add(frame);
        }
    }

    void CreateTexture(string texFilePath, int w, int h)
    {
        //创建文件读取流
        FileStream fileStream = new FileStream(texFilePath, FileMode.Open, FileAccess.Read);
        fileStream.Seek(0, SeekOrigin.Begin);
        //创建文件长度缓冲区
        byte[] bytes = new byte[fileStream.Length];
        //读取文件
        fileStream.Read(bytes, 0, (int)fileStream.Length);
        //释放文件读取流
        fileStream.Close();
        fileStream.Dispose();
        fileStream = null;

        //创建Texture
        texture = new Texture2D(w, h);
        texture.LoadImage(bytes);
    }

    public static Vector2 StrToVec2(string str)
    {

        str = str.Replace("{", "");
        str = str.Replace("}", "");
        string[] vs = str.Split(',');

        Vector2 v = new Vector2();
        v.x = float.Parse(vs[0]);
        v.y = float.Parse(vs[1]);
        return v;
    }
    public static Rect StrToRect(string str)
    {
        str = str.Replace("{", "");
        str = str.Replace("}", "");
        string[] vs = str.Split(',');

        Rect v = new Rect(float.Parse(vs[0]), float.Parse(vs[1]), float.Parse(vs[2]), float.Parse(vs[3]));
        return v;
    }

}
public class PList : Dictionary<string, object>
{
    public PList()
    {
    }

    public PList(string file)
    {
        Load(file);
    }

    public void Load(string file)
    {
        Clear();

        XDocument doc = XDocument.Load(file);
        XElement plist = doc.Element("plist");
        XElement dict = plist.Element("dict");

        var dictElements = dict.Elements();
        Parse(this, dictElements);
    }

    public void LoadText(string text)
    {
        Clear();
        XDocument doc = XDocument.Parse(text);
        XElement plist = doc.Element("plist");
        XElement dict = plist.Element("dict");

        var dictElements = dict.Elements();
        Parse(this, dictElements);
    }

    private void Parse(PList dict, IEnumerable<XElement> elements)
    {
        for (int i = 0; i < elements.Count(); i += 2)
        {
            XElement key = elements.ElementAt(i);
            XElement val = elements.ElementAt(i + 1);

            dict[key.Value] = ParseValue(val);
        }
    }

    private List<object> ParseArray(IEnumerable<XElement> elements)
    {
        List<object> list = new List<object>();
        foreach (XElement e in elements)
        {
            object one = ParseValue(e);
            list.Add(one);
        }

        return list;
    }

    private object ParseValue(XElement val)
    {
        switch (val.Name.ToString())
        {
            case "string":
                return val.Value;
            case "integer":
                return int.Parse(val.Value);
            case "real":
                return float.Parse(val.Value);
            case "true":
                return true;
            case "false":
                return false;
            case "dict":
                PList plist = new PList();
                Parse(plist, val.Elements());
                return plist;
            case "array":
                List<object> list = ParseArray(val.Elements());
                return list;
            default:
                throw new ArgumentException("Unsupported");
        }
    }
}