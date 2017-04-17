using UnityEngine;
using System;
using System.Net.Sockets;
using System.Net;
using System.IO;

using SimpleSync.Protocol;

namespace SimpleSync.Network
{
    public class ServerDeamon : Connector
    {

        private static Socket m_kServerSocket;
        string m_kIP = "127.0.0.1";
        int m_iPort = 15555;

        public void StartServer()
        {
            m_kServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(m_kIP), 15555);
            try
            {
                m_kServerSocket.Bind(ipe);
                m_kServerSocket.Listen(1000);
                m_kServerSocket.BeginAccept(new AsyncCallback(ConnectCallback), m_kServerSocket);
                Debug.Log("Bind Success IP: " + m_kIP + " Port : " + m_iPort);
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }

        protected override void ConnectCallback(IAsyncResult iar)
        {
            Socket server = (Socket)iar.AsyncState;
            m_kClientSocket = server.EndAccept(iar);
            base.ConnectCallback(iar);
        }

        protected override void Reconnect()
        {
            base.Reconnect();
            if (!m_bWaitConnect)
            {
                m_bWaitConnect = true;
                try
                {
                    m_kServerSocket.Listen(1000);
                    m_kServerSocket.BeginAccept(new AsyncCallback(ConnectCallback), m_kServerSocket);
                    Debug.Log("Bind Success IP: " + m_kIP + " Port : " + m_iPort);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                }
            }
        }

    }
}
