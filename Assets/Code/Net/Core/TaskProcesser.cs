
namespace Net
{
    public abstract class TaskProcesser
    {
        public abstract bool Start();
        public abstract void Stop();
        public abstract void Pause();
        public abstract void Continue();
        public abstract int PostTask(TaskNode taskNode, int delay = 0);
        public abstract int PostTask(TaskCallBack processDataCallBack, TaskCallBack releaseDataCallBack, object taskData, TaskMsg msg = TaskMsg.TMSG_DATA, int delay = 0);
    }


    public class  CommonTaskProcesser: TaskProcesser
    {
        TaskPump taskPump = null;
        public override bool Start()
        {
            if (taskPump != null)
                return true;

            taskPump = new TaskPump();
            return true;
        }

        public override void Stop()
        {
            if (taskPump != null)
            {
                taskPump.Quit();
                taskPump = null;
            }
        }

        public override void Pause()
        {
            if (taskPump != null)
                taskPump.Pause();
        }
        public override void Continue()
        {
            if (taskPump != null)
                taskPump.Continue();
        }

        public override int PostTask(TaskNode taskNode, int delay = 0)
        {
            if (taskPump == null)
                return -1;

            return taskPump.PostTask(taskNode, delay);
        }

        public override int PostTask(TaskCallBack processDataCallBack, TaskCallBack releaseDataCallBack, object taskData, TaskMsg msg =TaskMsg.TMSG_DATA, int delay = 0)
        {
            if (taskPump == null)
                return -1;

            TaskNode taskNode = new TaskNode(processDataCallBack, releaseDataCallBack, taskData, msg);
            return taskPump.PostTask(taskNode, delay);
        }
    }
}
