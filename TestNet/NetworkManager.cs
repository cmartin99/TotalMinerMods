using System.Collections.Generic;
using StudioForge.Engine.GamerServices;
using StudioForge.Engine.Net;
using StudioForge.TotalMiner.API;

namespace TestNet
{
    class NetworkManager : ITMNetworkManager
    {
        NetworkSession session;
        List<NetworkGamer> gamers;

        public INetworkSession Session
        {
            get { return session; }
        }

        public NetworkManager()
        {
            gamers = new List<NetworkGamer>();
        }

        public void Initialize(int exeVersion)
        {
        }

        public INetworkSession CreateSession(NetworkSessionType type, Gamer host)
        {
            gamers.Clear();
            session = new NetworkSession();
            return session;
        }

        public void EndSession(INetworkSession session)
        {
            gamers.Clear();
            session = null;
        }

        public List<IAvailableNetworkSession> FindSessions()
        {
            var sess = new AvailableNetworkSession(NetworkSessionType.PlayerMatch, "Hello");
            sess.SessionProperties[0] = 0;
            sess.QualityOfService = new QualityOfService();
            var result = new List<IAvailableNetworkSession>();
            result.Add(sess);
            return result;
        }

        public List<NetworkGamer> Gamers
        {
            get { return gamers; }
        }

        public void ReadData(PacketReader data, NetworkGamer sender, NetworkGamer recipient)
        {
        }

        public void SendData(PacketWriter data, SendDataOptions options, NetworkGamer recipient)
        {
        }

        void ITMNetworkManager.EndSession()
        {
        }

        INetworkSession ITMNetworkManager.JoinSession(IAvailableNetworkSession session, Gamer joiner)
        {
            return null;
        }

        bool ITMNetworkManager.ReadData(PacketReader data, out NetworkGamer sender)
        {
            sender = null;
            return false;
        }

        void ITMNetworkManager.SendData(PacketWriter data, SendDataOptions options, NetworkGamer recipient)
        {
        }

        bool ITMNetworkManager.ParseCustomPacket(PacketReader data, NetworkGamer sender)
        {
            return false;
        }
    }
}
