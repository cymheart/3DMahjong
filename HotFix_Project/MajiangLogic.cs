

using Assets;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HotFix_Project
{
    public class MajiangLogic
    {
        App app;
        MajiangGame mjGame;
        GameObject gameObject;

        Button btnXiPai;
        Button btnFenPai;

        Vector3 desk_mjtuo_west_pos;
        Vector3 desk_mjtuo_east_pos;
        Vector3 desk_mjtuo_north_pos;
        Vector3 desk_mjtuo_self_pos;

        MajiangGameState state = MajiangGameState.MG_STATE_END;

        GameObject[] mjDuiPai = new GameObject[145];

        List<GameObject> mjHandEastPaiList = new List<GameObject>();
        List<GameObject> mjHandSelfPaiList = new List<GameObject>();
        List<GameObject> mjHandWestPaiList = new List<GameObject>();
        List<GameObject> mjHandNorthPaiList = new List<GameObject>();

        List<GameObject> mjHandSelfFanPaiList = new List<GameObject>();

        List<GameObject>[] playerOrderMjPaiLists = new List<GameObject>[4];

        List<MajiangFaceValue> mjHandSelfPaiFaceValueList = new List<MajiangFaceValue>();

        Transform mjtableTransform;

        int dealerSeatIdx;

        float fenPaiSpeed = 0.2f;
        float fanPaiSpeed = 0.3f;

     
        public void Start(MajiangGame mjGame)
        {
            this.app = ResPool.app;
            this.mjGame = mjGame;
            this.gameObject = mjGame.gameObject;
          

            mjtableTransform = gameObject.transform.Find("mjtable");
            desk_mjtuo_west_pos = mjtableTransform.Find("desk_mjtuo_west").localPosition;
            desk_mjtuo_east_pos = mjtableTransform.Find("desk_mjtuo_east").localPosition;
            desk_mjtuo_north_pos = mjtableTransform.Find("desk_mjtuo_north").localPosition;
            desk_mjtuo_self_pos = mjtableTransform.Find("desk_mjtuo_self").localPosition;

   
            MajiangFaceValue[] values = new MajiangFaceValue[]
            {
            MajiangFaceValue.MJ_WANG_3, MajiangFaceValue.MJ_WANG_6, MajiangFaceValue.MJ_TONG_6, MajiangFaceValue.MJ_TONG_1,
            MajiangFaceValue.MJ_TONG_8, MajiangFaceValue.MJ_TIAO_8,MajiangFaceValue.MJ_TIAO_5,MajiangFaceValue.MJ_TIAO_4,
            MajiangFaceValue.MJ_FENG_DONG,MajiangFaceValue.MJ_HUA_CHUN,MajiangFaceValue.MJ_TONG_2,MajiangFaceValue.MJ_FENG_XI,
            MajiangFaceValue.MJ_WANG_7
            };

            SetSelfHandPaiFaceValueList(values);

            CreateMajiangPaiDui();

            TestMajiangFuncInterface();
        }

        void TestMajiangFuncInterface()
        {
            Canvas canvas = gameObject.transform.Find("Canvas").GetComponent<Canvas>();
            btnXiPai = canvas.transform.Find("btnXiPai").GetComponent<Button>();
            EventTriggerListener.Get(btnXiPai.gameObject).onClick = OnButtonClick;

            btnFenPai = canvas.transform.Find("btnFenPai").GetComponent<Button>();
            EventTriggerListener.Get(btnFenPai.gameObject).onClick = OnButtonClick;
        }
        private void OnButtonClick(GameObject go)
        {
            if (go == btnXiPai.gameObject)
            {
              
                XiPai();
            }
            else if (go == btnFenPai.gameObject)
            {
                FenPai();
            }
        }


        void SetSelfHandPaiFaceValueList(MajiangFaceValue[] mjFaceValueList)
        {
            mjHandSelfPaiFaceValueList.Clear();
            foreach (MajiangFaceValue value in mjFaceValueList)
                mjHandSelfPaiFaceValueList.Add(value);
        }

        MajiangFaceValue[] GetSelfHandMajiangFaceValues(int mjHandPosIdx, int mjCount)
        {
            if (mjHandSelfPaiFaceValueList.Count < mjCount + mjHandPosIdx)
                return null;

            MajiangFaceValue[] faceValues = new MajiangFaceValue[mjCount];
            int n = 0;

            for (int i = mjHandPosIdx; i < mjHandPosIdx + mjCount; i++)
            {
                faceValues[n++] = mjHandSelfPaiFaceValueList[i];
            }

            return faceValues;
        }

        void CreateMajiangPaiDui()
        {
            //自身牌堆生成
            int paiIdx = 1;
            Transform mjtuo_self_tf = mjtableTransform.Find("desk_mjtuo_self");
            Vector3 orgLocalPos = app.mj_dui_zheng.transform.localPosition;
            orgLocalPos.y = -0.01070001f;
            for (int i = 0; i < 18; i++)
            {
                GameObject mj = Object.Instantiate(app.mj_dui_zheng);
                mj.name = "mjDuiPai_" + paiIdx;
                Vector3 newPos = new Vector3(orgLocalPos.x + 0.0252f * i, orgLocalPos.y, orgLocalPos.z);
                mj.transform.localPosition = newPos;
                mj.transform.SetParent(mjtuo_self_tf, false);

                mjDuiPai[paiIdx] = mj;
                paiIdx += 2;
            }

            paiIdx = 2;
            orgLocalPos = app.mj_dui_zheng_shadow.transform.localPosition;
            orgLocalPos.y = -0.01070001f;
            for (int i = 0; i < 18; i++)
            {
                GameObject mj = Object.Instantiate(app.mj_dui_zheng_shadow);
                mj.name = "mjDuiPai_" + paiIdx;
                Vector3 newPos = new Vector3(orgLocalPos.x + 0.0252f * i, orgLocalPos.y, orgLocalPos.z);
                mj.transform.localPosition = newPos;
                mj.transform.SetParent(mjtuo_self_tf, false);

                mjDuiPai[paiIdx] = mj;
                paiIdx += 2;
            }


            //手西边牌堆生成
            paiIdx = 37;
            Transform mjtuo_west_tf = mjtableTransform.Find("desk_mjtuo_west");
            orgLocalPos = app.mj_dui_west.transform.localPosition;
            for (int i = 0; i < 18; i++)
            {
                GameObject mj = Object.Instantiate(app.mj_dui_west);
                mj.name = "mjDuiPai_" + paiIdx;
                Vector3 newPos = new Vector3(orgLocalPos.x, orgLocalPos.y + 0.0255f * i, orgLocalPos.z);
                mj.transform.localPosition = newPos;
                mj.transform.SetParent(mjtuo_west_tf, false);

                mjDuiPai[paiIdx] = mj;
                paiIdx += 2;
            }

            paiIdx = 38;
            orgLocalPos = app.mj_dui_west_shadow.transform.localPosition;
            for (int i = 0; i < 18; i++)
            {
                GameObject mj = Object.Instantiate(app.mj_dui_west_shadow);
                mj.name = "mjDuiPai_" + paiIdx;
                Vector3 newPos = new Vector3(orgLocalPos.x, orgLocalPos.y + 0.0255f * i, orgLocalPos.z);
                mj.transform.localPosition = newPos;
                mj.transform.SetParent(mjtuo_west_tf, false);

                mjDuiPai[paiIdx] = mj;
                paiIdx += 2;
            }

            //正南边牌堆生成
            paiIdx = 107;
            Transform mjtuo_north_tf = mjtableTransform.Find("desk_mjtuo_north");
            orgLocalPos = app.mj_dui_zheng.transform.localPosition;
            for (int i = 0; i < 18; i++)
            {
                GameObject mj = Object.Instantiate(app.mj_dui_zheng);
                mj.name = "mjDuiPai_" + paiIdx;
                Vector3 newPos = new Vector3(orgLocalPos.x + 0.0252f * i, orgLocalPos.y, orgLocalPos.z);
                mj.transform.localPosition = newPos;
                mj.transform.SetParent(mjtuo_north_tf, false);

                mjDuiPai[paiIdx] = mj;
                paiIdx -= 2;
            }

            paiIdx = 108;
            orgLocalPos = app.mj_dui_zheng_shadow.transform.localPosition;
            for (int i = 0; i < 18; i++)
            {
                GameObject mj = Object.Instantiate(app.mj_dui_zheng_shadow);
                mj.name = "mjDuiPai_" + paiIdx;
                Vector3 newPos = new Vector3(orgLocalPos.x + 0.0252f * i, orgLocalPos.y, orgLocalPos.z);
                mj.transform.localPosition = newPos;
                mj.transform.SetParent(mjtuo_north_tf, false);

                mjDuiPai[paiIdx] = mj;
                paiIdx -= 2;
            }


            //手东边牌堆生成
            paiIdx = 143;
            Transform mjtuo_east_tf = mjtableTransform.Find("desk_mjtuo_east");
            orgLocalPos = app.mj_dui_east.transform.localPosition;
            for (int i = 0; i < 18; i++)
            {
                GameObject mj = Object.Instantiate(app.mj_dui_east);
                mj.name = "mjDuiPai_" + paiIdx;
                Vector3 newPos = new Vector3(orgLocalPos.x, orgLocalPos.y + 0.0255f * i, orgLocalPos.z);
                mj.transform.localPosition = newPos;
                mj.transform.SetParent(mjtuo_east_tf, false);

                mjDuiPai[paiIdx] = mj;
                paiIdx -= 2;
            }

            paiIdx = 144;
            orgLocalPos = app.mj_dui_east_shadow.transform.localPosition;
            for (int i = 0; i < 18; i++)
            {
                GameObject mj = Object.Instantiate(app.mj_dui_east_shadow);
                mj.name = "mjDuiPai_" + paiIdx;
                Vector3 newPos = new Vector3(orgLocalPos.x, orgLocalPos.y + 0.0255f * i, orgLocalPos.z);
                mj.transform.localPosition = newPos;
                mj.transform.SetParent(mjtuo_east_tf, false);

                mjDuiPai[paiIdx] = mj;
                paiIdx -= 2;
            }
        }


        public void XiPai()
        {
            if (!(state == MajiangGameState.MG_STATE_END
                || state == MajiangGameState.MG_STATE_XIPAI_END
                || state == MajiangGameState.MG_STATE_FENPAI_END))
            {
                UnityEngine.Debug.Log(state);
                return;
            }

            state = MajiangGameState.MG_STATE_XIPAING;

            List<GameObject> mjHandList;
            for (int i = 0; i < 4; i++)
            {
                mjHandList = playerOrderMjPaiLists[i];
                if (mjHandList == null)
                    continue;

                foreach (GameObject mj in mjHandList)
                    Object.Destroy(mj);

                mjHandList.Clear();
            }

            foreach (GameObject mj in mjHandSelfFanPaiList)
                Object.Destroy(mj);
            mjHandSelfFanPaiList.Clear();


            foreach (GameObject mj in mjDuiPai)
            {
                if (mj == null)
                    continue;
                if (mj.activeSelf == false)
                    mj.SetActive(true);
            }

            XiPai("desk_mjtuo_west", desk_mjtuo_west_pos, 0.2416f, MajiangGameDir.MG_DIR_X);
            XiPai("desk_mjtuo_east", desk_mjtuo_east_pos, -0.2416f, MajiangGameDir.MG_DIR_X);
            XiPai("desk_mjtuo_north", desk_mjtuo_north_pos, -0.2185f, MajiangGameDir.MG_DIR_Z);
            XiPai("desk_mjtuo_self", desk_mjtuo_self_pos, 0.2185f, MajiangGameDir.MG_DIR_Z);
        }

        void XiPai(string mjtuoName, Vector3 mjtuoOrgPos, float xorzOffsetDstValue, MajiangGameDir dir)
        {
            Transform mjtuo_tf = mjtableTransform.Find(mjtuoName);
            Vector3 orgLocalPos = mjtuoOrgPos;

            Sequence seq = DOTween.Sequence();
            Tweener t;

            if (dir == MajiangGameDir.MG_DIR_X)
            {
                orgLocalPos.x = xorzOffsetDstValue;
                orgLocalPos.y = 0.02f;
                mjtuo_tf.localPosition = orgLocalPos;

                t = mjtuo_tf.DOLocalMoveX(mjtuoOrgPos.x, 0.5f).SetEase(Ease.Flash);
                seq.Append(t);

                t = mjtuo_tf.DOLocalMoveY(mjtuoOrgPos.y, 0.5f).SetEase(Ease.Flash);
                seq.Append(t);
            }
            else
            {
                orgLocalPos.z = xorzOffsetDstValue;
                orgLocalPos.y = 0.02f;
                mjtuo_tf.localPosition = orgLocalPos;

                t = mjtuo_tf.DOLocalMoveZ(mjtuoOrgPos.z, 0.5f).SetEase(Ease.Flash);
                seq.Append(t);

                t = mjtuo_tf.DOLocalMoveY(mjtuoOrgPos.y, 0.5f).SetEase(Ease.Flash);
                seq.Append(t);
            }

            mjGame.StartCoroutine(XiPaiOnComplete(1f + 0.1f));
        }

        IEnumerator XiPaiOnComplete(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);

            state = MajiangGameState.MG_STATE_XIPAI_END;
        }


        void FenPai()
        {
            if (state != MajiangGameState.MG_STATE_XIPAI_END)
                return;

            SetDealer(0);
            mjGame.StartCoroutine(FenPai(20));
        }

        IEnumerator FenPai(int startPaiDuiIdx)
        {
            state = MajiangGameState.MG_STATE_FENPAING;

            int startPaiIdx = startPaiDuiIdx * 2 - 1;
            int playerOrderIdx = 0;
            int fanPaiPosIdx = 0;
            int showCount = 4;

            while (fanPaiPosIdx < 4)
            {
                int seatIdx = GetSeatIdx(playerOrderMjPaiLists[playerOrderIdx]);
                MoMajiangPai(startPaiIdx, showCount);
                FanMajiangPai(seatIdx, fanPaiPosIdx, showCount);

                yield return new WaitForSeconds(fenPaiSpeed);

                playerOrderIdx++;
                startPaiIdx += showCount;

                if (startPaiIdx > 144)
                    startPaiIdx -= 144;

                //已经摸牌一圈
                if (playerOrderIdx == 4)
                {
                    playerOrderIdx = 0;
                    fanPaiPosIdx++;
                    if (fanPaiPosIdx == 3)
                        showCount = 1;
                }
            }

            yield return new WaitForSeconds(0.5f);
            //
            Vector3 dstPos = new Vector3(0.2896f, 0.1002f, 0.3601f);
            Vector3 dstPos2 = new Vector3(0.2896f, 0.1021f, 0.4144f);
            GameObject mj;
            Tweener t;

            for (int j = 0; j < mjHandSelfFanPaiList.Count; j++)
            {
                mj = mjHandSelfFanPaiList[j];
                Sequence seq = DOTween.Sequence();

                t = mj.transform.DOLocalMove(new Vector3(dstPos.x - j * 0.0488f, dstPos.y, dstPos.z), fanPaiSpeed).SetEase(Ease.Flash);
                seq.Append(t);
                t = mj.transform.DOLocalRotate(new Vector3(-112, 0, 0), fanPaiSpeed, RotateMode.LocalAxisAdd).SetEase(Ease.Flash);
                seq.Insert(0, t);

                t = mj.transform.DOLocalMove(new Vector3(dstPos2.x - j * 0.0488f, dstPos2.y, dstPos2.z), fanPaiSpeed).SetEase(Ease.Flash);
                seq.Append(t);
                t = mj.transform.DOLocalRotate(new Vector3(112, 0, 0), fanPaiSpeed, RotateMode.LocalAxisAdd).SetEase(Ease.Flash);
                seq.Insert(fanPaiSpeed, t);
            }

            mjGame.StartCoroutine(RotateSelfHandMjOnComplete(fanPaiSpeed * 2 + 0.1f));
        }

        IEnumerator RotateSelfHandMjOnComplete(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);

            state = MajiangGameState.MG_STATE_FENPAI_END;

            Transform canvasTransform = gameObject.transform.Find("Canvas");
            Vector3 pos = app.selfHandPai.transform.localPosition;
            float x = pos.x;

            for (int j = 0; j < mjHandSelfFanPaiList.Count; j++)
            {
                if (mjHandSelfPaiList.Count == j && j != 0)
                {
                    x = mjHandSelfPaiList[mjHandSelfPaiList.Count - 1].transform.localPosition.x;
                    x += 111f;
                }

                GameObject mj = Object.Instantiate(app.selfHandPai);
                Transform faceTransform = mj.transform.Find("face");
                faceTransform.gameObject.GetComponent<Image>().sprite = app.mjAtlas.GetSprite(app.mjSpriteNameDict[mjHandSelfPaiFaceValueList[j]]);

                mj.transform.localPosition = new Vector3(x, pos.y, pos.z);
                mj.transform.SetParent(canvasTransform, false);
                mjHandSelfPaiList.Add(mj);
            }

            foreach (GameObject mj in mjHandSelfFanPaiList)
                Object.Destroy(mj);
            mjHandSelfFanPaiList.Clear();
        }

        int GetSeatIdx(List<GameObject> playerOrderMjPai)
        {
            int seatIdx = 0;

            if (playerOrderMjPai == mjHandEastPaiList)
                seatIdx = 1;
            else if (playerOrderMjPai == mjHandNorthPaiList)
                seatIdx = 2;
            else if (playerOrderMjPai == mjHandWestPaiList)
                seatIdx = 3;

            return seatIdx;
        }


        void MoMajiangPai(int moPaiStartIdx, int moPaiCount)
        {
            for (int i = 0; i < moPaiCount; i++)
            {
                if (moPaiStartIdx == 145)
                    moPaiStartIdx = 1;

                mjDuiPai[moPaiStartIdx].SetActive(false);
                moPaiStartIdx++;
            }
        }

        GameObject[] CreateMajiangPaiFan(int seatIdx, int count)
        {
            GameObject[] mjHandFanPai = new GameObject[count];
            Vector3 orgLocalPosMjFan;
            Vector3 orgLocalRotMjFan;

            switch (seatIdx)
            {
                case 0:
                    orgLocalPosMjFan = app.mj_hand_main.transform.localPosition;
                    orgLocalRotMjFan = app.mj_hand_main.transform.localEulerAngles;

                    for (int i = 0; i < count; i++)
                    {
                        GameObject mj = Object.Instantiate(app.mj_hand_main);
                        Vector3 newPos = new Vector3(orgLocalPosMjFan.x - 0.0488f * i, orgLocalPosMjFan.y, orgLocalPosMjFan.z);
                        mj.transform.localPosition = newPos;
                        mj.transform.localEulerAngles = orgLocalRotMjFan;

                        mj.transform.SetParent(mjtableTransform, false);

                        mjHandFanPai[i] = mj;
                    }

                    break;

                case 1:
                case 2:
                case 3:

                    orgLocalPosMjFan = app.mj_hand_fan.transform.localPosition;
                    orgLocalRotMjFan = app.mj_hand_fan.transform.localEulerAngles;
                    orgLocalRotMjFan.x = -25f;

                    for (int i = 0; i < count; i++)
                    {
                        GameObject mj = Object.Instantiate(app.mj_hand_fan);
                        Vector3 newPos = new Vector3(orgLocalPosMjFan.x, orgLocalPosMjFan.y, orgLocalPosMjFan.z - 0.0335f * i);
                        mj.transform.localPosition = newPos;
                        mj.transform.localEulerAngles = orgLocalRotMjFan;

                        mj.transform.SetParent(mjtableTransform, false);

                        mjHandFanPai[i] = mj;
                    }
                    break;
            }

            return mjHandFanPai;
        }

        void FanMajiangPai(int seatIdx, int posIdx, int showCount)
        {
            GameObject[] mjHandFanPai = CreateMajiangPaiFan(seatIdx, showCount);
            Vector3 orgLocalPosMjFan;
            Vector3 orgLocalRotMjFan;

            switch (seatIdx)
            {
                case 0:
                    {
                        Vector3 dstPos = new Vector3(0.2896f, 0.1021f, 0.4144f);
                        GameObject mj;
                        Material mat;
                        Tweener t;
                        MajiangFaceValue[] mjFaceValues = GetSelfHandMajiangFaceValues(posIdx * 4, mjHandFanPai.Length);


                        for (int i = 0; i < mjHandFanPai.Length; i++)
                        {
                            mj = mjHandFanPai[i];

                            if (mjFaceValues != null && i < mjFaceValues.Length)
                            {
                                Transform faceTransform = mj.transform.Find("face");
                                faceTransform.gameObject.GetComponent<SpriteRenderer>().sprite = app.mjAtlas.GetSprite(app.mjSpriteNameDict[mjFaceValues[i]]);
                            }

                            Sequence seq = DOTween.Sequence();
                            mat = mj.GetComponent<Renderer>().material;

                            for (int j = 0; j < 2; j++)
                            {
                                t = mat.DOFloat(1, "_LightLevel", 0.3f);
                                seq = seq.Append(t);

                                t = mat.DOFloat(0, "_LightLevel", 0.2f);
                                seq = seq.Append(t);
                            }

                            Vector3 newPos = mjHandFanPai[i].transform.localPosition;
                            newPos.x -= posIdx * 0.0488f * 4;
                            mj.transform.localPosition = newPos;

                            mj.transform.DOLocalMove(new Vector3(dstPos.x - (posIdx * 0.0488f * 4 + i * 0.0488f), dstPos.y, dstPos.z), fanPaiSpeed).SetEase(Ease.Flash);
                            mj.transform.DOLocalRotate(new Vector3(112, 0, 0), fanPaiSpeed, RotateMode.LocalAxisAdd).SetEase(Ease.Flash);
                            mjGame.StartCoroutine(RotateOnComplete(mj, seatIdx, fanPaiSpeed + 0.1f));
                        }
                    }
                    break;

                case 1:

                    orgLocalPosMjFan = app.mj_hand_fan.transform.localPosition;
                    orgLocalRotMjFan = app.mj_hand_fan.transform.localEulerAngles;
                    orgLocalRotMjFan.x = -25f;

                    for (int i = 0; i < mjHandFanPai.Length; i++)
                    {
                        Vector3 newPos = mjHandFanPai[i].transform.localPosition;
                        newPos.z -= posIdx * 0.0335f * 4;
                        mjHandFanPai[i].transform.localPosition = newPos;
                        mjHandFanPai[i].transform.DOLocalRotate(new Vector3(0, 90, 0), fanPaiSpeed).SetEase(Ease.Flash);
                        mjGame.StartCoroutine(RotateOnComplete(mjHandFanPai[i], seatIdx, fanPaiSpeed + 0.1f));
                    }
                    break;

                case 2:

                    orgLocalPosMjFan = app.mj_hand_fan.transform.localPosition;
                    orgLocalRotMjFan = app.mj_hand_fan.transform.localEulerAngles;
                    orgLocalRotMjFan.x = -25f;

                    for (int i = 0; i < mjHandFanPai.Length; i++)
                    {
                        Vector3 newPos = new Vector3(-0.2f + 0.0335f * i, 0.111f, -0.3464f);
                        newPos.x += posIdx * 0.0335f * 4;
                        Vector3 newRot = new Vector3(orgLocalRotMjFan.x, 0f, orgLocalRotMjFan.z);
                        mjHandFanPai[i].transform.localPosition = newPos;
                        mjHandFanPai[i].transform.localEulerAngles = newRot;
                        mjHandFanPai[i].transform.DOLocalRotate(new Vector3(0, 0, 0), fanPaiSpeed).SetEase(Ease.Flash);
                        mjGame.StartCoroutine(RotateOnComplete(mjHandFanPai[i], seatIdx, fanPaiSpeed + 0.1f));
                    }
                    break;

                case 3:

                    orgLocalPosMjFan = app.mj_hand_fan.transform.localPosition;
                    orgLocalRotMjFan = app.mj_hand_fan.transform.localEulerAngles;
                    orgLocalRotMjFan.x = -25f;

                    for (int i = 0; i < mjHandFanPai.Length; i++)
                    {
                        Vector3 newPos = new Vector3(0.376f, orgLocalPosMjFan.y, -0.195f + 0.0335f * i);
                        newPos.z += posIdx * 0.0335f * 4;

                        Vector3 newRot = new Vector3(orgLocalRotMjFan.x, -90f, orgLocalRotMjFan.z);

                        mjHandFanPai[i].transform.localPosition = newPos;
                        mjHandFanPai[i].transform.localEulerAngles = newRot;

                        mjHandFanPai[i].transform.DOLocalRotate(new Vector3(0, -90, 0), fanPaiSpeed).SetEase(Ease.Flash);
                        mjGame.StartCoroutine(RotateOnComplete(mjHandFanPai[i], seatIdx, fanPaiSpeed + 0.1f));
                    }
                    break;
            }
        }

        IEnumerator RotateOnComplete(GameObject mjHandFanPai, int seatIdx, float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            GameObject mj;


            switch (seatIdx)
            {
                case 0:
                    mjHandSelfFanPaiList.Add(mjHandFanPai);
                    break;

                case 1:
                    mj = Object.Instantiate(app.mj_hand_east);
                    mj.transform.localPosition = mjHandFanPai.transform.localPosition;
                    mj.transform.SetParent(mjtableTransform, false);
                    mjHandEastPaiList.Add(mj);
                    Object.Destroy(mjHandFanPai);
                    break;

                case 2:
                    mj = Object.Instantiate(app.mj_hand_north);
                    mj.transform.localPosition = mjHandFanPai.transform.localPosition;
                    mj.transform.SetParent(mjtableTransform, false);
                    mjHandNorthPaiList.Add(mj);
                    Object.Destroy(mjHandFanPai);
                    break;

                case 3:
                    mj = Object.Instantiate(app.mj_hand_west);
                    mj.transform.localPosition = mjHandFanPai.transform.localPosition;
                    mj.transform.SetParent(mjtableTransform, false);
                    mjHandWestPaiList.Add(mj);
                    Object.Destroy(mjHandFanPai);
                    break;
            }


        }

        void SetDealer(int dealerSeatIdx)
        {
            this.dealerSeatIdx = dealerSeatIdx;

            switch (dealerSeatIdx)
            {
                case 0:
                    playerOrderMjPaiLists[0] = mjHandSelfPaiList;
                    playerOrderMjPaiLists[1] = mjHandEastPaiList;
                    playerOrderMjPaiLists[2] = mjHandNorthPaiList;
                    playerOrderMjPaiLists[3] = mjHandWestPaiList;
                    break;

                case 1:
                    playerOrderMjPaiLists[0] = mjHandEastPaiList;
                    playerOrderMjPaiLists[1] = mjHandNorthPaiList;
                    playerOrderMjPaiLists[2] = mjHandWestPaiList;
                    playerOrderMjPaiLists[3] = mjHandSelfPaiList;
                    break;

                case 2:
                    playerOrderMjPaiLists[0] = mjHandNorthPaiList;
                    playerOrderMjPaiLists[1] = mjHandWestPaiList;
                    playerOrderMjPaiLists[2] = mjHandSelfPaiList;
                    playerOrderMjPaiLists[3] = mjHandEastPaiList;
                    break;

                case 3:
                    playerOrderMjPaiLists[0] = mjHandWestPaiList;
                    playerOrderMjPaiLists[1] = mjHandSelfPaiList;
                    playerOrderMjPaiLists[2] = mjHandEastPaiList;
                    playerOrderMjPaiLists[3] = mjHandNorthPaiList;
                    break;
            }
        }  
    }
}
