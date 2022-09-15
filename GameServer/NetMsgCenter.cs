using System.Reflection.Emit;
using AphliyServer;
using GameServer.Logic;
using Protocol.Code;
using OpCode = Protocol.Code.OpCode;

namespace GameServer
{
    /// <summary>
    /// 网络的消息中心
    /// </summary>
    public class NetMsgCenter:IApplication
    {
        /// <summary>
        /// 账号处理的逻辑层
        /// </summary>
        private IHandle account = new AccountHandle();

        public void OnDisconnect(ClientPeer client)
        {
            account.OnDisconnect(client);
        }

        public void OnReceive(ClientPeer client, SocketMsg msg)
        {
            switch (msg.OpCode)
            {
                case OpCode.Account: 
                    account.OnReceive(client,msg.SubCode,msg.value);
                    break;
            }
            
        }
        
    }
}