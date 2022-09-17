namespace GameServer.Cache
{
    /// <summary>
    /// 所有Cache数据的缓存层
    /// </summary>
    public class CacheManager
    {
        public static AccountCache Account { get; set; }

        static CacheManager()
        {
            Account = new AccountCache();
        }


    }
}