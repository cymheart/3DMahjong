using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CoreDesgin
{
    /// <summary>
    /// 麻将颜色（种类）定义
    /// </summary>
    public enum MJColor
    {
        WAN = 0,  //万
        TONG,     //筒
        TIAO,     //条
        FenZi,    //风,字 牌
        Max
    }

    public struct TingData
    {
        public int tingCardIdx;
        public int huCardsEndIdx;
        public byte[] huCards;
    }



    public class MahjongHuPaiData
    {
        struct SingleKey
        {
            public byte count;
            public byte key1;
            public byte key2;
        }

        struct KeyPtr
        {
            public int key;
            public byte idx;
        }

        /// <summary>
        /// 赖子最大个数
        /// </summary>
        const int MAX_LAIZI_NUM = 7;

        /// <summary>
        /// 最大牌数量
        /// </summary>
        int max_pai_idx_num;

        /// <summary>
        /// 牌型最大胡牌数量
        /// </summary>
        public int MAX_HUPAI_NUM = 0;

        /// <summary>
        /// bit_flag
        /// </summary>
        const int BIT_VAL_FLAG = 0x07;


        /// <summary>
        ///一个值占的bit数 
        /// </summary>
        const int BIT_VAL_NUM = 3;

        int laziPaisCount = 30;
        int recordLaziDataLimitCount = 2;
        int[] laziLimits = new int[] { 1, 10, 30, 60, 90 };
        uint[] retSucessTempLaiDataData = new uint[1];

        /// <summary>
        /// 单个顺子+刻子,其中也包含赖子配对后的顺子和刻子
        /// </summary>
        HashSet<int> setSingle = null;
        Dictionary<int, SingleKey> dictLaiziSingleKeys = null;

        /// <summary>
        /// 单个将,其中也包含赖子配对后的将
        /// </summary>
        HashSet<int> setSingleJiang = null;
        Dictionary<int, byte> dictLaiziSingleJiangKeys = null;



        /// <summary>
        /// 组合单个key后的有效胡牌牌型的key值集合
        /// 其中key为不包含赖子参与计算的paikey
        /// 其中value为这组有效牌型用到的赖子的个数
        /// </summary>
        Dictionary<int, byte>[] dictHuPaiKeys = null;

        Dictionary<int, uint[]>[] dictLaiziKeys = null;


        byte[] _paiIndexs = null;
        int laziBitMask;

        bool isCreateLaiziDetailData = false;



        /// <summary>
        /// 是否生成单个顺子作为有效牌型
        /// </summary>
        public bool IsCreateSingleShunZiVaildPaiType { get; set; }

        /// <summary>
        /// 记录赖子数据的最大赖子个数
        /// </summary>
        public int RecordLaziDataLimitCount
        {
            get { return recordLaziDataLimitCount; }

            set
            {
                recordLaziDataLimitCount = value;
                laziPaisCount = laziLimits[recordLaziDataLimitCount];

                if (recordLaziDataLimitCount == 0)
                    IsCreateLaiziDetailData = false;
            }
        }

        /// <summary>
        /// 是否生成赖子的详细数据
        /// </summary>
        public bool IsCreateLaiziDetailData
        {
            get { return isCreateLaiziDetailData; }

            set
            {
                if (isCreateLaiziDetailData == value)
                    return;

                if (recordLaziDataLimitCount == 0)
                    isCreateLaiziDetailData = false;
                else
                    isCreateLaiziDetailData = value;

                if (isCreateLaiziDetailData)
                {
                    dictLaiziSingleKeys = new Dictionary<int, SingleKey>();
                    dictLaiziSingleJiangKeys = new Dictionary<int, byte>();

                    dictLaiziKeys = new Dictionary<int, uint[]>[MAX_HUPAI_NUM + 1];
                    for (int i = 0; i < dictLaiziKeys.Length; i++)
                    {
                        dictLaiziKeys[i] = new Dictionary<int, uint[]>();
                    }
                }
                else
                {
                    dictLaiziSingleKeys = null;
                    dictLaiziKeys = null;
                }
            }
        }


        /// <summary>
        /// 设置最大胡牌数量
        /// </summary>
        public int MaxHuPaiAmount
        {
            get
            {
                return MAX_HUPAI_NUM;
            }
            set
            {
                if (value == MAX_HUPAI_NUM)
                    return;

                MAX_HUPAI_NUM = value;

                dictHuPaiKeys = new Dictionary<int, byte>[MAX_HUPAI_NUM + 1];
                for (int i = 0; i < dictHuPaiKeys.Length; i++)
                {
                    dictHuPaiKeys[i] = new Dictionary<int, byte>();
                }

                IsCreateLaiziDetailData = false;
                IsCreateLaiziDetailData = true;
            }
        }

        public MahjongHuPaiData(int maxPaiIdxNum = 10, int maxHuaPaiNum = 14)
        {
            IsCreateSingleShunZiVaildPaiType = true;

            max_pai_idx_num = maxPaiIdxNum;
            MaxHuPaiAmount = maxHuaPaiNum;

            _paiIndexs = new byte[max_pai_idx_num];

            int n = 7 << ((max_pai_idx_num - 1) * BIT_VAL_NUM);
            laziBitMask = (int)(n ^ 0xFFFFFFFF);

            setSingle = new HashSet<int>();
            setSingleJiang = new HashSet<int>();
        }

        /// <summary>
        /// 根据牌型的索引数组获取牌型的key值
        /// </summary>
        /// <param name="indexs"></param>
        /// <returns></returns>
        int GetPaiKeyByPaiIndexs(byte[] indexs)
        {
            int nKey = 0;
            for (int i = 0; i < indexs.Length; ++i)
                nKey |= (indexs[i] & BIT_VAL_FLAG) << (BIT_VAL_NUM * i);
            return nKey;
        }


        /// <summary>
        /// 判断牌key值是否是有效能胡的key值
        /// </summary>
        /// <param name="paiKey"></param>
        /// <returns></returns>
        bool IsValidPaiKey(int paiKey)
        {
            for (int i = 0; i < _paiIndexs.Length; ++i)
                _paiIndexs[i] = (byte)((paiKey >> (BIT_VAL_NUM * i)) & BIT_VAL_FLAG);

            if (_paiIndexs[_paiIndexs.Length - 1] > MAX_LAIZI_NUM)
                return false;

            int count = 0;
            for (int i = 0; i < _paiIndexs.Length; ++i)
            {
                count += _paiIndexs[i];
                if (_paiIndexs[i] > 4 || count > MAX_HUPAI_NUM)
                    return false;
            }

            return count > 0;
        }


        /// <summary>
        /// 根据给定的牌型key值获取此牌型牌的数量
        /// </summary>
        /// <param name="paiKey"></param>
        /// <param name="isContainLaiZiPai">计算牌数量时，是否包含赖子牌</param>
        /// <returns></returns>
        byte GetPaiAmountByPaiKey(int paiKey, bool isContainLaiZiPai = false)
        {
            for (int i = 0; i < _paiIndexs.Length; ++i)
                _paiIndexs[i] = (byte)((paiKey >> (BIT_VAL_NUM * i)) & BIT_VAL_FLAG);

            byte amount = 0;
            int len = _paiIndexs.Length - 1;

            if (isContainLaiZiPai) //是否包含赖子牌的数量计算
                len++;

            for (int i = 0; i < len; ++i)
                amount += _paiIndexs[i];

            return amount;
        }


        /// <summary>
        /// 添加有效牌型key值到有效胡牌牌型的key值集合中
        /// </summary>
        /// <param name="mapTemp"></param>
        /// <param name="paiKey">不包含赖子参与计算的paikey</param>
        void AddPaiKeyToDictHuPaiKeys(int paiKey, uint laiziData)
        {
            byte amount = GetPaiAmountByPaiKey(paiKey, false);
            byte newLaiziAmount = (byte)((paiKey >> (BIT_VAL_NUM * (max_pai_idx_num - 1))) & BIT_VAL_FLAG);

            //去除高位赖子牌数量值，得到不包含赖子参与计算的paikey
            int paiKeyNotHaveLaizi = (paiKey & laziBitMask);
            bool isContains = dictHuPaiKeys[amount].ContainsKey(paiKeyNotHaveLaizi);
            byte orgLaiziAmount = 0;

            if (isContains)
            {
                orgLaiziAmount = dictHuPaiKeys[amount][paiKeyNotHaveLaizi];
                dictHuPaiKeys[amount][paiKeyNotHaveLaizi] = Math.Min(orgLaiziAmount, newLaiziAmount);
            }
            else
            {
                dictHuPaiKeys[amount][paiKeyNotHaveLaizi] = newLaiziAmount;
            }

            if (IsCreateLaiziDetailData == false)
                return;

            //生成赖子的详细数据
            if (isContains)
            {
                if (orgLaiziAmount == newLaiziAmount)
                {
                    if (GetLaiziCountFromCombData(laiziData) == 0)
                        return;

                    if (dictLaiziKeys[amount].ContainsKey(paiKeyNotHaveLaizi))
                    {
                        uint[] datas = dictLaiziKeys[amount][paiKeyNotHaveLaizi];
                        if (!CheckHavSameLaiziData(ref datas, laiziData))
                        {
                            datas[0]++;

                            if (newLaiziAmount <= RecordLaziDataLimitCount)
                                datas[datas[0]] = laiziData;
                        }
                    }
                    else
                    {
                        uint[] datas;
                        if (newLaiziAmount <= RecordLaziDataLimitCount)
                        {
                            datas = new uint[laziPaisCount];
                            datas[0] = 1;
                            datas[1] = laiziData;
                        }
                        else
                        {
                            datas = new uint[1];
                            datas[0] = 1;
                        }

                        dictLaiziKeys[amount][paiKeyNotHaveLaizi] = datas;
                    }
                }
                else if (newLaiziAmount < orgLaiziAmount)
                {
                    if (GetLaiziCountFromCombData(laiziData) == 0)
                    {
                        if (dictLaiziKeys[amount].ContainsKey(paiKeyNotHaveLaizi))
                            dictLaiziKeys[amount].Remove(paiKeyNotHaveLaizi);
                        return;
                    }

                    uint[] datas;
                    if (dictLaiziKeys[amount].ContainsKey(paiKeyNotHaveLaizi))
                    {
                        if (orgLaiziAmount > RecordLaziDataLimitCount)
                        {
                            datas = new uint[laziPaisCount];
                            dictLaiziKeys[amount][paiKeyNotHaveLaizi] = datas;
                        }
                        else
                        {
                            datas = dictLaiziKeys[amount][paiKeyNotHaveLaizi];
                        }

                        datas[0] = 1;
                        if (newLaiziAmount <= RecordLaziDataLimitCount)
                            datas[1] = laiziData;
                    }
                    else
                    {
                        if (newLaiziAmount <= RecordLaziDataLimitCount)
                        {
                            datas = new uint[laziPaisCount];
                            datas[0] = 1;
                            datas[1] = laiziData;
                        }
                        else
                        {
                            datas = new uint[1];
                            datas[0] = 1;
                        }

                        dictLaiziKeys[amount][paiKeyNotHaveLaizi] = datas;
                    }
                }
            }
            else
            {
                if (GetLaiziCountFromCombData(laiziData) == 0)
                    return;

                uint[] datas;
                if (newLaiziAmount <= RecordLaziDataLimitCount)
                {
                    datas = new uint[laziPaisCount];
                    datas[0] = 1;
                    datas[1] = laiziData;
                }
                else
                {
                    datas = new uint[1];
                    datas[0] = 1;
                }

                dictLaiziKeys[amount][paiKeyNotHaveLaizi] = datas;
            }
        }

        /// <summary>
        /// 添加有效牌型key值到有效胡牌牌型的key值集合中
        /// </summary>
        /// <param name="mapTemp"></param>
        /// <param name="paiKey">不包含赖子参与计算的paikey</param>
        void AddPaiKeyToDictHuPaiKeys(int paiKey, uint[] laiziDatas)
        {
            byte amount = GetPaiAmountByPaiKey(paiKey, false);
            byte newLaiziAmount = (byte)((paiKey >> (BIT_VAL_NUM * (max_pai_idx_num - 1))) & BIT_VAL_FLAG);

            //去除高位赖子牌数量值，得到不包含赖子参与计算的paikey
            int paiKeyNotHaveLaizi = (paiKey & laziBitMask);
            bool isContains = dictHuPaiKeys[amount].ContainsKey(paiKeyNotHaveLaizi);
            byte orgLaiziAmount = 0;

            if (isContains)
            {
                orgLaiziAmount = dictHuPaiKeys[amount][paiKeyNotHaveLaizi];
                dictHuPaiKeys[amount][paiKeyNotHaveLaizi] = Math.Min(orgLaiziAmount, newLaiziAmount);
            }
            else
            {
                dictHuPaiKeys[amount][paiKeyNotHaveLaizi] = newLaiziAmount;
            }


            if (IsCreateLaiziDetailData == false ||
                laiziDatas == null ||
                newLaiziAmount > RecordLaziDataLimitCount)
                return;

            //生成赖子的详细数据
            if (isContains)
            {
                if (orgLaiziAmount == newLaiziAmount)
                {
                    if (dictLaiziKeys[amount].ContainsKey(paiKeyNotHaveLaizi))
                    {
                        uint[] datas = dictLaiziKeys[amount][paiKeyNotHaveLaizi];
                        uint idx = datas[0];

                        if (newLaiziAmount <= RecordLaziDataLimitCount)
                        {
                            for (int i = 1; i <= laiziDatas[0]; i++)
                            {
                                if (!CheckHavSameLaiziData(ref datas, laiziDatas[i]))
                                    datas[++idx] = laiziDatas[i];
                            }
                        }
                        else
                        {
                            idx = datas[0] + laiziDatas[0];
                        }

                        datas[0] = idx;
                    }
                    else
                    {
                        dictLaiziKeys[amount][paiKeyNotHaveLaizi] = laiziDatas;
                    }
                }
                else if (newLaiziAmount < orgLaiziAmount)
                {
                    dictLaiziKeys[amount][paiKeyNotHaveLaizi] = laiziDatas;
                }
            }
            else
            {
                dictLaiziKeys[amount][paiKeyNotHaveLaizi] = laiziDatas;
            }
        }


        bool CheckHavSameLaiziData(ref uint[] datas, uint laiziData)
        {
            for (int i = 0; i < datas.Length; i++)
            {
                if (datas[i] == laiziData)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 组合赖子数据
        /// </summary>
        /// <param name="laziData"></param>
        /// <param name="lazi"></param>
        /// <returns></returns>
        uint CombLaiZiToData(uint laziData, byte lazi)
        {
            uint countInOldQue = GetLaiziCountFromCombData(laziData);
            if (countInOldQue == 0)
                return _CombLaiZiToData(laziData, lazi);

            uint newLaziData = 0;
            byte laziInOldQue;
            bool isAddNewLaizi = false;

            for (int i = 0; i < countInOldQue; i++)
            {
                laziInOldQue = GetLaiziFromCombData(laziData, i);

                if (!isAddNewLaizi && lazi <= laziInOldQue)
                {
                    newLaziData = _CombLaiZiToData(newLaziData, lazi);
                    isAddNewLaizi = true;
                }

                newLaziData = _CombLaiZiToData(newLaziData, laziInOldQue);
            }

            if (!isAddNewLaizi)
                newLaziData = _CombLaiZiToData(newLaziData, lazi);

            return newLaziData;
        }
        uint _CombLaiZiToData(uint laziData, byte lazi)
        {
            uint count = (laziData >> 27) & 0x1F;
            uint countHi = (laziData & 0x7FFFFFF) | (((count + 1) << 27) & 0xF8000000);
            int movBit = (int)count * 4;
            uint mask = ((uint)0xF << movBit);
            laziData = (((uint)lazi << movBit) & mask) | countHi;

            return laziData;
        }
        uint GetLaiziCountFromCombData(uint laziData)
        {
            return (laziData >> 27) & 0x1F;
        }
        byte GetLaiziFromCombData(uint laziData, int idx)
        {
            return (byte)((laziData >> (idx * 4)) & 0xF);
        }

        uint CombLaiZiToDataByKey(uint laziData, KeyPtr keyPtr)
        {
            if (!dictLaiziSingleKeys.ContainsKey(keyPtr.key))
                return laziData;

            SingleKey singleKey = dictLaiziSingleKeys[keyPtr.key];
            byte lazi;
            if (keyPtr.idx == 0) { lazi = singleKey.key1; }
            else { lazi = singleKey.key2; }

            return CombLaiZiToData(laziData, lazi);
        }

        uint CombJiangLaiZiToDataByKey(uint laziData, int key)
        {
            if (!dictLaiziSingleJiangKeys.ContainsKey(key))
                return laziData;

            return CombLaiZiToData(laziData, dictLaiziSingleJiangKeys[key]);
        }

        void AddToDictLaiziSingleKeys(int key, byte value)
        {
            if (dictLaiziSingleKeys.ContainsKey(key))
            {
                SingleKey singleKey = dictLaiziSingleKeys[key];
                singleKey.count = 2;
                singleKey.key2 = value;
                dictLaiziSingleKeys[key] = singleKey;
            }
            else
            {
                SingleKey singleKey = new SingleKey();
                singleKey.count = 1;
                singleKey.key1 = value;
                dictLaiziSingleKeys[key] = singleKey;
            }
        }

        KeyPtr[] CreateKeyPtrs()
        {
            int singlePaiKeyCount = setSingle.Count;
            int[] singlePaiKey = setSingle.ToArray();

            List<KeyPtr> keyPtrList = new List<KeyPtr>();
            SingleKey singleKey;
            KeyPtr keyPtr;
            for (int i = 0; i < singlePaiKey.Length; i++)
            {
                if (dictLaiziSingleKeys == null ||
                    !dictLaiziSingleKeys.ContainsKey(singlePaiKey[i]))
                {
                    keyPtr = new KeyPtr();
                    keyPtr.key = singlePaiKey[i];
                    keyPtr.idx = 0;
                    keyPtrList.Add(keyPtr);
                }
                else
                {
                    singleKey = dictLaiziSingleKeys[singlePaiKey[i]];

                    if (singleKey.count == 2)
                    {
                        keyPtr = new KeyPtr();
                        keyPtr.key = singlePaiKey[i];
                        keyPtr.idx = 1;
                        keyPtrList.Add(keyPtr);
                    }

                    keyPtr = new KeyPtr();
                    keyPtr.key = singlePaiKey[i];
                    keyPtr.idx = 0;
                    keyPtrList.Add(keyPtr);
                }
            }

            return keyPtrList.ToArray();
        }


        /// <summary>
        /// 生成单个有效牌型的key值
        /// </summary>
        void CreateSingleVaildPaiTypeKey()
        {
            if (setSingle.Count != 0)
                return;

            byte[] paiIndexs = new byte[max_pai_idx_num];
            int key;

            //三个赖子作顺子，或刻子
            Array.Clear(paiIndexs, 0, paiIndexs.Length);
            paiIndexs[paiIndexs.Length - 1] = 3;
            setSingle.Add(GetPaiKeyByPaiIndexs(paiIndexs));

            //刻子
            for (int i = 0; i < paiIndexs.Length - 1; ++i)
            {
                Array.Clear(paiIndexs, 0, paiIndexs.Length);

                for (int n = 0; n < 3; ++n)
                {
                    paiIndexs[i] = (byte)(3 - n);
                    paiIndexs[paiIndexs.Length - 1] = (byte)n;

                    key = GetPaiKeyByPaiIndexs(paiIndexs);
                    setSingle.Add(key);

                    if (IsCreateLaiziDetailData && n != 0)
                        AddToDictLaiziSingleKeys(key, (byte)i);
                }
            }


            if (IsCreateSingleShunZiVaildPaiType)
            {
                //顺子 没赖子
                for (int i = 0; i < paiIndexs.Length - 3; ++i)
                {
                    Array.Clear(paiIndexs, 0, paiIndexs.Length);

                    paiIndexs[i] = 1;
                    paiIndexs[i + 1] = 1;
                    paiIndexs[i + 2] = 1;

                    setSingle.Add(GetPaiKeyByPaiIndexs(paiIndexs));
                }


                //顺子 1个赖子 (2个赖子时也就是刻子,上面已经添加刻子)
                for (int i = 0; i < paiIndexs.Length - 3; ++i)
                {
                    for (int n = 0; n < 3; ++n)
                    {
                        Array.Clear(paiIndexs, 0, paiIndexs.Length);
                        paiIndexs[i] = 1;
                        paiIndexs[i + 1] = 1;
                        paiIndexs[i + 2] = 1;

                        paiIndexs[i + n] = 0;
                        paiIndexs[paiIndexs.Length - 1] = 1;

                        key = GetPaiKeyByPaiIndexs(paiIndexs);
                        setSingle.Add(key);

                        if (IsCreateLaiziDetailData)
                            AddToDictLaiziSingleKeys(key, (byte)(i + n));
                    }
                }

            }

            //将牌 两个赖子作将
            Array.Clear(paiIndexs, 0, paiIndexs.Length);
            paiIndexs[paiIndexs.Length - 1] = 2;
            setSingleJiang.Add(GetPaiKeyByPaiIndexs(paiIndexs));

            //将牌 (包含一个赖子作将的情况)
            for (int i = 0; i < paiIndexs.Length - 1; ++i)
            {
                Array.Clear(paiIndexs, 0, paiIndexs.Length);

                for (int n = 0; n < 2; ++n)
                {
                    paiIndexs[i] = (byte)(2 - n);
                    paiIndexs[paiIndexs.Length - 1] = (byte)n;

                    key = GetPaiKeyByPaiIndexs(paiIndexs);
                    setSingleJiang.Add(key);

                    if (IsCreateLaiziDetailData && n != 0)
                        dictLaiziSingleJiangKeys[key] = (byte)i;
                }
            }
        }


        /// <summary>
        /// 生成组合牌类型的有效key到dictHuPaiKeys
        /// 主要算法为：把所有的单个有效key值进行一个4组或5组的遍历组合，剔除掉组合后无效的key值组。
        /// key值之间能组合相加，主要是因为单张牌数量相加不能超过3个bit位，即7张牌的数量，
        /// 如果有进位组合相加的算法会有问题
        /// </summary>
        /// <param name="dictHuPaiKeys"></param>
        void CreateCombPaiTypeVaildKeyToDict()
        {

            KeyPtr[] singlePaiKey = CreateKeyPtrs();
            int singlePaiKeyCount = singlePaiKey.Length;
            int[] combPaiKey = new int[6];
            uint[] laziData = new uint[6];

            //组合所有可能的顺子,刻子组合
            for (int i1 = 0; i1 < singlePaiKeyCount; ++i1)
            {
                if (IsCreateLaiziDetailData)
                    laziData[1] = CombLaiZiToDataByKey(0, singlePaiKey[i1]);

                AddPaiKeyToDictHuPaiKeys(singlePaiKey[i1].key, laziData[1]);

                for (int i2 = i1; i2 < singlePaiKeyCount; ++i2)
                {
                    combPaiKey[2] = singlePaiKey[i1].key + singlePaiKey[i2].key;
                    if (!IsValidPaiKey(combPaiKey[2]))
                        continue;

                    if (IsCreateLaiziDetailData)
                        laziData[2] = CombLaiZiToDataByKey(laziData[1], singlePaiKey[i2]);

                    AddPaiKeyToDictHuPaiKeys(combPaiKey[2], laziData[2]);

                    for (int i3 = i2; i3 < singlePaiKeyCount; ++i3)
                    {
                        combPaiKey[3] = combPaiKey[2] + singlePaiKey[i3].key;
                        if (!IsValidPaiKey(combPaiKey[3]))
                            continue;

                        if (IsCreateLaiziDetailData)
                            laziData[3] = CombLaiZiToDataByKey(laziData[2], singlePaiKey[i3]);

                        AddPaiKeyToDictHuPaiKeys(combPaiKey[3], laziData[3]);

                        for (int i4 = i3; i4 < singlePaiKeyCount; ++i4)
                        {
                            combPaiKey[4] = combPaiKey[3] + singlePaiKey[i4].key;

                            if (!IsValidPaiKey(combPaiKey[4]))
                                continue;

                            if (IsCreateLaiziDetailData)
                                laziData[4] = CombLaiZiToDataByKey(laziData[3], singlePaiKey[i4]);

                            AddPaiKeyToDictHuPaiKeys(combPaiKey[4], laziData[4]);


                            if (MAX_HUPAI_NUM > 14)
                            {
                                for (int i5 = i4; i5 < singlePaiKeyCount; ++i5)
                                {
                                    combPaiKey[5] = combPaiKey[4] + singlePaiKey[i5].key;

                                    if (!IsValidPaiKey(combPaiKey[5]))
                                        continue;

                                    if (IsCreateLaiziDetailData)
                                        laziData[5] = CombLaiZiToDataByKey(laziData[4], singlePaiKey[i5]);

                                    AddPaiKeyToDictHuPaiKeys(combPaiKey[5], laziData[5]);
                                }
                            }
                        }
                    }
                }
            }


            //组合将,顺子,刻子
            int singleJiangPaiKeyCount = setSingleJiang.Count;
            int[] singleJiangPaiKey = setSingleJiang.ToArray();

            Dictionary<int, byte>[] tmpDictHuPaiKeys = new Dictionary<int, byte>[MAX_HUPAI_NUM + 1];
            Dictionary<int, uint[]>[] tmpDictLaiziKeys = new Dictionary<int, uint[]>[MAX_HUPAI_NUM + 1];

            for (int j = 0; j < tmpDictHuPaiKeys.Length; ++j)
            {
                tmpDictHuPaiKeys[j] = new Dictionary<int, byte>(dictHuPaiKeys[j]);

                if (IsCreateLaiziDetailData)
                {
                    tmpDictLaiziKeys[j] = new Dictionary<int, uint[]>();
                    foreach (int key in dictLaiziKeys[j].Keys)
                    {
                        uint[] obj = (uint[])dictLaiziKeys[j][key].Clone();
                        tmpDictLaiziKeys[j].Add(key, obj);
                    }
                }
            }


            uint lzData = 0;
            bool isHaveJiangLaizi = false;
            byte jiangLaizi = 0;
            uint[] laziDatas = null;

            for (int i = 0; i < singleJiangPaiKeyCount; ++i)
            {
                //直接把将作为有效组合牌型存入dictHuPaiKeys
                if (IsCreateLaiziDetailData)
                {
                    isHaveJiangLaizi = dictLaiziSingleJiangKeys.ContainsKey(singleJiangPaiKey[i]);
                    if (isHaveJiangLaizi)
                    {
                        jiangLaizi = dictLaiziSingleJiangKeys[singleJiangPaiKey[i]];
                        lzData = CombLaiZiToData(0, jiangLaizi);
                    }
                }

                AddPaiKeyToDictHuPaiKeys(singleJiangPaiKey[i], lzData);

                //组合将,顺子,刻子(形成：顺子-将，刻子-将，顺子-刻子-将 的有效组合)
                for (int j = 0; j < tmpDictHuPaiKeys.Length; ++j)
                {
                    foreach (var item in tmpDictHuPaiKeys[j])
                    {
                        laziDatas = null;
                        int nTemp = singleJiangPaiKey[i] + item.Key + ((item.Value & BIT_VAL_FLAG) << ((max_pai_idx_num - 1) * BIT_VAL_NUM));

                        if (!IsValidPaiKey(nTemp))
                            continue;

                        if (!IsCreateLaiziDetailData)
                        {
                            AddPaiKeyToDictHuPaiKeys(nTemp, lzData);
                        }
                        else
                        {
                            if (tmpDictLaiziKeys[j].ContainsKey(item.Key))
                            {
                                laziDatas = tmpDictLaiziKeys[j][item.Key];

                                byte laiziAmount = (byte)((nTemp >> (BIT_VAL_NUM * (max_pai_idx_num - 1))) & BIT_VAL_FLAG);
                                uint[] newLaziDatas;

                                if (isHaveJiangLaizi)
                                {
                                    if (laiziAmount <= recordLaziDataLimitCount)
                                    {
                                        newLaziDatas = new uint[laziPaisCount];

                                        for (int k = 1; k <= laziDatas[0]; k++)
                                            newLaziDatas[k] = CombLaiZiToData(laziDatas[k], jiangLaizi);
                                    }
                                    else
                                    {
                                        newLaziDatas = new uint[1];
                                    }
                                }
                                else
                                {
                                    if (laiziAmount <= recordLaziDataLimitCount)
                                    {
                                        newLaziDatas = new uint[laziPaisCount];
                                        for (int k = 1; k <= laziDatas[0]; k++)
                                            newLaziDatas[k] = laziDatas[k];
                                    }
                                    else
                                    {
                                        newLaziDatas = new uint[1];
                                    }
                                }
                                newLaziDatas[0] = laziDatas[0];

                                AddPaiKeyToDictHuPaiKeys(nTemp, newLaziDatas);
                            }
                            else
                            {
                                AddPaiKeyToDictHuPaiKeys(nTemp, lzData);
                            }
                        }
                    }
                }
            }
        }

        public uint[] CheckCanHuSingle(MJColor type, byte[] paiIndexs, ref byte outNeedLaiziCount, byte laiziMaxCount)
        {
            int paiKey = GetPaiKeyByPaiIndexs(paiIndexs);
            if (type == MJColor.FenZi)
                paiKey &= 0x1FFFFF;

            byte count = GetPaiAmountByPaiKey(paiKey);

            if (dictHuPaiKeys[count].ContainsKey(paiKey))
            {
                outNeedLaiziCount = dictHuPaiKeys[count][paiKey];

                if (outNeedLaiziCount <= laiziMaxCount)
                {
                    uint[] data = retSucessTempLaiDataData;
                    if (dictLaiziKeys != null && dictLaiziKeys[count].ContainsKey(paiKey))
                        data = dictLaiziKeys[count][paiKey];
                    return data;
                }
            }

            outNeedLaiziCount = 0;
            return null;
        }

        public void Train()
        {
            if (setSingle.Count != 0)
                return;

            CreateSingleVaildPaiTypeKey();
            CreateCombPaiTypeVaildKeyToDict();
        }


        public void CreateKeyDataToFile(string fileName)
        {
            byte val;
            // C:\Users\username\AppData\LocalLow\company name\product name
            FileStream fs = File.Open(Application.persistentDataPath + "\\" + fileName, FileMode.Create, FileAccess.Write);
            BinaryWriter bw = new BinaryWriter(fs);

            bw.Write((byte)MAX_HUPAI_NUM);

            for (int i = 0; i < dictHuPaiKeys.Length; ++i)
            {
                foreach (var item in dictHuPaiKeys[i])
                {
                    val = (byte)(((i & 0x1f) << 3) | (item.Value & 0x7));
                    bw.Write(val);
                    bw.Write(item.Key);
                }
            }

            bw.Flush();//清除缓冲区
            bw.Close();//关闭流
        }

        public void TrainByKeyDataFile(string fileName, bool isCreateDataFile = true)
        {
            byte val;
            int key;

            string filePath = Application.persistentDataPath + "\\" + fileName;

            if (!File.Exists(filePath) && isCreateDataFile)
            {
                Train();
                CreateKeyDataToFile(fileName);
                return;
            }

            FileStream fs = File.Open(filePath, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);

            if (br.PeekChar() > -1)
            {
                int hupaiNum = br.ReadByte();
                if (hupaiNum != MAX_HUPAI_NUM)
                {
                    MAX_HUPAI_NUM = hupaiNum;
                    dictHuPaiKeys = new Dictionary<int, byte>[MAX_HUPAI_NUM + 1];
                    for (int i = 0; i < dictHuPaiKeys.Length; i++)
                        dictHuPaiKeys[i] = new Dictionary<int, byte>();
                }
            }

            int idx;
            byte value;

            while (br.PeekChar() > -1)
            {
                val = br.ReadByte();
                key = br.ReadInt32();
                idx = val >> 3;
                value = (byte)(val & 0x7);
                dictHuPaiKeys[idx][key] = value;
            }

            br.Close();
        }
    }


    public class MahjongHuTingCheck
    {
        MahjongHuPaiData mjHuPaiData;
        MahjongHuPaiData mjHuPaiDataFengZi;

        public int RecordLaziDataLimitCount
        {
            get
            {
                return mjHuPaiData.RecordLaziDataLimitCount;
            }
            set
            {
                mjHuPaiData.RecordLaziDataLimitCount = value;
                mjHuPaiDataFengZi.RecordLaziDataLimitCount = value;
            }
        }

        public bool IsCreateLaiziDetailData
        {
            get
            {
                return mjHuPaiData.IsCreateLaiziDetailData;
            }
            set
            {
                mjHuPaiData.IsCreateLaiziDetailData = value;
                mjHuPaiDataFengZi.IsCreateLaiziDetailData = value;
            }
        }

        public int MaxHuPaiAmount
        {
            get
            {
                return mjHuPaiData.MaxHuPaiAmount;
            }
            set
            {
                mjHuPaiData.MaxHuPaiAmount = value;
                mjHuPaiDataFengZi.MaxHuPaiAmount = value;
            }
        }

        public MahjongHuTingCheck()
        {
            mjHuPaiData = new MahjongHuPaiData(10);
            mjHuPaiData.IsCreateLaiziDetailData = true;

            mjHuPaiDataFengZi = new MahjongHuPaiData(8);
            mjHuPaiDataFengZi.IsCreateSingleShunZiVaildPaiType = false;
            mjHuPaiDataFengZi.IsCreateLaiziDetailData = true;
        }

        public void Train()
        {
            mjHuPaiData.Train();
            mjHuPaiDataFengZi.Train();
        }

        public void CreateKeyDataToFile(string keyDataFileName, string keyDataFengZiFileName)
        {
            mjHuPaiData.CreateKeyDataToFile(keyDataFileName);
            mjHuPaiDataFengZi.CreateKeyDataToFile(keyDataFengZiFileName);
        }

        public void TrainByKeyDataFile(string keyDataFileName, string keyDataFengZiFileName, bool isCreateDataFile = true)
        {
            mjHuPaiData.TrainByKeyDataFile(keyDataFileName, isCreateDataFile);
            mjHuPaiDataFengZi.TrainByKeyDataFile(keyDataFengZiFileName, isCreateDataFile);
        }

        public bool CheckCanHu(byte[] cardSrc, byte laiziIndex)
        {
            byte[] wanCard = new byte[9];
            byte[] tongCard = new byte[9];
            byte[] tiaoCard = new byte[9];
            byte[] fengziCard = new byte[7];

            byte[][] cardArray = new byte[][]
            {
            wanCard, tongCard, tiaoCard, fengziCard
            };

            Array.Copy(cardSrc, wanCard, 9);
            Array.Copy(cardSrc, 9, tongCard, 0, 9);
            Array.Copy(cardSrc, 18, tiaoCard, 0, 9);
            Array.Copy(cardSrc, 27, fengziCard, 0, 7);

            byte richLaiziCount = 0;



            if (laiziIndex >= 0 && laiziIndex < 9)
            {
                richLaiziCount = wanCard[laiziIndex];
                wanCard[laiziIndex] = 0;
            }
            else if (laiziIndex < 18)
            {
                richLaiziCount = tongCard[laiziIndex - 9];
                tongCard[laiziIndex - 9] = 0;
            }
            else if (laiziIndex < 27)
            {
                richLaiziCount = tiaoCard[laiziIndex - 18];
                tiaoCard[laiziIndex - 18] = 0;
            }
            else if (laiziIndex < 34)
            {
                richLaiziCount = fengziCard[laiziIndex - 27];
                fengziCard[laiziIndex - 27] = 0;
            }

            byte jiangCount = 0;
            byte needLaiziCount = 0;
            uint[] laiziDatas;

            for (int cor = 0; cor < (int)MJColor.Max; ++cor)
            {
                int nMax = (cor == (int)MJColor.FenZi) ? 7 : 9;
                int totalCardCount = 0;

                for (int i = 0; i < nMax; ++i)
                    totalCardCount += cardArray[cor][i];

                if (totalCardCount == 0)
                    continue;

                if (cor == (int)MJColor.FenZi)
                    laiziDatas = mjHuPaiDataFengZi.CheckCanHuSingle((MJColor)cor, cardArray[cor], ref needLaiziCount, richLaiziCount);
                else
                    laiziDatas = mjHuPaiData.CheckCanHuSingle((MJColor)cor, cardArray[cor], ref needLaiziCount, richLaiziCount);

                if (laiziDatas == null)
                    return false;


                richLaiziCount -= needLaiziCount;

                if ((totalCardCount + needLaiziCount) % 3 == 2)
                    jiangCount += 1;


                if (jiangCount > richLaiziCount + 1)
                    return false;
            }

            return jiangCount > 0 || richLaiziCount >= 2;
        }


        public void CheckTing(byte[] cards, byte laiziIndex, ref Dictionary<int, List<int>> tingPaiDict)
        {
            for (int i = 0; i < cards.Length; i++)
            {
                if (i == laiziIndex || cards[i] == 0)
                    continue;

                cards[i]--;

                for (int cor = 0; cor < (int)MJColor.Max; ++cor)
                {
                    int nMax = (cor == (int)MJColor.FenZi) ? 7 : 9;
                    int offset = cor * 9;

                    for (int j = 0; j < nMax; j++)
                    {
                        if (cards[offset + j] >= 4)
                            continue;

                        cards[offset + j]++;

                        if (CheckCanHu(cards, laiziIndex))
                        {
                            if (tingPaiDict == null)
                                tingPaiDict = new Dictionary<int, List<int>>();

                            AddDataToTingPaiDict(tingPaiDict, i, offset + j);
                        }

                        cards[offset + j]--;
                    }
                }

                cards[i]++;
            }
        }

        void AddDataToTingPaiDict(Dictionary<int, List<int>> tingPaiDict, int key, int value)
        {
            if (tingPaiDict == null)
                return;

            List<int> tingHuPaiList;

            if (tingPaiDict.ContainsKey(key))
            {
                tingHuPaiList = tingPaiDict[key];
            }
            else
            {
                tingHuPaiList = new List<int>();
                tingPaiDict[key] = tingHuPaiList;
            }

            for (int i = 0; i < tingHuPaiList.Count; i++)
            {
                if (tingHuPaiList[i] == value)
                    return;
            }

            tingHuPaiList.Add(value);
        }

        public void CheckTing(byte[] cards, byte laiziIndex, ref TingData[] tingDatas)
        {

            int tingIdx = 0;
            tingDatas[0].tingCardIdx = -1;
            tingDatas[0].huCardsEndIdx = -1;

            for (int i = 0; i < cards.Length; i++)
            {
                if (i == laiziIndex || cards[i] == 0)
                    continue;

                cards[i]--;

                for (int cor = 0; cor < (int)MJColor.Max; ++cor)
                {
                    int nMax = (cor == (int)MJColor.FenZi) ? 7 : 9;
                    int offset = cor * 9;

                    for (int j = 0; j < nMax; j++)
                    {
                        if (cards[offset + j] >= 4)
                            continue;

                        cards[offset + j]++;

                        if (CheckCanHu(cards, laiziIndex))
                        {
                            if (tingDatas == null)
                                tingDatas = CreateTingDataMemory();

                            AddDataToTingDatas(tingDatas, tingIdx, i, (byte)(offset + j));
                        }

                        cards[offset + j]--;
                    }
                }

                cards[i]++;

                if (tingDatas[tingIdx].tingCardIdx != -1)
                    tingIdx++;

            }
        }

        public TingData[] CreateTingDataMemory()
        {
            TingData[] tingDatas = new TingData[mjHuPaiData.MAX_HUPAI_NUM + 1];
            for (int m = 0; m < mjHuPaiData.MAX_HUPAI_NUM + 1; m++)
                tingDatas[m].huCards = new byte[mjHuPaiData.MAX_HUPAI_NUM + 1];

            tingDatas[0].tingCardIdx = -1;
            tingDatas[0].huCardsEndIdx = -1;

            return tingDatas;
        }

        void AddDataToTingDatas(TingData[] tingDatas, int tingIdx, int tingCardIdx, byte huCardIdx)
        {
            if (tingDatas == null)
                return;

            tingDatas[tingIdx].tingCardIdx = tingCardIdx;
            tingDatas[tingIdx].huCardsEndIdx++;
            tingDatas[tingIdx].huCards[tingDatas[tingIdx].huCardsEndIdx] = huCardIdx;

            tingDatas[tingIdx + 1].tingCardIdx = -1;
            tingDatas[tingIdx + 1].huCardsEndIdx = -1;
        }

    }
}