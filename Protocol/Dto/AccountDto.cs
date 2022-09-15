using System;

namespace Protocol.Dto
{
    [Serializable]
    public class AccountDto
    {
        /// <summary>
        /// 账号
        /// </summary>
        public string Account;
        /// <summary>
        /// 密码
        /// </summary>
        public string PassWord;

        public AccountDto()
        {
            
        }

        public AccountDto(string acc,string pwd)
        {
            this.Account = acc;
            this.PassWord = pwd;
        }
    }
}