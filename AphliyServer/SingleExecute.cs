using System.Threading;

namespace AphliyServer
{
    /// <summary>
    /// 一个需要执行的方法
    /// </summary>
    public delegate  void  ExecuteDelegate();
    
    /// <summary>
    /// 单线程池
    /// </summary>
    public class SingleExecute
    {
        /// <summary>
        /// 互斥锁
        /// </summary>
        public Mutex Mutex;

        public SingleExecute()
        {
            Mutex = new Mutex();
        }
        
        /// <summary>
        /// 单线程处理逻辑
        /// </summary>
        /// <param name="delegate"></param>
        public void Execute(ExecuteDelegate @delegate)
        {
            lock (this)
            {
                Mutex.WaitOne();
                @delegate();
                Mutex.ReleaseMutex();
            }
        }

    }
}