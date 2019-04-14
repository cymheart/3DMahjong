using CoreDesgin;
using System.Collections.Generic;
using UnityEngine;

namespace ComponentDesgin
{
    public partial class Desk
    {
        public override void Init()
        {
            base.Init();
            Setting((MahjongMachine)Parent);
        }

        public void Setting(MahjongMachine mjMachine)
        {
            this.mjMachine = mjMachine;
            mjAssetsMgr = mjMachine.GetComponent<MahjongAssetsMgr>();
            fit = mjMachine.GetComponent<Fit>();
            scene = mjMachine.GetComponent<Scene>();
            mjtableTransform = scene.mjtableTransform;

            canvasHandPaiTransform = scene.canvasHandPaiTransform;
            canvasHandPaiRectTransform = canvasHandPaiTransform.GetComponent<RectTransform>();
        }

        public override void Load()
        {
            base.Load();     
            CreateInitLayoutPosForSeats();
            CreateMahjongPaiDui();
        }

        public override void Destory()
        {
            base.Destory();

            for (int i = 0; i < mjDuiPai.Length; i++)
                Object.Destroy(mjDuiPai[i]);
        }

        public override void ClearData()
        {
            highLightMjValue = MahjongFaceValue.MJ_UNKNOWN;

            ClearMjMoPaiList();
            ClearMjHandPaiList();
            ClearDeskGlobalMjPaiSetDict();

            for (int i = 0; i < curtDaPaiMjSeatDeskPosIdx.Length; i++)
            {
                curtDaPaiMjSeatDeskPosIdx[i] = new Vector3Int(0, -1, 0);
            };

            for (int i = 0; i < curtHuPaiMjSeatDeskPosIdx.Length; i++)
            {
                curtHuPaiMjSeatDeskPosIdx[i] = -1;
            };

            ShowOrHideMjDuiPai(true);

            for (int i = 0; i < deskPengPaiMjPosInfoList.Length; i++)
            {
                deskPengPaiMjPosInfoList[i].Clear();
            }

            ClearMjDicts(deskDaPaiMjDicts);
            ClearMjDicts(deskHuPaiMjDicts);

            ClearMjLists(deskPengPaiMjList);
            ClearMjLists(deskGangPaiMjList);
            ClearMjLists(deskChiPaiMjList);
        }


        void ClearMjDicts(Dictionary<int, GameObject>[] mjDicts)
        {
            GameObject go;
            Dictionary<int, GameObject> dict;
            for (int i = 0; i < mjDicts.Length; i++)
            {
                dict = mjDicts[i];
                foreach (var item in dict)
                {
                    go = item.Value;
                    if (go != null)
                    {
                        mjAssetsMgr.PushMjToDeskOrSelfHandMjPool(go);
                    }
                }

                dict.Clear();
            }
        }

        void ClearMjLists(List<GameObject[]>[] mjLists)
        {
            for (int i = 0; i < mjLists.Length; i++)
            {
                for (int j = 0; j < mjLists[i].Count; j++)
                {
                    for (int k = 0; k < mjLists[i][j].Length; k++)
                        mjAssetsMgr.PushMjToDeskOrSelfHandMjPool(mjLists[i][j][k]);
                }

                mjLists[i].Clear();
            }
        }


        public void Destory()
        {
            for (int i = 0; i < mjDuiPai.Length; i++)
                Object.Destroy(mjDuiPai[i]);
        }

        void CreateInitLayoutPosForSeats()
        {
            CreateMjHandPaiPosList(fit.mjHandCount);
            CreateMjDeskDaPaiPosForSeats();
            CreateMjDeskHuPaiPosForSeats();
        }

        #region 手牌列表相关功能

        /// <summary>
        /// 清理麻将列表数据
        /// </summary>
        /// <param name="mjPaiList"></param>
        /// <param name="type"></param>
        void ClearMjPaiList(List<GameObject> mjPaiList, int type = 0)
        {
            for (int j = 0; j < mjPaiList.Count; j++)
            {
                if (mjPaiList[j] != null)
                {
                    if (type == 0)
                    {
                        mjAssetsMgr.PushMjToDeskOrSelfHandMjPool(mjPaiList[j]);
                    }
                    else
                    {
                        mjAssetsMgr.PushMjToOtherHandMjPool(mjPaiList[j]);
                    }
                }
            }
        }

        /// <summary>
        /// 清除手牌麻将列表数据
        /// </summary>
        void ClearMjHandPaiList()
        {
            for (int i = 0; i < mjSeatHandPaiLists.Length; i++)
            {
                ClearMjPaiList(mjSeatHandPaiLists[i], i);
                mjSeatHandPaiLists[i].Clear();
            }
        }

        /// <summary>
        /// 清除摸牌麻将列表数据
        /// </summary>
        void ClearMjMoPaiList()
        {
            for (int i = 0; i < mjSeatMoPaiLists.Length; i++)
            {
                ClearMjPaiList(mjSeatMoPaiLists[i], i);
                mjSeatMoPaiLists[i].Clear();
            }
        }

        /// <summary>
        /// 根据指定麻将值获取手牌中相对应的麻将的数量
        /// </summary>
        /// <param name="mjFaceValue"></param>
        /// <returns></returns>
        public int GetHandPaiCountForMjFaceValue(MahjongFaceValue mjFaceValue)
        {
            MahjongFaceValue curtFaceValue;
            int count = 0;

            for (int i = 0; i < mjSeatHandPaiLists[0].Count; i++)
            {
                curtFaceValue = mjSeatHandPaiLists[0][i].GetComponent<MjPaiData>().mjFaceValue;

                if (curtFaceValue == mjFaceValue)
                    count++;
            }

            return count;
        }

        /// <summary>
        ///  根据指定麻将值获取摸牌中相对应的麻将的数量
        /// </summary>
        /// <param name="mjFaceValue"></param>
        /// <returns></returns>
        public int GetMoPaiCountForMjFaceValue(MahjongFaceValue mjFaceValue)
        {
            MahjongFaceValue curtFaceValue;
            int count = 0;

            for (int i = 0; i < mjSeatMoPaiLists[0].Count; i++)
            {
                curtFaceValue = mjSeatMoPaiLists[0][i].GetComponent<MjPaiData>().mjFaceValue;

                if (curtFaceValue == mjFaceValue)
                    count++;
            }

            return count;
        }

