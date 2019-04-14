using ComponentDesgin;
using CoreDesgin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

namespace ActionDesgin
{
    /// <summary>
    /// 麻将机手部动作
    /// </summary>
    public class BaseHandAction : MahjongMachineAction
    {

        /// <summary>
        /// 麻将资源管理模块
        /// </summary>
        public MahjongAssetsMgr mjAssetsMgr;


        /// <summary>
        /// 手部动作模块
        /// </summary>
        public Hand hands;

        /// <summary>
        /// 声音处理模块
        /// </summary>
        public Audio audio;

        /// <summary>
        /// 麻将桌模块
        /// </summary>
        public Desk desk;

        /// <summary>
        /// 尺寸角度阴影等适配模块
        /// </summary>
        public Fit fit;


        /// <summary>
        /// 预设值帮助模块
        /// </summary>
        public PreSettingHelper preSettingHelper;


        /// <summary>
        /// 场景模块
        /// </summary>
        public Scene scene;

        /// <summary>
        /// 胡听牌检查
        /// </summary>
        public MjHuTingCheck mjHuTingCheck;

        public BaseHandAction()      
            : base()
        {
        }

        public override void Init(MahjongMachine mjMachine)
        {
            base.Init(mjMachine);

            hands = mjMachine.GetComponent<Hand>();
            audio = mjMachine.GetComponent<Audio>();
            desk = mjMachine.GetComponent<Desk>();
            fit = mjMachine.GetComponent<Fit>();
            scene = mjMachine.GetComponent<Scene>();
            mjHuTingCheck = mjMachine.GetComponent<MjHuTingCheck>();
        }

        public void StopSelectPaiActionState(int seatIdx)
        {
            if (playerStateData[seatIdx].state >= SelectPaiStateData.SELECT_PAI_START &&
                playerStateData[seatIdx].state <= SelectPaiStateData.SELECT_PAI_END)
            {
                playerStateData[seatIdx].state = StateDataGroup.END;
            }
        }


        #region 动作共用功能

        public void MoveHandShadowForDaPai(int seatIdx, ActionStateData stateData)
        {
            Transform handShadowRef = stateData.handShadowAxis[0];
            float ang = 360 - handShadowRef.eulerAngles.y;

            float matUOffset =
                ((handShadowRef.position.x - hands.handShadowPlaneInfos[seatIdx].shadowOrgPos[0].x) / hands.planeSize.x) *
                hands.handShadowPlaneInfos[seatIdx].tiling[0].x + hands.handShadowPlaneInfos[seatIdx].offset[0].x;

            float matVOffset =
                ((handShadowRef.position.z - hands.handShadowPlaneInfos[seatIdx].shadowOrgPos[0].y) / hands.planeSize.z) *
                hands.handShadowPlaneInfos[seatIdx].tiling[0].y + hands.handShadowPlaneInfos[seatIdx].offset[0].y;

            hands.handShadowPlaneInfos[seatIdx].planeMat.SetTextureOffset(Hand.shaderTexNames[0], new Vector2(matUOffset, matVOffset));
            hands.handShadowPlaneInfos[seatIdx].planeMat.SetFloat(Hand.shaderAngNames[0], ang);
        }

        public void MoveHandShadowForChaPai(int seatIdx, ActionStateData stateData)
        {
            Transform handShadowRef = stateData.handShadowAxis[0];
            float ang = 360 - handShadowRef.eulerAngles.y;

            float matUOffset = ((handShadowRef.position.x - hands.handShadowPlaneInfos[seatIdx].shadowOrgPos[2].x) / hands.planeSize.x) *
                hands.handShadowPlaneInfos[seatIdx].tiling[2].x + hands.handShadowPlaneInfos[seatIdx].offset[2].x;

            float matVOffset = ((handShadowRef.position.z - hands.handShadowPlaneInfos[seatIdx].shadowOrgPos[2].y) / hands.planeSize.z) *
                hands.handShadowPlaneInfos[seatIdx].tiling[2].y + hands.handShadowPlaneInfos[seatIdx].offset[2].y;

            hands.handShadowPlaneInfos[seatIdx].planeMat.SetTextureOffset(Hand.shaderTexNames[2], new Vector2(matUOffset, matVOffset));
            hands.handShadowPlaneInfos[seatIdx].planeMat.SetFloat(Hand.shaderAngNames[2], ang);

        }


