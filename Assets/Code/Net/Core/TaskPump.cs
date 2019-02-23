using System;
using System.Collections.Generic;
using System.Threading;

namespace Net
{
    public delegate void TaskCallBack(object data);

    public enum TaskMsg
    {
        TMSG_DATA,
        TMSG_QUIT,
        TMSG_TIMER_START,
        TMSG_TIMER_RUN,
        TMSG_TIMER_STOP,
        TMSG_PAUSE
    }


    public class TaskNode
    {
        public DateTime startTime;
        public int delay;                //延迟执行时间
        public TaskCallBack processDataCallBack;  //处理数据回调函数
        public TaskCallBack releaseDataCallBack;  //释放数据回调函数
        public object data;                 //任务数据
        public TaskMsg msg;                  //任务标志

        public TaskNode(TaskMsg taskMsg)
        {
            msg = taskMsg;
        }

        public TaskNode( 
            TaskCallBack processDataCallBack,
            TaskCallBack releaseDataCallBack,
            object taskData, TaskMsg msg = TaskMsg.TMSG_DATA)
        {
            this.processDataCallBack = processDataCallBack;
            this.releaseDataCallBack = releaseDataCallBack;
            this.data = taskData;
            this.msg = msg;
        }
    }

    public class TaskQueue
    {
        public int isquit;
        public List<TaskNode> readList = new List<TaskNode>();
        public List<TaskNode> writeList = new List<TaskNode>();

        public List<TaskNode> timerReadList = new List<TaskNode>();
        public List<TaskNode> timerWriteList = new List<TaskNode>();

        public readonly object lockobj = new object();
    }

    public class TaskPump
    {
        readonly int MAX_DELAY_TIME = 0x7FFFFFFF;

        int type = 0;
        Thread thread;
        TaskQueue taskQue;
 
        EventWaitHandle readSem = new EventWaitHandle(false, EventResetMode.ManualReset);
        EventWaitHandle pauseSem = new EventWaitHandle(false, EventResetMode.ManualReset);
        EventWaitHandle quitSem = new EventWaitHandle(false, EventResetMode.ManualReset);

        public TaskPump(int _type = 0)
        {
            type = _type;
            taskQue = new TaskQueue();

            if (type == 0)
            {
                thread = new Thread(new ThreadStart(Run));
                thread.Start();
            }
        }

