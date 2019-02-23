
namespace Task
{
    public class Timer : TaskDataBase
    {
        public delegate bool TimerCallBack(TaskDataBase data);

        int durationMS = 0;
        TaskProcesser taskProcesser = null;
        TimerCallBack timerCB = null;
        TaskDataBase param = null;
        bool isStop = true;
        bool isRepeat = true;
        public TaskNode taskNode = null;

        public Timer(TaskProcesser taskProcesser, int durationMS, TimerCallBack timerCB, TaskDataBase param, bool isRepeat = true)
        {
            taskDataType = TaskDataType.TDATA_TIMER;

            this.param = param;
            this.timerCB = timerCB;

            this.taskProcesser = taskProcesser;
            this.durationMS = durationMS;
            this.isRepeat = isRepeat;
        }

        public void Start()
        {
            taskProcesser.PostTask(StartTask, null, this, TaskMsg.TMSG_TIMER_START);
        }
        public void Stop()
        {
            taskProcesser.PostTask(StopTask, null, this, TaskMsg.TMSG_TIMER_STOP);
        }

        void PostTask(TaskDataBase data)
        {
            taskNode = new TaskNode(RunTask, null, data, TaskMsg.TMSG_TIMER_RUN);
            taskProcesser.PostTask(taskNode, durationMS);
        }

        void RunTask(TaskDataBase data)
        {
            if (isStop)
                return;

            timerCB(param);
            taskNode = null;

            if (isRepeat)
                PostTask(data);
            else
                isStop = true;
        }

        void StartTask(TaskDataBase data)
        {
            if (isStop == false)
                return;

            isStop = false;
            PostTask(data);
        }

        void StopTask(object data)
        {
            taskNode = null;
            if (isStop)
                return;
            isStop = true;
        }
    }
}