        /// <summary>
        /// 获取手牌麻将列表
        /// </summary>
        /// <param name="seatIdx"></param>
        /// <returns></returns>
        public List<GameObject> GetHandPaiList(int seatIdx)
        {
            return mjSeatHandPaiLists[seatIdx];
        }

        /// <summary>
        /// 获取手牌麻将的面值
        /// </summary>
        /// <param name="seatIdx"></param>
        /// <param name="paiIdx"></param>
        /// <returns></returns>
        public MahjongFaceValue GetHandPaiMjFaceValue(int seatIdx, int paiIdx)
        {
            return GetHandPaiList(seatIdx)[paiIdx].GetComponent<MjPaiData>().mjFaceValue;
        }


        /// <summary>
        /// 获取摸牌麻将列表
        /// </summary>
        /// <param name="seatIdx"></param>
        /// <returns></returns>
        public List<GameObject> GetMoPaiList(int seatIdx)
        {
            return mjSeatMoPaiLists[seatIdx];
        }

        /// <summary>
        /// 获取麻将在手牌列表中的编号位置
        /// </summary>
        /// <param name="seatIdx"></param>
        /// <param name="mj"></param>
        /// <returns></returns>
        public int GetPaiIdxInHandPaiList(int seatIdx, GameObject mj)
        {
            return mjSeatHandPaiLists[seatIdx].IndexOf(mj);
        }


        /// <summary>
        /// 获取麻将在摸牌列表中的编号位置
        /// </summary>
        /// <param name="seatIdx"></param>
        /// <param name="mj"></param>
        /// <returns></returns>
        public int GetPaiIdxInMoPaiList(int seatIdx, GameObject mj)
        {
            return mjSeatMoPaiLists[seatIdx].IndexOf(mj);
        }


        /// <summary>
        /// 获取手牌列表中最后一张牌的idx
        /// </summary>
        /// <param name="seatIdx"></param>
        /// <returns></returns>
        public int GetHandPaiListLastPaiIdx(int seatIdx)
        {
            GameObject mj;
            for (int i = mjSeatHandPaiLists[seatIdx].Count - 1; i >= 0; i--)
            {
                mj = mjSeatHandPaiLists[seatIdx][i];
                if (mj != null)
                    return i;
            }

            return -1;
        }

        int GetEmptyHandPaiIdx(int seatIdx)
        {
            for (int i = 0; i < mjSeatHandPaiLists[seatIdx].Count; i++)
            {
                if (mjSeatHandPaiLists[seatIdx][i] == null)
                    return i;
            }

            return -1;
        }

        void EmptyHandPaiIdx(int seatIdx)
        {
            for (int i = 0; i < mjSeatHandPaiLists[seatIdx].Count; i++)
            {
                mjSeatHandPaiLists[seatIdx][i] = null;
            }
        }

        #endregion

        #region 生成桌面能摆放的打出牌的所有麻将位置
        /// <summary>
        /// 生成桌面能摆放的打出牌的所有麻将位置
        /// </summary>
        void CreateMjDeskDaPaiPosForSeats()
        {
            float spacing = 0.0005f;

            int maxMjCount = GetDaChuMjRowMaxMjCount();
            CreateMjDeskPaiPosSeat0(fit.deskRowMjLayoutCounts, 0.09f, spacing, spacing);
            CreateMjDeskPaiPosSeat1(fit.deskRowMjLayoutCounts, 0.11f, spacing, spacing);
            CreateMjDeskPaiPosSeat2(fit.deskRowMjLayoutCounts, 0.09f, spacing, spacing);
            CreateMjDeskPaiPosSeat3(fit.deskRowMjLayoutCounts, 0.11f, spacing, spacing);
        }

