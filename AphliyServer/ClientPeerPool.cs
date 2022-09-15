using System.Collections.Generic;

namespace AphliyServer
{
    /// <summary>
    /// 客户端的连接池
    ///     作用: 重用客户端的连接对象
    /// </summary>
    public class ClientPeerPool
    {
        private Queue<ClientPeer> clientPeerQueue;

        /// <summary>
        /// 构造对象池
        /// </summary>
        /// <param name="capacity">池中初始数量</param>
        public ClientPeerPool(int capacity)
        {
            clientPeerQueue = new Queue<ClientPeer>(capacity);
        }

        /// <summary>
        /// 将对象放入池中
        /// </summary>
        /// <param name="client"></param>
        public void Enqueue(ClientPeer client)
        {
            clientPeerQueue.Enqueue(client);
        }

        /// <summary>
        /// 获取池对象
        /// </summary>
        /// <returns></returns>
        public ClientPeer Dequeue()
        {
            return clientPeerQueue.Dequeue();
        }
    }
}