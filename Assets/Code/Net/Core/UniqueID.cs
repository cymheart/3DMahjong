using System;
using System.Threading;

namespace Net
{
    public class UniqueID
    {   
        long last_stamp = 0;
        long workid = 0;
        long seqid = 0;
        static long sequenceMask = -1L ^ (-1L << 12);

        // 定义一个静态变量来保存类的实例
        private static UniqueID uniqueInstance;

        private static readonly object objLock = new object();

        // 定义一个标识确保线程同步
        private static readonly object locker = new object();

        // 定义私有构造函数，使外界不能创建该类实例
        private UniqueID()
        {
        }

        /// <summary>
        /// 定义公有方法提供一个全局访问点,同时你也可以定义公有属性来提供全局访问点
        /// </summary>
        /// <returns></returns>
        public static UniqueID GetInstance()
        {
            // 当第一个线程运行到这里时，此时会对locker对象 "加锁"，
            // 当第二个线程运行该方法时，首先检测到locker对象为"加锁"状态，该线程就会挂起等待第一个线程解锁
            // lock语句运行完之后（即线程运行完之后）会对该对象"解锁"
            // 双重锁定只需要一句判断就可以了
            if (uniqueInstance == null)
            {
                lock (locker)
                {
                    // 如果类的实例不存在则创建，否则直接返回
                    if (uniqueInstance == null)
                    {
                        uniqueInstance = new UniqueID();
                    }
                }
            }
            return uniqueInstance;
        }


        public void set_workid(long workid)
        {
            this.workid = workid;
        }

        long get_curr_ms()
        {
            DateTime curtDate = DateTime.Now;
            long tm = curtDate.ToFileTimeUtc();
            return tm;
        }

        long wait_next_ms(long lastStamp)
        {
            long cur = 0;
            do
            {
                cur = get_curr_ms();
            } while (cur <= lastStamp);
            return cur;
        }

        public ulong gen()
        {
            ulong uniqueId = 0;
            long nowtime = get_curr_ms();
            uniqueId = (ulong)nowtime << 22;
            uniqueId |= (ulong)((workid & 0x3ff) << 12);

            if (nowtime < last_stamp)
            {
                return 0;
            }
            if (nowtime == last_stamp)
            {
                seqid = Interlocked.Add(ref seqid, 1) & sequenceMask;

                if (seqid == 0)
                {
                    nowtime = wait_next_ms(last_stamp);
                    uniqueId = (ulong)nowtime << 22;
                    uniqueId |= (ulong)((workid & 0x3ff) << 12);
                }
            }
            else
            {
                seqid = 0;
            }
            last_stamp = nowtime;
            uniqueId |= (ulong)seqid;
            return uniqueId;
        }


        public ulong gen_multi()
        {
            ulong uniqueId = 0;
            lock (objLock)
            {
                uniqueId = gen();
            }

            return uniqueId;
        }



    }
}
