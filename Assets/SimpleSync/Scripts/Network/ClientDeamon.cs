using UnityEngine;
using System;
using System.Net.Sockets;
using System.Net;
using System.IO;

using SimpleSync.Protocol;

namespace SimpleSync.Network
{


    public class ClientDeamon : Connector
    {
        string m_kIP = "127.0.0.1";
        int m_iPort = 15556;

        public void ConnectToServer()
        {
            if (m_bConnected)
                return;

            if (m_bWaitConnect)
            {
                if (Time.time < m_fStartWaitConnect + 5.0f)
                    return;

                CloseSocket(1);
            }

            m_bWaitConnect = true;
            m_fStartWaitConnect = Time.time;
            Debug.LogWarning("Start Connect To " + m_kIP + ":" + m_iPort);

            try
            {
                m_kClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                m_kClientSocket.SendBufferSize = m_iBuffSize;
                m_kClientSocket.ReceiveBufferSize = m_iBuffSize;

                IPAddress ip = IPAddress.Parse(m_kIP);
                IPEndPoint kAddress = new IPEndPoint(ip, m_iPort);
                m_kClientSocket.BeginConnect(kAddress, new AsyncCallback(ConnectCallback), null);
            }
            catch (Exception e)
            {
                Debug.Log("Connection Exception msg is " + e.Message);
            }
        }

        protected override void ConnectCallback(IAsyncResult iar)
        {
            m_kClientSocket.EndConnect(iar);
            base.ConnectCallback(iar);
            StartReceive();
        }

        protected override void Reconnect()
        {
            base.Reconnect();
            ConnectToServer();
        }

    }
}
