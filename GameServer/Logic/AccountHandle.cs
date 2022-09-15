using System;
using AphliyServer;
using Protocol.Code;
using Protocol.Dto;

namespace GameServer.Logic
{
    /// <summary>
    /// 账号处理的逻辑层
    /// </summary>
    public class AccountHandle:IHandle
    {
        //断开连接
        public void OnDisconnect(ClientPeer client)
        {
            
        }
        //接收数据
        public void OnReceive(ClientPeer client,int subCode,object value)
        {
            switch (subCode)
            {
                case AccountCode.REGIST_CREQ:
                {
                    AccountDto dto = value as AccountDto;
                    Console.WriteLine(dto.Account);
                    Console.WriteLine(dto.PassWord);
                }
                    break;
                case AccountCode.LOGIN:
                {
                    AccountDto dto = value as AccountDto;
                    Console.WriteLine(dto.Account);
                    Console.WriteLine(dto.PassWord);
                }
                    break;
                default:
                    break;
            }
        }
    }
}