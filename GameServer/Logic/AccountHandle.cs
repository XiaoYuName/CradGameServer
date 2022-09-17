using System;
using AphliyServer;
using GameServer.Cache;
using Protocol.Code;
using Protocol.Dto;
using Protocol.Exception;

namespace GameServer.Logic
{
    /// <summary>
    /// 账号处理的逻辑层
    /// </summary>
    public class AccountHandle:IHandle
    {
        private AccountCache Account = CacheManager.Account;
        
        //断开连接
        public void OnDisconnect(ClientPeer client)
        {
            if(Account.IsOnline(client)) //如果在线中,才可以进行下线处理
                Account.Offline(client);
        }
        //接收数据
        public void OnReceive(ClientPeer client,int subCode,object value)
        {
            switch (subCode)
            {
                case AccountCode.REGIST_CREQ:
                {
                    AccountDto dto = value as AccountDto;
                    SingleExecute.Instance.Execute(()=>regist(client,dto.Account,dto.PassWord));
                }
                    break;
                case AccountCode.LOGIN:
                {
                    AccountDto dto = value as AccountDto;
                    //单线程池处理
                    SingleExecute.Instance.Execute(()=>Login(client,dto.Account,dto.PassWord));
                }
                    break;
                default:
                    break;
            }
            
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="client">连接对象</param>
        /// <param name="account">账号</param>
        /// <param name="password">密码</param>
        private void regist(ClientPeer client ,string account, string password)
        {
            if (Account.isExist(account))
                {
                    //表示账号已经存在,TODO: 给客户端返回账号已存在
                    client.Send(OpCode.Account,AccountCode.REGIST_SRES,AccountException.AccountExist);
                    return;
                }
            if (string.IsNullOrEmpty(account) )
                {
                    //表示账号或密码输入不合法
                    client.Send(OpCode.Account,AccountCode.REGIST_SRES,AccountException.AccountisNull);
                    return;
                }
            if (string.IsNullOrEmpty(password)|| password.Length < 4 || password.Length > 16)
                {
                    //表示密码长度不合法
                    client.Send(OpCode.Account,AccountCode.REGIST_SRES,AccountException.AccountPasswordNull);
                    return;
                }
            client.Send(OpCode.Account,AccountCode.REGIST_SRES,AccountException.Regist);
            Account.Create(account,password);
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="client">连接对象</param>
        /// <param name="account">账号</param>
        /// <param name="password">密码</param>
        private void Login(ClientPeer client, string account, string password)
        {
           
            if (!Account.isExist(account)) //判断是否有该账号
            {
                client.Send(OpCode.Account,AccountCode.REGIST_SRES,AccountException.LoginExist);
                return;
            }
            if (Account.IsOnline(client)) //判断账号是否已经登录
            {
                client.Send(OpCode.Account,AccountCode.REGIST_SRES,(int)AccountException.LoginOnline);
                return;
            }

            if (!Account.IsMatch(account, password))//表示账号密码不匹配
            {
                client.Send(OpCode.Account,AccountCode.REGIST_SRES,(int)AccountException.LoginMatch);
                return;
            }
            //登录成功
            client.Send(OpCode.Account,AccountCode.REGIST_SRES,(int)AccountException.Login);
            Account.OnLine(client,account);
        }
    }
}