        /// <summary>
        /// 生成麻将在桌面的位置
        /// </summary>
        /// <param name="rowMjLayoutCounts">桌面麻将数量布局</param>
        /// <param name="posOffset">离桌面中心位置的距离</param>
        /// <param name="mjHorSpacing">麻将间的水平间隔</param>
        /// <param name="mjVerSpacing">麻将间的垂直间隔</param>
        void CreateMjDeskPaiPosSeat0(int[] rowMjLayoutCounts, float posOffset, float mjHorSpacing, float mjVerSpacing)
        {
            List<List<Vector3>> mjSeatDeskPaiPosList = mjSeatDeskPaiPosLists[0];
            float mjWidth = fit.GetDeskMjSizeByAxis(Axis.X);
            float mjHeight = fit.GetDeskMjSizeByAxis(Axis.Y);

            float rowMjTotalWidth;
            float rowMjPosZ, rowMjPosX, rowMjPosY;
            List<Vector3> rowMjList;

            rowMjPosY = deskFacePosY + fit.GetDeskMjSizeByAxis(Axis.Z) / 2 + 0.0002f;

            for (int i = 0; i < rowMjLayoutCounts.Length; i++)
            {
                rowMjTotalWidth = rowMjLayoutCounts[i] * mjWidth + (rowMjLayoutCounts[i] - 1) * mjHorSpacing;
                rowMjPosX = rowMjTotalWidth / 2 - mjWidth / 2;
                rowMjPosZ = posOffset + i * (mjHeight + mjVerSpacing) + mjHeight / 2;

                rowMjList = new List<Vector3>();

                for (int j = 0; j < rowMjLayoutCounts[i]; j++)
                {
                    rowMjList.Add(new Vector3(rowMjPosX, rowMjPosY, rowMjPosZ));
                    rowMjPosX -= mjWidth + mjHorSpacing;
                }

                mjSeatDeskPaiPosList.Add(rowMjList);
            }
        }
        void CreateMjDeskPaiPosSeat1(int[] rowMjLayoutCounts, float posOffset, float mjHorSpacing, float mjVerSpacing)
        {
            List<List<Vector3>> mjSeatDeskPaiPosList = mjSeatDeskPaiPosLists[1];

            float mjWidth = fit.GetDeskMjSizeByAxis(Axis.X);
            float mjHeight = fit.GetDeskMjSizeByAxis(Axis.Y);

            float rowMjTotalWidth;
            float rowMjPosZ, rowMjPosX, rowMjPosY;
            List<Vector3> rowMjList;

            rowMjPosY = deskFacePosY + fit.GetDeskMjSizeByAxis(Axis.Z) / 2 + 0.0002f;

            for (int i = 0; i < rowMjLayoutCounts.Length; i++)
            {
                rowMjTotalWidth = rowMjLayoutCounts[i] * mjWidth + (rowMjLayoutCounts[i] - 1) * mjHorSpacing;
                rowMjPosZ = -rowMjTotalWidth / 2 + mjWidth / 2;
                rowMjPosX = posOffset + i * (mjHeight + mjVerSpacing) + mjHeight / 2;

                rowMjList = new List<Vector3>();

                for (int j = 0; j < rowMjLayoutCounts[i]; j++)
                {
                    rowMjList.Add(new Vector3(rowMjPosX, rowMjPosY, rowMjPosZ));
                    rowMjPosZ += mjWidth + mjHorSpacing;
                }

                mjSeatDeskPaiPosList.Add(rowMjList);
            }
        }
        void CreateMjDeskPaiPosSeat2(int[] rowMjLayoutCounts, float posOffset, float mjHorSpacing, float mjVerSpacing)
        {
            List<List<Vector3>> mjSeatDeskPaiPosList = mjSeatDeskPaiPosLists[2];

            float mjWidth = fit.GetDeskMjSizeByAxis(Axis.X);
            float mjHeight = fit.GetDeskMjSizeByAxis(Axis.Y);

            float rowMjTotalWidth;
            float rowMjPosZ, rowMjPosX, rowMjPosY;
            List<Vector3> rowMjList;

            rowMjPosY = deskFacePosY + fit.GetDeskMjSizeByAxis(Axis.Z) / 2 + 0.0002f;

            for (int i = 0; i < rowMjLayoutCounts.Length; i++)
            {
                rowMjTotalWidth = rowMjLayoutCounts[i] * mjWidth + (rowMjLayoutCounts[i] - 1) * mjHorSpacing;
                rowMjPosX = -rowMjTotalWidth / 2 + mjWidth / 2;
                rowMjPosZ = -posOffset - i * (mjHeight + mjVerSpacing) - mjHeight / 2;

                rowMjList = new List<Vector3>();

                for (int j = 0; j < rowMjLayoutCounts[i]; j++)
                {
                    rowMjList.Add(new Vector3(rowMjPosX, rowMjPosY, rowMjPosZ));
                    rowMjPosX += mjWidth + mjHorSpacing;
                }

                mjSeatDeskPaiPosList.Add(rowMjList);
            }
        }
        void CreateMjDeskPaiPosSeat3(int[] rowMjLayoutCounts, float posOffset, float mjHorSpacing, float mjVerSpacing)
        {
            List<List<Vector3>> mjSeatDeskPaiPosList = mjSeatDeskPaiPosLists[3];

            float mjWidth = fit.GetDeskMjSizeByAxis(Axis.X);
            float mjHeight = fit.GetDeskMjSizeByAxis(Axis.Y);

            float rowMjTotalWidth;
            float rowMjPosZ, rowMjPosX, rowMjPosY;
            List<Vector3> rowMjList;

            rowMjPosY = deskFacePosY + fit.GetDeskMjSizeByAxis(Axis.Z) / 2 + 0.0002f;

            for (int i = 0; i < rowMjLayoutCounts.Length; i++)
            {
                rowMjTotalWidth = rowMjLayoutCounts[i] * mjWidth + (rowMjLayoutCounts[i] - 1) * mjHorSpacing;
                rowMjPosZ = rowMjTotalWidth / 2 - mjWidth / 2;
                rowMjPosX = -posOffset - i * (mjHeight + mjVerSpacing) - mjHeight / 2;

                rowMjList = new List<Vector3>();

                for (int j = 0; j < rowMjLayoutCounts[i]; j++)
                {
                    rowMjList.Add(new Vector3(rowMjPosX, rowMjPosY, rowMjPosZ));
                    rowMjPosZ -= mjWidth + mjHorSpacing;
                }

                mjSeatDeskPaiPosList.Add(rowMjList);
            }
        }


        /// <summary>
        /// 获取打出麻将可摆放行的最大麻将个数
        /// </summary>
        /// <returns></returns>
        public int GetDaChuMjRowMaxMjCount()
        {
            int maxMjCount = 0;
            for (int i = 0; i < fit.deskRowMjLayoutCounts.Length; i++)
            {
                if (fit.deskRowMjLayoutCounts[i] > maxMjCount)
                    maxMjCount = fit.deskRowMjLayoutCounts[i];
            }

            return maxMjCount;
        }


        /// <summary>
        /// 获取打出麻将所在的桌面位置
        /// </summary>
        /// <param name="seat"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public Vector3 GetMjDeskPaiPos(int seat, int row, int col)
        {
            return mjSeatDeskPaiPosLists[seat][row][col];
        }

        /// <summary>
        /// 指向下一个桌面打牌麻将位置
        /// </summary>
        /// <param name="seatIdx"></param>
        /// <returns></returns>
        public Vector3 NextDeskMjPos(int seatIdx)
        {
            List<List<Vector3>> mjSeatDeskPaiPosList = mjSeatDeskPaiPosLists[seatIdx];
            curtDaPaiMjSeatDeskPosIdx[seatIdx] = GetNextDeskMjPosIdx(seatIdx);

            float mjHeight = fit.GetDeskMjSizeByAxis(Axis.Z);
            Vector3 mjPos = mjSeatDeskPaiPosList[curtDaPaiMjSeatDeskPosIdx[seatIdx].x][curtDaPaiMjSeatDeskPosIdx[seatIdx].y];
            mjPos.y += mjHeight * curtDaPaiMjSeatDeskPosIdx[seatIdx].z;
            return mjPos;
        }


