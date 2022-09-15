using System;
using AphliyServer;

namespace GameServer
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerPeer Server = new ServerPeer();
            Server.SetApplication( new NetMsgCenter());
            Server.Start(6666,10);
            Console.ReadKey();
        }
    }
}