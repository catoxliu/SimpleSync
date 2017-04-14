using System.IO;

using SimpleSync.Protocol;
using FlatBuffers;

namespace SimpleSync.Network
{
    public class MessageHelper
    {
        #region Member Data
        private static MSG_HEAD m_kCSMsgHead;
        private static byte[] m_cSendBuffer;
        private static MemoryStream m_kSendStream;
        private static BinaryWriter m_kStreamWriter;
        //private GamePBSerializer 	m_kPBSerializer;
        #endregion

        static MessageHelper()
        {
            m_kCSMsgHead = new MSG_HEAD();
            m_cSendBuffer = new byte[65536];
            m_kSendStream = new MemoryStream(m_cSendBuffer);
            m_kStreamWriter = new BinaryWriter(m_kSendStream, System.Text.Encoding.BigEndianUnicode);
        }

        public static void SendEmptyMessage(Connector sender, MessageType eMsgType)
        {
            if (sender == null)
                return;

            m_kSendStream.Position = 0;

            m_kCSMsgHead.m_usMsgType = (ushort)eMsgType;
            m_kCSMsgHead.m_usSize = (ushort)MsgHeadSize.CS_MG_HEAD_SIZE;
            m_kCSMsgHead.Encode(m_kStreamWriter);

            sender.Send(m_cSendBuffer, m_kCSMsgHead.m_usSize);
        }

        public static void SendShortMessage(Connector sendor, MessageType eMsgType, ushort data)
        {
            if (sendor == null)
                return;

            m_kSendStream.Position = 0;

            m_kCSMsgHead.m_usMsgType = (ushort)eMsgType;
            m_kCSMsgHead.m_usSize = (ushort)MsgHeadSize.CS_MG_HEAD_SIZE + 2;

            m_kCSMsgHead.Encode(m_kStreamWriter);

            m_kStreamWriter.Write(data);

            sendor.Send(m_cSendBuffer, m_kCSMsgHead.m_usSize);
        }

        public static void SendMessage(Connector sendor, MessageType eMsgType, byte[] msgBody)
        {
            if (sendor == null)
                return;

            m_kSendStream.Position = 0;

            m_kCSMsgHead.m_usMsgType = (ushort)eMsgType;
            m_kCSMsgHead.m_usSize = (ushort)MsgHeadSize.CS_MG_HEAD_SIZE;

            if (msgBody != null)
            {
                m_kStreamWriter.Write(msgBody);
                m_kCSMsgHead.m_usSize += (ushort)(m_kSendStream.Position);
                m_kSendStream.Position = 0;
            }
            m_kCSMsgHead.Encode(m_kStreamWriter);

            if (msgBody != null)
            {
                m_kStreamWriter.Write(msgBody);
            }

            sendor.Send(m_cSendBuffer, m_kCSMsgHead.m_usSize);
        }

    }

}