        public void ProcessHandActionmjCmdMgr(int seatIdx, StateData stateData)
        {
            if (mjCmdMgr != null && stateData.opCmdNode != null)
            {
                if (stateData.opCmdNode.Value.canSelectPaiAfterCmdEnd == true)
                    playerStateData[seatIdx].state = SelectPaiStateData.SELECT_PAI_START;

                LinkedListNode<MahjongMachineCmd> tmp = stateData.opCmdNode;
                stateData.opCmdNode = null;
                mjCmdMgr.RemoveCmd(tmp);
            }
        }


        public void ProcessCommonActionmjCmdMgr(StateData stateData)
        {
            if (mjCmdMgr != null && stateData.opCmdNode != null)
            {
                LinkedListNode<MahjongMachineCmd> tmp = stateData.opCmdNode;
                stateData.opCmdNode = null;
                mjCmdMgr.RemoveCmd(tmp);
            }
        }


        /// <summary>
        /// 准备好动作的初始手势
        /// </summary>
        /// <param name="seatIdx">当前动作玩家座位号</param>
        /// <param name="actionName">初始手势名称</param>
        /// <returns></returns>
        public float ReadyFirstHand(int seatIdx, PlayerType handStyle, HandDirection handDir, string actionName)
        {
            float waitTime = 0.3f;
            GameObject hand = hands.GetHand(seatIdx, handStyle, handDir);
            Animation anim = hands.GetHandAnimation(seatIdx, handStyle, handDir);

            hand.transform.position = new Vector3(1.215f, 0, 0);
            hand.transform.eulerAngles = new Vector3(0, 0, 0);

            anim.Play(actionName);
            waitTime = hands.GetHandActionWaitTime(seatIdx, handStyle, handDir, actionName);

            return waitTime;
        }

        /// <summary>
        /// 适配手部姿态到当前座位
        /// </summary>
        /// <param name="seatIdx"></param>
        public void FitHandPoseForSeat(int seatIdx, PlayerType handStyle, HandDirection handDirection, ActionCombineNum actionCombineNum)
        {
            GameObject hand = hands.GetHand(seatIdx, handStyle, handDirection);
            Vector3 pos = new Vector3(1.215f, 0, 0);

            switch (actionCombineNum)
            {
                case ActionCombineNum.ChaPai:
                    pos = new Vector3(1.215f, 0, 0.334f);
                    break;
            }

            switch (seatIdx)
            {
                case 0:
                    hand.transform.localRotation = Quaternion.Euler(new Vector3(0, 270, 0));
                    hand.transform.position = new Vector3(-pos.z, pos.y, pos.x);
                    break;

                case 1:
                    hand.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
                    hand.transform.position = pos;
                    break;

                case 2:
                    hand.transform.localRotation = Quaternion.Euler(new Vector3(0, 90, 0));
                    hand.transform.position = new Vector3(pos.z, pos.y, -pos.x);
                    break;

                case 3:
                    hand.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
                    hand.transform.position = new Vector3(-pos.x, pos.y, -pos.z);
                    break;
            }
        }

