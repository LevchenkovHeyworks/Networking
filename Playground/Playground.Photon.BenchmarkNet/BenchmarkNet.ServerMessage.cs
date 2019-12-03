using System;

namespace NX
{
    [Serializable]
    public struct ServerMessage
    {
        public bool uninitialized;
        public int reliableSent;
        public int reliableReceived;
        public int reliableBytesSent;
        public int reliableBytesReceived;
        public int unreliableSent;
        public int unreliableReceived;
        public int unreliableBytesSent;
        public int unreliableBytesReceived;
    }
}