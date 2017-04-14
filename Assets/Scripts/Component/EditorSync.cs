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

        public static void UpdateLogic()
        {
            m_kConnector.UpdateLogic();
        }

        public static void StartSync()
        {
            MessageHelper.SendEmptyMessage(m_kConnector, MessageType.MT_E2D_START);
        }

        public static void StopSync()
        {
            MessageHelper.SendEmptyMessage(m_kConnector, MessageType.MT_E2D_STOP_SYNC);
            SyncManager.IsSyncStart = false;
        }

        public static void SendSyncRequest(byte[] data)
        {
            MessageHelper.SendMessage(m_kConnector, MessageType.MT_E2D_SYNC_REQUEST, data);
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
                case (ushort)MessageType.MT_D2E_REQUEST_RESULT:
                    int syncCount = System.BitConverter.ToInt16(kMsgBodyStream.ToArray(), 0);
                    Debug.Log("Sync Object count : " + syncCount);
                    if (syncCount > 0)
                    {
                        MessageHelper.SendEmptyMessage(m_kConnector, MessageType.MT_E2D_START_SYNC);
                        SyncManager.IsSyncStart = true;
                    }
                    break;
                case (ushort)MessageType.MT_D2E_SYNC_UPDATE:
                    byte[] data = kMsgBodyStream.ToArray();
                    FBHelper.DeserializeSyncObject(data);
                    break;
                default:
                    Debug.LogError("Wrong Message Type : " + msgHead.m_usMsgType);
                    break;
            }
        }

    }

}