        /// <summary>
        /// 指向上一个桌面打牌麻将位置（再次出牌时将会从当前麻将位置往后摆牌）
        /// </summary>
        /// <param name="seatIdx"></param>
        /// <returns></returns>
        public Vector3 PrevDeskMjPos(int seatIdx)
        {
            List<List<Vector3>> mjSeatDeskPaiPosList = mjSeatDeskPaiPosLists[seatIdx];
            curtDaPaiMjSeatDeskPosIdx[seatIdx] = GetPrevDeskMjPosIdx(seatIdx);

            if (curtDaPaiMjSeatDeskPosIdx[seatIdx].x < 0 || curtDaPaiMjSeatDeskPosIdx[seatIdx].y < 0)
                return Vector3.zero;

            float mjHeight = fit.GetDeskMjSizeByAxis(Axis.Z);
            Vector3 mjPos = mjSeatDeskPaiPosList[curtDaPaiMjSeatDeskPosIdx[seatIdx].x][curtDaPaiMjSeatDeskPosIdx[seatIdx].y];
            mjPos.y += mjHeight * curtDaPaiMjSeatDeskPosIdx[seatIdx].z;

            return mjPos;
        }


        /// <summary>
        /// 获取当前座位打牌麻将的最后桌面位置，以行列数据形式返回
        /// </summary>
        /// <param name="seatIdx"></param>
        /// <returns></returns>
        public Vector3Int GetCurtDeskMjPosIdx(int seatIdx)
        {
            return curtDaPaiMjSeatDeskPosIdx[seatIdx];
        }

        /// <summary>
        /// 获取当前座位打牌麻将最后打出牌的桌面位置,以行列层数据形式返回
        /// </summary>
        /// <param name="seatIdx"></param>
        /// <returns></returns>
        public Vector3Int GetNextDeskMjPosIdx(int seatIdx)
        {
            Vector3Int posIdx = curtDaPaiMjSeatDeskPosIdx[seatIdx];
            List<List<Vector3>> mjSeatDeskPaiPosList = mjSeatDeskPaiPosLists[seatIdx];

            int colIdx = posIdx.y;
            int colCount = mjSeatDeskPaiPosList[posIdx.x].Count;

            if (colIdx < colCount - 1)
            {
                colIdx++;
                return new Vector3Int(posIdx.x, colIdx, posIdx.z);
            }
            else if (posIdx.x < mjSeatDeskPaiPosList.Count - 1)
            {
                return new Vector3Int(posIdx.x + 1, 0, posIdx.z);
            }

            return new Vector3Int(0, 0, posIdx.z + 1);
        }


        /// <summary>
        /// 获取当前座位打牌麻将最后打出牌的桌面位置的前一个位置,以行列数据形式返回
        /// </summary>
        /// <param name="seatIdx"></param>
        /// <returns></returns>
        Vector3Int GetPrevDeskMjPosIdx(int seatIdx)
        {
            Vector3Int posIdx = curtDaPaiMjSeatDeskPosIdx[seatIdx];
            List<List<Vector3>> mjSeatDeskPaiPosList = mjSeatDeskPaiPosLists[seatIdx];

            int colIdx = posIdx.y;
            int colCount = mjSeatDeskPaiPosList[posIdx.x].Count;

            if (colIdx >= 0)
            {
                colIdx--;
                return new Vector3Int(posIdx.x, colIdx, posIdx.z);
            }
            else if (posIdx.x > 0)
            {
                colCount = mjSeatDeskPaiPosList[posIdx.x - 1].Count;
                return new Vector3Int(posIdx.x - 1, colCount - 1, posIdx.z);
            }
            else if (posIdx.z > 0)
            {
                colCount = mjSeatDeskPaiPosList[mjSeatDeskPaiPosList.Count - 1].Count;
                return new Vector3Int(mjSeatDeskPaiPosList.Count - 1, colCount - 1, posIdx.z - 1);
            }

            return new Vector3Int(0, -1, 0);
        }

        #endregion

        #region 生成桌面能摆放的胡牌麻将位置
        /// <summary>
        /// 生成桌面能摆放的胡牌麻将位置
        /// </summary>
        void CreateMjDeskHuPaiPosForSeats()
        {
            mjSeatDeskHuPaiPosList = new Vector3[4, huPaiDeskPosMjLayoutRowCount, huPaiDeskPosMjLayoutColCount];
            float mjSpacing = 0.001f;

            CreateMjDeskHuPaiPosSeat0(huPaiDeskPosMjLayoutRowCount, huPaiDeskPosMjLayoutColCount, new Vector3(-0.27f, 0, 0.299f), mjSpacing);
            CreateMjDeskHuPaiPosSeat1(huPaiDeskPosMjLayoutRowCount, huPaiDeskPosMjLayoutColCount, new Vector3(0.2952f, 0, 0.27f), mjSpacing);
            CreateMjDeskHuPaiPosSeat2(huPaiDeskPosMjLayoutRowCount, huPaiDeskPosMjLayoutColCount, new Vector3(0.27f, 0, -0.2867f), mjSpacing);
            CreateMjDeskHuPaiPosSeat3(huPaiDeskPosMjLayoutRowCount, huPaiDeskPosMjLayoutColCount, new Vector3(-0.2952f, 0, -0.27f), mjSpacing);
        }

        void CreateMjDeskHuPaiPosSeat0(int rowCount, int colCount, Vector3 startPos, float mjSpacing)
        {
            int seat = 0;
            float mjSizeX = fit.GetDeskMjSizeByAxis(Axis.X);
            float mjSizeY = fit.GetDeskMjSizeByAxis(Axis.Y);
            float x = startPos.x - mjSizeX / 2;
            float z = startPos.z;

            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < colCount; j++)
                {
                    mjSeatDeskHuPaiPosList[seat, i, j] = new Vector3(x, deskFacePosY, z);
                    x -= mjSizeX + mjSpacing;
                }

