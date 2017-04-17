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
            if (SyncManager.IsD2ESyncStart)
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
            SyncManager.IsD2ESyncStart = false;
            m_iActiveSceneID = SceneManager.GetActiveScene().buildIndex;
            MessageHelper.SendShortMessage(m_kConnector, update ? MessageType.MT_D2E_SCENE_CHANGED : MessageType.MT_D2E_SCENE_ID, (ushort)m_iActiveSceneID);
        }

        public static void SendSyncUpdate()
        {
            MessageHelper.SendMessage(m_kConnector, MessageType.MT_D2E_SYNC_UPDATE, SyncManager.GetD2ESyncData());
        }

        static void HandleReceiveMessage(MSG_HEAD msgHead, MemoryStream kMsgBodyStream, int iMsgBodySize)
        {
            switch (msgHead.m_usMsgType)
            {
                case (ushort)MessageType.MT_E2D_START:
                    SyncManager.SyncFrameRate = System.BitConverter.ToInt16(kMsgBodyStream.ToArray(), 0);
                    SendSceneID();
                    break;
                case (ushort)MessageType.MT_E2D_SYNC_REQUEST:
                    SyncManager.FindD2ESyncObjects(FBHelper.DeserializeSyncIDList(kMsgBodyStream.ToArray()));
                    MessageHelper.SendShortMessage(m_kConnector, MessageType.MT_D2E_SYNC_RESPOND, (ushort)SyncManager.D2ESyncObjectsCount);
                    break;
                case (ushort)MessageType.MT_E2D_START_SYNC:
                    SyncManager.IsD2ESyncStart = true;
                    break;
                case (ushort)MessageType.MT_E2D_STOP_SYNC:
                    SyncManager.IsD2ESyncStart = false;
                    break;
                case (ushort)MessageType.MT_E2D_CONTROL_REQUEST:
                    SyncManager.FindE2DControlObjects(FBHelper.DeserializeSyncIDList(kMsgBodyStream.ToArray()));
                    MessageHelper.SendShortMessage(m_kConnector, MessageType.MT_D2E_CONTROL_RESPOND, (ushort)SyncManager.E2DControlObjectsCount);
                    break;
                case (ushort)MessageType.MT_E2D_CONTROL_UPDATE:
                    SyncManager.E2DControlUpdate(kMsgBodyStream.ToArray());
                    break;
                case (ushort)MessageType.MT_E2D_STOP_CONTROL:
                    break;
                case (ushort)MessageType.MT_E2D_UPDATE_SYNC_RATE:
                    SyncManager.SyncFrameRate = System.BitConverter.ToInt16(kMsgBodyStream.ToArray(), 0);
                    break;
                default:
                    Debug.LogError("Wrong Message Type : " + msgHead.m_usMsgType);
                    break;
            }
        }

    }

}
