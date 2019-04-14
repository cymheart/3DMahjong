using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ComponentDesgin
{
    public enum ParseDataType
    {
        Unknown,
        Integer,
        Float,
        String,
        StringGroup,
        StringD,
        Vector3,
        DataWrap
    }

    public struct ParseDataWrap
    {
        public ParseDataType type;
        public string key;
    }

    public class ParseTextData
    {
        public AssetBundle ab;

        Dictionary<string, ParseDataType> dateTypeDict = new Dictionary<string, ParseDataType>();

        Dictionary<string, int> integerDefineDataDict = new Dictionary<string, int>();
        Dictionary<string, float> floatDefineDataDict = new Dictionary<string, float>();
        Dictionary<string, string> stringDefineDataDict = new Dictionary<string, string>();
        Dictionary<string, string[]> stringGroupDefineDataDict = new Dictionary<string, string[]>();
        Dictionary<string, Vector3> vector3DefineDataDict = new Dictionary<string, Vector3>();
        Dictionary<string, ParseDataWrap[]> parseDataWrapDict = new Dictionary<string, ParseDataWrap[]>();

        public ParseTextData()
        {
            dateTypeDict.Add("Integer", ParseDataType.Integer);
            dateTypeDict.Add("Float", ParseDataType.Float);
            dateTypeDict.Add("String", ParseDataType.String);
            dateTypeDict.Add("StringGroup", ParseDataType.StringGroup);
            dateTypeDict.Add("StringD", ParseDataType.StringD);
            dateTypeDict.Add("Vector3", ParseDataType.Vector3);
            dateTypeDict.Add("DataWrap", ParseDataType.DataWrap);
        }

        public string[] GetStringGroup(string key)
        {
            if (stringGroupDefineDataDict.ContainsKey(key))
                return stringGroupDefineDataDict[key];
            return null;
        }

        public string GetString(string key)
        {
            if (stringDefineDataDict.ContainsKey(key))
                return stringDefineDataDict[key];
            return null;
        }

        public int GetInteger(string key)
        {
            if (integerDefineDataDict.ContainsKey(key))
                return integerDefineDataDict[key];
            return -1;
        }

        public float GetFloat(string key)
        {
            if (floatDefineDataDict.ContainsKey(key))
                return floatDefineDataDict[key];
            return 0;
        }

        public Vector3 GetVector3(string key)
        {
            if (vector3DefineDataDict.ContainsKey(key))
                return vector3DefineDataDict[key];
            return new Vector3(0, 0, 0);
        }


        public ParseDataWrap[] GetDataWrap(string key)
        {
            if (parseDataWrapDict.ContainsKey(key))
                return parseDataWrapDict[key];
            return null;
        }


        /// <summary>
        /// 解析手部动作速度数据
        /// </summary>
        /// <param name="text"></param>
        /// <param name="handActionDataInfoDicts"></param>
        public void ParseHandActionSpeedData(string text, Dictionary<string, Dictionary<string, ActionDataInfo>> handActionDataInfoDicts)
        {
            using (StringReader sr = new StringReader(text))
            {
                string line;
                int lineIndex = 0;
                Dictionary<string, ActionDataInfo> curtActionDataInfoDict = null;
                int actionCount = 0;
                string[] datas;
                ActionDataInfo actionDataInfo;

                do
                {
                    line = sr.ReadLine();

                    if (line == null)
                        break;

                    else if (string.IsNullOrEmpty(line) || line[0] == '=')
                        continue;

                    if (lineIndex == 0)
                    {
                        datas = line.Split(' ');
                        curtActionDataInfoDict = new Dictionary<string, ActionDataInfo>();
                        handActionDataInfoDicts[datas[0]] = curtActionDataInfoDict;
                        actionCount = int.Parse(datas[1]);
                        lineIndex = 1;
                    }
                    else
                    {
                        if (lineIndex <= actionCount)
                        {
                            datas = line.Split(' ');
                            actionDataInfo = new ActionDataInfo();
                            actionDataInfo.speed = float.Parse(datas[1]);
                            actionDataInfo.crossFadeNormalTime = float.Parse(datas[2]);
                            curtActionDataInfoDict[datas[0]] = actionDataInfo;
                            lineIndex++;
                        }

                        if (lineIndex > actionCount)
                        {
                            lineIndex = 0;
                        }
                    }
                } while (true);
            }
        }


        /// <summary>
        /// 解析数据
        /// </summary>
        /// <param name="text"></param>
        /// <param name="ab"></param>
        /// <param name="speakAudioDict"></param>
        public void Parse(
            string text,
            Dictionary<string, Dictionary<int, Dictionary<int, AudioClip[]>>> audioDict = null,
            Dictionary<int, Dictionary<int, GameObject[]>> prefabsDict = null,
            Dictionary<int, Dictionary<string, GameObject[]>> prefabsByStrDict = null,
            Dictionary<int, Dictionary<int, Sprite[]>> spritesDict = null,
            Dictionary<int, Dictionary<string, Sprite[]>> spritesByStrDict = null,
            Dictionary<string, Sprite> fullSpriteDict = null)
        {
            using (StringReader sr = new StringReader(text))
            {
                string line;
                string speakType = null;
                int playerType = (int)PlayerType.FEMALE;
                Dictionary<int, Dictionary<int, AudioClip[]>> audioByPlayerTypeDict;
                Dictionary<int, AudioClip[]> audioByIdxDict = null;
                List<AudioClip> audioClipList = new List<AudioClip>();

                int spriteType = 0;
                int spriteDefaultVarType = 0;
                Dictionary<int, Sprite[]> spriteByIdxDict = null;
                Dictionary<string, Sprite[]> spriteByStrDict = null;
                List<Sprite> spriteList = new List<Sprite>();

                int prefabType = 0;
                int prefabDefaultVarType = 0;
                Dictionary<int, GameObject[]> prefabByIdxDict = null;
                Dictionary<string, GameObject[]> prefabByStrDict = null;
                List<GameObject> prefabList = new List<GameObject>();

                int key = 0;
                string keyStr = null;
                int state = 0;
                int type = 1;

                int defualtIntValue = 0;
                float defaultFloatValue = 0;


                char[] seqs = { ',', '}', '{' };

                do
                {
                    line = sr.ReadLine();

                    if (line == null)
                        break;

                    else if (string.IsNullOrEmpty(line) || line[0] == '=')
                        continue;

                    else if (line.Length >= 2 && line[0] == '/' && line[1] == '/')
                        continue;


                    switch (state)
                    {
                        case 0:
                            {
                                char[] str = line.ToCharArray();
                                int startpos = 0;
                                int identlen = 0;
                                int nextpos;
                                int cmp;

                                startpos = SkipSpace(str, str.Length, startpos);
                                if (str[startpos] != '#')
                                    continue;

                                startpos++;
                                nextpos = GetIdentifier(str, str.Length, ref startpos, ref identlen);
                                if (identlen == 0)
                                    continue;

                                string key0 = new string(str, startpos, identlen);
                                cmp = string.Compare(key0, "Define");
                                if (cmp != 0)
                                    continue;


                                startpos = nextpos;
                                nextpos = GetIdentifier(str, str.Length, ref startpos, ref identlen);
                                if (identlen == 0)
                                    continue;

                                string key2 = new string(str, startpos, identlen);
                                if (string.Compare(key2, "AUDIO") == 0)
                                {
                                    startpos = nextpos;
                                    nextpos = GetIdentifier(str, str.Length, ref startpos, ref identlen);
                                    if (identlen == 0)
                                        continue;

                                    speakType = new string(str, startpos, identlen);

                                    startpos = nextpos;
                                    nextpos = GetIdentifier(str, str.Length, ref startpos, ref identlen);
                                    if (identlen == 0)
                                        continue;

                                    string key1 = new string(str, startpos, identlen);
                                    playerType = integerDefineDataDict[key1];

                                    audioByPlayerTypeDict = GetAudioByPlayerTypeDict(audioDict, speakType);
                                    audioByIdxDict = GetAudioByIdxDict(audioByPlayerTypeDict, playerType);
                                    type = 1;
                                    state = 1;
                                }
                                else if (string.Compare(key2, "SPRITE") == 0)
                                {
                                    if (fullSpriteDict == null)
                                        continue;

                                    startpos = nextpos;
                                    nextpos = GetIdentifier(str, str.Length, ref startpos, ref identlen);
                                    if (identlen == 0)
                                        continue;

                                    string varType = new string(str, startpos, identlen);

                                    if (string.Compare(varType, "Integer") == 0)
                                    {
                                        spriteDefaultVarType = 0;
                                    }
                                    else if (string.Compare(varType, "String") == 0)
                                    {
                                        spriteDefaultVarType = 1;
                                    }
                                    else if (string.Compare(varType, "StringD") == 0)
                                    {
                                        spriteDefaultVarType = 2;
                                    }
                                    else
                                    {
                                        continue;
                                    }


                                    startpos = nextpos;
                                    nextpos = GetIdentifier(str, str.Length, ref startpos, ref identlen);
                                    if (identlen == 0)
                                        continue;

                                    string spriteTypeStr = new string(str, startpos, identlen);
                                    spriteType = integerDefineDataDict[spriteTypeStr];

                                    type = 2;
                                    state = 1;
                                }
                                else if (string.Compare(key2, "PREFAB") == 0)
                                {
                                    startpos = nextpos;
                                    nextpos = GetIdentifier(str, str.Length, ref startpos, ref identlen);
                                    if (identlen == 0)
                                        continue;

                                    string varType = new string(str, startpos, identlen);

                                    if (string.Compare(varType, "Integer") == 0)
                                    {
                                        prefabDefaultVarType = 0;
                                    }
                                    else if (string.Compare(varType, "String") == 0)
                                    {
                                        prefabDefaultVarType = 1;
                                    }
                                    else if (string.Compare(varType, "StringD") == 0)
                                    {
                                        prefabDefaultVarType = 2;
                                    }
                                    else
                                    {
                                        continue;
                                    }

                                    startpos = nextpos;
                                    nextpos = GetIdentifier(str, str.Length, ref startpos, ref identlen);
                                    if (identlen == 0)
                                        continue;

                                    string prefabTypeStr = new string(str, startpos, identlen);
                                    prefabType = integerDefineDataDict[prefabTypeStr];

                                    type = 3;
                                    state = 1;

                                }
                                else if (string.Compare(key2, "VAR") == 0)
                                {
                                    startpos = nextpos;
                                    nextpos = GetIdentifier(str, str.Length, ref startpos, ref identlen);
                                    if (identlen == 0)
                                        continue;

                                    string key3 = new string(str, startpos, identlen);

                                    if (string.Compare(key3, "Integer") == 0)
                                    {
                                        defualtIntValue = -1;
                                        type = 1;
                                        state = 2;
                                    }
                                    else if (string.Compare(key3, "String") == 0)
                                    {
                                        type = 2;
                                        state = 2;
                                    }
                                    else if (string.Compare(key3, "StringGroup") == 0)
                                    {
                                        state = 3;
                                    }
                                    else if (string.Compare(key3, "Float") == 0)
                                    {
                                        defaultFloatValue = 0;
                                        type = 3;
                                        state = 2;
                                    }
                                    else if (string.Compare(key3, "Vector3") == 0)
                                    {
                                        state = 4;
                                    }
                                    else if (string.Compare(key3, "DataWrap") == 0)
                                    {
                                        state = 5;
                                    }
                                }
                            }
                            break;

                        case 1:
                            {
                                spriteByIdxDict = null;
                                spriteByStrDict = null;

                                prefabByIdxDict = null;
                                prefabByStrDict = null;

                                audioClipList.Clear();
                                spriteList.Clear();
                                prefabList.Clear();

                                char[] str = line.ToCharArray();
                                int startpos = 0;
                                int identlen = 0;
                                int nextpos;
                                int cmp;

                                startpos = SkipSpace(str, str.Length, startpos);
                                if (str[startpos] == '#')
                                {
                                    startpos++;
                                    nextpos = GetIdentifier(str, str.Length, ref startpos, ref identlen);
                                    if (identlen == 0)
                                        continue;

                                    string key0 = new string(str, startpos, identlen);
                                    cmp = string.Compare(key0, "EndDefine");
                                    if (cmp == 0)
                                        state = 0;
                                    continue;
                                }

                                nextpos = GetIdentifier(str, str.Length, ref startpos, ref identlen);
                                if (identlen == 0)
                                    continue;

                                string key1 = new string(str, startpos, identlen);

                                if (type == 2)
                                {
                                    startpos = nextpos;
                                    startpos = SkipSpace(str, str.Length, startpos);
                                    if (str[startpos] != ':')
                                    {
                                        startpos++;

                                        switch (spriteDefaultVarType)
                                        {
                                            case 0:  //integer
                                                spriteByStrDict = null;
                                                spriteByIdxDict = GetSpriteByTypeDict(spritesDict, spriteType);
                                                key = integerDefineDataDict[key1];
                                                if (spriteByIdxDict.ContainsKey(key))
                                                    continue;
                                                break;

                                            case 1:   //string
                                                spriteByIdxDict = null;
                                                spriteByStrDict = GetSpriteByStrTypeDict(spritesByStrDict, spriteType);
                                                keyStr = stringDefineDataDict[key1];
                                                if (spriteByStrDict.ContainsKey(keyStr))
                                                    continue;
                                                break;


                                            case 2:   //stringd
                                                spriteByIdxDict = null;
                                                spriteByStrDict = GetSpriteByStrTypeDict(spritesByStrDict, spriteType);
                                                keyStr = key1;
                                                if (spriteByStrDict.ContainsKey(keyStr))
                                                    continue;
                                                break;
                                            default:
                                                continue;
                                        }
                                    }
                                    else
                                    {
                                        startpos++;
                                        nextpos = GetIdentifier(str, str.Length, ref startpos, ref identlen);
                                        if (identlen == 0)
                                            continue;

                                        string keyx = new string(str, startpos, identlen);
                                        cmp = string.Compare(keyx, "integer");
                                        if (cmp == 0)
                                        {
                                            spriteByStrDict = null;
                                            spriteByIdxDict = GetSpriteByTypeDict(spritesDict, spriteType);
                                            key = integerDefineDataDict[key1];
                                            if (spriteByIdxDict.ContainsKey(key))
                                                continue;
                                        }
                                        else
                                        {
                                            cmp = string.Compare(keyx, "stringd");
                                            if (cmp == 0)
                                            {
                                                spriteByIdxDict = null;
                                                spriteByStrDict = GetSpriteByStrTypeDict(spritesByStrDict, spriteType);
                                                keyStr = key1;
                                                if (spriteByStrDict.ContainsKey(keyStr))
                                                    continue;

                                            }
                                            else
                                            {
                                                cmp = string.Compare(keyx, "string");
                                                if (cmp == 0)
                                                {
                                                    spriteByIdxDict = null;
                                                    spriteByStrDict = GetSpriteByStrTypeDict(spritesByStrDict, spriteType);
                                                    keyStr = stringDefineDataDict[key1];

                                                    if (spriteByStrDict.ContainsKey(keyStr))
                                                        continue;
                                                }
                                                else
                                                {
                                                    continue;
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (type == 3)
                                {
                                    startpos = nextpos;
                                    startpos = SkipSpace(str, str.Length, startpos);
                                    if (str[startpos] == '=')
                                    {
                                        switch (prefabDefaultVarType)
                                        {
                                            case 0:  //integer
                                                prefabByStrDict = null;
                                                prefabByIdxDict = GetPrefabByTypeDict(prefabsDict, prefabType);
                                                key = integerDefineDataDict[key1];
                                                if (prefabByIdxDict.ContainsKey(key))
                                                    continue;
                                                break;

                                            case 1:   //string
                                                prefabByIdxDict = null;
                                                prefabByStrDict = GetPrefabByStrTypeDict(prefabsByStrDict, prefabType);
                                                keyStr = stringDefineDataDict[key1];
                                                if (prefabByStrDict.ContainsKey(keyStr))
                                                    continue;
                                                break;


                                            case 2:   //stringd
                                                prefabByIdxDict = null;
                                                prefabByStrDict = GetPrefabByStrTypeDict(prefabsByStrDict, prefabType);
                                                keyStr = key1;
                                                if (prefabByStrDict.ContainsKey(keyStr))
                                                    continue;
                                                break;
                                            default:
                                                continue;
                                        }
                                    }
                                    else if (str[startpos] == ':')
                                    {
                                        startpos++;
                                        nextpos = GetIdentifier(str, str.Length, ref startpos, ref identlen);
                                        if (identlen == 0)
                                            continue;

                                        string keyx = new string(str, startpos, identlen);
                                        cmp = string.Compare(keyx, "integer");
                                        if (cmp == 0)
                                        {
                                            prefabByStrDict = null;
                                            prefabByIdxDict = GetPrefabByTypeDict(prefabsDict, spriteType);
                                            key = integerDefineDataDict[key1];
                                            if (spriteByIdxDict.ContainsKey(key))
                                                continue;
                                        }
                                        else
                                        {
                                            cmp = string.Compare(keyx, "stringd");
                                            if (cmp == 0)
                                            {
                                                prefabByIdxDict = null;
                                                prefabByStrDict = GetPrefabByStrTypeDict(prefabsByStrDict, spriteType);
                                                keyStr = key1;
                                                if (prefabByStrDict.ContainsKey(keyStr))
                                                    continue;

                                            }
                                            else
                                            {
                                                cmp = string.Compare(keyx, "string");
                                                if (cmp == 0)
                                                {
                                                    prefabByIdxDict = null;
                                                    prefabByStrDict = GetPrefabByStrTypeDict(prefabsByStrDict, spriteType);
                                                    keyStr = stringDefineDataDict[key1];

                                                    if (prefabByStrDict.ContainsKey(keyStr))
                                                        continue;
                                                }
                                                else
                                                {
                                                    continue;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }
                                else
                                {
                                    key = integerDefineDataDict[key1];
                                    if (audioByIdxDict.ContainsKey(key))
                                        continue;
                                }

                                startpos = nextpos;
                                startpos = SkipSpace(str, str.Length, startpos);
                                if (str[startpos] != '=')
                                    continue;

                                startpos = SkipSpace(str, str.Length, ++startpos);
                                if (str[startpos] != '{')
                                    continue;

                                nextpos = ++startpos;
                                int endState = 0;
                                string key2;
                                AudioClip audioClip;
                                Sprite sprite;
                                GameObject prefab;

                                for (; ; )
                                {
                                    startpos = nextpos;
                                    nextpos = GetIdentifier(seqs, str, str.Length, ref startpos, ref identlen);
                                    if (identlen == 0)
                                    {
                                        startpos = SkipSpace(str, str.Length, startpos);
                                        if (str[startpos] == '}')
                                        {
                                            endState = 1;
                                            break;
                                        }
                                        else if (str[startpos] == ',')
                                        {
                                            nextpos = ++startpos;
                                            continue;
                                        }
                                    }

                                    if (type == 1)
                                    {
                                        key2 = new string(str, startpos, identlen);
                                        audioClip = ab.LoadAsset<AudioClip>(key2);
                                        audioClipList.Add(audioClip);
                                    }
                                    else if (type == 2)
                                    {
                                        key2 = new string(str, startpos, identlen);
                                        sprite = fullSpriteDict[key2];
                                        spriteList.Add(sprite);
                                    }
                                    else if (type == 3)
                                    {
                                        key2 = new string(str, startpos, identlen);
                                        prefab = ab.LoadAsset<GameObject>(key2);
                                        prefabList.Add(prefab);
                                    }
                                }

                                if (endState == 1)
                                {
                                    if (type == 1)
                                    {
                                        audioByIdxDict[key] = audioClipList.ToArray();
                                    }
                                    else if (type == 2)
                                    {
                                        if (spriteByIdxDict != null)
                                        {
                                            spriteByIdxDict[key] = spriteList.ToArray();
                                        }
                                        else if (spriteByStrDict != null)
                                        {
                                            spriteByStrDict[keyStr] = spriteList.ToArray();
                                        }
                                    }
                                    else if (type == 3)
                                    {
                                        if (prefabByIdxDict != null)
                                        {
                                            prefabByIdxDict[key] = prefabList.ToArray();
                                        }
                                        else if (prefabByStrDict != null)
                                        {
                                            prefabByStrDict[keyStr] = prefabList.ToArray();
                                        }
                                    }

                                    continue;
                                }
                            }
                            break;

                        case 2:
                            {
                                char[] str = line.ToCharArray();
                                int startpos = 0;
                                int identlen = 0;
                                int nextpos;
                                int cmp;

                                startpos = SkipSpace(str, str.Length, startpos);
                                if (str[startpos] == '#')
                                {
                                    startpos++;
                                    nextpos = GetIdentifier(str, str.Length, ref startpos, ref identlen);
                                    if (identlen == 0)
                                        continue;

                                    string key0 = new string(str, startpos, identlen);
                                    cmp = string.Compare(key0, "EndDefine");
                                    if (cmp == 0)
                                        state = 0;
                                    continue;
                                }

                                nextpos = GetIdentifier(str, str.Length, ref startpos, ref identlen);
                                string key1 = new string(str, startpos, identlen);
                                if (identlen == 0)
                                    continue;

                                startpos = nextpos;
                                startpos = SkipSpace(str, str.Length, startpos);
                                string key3;

                                if (type == 1)
                                {
                                    if (startpos == str.Length)
                                    {
                                        defualtIntValue++;
                                        integerDefineDataDict[key1] = defualtIntValue;
                                    }
                                    else
                                    {
                                        if (str[startpos] != '=')
                                            continue;
                                        startpos++;
                                        nextpos = GetInteger(str, str.Length, ref startpos, ref identlen);
                                        key3 = new string(str, startpos, identlen);
                                        nextpos = SkipSpace(str, str.Length, nextpos);
                                        if (nextpos == str.Length)
                                        {
                                            defualtIntValue = int.Parse(key3);
                                            integerDefineDataDict[key1] = defualtIntValue;
                                        }
                                    }
                                }
                                else if (type == 3)
                                {
                                    if (startpos == str.Length)
                                    {
                                        defualtIntValue++;
                                        floatDefineDataDict[key1] = defaultFloatValue;
                                    }
                                    else
                                    {
                                        if (str[startpos] != '=')
                                            continue;
                                        startpos++;
                                        nextpos = GetFloat(str, str.Length, ref startpos, ref identlen);
                                        key3 = new string(str, startpos, identlen);
                                        nextpos = SkipSpace(str, str.Length, nextpos);
                                        if (nextpos == str.Length)
                                        {
                                            defaultFloatValue = float.Parse(key3);
                                            floatDefineDataDict[key1] = defaultFloatValue;
                                        }
                                    }

                                }
                                else
                                {
                                    if (str[startpos] != '=')
                                        continue;

                                    startpos++;
                                    nextpos = GetIdentifier(str, str.Length, ref startpos, ref identlen);
                                    if (identlen == 0)
                                        continue;

                                    key3 = new string(str, startpos, identlen);
                                    nextpos = SkipSpace(str, str.Length, nextpos);

                                    if (nextpos == str.Length)
                                        stringDefineDataDict[key1] = key3;

                                }
                            }
                            break;

                        case 3:
                            {
                                List<string> strs = new List<string>();
                                char[] str = line.ToCharArray();
                                int startpos = 0;
                                int identlen = 0;
                                int nextpos;
                                int cmp;

                                startpos = SkipSpace(str, str.Length, startpos);
                                if (str[startpos] == '#')
                                {
                                    startpos++;
                                    nextpos = GetIdentifier(str, str.Length, ref startpos, ref identlen);
                                    if (identlen == 0)
                                        continue;

                                    string key0 = new string(str, startpos, identlen);
                                    cmp = string.Compare(key0, "EndDefine");
                                    if (cmp == 0)
                                        state = 0;
                                    continue;
                                }

                                nextpos = GetIdentifier(str, str.Length, ref startpos, ref identlen);
                                string key1 = new string(str, startpos, identlen);
                                if (identlen == 0)
                                    continue;

                                startpos = nextpos;
                                startpos = SkipSpace(str, str.Length, startpos);
                                if (str[startpos] != '=')
                                    continue;

                                startpos = SkipSpace(str, str.Length, ++startpos);
                                if (str[startpos] != '{')
                                    continue;

                                nextpos = ++startpos;
                                int endState = 0;
                                string key2;

                                for (; ; )
                                {
                                    startpos = nextpos;
                                    nextpos = GetIdentifier(str, str.Length, ref startpos, ref identlen);
                                    if (identlen == 0)
                                    {
                                        startpos = SkipSpace(str, str.Length, startpos);
                                        if (str[startpos] == '}')
                                        {
                                            endState = 1;
                                            break;
                                        }
                                        else if (str[startpos] == ',')
                                        {
                                            nextpos = ++startpos;
                                            continue;
                                        }
                                    }

                                    key2 = new string(str, startpos, identlen);
                                    strs.Add(key2);

                                }

                                if (endState == 1)
                                {
                                    stringGroupDefineDataDict[key1] = strs.ToArray();
                                    continue;
                                }
                            }
                            break;

                        case 4:
                            {
                                char[] str = line.ToCharArray();
                                int startpos = 0;
                                int identlen = 0;
                                int nextpos;
                                int cmp;

                                startpos = SkipSpace(str, str.Length, startpos);
                                if (str[startpos] == '#')
                                {
                                    startpos++;
                                    nextpos = GetIdentifier(str, str.Length, ref startpos, ref identlen);
                                    if (identlen == 0)
                                        continue;

                                    string key0 = new string(str, startpos, identlen);
                                    cmp = string.Compare(key0, "EndDefine");
                                    if (cmp == 0)
                                        state = 0;
                                    continue;
                                }

                                nextpos = GetIdentifier(str, str.Length, ref startpos, ref identlen);
                                string key1 = new string(str, startpos, identlen);
                                if (identlen == 0)
                                    continue;

                                startpos = nextpos;
                                startpos = SkipSpace(str, str.Length, startpos);
                                if (str[startpos] != '=')
                                    continue;

                                startpos = SkipSpace(str, str.Length, ++startpos);
                                if (str[startpos] != '(')
                                    continue;

                                nextpos = ++startpos;
                                int endState = 0;
                                string key2;
                                float[] vec = new float[3];
                                int i = 0;

                                for (; ; )
                                {
                                    startpos = nextpos;
                                    nextpos = GetFloat(str, str.Length, ref startpos, ref identlen);
                                    if (identlen == 0)
                                    {
                                        startpos = SkipSpace(str, str.Length, startpos);
                                        if (str[startpos] == ')')
                                        {
                                            endState = 1;
                                            break;
                                        }
                                        else if (str[startpos] == ',')
                                        {
                                            nextpos = ++startpos;
                                            continue;
                                        }
                                    }

                                    if (i == 3)
                                        break;

                                    key2 = new string(str, startpos, identlen);
                                    vec[i] = float.Parse(key2);
                                    i++;
                                }

                                if (endState == 1)
                                {
                                    Vector3 v = new Vector3(vec[0], vec[1], vec[2]);
                                    vector3DefineDataDict[key1] = v;
                                    continue;
                                }
                            }
                            break;

                        case 5:
                            {
                                List<ParseDataWrap> dataWraps = new List<ParseDataWrap>();
                                char[] str = line.ToCharArray();
                                int startpos = 0;
                                int identlen = 0;
                                int nextpos;
                                int cmp;

                                startpos = SkipSpace(str, str.Length, startpos);
                                if (str[startpos] == '#')
                                {
                                    startpos++;
                                    nextpos = GetIdentifier(str, str.Length, ref startpos, ref identlen);
                                    if (identlen == 0)
                                        continue;

                                    string key0 = new string(str, startpos, identlen);
                                    cmp = string.Compare(key0, "EndDefine");
                                    if (cmp == 0)
                                        state = 0;
                                    continue;
                                }

                                nextpos = GetIdentifier(str, str.Length, ref startpos, ref identlen);
                                if (identlen == 0)
                                    continue;
                                string key1 = new string(str, startpos, identlen);

                                startpos = nextpos;
                                startpos = SkipSpace(str, str.Length, startpos);
                                if (str[startpos] != '=')
                                    continue;

                                startpos = SkipSpace(str, str.Length, ++startpos);
                                if (str[startpos] != '{')
                                    continue;

                                nextpos = ++startpos;
                                int endState = 0;
                                string wrapkey = null;
                                ParseDataWrap dataWrap = new ParseDataWrap();

                                for (; ; )
                                {
                                    startpos = nextpos;
                                    startpos = SkipSpace(str, str.Length, startpos);

                                    if (str[startpos] == ':')
                                    {
                                        startpos++;
                                        nextpos = GetIdentifier(str, str.Length, ref startpos, ref identlen);
                                        string datatypekey = new string(str, startpos, identlen);
                                        if (identlen == 0 || !dateTypeDict.ContainsKey(datatypekey))
                                        {
                                            break;
                                        }

                                        ParseDataType dataType = dateTypeDict[datatypekey];

                                        dataWrap = new ParseDataWrap();
                                        dataWrap.type = dataType;
                                        dataWrap.key = wrapkey;

                                    }
                                    else if (str[startpos] == ',')
                                    {
                                        dataWraps.Add(dataWrap);
                                        dataWrap.type = ParseDataType.StringD;
                                        dataWrap.key = null;
                                        nextpos = ++startpos;
                                        continue;
                                    }
                                    else if (str[startpos] == '}')
                                    {
                                        dataWraps.Add(dataWrap);
                                        endState = 1;
                                        break;
                                    }
                                    else
                                    {
                                        nextpos = GetIdentifier(str, str.Length, ref startpos, ref identlen);
                                        if (identlen == 0)
                                            break;

                                        wrapkey = new string(str, startpos, identlen);
                                    }
                                }

                                if (endState == 1)
                                {
                                    parseDataWrapDict[key1] = dataWraps.ToArray();
                                    continue;
                                }
                            }
                            break;
                    }

                } while (true);
            }
        }

        public Dictionary<int, Dictionary<int, AudioClip[]>> GetAudioByPlayerTypeDict(Dictionary<string, Dictionary<int, Dictionary<int, AudioClip[]>>> audioDict, string audioType)
        {
            if (audioDict.ContainsKey(audioType))
                return audioDict[audioType];

            Dictionary<int, Dictionary<int, AudioClip[]>> audioByPlayerTypeDict = new Dictionary<int, Dictionary<int, AudioClip[]>>();
            audioDict[audioType] = audioByPlayerTypeDict;
            return audioByPlayerTypeDict;
        }


        public Dictionary<int, AudioClip[]> GetAudioByIdxDict(Dictionary<int, Dictionary<int, AudioClip[]>> audioByPlayerTypeDict, int playerType)
        {
            if (audioByPlayerTypeDict.ContainsKey(playerType))
                return audioByPlayerTypeDict[playerType];

            Dictionary<int, AudioClip[]> audioByIdxDict = new Dictionary<int, AudioClip[]>();
            audioByPlayerTypeDict[playerType] = audioByIdxDict;
            return audioByIdxDict;
        }

        public Dictionary<int, GameObject[]> GetPrefabByTypeDict(Dictionary<int, Dictionary<int, GameObject[]>> prefabsDict, int prefabType)
        {
            if (prefabsDict.ContainsKey(prefabType))
                return prefabsDict[prefabType];

            Dictionary<int, GameObject[]> prefabByTypeDict = new Dictionary<int, GameObject[]>();
            prefabsDict[prefabType] = prefabByTypeDict;
            return prefabByTypeDict;
        }

        public Dictionary<string, GameObject[]> GetPrefabByStrTypeDict(Dictionary<int, Dictionary<string, GameObject[]>> prefabsDict, int prefabType)
        {
            if (prefabsDict.ContainsKey(prefabType))
                return prefabsDict[prefabType];

            Dictionary<string, GameObject[]> prefabByTypeDict = new Dictionary<string, GameObject[]>();
            prefabsDict[prefabType] = prefabByTypeDict;
            return prefabByTypeDict;
        }


        public Dictionary<int, Sprite[]> GetSpriteByTypeDict(Dictionary<int, Dictionary<int, Sprite[]>> spritesDict, int spriteType)
        {
            if (spritesDict.ContainsKey(spriteType))
                return spritesDict[spriteType];

            Dictionary<int, Sprite[]> spriteByTypeDict = new Dictionary<int, Sprite[]>();
            spritesDict[spriteType] = spriteByTypeDict;
            return spriteByTypeDict;
        }

        public Dictionary<string, Sprite[]> GetSpriteByStrTypeDict(Dictionary<int, Dictionary<string, Sprite[]>> spritesDict, int spriteType)
        {
            if (spritesDict.ContainsKey(spriteType))
                return spritesDict[spriteType];

            Dictionary<string, Sprite[]> spriteByTypeDict = new Dictionary<string, Sprite[]>();
            spritesDict[spriteType] = spriteByTypeDict;
            return spriteByTypeDict;
        }


        int GetIdentifier(char[] seps, char[] pack, int packlen, ref int startpos, ref int identlen)
        {
            int endpos, nextpos;
            startpos = SkipSpace(pack, packlen, startpos);

            for (int i = startpos; i < packlen; i++)
            {
                for (int n = 0; n < seps.Length; n++)
                {
                    if (pack[i] == seps[n])
                    {
                        nextpos = i;
                        endpos = i - 1;
                        identlen = endpos - startpos + 1;
                        return nextpos;
                    }
                }
            }

            identlen = packlen - startpos;
            return packlen + 1;
        }

        int GetIdentifier(char[] pack, int packlen, ref int startpos, ref int identlen)
        {
            int endpos, nextpos;
            startpos = SkipSpace(pack, packlen, startpos);

            if (!((pack[startpos] >= 'a' && pack[startpos] <= 'z') ||
                (pack[startpos] >= 'A' && pack[startpos] <= 'Z') ||
                pack[startpos] == '_' ||
                pack[startpos] == '.' ||
                pack[startpos] == '-'))
            {
                identlen = 0;
                return startpos;
            }

            for (int i = startpos + 1; i < packlen; i++)
            {
                if (!((pack[i] >= 'a' && pack[i] <= 'z') ||
                    (pack[i] >= 'A' && pack[i] <= 'Z') ||
                    pack[i] == '_' ||
                    pack[i] == '-' ||
                    pack[i] == '.' ||
                    (pack[i] >= '0' && pack[i] <= '9')))
                {
                    nextpos = i;
                    endpos = i - 1;
                    identlen = endpos - startpos + 1;
                    return nextpos;
                }
            }

            identlen = packlen - startpos;
            return packlen + 1;
        }

        int GetInteger(char[] pack, int packlen, ref int startpos, ref int identlen)
        {
            int endpos, nextpos;
            startpos = SkipSpace(pack, packlen, startpos);

            for (int i = startpos; i < packlen; i++)
            {
                if (!(pack[i] >= '0' && pack[i] <= '9'))
                {
                    nextpos = i;
                    endpos = i - 1;
                    identlen = endpos - startpos + 1;
                    return nextpos;
                }
            }

            identlen = packlen - startpos;
            return packlen + 1;
        }

        int GetFloat(char[] pack, int packlen, ref int startpos, ref int identlen)
        {
            int endpos, nextpos;
            int state = 0;

            startpos = SkipSpace(pack, packlen, startpos);
            for (int i = startpos; i < packlen; i++)
            {
                switch (state)
                {
                    case 0:
                        {
                            if (pack[i] == '-')
                            {
                                state = 1;
                            }
                            else if (pack[i] == '.')
                            {
                                state = 2;
                            }
                            else
                            {
                                if (!(pack[i] >= '0' && pack[i] <= '9'))
                                {
                                    nextpos = i;
                                    endpos = i - 1;
                                    identlen = endpos - startpos + 1;
                                    return nextpos;
                                }

                                state = 1;
                            }
                        }
                        break;

                    case 1:
                        if (pack[i] == '.')
                        {
                            state = 2;
                        }
                        else
                        {
                            if (!(pack[i] >= '0' && pack[i] <= '9'))
                            {
                                nextpos = i;
                                endpos = i - 1;
                                identlen = endpos - startpos + 1;
                                return nextpos;
                            }
                        }
                        break;

                    case 2:
                        {
                            if (!(pack[i] >= '0' && pack[i] <= '9'))
                            {
                                nextpos = i;
                                endpos = i - 1;
                                identlen = endpos - startpos + 1;
                                return nextpos;
                            }
                        }
                        break;
                }
            }

            identlen = packlen - startpos;
            return packlen + 1;
        }


        int SkipSpace(char[] pack, int packlen, int startpos)
        {
            for (int i = startpos; i < packlen; i++)
            {
                if (pack[i] == ' ')
                    continue;
                return i;
            }

            return packlen;
        }
    }
}