                x = startPos.x - mjSizeX / 2;
                z += mjSizeY;
            }
        }

        void CreateMjDeskHuPaiPosSeat1(int rowCount, int colCount, Vector3 startPos, float mjSpacing)
        {
            int seat = 1;
            float mjSizeX = fit.GetDeskMjSizeByAxis(Axis.X);
            float mjSizeY = fit.GetDeskMjSizeByAxis(Axis.Y);
            float z = startPos.z + mjSizeX / 2;
            float x = startPos.x;

            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < colCount; j++)
                {
                    mjSeatDeskHuPaiPosList[seat, i, j] = new Vector3(x, deskFacePosY, z);
                    z += mjSizeX + mjSpacing;
                }

                z = startPos.z + mjSizeX / 2;
                x += mjSizeY;
            }
        }

        void CreateMjDeskHuPaiPosSeat2(int rowCount, int colCount, Vector3 startPos, float mjSpacing)
        {
            int seat = 2;
            float mjSizeX = fit.GetDeskMjSizeByAxis(Axis.X);
            float mjSizeY = fit.GetDeskMjSizeByAxis(Axis.Y);
            float x = startPos.x + mjSizeX / 2;
            float z = startPos.z;

            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < colCount; j++)
                {
                    mjSeatDeskHuPaiPosList[seat, i, j] = new Vector3(x, deskFacePosY, z);
                    x += mjSizeX + mjSpacing;
                }

                x = startPos.x + mjSizeX / 2;
                z -= mjSizeY;
            }
        }

        void CreateMjDeskHuPaiPosSeat3(int rowCount, int colCount, Vector3 startPos, float mjSpacing)
        {
            int seat = 3;
            float mjSizeX = fit.GetDeskMjSizeByAxis(Axis.X);
            float mjSizeY = fit.GetDeskMjSizeByAxis(Axis.Y);
            float z = startPos.z - mjSizeX / 2;
            float x = startPos.x;

            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < colCount; j++)
                {
                    mjSeatDeskHuPaiPosList[seat, i, j] = new Vector3(x, deskFacePosY, z);
                    z -= mjSizeX + mjSpacing;
                }

                z = startPos.z + mjSizeX / 2;
                x -= mjSizeY;
            }

        }

        /// <summary>
        /// 指向下一个胡牌麻将可放置的位置
        /// </summary>
        /// <param name="seatIdx"></param>
        /// <returns></returns>
        public Vector3 NextDeskHuPaiMjPos(int seatIdx)
        {
            curtHuPaiMjSeatDeskPosIdx[seatIdx] += 1;
            int idx = curtHuPaiMjSeatDeskPosIdx[seatIdx];
            Vector3 pos = GetDeskHuPaiMjPos(seatIdx, idx);
            return pos;
        }

        int GetNextDeskHuPaiMjPosIdx(int seatIdx)
        {
            int idx = curtHuPaiMjSeatDeskPosIdx[seatIdx] + 1;
            return idx;
        }


        /// <summary>
        /// 获取桌面可以放置胡牌麻将的位置的Idx
        /// </summary>
        /// <param name="seatIdx"></param>
        /// <returns></returns>
        public int GetCurtDeskHuPaiMjPosIdx(int seatIdx)
        {
            return curtHuPaiMjSeatDeskPosIdx[seatIdx];
        }

        /// <summary>
        /// 获取桌面可以放置胡牌麻将的位置
        /// </summary>
        /// <param name="seatIdx"></param>
        /// <param name="idx"></param>
        /// <returns></returns>
        public Vector3 GetDeskHuPaiMjPos(int seatIdx, int idx)
        {
            float mjSizeZ = fit.GetDeskMjSizeByAxis(Axis.Z);

            int rowCount = mjSeatDeskHuPaiPosList.GetLength(1);
            int colCount = mjSeatDeskHuPaiPosList.GetLength(2);

            int realIdxZ = idx / (rowCount * colCount);
            int realIdx = idx % (rowCount * colCount);

            int row = realIdx / colCount;
            int col = realIdx % colCount;

            Vector3 pos = mjSeatDeskHuPaiPosList[seatIdx, row, col];
            pos.y += realIdxZ * mjSizeZ + mjSizeZ / 2 + 0.0002f;

            return pos;
        }

        #endregion

        #region 生成手牌的所有麻将初始位置
        /// <summary>
        /// 生成碰，吃，杠牌的起始位置
        /// </summary>
        /// <param name="mjHandTotalCount"></param>
        void CreateMjHandPaiPosList(int mjHandTotalCount)
        {
            float mjOffsetLR = 0;
            float mjSpacing = 0.0001f;
            float mjOffsetFB = 0.02f;

            CreateMjHandPaiCanvasPosListSeat0(mjHandTotalCount, 0, -50, 5);
            CreateMjHandPaiPosListSeat0(mjHandTotalCount, mjSpacing, mjOffsetLR, 0.313f, mjOffsetFB);
            CreateMjHandPaiPosListSeat1(mjHandTotalCount, mjSpacing, mjOffsetLR, 0.356f, mjOffsetFB);
            CreateMjHandPaiPosListSeat2(mjHandTotalCount, mjSpacing, mjOffsetLR, -0.313f, -mjOffsetFB);
            CreateMjHandPaiPosListSeat3(mjHandTotalCount, mjSpacing, mjOffsetLR, -0.343f, -mjOffsetFB);
        }

        void CreateMjHandPaiCanvasPosListSeat0(int mjCount, float mjSpacing, float mjOffsetLR, float mjOffsetUD)
        {
            float mjsize = fit.GetCanvasHandMjSizeByAxis(Axis.X);
            float mjWallLen = mjCount * mjsize + (mjCount - 1) * mjSpacing;
            float mjStartPos = -mjWallLen / 2 + mjsize / 2 + mjOffsetLR;
            float mjAxisSpacing = mjsize + mjSpacing;

            float mjsizeZ = fit.GetCanvasHandMjSizeByAxis(Axis.Z);
            float mjsizeY = fit.GetCanvasHandMjSizeByAxis(Axis.Y);
            float height = canvasHandPaiRectTransform.sizeDelta.y;

            for (int i = 0; i < mjCount; i++)
            {
                mjSeatHandPaiPosLists[0].Add(new Vector3(mjStartPos + i * mjAxisSpacing, -height / 2 + mjsizeY / 2 + mjOffsetUD, mjsizeZ / 2));
            }
        }

        void CreateMjHandPaiPosListSeat0(int mjCount, float mjSpacing, float mjOffsetLR, float posFB, float mjOffsetFB)
        {
            Transform mjtuo_self_tf = mjtableTransform.Find("desk_mjtuo_self");
            float mjsize = fit.GetHandMjSizeByAxis(Axis.X);
            float mjWallLen = mjCount * mjsize + (mjCount - 1) * mjSpacing;
            float mjStartPos = mjWallLen / 2 - mjsize / 2 + mjtuo_self_tf.transform.position.x + mjOffsetLR;
            float mjAxisSpacing = mjsize + mjSpacing;

            float mjsizeY = fit.GetHandMjSizeByAxis(Axis.Y);

            for (int i = 0; i < mjCount; i++)
            {
                mjSeatHandPaiPosLists[4].Add(new Vector3(mjStartPos - i * mjAxisSpacing, deskFacePosY + 0.0002f + mjsizeY / 2, posFB + mjOffsetFB));
            }
        }

        void CreateMjHandPaiPosListSeat1(int mjCount, float mjSpacing, float mjOffsetLR, float posFB, float mjOffsetFB)
        {
            Transform mjtuo_west_tf = mjtableTransform.Find("desk_mjtuo_west");
            float mjsize = fit.GetHandMjSizeByAxis(Axis.X);
            float mjWallLen = mjCount * mjsize + (mjCount - 1) * mjSpacing;
            float mjStartPos = -mjWallLen / 2 + mjsize / 2 + mjtuo_west_tf.transform.position.z + mjOffsetLR;
            float mjAxisSpacing = mjsize + mjSpacing;
            float mjsizeY = fit.GetHandMjSizeByAxis(Axis.Y);

            for (int i = 0; i < mjCount; i++)
            {
                mjSeatHandPaiPosLists[1].Add(new Vector3(posFB + mjOffsetFB, deskFacePosY + 0.0002f + mjsizeY / 2, mjStartPos + i * mjAxisSpacing));
            }
        }

        void CreateMjHandPaiPosListSeat2(int mjCount, float mjSpacing, float mjOffsetLR, float posFB, float mjOffsetFB)
        {
            Transform mjtuo_north_tf = mjtableTransform.Find("desk_mjtuo_north");
            float mjsize = fit.GetHandMjSizeByAxis(Axis.X);
            float mjWallLen = mjCount * mjsize + (mjCount - 1) * mjSpacing;
            float mjStartPos = -mjWallLen / 2 + mjsize / 2 + mjtuo_north_tf.transform.position.x + mjOffsetLR;
            float mjAxisSpacing = mjsize + mjSpacing;
            float mjsizeY = fit.GetHandMjSizeByAxis(Axis.Y);

            for (int i = 0; i < mjCount; i++)
            {
                mjSeatHandPaiPosLists[2].Add(new Vector3(mjStartPos + i * mjAxisSpacing, deskFacePosY + 0.0002f + mjsizeY / 2, posFB + mjOffsetFB));
            }
        }

        void CreateMjHandPaiPosListSeat3(int mjCount, float mjSpacing, float mjOffsetLR, float posFB, float mjOffsetFB)
        {
            Transform mjtuo_east_tf = mjtableTransform.Find("desk_mjtuo_east");
            float mjsize = fit.GetHandMjSizeByAxis(Axis.X);
            float mjWallLen = mjCount * mjsize + (mjCount - 1) * mjSpacing;
            float mjStartPos = mjWallLen / 2 - mjsize / 2 + mjtuo_east_tf.transform.position.z + mjOffsetLR;
            float mjAxisSpacing = mjsize + mjSpacing;
            float mjsizeY = fit.GetHandMjSizeByAxis(Axis.Y);

            for (int i = 0; i < mjCount; i++)
            {
                mjSeatHandPaiPosLists[3].Add(new Vector3(posFB + mjOffsetFB, deskFacePosY + 0.0002f + mjsizeY / 2, mjStartPos - i * mjAxisSpacing));
            }
        }

        #endregion

        #region DeskGlobalMjPaiSetDict操作

        /// <summary>
        /// 根据行列，获取桌面打出牌麻将的key值,通过dict可获取麻将物件
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="floor"></param>
        /// <returns></returns>
        public int GetDeskDaPaiMjDictKey(int row, int col, int floor)
        {
            return row * 1000 + col * 100 + floor;
        }

        /// <summary>
        /// 添加麻将到桌面全局麻将dict集中
        /// </summary>
        /// <param name="mjFaceValue"></param>
        /// <param name="mj"></param>
        public void AppendMjToDeskGlobalMjPaiSetDict(MahjongFaceValue mjFaceValue, GameObject mj)
        {
            List<GameObject> mjList;

            if (deskGlobalMjPaiSetDict.ContainsKey(mjFaceValue))
            {
                mjList = deskGlobalMjPaiSetDict[mjFaceValue];
            }
            else
            {
                mjList = new List<GameObject>();
                deskGlobalMjPaiSetDict[mjFaceValue] = mjList;
            }

            if (highLightMjValue == mjFaceValue)
                highLightMjValue = MahjongFaceValue.MJ_UNKNOWN;

            mjList.Add(mj);
        }

        /// <summary>
        /// 根据麻将值从全局麻将牌Dict中移除相同麻将值的对应GameObject(mj)
        /// </summary>
        /// <param name="mjFaceValue"></param>
        /// <param name="mj"></param>
        public void RemoveMjFromDeskGlobalMjPaiSetDict(MahjongFaceValue mjFaceValue, GameObject mj)
        {
            if (!deskGlobalMjPaiSetDict.ContainsKey(mjFaceValue))
                return;

            deskGlobalMjPaiSetDict[mjFaceValue].Remove(mj);
        }


        /// <summary>
        /// 根据麻将值从全局麻将牌Dict中获取相同麻将值的对应的GameObjects的列表
        /// </summary>
        /// <param name="mjFaceValue"></param>
        /// <returns></returns>
        public GameObject[] GetMjGroupByDeskGlobalMjPaiSetDict(MahjongFaceValue mjFaceValue)
        {
            if (!deskGlobalMjPaiSetDict.ContainsKey(mjFaceValue))
                return null;

            List<GameObject> mjList = deskGlobalMjPaiSetDict[mjFaceValue];
            return mjList.ToArray();
        }


        /// <summary>
        /// 清理桌面全局麻将牌集
        /// </summary>
        void ClearDeskGlobalMjPaiSetDict()
        {
            deskGlobalMjPaiSetDict.Clear();
        }

        #endregion

        #region 生成桌面麻将牌堆


        /// <summary>
        /// 生成桌面麻将牌堆
        /// </summary>
        void CreateMahjongPaiDui()
        {
            int duiCount = fit.mjPaiTotalCount / 2;
            int rCount = fit.mjPaiTotalCount % 2;

            int tuoMjCount = duiCount / fit.canDuiPaiMjTuoCount * 2;
            rCount = (duiCount % fit.canDuiPaiMjTuoCount) * 2 + rCount;

            int useMjTuoCount = fit.canDuiPaiMjTuoCount;

            if (fit.canDuiPaiMjTuoCount == 3)
                useMjTuoCount -= 1;
            else if (fit.canDuiPaiMjTuoCount == 4)
                useMjTuoCount -= 2;
            else
                useMjTuoCount = 1;

            int m2 = rCount / useMjTuoCount;
            int n2 = rCount % useMjTuoCount;


            int MjCountSeat0 = 0;
            int MjCountSeat1 = 0;
            int MjCountSeat2 = 0;
            int MjCountSeat3 = 0;


            if (fit.isSeatMjTuoCanDuiPai[0])
            {
                MjCountSeat0 = tuoMjCount + m2 + n2;
                CreateMahjongPaiDuiSeat(0, 1, MjCountSeat0, 0.0002f, "desk_mjtuo_self", new Vector3(0, 0, 0.015f));
            }

            if (fit.isSeatMjTuoCanDuiPai[1])
            {
                MjCountSeat1 = tuoMjCount;
                CreateMahjongPaiDuiSeat(1, MjCountSeat0 + 1, MjCountSeat1, 0.0005f, "desk_mjtuo_west", new Vector3(0.003f, 0, 0));
            }

            if (fit.isSeatMjTuoCanDuiPai[2])
            {
                MjCountSeat2 = tuoMjCount + m2;
                CreateMahjongPaiDuiSeat(2, MjCountSeat0 + MjCountSeat1 + 1, MjCountSeat2, 0.0002f, "desk_mjtuo_north", new Vector3(0, 0, 0.004f));
            }

            if (fit.isSeatMjTuoCanDuiPai[3])
            {
                MjCountSeat3 = tuoMjCount;
                CreateMahjongPaiDuiSeat(3, MjCountSeat0 + MjCountSeat1 + MjCountSeat2 + 1, MjCountSeat3, 0.0005f, "desk_mjtuo_east", new Vector3(0.004f, 0, 0.01f));
            }

        }

        void CreateMahjongPaiDuiSeat(int seat, int paiStartIdx, int mjCount, float mjSpacing, string mjTuoName, Vector3 mjTuoCenterOffset)
        {
            int upMjCount = mjCount / 2;
            int downMjCount = upMjCount;
            int richMjCount = mjCount % 2;

            if (richMjCount != 0)
                downMjCount += richMjCount;

            int paiIdx = paiStartIdx;
            Transform mjtuo_tf = mjtableTransform.Find(mjTuoName);
            float mjtuoHeight = mjtuo_tf.GetComponent<Renderer>().bounds.size.y;
            float tuoFacePosY = mjtuo_tf.position.y + mjTuoCenterOffset.y + mjtuoHeight / 2;

            GameObject premj = mjAssetsMgr.emptyMjPai;
            float mjSizeX = fit.GetWallDuiMjSizeByAxis(Axis.X);
            float mjSizeZ = fit.GetWallDuiMjSizeByAxis(Axis.Z);

            float mjAxisSpacing = mjSizeX + mjSpacing;
            float mjWallLen = downMjCount * mjSizeX + (downMjCount - 1) * mjSpacing;
            float mjStartPos = 0;


            switch (seat)
            {
                case 0:
                    mjStartPos = mjtuo_tf.position.x + mjTuoCenterOffset.x - mjWallLen / 2 + mjSizeX / 2;
                    break;

                case 1:
                    mjStartPos = mjtuo_tf.position.z + mjTuoCenterOffset.z + mjWallLen / 2 - mjSizeX / 2;
                    break;

                case 2:
                    mjStartPos = mjtuo_tf.position.x + mjTuoCenterOffset.x + mjWallLen / 2 - mjSizeX / 2;
                    break;

                case 3:
                    mjStartPos = mjtuo_tf.position.z + mjTuoCenterOffset.z - mjWallLen / 2 + mjSizeX / 2;
                    break;
            }

            float yOffset = mjSizeZ / 2 + mjSizeZ + 0.0002f;
            int udMjCount = upMjCount;

            for (int j = 0; j < 2; j++)
            {
                for (int i = 0; i < udMjCount; i++)
                {
                    GameObject mj = Object.Instantiate(premj);
                    fit.FitSeatWallDuiMj(seat, mj);
                    mj.name = "mjDuiPai_" + paiIdx;

                    switch (seat)
                    {
                        case 0:
                            mj.transform.position = new Vector3(mjStartPos + i * mjAxisSpacing, tuoFacePosY + yOffset, mjtuo_tf.position.z + mjTuoCenterOffset.z);
                            break;
                        case 1:
                            mj.transform.position = new Vector3(mjtuo_tf.position.x + mjTuoCenterOffset.x, tuoFacePosY + yOffset, mjStartPos - i * mjAxisSpacing);
                            break;
                        case 2:
                            mj.transform.position = new Vector3(mjStartPos - i * mjAxisSpacing, tuoFacePosY + yOffset, mjtuo_tf.position.z + mjTuoCenterOffset.z);
                            break;
                        case 3:
                            mj.transform.position = new Vector3(mjtuo_tf.position.x + mjTuoCenterOffset.x, tuoFacePosY + yOffset, mjStartPos + i * mjAxisSpacing);
                            break;
                    }

                    mj.transform.SetParent(mjtuo_tf, true);
                    mjDuiPai[paiIdx] = mj;

                    if (j == 0)
                    {
                        GameObject mjShadow = mjAssetsMgr.effectPrefabDict[(int)PrefabIdx.MJ_SHADOW][0];
                        Vector3 shadowLocalScale = mjShadow.transform.localScale;
                        Transform shadow = mj.transform.GetChild(0);

                        shadow.transform.localScale =
                            new Vector3(fit.wallduiMjScale.x * shadowLocalScale.x * 0.95f,     
                            fit.wallduiMjScale.y * shadowLocalScale.y * 0.95f, 1);

                        mjDuiPaiUpDown[paiIdx] = MahjongUpDown.MG_UP;
                    }
                    else
                        mjDuiPaiUpDown[paiIdx] = MahjongUpDown.MG_DOWN;

                    if (j == 1 && i >= upMjCount - 1)
                        paiIdx++;
                    else
                        paiIdx += 2;
                }

                udMjCount = downMjCount;
                paiIdx = paiStartIdx + 1;
                yOffset = mjSizeZ / 2 + 0.0002f;
            }
        }
        #endregion

        /// <summary>
        /// 从牌堆摸一张麻将牌
        /// </summary>
        /// <param name="fenPaiStartIdx"></param>
        /// <param name="fenPaiCount"></param>
        public void FenMahjongPaiFromPaiDui(int fenPaiStartIdx, int fenPaiCount)
        {
            if (mjDuiPai[fenPaiStartIdx] == null)
                return;

            if (fenPaiCount > paiDuiRichCount)
                fenPaiCount = paiDuiRichCount;

            for (int i = 0; i < fenPaiCount;)
            {
                if (fenPaiStartIdx > fit.mjPaiTotalCount)
                    fenPaiStartIdx -= fit.mjPaiTotalCount;

                if (mjDuiPai[fenPaiStartIdx].activeSelf == false)
                {
                    fenPaiStartIdx++;
                    continue;
                }

                mjDuiPai[fenPaiStartIdx].SetActive(false);
                fenPaiStartIdx++;
                paiDuiRichCount--;
                i++;
            }

            curtPaiDuiPos = fenPaiStartIdx;
            if (curtPaiDuiPos > fit.mjPaiTotalCount)
                curtPaiDuiPos -= fit.mjPaiTotalCount;
        }

        #region 高亮桌面指定的麻将
        /// <summary>
        /// 高亮桌面指定的麻将
        /// </summary>
        /// <param name="highLightDeskMjFaceValue"></param>
        public void OnDeskMjHighLight(MahjongFaceValue highLightDeskMjFaceValue)
        {
            if (highLightDeskMjFaceValue == MahjongFaceValue.MJ_UNKNOWN)
                return;

            GameObject[] mjGroup = GetMjGroupByDeskGlobalMjPaiSetDict(highLightDeskMjFaceValue);
            if (mjGroup == null)
                return;

            Renderer renderer;
            for (int i = 0; i < mjGroup.Length; i++)
            {
                renderer = mjGroup[i].GetComponent<Renderer>();
                renderer.sharedMaterial = mjAssetsMgr.mjHighLightFaceMat;
            }

            highLightMjValue = highLightDeskMjFaceValue;
        }

        /// <summary>
        /// 关闭桌面指定麻将的高亮
        /// </summary>
        /// <param name="offHighLightDeskMjFaceValue"></param>
        public void OffDeskMjHighLight(MahjongFaceValue offHighLightDeskMjFaceValue)
        {
            if (offHighLightDeskMjFaceValue == MahjongFaceValue.MJ_UNKNOWN)
                return;

            GameObject[] mjGroup = GetMjGroupByDeskGlobalMjPaiSetDict(offHighLightDeskMjFaceValue);
            if (mjGroup == null)
                return;

            Renderer renderer;
            for (int i = 0; i < mjGroup.Length; i++)
            {
                renderer = mjGroup[i].GetComponent<Renderer>();
                renderer.sharedMaterial = mjAssetsMgr.mjNormalFaceMat;
            }

            highLightMjValue = MahjongFaceValue.MJ_UNKNOWN;
        }

        #endregion


        /// <summary>
        /// 生成自身手牌的byte数据列表
        /// </summary>
        /// <param name="cards"></param>
        public void CreateHandPaiCardList(ref byte[] cards)
        {
            if (cards == null)
                return;

            int cardIdx;
            for (int i = 0; i < mjSeatHandPaiLists[0].Count; i++)
            {
                if (mjSeatHandPaiLists[0][i] == null)
                    continue;

                cardIdx = (int)(mjSeatHandPaiLists[0][i].GetComponent<MjPaiData>().mjFaceValue);
                if (cardIdx >= 34)
                    continue;

                cards[cardIdx]++;
            }

            for (int i = 0; i < mjSeatMoPaiLists[0].Count; i++)
            {
                if (mjSeatMoPaiLists[0][i] == null)
                    continue;

                cardIdx = (int)(mjSeatMoPaiLists[0][i].GetComponent<MjPaiData>().mjFaceValue);
                if (cardIdx >= 34)
                    continue;

                cards[cardIdx]++;
            }
        }


        /// <summary>
        /// 显示或隐藏麻将牌堆
        /// </summary>
        /// <param name="isShow"></param>
        public void ShowOrHideMjDuiPai(bool isShow = false)
        {
            for (int i = 0; i < mjDuiPai.Length; i++)
            {
                if (mjDuiPai[i] == null)
                    continue;

                if (mjDuiPai[i].activeInHierarchy == !isShow)
                    mjDuiPai[i].SetActive(isShow);
            }
        }


        /// <summary>
        /// 获取刮风下雨等特效的组合特效中的子特效
        /// </summary>
        /// <param name="effects"></param>
        /// <param name="idx"></param>
        /// <returns></returns>
        public EffectFengRainEtcType GetEffectFengRainEtcType(EffectFengRainEtcType effects, int idx)
        {
            int mask = 0x1 << idx;
            return (EffectFengRainEtcType)((int)effects & mask);
        }

    }
}