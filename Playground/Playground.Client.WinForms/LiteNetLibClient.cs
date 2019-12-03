using System;
using System.Net;
using System.Net.Sockets;
using LiteNetLib;

namespace Playground.Client.WinForms
{
    public class LiteNetLibClient : INetEventListener
    {
        private readonly NetManager client;
        private NetPeer peer;

        public event EventHadler MessageReceived;
        public event EmptyEventHadler Connected;
        public event EmptyEventHadler Disconected;

        public LiteNetLibClient()
        {
            client = new NetManager(this);
            client.UnsyncedEvents = true;
            client.AutoRecycle = true;
            client.Start();
        }

        public void Connect()
        {
            peer = client.Connect("localhost", 9050, "qwe");
        }

        public void PollEvents()
        {
            client.PollEvents();
        }

        public void OnPeerConnected(NetPeer peer)
        {
            Connected?.Invoke(this, EventArgs.Empty);
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Disconected?.Invoke(this, EventArgs.Empty);
        }

        public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
        }

        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            var message = reader.GetString();
            MessageReceived?.Invoke(this, message);
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

    public delegate void EventHadler(object sender, string message);
    public delegate void EmptyEventHadler(object sender, EventArgs eventArgs);
}
