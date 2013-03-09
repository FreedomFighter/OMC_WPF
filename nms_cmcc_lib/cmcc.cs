using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CmccLib
{
    // 中国移动直放站设备网管接口技术规范
    // 文档版本：Version 1.0.0
    // 发布日期：2005年07月

    public class Cmcc
    {
        public class CmccConst
        {
            public class ApId
            {
                public const byte A = 0x01;
                public const byte B = 0x02;
                public const byte C = 0x03;
            }

            public class NpId
            {
                public const byte A = 0x01;
            }

            public class MapId
            {
                public const byte A = 0x01;
                public const byte B = 0x02;
                public const byte C = 0x03;
            }

            public class ApFlag
            {
                public const byte A = 0x7E;
                public const byte B = 0x21;
            }

            public class PacketId
            {
                public const ushort Min = 0x0000;
                public const ushort Max = 0x7FFF;
            }

            public class NpFlag
            {
                public const byte Succeed = 0x00;
                public const byte Busy = 0x01;
                public const byte Command = 0x80;
            }

            public class Command
            {
                public const byte None = 0x00;
                public const byte Report = 0x01;
                public const byte Query = 0x02;
                public const byte Setting = 0x03;
            }

            public class AckFlag
            {
                public const byte Succeed = 0x00;
                public const byte ErrorPartial = 0x01;
                public const byte ErrorCommand = 0x02;
                public const byte ErrorLength = 0x03;
                public const byte ErrorCrc = 0x04;
                public const byte ErrorOthers = 0xFE;
                public const byte Command = 0xFF;
            }

            public class moErrorCode
            {
                public const byte Succeed = 0x00;  
                public const byte ErrorMoid = 0x01; 
                public const byte OutofRange = 0x02; 
                public const byte InvalidData = 0x03; 
                public const byte ErrorLength = 0x04; 
                public const byte LowLimit = 0x05; 
                public const byte HighLimit = 0x06; 
                public const byte ErrorOthers = 0x09; 
                public const byte ErrorSystem = 0x0A; 
            }
        }

        public enum CmccUnpacketResult
        {
            Succeed, CrcError, OtherError
        }

        public class MonitorObject
        {
            private byte error;     // error code,监控对象ID里面的最高4位为错误码
            private ushort moid;    // monitor object id， 监控对象ID里面的最低12位为监控标识
            public byte[] Data;     // store value for all kinds of datatype

            /// <summary>
            /// 数据长度
            /// </summary>
            public byte Length
            {
                get { return (Data != null) ? (byte)Data.Length : (byte)0; }
            }
            /// <summary>
            /// 监控对象标识
            /// </summary>
            public ushort Moid
            {
                get { return (ushort)((error << 12) + (moid << 0)); }
                set
                {
                    error = (byte)((value >> 12) & 0x000F);
                    moid = (ushort)((value >> 0) & 0x0FFF);
                }
            }
            /// <summary>
            /// 错误码
            /// </summary>
            public byte Error
            {
                get { return error; }
                set { error = value; }
            }
        }

        /// <summary>
        /// MAP层类定义
        /// </summary>
        public class MapLayer
        {
            public byte mapId;
            public byte commandId;
            public byte ackFlag;
            public MonitorObject[] objects;
        }

        /// <summary>
        /// NP层类定义
        /// </summary>
        public class NPLayer
        {
            public byte npId;
            public uint siteId;    // for NP.A
            public byte subId;
            public ushort packetId;
            public byte npFlag;
        }
        /// <summary>
        /// AP层类定义
        /// </summary>
        public class APLayer
        {
            public byte apId;
        }

        public class CmccModule
        {
            /// <summary>
            /// 获得AP层协议数据包的最大长度
            /// </summary>
            /// <param name="apId"></param>
            /// <param name="npId"></param>
            /// <returns></returns>
            public static int GetMaxMapLayerBytes(byte apId, byte npId)
            {
                const ushort APA_MAX_LENGTH = 256; /* begin/end flag excluded, and before converting char */
                const ushort APB_MAX_LENGTH = 140; /* begin/end flag included, and after splitting byte */

                int npMaxBytes = 0;
                int mapMaxBytes = 0;

                if ((apId == CmccConst.ApId.A) || (apId == CmccConst.ApId.C))
                {
                    npMaxBytes = APA_MAX_LENGTH - 4;
                }
                else if (apId == CmccConst.ApId.B)
                {
                    npMaxBytes = (APB_MAX_LENGTH - 2) / 2 - 4;
                }
                else
                {
                    return 0;
                }

                if (npId == CmccConst.NpId.A)
                {
                    mapMaxBytes = npMaxBytes - 9;
                }
                else
                {
                    return 0;
                }
                  
                return (mapMaxBytes > 0) ? mapMaxBytes : 0;
            }

            /// <summary>
            /// 转义
            /// </summary>
            /// <param name="list"></param>
            /// <param name="data"></param>
            private static void AddByteWithConverting(List<byte> list, byte data)
            {
                switch (data)
                {
                    case 0x5E:
                        list.Add(0x5E);
                        list.Add(0x5D);
                        break;

                    case 0x7E:
                        list.Add(0x5E);
                        list.Add(0x7D);
                        break;

                    default:
                        list.Add(data);
                        break;
                }
            }

            /// <summary>
            /// 拆分
            /// </summary>
            /// <param name="list"></param>
            /// <param name="data"></param>
            private static void AddByteWithSpliting(List<byte> list, byte data)
            {
                char hiChar, loChar;

                ProtocolBase.SplitByte(data, out hiChar, out loChar);

                list.Add((byte)hiChar);
                list.Add((byte)loChar);
            }

            /// <summary>
            ///  打包
            /// </summary>
            /// <param name="mapLayer"></param>
            /// <returns></returns>
            private static List<byte> PacketMapLayer(MapLayer mapLayer)
            {
                List<byte> mapData = new List<byte>();

                mapData.Add(mapLayer.commandId);
                mapData.Add(mapLayer.ackFlag);

                switch (mapLayer.mapId)
                {
                    case CmccConst.MapId.A:
                    case CmccConst.MapId.B:
                    case CmccConst.MapId.C:
                        if (mapLayer.objects != null)
                        {
                            byte[] bytes = null;

                            foreach (MonitorObject mo in mapLayer.objects)
                            {
                                mapData.Add((byte)(mo.Length + 3));
                                bytes = BitConverter.GetBytes(mo.Moid);
                               
                                foreach (byte element in bytes)
                                {
                                    mapData.Add(element);
                                }

                                if (mo.Data != null)
                                {
                                    foreach (byte element in mo.Data)
                                    {
                                        mapData.Add(element);
                                    }
                                }
                            }
                        }
                        break;

                    default:
                        break;
                }

                return mapData;
            }

            /// <summary>
            /// 打包NP.A
            /// </summary>
            /// <param name="npLayer"></param>
            /// <param name="mapId"></param>
            /// <param name="mapData"></param>
            /// <returns></returns>
            private static List<byte> PacketNPLayer(NPLayer npLayer, byte mapId, List<byte> mapData)
            {
                List<byte> npData = new List<byte>();

                byte[] bytes = null;

                bytes = BitConverter.GetBytes(npLayer.siteId);
                foreach (byte element in bytes)
                {
                    npData.Add(element);
                }

                npData.Add(npLayer.subId);

                bytes = BitConverter.GetBytes(npLayer.packetId);
                foreach (byte element in bytes)
                {
                    npData.Add(element);
                }

                npData.Add(npLayer.npFlag);
                npData.Add(mapId);

                foreach (byte element in mapData)
                {
                    npData.Add(element);
                }

                return npData;
            }

            /// <summary>
            /// 打包AP.A
            /// </summary>
            /// <param name="npId"></param>
            /// <param name="npData"></param>
            /// <returns></returns>
            private static List<byte> PacketAPA(byte npId, List<byte> npData)
            {
                List<byte> apData = new List<byte>();

                ushort crc = 0;

                crc = ProtocolBase.CRC16(CmccConst.ApId.A, crc);
                crc = ProtocolBase.CRC16(npId, crc);
                crc = ProtocolBase.CRC16(npData.ToArray(), crc);

                apData.Add(CmccConst.ApFlag.A);

                AddByteWithConverting(apData, CmccConst.ApId.A);
                AddByteWithConverting(apData, (byte)npId);

                foreach (byte element in npData)
                {
                    AddByteWithConverting(apData, element);
                }

                byte[] bytes = BitConverter.GetBytes(crc);

                foreach (byte element in bytes)
                {
                    AddByteWithConverting(apData, element);
                }

                apData.Add(CmccConst.ApFlag.A);

                return apData;
            }
            /// <summary>
            /// 打包AP.C
            /// </summary>
            /// <param name="npId"></param>
            /// <param name="npData"></param>
            /// <returns></returns>
            private static List<byte> PacketAPC(byte npId, List<byte> npData)
            {
                List<byte> apData = new List<byte>();

                ushort crc = 0;

                crc = ProtocolBase.CRC16(CmccConst.ApId.C, crc);
                crc = ProtocolBase.CRC16(npId, crc);
                crc = ProtocolBase.CRC16(npData.ToArray(), crc);

                apData.Add(CmccConst.ApFlag.A);

                AddByteWithConverting(apData, CmccConst.ApId.C);
                AddByteWithConverting(apData, (byte)npId);

                foreach (byte element in npData)
                {
                    AddByteWithConverting(apData, element);
                }

                byte[] bytes = BitConverter.GetBytes(crc);
                foreach (byte element in bytes)
                {
                    AddByteWithConverting(apData, element);
                }

                apData.Add(CmccConst.ApFlag.A);

                return apData;
            }

            /// <summary>
            /// 打包AP.B
            /// </summary>
            /// <param name="npId"></param>
            /// <param name="npData"></param>
            /// <returns></returns>
            private static List<byte> PacketAPB(byte npId, List<byte> npData)
            {
                List<byte> apData = new List<byte>();

                ushort crc = 0;

                crc = ProtocolBase.CRC16(CmccConst.ApId.B, crc);
                crc = ProtocolBase.CRC16(npId, crc);
                crc = ProtocolBase.CRC16(npData.ToArray(), crc);

                apData.Add(CmccConst.ApFlag.B);

                AddByteWithSpliting(apData, CmccConst.ApId.B);
                AddByteWithSpliting(apData, (byte)npId);

                foreach (byte element in npData)
                {
                    AddByteWithSpliting(apData, element);
                }

                byte[] bytes = BitConverter.GetBytes(crc);

                foreach (byte element in bytes)
                {
                    AddByteWithSpliting(apData, element);
                }

                apData.Add(CmccConst.ApFlag.B);

                return apData;
            }

            /// <summary>
            /// 提供给用户的打包接口
            /// </summary>
            /// <param name="apLayer"></param>
            /// <param name="npLayer"></param>
            /// <param name="mapLayer"></param>
            /// <returns></returns>
            public static byte[] Packet(APLayer apLayer, NPLayer npLayer, MapLayer mapLayer)
            {
                List<byte> apData = new List<byte>();
                List<byte> npData = new List<byte>();
                List<byte> mapData = new List<byte>();

                // 打包MAP层数据包
                mapData = PacketMapLayer(mapLayer);

                switch (npLayer.npId)
                {
                    case CmccConst.NpId.A:
                        npData = PacketNPLayer(npLayer, mapLayer.mapId, mapData);
                        break;

                    default:
                        npData = mapData;
                        break;
                }

                switch (apLayer.apId)
                {
                    case CmccConst.ApId.A:
                        apData = PacketAPA(npLayer.npId, npData);
                        break;

                    case CmccConst.ApId.B:
                        apData = PacketAPB(npLayer.npId, npData);
                        break;
                    case CmccConst.ApId.C:
                        apData = PacketAPC(npLayer.npId, npData);
                        break;
                    default:
                        apData = npData;
                        break;
                }

                return apData.ToArray();
            }

            /// <summary>
            /// 解包AP.A协议数据包
            /// </summary>
            /// <param name="apData"></param>
            /// <param name="npLayer"></param>
            /// <param name="mapLayer"></param>
            /// <returns></returns>
            private static CmccUnpacketResult UnpacketAPA(byte[] apData, ref NPLayer npLayer, ref MapLayer mapLayer)
            {
                List<byte> apDataEx = new List<byte>(); // Cancel Converting and remove start/stop flag
                ProtocolBase.PrintBytes("apData", apData);
                bool convert = false;

                foreach (byte element in apData)
                {
                    if (element == CmccConst.ApFlag.A)
                    {
                        continue;
                    }

                    if (convert)
                    {
                        convert = false;

                        switch (element)
                        {
                            case 0x5D:
                                apDataEx.Add(0x5E);
                                break;

                            case 0x7D:
                                apDataEx.Add(0x7E);
                                break;

                            default:
                                return CmccUnpacketResult.OtherError;
                        }
                    }
                    else
                    {
                        if (element == 0x5E)
                        {
                            convert = true;
                        }
                        else
                        {
                            apDataEx.Add(element);
                        }
                    }
                }

                byte[] bytes = apDataEx.ToArray();

                if (bytes.Length <= 4)
                {
                    return CmccUnpacketResult.OtherError; // without np data
                }

                int offset = 0;
                ProtocolBase.PrintBytes("UnpacketAPA", bytes);
                if (bytes[offset++] != CmccConst.ApId.A)
                {
                    return CmccUnpacketResult.OtherError;
                }

                npLayer.npId = bytes[offset++];

                byte[] npData = new byte[bytes.Length - 4];

                Array.Copy(bytes, offset, npData, 0, npData.Length);
                offset += npData.Length;

                ushort crc_count = 0;
                ushort crc_read = 0;

                crc_count = ProtocolBase.CRC16(bytes[0], crc_count);
                crc_count = ProtocolBase.CRC16(bytes[1], crc_count);
                crc_count = ProtocolBase.CRC16(npData, crc_count);

                crc_read = BitConverter.ToUInt16(bytes, offset);
                offset += 2;

                if (crc_count != crc_read)
                {
                    return CmccUnpacketResult.CrcError;
                }

                switch (npLayer.npId)
                {
                    case CmccConst.NpId.A:
                        return UnpacketNPA(npData, ref npLayer, ref mapLayer);

                    default:
                        return CmccUnpacketResult.OtherError;
                }
            }


            /// <summary>
            /// 解包AP.A协议数据包
            /// </summary>
            /// <param name="apData"></param>
            /// <param name="npLayer"></param>
            /// <param name="mapLayer"></param>
            /// <returns></returns>
            private static CmccUnpacketResult UnpacketAPC(byte[] apData, ref NPLayer npLayer, ref MapLayer mapLayer)
            {
                List<byte> apDataEx = new List<byte>(); // Cancel Converting and remove start/stop flag
                ProtocolBase.PrintBytes("apData", apData);
                bool convert = false;

                foreach (byte element in apData)
                {
                    if (element == CmccConst.ApFlag.A)
                    {
                        continue;
                    }

                    if (convert)
                    {
                        convert = false;

                        switch (element)
                        {
                            case 0x5D:
                                apDataEx.Add(0x5E);
                                break;

                            case 0x7D:
                                apDataEx.Add(0x7E);
                                break;

                            default:
                                return CmccUnpacketResult.OtherError;
                        }
                    }
                    else
                    {
                        if (element == 0x5E)
                        {
                            convert = true;
                        }
                        else
                        {
                            apDataEx.Add(element);
                        }
                    }
                }

                byte[] bytes = apDataEx.ToArray();

                if (bytes.Length <= 4)
                {
                    return CmccUnpacketResult.OtherError; // without np data
                }

                int offset = 0;
                ProtocolBase.PrintBytes("UnpacketAPA", bytes);
                if (bytes[offset++] != CmccConst.ApId.C)
                {
                    return CmccUnpacketResult.OtherError;
                }

                npLayer.npId = bytes[offset++];

                byte[] npData = new byte[bytes.Length - 4];

                Array.Copy(bytes, offset, npData, 0, npData.Length);
                offset += npData.Length;

                ushort crc_count = 0;
                ushort crc_read = 0;

                crc_count = ProtocolBase.CRC16(bytes[0], crc_count);
                crc_count = ProtocolBase.CRC16(bytes[1], crc_count);
                crc_count = ProtocolBase.CRC16(npData, crc_count);

                crc_read = BitConverter.ToUInt16(bytes, offset);
                offset += 2;

                if (crc_count != crc_read)
                {
                    return CmccUnpacketResult.CrcError;
                }

                switch (npLayer.npId)
                {
                    case CmccConst.NpId.A:                       
                        return UnpacketNPA(npData, ref npLayer, ref mapLayer);

                    default:
                        return CmccUnpacketResult.OtherError;
                }
            }
                        
            /// <summary>
            /// 解包AP.B层协议数据包   
            /// </summary>
            /// <param name="apData"></param>
            /// <param name="npLayer"></param>
            /// <param name="mapLayer"></param>
            /// <returns></returns>
            private static CmccUnpacketResult UnpacketAPB(byte[] apData, ref NPLayer npLayer, ref MapLayer mapLayer)
            {
                List<byte> apDataEx = new List<byte>(); // Cancel Spliting and remove start/stop flag

                char hiChar = '-';
                char loChar = '-';

                byte combine = 0;

                foreach (byte element in apData)
                {
                    if (element == CmccConst.ApFlag.B)
                    {
                        continue;
                    }

                    if (hiChar != '-')
                    {
                        loChar = (char)element;

                        if (!ProtocolBase.CombineByte(hiChar, loChar, out combine))
                        {
                            return CmccUnpacketResult.OtherError;
                        }

                        apDataEx.Add(combine);

                        hiChar = '-';
                    }
                    else
                    {
                        hiChar = (char)element;
                    }
                }

                byte[] bytes = apDataEx.ToArray();

                if (bytes.Length <= 4)
                {
                    return CmccUnpacketResult.OtherError; // without np data
                }

                int offset = 0;

                if (bytes[offset++] != CmccConst.ApId.B)
                {
                    return CmccUnpacketResult.OtherError;
                }

                npLayer.npId = bytes[offset++];

                byte[] npData = new byte[bytes.Length - 4];

                Array.Copy(bytes, offset, npData, 0, npData.Length);
                offset += npData.Length;

                ushort crc_count = 0;
                ushort crc_read = 0;

                crc_count = ProtocolBase.CRC16(bytes[0], crc_count);
                crc_count = ProtocolBase.CRC16(bytes[1], crc_count);
                crc_count = ProtocolBase.CRC16(npData, crc_count);

                crc_read = BitConverter.ToUInt16(bytes, offset);
                offset += 2;

                if (crc_count != crc_read)
                {
                    return CmccUnpacketResult.CrcError;
                }

                switch (npLayer.npId)
                {
                    case CmccConst.NpId.A:                       
                        return UnpacketNPA(npData, ref npLayer, ref mapLayer);

                    default:
                        return CmccUnpacketResult.OtherError;
                }
            }

            /// <summary>
            /// 解包NP层协议数据包
            /// </summary>
            /// <param name="npData"></param>
            /// <param name="npLayer"></param>
            /// <param name="mapLayer"></param>
            /// <returns></returns>
            private static CmccUnpacketResult UnpacketNPA(byte[] npData, ref NPLayer npLayer, ref MapLayer mapLayer)
            {
                if (npData.Length <= 9)
                {
                    return CmccUnpacketResult.OtherError; // without map data
                }

                int offset = 0;

                npLayer.siteId = BitConverter.ToUInt32(npData, offset);
                offset += 4;

                npLayer.subId = npData[offset++];

                npLayer.packetId = BitConverter.ToUInt16(npData, offset);
                offset += 2;

                npLayer.npFlag = npData[offset++];
                mapLayer.mapId = npData[offset++];
               
                byte[] mapData = new byte[npData.Length - 9];

                Array.Copy(npData, offset, mapData, 0, mapData.Length);
                offset += mapData.Length;

                switch (mapLayer.mapId)
                {
                    case CmccConst.MapId.A:
                    case CmccConst.MapId.B:
                    case CmccConst.MapId.C:
                        return UnpacketMapLayer(mapData, ref mapLayer);

                    default:
                        return CmccUnpacketResult.OtherError;
                }
            }
                       
            /// <summary>
            /// 解包MAP层协议数据包
            /// </summary>
            /// <param name="mapData"></param>
            /// <param name="mapLayer"></param>
            /// <returns></returns>
            private static CmccUnpacketResult UnpacketMapLayer(byte[] mapData, ref MapLayer mapLayer)
            {
                if (mapData.Length <= 2)
                {
                    if (mapData.Length == 2)
                    {
                        mapLayer.objects = null;
                    }
                    else
                    {
                        return CmccUnpacketResult.OtherError;
                    }
                }

                int offset = 0;

                mapLayer.commandId = mapData[offset++];
                mapLayer.ackFlag = mapData[offset++];

                byte OL = 0;

                List<MonitorObject> list = new List<MonitorObject>();

                while ((offset + 1) <= mapData.Length)
                {
                    MonitorObject mo = new MonitorObject();

                    OL = mapData[offset++];

                    if (OL < 3)
                    {
                        return CmccUnpacketResult.OtherError;
                    }

                    if ((offset + 2) <= mapData.Length)
                    {
                        mo.Moid = BitConverter.ToUInt16(mapData, offset);
                      
                        offset += 2;
                    }
                    else
                    {
                        return CmccUnpacketResult.OtherError;
                    }

                    if ((offset + (OL - 3)) <= mapData.Length)
                    {
                        if ((OL - 3) > 0)
                        {
                            mo.Data = new byte[OL - 3];

                            Array.Copy(mapData, offset, mo.Data, 0, mo.Length);
                        }
                        else
                        {
                            mo.Data = null;
                        }

                        offset += (OL - 3);
                    }
                    else
                    {
                        return CmccUnpacketResult.OtherError;
                    }

                    list.Add(mo);
                }

                mapLayer.objects = list.ToArray();

                return CmccUnpacketResult.Succeed;
            }

            /// <summary>
            /// 探测并解包数据,属于外部接口
            /// </summary>
            /// <param name="data"></param>
            /// <param name="apLayer"></param>
            /// <param name="npLayer"></param>
            /// <param name="mapLayer"></param>
            /// <returns></returns>
            public static CmccUnpacketResult Unpacket(byte[] data, ref APLayer apLayer, ref NPLayer npLayer, ref MapLayer mapLayer)
            {
                if (data == null)
                {
                    return CmccUnpacketResult.OtherError;
                }

                int begin = -1;
                int end = -1;

                for (int n = 0; n < data.Length; n++)
                {
                    if (data[n] == CmccConst.ApFlag.A)
                    {
          
                        if (data[n + 1] == CmccConst.ApId.A)
                        {
                            apLayer.apId = CmccConst.ApId.A;
                            begin = n;
                            break;
                        }
                        else if (data[n + 1] == CmccConst.ApId.C)
                        {
                            apLayer.apId = CmccConst.ApId.C;
                            begin = n;
                            break;
                        }
                    }

                    if (data[n] == CmccConst.ApFlag.B)
                    {
                        apLayer.apId = CmccConst.ApId.B;
                        begin = n;
                        break;
                    }
                }

                if (begin < 0)
                {
                    return CmccUnpacketResult.OtherError;
                }

                for (int n = begin + 1; n < data.Length; n++)
                {
                    if (data[n] == data[begin])
                    {
                        end = n;
                        break;
                    }
                }

                if (end < 0)
                {
                    return CmccUnpacketResult.OtherError;
                }

                byte[] apData = new byte[end - begin + 1];

                for (int n = 0; n < apData.Length; n++)
                {
                    apData[n] = data[n + begin];
                }

                switch (apLayer.apId)
                {
                    case CmccConst.ApId.A:
                        return UnpacketAPA(apData, ref npLayer, ref mapLayer);
                      
                    case CmccConst.ApId.B:
                        return UnpacketAPB(apData, ref npLayer, ref mapLayer);
                    
                    case CmccConst.ApId.C:
                        return UnpacketAPC(apData, ref npLayer, ref mapLayer);
                      
                    default:
                        return CmccUnpacketResult.OtherError;
                }
            }
        }
    }
}
