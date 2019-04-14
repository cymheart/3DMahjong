using CoreDesgin;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using ComponentDesgin;

namespace ActionDesgin
{

    /// <summary>
    /// 整理牌动作
    /// </summary>
    public class SortPaiAction : BaseHandAction
    {
        private static SortPaiAction instance = null;
        public static SortPaiAction Instance
        {
            get
            {
                if (instance == null)
                    instance = new SortPaiAction();
                return instance;
            }
        }

        public override void Install()
        {
            mjMachineUpdater.Reg("SortPai", ActionSortPai);
        }

        public override void UnInstall()
        {
            mjMachineUpdater.UnReg("SortPai");
        }

        #region 整理牌动作

        /// <summary>
        /// 整理牌
        /// </summary>
        /// <param name="seatIdx">对应的玩家座号</param>
        public void SortPai(int seatIdx, SortPaiType sortPaiType = SortPaiType.LEFT, LinkedListNode<MahjongMachineCmd> opCmdNode = null)
        {
            StopSelectPaiActionState(seatIdx);

            if (playerStateData[seatIdx].state != StateDataGroup.END ||
                desk.mjSeatHandPaiLists[seatIdx].Count == 0 || desk.mjSeatHandPaiLists[seatIdx][0] == null)
            {
                mjCmdMgr.RemoveCmd(opCmdNode);
                return;
            }

            SortPaiStateData stateData = playerStateData[seatIdx].GetComponent<SortPaiStateData>();

            stateData.SetSortPaiData(sortPaiType, opCmdNode);
            playerStateData[seatIdx].SetState(SortPaiStateData.SORT_PAI_START, Time.time, -1);
        }

