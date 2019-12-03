using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using LiteNetLib;
using LiteNetLib.Utils;

namespace Playground.Server.WinForms
{
    public class LiteNetLibServer : INetEventListener
    {
        private readonly NetManager server;
        private readonly NetDataWriter writer;

        private IList<NetPeer> peers;

        public LiteNetLibServer()
        {
            server = new NetManager(this);
            server.AutoRecycle = true;
            server.UpdateTime = 1;
            server.Start(9050);

            writer = new NetDataWriter();
            peers = new List<NetPeer>();
        }

        public void PollEvents()
        {
            server.PollEvents();
        }

        public int GetClientsCount()
        {
            return peers.Count;
        }

        public void SendReliable(string message)
        {
            writer.Reset();
            writer.Put(message);

            foreach (var peer in peers)
            {
                peer.Send(writer, DeliveryMethod.ReliableOrdered);
            }
        }

        public void SendUnreliable(string message)
        {
            writer.Reset();
            writer.Put(message);

            foreach (var peer in peers)
            {
                peer.Send(writer, DeliveryMethod.Unreliable);
            }
        }

        public void OnPeerConnected(NetPeer peer)
        {
            peers.Add(peer);
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            var existing = peers.FirstOrDefault(x => x.Id == peer.Id);

            if (existing != null)
            {
                peers.Remove(existing);
            }
        }

        public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
        }

        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
        }

        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
        }

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {
        }

        public void OnConnectionRequest(ConnectionRequest request)
        {
            request.Accept();
        }
    }
}
