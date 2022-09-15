using AphliyServer;

namespace GameServer.Logic
{
    public interface IHandle
    {
        /// <summary>
        /// 断开连接
        /// </summary>
        /// <param name="client">连接的客户端对象</param>
        public void OnDisconnect(ClientPeer client);
        
        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="client">客户端对象</param>
        /// <param name="subCode">子操作码</param>
        /// <param name="value">数据</param>
        public void OnReceive(ClientPeer client, int subCode, object value);
    }
}