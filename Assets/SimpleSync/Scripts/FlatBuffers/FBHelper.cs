using FlatBuffers;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

namespace SimpleSync.Serializer
{
    public class FBHelper
    {

        public static byte[] SerializeSyncIDList(List<string> syncObjects)
        {
            var builder = new FlatBufferBuilder(1);

            int length = syncObjects.Count;

            StringOffset[] names = new StringOffset[length];

            for (int index = 0; index < length; index++)
            {
                names[index] = builder.CreateString(syncObjects[index]);
            }

            Offset<SyncObjectID>[] off = new Offset<SyncObjectID>[length];
            for (int index = 0; index < length; index++)
            {
                SyncObjectID.StartSyncObjectID(builder);
                SyncObjectID.AddId(builder, (ushort)index);
                SyncObjectID.AddName(builder, names[index]);
                off[index] = SyncObjectID.EndSyncObjectID(builder);
            }
            
            var ids = SyncIDList.CreateListVector(builder, off);
            

            SyncIDList.StartSyncIDList(builder);
            SyncIDList.AddLength(builder, (ushort)length);
            SyncIDList.AddList(builder, ids);
            var list = SyncIDList.EndSyncIDList(builder);

            SyncIDList.FinishSyncIDListBuffer(builder, list);
            using (var ms = new MemoryStream(builder.DataBuffer.Data, builder.DataBuffer.Position, builder.Offset))
            {
                var data = ms.ToArray();
                return data;
            }
        }

        public static Dictionary<int, string> DeserializeSyncIDList(byte[] data)
        {
            ByteBuffer b = new ByteBuffer(data);
            var list = SyncIDList.GetRootAsSyncIDList(b);
            int length = list.Length;
            Debug.Log(list.ListLength);
            Dictionary<int, string> results = new Dictionary<int, string>();
            if (length != list.ListLength) return results;
            for (int index = 0; index < length; index++)
            {
                if (list.List(index).HasValue)
                {
                    SyncObjectID soid = list.List(index).Value;
                    results.Add((int)soid.Id, soid.Name);
                }
            }
            return results;
        }


        public static byte[] SerializeSyncObject(Dictionary<int, GameObject> dic)
        {
            var builder = new FlatBufferBuilder(1);

            int length = dic.Count;

            Offset<SyncObject>[] off = new Offset<SyncObject>[length];

            int index = 0;
            foreach (var item in dic)
            {
                Transform transform = item.Value.transform;
                SyncObject.StartSyncObject(builder);
                SyncObject.AddId(builder, (ushort)item.Key);
                SyncObject.AddPos(builder, Vec3.CreateVec3(builder, transform.position.x, transform.position.y, transform.position.z));
                SyncObject.AddRot(builder, Vec4.CreateVec4(builder, transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w));
                SyncObject.AddScale(builder, Vec3.CreateVec3(builder, transform.localScale.x, transform.localScale.y, transform.localScale.z));
                off[index] = SyncObject.EndSyncObject(builder);
                index++;
            }

            var objs = ObjectList.CreateListVector(builder, off);

            ObjectList.StartObjectList(builder);
            ObjectList.AddLength(builder, (ushort)length);
            ObjectList.AddList(builder, objs);
            var list = ObjectList.EndObjectList(builder);

            ObjectList.FinishObjectListBuffer(builder, list);

            using (var ms = new MemoryStream(builder.DataBuffer.Data, builder.DataBuffer.Position, builder.Offset))
            {
                var data = ms.ToArray();
                return data;
            }
        }

        public static void DeserializeSyncObject(byte[] data, Dictionary<int, GameObject> dic)
        {
            ByteBuffer b = new ByteBuffer(data);
            var list = ObjectList.GetRootAsObjectList(b);
            //Debug.Log(go.Pos.HasValue);
            int length = list.Length;
            for (int index = 0; index < length; index++)
            {
                if (list.List(index).HasValue)
                {
                    SyncObject so = list.List(index).Value;
                    if (dic.ContainsKey(so.Id))
                    {
                        GameObject go = dic[so.Id];
                        //Debug.Log("Update GameObject : " + go.name);
                        go.transform.position = new Vector3(so.Pos.Value.X, so.Pos.Value.Y, so.Pos.Value.Z);
                        go.transform.localScale = new Vector3(so.Scale.Value.X, so.Scale.Value.Y, so.Scale.Value.Z);
                        go.transform.rotation = new Quaternion(so.Rot.Value.X, so.Rot.Value.Y, so.Rot.Value.Z, so.Rot.Value.W);
                    }
                }
            }
        }
    }

}