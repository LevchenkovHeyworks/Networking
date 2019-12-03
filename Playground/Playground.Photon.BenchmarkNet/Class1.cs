using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.IO.Pipes;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;
using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;

namespace NX
{
    public class BenchmarkNet : ApplicationBase
    {
        public static volatile bool processActive = false;
        public static volatile int reliableSent = 0;
        public static volatile int reliableReceived = 0;
        public static volatile int reliableBytesSent = 0;
        public static volatile int reliableBytesReceived = 0;
        public static volatile int unreliableSent = 0;
        public static volatile int unreliableReceived = 0;
        public static volatile int unreliableBytesSent = 0;
        public static volatile int unreliableBytesReceived = 0;
        private const int memoryMappedLength = 512;
        private static BinaryFormatter binaryFormatter;
        private static ServerMessage serverMessage;
        private static MemoryMappedFile serverData;
        private static MemoryMappedViewStream serverStream;
        private static NamedPipeClientStream serverPipeStream;

        protected override PeerBase CreatePeer(InitRequest initRequest)
        {
            return new BenchmarkNetPeer(initRequest);
        }

        protected override void Setup()
        {
            binaryFormatter = new BinaryFormatter();

            serverData = MemoryMappedFile.CreateOrOpen(nameof(BenchmarkNet) + "ServerData", memoryMappedLength, MemoryMappedFileAccess.ReadWrite);
            serverStream = serverData.CreateViewStream(0, memoryMappedLength);
            binaryFormatter.Serialize(serverStream, serverMessage);
            serverStream.Seek(0, SeekOrigin.Begin);

            processActive = true;

            Task.Factory.StartNew(() => {
                serverPipeStream = new NamedPipeClientStream(".", nameof(BenchmarkNet) + "Server", PipeDirection.In);
                serverPipeStream.Connect();
                serverPipeStream.BeginRead(new byte[1], 0, 1, (result) => Process.GetCurrentProcess().Kill(), serverPipeStream);
            }, TaskCreationOptions.LongRunning);

            Task.Factory.StartNew(async () => {
                while (processActive)
                {
                    serverMessage.uninitialized = false;
                    serverMessage.reliableSent = reliableSent;
                    serverMessage.reliableReceived = reliableReceived;
                    serverMessage.reliableBytesSent = reliableBytesSent;
                    serverMessage.reliableBytesReceived = reliableBytesReceived;
                    serverMessage.unreliableSent = unreliableSent;
                    serverMessage.unreliableReceived = unreliableReceived;
                    serverMessage.unreliableBytesSent = unreliableBytesSent;
                    serverMessage.unreliableBytesReceived = unreliableBytesReceived;

                    binaryFormatter.Serialize(serverStream, serverMessage);
                    serverStream.Seek(0, SeekOrigin.Begin);

                    await Task.Delay(15);
                }
            }, TaskCreationOptions.LongRunning);
        }

        protected override void TearDown()
        {
            processActive = false;
        }
    }

    public class BenchmarkNetPeer : ClientPeer
    {
        public BenchmarkNetPeer(InitRequest initRequest) : base(initRequest) { }

        protected override void OnMessage(object message, SendParameters sendParameters)
        {
            byte[] data = (byte[])message;

            if (sendParameters.ChannelId == 2)
            {
                Interlocked.Increment(ref BenchmarkNet.reliableReceived);
                Interlocked.Add(ref BenchmarkNet.reliableBytesReceived, data.Length);
                SendMessage(data, new SendParameters { ChannelId = 0 });
                Interlocked.Increment(ref BenchmarkNet.reliableSent);
                Interlocked.Add(ref BenchmarkNet.reliableBytesSent, data.Length);
            }
            else if (sendParameters.ChannelId == 3)
            {
                Interlocked.Increment(ref BenchmarkNet.unreliableReceived);
                Interlocked.Add(ref BenchmarkNet.unreliableBytesReceived, data.Length);
                SendMessage(data, new SendParameters { ChannelId = 1, Unreliable = true });
                Interlocked.Increment(ref BenchmarkNet.unreliableSent);
                Interlocked.Add(ref BenchmarkNet.unreliableBytesSent, data.Length);
            }
        }

        protected override void OnDisconnect(DisconnectReason disconnectCode, string reasonDetail) { }

        protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters) { }
    }
}