using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using LiteNetLib;

namespace Playground.LiteNetLib
{
    public class LiteNetLibServer : INetEventListener
    {
        private readonly NetManager server;
        private readonly TextWriter logger;

        public LiteNetLibServer(TextWriter logger)
        {
            this.logger = logger;

            server = new NetManager(this);
            server.AutoRecycle = true;
            server.UpdateTime = 1;
            server.SimulatePacketLoss = false;
            server.SimulationPacketLossChance = 20;
            server.Start(9050);
        }

        public void PollEvents()
        {
            server.PollEvents();
        }

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
