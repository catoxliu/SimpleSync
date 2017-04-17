using UnityEngine;
using System.IO;

using SimpleSync.Network;
using SimpleSync.Protocol;
using SimpleSync.Serializer;

namespace SimpleSync
{

    public class EditorSync
    {

        static ClientDeamon m_kConnector = new ClientDeamon();

        public static void Init()
        {
            m_kConnector.Init();
            m_kConnector.ConnectToServer();
            m_kConnector.RegisterMessageHanlder(HandleReceiveMessage);
        }

        public static bool IsConnected
        {
            get { return m_kConnector == null ? false : m_kConnector.m_bConnected; }
        }

        public static void UpdateLogic()
        {
            m_kConnector.UpdateLogic();
            if (SyncManager.IsE2DSyncStart)
            {
                SendControlUpdate();
            }
        }

        public static void SendControlUpdate()
        {
            MessageHelper.SendMessage(m_kConnector, MessageType.MT_E2D_CONTROL_UPDATE, SyncManager.GetE2DControlData());
        }

        public static void UpdateSyncFrameRate()
        {
            MessageHelper.SendShortMessage(m_kConnector, MessageType.MT_E2D_UPDATE_SYNC_RATE, (ushort)SyncManager.SyncFrameRate);
        }

        public static void StartSync()
        {
            MessageHelper.SendShortMessage(m_kConnector, MessageType.MT_E2D_START, (ushort)SyncManager.SyncFrameRate);
        }

        public static void StopSync()
        {
            MessageHelper.SendEmptyMessage(m_kConnector, MessageType.MT_E2D_STOP_SYNC);
            SyncManager.IsD2ESyncStart = false;
        }

        public static void StopControl()
        {
            MessageHelper.SendEmptyMessage(m_kConnector, MessageType.MT_E2D_STOP_CONTROL);
            SyncManager.IsE2DSyncStart = false;
        }

        public static void SendSyncRequest(byte[] data)
        {
            MessageHelper.SendMessage(m_kConnector, MessageType.MT_E2D_SYNC_REQUEST, data);
        }

        public static void SendControlRequest(byte[] data)
        {
            MessageHelper.SendMessage(m_kConnector, MessageType.MT_E2D_CONTROL_REQUEST, data);
        }

        static void HandleReceiveMessage(MSG_HEAD msgHead, MemoryStream kMsgBodyStream, int iMsgBodySize)
        {
            switch (msgHead.m_usMsgType)
            {
                case (ushort)MessageType.MT_D2E_SCENE_ID:
                case (ushort)MessageType.MT_D2E_SCENE_CHANGED:
                    int sceneid = System.BitConverter.ToInt16(kMsgBodyStream.ToArray(), 0);
                    Debug.Log("Scene ID : " + sceneid);
                    SyncManager.SyncSceneID = sceneid;
                    break;
                case (ushort)MessageType.MT_D2E_SYNC_RESPOND:
                    int syncCount = System.BitConverter.ToInt16(kMsgBodyStream.ToArray(), 0);
                    Debug.Log("Sync Object count : " + syncCount);
                    if (syncCount > 0)
                    {
                        MessageHelper.SendEmptyMessage(m_kConnector, MessageType.MT_E2D_START_SYNC);
                        SyncManager.IsD2ESyncStart = true;
                    }
                    break;
                case (ushort)MessageType.MT_D2E_SYNC_UPDATE:
                    SyncManager.D2ESyncUpdate(kMsgBodyStream.ToArray());
                    break;
                case (ushort)MessageType.MT_D2E_CONTROL_RESPOND:
                    int controlCount = System.BitConverter.ToInt16(kMsgBodyStream.ToArray(), 0);
                    Debug.Log("Contorl Object count : " + controlCount);
                    SyncManager.IsE2DSyncStart = controlCount > 0;
                    break;
                default:
                    Debug.LogError("Wrong Message Type : " + msgHead.m_usMsgType);
                    break;
            }
        }

    }

}
