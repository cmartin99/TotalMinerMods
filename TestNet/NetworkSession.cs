using System;
using System.Collections.Generic;
using StudioForge.Engine.GamerServices;
using StudioForge.Engine.Net;
using StudioForge.TotalMiner;

namespace TestNet
{
    class NetworkSession : INetworkSession
    {
        public event EventHandler<GamerEventArgs> GamerJoined;
        public event EventHandler<GamerEventArgs> GamerLeft;
        public event EventHandler<GameEventArgs> GameStarted;
        public event EventHandler<GameEventArgs> GameEnded;
        public event EventHandler<NetworkSessionEndedEventArgs> SessionEnded;

        List<NetworkGamer> localGamers = new List<NetworkGamer>();
        List<NetworkGamer> remoteGamers = new List<NetworkGamer>();
        List<NetworkGamer> allGamers = new List<NetworkGamer>();
        SessionProperties properties;
        bool isDisposed;

        public NetworkSession(SessionProperties properties)
        {
            this.properties = properties;
        }

        public void Update()
        {
        }

        List<NetworkGamer> INetworkSession.AllGamers
        {
            get { return allGamers; }
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
            get { return isDisposed; }
        }

        bool INetworkSession.IsHost
        {
            get { return false; }
        }

        List<NetworkGamer> INetworkSession.LocalGamers
        {
            get { return localGamers; }
        }

        List<NetworkGamer> INetworkSession.RemoteGamers
        {
            get { return remoteGamers; }
        }

        object INetworkSession.SessionProperties
        {
            get { return properties; }
        }

        NetworkSessionState INetworkSession.SessionState
        {
            get { return NetworkSessionState.Playing; }
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
            isDisposed = true;
        }
    }
}
