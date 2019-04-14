using UnityEngine;

namespace CoreDesgin
{

/*
    public struct Vector3Int
    {
        public int x;
        public int y;
        public int z;

        public Vector3Int(int _x, int _y, int _z)
        {
            x = _x; y = _y; z = _z;
        }
    }
    */

    /// <summary>
    /// 风位
    /// </summary>
    public enum FengWei
    {
        /// <summary>
        /// 东
        /// </summary>
        EAST,

        /// <summary>
        /// 南
        /// </summary>
        SOUTH,

        /// <summary>
        /// 西
        /// </summary>
        WEST,

        /// <summary>
        /// 北
        /// </summary>
        NORTH
    }

    /// <summary>
    /// 麻将机命令类型
    /// </summary>
    public enum MahjongMachineCmdType
    {
        /// <summary>
        /// 普通类型
        /// </summary>
        Common,

        /// <summary>
        /// 手部动作类型
        /// </summary>
        HandAction,

        /// <summary>
        /// 请求类型
        /// </summary>
        Request
    }
}