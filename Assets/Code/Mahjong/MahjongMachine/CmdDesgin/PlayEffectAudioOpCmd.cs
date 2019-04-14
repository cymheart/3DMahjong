using ActionDesgin;
using ComponentDesgin;
using CoreDesgin;
using System.Collections.Generic;

namespace CmdDesgin
{
    /// <summary>
    /// 播放音效命令
    /// </summary>
    public class PlayEffectAudioOpCmd : MahjongMachineCmd
    {
        public AudioIdx audioIdx = AudioIdx.AUDIO_EFFECT_DEAL;
        public int numIdx = 0;
        public PlayEffectAudioOpCmd()
        {
        }

        public static void InitCmd(MahjongMachineCmd cmd)
        {

        }

        public override void Execute(LinkedListNode<MahjongMachineCmd> opCmdNode = null)
        {
            mjMachine.GetComponent<Audio>().PlayEffectAudio(audioIdx, numIdx);
            cmdList.RemoveCmd(opCmdNode);
        }


    }
}
