namespace GameServer.Model
{
    /// <summary>
    /// 账号的模型层
    /// </summary>
    public class AccountModel
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public int id;
        /// <summary>
        /// 账号:
        /// </summary>
        public string Account;
        /// <summary>
        /// 密码:
        /// </summary>
        public string Password;
        
        //TODO: 模型层表示存储的信息,可以添加更多例如:创建日期,电话号码 
        public AccountModel(int id,string account,string password)
        {
            this.id = id;
            this.Account = account;
            this.Password = password;
        }
    }
}