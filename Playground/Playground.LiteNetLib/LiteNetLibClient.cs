using System.IO;
using System.Net;
using System.Net.Sockets;
using LiteNetLib;
using LiteNetLib.Utils;

namespace Playground.LiteNetLib
{
    public class LiteNetLibClient : INetEventListener
    {
        private readonly TextWriter logger;

        private readonly NetManager client;
        private readonly NetDataWriter writer;
        private NetPeer peer;

        public LiteNetLibClient(TextWriter logger)
        {
            this.logger = logger;

            writer = new NetDataWriter();

            client = new NetManager(this);
            client.UnsyncedEvents = true;
            client.AutoRecycle = true;
            client.SimulatePacketLoss = false;
            client.SimulationPacketLossChance = 20;
            client.Start();
        }

        public void SendReliable(string message)
        {
            writer.Reset();
            writer.Put(message);

            peer.Send(writer, DeliveryMethod.ReliableOrdered);
        }

        public void Connect()
        {
            peer = client.Connect("localhost", 9050, "qwe");
        }

        public NetStatistics Stats => client.Statistics;

        public void OnPeerConnected(NetPeer peer)
        {
            logger.WriteLine("OnPeerConnected Id: {0}, State: {1}", peer.Id, peer.ConnectionState);
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            logger.WriteLine("OnPeerDisconnected Id: {0}, State: {1}", peer.Id, peer.ConnectionState);
        }

        public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
            logger.WriteLine("OnNetworkError {0}", socketError);
        }

        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            logger.WriteLine("Received: {0}", reader.GetString());
        }

        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
            logger.WriteLine("OnNetworkReceiveUnconnected");
        }

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {
            logger.WriteLine("OnNetworkLatencyUpdate {0}", latency);
        }

        public void OnConnectionRequest(ConnectionRequest request)
        {
            request.Accept();
            logger.WriteLine("OnConnectionRequest {0}", request.Result);
        }
    }
}