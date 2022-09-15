namespace AphliyServer.Timera
{
    /// <summary>
    /// 当定时器达到时间后的触发
    /// </summary>
    public  delegate  void TimerDelegate();
    
    /// <summary>
    /// 定时器的数据模型
    /// </summary>
    public class TimerModel
    {
        public int id;
        /// <summary>
        /// 任务执行的事件
        /// </summary>
        public long Timer;
        private TimerDelegate timeDelegate;
        
        public TimerModel(int id,long Timer,TimerDelegate timeDelegate)
        {
            this.id = id;
            this.Timer = Timer;
            this.timeDelegate = timeDelegate;
        }

        /// <summary>
        /// 触发任务
        /// </summary>
        public void Run()
        {
            // timeDelegate();
            timeDelegate?.Invoke();
        }
    }
}