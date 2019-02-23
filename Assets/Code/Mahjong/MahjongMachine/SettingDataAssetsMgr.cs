using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MahjongMachineNS
{
    public class SettingDataAssetsMgr
    {
        public AssetBundle AssetBundle
        {
            set
            {
                assetBundle = value;
                parseTextSetting.ab = assetBundle;
            }
        }

        public Dictionary<string, Sprite> FullSpriteDict
        {
            set
            {
                fullSpriteDict = value;
            }

            get
            {
                return fullSpriteDict;
            }
        }

        AssetBundle assetBundle;
        Sprite[] sprites = null;
        public ParseTextData parseTextSetting = new ParseTextData();

        Dictionary<string, Dictionary<int, Dictionary<int, AudioClip[]>>> audioDict = new Dictionary<string, Dictionary<int, Dictionary<int, AudioClip[]>>>();

        Dictionary<int, Dictionary<int, GameObject[]>> prefabsDict = new Dictionary<int, Dictionary<int, GameObject[]>>();
        Dictionary<int, Dictionary<string, GameObject[]>> prefabsByStrDict = new Dictionary<int, Dictionary<string, GameObject[]>>();

        Dictionary<int, Dictionary<int, Sprite[]>> spritesDict = new Dictionary<int, Dictionary<int, Sprite[]>>();
        Dictionary<int, Dictionary<string, Sprite[]>> spritesByStrDict = new Dictionary<int, Dictionary<string, Sprite[]>>();

        Dictionary<string, Dictionary<string, ActionDataInfo>> handActionDataInfoDicts = new Dictionary<string, Dictionary<string, ActionDataInfo>>();
        List<ScreenFitInfo> screenFitInfoList = new List<ScreenFitInfo>();

        Dictionary<string, Sprite> fullSpriteDict = null;

        public SettingDataAssetsMgr()
        {


        }

        public void ParseDefineData(string[] defineDataName)
        {
            for (int i = 0; i < defineDataName.Length; i++)
            {
                TextAsset defineDataText = assetBundle.LoadAsset<TextAsset>(defineDataName[i]);
                parseTextSetting.Parse(defineDataText.text);
            }
        }

        public void ParseSettingData(string[] settingDataName)
        {
            if (fullSpriteDict == null)
                LoadFullSprites();

            for (int i = 0; i < settingDataName.Length; i++)
            {
                TextAsset settingDataText = assetBundle.LoadAsset<TextAsset>(settingDataName[i]);
                parseTextSetting.Parse(settingDataText.text, audioDict, prefabsDict, prefabsByStrDict, spritesDict, spritesByStrDict, fullSpriteDict);
            }
        }

        public void ParseHandActionSpeedData(string dataName)
        {
            TextAsset handActionSpeedText = assetBundle.LoadAsset<TextAsset>(dataName);
            parseTextSetting.ParseHandActionSpeedData(handActionSpeedText.text, handActionDataInfoDicts);
        }

        void LoadFullSprites()
        {
            sprites = assetBundle.LoadAllAssets<Sprite>();
            fullSpriteDict = new Dictionary<string, Sprite>();

            for (int i = 0; i < sprites.Length; i++)
            {
                fullSpriteDict.Add(sprites[i].name, sprites[i]);
            }
        }

        public void CreateScreenFitInfo()
        {
            ParseDataWrap[] screenDataWapSets = parseTextSetting.GetDataWrap("SCREEN_FIT_INFO_SET");

            for (int i = 0; i < screenDataWapSets.Length; i++)
            {
                if (screenDataWapSets[i].type == ParseDataType.DataWrap)
                {
                    ParseDataWrap[] screenDataWaps = parseTextSetting.GetDataWrap(screenDataWapSets[i].key);
                    if (screenDataWaps.Length < 5)
                        continue;

                    ScreenFitInfo screenFitInfo = new ScreenFitInfo();
                    screenFitInfo.screenAspect = parseTextSetting.GetFloat(screenDataWaps[0].key);
                    screenFitInfo.camPosition = parseTextSetting.GetVector3(screenDataWaps[1].key);
                    screenFitInfo.camEluerAngle = parseTextSetting.GetVector3(screenDataWaps[2].key);
                    screenFitInfo.camFieldOfView = parseTextSetting.GetFloat(screenDataWaps[3].key);
                    screenFitInfo.mjScale = parseTextSetting.GetFloat(screenDataWaps[4].key);
                    screenFitInfoList.Add(screenFitInfo);
                }
            }
        }

        public List<ScreenFitInfo> GetScreenFitInfoList()
        {
            return screenFitInfoList;
        }

        public Dictionary<int, AudioClip[]> GetAudiosDict(string audioType, int playerType = 2)
        {
            if (!audioDict.ContainsKey(audioType))
                return null;

            Dictionary<int, Dictionary<int, AudioClip[]>> dict = audioDict[audioType];
            if (!dict.ContainsKey(playerType))
                return null;

            return audioDict[audioType][playerType];
        }

        public AudioClip GetAudio(Dictionary<int, AudioClip[]> audiosDict, int audioIdx, int idx = 0)
        {
            AudioClip[] audioClips = audiosDict[audioIdx];

            if (audioClips == null)
                return null;

            if (idx < 0 && audioClips.Length >= 2)
            {
                idx = UnityEngine.Random.Range(0, audioClips.Length);
            }

            if (idx >= audioClips.Length)
                idx = audioClips.Length - 1;

            return audioClips[idx];
        }

        public Dictionary<int, GameObject[]> GetPrefabsIntKeyDict(int prefabType)
        {
            if (!prefabsDict.ContainsKey(prefabType))
                return null;

            return prefabsDict[prefabType];
        }

        public Dictionary<string, Dictionary<string, ActionDataInfo>> GetHandActionDataInfoDicts()
        {
            return handActionDataInfoDicts;
        }

        public Dictionary<string, GameObject[]> GetPrefabsStrKeyDict(int prefabType)
        {
            if (!prefabsByStrDict.ContainsKey(prefabType))
                return null;

            return prefabsByStrDict[prefabType];
        }


        public Dictionary<int, Sprite[]> GetSpritesIntKeyDict(int spriteType)
        {
            if (!spritesDict.ContainsKey(spriteType))
                return null;

            return spritesDict[spriteType];
        }


        public Dictionary<string, Sprite[]> GetSpritesStrKeyDict(int spriteType)
        {
            if (!spritesByStrDict.ContainsKey(spriteType))
                return null;

            return spritesByStrDict[spriteType];
        }

        public void DestroyAssets()
        {
            DestroyAudio();

            if (sprites != null)
            {
                for (int i = 0; i < sprites.Length; i++)
                    Resources.UnloadAsset(sprites[i]);
            }
        }

        void DestroyAudio()
        {
            foreach (var audioByPlayerTypeDict in audioDict)
            {
                foreach (var audioByIdxDict in audioByPlayerTypeDict.Value)
                {
                    foreach (var audioClip in audioByIdxDict.Value)
                    {
                        for (int i = 0; i < audioClip.Value.Length; i++)
                        {
                            Resources.UnloadAsset(audioClip.Value[i]);
                        }
                    }
                    audioByIdxDict.Value.Clear();
                }

                audioByPlayerTypeDict.Value.Clear();
            }

            audioDict.Clear();
        }


    }
}