        public void Run()
        {
            List<TaskNode> readList = taskQue.readList;
            List<TaskNode>  writeList = taskQue.writeList;

            List<TaskNode> timerReadList = taskQue.timerReadList;
            List<TaskNode> timerWriteList = taskQue.timerWriteList;

            DateTime curTime;
            Timer timer;
            int minDelay = MAX_DELAY_TIME;
            int curdelay;
            TaskNode node;
            List<TaskNode> removeItemIdxList = new List<TaskNode>();

            for (;;)
            {
                curTime = DateTime.Now;

                for(int i = 0; i< timerReadList.Count; i++)
                {
                    node = timerReadList[i];
                    curdelay = (int)(curTime - node.startTime).TotalMilliseconds;

                    if (node.delay > 0 && curdelay < node.delay)
                    {
                        if (node.delay - curdelay < minDelay)
                            minDelay = node.delay - curdelay;
                        break;
                    }

                    if (node.msg == TaskMsg.TMSG_TIMER_STOP)
                    {
                        timer = (Timer)node.data;
                        if (timer.taskNode != null){
                            removeItemIdxList.Add(timer.taskNode);         
                        }
                    }

                    if (ProcessTaskNodeData(node) == 1)
                        goto end;

                    removeItemIdxList.Add(node);
                }

                if (removeItemIdxList.Count > 0)
                {
                    foreach (TaskNode n in removeItemIdxList)
                        timerReadList.Remove(n);
                    removeItemIdxList.Clear();
                }


                for (int i = 0; i < readList.Count; i++)
                {
                    node = readList[i];

                    if (ProcessTaskNodeData(node) == 1)
                        goto end;

                    removeItemIdxList.Add(node);
                }

                if (removeItemIdxList.Count > 0)
                {
                    foreach (TaskNode n in removeItemIdxList)
                        readList.Remove(n);
                    removeItemIdxList.Clear();
                }


                //
                if(type == 1 && 
                    writeList.Count == 0 && 
                    timerWriteList.Count == 0)
                {
                    goto end;
                }


                Monitor.Enter(taskQue.lockobj);

                while (type == 0 &&            
                    writeList.Count == 0 &&        
                    timerWriteList.Count == 0)
                {
                    Monitor.Exit(taskQue.lockobj);

                    if (minDelay == MAX_DELAY_TIME)
                    {
                        readSem.WaitOne();
                        readSem.Reset();

                    }
                    else
                    {
                        readSem.WaitOne((int)minDelay);
                        readSem.Reset();

                        Monitor.Enter(taskQue.lockobj);
                        minDelay = MAX_DELAY_TIME;
                        break;
                    }

                    Monitor.Enter(taskQue.lockobj);
                }


                //common
                if (writeList.Count > 0)
                {
                    taskQue.writeList = readList;
                    taskQue.readList = writeList;

                    readList = taskQue.readList;
                    writeList = taskQue.writeList;
                }


                //timer
                if (timerWriteList.Count > 0)
                {
                    minDelay = MAX_DELAY_TIME;

                    if (timerReadList.Count > 0)
                    {
                        timerReadList.AddRange(timerWriteList);
                    }
                    else
                    {
                        taskQue.timerWriteList = timerReadList;
                        taskQue.timerReadList = timerWriteList;
                        timerReadList = taskQue.timerReadList;
                        timerWriteList = taskQue.timerWriteList;
                    }

                    timerReadList.Sort(
                        delegate (TaskNode left, TaskNode right)
                        {
                            int delayA = (int)((curTime - left.startTime).TotalMilliseconds - left.delay);
                            int delayB = (int)((curTime - right.startTime).TotalMilliseconds - right.delay);
                            return delayA.CompareTo(delayB);
                        }
                        );
                }


                Monitor.Exit(taskQue.lockobj);

                if (type != 0)
                    break;
            }

            end:

            if (type == 0)
                quitSem.Set();

        }

        
        int ProcessTaskNodeData(TaskNode node)
        {
            if (node.processDataCallBack != null)
                node.processDataCallBack(node.data);

            if (node.releaseDataCallBack != null)
                node.releaseDataCallBack(node.data);

            TaskMsg taskMsg = node.msg;

            switch (taskMsg)
            {
                case TaskMsg.TMSG_QUIT:
                    quitSem.Set();
                    return 1;

                case TaskMsg.TMSG_PAUSE:
                    pauseSem.WaitOne();
                    pauseSem.Reset();
                    break;
            }

            return 0;

        }

        public int PostTask(TaskNode taskNode, int delay = 0)
        {
            int ret = 0;

            Monitor.Enter(taskQue.lockobj);

            switch (taskNode.msg)
            {
                case TaskMsg.TMSG_TIMER_START:
                    {
                        ret = PostTimerTask(taskNode, -2000);
                    }
                    break;

                case TaskMsg.TMSG_TIMER_RUN:
                    {
                        ret = PostTimerTask(taskNode, delay);
                    }
                    break;

                case TaskMsg.TMSG_TIMER_STOP:
                    {
                        ret = PostCommonTask(taskNode, -1000);
                    }
                    break;

                default:
                    ret = PostCommonTask(taskNode, delay);
                    break;
            }

            Monitor.Exit(taskQue.lockobj);

            //设置任务处理可读信号
            readSem.Set();

            return ret;
        }

        int PostCommonTask(TaskNode taskNode, int delay)
        {
            int ret = 0;

            taskNode.delay = delay;
            if (delay != 0)
                taskNode.startTime = DateTime.Now;

            if (delay == 0)
            {
                taskQue.writeList.Add(taskNode);
            }
            else
            {
                taskQue.timerWriteList.Add(taskNode);
            }

            return ret;
        }

        int PostTimerTask(TaskNode taskNode, int delay)
        {
            taskNode.delay = delay;
            taskNode.startTime = DateTime.Now;
            taskQue.timerWriteList.Add(taskNode);
            return 0;

        }


        public int Quit()
        {
            Continue();

            TaskNode node = new TaskNode(TaskMsg.TMSG_QUIT);
            if (PostTask(node, 0) != 0)
                return -1;

            quitSem.WaitOne();
            quitSem.Reset();
            return 0;
        }

        public int Pause()
        {
            TaskNode node = new TaskNode(TaskMsg.TMSG_PAUSE);
            if (PostTask(node, 0) != 0)
                return -1;

            return 0;
        }

        public int Continue()
        {
            pauseSem.Set();
            return 0;
        }

    }
}
