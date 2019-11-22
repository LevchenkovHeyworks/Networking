using System;
using System.Text;
using System.Threading;
using Playground.LiteNetLib;

namespace Playground.Client.Console
{
    class Program
    {
        private const string DATA = "The quick brown fox jumps over the lazy dog";
        private static int MAX_LOOP_COUNT = 100;
        private static int UNRELIABLE_MESSAGES_PER_LOOP = 1000;
        private static int RELIABLE_MESSAGES_PER_LOOP = 100;

        static void Main(string[] args)
        {
            Thread clientThread = new Thread(StartClient);
            System.Console.WriteLine("Client started...");
            clientThread.Start();
            System.Console.ReadKey();
        }

        private static void StartClient()
        {
            var logger = System.Console.Out;

            var client = new LiteNetLibClient(logger);
            client.Connect();

            for (int i = 0; i < MAX_LOOP_COUNT; i++)
            {
                //for (int ui = 0; ui < UNRELIABLE_MESSAGES_PER_LOOP; ui++)
                //    c.SendUnreliable(DATA);

                for (int ri = 0; ri < RELIABLE_MESSAGES_PER_LOOP; ri++)
                {
                    client.SendReliable(DATA);
                }

                logger.WriteLine("Sent {0} messages", RELIABLE_MESSAGES_PER_LOOP);
                Thread.Sleep(10);
            }

            int dataSize = MAX_LOOP_COUNT * Encoding.UTF8.GetByteCount(DATA) * (RELIABLE_MESSAGES_PER_LOOP);
            logger.WriteLine("DataSize: {0}b, {1}kb, {2}mb", dataSize, dataSize / 1024, dataSize / 1024 / 1024);

            Thread.Sleep(10000);
            //CLIENT_RUNNING = false;

            logger.WriteLine("CLIENT STATS:\n" + client.Stats);
        }
    }
}
