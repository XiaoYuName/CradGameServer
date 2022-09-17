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
        private static SingleExecute instance = null;
        private static object lo = new object();

        /// <summary>
        /// 单例
        /// </summary>
        public static SingleExecute Instance
        {
            get
            {
                lock (lo)
                {
                    return instance ?? (instance = new SingleExecute());
                }
                
            }
        }

        /// <summary>
        /// 互斥锁
        /// </summary>
        public Mutex Mutex;

        private SingleExecute()
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