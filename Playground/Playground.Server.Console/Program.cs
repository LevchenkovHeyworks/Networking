using System;
using System.Threading;
using Playground.LiteNetLib;

namespace Playground.Server.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            StartLiteNetLibServer();
            System.Console.WriteLine("Server started...");
            System.Console.ReadKey();
        }

        private static void StartLiteNetLibServer()
        {
            var logger = System.Console.Out;
            var server = new LiteNetLibServer(logger);

            var timer = new Timer(state => server.PollEvents(), null, 1000, 10);
        }
    }
}
