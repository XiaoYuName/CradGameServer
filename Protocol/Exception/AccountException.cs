namespace Protocol.Exception
{
    /// <summary>
    /// 服务器反馈处理信息
    /// </summary>
    public enum AccountException
    {
        #region 注册
        /// <summary>
        /// 账号已存在
        /// </summary>
        AccountExist,
        /// <summary>
        /// 账号不合法
        /// </summary>
        AccountisNull,
        /// <summary>
        /// 密码不合法
        /// </summary>
        AccountPasswordNull,
        /// <summary>
        /// 注册成功
        /// </summary>
        Regist,
        #endregion

        #region 登录
        /// <summary>
        /// 没有该账号
        /// </summary>
        LoginExist,
        /// <summary>
        /// 账号在线中
        /// </summary>
        LoginOnline,
        /// <summary>
        /// 账号密码不匹配
        /// </summary>
        LoginMatch,
        /// <summary>
        /// 账号登录成功
        /// </summary>
        Login,
        #endregion
    }
}