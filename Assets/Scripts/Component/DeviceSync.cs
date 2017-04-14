using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

using SimpleSync.Network;
using SimpleSync.Protocol;
using SimpleSync.Serializer;

namespace SimpleSync
{
    public class DeviceSync
    {
        static ServerDeamon m_kConnector = new ServerDeamon();
        static int m_iActiveSceneID = -1;

        public static void Init()
        {
            m_kConnector.Init();
            m_kConnector.StartServer();
            m_kConnector.RegisterMessageHanlder(HandleReceiveMessage);
        }

        public static void UpdateLogic()
        {
            m_kConnector.UpdateLogic();
            if (SyncManager.IsSyncStart)
            {
                if (SceneManager.GetActiveScene().buildIndex != m_iActiveSceneID)
                {
                    SendSceneID(true);
                }
                else
                {
                    SendSyncUpdate();
                }
            }
        }

        public static void SendSceneID(bool update = false)
        {
            SyncManager.IsSyncStart = false;
            m_iActiveSceneID = SceneManager.GetActiveScene().buildIndex;
            MessageHelper.SendShortMessage(m_kConnector, update ? MessageType.MT_D2E_SCENE_CHANGED : MessageType.MT_D2E_SCENE_ID, (ushort)m_iActiveSceneID);
        }

        public static void SendSyncUpdate()
        {
            MessageHelper.SendMessage(m_kConnector, MessageType.MT_D2E_SYNC_UPDATE, SyncManager.GetSyncData());
        }

        static void HandleReceiveMessage(MSG_HEAD msgHead, MemoryStream kMsgBodyStream, int iMsgBodySize)
        {
            switch (msgHead.m_usMsgType)
            {
                case (ushort)MessageType.MT_E2D_START:
                    SendSceneID();
                    break;
                case (ushort)MessageType.MT_E2D_SYNC_REQUEST:
                    byte[] data = kMsgBodyStream.ToArray();
                    SyncManager.FindSyncObjects(FBHelper.DeserializeSyncIDList(data));
                    MessageHelper.SendShortMessage(m_kConnector, MessageType.MT_D2E_REQUEST_RESULT, (ushort)SyncManager.SyncObjectsCount);
                    break;
                case (ushort)MessageType.MT_E2D_START_SYNC:
                    SyncManager.IsSyncStart = true;
                    break;
                case (ushort)MessageType.MT_E2D_STOP_SYNC:
                    SyncManager.IsSyncStart = false;
                    break;
                default:
                    Debug.LogError("Wrong Message Type : " + msgHead.m_usMsgType);
                    break;
            }
        }

    }

}
