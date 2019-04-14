using System.Collections.Generic;
using UnityEngine;

namespace CoreDesgin
{
    /// <summary>
    /// 麻将机命令管理
    /// </summary>
    public class MahjongMachineCmdMgr
    {
        MahjongMachine mjMachine = null;
        LinkedList<MahjongMachineCmd> mjCmdMgr = new LinkedList<MahjongMachineCmd>();
        LinkedList<MahjongMachineCmd> delayOpCmdList = new LinkedList<MahjongMachineCmd>();

        public LinkedList<MahjongMachineCmd>[] playerActionOpCmdLists = new LinkedList<MahjongMachineCmd>[4]
        {
        new LinkedList<MahjongMachineCmd>(),
        new LinkedList<MahjongMachineCmd>(),
        new LinkedList<MahjongMachineCmd>(),
        new LinkedList<MahjongMachineCmd>()
        };

        LinkedListNode<MahjongMachineCmd> cantExecuteCmdNode = null;

        /// <summary>
        /// 阻塞的命令数量
        /// </summary>
        int blockCmdCount = 0;

        public void Init(MahjongMachine mjMachine)
        {
            this.mjMachine = mjMachine;
        }

        public void ReleaseCmd(MahjongMachineCmd cmd)
        {
            MjMachineCmdPool.Instance.ReleaseCmd(cmd);
        }

        public LinkedListNode<MahjongMachineCmd> CreateCmdNode(MahjongMachineCmd mjOpCmd)
        {
            return MjMachineCmdPool.Instance.CreateCmdNode(mjOpCmd);
        }

        public void ReleaseCmdNode(LinkedListNode<MahjongMachineCmd> cmdNode)
        {
            MjMachineCmdPool.Instance.ReleaseCmdNode(cmdNode);
        }


        public LinkedList<MahjongMachineCmd> CreateCmdLinkedList()
        {
            return MjMachineCmdPool.Instance.CreateCmdLinkedList();
        }

        public void ReleaseCmdLinkedList(LinkedList<MahjongMachineCmd> list)
        {
            MjMachineCmdPool.Instance.ReleaseCmdLikedList(list);
        }

        public void Append(MahjongMachineCmd mjOpCmd)
        {
            if (mjOpCmd.AppendProcessFunc == null)
            {
                var node = CreateCmdNode(mjOpCmd);
                mjCmdMgr.AddLast(node);
            }
            else
            {
                LinkedListNode<MahjongMachineCmd>[] cmdNodes = mjOpCmd.AppendProcessFunc();
                for(int i=0; i< cmdNodes.Length; i++)
                    mjCmdMgr.AddLast(cmdNodes[i]);
            }
        }

        public void Reset()
        {
            cantExecuteCmdNode = null;
            blockCmdCount = 0;

            mjCmdMgr.Clear();
            delayOpCmdList.Clear();

            for (int i = 0; i < playerActionOpCmdLists.Length; i++)
                playerActionOpCmdLists[i].Clear();
        }

        public LinkedList<MahjongMachineCmd> GetPlayerActionOpCmdList(int seatIdx)
        {
            return playerActionOpCmdLists[seatIdx];
        }

        public int GetPlayerActionOpCmdCount(int seatIdx)
        {
            return playerActionOpCmdLists[seatIdx].Count;
        }

        public void ExecuteNextHandActionCmd(int seatIdx)
        {
            CmdOp(playerActionOpCmdLists[seatIdx].First);
        }

        /// <summary>
        /// 如果动作已经可以确认结束或者不再需要保留这个动作，命令列表将释放这个操作命令所占用的内存
        /// 同时如果这个命令是阻塞命令，将会解除此命令的阻塞
        /// </summary>
        /// <param name="opCmdNode"></param>
        public void RemoveCmd(LinkedListNode<MahjongMachineCmd> opCmdNode)
        {
            if (opCmdNode == null)
                return;

            switch(opCmdNode.Value.type)
            {
                case MahjongMachineCmdType.HandAction:
                case MahjongMachineCmdType.Request:
                    {
                        RemoveHandActionOpCmd(((MahjongMachineHandActionCmd)(opCmdNode.Value)).seatIdx, opCmdNode);
                    }
                    break;

                case MahjongMachineCmdType.Common:
                    RemoveCommonActionOpCmd(opCmdNode);
                    break;
            }
        }

        void RemoveHandActionOpCmd(int seatIdx, LinkedListNode<MahjongMachineCmd> opCmdNode)
        {
            if (opCmdNode != null)
            {
                if (opCmdNode.Value.isBlock)
                    blockCmdCount--;

                playerActionOpCmdLists[seatIdx].Remove(opCmdNode);
                ReleaseCmd(opCmdNode.Value);
                ReleaseCmdNode(opCmdNode);
            }
        }

