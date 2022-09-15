using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace AphliyServer
{
    /// <summary>
    /// 关于编码的工具类
    /// </summary>
    public static class EncodeTool
    {
        #region 粘包拆包问题 封装一个有规定的数据包

        /// <summary>
        /// 构造数据包 : 包头 + 包尾
        /// </summary>
        /// <param name="data">正常要发送的数据包</param>
        /// <returns>返回构造完毕的数据包</returns>
        public static byte[] EncodePacket(byte[] data)
        {
            //内存流对象
            using (MemoryStream ms = new MemoryStream()) //直接使用using 这样花括号之后就会自动释放对象
            {
                //二进制写入对象
                using ( BinaryWriter bw = new BinaryWriter(ms))
                {
                    //先写入长度
                    bw.Write(data.Length);
                    //再写入消息
                    bw.Write(data);

                    byte[] byteArray = new byte[(int)ms.Length];
                    Buffer.BlockCopy(ms.GetBuffer(),0,byteArray,0,(int)ms.Length);
                    return byteArray;
                }
            }
        }

        /// <summary>
        /// 解析数据包: 从缓存区里取出一个一个完整的数据包 
        /// </summary>
        /// <param name="dataCache">缓存区</param>
        /// <returns>解析后的数据包</returns>
        public static byte[] DecodePacket(ref List<byte> dataCache)
        {
            // 4个字节 构成一个int 长度 不能构成一个完整的消息,那么就抛出一个异常
            if (dataCache.Count < 4)
                return null;// throw new Exception("数据缓存长度不足4,不能构成一个完整的消息");
            using (MemoryStream ms = new MemoryStream(dataCache.ToArray())) //直接使用using 这样花括号之后就会自动释放对象                                    
            {                                                                                                           
                //二进制写入对象                                                                                               
                using ( BinaryReader br = new BinaryReader(ms))
                {
                    int length = br.ReadInt32();
                    int dataRemainLenght = (int) (ms.Length - ms.Position);
                    if (length > dataRemainLenght)
                        return null;//throw new Exception("数据缓存长度不够包头约定的长度,不能构成一个完整的消息");
                    byte[] data = br.ReadBytes(length);
                    //更新下缓存区
                    dataCache.Clear();
                    dataCache.AddRange(br.ReadBytes(dataRemainLenght));
                    return data;
                }                                                                                                       
            }                                                                                                           
        }

        #endregion

        #region 构造发送的SocketMsg 类

        /// <summary>
        /// 把SocketMsg 转换成字节数组,发送出去
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static byte[] EncodeMsg(SocketMsg msg)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    bw.Write(msg.OpCode);
                    bw.Write(msg.SubCode);
                    //如果不等于null 才需要吧Object 转换成字节数组存起来
                    if (msg.value != null)
                    {
                        byte[] valueBytes = EncodeObj(msg.value);
                        bw.Write(valueBytes);
                    }

                    byte[] data = new byte[ms.Length];
                    Buffer.BlockCopy(ms.GetBuffer(),0,data,0,(int)ms.Length);
                    return data;
                }
            }
            
        }

        /// <summary>
        /// 将收到的字节数组转化成SocketMsg 对象,供我们使用
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static SocketMsg DecodeMsg(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    SocketMsg msg = new SocketMsg();
                    msg.OpCode = br.ReadInt32();
                    msg.SubCode = br.ReadInt32();
                    //还有剩余的字节数据没读取 代表value 有值
                    if (ms.Length > ms.Position)
                    {
                        byte[] valueBytes = br.ReadBytes((int) (ms.Length - ms.Position));
                        object value = DecodeObj(valueBytes);
                        msg.value = value;
                    }
                    return msg;
                }
            }
        }

        #endregion


        #region 把一个Object 类型转换成字节数组byte[]
        /*Prompt: 这是使用了C# 的序列化, 如果要使用Unity.ScriptableObject 的话,就进行Json序列化,进行更改或者重载
         更好的方法,就是将此类作为接口使用,应用层就去实现各自的接口*/
        
        /// <summary>
        /// 序列化对象
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] EncodeObj(object value)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms,value);
                byte[] valueBytes = new byte[ms.Length];
                Buffer.BlockCopy(ms.GetBuffer(),0,valueBytes,0,(int)ms.Length);
                return valueBytes;
            }
        }

        /// <summary>
        /// 反序列化对象
        /// </summary>
        /// <param name="valueBytes"></param>
        /// <returns></returns>
        public static object DecodeObj(byte[] valueBytes)
        {
            using (MemoryStream ms = new MemoryStream(valueBytes))
            {
                BinaryFormatter bf = new BinaryFormatter();
                object value = bf.Deserialize(ms);
                return value;
            }
        }



        #endregion
    }
}