using UnityEngine;
using System;
using System.Net.Sockets;
using System.IO;

using SimpleSync.Protocol;

namespace SimpleSync.Network
{
    public class Connector
    {
        public bool m_bConnected;
        public bool m_bWaitConnect = false;
        public float m_fStartWaitConnect;
        public int m_iDisconnectFlag = 0;   // 0 unkown 1 Connect Error 2 Connect TimeOut
        public delegate void m_dMessageHandler(MSG_HEAD msgHead, MemoryStream kMsgBodyStream, int iMsgBodySize);

        protected static int m_iBuffSize = 65535;
        protected bool m_bReceiveFlag = true;
        protected ReceiveBuffer m_kRecvBuffer;
        protected Socket m_kClientSocket = null;
        protected m_dMessageHandler m_kHandler = null;

        private byte[] m_cReadData = new byte[m_iBuffSize];

        public void Init()
        {
            m_kRecvBuffer = new ReceiveBuffer();
            m_kRecvBuffer.Init(m_iBuffSize * 5);
            m_bConnected = false;
            m_bWaitConnect = false;
            m_fStartWaitConnect = 0.0f;
        }

        protected virtual void ConnectCallback(IAsyncResult iar)
        {
            m_bConnected = true;
            m_bWaitConnect = false;
            m_bReceiveFlag = true;
            m_iDisconnectFlag = 0;
        }

        public void Send(byte[] bytes, Int32 length)
        {
            if (!m_bConnected)
                return;

            int iSendSize = 0;

            try
            {
                iSendSize = m_kClientSocket.Send(bytes, length, SocketFlags.None);
            }
            catch (Exception e)
            {
                Debug.LogError("Socket Error,Disconnect with msg : " + e.ToString());
                CloseSocket(1);
            }

            if (iSendSize <= 0)
            {
                Debug.Log("Socket Can't Send,Disconnect");
                CloseSocket(1);
            }
        }

        public void UpdateLogic()
        {
            if (!m_bConnected)
            {
                Reconnect();
                return;
            }
                
            StartReceive();

            ProcessRecvedMessage();
        }

        protected virtual void Reconnect()
        {
            SyncManager.IsD2ESyncStart = false;
            SyncManager.IsE2DSyncStart = false;
        }

        protected void StartReceive()
        {
            try
            {
                if (m_bReceiveFlag)
                {
                    m_bReceiveFlag = false;
                    m_kClientSocket.BeginReceive(m_cReadData, 0, m_cReadData.Length, SocketFlags.None, new AsyncCallback(EndReceive), m_kClientSocket);
                }
            }
            catch (Exception e)
            {
                Debug.Log("Socket Error,Disconnect with msg: " + e.ToString());
                CloseSocket(1);
            }
        }

        void EndReceive(IAsyncResult iar)
        {
            try
            {
                m_bReceiveFlag = true;
                Socket remote = (Socket)iar.AsyncState;
                int recv = remote.EndReceive(iar);
                if (recv > 0)
                {
                    m_kRecvBuffer.AppendData(ref m_cReadData, recv);
                }

                if (recv == 0)
                {
                    Debug.Log("Socket Error recv = 0,Disconnect");
                    CloseSocket(1);
                }
            }
            catch (Exception e)
            {
                Debug.Log("Socket Error recv exception,Disconnect " + e.ToString());
                CloseSocket(1);
            }
        }

        protected virtual void ProcessRecvedMessage()
        {
            byte[] headData = new byte[(int)MsgHeadSize.CS_MG_HEAD_SIZE];
            MemoryStream kHeadReadStream = new MemoryStream(headData);
            BinaryReader m_kStreamReader = new BinaryReader(kHeadReadStream, System.Text.Encoding.BigEndianUnicode);

            MSG_HEAD msgHead = new MSG_HEAD();

            while (m_kRecvBuffer.GetBuffValidSize() >= (int)MsgHeadSize.CS_MG_HEAD_SIZE)
            {
                m_kRecvBuffer.GetData(ref headData, (int)MsgHeadSize.CS_MG_HEAD_SIZE);
                kHeadReadStream.Position = 0;
                msgHead.Decode(m_kStreamReader);

                if (msgHead.m_usSize > m_kRecvBuffer.GetBuffValidSize())
                {
                    break;
                }

                if (!m_kRecvBuffer.PopData((int)MsgHeadSize.CS_MG_HEAD_SIZE))
                    break;

                int iMsgBodySize = msgHead.m_usSize - (int)MsgHeadSize.CS_MG_HEAD_SIZE;

                MemoryStream kMsgBodyStream = null;

                if (iMsgBodySize > 0)
                {
                    byte[] bodyBuff = new byte[iMsgBodySize];
                    bool bResult = m_kRecvBuffer.PopData(ref bodyBuff, iMsgBodySize);
                    if (bResult == false)
                        break;
                    kMsgBodyStream = new MemoryStream(bodyBuff);
                }

                //Debug.Log("Receive Message with type " + msgHead.m_usMsgType);

                if (m_kHandler != null)
                {
                    m_kHandler(msgHead, kMsgBodyStream, iMsgBodySize);
                }
            }

            m_kRecvBuffer.Regroup();
        }

        public void RegisterMessageHanlder(m_dMessageHandler handler)
        {
            m_kHandler = handler;
        }

        public void RemoveMessageHanlder()
        {
            m_kHandler = null;
        }

        public virtual void CloseSocket(int iDisconnectFlag)
        {
            if (!m_bConnected)
                return;

            if (m_kClientSocket != null && m_kClientSocket.Connected)
            {
                m_kClientSocket.Close();
            }

            m_bConnected = false;
            m_bWaitConnect = false;
            m_bReceiveFlag = true;

            if (iDisconnectFlag != 0)
                m_iDisconnectFlag = iDisconnectFlag;
        }
    }
}
