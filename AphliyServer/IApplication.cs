namespace AphliyServer
{
    public interface IApplication
    {
        /// <summary>
        /// 断开数据
        /// </summary>
        /// <param name="client"></param>
        void OnDisconnect(ClientPeer client);

        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="client"></param>
        /// <param name="msg"></param>
        void OnReceive(ClientPeer client, SocketMsg msg);

        // /// <summary>
        // /// 连接
        // /// </summary>
        // /// <param name="client"></param>
        // void OnConnect(ClientPeer client);
    }
}