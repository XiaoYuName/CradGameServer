using System.Collections.Generic;
using AphliyServer;
using AphliyServer.Concurrent;
using GameServer.Model;

namespace GameServer.Cache
{
    /// <summary>
    /// 账号的缓存区
    ///     临时存储账号密码: 
    /// </summary>
    public class AccountCache
    {
        /// <summary>
        /// 用来存储账号的ID   
        /// </summary>
        private ConcurrentInt id = new ConcurrentInt(-1);

        /// <summary>
        /// 账号对应的数据模型缓存区
        /// </summary>
        private Dictionary<string, AccountModel> AccountModelDict = new Dictionary<string, AccountModel>();

        /// <summary>
        /// 判断服务器是否已经有这个账号
        /// </summary>
        /// <param name="account">账号</param>
        /// <returns>如果有返回true,如果没有返回false</returns>
        public bool isExist(string account)
        {
            return AccountModelDict.ContainsKey(account);
        }

        /// <summary>
        /// 创建账号数据模型信息
        /// </summary>
        /// <param name="accout">账号</param>
        /// <param name="password">密码</param>
        public void Create(string accout, string password)
        {
            AccountModelDict.TryAdd(accout, new AccountModel(id.Add_Get(), accout, password));
        }

        /// <summary>
        /// 获取账号对应的数据模型
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public AccountModel GetAccountModel(string account)
        {
            return AccountModelDict[account];
        }

        /// <summary>
        /// 账号密码是否匹配
        /// </summary>
        /// <param name="account">账号</param>
        /// <param name="password">密码</param>
        /// <returns>如果匹配则返回true,否则返回false</returns>
        public bool IsMatch(string account, string password)
        {
            AccountModel model = AccountModelDict[account];
            return model.Password == password;
        }

        /// <summary>
        /// 账号对应的连接对象
        /// </summary>
        private Dictionary<string, ClientPeer> accClientDict = new Dictionary<string, ClientPeer>();

        private Dictionary<ClientPeer, string> clientaccDic = new Dictionary<ClientPeer, string>();

        /// <summary>
        /// 判断账号是否在线
        /// </summary>
        /// <param name="accout">账号</param>
        /// <returns>如果在线返回true,否则返回false</returns>
        public bool IsOnline(string accout)
        {
            return accClientDict.ContainsKey(accout);
        }

        /// <summary>
        /// 判断账号是否在线
        /// </summary>
        /// <param name="peer">连接对象</param>
        /// <returns>如果在线返回true,否则返回false</returns>
        public bool IsOnline(ClientPeer peer)
        {
            return clientaccDic.ContainsKey(peer);
        }

        /// <summary>
        /// 上线客户端连接对象
        /// </summary>
        /// <param name="peer">连接对象</param>
        public void OnLine(ClientPeer peer, string account)
        {
            accClientDict.Add(account, peer);
            clientaccDic.Add(peer, account);
        }

        /// <summary>
        /// 下线客户端连接对象
        /// </summary>
        /// <param name="peer"></param>
        public void Offline(ClientPeer peer)
        {
            string account = clientaccDic[peer];
            clientaccDic.Remove(peer);
            accClientDict.Remove(account);
        }

        /// <summary>
        /// 下线客户端连接对象
        /// </summary>
        /// <param name="account">账号</param>
        public void Offline(string account)
        {
            ClientPeer client = accClientDict[account];
            accClientDict.Remove(account);
            clientaccDic.Remove(client);
        }

        /// <summary>
        /// 获取在线玩家的ID
        /// </summary>
        /// <param name="peer">连接对象</param>
        /// <returns></returns>
        public int GetID(ClientPeer peer)
        {
            string account = clientaccDic[peer];
            AccountModel model = AccountModelDict[account];
            return model.id;
        }
    }
}