using UnityEngine;

namespace ComponentDesgin
{
    /// <summary>
    /// 动作组合编号
    /// </summary>
    public enum ActionCombineNum
    {
        DaPai1_TaiHand1,
        DaPai1_TaiHand2,
        DaPai1_MovPai1_TaiHand1,
        DaPai1_MovPai1_TaiHand2,
        DaPai1_ZhengPai_TaiHand,
        DaPai1_MovPai1_ZhengPai_TaiHand,
        DaPai2_MovPai_TaiHand1,
        DaPai2_MovPai_TaiHand2,
        DaPai3_TaiHand,
        FirstTaiHand2_DaPai4_TaiHand,
        DaPai5,
        ChaPai,
        PengPai,
        ChiPai,
        GangPai,
        HuPai,
        TuiDaoPai,
        QiDongDiceMachine,
        End
    }


    /// <summary>
    /// 玩家类型
    /// </summary>
    public enum PlayerType
    {
        /// <summary>
        /// 男人
        /// </summary>
        MALE,

        /// <summary>
        /// 女人
        /// </summary>
        FEMALE,

        /// <summary>      
        /// 无类型
        /// </summary>
        NONE,
    }

    /// <summary>
    /// 用手方向
    /// </summary>
    public enum HandDirection
    {
        /// <summary>
        /// 左手
        /// </summary>
        LeftHand,

        /// <summary>
        /// 右手
        /// </summary>
        RightHand
    }

    public struct ActionDataInfo
    {
        public float speed;
        public float crossFadeNormalTime;
    }

    public class HandShadowPlaneInfo
    {
        public Material planeMat;
        public Vector2[] tiling = new Vector2[3];
        public Vector2[] offset = new Vector2[3];
        public Vector2[] shadowOrgPos = new Vector2[3];
    }
}