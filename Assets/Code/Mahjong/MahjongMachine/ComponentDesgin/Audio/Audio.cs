using CoreDesgin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ComponentDesgin
{
    public class Audio: MahjongMachineComponent
    {
        public MahjongMachine mjMachine;
        AudioSource bgMusicAudioSource;
        MahjongAssetsMgr mjAssetsMgr;
        SettingDataAssetsMgr settingDataAssetsMgr;
        Scene scene;

        /// <summary>
        /// 声音类型
        /// </summary>
        string voiceType = "PT_SPEAK";
        string[] speakTypes;
        Dictionary<int, AudioClip[]>[] speakAudiosDicts;
        Dictionary<int, AudioClip[]> musicAudiosDict;
        Dictionary<int, AudioClip[]> effectAudiosDict;

        /// <summary>
        /// 是否开启人声
        /// </summary>
        public bool onVoice = true;

        /// <summary>
        /// 是否开启背景音乐
        /// </summary>
        bool onBgMusic = true;
        bool isMusicPlaying = false;

        public override void Init()
        {
            Setting((MahjongMachine)Parent);
        }

        public void Setting(MahjongMachine mjMachine)
        {
            this.mjMachine = mjMachine;
            mjAssetsMgr = mjMachine.GetComponent<MahjongAssetsMgr>();
            scene = mjMachine.GetComponent<Scene>();
            settingDataAssetsMgr = mjMachine.GetComponent<SettingDataAssetsMgr>();

            CreateAudios();
        }

        public override void Load()
        {
            base.Load();

            bgMusicAudioSource = scene.cameraTransform.GetComponent<AudioSource>();
        }


        void CreateAudios()
        {
            speakTypes = settingDataAssetsMgr.parseTextSetting.GetStringGroup("SPEAK");

            speakAudiosDicts = new Dictionary<int, AudioClip[]>[(int)PlayerType.NONE];
            musicAudiosDict = settingDataAssetsMgr.GetAudiosDict("MUSIC_AUDIO");
            effectAudiosDict = settingDataAssetsMgr.GetAudiosDict("EFFECT_AUDIO");

            SetSpeakAudioDict(voiceType);
        }


        public override void ClearData()
        {
            voiceType = "PT_SPEAK";
            speakTypes = null;
        }

        public void SetSpeakAudioDict(string audioType)
        {
            for (int i = 0; i < (int)PlayerType.NONE; i++)
            {
                speakAudiosDicts[i] = settingDataAssetsMgr.GetAudiosDict(audioType, i);
            }
        }


        /// <summary>
        /// 播放说话声
        /// </summary>
        /// <param name="playerType"></param>
        /// <param name="audioIdx"></param>
        /// <param name="idx"></param>
        public void PlaySpeakAudio(PlayerType playerType, AudioIdx audioIdx, int idx = 0)
        {
            AudioClip clip = GetSpeakAudio(playerType, (int)audioIdx, idx);
            if (clip != null)
                AudioSource.PlayClipAtPoint(clip, scene.cameraTransform.position);
        }

        /// <summary>
        /// 播放说话声
        /// </summary>
        public void PlaySpeakAudio(PlayerType playerType, MahjongFaceValue mjFaceValue, Vector3 playPos, int idx = 0)
        {
            AudioClip clip = GetSpeakAudio(playerType, (int)mjFaceValue, idx);
            if (clip != null)
                AudioSource.PlayClipAtPoint(clip, playPos);
        }

        /// <summary>
        /// 播放说话声
        /// </summary>
        public void PlaySpeakAudio(PlayerType playerType, AudioIdx audioIdx, Vector3 playPos, int idx = 0)
        {
            AudioClip clip = GetSpeakAudio(playerType, (int)audioIdx, idx);
            if (clip != null)
                AudioSource.PlayClipAtPoint(clip, playPos);
        }


        public AudioClip GetSpeakAudio(PlayerType playerType, int audioIdx, int idx = 0)
        {
            Dictionary<int, AudioClip[]> audioClipDict = speakAudiosDicts[(int)playerType];

            if (audioClipDict == null || !audioClipDict.ContainsKey(audioIdx))
                return null;

            return settingDataAssetsMgr.GetAudio(audioClipDict, audioIdx, idx);
        }

        public AudioClip GetMusicAudio(int audioIdx, int idx = 0)
        {
            return settingDataAssetsMgr.GetAudio(musicAudiosDict, audioIdx, idx);
        }


        /// <summary>
        /// 播放音效
        /// </summary>
        /// <param name="playerType"></param>
        /// <param name="audioIdx"></param>
        /// <param name="idx"></param>
        public void PlayEffectAudio(AudioIdx audioIdx, int idx = 0)
        {
            AudioClip clip = GetEffectAudio((int)audioIdx, idx);
            AudioSource.PlayClipAtPoint(clip, scene.cameraTransform.position);
        }

        public AudioClip GetEffectAudio(int audioIdx, int idx = 0)
        {
            return settingDataAssetsMgr.GetAudio(effectAudiosDict, audioIdx, idx);
        }


        public void PlayBgMusic()
        {
            bgMusicAudioSource.clip = GetMusicAudio((int)AudioIdx.AUDIO_BG_MUSIC, 1);
            bgMusicAudioSource.Play();
            isMusicPlaying = true;
        }

        public void OnBgMusic()
        {
            onBgMusic = true;
        }

        public void OffBgMusic()
        {
            bgMusicAudioSource.Stop();
            isMusicPlaying = false;
            onBgMusic = false;
        }
    }
}
