using System;
using System.Collections.Generic;
using StudioForge.Engine.GamerServices;
using StudioForge.Engine.Net;

namespace TestNet
{
    class NetworkSession : INetworkSession
    {
        public event EventHandler<GamerEventArgs> GamerJoined;
        public event EventHandler<GamerEventArgs> GamerLeft;
        public event EventHandler<GameEventArgs> GameStarted;
        public event EventHandler<GameEventArgs> GameEnded;
        public event EventHandler<NetworkSessionEndedEventArgs> SessionEnded;

        public void Update()
        {
        }

        List<NetworkGamer> INetworkSession.AllGamers
        {
            get { return null; }
        }

        void INetworkSession.EndGame()
        {
        }

        NetworkGamer INetworkSession.FindGamerById(GamerID gamerId)
        {
            return null;
        }


        NetworkGamer INetworkSession.Host
        {
            get { return null; }
        }

        bool INetworkSession.IsDisposed
        {
            get { return false; }
        }

        bool INetworkSession.IsHost
        {
            get { return false; }
        }

        List<NetworkGamer> INetworkSession.LocalGamers
        {
            get { return null; }
        }

        List<NetworkGamer> INetworkSession.RemoteGamers
        {
            get { return null; }
        }

        NetworkSessionProperties INetworkSession.SessionProperties
        {
            get { return null; }
        }

        NetworkSessionState INetworkSession.SessionState
        {
            get { throw new NotImplementedException(); }
        }

        NetworkSessionType INetworkSession.SessionType
        {
            get { return NetworkSessionType.Local; }
        }

        void INetworkSession.StartGame()
        {
        }

        void INetworkSession.Update()
        {
        }

        void IDisposable.Dispose()
        {
        }
    }
}
