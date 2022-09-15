namespace AphliyServer.Concurrent
{
    /// <summary>
    /// 线程安全的Int 类型
    /// </summary>
    public class ConcurrentInt
    {
        private int value;

        public ConcurrentInt(int value)
        {
            this.value = value;
        }

        /// <summary>
        /// 添加并获取
        /// </summary>
        /// <returns></returns>
        public int Add_Get()
        {
            lock (this)
            {
                value++;
                return value;
            }
        }
        
        /// <summary>
        /// 减少并获取
        /// </summary>
        /// <returns></returns>
        public int Reduce_Get()
        {
            lock (this)
            {
                value--;
                return value;
            }
        }
        
        /// <summary>
        /// 获取
        /// </summary>
        /// <returns></returns>
        public int Getvalue()
        {
            return value;
        }
    }
}