        /// <summary>
        /// 移动手部到目标偏移位置，为下一步精确动作做好准备
        /// </summary>
        /// <param name="seatIdx">当前动作玩家座位号</param>
        /// <param name="pos">目标位置</param>
        /// <param name="actionCombineNum">打牌动作</param>
        public float MoveHandToDstOffsetPos(int seatIdx, PlayerType handStyle, HandDirection handDir, Vector3 dstPos, ActionCombineNum actionCombineNum)
        {
            //
            float waitTime = 0.3f;
            List<Vector3> handOffsetList = hands.GetDeskMjHandOffsetList(seatIdx, handStyle, handDir);
            GameObject hand = hands.GetHand(seatIdx, handStyle, handDir);
            Animation anim = hands.GetHandAnimation(seatIdx, handStyle, handDir);
            Vector3 endValue = dstPos + handOffsetList[(int)actionCombineNum];

            switch (actionCombineNum)
            {
                case ActionCombineNum.FirstTaiHand2_DaPai4_TaiHand:
                    {
                        anim.Play("FirstTaiHand2");
                        waitTime = hands.GetHandActionWaitTime(seatIdx, handStyle, handDir, "FirstTaiHand2");
                    }
                    break;

                case ActionCombineNum.HuPai:
                    {
                        anim.Play("FirstTaiHand1");
                        waitTime = hands.GetHandActionWaitTime(seatIdx, handStyle, handDir, "FirstTaiHand1");
                    }
                    break;

                case ActionCombineNum.ChaPai:
                    {
                        anim.Play("FirstHand");

                        Dictionary<string, ActionDataInfo> actionDataDict = hands.GetHandActionDataDict(seatIdx, handStyle, handDir);
                        if (actionDataDict != null)
                        {
                            ActionDataInfo info = actionDataDict["MoveHandToDstPos"];
                            waitTime = 1f / info.speed * 0.3f * info.crossFadeNormalTime;
                        }
                    }
                    break;

                case ActionCombineNum.DaPai5:
                    {
                        anim.Play("FirstTaiHand3");
                        waitTime = hands.GetHandActionWaitTime(seatIdx, handStyle, handDir, "FirstTaiHand3");
                    }
                    break;

                default:
                    {
                        Dictionary<string, ActionDataInfo> actionDataDict = hands.GetHandActionDataDict(seatIdx, handStyle, handDir);
                        if (actionDataDict != null)
                        {
                            ActionDataInfo info = actionDataDict["MoveHandToDstPos"];
                            waitTime = 1f / info.speed * 0.3f * info.crossFadeNormalTime;
                        }
                    }

                    break;
            }

            hand.transform.DOMove(endValue, waitTime).SetEase(Ease.OutSine);
            return waitTime;
        }


        /// <summary>
        /// 手部动作结束移出手到屏幕外
        /// </summary>
        /// <param name="seatIdx"></param>
        /// <returns></returns>
        public float HandActionEndMovHandOutScreen(int seatIdx, PlayerType handStyle, HandDirection handDir, ActionCombineNum actionCombinNum = ActionCombineNum.End, int type = 0)
        {
            GameObject hand = hands.GetHand(seatIdx, handStyle, handDir);
            Dictionary<string, ActionDataInfo> actionDataDict = hands.GetHandActionDataDict(seatIdx, handStyle, handDir);
            float waitTime = 0.6f;
            float speed = 1f;

            if (actionDataDict != null)
            {
                ActionDataInfo info = actionDataDict["MoveHandOutScreen"];
                waitTime = 1f / info.speed * 0.6f * info.crossFadeNormalTime;
                speed = info.speed;

            }


            if (type == 0)
            {
                Vector3[] dstpos;

                switch (actionCombinNum)
                {
                    case ActionCombineNum.DaPai5:
                        dstpos = hands.handActionLeaveScreenPosSeat2;
                        break;

                    default:
                        dstpos = hands.handActionLeaveScreenPosSeat;
                        break;
                }

                hand.transform.DOMove(dstpos[seatIdx], 1f / speed * 0.6f); //.SetEase(Ease.InQuad);
            }
            else
            {
                switch (seatIdx)
                {
                    case 1:
                        hand.transform.DOLocalMoveX(1f, 1f / speed * 0.6f).SetRelative();
                        break;
                    case 3:
                        hand.transform.DOLocalMoveX(-1f, 1f / speed * 0.6f).SetRelative();
                        break;
                    case 2:
                        hand.transform.DOLocalMoveZ(-1f, 1f / speed * 0.6f).SetRelative();
                        break;
                    case 0:
                        hand.transform.DOLocalMoveZ(1f, 1f / speed * 0.6f).SetRelative();
                        break;
                }

            }

            return waitTime;
        }

        #endregion
    }
}
