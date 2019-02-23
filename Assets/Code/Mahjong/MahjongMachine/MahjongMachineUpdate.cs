using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MahjongMachineNS
{
    public partial class MahjongMachine
    {
        #region 更新游戏状态
        public void UpdateGame()
        {
            if (isGameAllReady == false)
                return;

            if (isStopedGame)
                return;

            if (onBgMusic && !isMusicPlaying)
                PlayBgMusic();

            uiTouXiang.Update();
            diceMachine.Update();
            mjOpCmdList.Update();
            PlayerActionStateUpdate();
            uiPcghtBtnMgr.Update();
            uiScore.Update();

            uiSelectQueMen.Update();
            uiSelectSwapHandPai.Update();

            UpdateHighLightMjView();
            UpdateDapaiMjPointPos();

            mjAssetsMgr.RecoverParticles();
        }

        #endregion

        void PlayerActionStateUpdate()
        {
            ActionXiPai();
            ActionFaPai();
            ActionQiDongDiceMachine();
            ActionSelectSwapPai();
            ActionSelectQueMen();
            ActionSelectPCGTHPai();
            ActionSelectPai();
            ActionSwapPaiHint();
            ActionSwapPai();
            ActionMoPai();
            ActionSelectDaPai();
            ActionDaPai();
            ActionChaPai();
            ActionSortPai();
            ActionPengChiGangPai();
            ActionHuPai();
            ActionBuHua();
            ActionTuiDaoPai();
        }


        /// <summary>
        /// 更新高亮麻将的显示
        /// </summary>
        void UpdateHighLightMjView()
        {
            if (playerStateData[0].selectedUpMj)
            {
                MahjongFaceValue selectedMjFaceValue = playerStateData[0].selectedUpMj.GetComponent<MjPaiData>().mjFaceValue;
                if (selectedMjFaceValue == highLightMjValue)
                    return;

                OffDeskMjHighLight(highLightMjValue);
                OnDeskMjHighLight(selectedMjFaceValue);
                return;
            }
            else if (mjSeatMoPaiLists[0].Count > 0)
            {
                MahjongFaceValue moPaiMjFaceValue = mjSeatMoPaiLists[0][0].GetComponent<MjPaiData>().mjFaceValue;
                if (moPaiMjFaceValue == highLightMjValue)
                    return;

                OffDeskMjHighLight(highLightMjValue);
                OnDeskMjHighLight(moPaiMjFaceValue);
                return;
            }
            else
            {
                OffDeskMjHighLight(highLightMjValue);
            }
        }


        /// <summary>
        /// 更新打出麻将的指示位置
        /// </summary>
        void UpdateDapaiMjPointPos()
        {
            if (lastDaPaiMj)
                mjPoint.Show(lastDaPaiMj);
            else
                mjPoint.Hide();
        }

    }
}