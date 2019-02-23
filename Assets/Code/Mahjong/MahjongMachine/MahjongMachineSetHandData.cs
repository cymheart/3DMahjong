using Assets;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MahjongMachineNS
{
    public partial class MahjongMachine
    {
        #region 生成手
        void CreateHands()
        {
            CreateHands(PlayerType.FEMALE, HandDirection.LeftHand);
            CreateHands(PlayerType.FEMALE, HandDirection.RightHand);
            CreateHands(PlayerType.MALE, HandDirection.LeftHand);
            CreateHands(PlayerType.MALE, HandDirection.RightHand);
        }
        void CreateHands(PlayerType handStyle, HandDirection handDir)
        {
            string sexStr = "Female";

            GameObject prefabHand = mjAssets.defaultPrefabDict[(int)PrefabIdx.FEMALE_HAND][0];

            if (handStyle == PlayerType.MALE)
            {
                //prefabHand = mjAssets.maleHand;
                sexStr = "Male";
            }

            for (int i = 0; i < 4; i++)
            {
                GameObject cpHand = Object.Instantiate(prefabHand);

                if (handDir == HandDirection.LeftHand)
                {
                    cpHand.name = sexStr + "LeftHand" + i;
                    Vector3 scale = cpHand.transform.localScale;
                    cpHand.transform.localScale = new Vector3(scale.x, scale.y, -scale.z);
                }
                else
                {
                    cpHand.name = sexStr + "RightHand" + i;
                }

                cpHand.transform.SetParent(gameObject.transform, false);
                cpHand.SetActive(false);
                SaveHand(i, handStyle, handDir, cpHand);
                SaveHandBones(i, handStyle, handDir);
            }
        }


        void DestroyAllHand()
        {
            for (int i = 0; i < 4; i++)
            {
                Object.Destroy(femaleLHands[i]);
                Object.Destroy(femaleRHands[i]);
                Object.Destroy(maleLHands[i]);
                Object.Destroy(maleRHands[i]);
            }
        }


        void SaveHand(int seat, PlayerType handStyle, HandDirection handDir, GameObject saveHand)
        {
            if (handStyle == PlayerType.FEMALE)
            {
                if (handDir == HandDirection.LeftHand)
                {
                    femaleLHands[seat] = saveHand;
                    femaleLHandsAnimation[seat] = saveHand.GetComponent<Animation>();

                    if (mjAssets.handActionDataInfoDicts.ContainsKey(saveHand.name))
                        femaleLHandsActionDataInfo[seat] = mjAssets.handActionDataInfoDicts[saveHand.name];
                }
                else
                {
                    femaleRHands[seat] = saveHand;
                    femaleRHandsAnimation[seat] = saveHand.GetComponent<Animation>();

                    if (mjAssets.handActionDataInfoDicts.ContainsKey(saveHand.name))
                        femaleRHandsActionDataInfo[seat] = mjAssets.handActionDataInfoDicts[saveHand.name];
                }
            }
            else
            {
                if (handDir == HandDirection.LeftHand)
                {
                    maleLHands[seat] = saveHand;
                    maleLHandsAnimation[seat] = saveHand.GetComponent<Animation>();

                    if (mjAssets.handActionDataInfoDicts.ContainsKey(saveHand.name))
                        maleLHandsActionDataInfo[seat] = mjAssets.handActionDataInfoDicts[saveHand.name];
                }
                else
                {
                    maleRHands[seat] = saveHand;
                    maleRHandsAnimation[seat] = saveHand.GetComponent<Animation>();

                    if (mjAssets.handActionDataInfoDicts.ContainsKey(saveHand.name))
                        maleRHandsActionDataInfo[seat] = mjAssets.handActionDataInfoDicts[saveHand.name];
                }
            }
        }

        void SaveHandBones(int seat, PlayerType handStyle, HandDirection handDir)
        {
            //插牌手部位置节点路径
            string[] chaPaiNodePathNames = new string[] { "rShldrTwist", "rForearmBend", "rForearmTwist", "rHand", "rThumb1", "rThumb2", "rThumb3" };

            //打牌手部位置节点路径
            string[] daPaiNodePathNames = new string[] { "rShldrTwist", "rForearmBend", "rForearmTwist", "rHand", "rCarpal1", "rIndex1", "rIndex2", "rIndex3", "Bone001" };

            //打牌手部位置节点路径2
            string[] daPaiNodePathNames2 = new string[] { "rShldrTwist", "rForearmBend", "rForearmTwist", "rHand", "rThumb1", "rThumb2", "rThumb3", "Bone002" };


            //打牌手部阴影1
            string[] handShadowPathNames1 = new string[] { "rShldrTwist", "shadow0" };
            string[] handShadowPathNames2 = new string[] { "rShldrTwist", "shadow1" };
            string[] handShadowPathNames3 = new string[] { "rShldrTwist", "shadow2" };

            if (handStyle == PlayerType.FEMALE)
            {
                if (handDir == HandDirection.LeftHand)
                {
                    femaleLHandMjBone[2, seat] = GetNodeTransformByNodePathNames(femaleLHands[seat], daPaiNodePathNames2);
                    femaleLHandMjBone[1, seat] = GetNodeTransformByNodePathNames(femaleLHands[seat], daPaiNodePathNames);
                    femaleLHandMjBone[0, seat] = GetNodeTransformByNodePathNames(femaleLHands[seat], chaPaiNodePathNames);

                    femaleLHandShadow[0, seat] = GetNodeTransformByNodePathNames(femaleLHands[seat], handShadowPathNames1);
                    femaleLHandShadow[1, seat] = GetNodeTransformByNodePathNames(femaleLHands[seat], handShadowPathNames2);
                    femaleLHandShadow[2, seat] = GetNodeTransformByNodePathNames(femaleLHands[seat], handShadowPathNames3);
                }
                else
                {
                    femaleRHandMjBone[2, seat] = GetNodeTransformByNodePathNames(femaleRHands[seat], daPaiNodePathNames2);
                    femaleRHandMjBone[1, seat] = GetNodeTransformByNodePathNames(femaleRHands[seat], daPaiNodePathNames);
                    femaleRHandMjBone[0, seat] = GetNodeTransformByNodePathNames(femaleRHands[seat], chaPaiNodePathNames);

                    femaleRHandShadow[0, seat] = GetNodeTransformByNodePathNames(femaleRHands[seat], handShadowPathNames1);
                    femaleRHandShadow[1, seat] = GetNodeTransformByNodePathNames(femaleRHands[seat], handShadowPathNames2);
                    femaleRHandShadow[2, seat] = GetNodeTransformByNodePathNames(femaleRHands[seat], handShadowPathNames3);
                }
            }
            else
            {
                if (handDir == HandDirection.LeftHand)
                {
                    maleLHandMjBone[2, seat] = GetNodeTransformByNodePathNames(maleLHands[seat], daPaiNodePathNames2);
                    maleLHandMjBone[1, seat] = GetNodeTransformByNodePathNames(maleLHands[seat], daPaiNodePathNames);
                    maleLHandMjBone[0, seat] = GetNodeTransformByNodePathNames(maleLHands[seat], chaPaiNodePathNames);

                    maleLHandShadow[0, seat] = GetNodeTransformByNodePathNames(maleLHands[seat], handShadowPathNames1);
                    maleLHandShadow[1, seat] = GetNodeTransformByNodePathNames(maleLHands[seat], handShadowPathNames2);
                    maleLHandShadow[2, seat] = GetNodeTransformByNodePathNames(maleLHands[seat], handShadowPathNames3);
                }
                else
                {
                    maleRHandMjBone[2, seat] = GetNodeTransformByNodePathNames(maleRHands[seat], daPaiNodePathNames2);
                    maleRHandMjBone[1, seat] = GetNodeTransformByNodePathNames(maleRHands[seat], daPaiNodePathNames);
                    maleRHandMjBone[0, seat] = GetNodeTransformByNodePathNames(maleRHands[seat], chaPaiNodePathNames);

                    maleRHandShadow[0, seat] = GetNodeTransformByNodePathNames(maleRHands[seat], handShadowPathNames1);
                    maleRHandShadow[1, seat] = GetNodeTransformByNodePathNames(maleRHands[seat], handShadowPathNames2);
                    maleRHandShadow[2, seat] = GetNodeTransformByNodePathNames(maleRHands[seat], handShadowPathNames3);
                }
            }
        }

        GameObject GetNodeTransformByNodePathNames(GameObject parentNode, string[] nodePathNames)
        {
            Transform cparent = parentNode.transform;
            foreach (string nodename in nodePathNames)
            {
                cparent = cparent.Find(nodename);
            }
            return cparent.gameObject;
        }

        GameObject GetHandBone(int seat, PlayerType handStyle, HandDirection handDir, int boneIdx)
        {
            if (handStyle == PlayerType.FEMALE)
            {
                if (handDir == HandDirection.LeftHand)
                {
                    return femaleLHandMjBone[boneIdx, seat];
                }
                else
                {
                    return femaleRHandMjBone[boneIdx, seat];
                }
            }
            else
            {
                if (handDir == HandDirection.LeftHand)
                {
                    return maleLHandMjBone[boneIdx, seat];
                }
                else
                {
                    return maleRHandMjBone[boneIdx, seat];
                }
            }
        }


        GameObject GetHandShadowAxis(int seat, PlayerType handStyle, HandDirection handDir, int shadowIdx)
        {
            if (handStyle == PlayerType.FEMALE)
            {
                if (handDir == HandDirection.LeftHand)
                {
                    return femaleLHandShadow[shadowIdx, seat];
                }
                else
                {
                    return femaleRHandShadow[shadowIdx, seat];
                }
            }
            else
            {
                if (handDir == HandDirection.LeftHand)
                {
                    return maleLHandShadow[shadowIdx, seat];
                }
                else
                {
                    return maleRHandShadow[shadowIdx, seat];
                }
            }
        }

        Dictionary<string, ActionDataInfo> GetHandActionDataDict(int seat, PlayerType handStyle, HandDirection handDir)
        {
            if (handStyle == PlayerType.FEMALE)
            {
                if (handDir == HandDirection.LeftHand)
                {
                    return femaleLHandsActionDataInfo[seat];
                }
                else
                {
                    return femaleRHandsActionDataInfo[seat];
                }
            }
            else
            {
                if (handDir == HandDirection.LeftHand)
                {
                    return maleLHandsActionDataInfo[seat];
                }
                else
                {
                    return maleRHandsActionDataInfo[seat];
                }
            }
        }


        /// <summary>
        /// 获取指定类型的手部物件
        /// </summary>
        /// <param name="seat"></param>
        /// <param name="handStyle"></param>
        /// <param name="handDir"></param>
        /// <returns></returns>
        GameObject GetHand(int seat, PlayerType handStyle, HandDirection handDir)
        {
            if (handStyle == PlayerType.FEMALE)
            {
                if (handDir == HandDirection.LeftHand)
                    return femaleLHands[seat];
                else
                    return femaleRHands[seat];
            }
            else
            {
                if (handDir == HandDirection.LeftHand)
                    return maleLHands[seat];
                else
                    return maleRHands[seat];
            }

        }

        /// <summary>
        /// 获取指定类型的手部动画组件
        /// </summary>
        /// <param name="seat"></param>
        /// <param name="handStyle"></param>
        /// <param name="handDir"></param>
        /// <returns></returns>
        Animation GetHandAnimation(int seat, PlayerType handStyle, HandDirection handDir)
        {
            if (handStyle == PlayerType.FEMALE)
            {
                if (handDir == HandDirection.LeftHand)
                    return femaleLHandsAnimation[seat];
                else
                    return femaleRHandsAnimation[seat];
            }
            else
            {
                if (handDir == HandDirection.LeftHand)
                    return maleLHandsAnimation[seat];
                else
                    return maleRHandsAnimation[seat];
            }
        }

        /// <summary>
        /// 获取对应座位女性的打牌右手
        /// </summary>
        /// <param name="seat"></param>
        /// <returns></returns>
        GameObject GetFemaleRightHand(int seat)
        {
            return GetHand(seat, PlayerType.FEMALE, HandDirection.RightHand);
        }

        /// <summary>
        /// 获取对应座位女性的打牌左手
        /// </summary>
        /// <param name="seat"></param>
        /// <returns></returns>
        GameObject GetFemaleLeftHand(int seat)
        {
            return GetHand(seat, PlayerType.FEMALE, HandDirection.LeftHand);
        }

        /// <summary>
        /// 获取对应座位男性的打牌右手
        /// </summary>
        /// <param name="seat"></param>
        /// <returns></returns>
        GameObject GetMaleRightHand(int seat)
        {
            return GetHand(seat, PlayerType.MALE, HandDirection.RightHand);
        }

        /// <summary>
        /// 获取对应座位男性的打牌左手
        /// </summary>
        /// <param name="seat"></param>
        /// <returns></returns>
        GameObject GetMaleLeftHand(int seat)
        {
            return GetHand(seat, PlayerType.MALE, HandDirection.LeftHand);
        }

        #endregion

        #region 设置手在桌面上的阴影平面
        /// <summary>
        /// 设置手在桌面上的阴影平面
        /// </summary>
        void SetHandShadowShaders()
        {
            for (int n = 0; n < 4; n++)
            {
                Transform plane = mjAssetsMgr.handShadowPlanes[n].transform;
                Renderer planeRenderer = plane.GetComponent<Renderer>();

                planeSize = planeRenderer.bounds.size;
                handShadowPlaneInfos[n].planeMat = planeRenderer.material;

                for (int i = 0; i < 3; i++)
                {
                    handShadowPlaneInfos[n].tiling[i] = handShadowPlaneInfos[n].planeMat.GetTextureScale(shaderTexNames[i]);
                    handShadowPlaneInfos[n].offset[i] = handShadowPlaneInfos[n].planeMat.GetTextureOffset(shaderTexNames[i]);

                    //以平面的实际尺寸除以tiling数，获取当前阴影贴图被缩小的尺寸，
                    //tiling为1时，表示整个平面正好被（1*1 = 1）一张阴影贴图铺满
                    //tiling为2时，表示整个平面正好被（2*2 = 4）四张阴影贴图铺满
                    float uCellSize = planeSize.x / handShadowPlaneInfos[n].tiling[i].x;  //计算阴影贴图在平面上的实际宽度
                    float vCellSize = planeSize.z / handShadowPlaneInfos[n].tiling[i].y;  //计算阴影贴图在平面上的实际高度

                    //获取被缩放后的阴影贴图在平面中的初始位置
                    float xpos = plane.position.x + planeSize.x / 2 - uCellSize / 2 + handShadowPlaneInfos[n].offset[i].x / handShadowPlaneInfos[n].tiling[i].x * planeSize.x;
                    float zpos = plane.position.z + planeSize.z / 2 - vCellSize / 2 + handShadowPlaneInfos[n].offset[i].y / handShadowPlaneInfos[n].tiling[i].y * planeSize.z;
                    handShadowPlaneInfos[n].shadowOrgPos[i] = new Vector2(xpos, zpos);

                    //先偏移阴影出平面
                    handShadowPlaneInfos[n].planeMat.SetTextureOffset(shaderTexNames[i], new Vector2(-10, -10));
                }
            }
        }

        #endregion

        #region 设置手部动作播放速度
        void SetHandsActionsPlaySpeed()
        {
            for (int i = 0; i < 4; i++)
            {
                if (isUseHandActionSpeedSettingFile)
                {
                    SetHandsActionsPlaySpeedFromSetting(i, PlayerType.MALE, HandDirection.LeftHand);
                    SetHandsActionsPlaySpeedFromSetting(i, PlayerType.MALE, HandDirection.RightHand);
                    SetHandsActionsPlaySpeedFromSetting(i, PlayerType.FEMALE, HandDirection.LeftHand);
                    SetHandsActionsPlaySpeedFromSetting(i, PlayerType.FEMALE, HandDirection.RightHand);
                }
                else
                {
                    SetHandsActionsPlaySpeed(i, PlayerType.MALE, HandDirection.LeftHand);
                    SetHandsActionsPlaySpeed(i, PlayerType.MALE, HandDirection.RightHand);
                    SetHandsActionsPlaySpeed(i, PlayerType.FEMALE, HandDirection.LeftHand);
                    SetHandsActionsPlaySpeed(i, PlayerType.FEMALE, HandDirection.RightHand);
                }
            }
        }

        void SetHandsActionsPlaySpeedFromSetting(int seatIdx, PlayerType handStyle, HandDirection handDir)
        {
            Animation anim = GetHandAnimation(seatIdx, handStyle, handDir);
            Dictionary<string, ActionDataInfo> actionDataDict = GetHandActionDataDict(seatIdx, handStyle, handDir);
            AnimationClip clip;

            foreach (var item in actionDataDict)
            {
                clip = anim.GetClip(item.Key);

                if (clip != null)
                {
                    anim[item.Key].speed = item.Value.speed;
                }
            }
        }


        void SetHandsActionsPlaySpeed(int seatIdx, PlayerType handStyle, HandDirection handDir)
        {
            Animation anim = GetHandAnimation(seatIdx, handStyle, handDir);

            float scale = 1.6f;
            float scale2 = 1f;

            anim["DaPai1"].speed = 2f * scale;
            // anim["DaPai1_b"].speed = 1.5f;
            anim["DaPai1_1_a"].speed = 1f * scale;
            anim["DaPai1_1_b"].speed = 3.5f;
            anim["DaPai1EndTaiHand1"].speed = 0.4f * scale;
            anim["DaPai1EndTaiHand2"].speed = 0.4f * scale;
            anim["DaPai1EndZhengPai"].speed = 1f;
            anim["DaPai1EndZhengPaiEndTaiHand"].speed = 1f;
            anim["DaPai1EndMovPai1"].speed = 1f * scale;
            anim["DaPai1EndMovPai1EndTaiHand1"].speed = 1f * scale;
            anim["DaPai1EndMovPai1EndTaiHand2"].speed = 1f * scale;
            anim["DaPai1EndMovPai1EndZhengPai"].speed = 1f * scale;
            anim["DaPai1EndMovPai1EndZhengPaiEndTaiHand"].speed = 1f;

            anim["DaPai1EndMovPai2"].speed = 1.6f * scale;
            anim["DaPai1EndMovPai2EndTaiHand1"].speed = 1.3f * scale;
            anim["DaPai1EndMovPai2EndTaiHand2"].speed = 1.3f * scale;

            anim["DaPai2EndMovPai"].speed = 1f * scale;
            anim["DaPai2EndMovPaiEndTaiHand1"].speed = 1f * scale;
            anim["DaPai2EndMovPaiEndTaiHand2"].speed = 1f * scale;
            anim["DaPai3"].speed = 1f * scale;
            anim["DaPai3EndTaiHand"].speed = 1f * scale;

            anim["FirstTaiHand1EndHuPai"].speed = 1f * scale2;
            anim["FirstTaiHand1EndHuPaiEndTaiHand"].speed = 1f * scale2;

            anim["FirstTaiHand2EndDaPai4"].speed = 1.5f;
            anim["FirstTaiHand2EndDaPai4EndTaiHand"].speed = 1f * scale2;

            anim["FirstHand"].speed = 0.2f;
            anim["zhuaHandPai"].speed = 1f;
            anim["PutDownHandPai1"].speed = 1.6f * scale;
            anim["PutDownHandPai2"].speed = 1.6f * scale;
            anim["TaiHand"].speed = 2f * scale;

            float n = 0.3f / anim.GetClip("MovHandPai").length;
            anim["MovHandPai"].speed = 1f / n;

        }


        float GetHandActionWaitTime(int seatIdx, PlayerType handStyle, HandDirection handDir, string actionName)
        {
            Dictionary<string, ActionDataInfo> actionDataDict = GetHandActionDataDict(seatIdx, handStyle, handDir);
            Animation anim = GetHandAnimation(seatIdx, handStyle, handDir);
            float waitTime = anim.GetClip(actionName).length / anim[actionName].speed;

            if (actionDataDict == null)
                return waitTime;

            if (actionDataDict.ContainsKey(actionName))
                waitTime *= actionDataDict[actionName].crossFadeNormalTime;
            else
                return waitTime;

            return waitTime;
        }

        float GetAnimationActionWaitTime(Animation anim, string actionName, float crossFadeNormalTime)
        {
            float waitTime = anim.GetClip(actionName).length / anim[actionName].speed;
            waitTime *= crossFadeNormalTime;
            return waitTime;
        }

        #endregion

        #region 设置麻将在手中打牌的起始动作位置，角度

        /// <summary>
        /// 设置麻将在手中打牌的起始动作位置，角度
        /// </summary>
        void SetMjDaPaiFirstHandPosAndEulerAngles()
        {
            Vector3 pos = new Vector3(-0.005775623f, -0.0296248f, 0.01050866f);
            Vector3 angles = new Vector3(346.4407f, 86.18822f, 2.129383f);

            Vector3 pos2 = new Vector3(-0.007805936f, 0.002875186f, -0.008940288f);
            Vector3 angles2 = new Vector3(0.7325219f, 89.99999f, 90f);

            mjDaPaiFirstHandPos = new Vector3[]
            {
                pos,
                pos,
                pos,
                pos,
            };

            mjDaPaiFirstHandEulerAngles = new Vector3[]
            {
               angles,
               angles,
               angles,
               angles,
            };


            mjDaPaiFirstHandPos2 = new Vector3[]
            {
                pos2,
                pos2,
                pos2,
                pos2,
            };

            mjDaPaiFirstHandEulerAngles2 = new Vector3[]
           {
               angles2,
               angles2,
               angles2,
               angles2,
           };
        }

        #endregion

        #region 生成麻将桌目标牌到手部相对偏移值

        void CreateMjToHandOffsetVector()
        {
            CreateMjToFemaleRHandOffsetVector();
            CreateMjToFemaleLHandOffsetVector();
        }

        /// <summary>
        /// 生成麻将桌目标牌到女性右手部相对偏移值
        /// </summary>
        void CreateMjToFemaleRHandOffsetVector()
        {
            Vector3[] offsets_seat1 = new Vector3[]
           {
                //DaPai1
                new Vector3(0.3407079f, -0.01009113f, -0.02932134f),

                //DaPai1_MovPai1
                new Vector3(0.3693f, -0.009610415f, 0.002099998f),


                new Vector3(0, 0, 0),

                //DaPai2_MovPai
                new Vector3(0.387f, -0.009400003f, -0.024f),


                new Vector3(0.3122329f, -0.01485004f, -0.02237723f),
                new Vector3(0.4932073f, -0.009729922f, -0.02568223f),
                new Vector3(0.4416f, -0.0075f, -0.0591f),
                new Vector3(0.5303257f, -0.01017659f, -0.04001369f),

                //插牌动作ActionCombineNum.ChaPai
                new Vector3(0.3412f, -0.01966844f, -0.031f),

                new Vector3(0.3619999f, -0.0103f, -0.0767f),

                //ActionCombineNum.TuiDaoPai
                new Vector3(0.3306f, -0.02186844f, -0.03f),

                //ActionCombineNum.QiDongDiceMachine
               new Vector3( 0.712f, -0.006999999f, -0.065f),
           };

            Vector3[] offsets_seat0 = new Vector3[offsets_seat1.Length];
            Vector3[] offsets_seat2 = new Vector3[offsets_seat1.Length];
            Vector3[] offsets_seat3 = new Vector3[offsets_seat1.Length];

            for (int i = 0; i < offsets_seat1.Length; i++)
            {
                offsets_seat0[i] = new Vector3(-offsets_seat1[i].z, offsets_seat1[i].y, offsets_seat1[i].x);
                offsets_seat2[i] = new Vector3(offsets_seat1[i].z, offsets_seat1[i].y, -offsets_seat1[i].x);
                offsets_seat3[i] = new Vector3(-offsets_seat1[i].x, offsets_seat1[i].y, -offsets_seat1[i].z);
            }

            CreateMjToFemaleRHandOffsetVectorSeat(0, offsets_seat0);
            CreateMjToFemaleRHandOffsetVectorSeat(1, offsets_seat1);
            CreateMjToFemaleRHandOffsetVectorSeat(2, offsets_seat2);
            CreateMjToFemaleRHandOffsetVectorSeat(3, offsets_seat3);
        }

        void CreateMjToFemaleRHandOffsetVectorSeat(int seatIdx, Vector3[] offsets)
        {
            for (int i = 0; i < (int)ActionCombineNum.End; i++)
            {
                mjToFemaleRHandOffsetList[seatIdx].Add(new Vector3(0, 0, 0));
            }

            float yoffset = 0.0005f;
            float xsign = 0f, zsign = 0f;

            switch (seatIdx)
            {
                case 0: xsign = -1; zsign = 1f; break;
                case 1: xsign = 1f; zsign = 1f; break;
                case 2: xsign = 1f; zsign = -1f; break;
                case 3: xsign = -1f; zsign = -1f; break;
            }

            Vector3 offset0 = new Vector3(offsets[0].x, offsets[0].y + yoffset, offsets[0].z);
            mjToFemaleRHandOffsetList[seatIdx][(int)ActionCombineNum.DaPai1_TaiHand1] = offset0;
            mjToFemaleRHandOffsetList[seatIdx][(int)ActionCombineNum.DaPai1_TaiHand2] = offset0;

            Vector3 offset1 = new Vector3(offsets[0].x + 0.01f * xsign, offsets[0].y + yoffset, offsets[0].z + 0.01f * zsign);
            mjToFemaleRHandOffsetList[seatIdx][(int)ActionCombineNum.DaPai1_ZhengPai_TaiHand] = offset1;

            Vector3 offset2 = new Vector3(offsets[1].x + 0.01f * xsign, offsets[1].y + yoffset, offsets[1].z + 0.01f * zsign);
            mjToFemaleRHandOffsetList[seatIdx][(int)ActionCombineNum.DaPai1_MovPai1_TaiHand1] = offset2;
            mjToFemaleRHandOffsetList[seatIdx][(int)ActionCombineNum.DaPai1_MovPai1_TaiHand2] = offset2;

            Vector3 offset3 = new Vector3(offsets[1].x, offsets[1].y + yoffset, offsets[1].z);
            mjToFemaleRHandOffsetList[seatIdx][(int)ActionCombineNum.DaPai1_MovPai1_ZhengPai_TaiHand] = offset3;

            Vector3 offset5 = new Vector3(offsets[3].x, offsets[3].y + yoffset, offsets[3].z);
            mjToFemaleRHandOffsetList[seatIdx][(int)ActionCombineNum.DaPai2_MovPai_TaiHand1] = offset5;
            mjToFemaleRHandOffsetList[seatIdx][(int)ActionCombineNum.DaPai2_MovPai_TaiHand2] = offset5;

            Vector3 offset6 = new Vector3(offsets[4].x, offsets[4].y + yoffset, offsets[4].z);
            mjToFemaleRHandOffsetList[seatIdx][(int)ActionCombineNum.DaPai3_TaiHand] = offset6;

            Vector3 offset7 = new Vector3(offsets[5].x, offsets[5].y + yoffset, offsets[5].z);
            mjToFemaleRHandOffsetList[seatIdx][(int)ActionCombineNum.FirstTaiHand2_DaPai4_TaiHand] = offset7;

            Vector3 offset8 = new Vector3(offsets[6].x, offsets[6].y, offsets[6].z);
            mjToFemaleRHandOffsetList[seatIdx][(int)ActionCombineNum.HuPai] = offset8;

            Vector3 offset9 = new Vector3(offsets[7].x, offsets[7].y + yoffset, offsets[7].z);
            mjToFemaleRHandOffsetList[seatIdx][(int)ActionCombineNum.DaPai5] = offset9;

            //插牌手势距麻将位置偏移值
            mjToFemaleRHandOffsetList[seatIdx][(int)ActionCombineNum.ChaPai] = offsets[8];

            //碰牌手势距麻将位置偏移值
            mjToFemaleRHandOffsetList[seatIdx][(int)ActionCombineNum.PengPai] = offsets[9];

            //推倒牌手势距麻将位置偏移值
            mjToFemaleRHandOffsetList[seatIdx][(int)ActionCombineNum.TuiDaoPai] = offsets[10];

            //启动骰子器手势距启动位置偏移值
            mjToFemaleRHandOffsetList[seatIdx][(int)ActionCombineNum.QiDongDiceMachine] = offsets[11];
        }

        void CreateMjToFemaleLHandOffsetVector()
        {
            Vector3[] offsets_seat1 = new Vector3[]
            {
                new Vector3(0.3517f, -0.0205f, -0.0642f),

                //ActionCombineNum.TuiDaoPai
                new Vector3(0.3306f, -0.02186844f, 0.03f)
            };

            Vector3[] offsets_seat0 = new Vector3[offsets_seat1.Length];
            Vector3[] offsets_seat2 = new Vector3[offsets_seat1.Length];
            Vector3[] offsets_seat3 = new Vector3[offsets_seat1.Length];

            for (int i = 0; i < offsets_seat1.Length; i++)
            {
                offsets_seat0[i] = new Vector3(-offsets_seat1[i].z, offsets_seat1[i].y, offsets_seat1[i].x);
                offsets_seat2[i] = new Vector3(offsets_seat1[i].z, offsets_seat1[i].y, -offsets_seat1[i].x);
                offsets_seat3[i] = new Vector3(-offsets_seat1[i].x, offsets_seat1[i].y, -offsets_seat1[i].z);
            }


            CreateMjToFemaleLHandOffsetVectorSeat(0, offsets_seat0);
            CreateMjToFemaleLHandOffsetVectorSeat(1, offsets_seat1);
            CreateMjToFemaleLHandOffsetVectorSeat(2, offsets_seat2);
            CreateMjToFemaleLHandOffsetVectorSeat(3, offsets_seat3);
        }

        void CreateMjToFemaleLHandOffsetVectorSeat(int seatIdx, Vector3[] offsets)
        {
            for (int i = 0; i < (int)ActionCombineNum.End; i++)
            {
                mjToFemaleLHandOffsetList[seatIdx].Add(new Vector3(0, 0, 0));
            }

            //碰牌手势据麻将位置偏移值
            mjToFemaleLHandOffsetList[seatIdx][(int)ActionCombineNum.PengPai] = offsets[0];

            //推倒牌手势距麻将位置偏移值
            mjToFemaleLHandOffsetList[seatIdx][(int)ActionCombineNum.TuiDaoPai] = offsets[1];
        }


        /// <summary>
        /// 获取麻将桌目标牌到手部相对偏移值列表
        /// </summary>
        /// <param name="seat"></param>
        /// <param name="handStyle"></param>
        /// <param name="handDir"></param>
        /// <returns></returns>
        List<Vector3> GetDeskMjHandOffsetList(int seat, PlayerType handStyle, HandDirection handDir)
        {
            if (handStyle == PlayerType.FEMALE)
            {
                if (handDir == HandDirection.LeftHand)
                {
                    return mjToFemaleLHandOffsetList[seat];
                }
                else
                {
                    return mjToFemaleRHandOffsetList[seat];
                }
            }
            else
            {
                if (handDir == HandDirection.LeftHand)
                {
                    return mjToMaleLHandOffsetList[seat];
                }
                else
                {
                    return mjToMaleRHandOffsetList[seat];
                }
            }
        }

        #endregion

    }
}