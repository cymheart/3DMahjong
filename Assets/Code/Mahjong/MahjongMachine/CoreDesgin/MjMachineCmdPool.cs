using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreDesgin
{
    public class CmdData
    {
        public delegate void ProcessCmdClassDelegate(MahjongMachineCmd cmd);

        public List<MahjongMachineCmd> cmds = new List<MahjongMachineCmd>();
        public ProcessCmdClassDelegate processCmdClassFunc;
        public int idx = 0;
    }

    public class MjMachineCmdPool
    {
        MahjongMachine mjMachine;
        Dictionary<Type, CmdData> cmdDataDict = new Dictionary<Type, CmdData>();

        //
        List<LinkedListNode<MahjongMachineCmd>> cmdNodeList = new List<LinkedListNode<MahjongMachineCmd>>();
        int cmdNodeListIdx;
        List<bool> isCmdNodeListFree = new List<bool>();

        //
        List<LinkedList<MahjongMachineCmd>> cmdLinkedListList = new List<LinkedList<MahjongMachineCmd>>();
        int cmdLinkedListListIdx;
        List<bool> isCmdLinkedListListFree = new List<bool>();

        private MjMachineCmdPool() { }
        public static MjMachineCmdPool Instance { get; } = new MjMachineCmdPool();

        /// <summary>
        /// 添加麻将机命令类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="processCmdClassFunc"></param>
        public void AppendCmdClass<T>(CmdData.ProcessCmdClassDelegate processCmdClassFunc) where T : MahjongMachineCmd
        {
            Type cmdType = typeof(T);
            List<MahjongMachineCmd> cmds = new List<MahjongMachineCmd>();
            CmdData cmdData;

            bool ret = cmdDataDict.TryGetValue(cmdType, out cmdData);
            if (ret == false)
            {
                cmdData = new CmdData();
                cmdDataDict[cmdType] = cmdData;
            }

            MahjongMachineCmd cmd;

            for (int i = 0; i < 3; i++)
            {
                cmd = NewCmd<T>();
                cmd.mjMachine = mjMachine;
                cmd.cmdList = mjMachine.mjCmdMgr;
                cmdData.cmds.Add(cmd);
            }

            cmdData.processCmdClassFunc = processCmdClassFunc;
        }

        public void CreatePool(MahjongMachine mjMachine)
        {
            this.mjMachine = mjMachine;

            cmdNodeListIdx = 0;
            LinkedListNode<MahjongMachineCmd> cmdNode;
            for (int i = 0; i < 50; i++)
            {
                cmdNode = new LinkedListNode<MahjongMachineCmd>(null);
                cmdNodeList.Add(cmdNode);
                isCmdNodeListFree.Add(true);
            }

            //
            cmdNodeListIdx = 0;
            LinkedList<MahjongMachineCmd> linkedList;
            for (int i = 0; i < 10; i++)
            {
                linkedList = new LinkedList<MahjongMachineCmd>();
                cmdLinkedListList.Add(linkedList);
                isCmdNodeListFree.Add(true);
            }
        }

        public void DestroyPool()
        {
            cmdDataDict.Clear();
            cmdNodeList.Clear();
            cmdLinkedListList.Clear();
        }


        public T CreateCmd<T>() where T : MahjongMachineCmd
        {
            CmdData cmdData;
            Type cmdType = typeof(T);

            bool ret = cmdDataDict.TryGetValue(cmdType, out cmdData);
            if (ret == false)
                return null;

            List<MahjongMachineCmd> cmds = cmdData.cmds;

            for (int i = 0; i < cmds.Count; ++i)
            {
                int temI = (cmdData.idx + i) % cmds.Count;
                if (cmds[temI].isFree)
                {
                    cmdData.processCmdClassFunc(cmds[temI]);
                    cmdData.idx = (temI + 1) % cmds.Count;
                    cmds[temI].isFree = false;
                    return (T)cmds[temI];
                }
            }

            T cmd = NewCmd<T>();
            cmd.mjMachine = mjMachine;
            cmd.cmdList = mjMachine.mjCmdMgr;
            cmdData.processCmdClassFunc(cmd);
            cmd.isFree = false;
            cmds.Add(cmd);
            return cmd;
        }

        T NewCmd<T>()
        {
            object obj = Activator.CreateInstance(typeof(T), true);
            return (T)obj;
        }

        public void ReleaseCmd(MahjongMachineCmd cmd)
        {
            if (cmd == null)
                return;

            cmd.isFree = true;
            cmd.ClearChildCmdList();
        }

        public LinkedListNode<MahjongMachineCmd> CreateCmdNode(MahjongMachineCmd cmd)
        {
            for (int i = 0; i < cmdNodeList.Count; ++i)
            {
                int temI = (cmdNodeListIdx + i) % cmdNodeList.Count;
                if (isCmdNodeListFree[temI])
                {
                    cmdNodeListIdx = (temI + 1) % cmdNodeList.Count;
                    isCmdNodeListFree[temI] = false;
                    cmdNodeList[temI].Value = cmd;
                    return cmdNodeList[temI];
                }
            }

            LinkedListNode<MahjongMachineCmd> node = new LinkedListNode<MahjongMachineCmd>(cmd);
            cmdNodeList.Add(node);
            isCmdNodeListFree.Add(false);
            return node;
        }

        public void ReleaseCmdNode(LinkedListNode<MahjongMachineCmd> node)
        {
            if (node == null)
                return;

            node.Value = null;
            int idx = cmdNodeList.IndexOf(node);
            isCmdNodeListFree[idx] = true;
        }

        public LinkedList<MahjongMachineCmd> CreateCmdLinkedList()
        {
            for (int i = 0; i < cmdLinkedListList.Count; ++i)
            {
                int temI = (cmdLinkedListListIdx + i) % cmdLinkedListList.Count;
                if (isCmdLinkedListListFree[temI])
                {
                    cmdLinkedListListIdx = (temI + 1) % cmdLinkedListList.Count;
                    isCmdLinkedListListFree[temI] = false;
                    return cmdLinkedListList[temI];
                }
            }

            LinkedList<MahjongMachineCmd> list = new LinkedList<MahjongMachineCmd>();
            cmdLinkedListList.Add(list);
            isCmdLinkedListListFree.Add(false);
            return list;
        }

        public void ReleaseCmdLikedList(LinkedList<MahjongMachineCmd> list)
        {
            int idx = cmdLinkedListList.IndexOf(list);
            isCmdLinkedListListFree[idx] = true;
        }

    }
}
