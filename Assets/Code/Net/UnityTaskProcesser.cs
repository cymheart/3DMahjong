using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Net
{
    public class UnityTaskProcesser : TaskProcesser
    {
        UnityUpdate unityUpdate;
        public UnityTaskProcesser(UnityUpdate unityUpdate)
        {
            this.unityUpdate = unityUpdate;
        }

        public override void Continue()
        {
            throw new NotImplementedException();
        }

        public override void Pause()
        {
            throw new NotImplementedException();
        }

        public override int PostTask(TaskNode taskNode, int delay = 0)
        {
            if (unityUpdate.taskPump == null)
                return -1;

            return unityUpdate.taskPump.PostTask(taskNode, delay);
        }

        public override int PostTask(TaskCallBack processDataCallBack, TaskCallBack releaseDataCallBack, object taskData, TaskMsg msg = TaskMsg.TMSG_DATA, int delay = 0)
        {
            if (unityUpdate.taskPump == null)
                return -1;

            TaskNode taskNode = new TaskNode(processDataCallBack, releaseDataCallBack, taskData, msg);
            return unityUpdate.taskPump.PostTask(taskNode, delay);
        }

        public override bool Start()
        { 
            unityUpdate.taskPump = new TaskPump(1);
            return true;
        }

        public override void Stop()
        {
     
        }

    }


    public class UnityUpdate : MonoBehaviour
    {
        public TaskPump taskPump = null;
        private void Update()
        {
            if(taskPump != null)
                taskPump.Run();
        }    
    }

}
