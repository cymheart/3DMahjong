using System;
using System.Collections.Generic;
using UnityEngine;
using Task;
using System.Reflection;

namespace CoreDesgin
{
    /// <summary>
    /// 麻将机命令
    /// </summary>
    public class MahjongMachineCmd : TaskDataBase
    {
        public delegate LinkedListNode<MahjongMachineCmd>[] AppendProcessDelegate();

        public string name;
        public MahjongMachine mjMachine = null;
        public MahjongMachineCmdMgr cmdList = null;

        public MahjongMachineCmdType type = MahjongMachineCmdType.Common;

        /// <summary>
        /// 自身相对父节点命令延迟执行时间
        /// </summary>
        public float delayExecuteTime = 0;

        /// <summary>
        /// 是否空闲可以再次利用
        /// </summary>
        public bool isFree = true;

        /// <summary>
        /// 是否是阻塞命令
        /// 当前命令的阻塞开启:意味着所有在这个命令之后添加的命令必须等待这条阻塞命令执行完成才能执行
        /// </summary>
        public bool isBlock = false;

        /// <summary>
        /// 是否在命令执行结束之后可以点击选择查看手牌
        /// </summary>
        public bool canSelectPaiAfterCmdEnd = false;


        /// <summary>
        /// 命令列表添加此命令时的调用添加处理
        /// </summary>
        public AppendProcessDelegate AppendProcessFunc = null;


        public LinkedList<MahjongMachineCmd> delayOpCmdLinkedList = new LinkedList<MahjongMachineCmd>();

        public void Append(LinkedListNode<MahjongMachineCmd> cmdNode)
        {
            delayOpCmdLinkedList.AddLast(cmdNode);
        }

        public void ClearChildCmdList()
        {
            if (delayOpCmdLinkedList != null && delayOpCmdLinkedList.Count > 0)
                delayOpCmdLinkedList.Clear();
        }

        /// <summary>
        /// 安装命令
        /// </summary>
        /// <param name="processCmdClassFunc"></param>
        public static void Install<T>(CmdData.ProcessCmdClassDelegate processCmdClassFunc) where T : MahjongMachineCmd
        {
            MjMachineCmdPool.Instance.AppendCmdClass<T>(processCmdClassFunc);
        }

        /// <summary>
        /// 是否可以执行
        /// </summary>
        public virtual bool CanExecute()
        {
            return true;
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="opCmdNode"></param>
        public virtual void Execute(LinkedListNode<MahjongMachineCmd> opCmdNode = null)
        {

        }
    }

    /// <summary>
    /// 麻将机手部动作命令
    /// </summary>
    public class MahjongMachineHandActionCmd : MahjongMachineCmd
    {
        public int seatIdx = 0;
        public MahjongMachineHandActionCmd()
        {
            type = MahjongMachineCmdType.HandAction;
        }
    }


    /// <summary>
    /// 麻将机请求命令
    /// </summary>
    public class MahjongMachineRequestCmd : MahjongMachineCmd
    {
        public int seatIdx = 0;
        public MahjongMachineRequestCmd()
        {
            type = MahjongMachineCmdType.Request;
        }
    }

}