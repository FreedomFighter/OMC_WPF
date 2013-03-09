using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CmccLib
{
    class ProtocolBase
    {
        /// <summary>
        /// 计算字节流的CRC值
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static ushort CRC16(byte[] data)
        {
            return CRC16(data, (ushort)0);
        }

        /// <summary>
        /// 计算byte型数据的CRC值，crcOrigin使得CRC结果可以累积运算
        /// </summary>
        /// <param name="data"></param>
        /// <param name="crcOrigin"></param>
        /// <returns></returns>
        public static ushort CRC16(byte data, ushort crcOrigin)
        {
            byte[] bytesData = { data };

            return CRC16(bytesData, crcOrigin);
        }

        /// <summary>
        /// 计算sbyte型数据的CRC值，crcOrigin使得CRC结果可以累积运算
        /// </summary>
        /// <param name="data"></param>
        /// <param name="crcOrigin"></param>
        /// <returns></returns>
        public static ushort CRC16(sbyte data, ushort crcOrigin)
        {
            byte[] bytesData = { (byte)data };

            return CRC16(bytesData, crcOrigin);
        }

        /// <summary>
        /// 计算ushort型数据的CRC值，crcOrigin使得CRC结果可以累积运算
        /// </summary>
        /// <param name="data"></param>
        /// <param name="crcOrigin"></param>
        /// <returns></returns>
        public static ushort CRC16(ushort data, ushort crcOrigin)
        {
            return CRC16(BitConverter.GetBytes(data), crcOrigin);
        }

        /// <summary>
        /// 计算short型数据的CRC值，crcOrigin使得CRC结果可以累积运算
        /// </summary>
        /// <param name="data"></param>
        /// <param name="crcOrigin"></param>
        /// <returns></returns>
        public static ushort CRC16(short data, ushort crcOrigin)
        {
            return CRC16(BitConverter.GetBytes(data), crcOrigin);
        }

        /// <summary>
        /// 计算uint型数据的CRC值，crcOrigin使得CRC结果可以累积运算
        /// </summary>
        /// <param name="data"></param>
        /// <param name="crcOrigin"></param>
        /// <returns></returns>
        public static ushort CRC16(uint data, ushort crcOrigin)
        {
            return CRC16(BitConverter.GetBytes(data), crcOrigin);
        }

        /// <summary>
        /// 计算int型数据的CRC值，crcOrigin使得CRC结果可以累积运算
        /// </summary>
        /// <param name="data"></param>
        /// <param name="crcOrigin"></param>
        /// <returns></returns>
        public static ushort CRC16(int data, ushort crcOrigin)
        {
            return CRC16(BitConverter.GetBytes(data), crcOrigin);
        }

        /// <summary>
        /// 计算string型数据(ASCII编码)的CRC值，crcOrigin使得CRC结果可以累积运算
        /// 如果输入数据包含非ASCII码字符，此函数将抛掷异常
        /// </summary>
        /// <param name="data"></param>
        /// <param name="crcOrigin"></param>
        /// <returns></returns>
        public static ushort CRC16(string data, ushort crcOrigin)
        {
            if (Encoding.UTF8.GetByteCount(data) != data.Length)
            {
                throw new Exception("Only ASCII characters could use CRC16() function");
            }

            return CRC16(Encoding.ASCII.GetBytes(data), crcOrigin);
        }

        /// <summary>
        /// 计算字节流数据的CRC值，crcOrigin使得CRC结果可以累积运算
        /// </summary>
        /// <param name="data"></param>
        /// <param name="crcOrigin"></param>
        /// <returns></returns>
        public static ushort CRC16(byte[] data, ushort crcOrigin)
        {
            ushort crcResult = crcOrigin;
            ushort crcExpress = 0x1021; // CRC16 expression

            foreach (byte value in data)
            {
                for (int n = 0; n < 8; n++)
                {
                    if ((crcResult & 0x8000) != 0)
                    {
                        crcResult <<= 1;
                        crcResult ^= crcExpress;
                    }
                    else
                    {
                        crcResult <<= 1;
                    }

                    if ((value & (1 << (7 - n))) != 0)
                    {
                        crcResult ^= crcExpress;
                    }
                }
            }

            return crcResult;
        }
        
        /// <summary>
        ///  计算字节流的校验和
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static ushort Checksum(byte[] data)
        {
            return Checksum(data, 0);
        }

        /// <summary>
        /// 计算byte型数据的校验和，sumOrigin使得Checksum能够累积运算
        /// </summary>
        /// <param name="data"></param>
        /// <param name="sumOrigin"></param>
        /// <returns></returns>
        public static ushort Checksum(byte data, ushort sumOrigin)
        {
            return (ushort)(data + sumOrigin);
        }

        /// <summary>
        /// 计算sbyte型数据的校验和，sumOrigin使得Checksum能够累积运算
        /// </summary>
        /// <param name="data"></param>
        /// <param name="sumOrigin"></param>
        /// <returns></returns>
        public static ushort Checksum(sbyte data, ushort sumOrigin)
        {
            return (ushort)((byte)data + sumOrigin);
        }

        /// <summary>
        /// 计算ushort型数据的校验和，sumOrigin使得Checksum能够累积运算
        /// </summary>
        /// <param name="data"></param>
        /// <param name="sumOrigin"></param>
        /// <returns></returns>
        public static ushort Checksum(ushort data, ushort sumOrigin)
        {
            return Checksum(BitConverter.GetBytes(data), sumOrigin);
        }

        /// <summary>
        /// 计算short型数据的校验和，sumOrigin使得Checksum能够累积运算
        /// </summary>
        /// <param name="data"></param>
        /// <param name="sumOrigin"></param>
        /// <returns></returns>
        public static ushort Checksum(short data, ushort sumOrigin)
        {
            return Checksum(BitConverter.GetBytes(data), sumOrigin);
        }

        /// <summary>
        /// 计算uint型数据的校验和，sumOrigin使得Checksum能够累积运算
        /// </summary>
        /// <param name="data"></param>
        /// <param name="sumOrigin"></param>
        /// <returns></returns>
        public static ushort Checksum(uint data, ushort sumOrigin)
        {
            return Checksum(BitConverter.GetBytes(data), sumOrigin);
        }
               
        /// <summary>
        /// 计算int型数据的校验和，sumOrigin使得Checksum能够累积运算
        /// </summary>
        /// <param name="data"></param>
        /// <param name="sumOrigin"></param>
        /// <returns></returns>
        public static ushort Checksum(int data, ushort sumOrigin)
        {
            return Checksum(BitConverter.GetBytes(data), sumOrigin);
        }

        /// <summary>
        ///  计算string型数据(ASCII编码)的校验和，sumOrigin使得Checksum可以累积运算
        ///  如果输入数据包含非ASCII码字符，此函数将抛掷异常
        /// </summary>
        /// <param name="data"></param>
        /// <param name="sumOrigin"></param>
        /// <returns></returns>
        public static ushort Checksum(string data, ushort sumOrigin)
        {
            if (Encoding.UTF8.GetByteCount(data) != data.Length)
            {
                throw new Exception("Only ASCII characters could use Checksum() function");
            }

            return Checksum(Encoding.ASCII.GetBytes(data), sumOrigin);
        }

        /// <summary>
        /// 计算字节流数据的校验和，sumOrigin使得Checksum可以累积运算
        /// </summary>
        /// <param name="data"></param>
        /// <param name="sumOrigin"></param>
        /// <returns></returns>
        public static ushort Checksum(byte[] data, ushort sumOrigin)
        {
            ushort result = sumOrigin;

            foreach (byte element in data)
            {
                result += (ushort)element;
            }

            return result;
        }

        /// <summary>
        /// 0~9 => '0'~'9'; 10~15 => 'A'~'F'
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        private static bool HalfByteToChar(byte input, out char output)
        {
            output = '0';

            if ((input >= 0) && (input <= 9))
            {
                output = (char)('0' + input);
                return true;
            }

            if ((input >= 10) && (input <= 15))
            {
                output = (char)('A' + (input - 10));
                return true;
            }

            return false;
        }

        /// <summary>
        /// '0'~'9' => 0~9; 'A'~'F' => 10~15
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        private static bool CharToHalfByte(char input, out byte output)
        {
            output = 0;

            if ((input >= '0') && (input <= '9'))
            {
                output = (byte)(input - '0');
                return true;
            }

            if ((input >= 'A') && (input <= 'F'))
            {
                output = (byte)((input - 'A') + 10);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 字节拆分(16进制显示)，输出第1个字符(hiChar)和第2个字符(loChar)
        /// </summary>
        /// <param name="data"></param>
        /// <param name="hiChar"></param>
        /// <param name="loChar"></param>
        public static void SplitByte(byte data, out char hiChar, out char loChar)
        {
            byte hiHalfByte = (byte)((data >> 4) & 0x0F);
            byte loHalfByte = (byte)((data >> 0) & 0x0F);

            HalfByteToChar(hiHalfByte, out hiChar);
            HalfByteToChar(loHalfByte, out loChar);
        }

        /// <summary>
        /// 字节拆分(16进制显示)，直接输出X2格式化的结果
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string SplitByte(byte data)
        {
            return string.Format("{0:X2}", data);
        }

        /// <summary>
        /// 字节拆分的反向处理
        /// </summary>
        /// <param name="hiChar"></param>
        /// <param name="loChar"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool CombineByte(char hiChar, char loChar, out byte data)
        {
            data = 0;

            byte hiHalfByte = 0;

            if (!CharToHalfByte(hiChar, out hiHalfByte))
            {
                return false;
            }

            byte loHalfByte = 0;

            if (!CharToHalfByte(loChar, out loHalfByte))
            {
                return false;
            }

            data = (byte)((hiHalfByte << 4) + (loHalfByte << 0));

            return true;
        }

        /// <summary>
        /// 将字符串转成固定长度的字节流，不足部分以extra填充
        /// </summary>
        /// <param name="data"></param>
        /// <param name="length"></param>
        /// <param name="extra"></param>
        /// <returns></returns>
        public static byte[] StringToBytes(string data, int length, byte extra)
        {
            byte[] result = new byte[length];

            for (int n = 0; n < length; n++)
            {
                if (n < data.Length)
                {
                    result[n] = (byte)data[n];
                }
                else
                {
                    result[n] = extra;
                }
            }

            return result;
        }

        /// <summary>
        /// debug打印数据
        /// </summary>
        /// <param name="strDebugName"></param>
        /// <param name="bytes"></param>
        public static void PrintBytes(String strDebugName, byte[] bytes)
        {
            Console.WriteLine(strDebugName);

            foreach (byte b in bytes)
            {
                Console.Write(string.Format("{0:X2} ", b));
            }
        }
    }
}
