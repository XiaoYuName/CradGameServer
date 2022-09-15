using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace AphliyServer
{
    /// <summary>
    /// 封装的客户端
    /// </summary>
    public class ClientPeer
    {
        /// <summary>
        /// 客户端Socket对象
        /// </summary>
        public Socket ClientSocket { get; set;}

        public ClientPeer()
        {
            this.ReceiveArgs = new SocketAsyncEventArgs();
            this.ReceiveArgs.UserToken = this;
            this.ReceiveArgs.SetBuffer(new byte[1024],0,1024);
            this.SendArgs = new SocketAsyncEventArgs();
            this.SendArgs.Completed += SendArgs_Completend;
        }


        #region 接收数据
        
        /// <summary>
        /// 接收的网络异步请求
        /// </summary>
        public SocketAsyncEventArgs ReceiveArgs { get; set; }
        /// <summary>
        /// 是否正在处理接收的数据
        /// </summary>
        private bool IsReceiveProcess = false;
        public delegate void ReceiveCompleted(ClientPeer client, SocketMsg value);
        /// <summary>
        /// 一个消息解析完成的回调
        /// </summary>
        public ReceiveCompleted receiveCompleted;
        /// <summary>
        /// 一旦接收到数据,就存到缓存区里面    
        /// </summary>
        private List<Byte> dataCache = new List<byte>();
        //粘包和拆包问题 : 解决策略:  包头+包未(将消息分为消息头和消息尾)
        //比如发送的数据 : 12345
        /* void test() 粘包和拆包问题代码思路
        // {
        //     byte[] bt = Encoding.UTF8.GetBytes("12345");
        //     // 消息头  : 消息的长度
        //     // 消息尾  : 具体的消息
        //
        //     int lenght = bt.Length;
        //     byte[] bt1 = BitConverter.GetBytes(lenght);
        //     
        //     //最终发送的消息即为 bt1 + bt;
        //     
        //     //怎么读取消息
        //     //int lenght = 前4个字节转换为int类型 
        //     //然后读取 这个长度的数据
        //     
        // }
        */

        /// <summary>
        /// 自身处理数据包
        /// </summary>
        /// <param name="packet"></param>
        public void StartReceive(byte[] packet)
        {
            dataCache.AddRange(packet);
            if(!IsReceiveProcess) //如果没有进行过处理
                ProcessReceive();//进行处理
        }

        /// <summary>
        /// 处理接收的数据
        /// </summary>
        private void ProcessReceive()
        {
            IsReceiveProcess = true;
            //解析数据包
           byte[] data = EncodeTool.DecodePacket(ref dataCache);
           //data 175
           if (data == null)
           {
               IsReceiveProcess = false;
               return;
           }
           
           SocketMsg msg = EncodeTool.DecodeMsg(data);
           //回调给上层
           receiveCompleted?.Invoke(this, msg);
           
           //尾递归
           ProcessReceive();
        }



        #endregion


        #region 断开连接

        /// <summary>
        /// 断开连接
        /// </summary>
        public void Disconnect()
        {
            //1.情况数据
            dataCache.Clear();
            IsReceiveProcess = false;
            sendQueue.Clear();
            isSendProcess = false;
            ClientSocket.Shutdown(SocketShutdown.Both);
            ClientSocket.Close();
            ClientSocket = null;
        }

        #endregion


        #region 发送数据

        /// <summary>
        /// 发送消息的一个队列
        /// </summary>
        private Queue<byte[]> sendQueue = new Queue<byte[]>();
        private bool isSendProcess = false;

        /// <summary>
        /// 发送的时候,发现断开连接的回调
        /// </summary>
        public delegate void SendDisconnect(ClientPeer clientPeer,string reason);

        public SendDisconnect sendDisconnect;
        /// <summary>
        /// 发送的异步套接字
        /// </summary>
        private SocketAsyncEventArgs SendArgs;
        
        /// <summary>
        /// 发送网络消息
        /// </summary>
        /// <param name="opCode">操作吗</param>
        /// <param name="subCode">子操作</param>
        /// <param name="value">参数</param>
        public void Send(int opCode, int subCode,object value)
        {
            SocketMsg msg = new SocketMsg(opCode, subCode, value);
            byte[] data = EncodeTool.EncodeMsg(msg);
            byte[] packet = EncodeTool.EncodePacket(data);
            
            //存入消息队列里
            sendQueue.Enqueue(packet);
            if(!isSendProcess)
                send();
        }

        /// <summary>
        /// 处理发送的消息
        /// </summary>
        private void send()
        {
            isSendProcess = true;
            //如果数据的条数等于0的话,就停止发送
            if (sendQueue.Count == 0)
            {
                isSendProcess = false;
                return;
            }

            //取出一条数据
            byte[] packet = sendQueue.Dequeue();
            
            //设置消息 发送异步套接字操作 的发送数据缓冲区
            SendArgs.SetBuffer(packet,0,packet.Length);
            bool result = ClientSocket.SendAsync(SendArgs);
            if (result == false)
            {
                ProsessSend();
            }
        }

        private void SendArgs_Completend(object sender,SocketAsyncEventArgs e )
        {
            ProsessSend();
        }
        /// <summary>
        /// 当异步发送请求完成的时候调用
        /// </summary>
        private void ProsessSend()
        {
            //发送的有没有错误
            if (SendArgs.SocketError != SocketError.Success)
            {
                // 处理 发送出错了,出错的唯一原因就是客户端断开连接了,这时候就回调给上层,让上层移除自身连接对象引用
                sendDisconnect(this, SendArgs.SocketError.ToString()); 
            }
            else
            {
                send(); //递归调用,处理吓一条消息,知道没有消息处理
            }
            
        } 

        #endregion
    }
}