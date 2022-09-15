using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace AphliyServer
{
    /// <summary>
    /// 服务器端
    /// </summary>
    public class ServerPeer
    {
        /// <summary>
        /// 服务器的Socket 对象
        /// </summary>
        private Socket SeverSocket;
        /// <summary>
        /// 限制客户端连接数量的信号量
        /// </summary>
        private Semaphore acceptSemaphore;
        /// <summary>
        /// 客户端对象的连接池
        /// </summary>
        private ClientPeerPool clientPeerPool;

        /// <summary>
        /// 应用层
        /// </summary>
        private IApplication application;

        /// <summary>
        /// 设置应用层
        /// </summary>
        /// <param name="app"></param>
        public void SetApplication(IApplication app)
        {
            this.application = app;
        }

        /// <summary>
        /// 用来开启服务器
        /// </summary>
        /// <param name="port">端口号</param>
        /// <param name="maxCount">最大连接数量</param>
        public void Start(int port, int maxCount)
        {
            try
            {
                //实例化Socket 对象
                SeverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                acceptSemaphore = new Semaphore(maxCount, maxCount);
                //创建客户端对象池
                clientPeerPool = new ClientPeerPool(maxCount);
                //直接new 出最大的连接数量,这样除了服务器启动的瞬间有些占用内存和卡顿之外,使用时候就没有卡顿状况了
                for (int i = 0; i < maxCount; i++)
                {
                    var tempClientPeer = new ClientPeer();
                    tempClientPeer.ReceiveArgs.Completed += receive_Completed;
                    tempClientPeer.receiveCompleted = receiveCompleted;
                    tempClientPeer.sendDisconnect = Disconnect;
                    clientPeerPool.Enqueue(tempClientPeer);
                }
                
                //1.绑定Socket 对象,接收(IPAdderess.Any) 所有IP地址的连接
                SeverSocket.Bind(new IPEndPoint(IPAddress.Any,port));
                //2.开始监听来自客户端的连接
                SeverSocket.Listen(10);
                
                Console.WriteLine("服务器启动.....");
                StartAccept(null);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }


        #region 连接

        /// <summary>
        /// 开始等待客户端的连接
        /// </summary>
        private void StartAccept(SocketAsyncEventArgs Async)
        {
            if (Async == null)
            {
                Async = new SocketAsyncEventArgs();
                Async.Completed += AsyncOnCompleted;
            }
            //返回值判断异步事件是否执行完毕,如果返回了ture 则表示正在执行,执行完毕后会触发
            //如果返回false 代表已经执行完成,直接处理
            bool result = SeverSocket.AcceptAsync(Async);
            if (result == false)
            {
                ProcessAccpet(Async);
            }
        }

        /// <summary>
        /// 接收连接请求异步事件完成后触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AsyncOnCompleted(object sender, SocketAsyncEventArgs e)
        {
            ProcessAccpet(e);
        }
        
        /// <summary>
        /// 处理连接请求
        /// </summary>
        private void ProcessAccpet(SocketAsyncEventArgs Async)
        {
            //限制线程的访问
            acceptSemaphore.WaitOne();  
                
            //得到客户端连接的对象
            ClientPeer clientSocket = clientPeerPool.Dequeue();
            clientSocket.ClientSocket =Async.AcceptSocket;
            // application.OnConnect(clientSocket);
            Console.WriteLine("客户端连接成功 :" +  clientSocket.ClientSocket.RemoteEndPoint.ToString());
            //开始接收数据
            StarReceive(clientSocket);
            
            Async.AcceptSocket = null;
            StartAccept(Async);
        }
        
        #endregion
        
        #region 接收数据

        /// <summary>
        /// 开始接收数据
        /// </summary>
        /// <param name="client"></param>
        private void StarReceive(ClientPeer client)
        {
            try
            {
                bool result = client.ClientSocket.ReceiveAsync(client.ReceiveArgs);
                if (result == false)
                {
                    ProcessReceive(client.ReceiveArgs);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        /// <summary>
        /// 处理接收的请求
        /// </summary>
        /// <param name="Async"></param>
        private void ProcessReceive(SocketAsyncEventArgs Async)
        {
            ClientPeer client = Async.UserToken as ClientPeer;
            //判断网络消息是否接收成功
            if (client.ReceiveArgs.SocketError == SocketError.Success && client.ReceiveArgs.BytesTransferred >0)
            {
                byte[] packet = new byte[client.ReceiveArgs.BytesTransferred];
                //对接收到的数据进行拷贝
                Buffer.BlockCopy(client.ReceiveArgs.Buffer,0,packet,0,
                    client.ReceiveArgs.BytesTransferred);
                //让客户端自身处理这个数据包 自身解析
                client.StartReceive(packet);
                //开启一个伪递归
                StarReceive(client);
            }
            //断开连接 //如果没有传输的数据数, 就代表断开连接了
            else if (client.ReceiveArgs.BytesTransferred == 0)
            {
                if (client.ReceiveArgs.SocketError == SocketError.Success)
                {
                    //客户端主动断开连接
                    Disconnect(client,"客户端主动断开连接");
                }
                else
                {
                    //由于网络异常 导致被动断开连接
                    Disconnect(client,client.ReceiveArgs.SocketError.ToString());
                }
            }

        }
        /// <summary>
        /// 当接收完成时候触发的事件
        /// </summary>
        /// <param name="Async"></param>
        private void receive_Completed(object sender,SocketAsyncEventArgs Async)
        {
            ProcessReceive(Async);
        }

        /// <summary>
        /// 一条数据解析完成的回调
        /// </summary>
        /// <param name="client">对应的连接客户端对象</param>
        /// <param name="value">解析出来一个具体能使用的类型</param>
        private void receiveCompleted(ClientPeer client, SocketMsg value)
        {
            //给应用层,让其使用
            application.OnReceive(client,value);
        }

        #endregion

        #region 发送

        

        #endregion

        #region 断开连接

        /// <summary>
        /// 断开连接
        /// </summary>
        /// <param name="client">连接对象</param>
        /// <param name="reason">断开连接的原因</param>
        public void Disconnect(ClientPeer client,string reason)
        {
            try //捕获异常
            {
                //情空一些数据
                if (client == null)
                    throw new Exception("当前指定的客户端连接对象为空,无法断开连接");
                
                Console.WriteLine(client.ClientSocket.RemoteEndPoint + " 客户端断开连接 原因: "+reason);
                //通知应用层 客户端断开连接
                application.OnDisconnect(client);
                client.Disconnect();
                //回收对象,方便下次使用
                clientPeerPool.Enqueue(client);
                acceptSemaphore.Release();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }


        #endregion


    }
}