using ComponentDesgin;
using CoreDesgin;
using System.Collections.Generic;
using UnityEngine;

namespace ActionDesgin
{
    public class SelectDaPaiStateData: ActionStateData
    {

        #region 选择打出麻将动作数据
        public GameObject rayPickMj;
        public Vector3 rayPickMjMouseOrgPos;
        public Vector3 rayPickMjOrgPos;
        public float rayPickMjLastKickTime = -1000;
        public float rayPickMjMoveDistPreDuration = 0;
        public GameObject selectPaiRayPickMj;
        public GameObject selectedUpMj;

        public int[] selectPaiHuPaiInHandPaiIdxs;
        public List<HuPaiTipsInfo[]> selectPaiHuPaiInfosInHandPai;
        public int[] selectPaiHuPaiInMoPaiIdxs;
        public List<HuPaiTipsInfo[]> selectPaiHuPaiInfosInMoPai;
        public int selectPaiHandPaiIdx;
        public HandPaiType selectPaiType;

        /// <summary>
        /// 射线点击拾取的麻将位置号列表
        /// </summary>
        public List<int> rayClickPickHandPaiMjIdxList = new List<int>();
        public List<int> rayClickPickMoPaiMjIdxList = new List<int>();
        #endregion


        /// <summary>
        /// 设置选择出牌动作数据
        /// </summary>
        /// <param name="selectPaiHuPaiInHandPaiIdxs"></param>
        /// <param name="selectPaiHuPaiInfosInHandPai"></param>
        /// <param name="selectPaiHuPaiInMoPaiIdxs"></param>
        /// <param name="selectPaiHuPaiInfosInMoPai"></param>
        /// <param name="opCmdNode"></param>
        public void SetSelectDaPaiData(
            int[] selectPaiHuPaiInHandPaiIdxs, List<HuPaiTipsInfo[]> selectPaiHuPaiInfosInHandPai,
            int[] selectPaiHuPaiInMoPaiIdxs, List<HuPaiTipsInfo[]> selectPaiHuPaiInfosInMoPai,
            LinkedListNode<MahjongMachineCmd> opCmdNode = null)
        {
            this.selectPaiHuPaiInHandPaiIdxs = selectPaiHuPaiInHandPaiIdxs;
            this.selectPaiHuPaiInfosInHandPai = selectPaiHuPaiInfosInHandPai;
            this.selectPaiHuPaiInMoPaiIdxs = selectPaiHuPaiInMoPaiIdxs;
            this.selectPaiHuPaiInfosInMoPai = selectPaiHuPaiInfosInMoPai;
            this.opCmdNode = opCmdNode;
        }

    }
}
