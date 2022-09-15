namespace Protocol.Code
{
    /// <summary>
    /// 登录操作码
    /// </summary>
    public class AccountCode
    {
        /// <summary>
        /// 注册请求
        /// </summary>
        public const int REGIST_CREQ = 0; //Client Request  //参数 AccountDto
        /// <summary>
        /// 服务器登录响应
        /// </summary>
        public const int REGIST_SRES = 1; //Sever Request  参数 AccountDto
        /// <summary>
        /// 登录操作码
        /// </summary>
        public const int LOGIN = 2; // 参数 AccountDto
    }
}