        void RemoveCommonActionOpCmd(LinkedListNode<MahjongMachineCmd> opCmdNode)
        {
            if (opCmdNode != null)
            {
                if (opCmdNode.Value.isBlock)
                    blockCmdCount--;

                ReleaseCmd(opCmdNode.Value);
                ReleaseCmdNode(opCmdNode);
            }
        }

        public void CmdOp(LinkedListNode<MahjongMachineCmd> opCmdNode, MahjongMachineCmd mjOpCmd = null)
        {
            if (opCmdNode == null && mjOpCmd == null)
                return;

            MahjongMachineCmd opCmd = mjOpCmd;

            if (opCmdNode != null)
            {
                opCmd = opCmdNode.Value;

                if (opCmd.isBlock)
                    blockCmdCount++;
            }

            if (opCmd.delayOpCmdLinkedList != null && opCmd.delayOpCmdLinkedList.Count > 0)
            {
                for (LinkedListNode<MahjongMachineCmd> node = opCmd.delayOpCmdLinkedList.First; node != null; node = opCmd.delayOpCmdLinkedList.First)
                {
                    opCmd.delayOpCmdLinkedList.RemoveFirst();
                    AppendCmdToDelayCmdList(node);
                }

                opCmd.delayOpCmdLinkedList.Clear();
            }

            opCmd.Execute(opCmdNode);
        }


        public void AppendCmdToDelayCmdList(MahjongMachineCmd cmd)
        {
            var cmdNode = CreateCmdNode(cmd);
            cmd.delayExecuteTime += Time.time;
            delayOpCmdList.AddLast(cmdNode);
        }

        public void AppendCmdToDelayCmdList(LinkedListNode<MahjongMachineCmd> cmdNode)
        {
            cmdNode.Value.delayExecuteTime += Time.time;
            delayOpCmdList.AddLast(cmdNode);
        }

        public void Update()
        {
            if (blockCmdCount > 0)
                return;

            if (cantExecuteCmdNode != null)
                ProcessCmdNode(cantExecuteCmdNode);

            bool isBlock = false;
            for (LinkedListNode<MahjongMachineCmd> opCmdNode = mjCmdMgr.First; opCmdNode != null; opCmdNode = mjCmdMgr.First)
            {
                mjCmdMgr.RemoveFirst();

                if (opCmdNode.Value.delayExecuteTime > 0)
                {
                    AppendCmdToDelayCmdList(opCmdNode);
                    continue;
                }

                isBlock = opCmdNode.Value.isBlock;

                if (ProcessCmdNode(opCmdNode) == false)
                    return;

                if (isBlock == true)
                    break;
            }

            UpdateDelayExecuteOpCmd();
        }


        /// <summary>
        /// 更新延迟执行操作命令
        /// </summary>
        public void UpdateDelayExecuteOpCmd()
        {
            MahjongMachineCmd delayOpCmd;
            LinkedListNode<MahjongMachineCmd> nextNode;

            for (LinkedListNode<MahjongMachineCmd> opCmdNode = delayOpCmdList.First; opCmdNode != null;)
            {
                delayOpCmd = opCmdNode.Value;

                if (Time.time >= delayOpCmd.delayExecuteTime)
                {
                    nextNode = opCmdNode.Next;
                    delayOpCmdList.Remove(opCmdNode);
                    ProcessCmdNode(opCmdNode);
                    opCmdNode = nextNode;
                }
                else
                {
                    opCmdNode = opCmdNode.Next;
                }
            }
        }

        bool ProcessCmdNode(LinkedListNode<MahjongMachineCmd> opCmdNode)
        {
            MahjongMachineCmd curtCmd = opCmdNode.Value;
            cantExecuteCmdNode = opCmdNode;

            switch (curtCmd.type)
            {
                case MahjongMachineCmdType.HandAction:
                    {
                        if (curtCmd.CanExecute())
                        {
                            MahjongMachineHandActionCmd cmd = (MahjongMachineHandActionCmd)curtCmd;
                            playerActionOpCmdLists[cmd.seatIdx].AddLast(opCmdNode);
                            cantExecuteCmdNode = null;

                            if (playerActionOpCmdLists[cmd.seatIdx].Count == 1)
                            {
                                CmdOp(playerActionOpCmdLists[cmd.seatIdx].First);
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                    break;

                case MahjongMachineCmdType.Common:
                    {
                        if (curtCmd.CanExecute())
                        {
                            CmdOp(opCmdNode);
                            cantExecuteCmdNode = null;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    break;

                case MahjongMachineCmdType.Request:
                    {
                        if (curtCmd.CanExecute())
                        {
                            playerActionOpCmdLists[0].AddLast(opCmdNode);
                            cantExecuteCmdNode = null;

                            if (playerActionOpCmdLists[0].Count == 1)
                            {
                                CmdOp(playerActionOpCmdLists[0].First);
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                    break;
            }

            return true;
        }
    }
}