        /// <summary>
        /// 整理牌动作
        /// </summary>
        /// <param name="seatIdx">对应的玩家座号</param>
        /// <returns></returns>
        public void ActionSortPai()
        {
            for (int i = 0; i < Fit.playerCount; i++)
            {
                if (fit.isCanUseSeatPlayer[i])
                    ActionSortPai(i);
            }
        }
        void ActionSortPai(int seatIdx)
        {
            if (playerStateData[seatIdx].state < SortPaiStateData.SORT_PAI_START ||
             playerStateData[seatIdx].state > SortPaiStateData.SORT_PAI_END ||
             Time.time - playerStateData[seatIdx].stateStartTime < playerStateData[seatIdx].stateLiveTime)
            {
                return;
            }

            SortPaiStateData stateData = playerStateData[seatIdx].GetComponent<SortPaiStateData>();

            if (playerStateData[seatIdx].state == SortPaiStateData.SORT_PAI_END)
            {
                playerStateData[seatIdx].state = StateDataGroup.END;
                ProcessHandActionmjCmdMgr(seatIdx, stateData);
                return;
            }

            SortPaiType sortPaiType = stateData.sortPaiType;

            List<GameObject> mjHandPaiList = desk.mjSeatHandPaiLists[seatIdx];
            List<GameObject> mjMoPaiList = desk.mjSeatMoPaiLists[seatIdx];
            float mjSpacing = 0;
            float mjCount = mjHandPaiList.Count;
            Transform mjtf;
            float tm = 0.2f;
            Vector3 dstPos = new Vector3();

            switch (seatIdx)
            {
                case 0:
                    {
                        float mjsize = fit.GetCanvasHandMjSizeByAxis(Axis.X);
                        float mjWallLen = mjCount * mjsize + (mjCount - 1) * mjSpacing;
                        float mjStartPos = 0;
                        float mjAxisSpacing = mjsize + mjSpacing;

                        switch (sortPaiType)
                        {
                            case SortPaiType.LEFT:
                                mjStartPos = desk.mjSeatHandPaiPosLists[seatIdx][0].x;
                                break;

                            case SortPaiType.MIDDLE:
                                mjStartPos = -mjWallLen / 2 + mjsize / 2;
                                break;
                        }


                        dstPos = new Vector3(mjStartPos, desk.mjSeatHandPaiPosLists[0][0].y, desk.mjSeatHandPaiPosLists[0][0].z);

                        for (int i = 0; i < mjCount; i++)
                        {
                            mjtf = mjHandPaiList[i].transform;
                            dstPos = new Vector3(mjStartPos + i * mjAxisSpacing, mjtf.localPosition.y, mjtf.localPosition.z);
                            mjtf.DOLocalMove(dstPos, tm);
                        }

                        if (mjCount > 0)
                            dstPos.x += mjsize + fit.moPaiToHandPaiCanvasOffset;

                        if (mjMoPaiList.Count > 0)
                        {
                            Vector3 offset = dstPos - mjMoPaiList[0].transform.localPosition;

                            for (int i = 0; i < mjMoPaiList.Count; i++)
                            {
                                mjtf = mjMoPaiList[i].transform;
                                dstPos = mjtf.transform.localPosition + offset;
                                mjtf.DOLocalMove(dstPos, tm);
                            }
                        }
                    }
                    break;

                case 1:
                    {
                        float mjsize = fit.GetHandMjSizeByAxis(Axis.X);
                        float mjWallLen = mjCount * mjsize + (mjCount - 1) * mjSpacing;
                        float mjStartPos = 0;
                        float mjAxisSpacing = mjsize + mjSpacing;

                        switch (sortPaiType)
                        {
                            case SortPaiType.LEFT:
                                mjStartPos = desk.mjSeatHandPaiPosLists[seatIdx][0].z;
                                break;

                            case SortPaiType.MIDDLE:
                                mjStartPos = -mjWallLen / 2 + mjsize / 2 + desk.mjtableTransform.transform.position.z;
                                break;
                        }


                        dstPos = new Vector3(desk.mjSeatHandPaiPosLists[1][0].x, desk.mjSeatHandPaiPosLists[1][0].y, mjStartPos);

                        for (int i = 0; i < mjCount; i++)
                        {
                            mjtf = mjHandPaiList[i].transform;
                            dstPos = new Vector3(mjtf.position.x, mjtf.position.y, mjStartPos + i * mjAxisSpacing);
                            mjtf.DOMove(dstPos, tm);
                        }

                        if (mjCount > 0)
                            dstPos.z += mjsize + fit.moPaiToHandPaiOffset;

                        if (mjMoPaiList.Count > 0)
                        {
                            Vector3 offset = dstPos - mjMoPaiList[0].transform.position;

                            for (int i = 0; i < mjMoPaiList.Count; i++)
                            {
                                mjtf = mjMoPaiList[i].transform;
                                dstPos = mjtf.transform.position + offset;
                                mjtf.DOMove(dstPos, tm);
                            }
                        }
                    }
                    break;


                case 2:
                    {
                        float mjsize = fit.GetHandMjSizeByAxis(Axis.X);
                        float mjWallLen = mjCount * mjsize + (mjCount - 1) * mjSpacing;
                        float mjStartPos = 0;
                        float mjAxisSpacing = mjsize + mjSpacing;

                        switch (sortPaiType)
                        {
                            case SortPaiType.LEFT:
                                mjStartPos = desk.mjSeatHandPaiPosLists[seatIdx][0].x;
                                break;

                            case SortPaiType.MIDDLE:
                                mjStartPos = -mjWallLen / 2 + mjsize / 2 + desk.mjtableTransform.transform.position.x;
                                break;
                        }

                        dstPos = new Vector3(desk.mjSeatHandPaiPosLists[2][0].x, desk.mjSeatHandPaiPosLists[2][0].y, mjStartPos);

                        for (int i = 0; i < mjCount; i++)
                        {
                            mjtf = mjHandPaiList[i].transform;
                            dstPos = new Vector3(mjStartPos + i * mjAxisSpacing, mjtf.position.y, mjtf.position.z);
                            mjtf.DOMove(dstPos, tm);
                        }

                        if (mjCount > 0)
                            dstPos.x += mjsize + fit.moPaiToHandPaiOffset;

                        if (mjMoPaiList.Count > 0)
                        {
                            Vector3 offset = dstPos - mjMoPaiList[0].transform.position;

                            for (int i = 0; i < mjMoPaiList.Count; i++)
                            {
                                mjtf = mjMoPaiList[i].transform;
                                dstPos = mjtf.transform.position + offset;
                                mjtf.DOMove(dstPos, tm);
                            }
                        }
                    }
                    break;


                case 3:
                    {
                        float mjsize = fit.GetHandMjSizeByAxis(Axis.X);
                        float mjWallLen = mjCount * mjsize + (mjCount - 1) * mjSpacing;
                        float mjStartPos = 0;
                        float mjAxisSpacing = mjsize + mjSpacing;

                        switch (sortPaiType)
                        {
                            case SortPaiType.LEFT:
                                mjStartPos = desk.mjSeatHandPaiPosLists[seatIdx][0].z;
                                break;

                            case SortPaiType.MIDDLE:
                                mjStartPos = mjWallLen / 2 - mjsize / 2 + desk.mjtableTransform.transform.position.z;
                                break;
                        }

                        dstPos = new Vector3(desk.mjSeatHandPaiPosLists[3][0].x, desk.mjSeatHandPaiPosLists[3][0].y, mjStartPos);

                        for (int i = 0; i < mjCount; i++)
                        {
                            mjtf = mjHandPaiList[i].transform;
                            dstPos = new Vector3(mjtf.position.x, mjtf.position.y, mjStartPos - i * mjAxisSpacing);
                            mjtf.DOMove(dstPos, tm);
                        }

                        if (mjCount > 0)
                            dstPos.z -= mjsize + fit.moPaiToHandPaiOffset;

                        if (mjMoPaiList.Count > 0)
                        {
                            Vector3 offset = dstPos - mjMoPaiList[0].transform.position;

                            for (int i = 0; i < mjMoPaiList.Count; i++)
                            {
                                mjtf = mjMoPaiList[i].transform;
                                dstPos = mjtf.transform.position + offset;
                                mjtf.DOMove(dstPos, tm);
                            }
                        }
                    }

                    break;

            }

            playerStateData[seatIdx].SetState(SortPaiStateData.SORT_PAI_END, Time.time, tm);
        }

        #endregion

    }
}
