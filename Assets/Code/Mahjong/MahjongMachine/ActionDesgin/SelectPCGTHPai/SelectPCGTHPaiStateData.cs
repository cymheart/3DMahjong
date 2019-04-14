using ComponentDesgin;
using CoreDesgin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ActionDesgin
{
    public class SelectPCGTHPaiStateData : ActionStateData
    {
        /// <summary>
        /// 选择碰吃杠听胡牌开始
        /// </summary>
        public const int SELECT_PCGTH_PAI_START = 82;
        public const int SELECT_PCGTH_PAI_SELECTTING = 83;
        public const int SELECT_PCGTH_PAI_SELECTTING_CHIPAI = 84;
        public const int SELECT_PCGTH_PAI_SELECT_TING_PAI_START = 85;
        public const int SELECT_PCGTH_PAI_SELECT_TING_PAI_READY_CLICK = 86;
        public const int SELECT_PCGTH_PAI_SELECT_TING_PAIING = 87;
        public const int SELECT_PCGTH_PAI_SELECT_TING_PAI_RESTORE = 88;
        public const int SELECT_PCGTH_PAI_SELECT_TING_PAI_SELECT_END = 89;

        /// <summary>
        /// 选择碰吃杠听胡牌结束
        /// </summary>
        public const int SELECT_PCGTH_PAI_END = 90;



        #region 选择碰吃杠听胡牌动作数据
        public PengChiGangTingHuType[] selectPcgthBtnTypes;
        public List<MahjongFaceValue[]> selectPcgthChiPaiMjValueList = null;
        public PengChiGangTingHuType selectPcgthedType;
        public int selectPcgthedChiPaiMjValueIdx;

        public int[] selectPaiHuPaiInHandPaiIdxs;
        public List<HuPaiTipsInfo[]> selectPaiHuPaiInfosInHandPai;
        public int[] selectPaiHuPaiInMoPaiIdxs;
        public List<HuPaiTipsInfo[]> selectPaiHuPaiInfosInMoPai;

        public GameObject rayPickMj;
        public Vector3 rayPickMjMouseOrgPos;
        public Vector3 rayPickMjOrgPos;
        public float rayPickMjLastKickTime = -1000;
        public float rayPickMjMoveDistPreDuration = 0;
        public GameObject selectPaiRayPickMj;
        public GameObject selectedUpMj;
        public int selectPaiHandPaiIdx;
        public HandPaiType selectPaiType;
        #endregion

        /// <summary>
        /// 设置选择碰吃杠听胡牌动作数据
        /// </summary>
        /// <param name="pcgthBtnTypes"></param>
        /// <param name="chiPaiMjValueList"></param>
        /// <param name="selectPaiHuPaiInHandPaiIdxs"></param>
        /// <param name="selectPaiHuPaiInfosInHandPai"></param>
        /// <param name="selectPaiHuPaiInMoPaiIdxs"></param>
        /// <param name="selectPaiHuPaiInfosInMoPai"></param>
        /// <param name="opCmdNode"></param>
        public void SetSelectPCGTHPaiData(
            PengChiGangTingHuType[] pcgthBtnTypes,
            List<MahjongFaceValue[]> chiPaiMjValueList,
            int[] selectPaiHuPaiInHandPaiIdxs, List<HuPaiTipsInfo[]> selectPaiHuPaiInfosInHandPai,
            int[] selectPaiHuPaiInMoPaiIdxs, List<HuPaiTipsInfo[]> selectPaiHuPaiInfosInMoPai,
            LinkedListNode<MahjongMachineCmd> opCmdNode = null)
        {
            selectPcgthBtnTypes = pcgthBtnTypes;
            selectPcgthChiPaiMjValueList = chiPaiMjValueList;
            this.selectPaiHuPaiInHandPaiIdxs = selectPaiHuPaiInHandPaiIdxs;
            this.selectPaiHuPaiInfosInHandPai = selectPaiHuPaiInfosInHandPai;
            this.selectPaiHuPaiInMoPaiIdxs = selectPaiHuPaiInMoPaiIdxs;
            this.selectPaiHuPaiInfosInMoPai = selectPaiHuPaiInfosInMoPai;
            this.opCmdNode = opCmdNode;
        }


    }
}
