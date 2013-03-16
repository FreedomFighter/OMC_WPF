using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;

namespace nms_datatype_lib
{
    public enum DataType
    {
        Unsigned, // Un, Un<Scale>, Un<Scale,Offset> -- n:{1,2,4}
        Signed, // Sn, Sn<Scale>, Sn<Scale,Offset> -- n:{1,2,4}
        Enum, // En<EnumName> -- n:{1,2,4}
        Hex, // Hn -- n:{1,2,4}
        Float, // Fn -- n:{4,8}
        LongOrLatitude, // Ln -- n:{1~80}
        Longtitude, //On -- n:{1~80}
        Latitude, // Nn -- n:{1~80}
        String, // Tn -- n:{1~80}
        Phone, // Pn -- n:{1~80}
        Bytes, // Bn -- n:{0~80}
        Bit, // BIT, BIT<Offset>
        IPv4, // IPv4
        IPv6, // IPv6
        Special, // X
        Att,// An, An<Scale>,Un<Scale,Offset>--n:{1,2,4}
        Date,// Dn
        Unknown // Cn,Dn,Fn,Gn,In,Jn,Kn,Ln,Mn,Qn,Rn,Tn,Vn,Wn
    }

    public class EnumObject
    {
        public static string ToEnumType(object name, int bytes)
        {
            return string.Format("E{0}<{1}>", bytes, name);
        }

        public override string ToString()
        {
            return string.Format("{0}: ({1},{2})", Name, Key, Value);
        }

        private string _Name; // 枚举名称(EnumName)
        public string Name
        {
            get { return _Name; }
            private set { _Name = value; }
        }

        private uint _Key; // 关键字
        public uint Key
        {
            get { return _Key; }
            private set { _Key = value; }
        }

        private string _Value; // Key对应的文本
        public string Value
        {
            get { return _Value; }
            private set { _Value = value; }
        }

        private object _Tag;
        public object Tag
        {
            get { return _Tag; }
            set { _Tag = value; }
        }

        public EnumObject(object name, uint key, string value)
        {
            this.Name = name.ToString();
            this.Key = key;
            this.Value = value;
            this.Tag = null;
        }

        public EnumObject(object name, uint key, string value, object tag)
        {
            this.Name = name.ToString();
            this.Key = key;
            this.Value = value;
            this.Tag = tag;
        }

        public EnumObject Clone()
        {
            return new EnumObject(this.Name, this.Key, this.Value, this.Tag);
        }
    }

    class InternalPublic
    {
        // 分析十进制或者十六进制的非负整数
        public static bool TryParseUint(string s, out uint result)
        {
            if (s.StartsWith("0x") || s.StartsWith("0X"))
            {
                return uint.TryParse(s.Substring(2, s.Length - 2), NumberStyles.HexNumber, null, out result);
            }
            else
            {
                return uint.TryParse(s, out result);
            }
        }

        // 检查无符号数值是否在指定字节长度内
        public static bool CheckUnsigned(uint uintValue, int typeLength)
        {
            bool result = false;

            switch (typeLength)
            {
                case 1:
                    result = (uintValue >= byte.MinValue) && (uintValue <= byte.MaxValue);
                    break;

                case 2:
                    result = (uintValue >= ushort.MinValue) && (uintValue <= ushort.MaxValue);
                    break;

                case 4:
                    result = true;
                    break;

                default:
                    break;
            }

            return result;
        }

        // 检查衰减值值是否在指定字节长度内
        public static bool CheckAtt(uint uintValue, int typeLength)
        {
            bool result = false;

            switch (typeLength)
            {
                case 1:
                    result = (uintValue >= byte.MinValue) && (uintValue <= 31);
                    break;

                default:
                    break;
            }

            return result;
        }

        // 检查无符号数值是否在指定字节长度内
        public static bool CheckSigned(int intValue, int typeLength)
        {
            bool result = false;

            switch (typeLength)
            {
                case 1:
                    result = (intValue >= sbyte.MinValue) && (intValue <= sbyte.MaxValue);
                    break;

                case 2:
                    result = (intValue >= short.MinValue) && (intValue <= short.MaxValue);
                    break;

                case 4:
                    result = true;
                    break;

                default:
                    break;
            }

            return result;
        }

        // 判断string是否只包含ASCII字符
        public static bool IsASCII(string s)
        {
            return (Encoding.UTF8.GetByteCount(s) == s.Length);
        }

        // 以double类型分析描述范围的数值，除常规数值外，允许以0x开头的16进制数和min、max等特殊值
        // throw Exception
        private static double ParseRangeValue(string s)
        {
            s = s.Trim().ToLower();

            switch (s)
            {
                case "min":
                    return double.MinValue;

                case "max":
                    return double.MaxValue;

                default:
                    if (s.StartsWith("0x"))
                    {
                        uint uintValue = uint.MaxValue;

                        if (TryParseUint(s, out uintValue))
                        {
                            return (double)uintValue;
                        }
                    }
                    else
                    {
                        double doubleValue = double.MaxValue;

                        if (double.TryParse(s, out doubleValue))
                        {
                            return doubleValue;
                        }
                    }
                    break;
            }

            throw new Exception("Invalid range: " + s);
        }

        // 分析数值是否落在指定区间内
        // throw Exception
        public static bool IsBetween(double value, string between)
        {
            between = between.Trim();

            if (string.IsNullOrEmpty(between))
            {
                return false;
            }

            string[] s = between.Split('~');

            try
            {
                double minValue = (s.Length > 0) ? ParseRangeValue(s[0]) : double.MinValue;
                double maxValue = (s.Length > 1) ? ParseRangeValue(s[1]) : minValue;

                return (value >= minValue) && (value <= maxValue);
            }
            catch (Exception e)
            {
                throw e; // 捕获异常后被重新抛掷，明确告诉调用函数需处理异常
            }
        }

        // 判断value是否在指定范围内
        public static bool InRange(int value, params int[] range)
        {
            foreach (int element in range)
            {
                if (value == element)
                {
                    return true;
                }
            }

            return false;
        }
    }

    // 根据指定的枚举对象集合进行相关转换
    public class EnumParser
    {
        private EnumObject[] Objects;

        public EnumParser(EnumObject[] objects)
        {
            this.Objects = objects;
        }

        // 获取指定枚举类型的枚举对象集合
        public EnumObject[] GetEnumObjects(object name)
        {
            List<EnumObject> list = new List<EnumObject>();

            string sName = name.ToString();

            foreach (EnumObject element in Objects)
            {
                if (element.Name == sName)
                {
                    list.Add(element);
                }
            }

            return list.ToArray();
        }

        // 从枚举数值获取对应的文本(查找失败时value依然有效)
        public void GetEnumValue(object name, uint key, out string value)
        {
            value = string.Empty; // out object

            string sName = name.ToString();

            foreach (EnumObject element in Objects)
            {
                if ((element.Name == sName) && (element.Key == key))
                {
                    value = element.Value;
                    return;
                }
            }

            // 未定义时以约定格式0xHex(Decimal)呈现
            value = string.Format("0x{0:X}({0})", key);
            return;
        }

        // 根据对应的文本反向获取枚举数值
        public bool GetEnumKey(object name, string value, out uint key)
        {
            key = 0; // out object

            string sName = name.ToString();

            // 符合约定格式0xHex(Decimal)的直接提取key的数值
            if (value.StartsWith("0x"))
            {
                int left = value.IndexOf('(');
                int right = value.IndexOf(')');

                if ((left > 0) && (right > left))
                {
                    if (uint.TryParse(value.Substring(left + 1, right - left - 1), out key))
                    {
                        return true;
                    }
                }
            }

            foreach (EnumObject element in Objects)
            {
                if ((element.Name == sName) && (element.Value == value))
                {
                    key = element.Key;
                    return true;
                }
            }

            return false; // 保留作为无效值
        }

        public bool GetFirstKey(object name, out uint key)
        {
            key = 0; // out object

            string sName = name.ToString();

            foreach (EnumObject element in Objects)
            {
                if (element.Name == sName)
                {
                    key = element.Key;
                    return true;
                }
            }

            return false;
        }

        public bool GetFirstValue(object name, out string value)
        {
            value = string.Empty; // out object

            string sName = name.ToString();

            foreach (EnumObject element in Objects)
            {
                if (element.Name == sName)
                {
                    value = element.Value;
                    return true;
                }
            }

            return false;
        }
    }

    // 根据指定的数据类型字符串进行相关解析
    public class TypeParser
    {
        private string sDataType;
        private EnumObject[] EnumObjects;

        public TypeParser(string dataType, EnumObject[] objects)
        {
            this.sDataType = dataType;
            this.EnumObjects = (objects != null) ? objects : new EnumObject[0];
        }

        // 获取一对尖括号之间的字符串
        private string GetOption(string s)
        {
            int begin = s.IndexOf('<');
            int end = s.IndexOf('>');

            if ((begin < 0) || (end < 0) || (begin > end))
            {
                return null;
            }

            string result = s.Substring(begin + 1, end - begin - 1);

            return result.Trim();
        }

        // 解析类型信息
        public DataType Type
        {
            get
            {
                if (sDataType.StartsWith("BIT"))
                {
                    string sOption = GetOption(sDataType).Trim();

                    if (!string.IsNullOrEmpty(sOption))
                    {
                        byte byteValue = 0;

                        if (!byte.TryParse(sOption, NumberStyles.HexNumber, null, out byteValue))
                        {
                            return DataType.Unknown;
                        }
                    }

                    return DataType.Bit;
                }

                if (sDataType == "IPv4")
                {
                    return DataType.IPv4;
                }

                if (sDataType == "IPv6")
                {
                    return DataType.IPv6;
                }

                int length = 0;

                // test length
                {
                    int pos = sDataType.IndexOf('<');

                    if (pos < 0) pos = sDataType.Length;

                    string lenstr = sDataType.Substring(1, pos - 1);

                    if (!int.TryParse(lenstr, out length))
                    {
                        length = 0;
                    }
                }

                switch (sDataType[0])
                {
                    case 'U':
                        if (!InternalPublic.InRange(length, new int[] { 1, 2, 4 }))
                        {
                            return DataType.Unknown;
                        }

                        return DataType.Unsigned;

                    case 'A':
                        if (!InternalPublic.InRange(length, new int[] { 1, 2, 4 }))
                        {
                            return DataType.Unknown;
                        }

                        return DataType.Att;

                    case 'S':
                        if (!InternalPublic.InRange(length, new int[] { 1, 2, 4 }))
                        {
                            return DataType.Unknown;
                        }

                        return DataType.Signed;

                    case 'H':
                        if (!InternalPublic.InRange(length, new int[] { 1, 2, 4 }))
                        {
                            return DataType.Unknown;
                        }

                        return DataType.Hex;

                    case 'F':
                        if (!InternalPublic.InRange(length, new int[] { 1, 2, 4, 8 }))
                        {
                            return DataType.Unknown;
                        }

                        return DataType.Float;

                    case 'E':
                        if (!InternalPublic.InRange(length, new int[] { 1, 2, 4 }))
                        {
                            return DataType.Unknown;
                        }

                        return DataType.Enum;

                    case 'T':
                        if ((length < 1) || (length > 80))
                        {
                            return DataType.Unknown;
                        }

                        return DataType.String;

                    case 'P':
                        if ((length < 1) || (length > 80))
                        {
                            return DataType.Unknown;
                        }

                        return DataType.Phone;

                    case 'D': // 2009.11.18
                        if ((length < 1) || (length > 80))
                        {
                            return DataType.Unknown;
                        }

                        return DataType.Date;

                    case 'L':
                        if ((length < 1) || (length > 80))
                        {
                            return DataType.Unknown;
                        }

                        return DataType.LongOrLatitude;
                    case 'O':
                        if ((length < 1) || (length > 80))
                        {
                            return DataType.Unknown;
                        }

                        return DataType.Longtitude;
                    case 'N':
                        if ((length < 1) || (length > 80))
                        {
                            return DataType.Unknown;
                        }

                        return DataType.Latitude;

                    case 'B':
                        if ((length < 0) || (length > 80))
                        {
                            return DataType.Unknown;
                        }

                        return DataType.Bytes;

                    case 'X':
                        return DataType.Special;

                    default:
                        return DataType.Unknown;
                }
            }
        }

        // 解析长度信息(对于T、L、P等类型代表最大字节数，否则代表固定的字节数)
        public byte Length
        {
            get
            {
                switch (Type)
                {
                    case DataType.Bit:
                        return 1;

                    case DataType.IPv4:
                        return 4;

                    case DataType.IPv6:
                        return 6;

                    case DataType.Special:
                    case DataType.Unknown:
                        return 0;

                    default:
                        break;
                }

                int result = 0;
                int pos = sDataType.IndexOf('<');

                if (pos < 0) pos = sDataType.Length;

                string lenstr = sDataType.Substring(1, pos - 1);

                if (!int.TryParse(lenstr, out result))
                {
                    result = 0;
                }

                return (byte)result;
            }
        }

        // 对于枚举类型返回类型名称，否则返回空
        public string EnumName
        {
            get
            {
                if (Type != DataType.Enum)
                {
                    return string.Empty;
                }

                return GetOption(sDataType);
            }
        }

        // 对于U、S类型返回scale值(非零值)，否则返回1
        public int Scale
        {
            get
            {
                int result = 1;

                switch (Type)
                {
                    case DataType.Unsigned:
                    case DataType.Att:
                    case DataType.Signed:
                        {
                            string s = GetOption(sDataType);

                            if (!string.IsNullOrEmpty(s))
                            {
                                int split = s.IndexOf(',');

                                if (split >= 0)
                                {
                                    s = s.Substring(0, split);
                                }

                                if (!int.TryParse(s.Trim(), out result))
                                {
                                    result = 1;
                                }

                                // 0值运算时可能发生异常
                                if (result == 0)
                                {
                                    result = 1;
                                }
                            }
                        }
                        break;

                    default:
                        break;
                }

                return result;
            }
        }

        // 对于U、S类型返回offset值，对于Bit类型返回占位信息，否则返回0
        public int Offset
        {
            get
            {
                int result = 0;

                switch (Type)
                {
                    case DataType.Unsigned:
                    case DataType.Att:
                    case DataType.Signed:
                        {
                            string s = GetOption(sDataType);

                            if (!string.IsNullOrEmpty(s))
                            {
                                int split = s.IndexOf(',');

                                if (split >= 0)
                                {
                                    s = s.Substring(split + 1, s.Length - (split + 1));

                                    if (!int.TryParse(s.Trim(), out result))
                                    {
                                        result = 0; // 缺省值
                                    }
                                }
                                else
                                {
                                    result = 0; // 缺省值
                                }
                            }
                        }
                        break;

                    case DataType.Bit:
                        {
                            string s = GetOption(sDataType);

                            if (!string.IsNullOrEmpty(s))
                            {
                                if (!int.TryParse(s, NumberStyles.HexNumber, null, out result))
                                {
                                    result = 0x01; // 缺省值
                                }
                            }
                            else
                            {
                                result = 0x01; // 缺省值
                            }
                        }
                        break;

                    default:
                        break;
                }

                return result;
            }
        }

        // 判断value是否是type限定下的有效值
        // 对于特殊类型和未知类型，返回false
        public bool TryParse(string value)
        {
            bool result = false;

            if (this.Type != DataType.Enum)
            {
                if (!InternalPublic.IsASCII(value))
                {
                    return false;
                }
            }
            else
            {
                EnumParser enumParser = new EnumParser(EnumObjects);

                uint enumKey = 0;

                return enumParser.GetEnumKey(this.EnumName, value, out enumKey);
            }

            if ((this.Type == DataType.Unsigned) || (this.Type == DataType.Signed) || (this.Type == DataType.Att))
            {
                // 计算存储值并替换到value变量
                if ((this.Scale != 1) || (this.Offset != 0))
                {
                    double doubleValue = double.MaxValue;

                    if (double.TryParse(value, out doubleValue))
                    {
                        value = ((Int64)(doubleValue * this.Scale + this.Offset)).ToString();
                    }
                    else
                    {
                        value = "<invalid>"; // 后续分析作为无效处理
                    }
                }
            }

            uint uintValue = 0;
            int intValue = 0;

            switch (this.Type)
            {
                case DataType.Unsigned:
                    if (result = uint.TryParse(value, out uintValue))
                    {
                        result = InternalPublic.CheckUnsigned(uintValue, this.Length);
                    }
                    break;

                case DataType.Att:////////////////////////////////////////////////////
                    if (result = uint.TryParse(value, out uintValue))
                    {
                        result = InternalPublic.CheckAtt(uintValue, this.Length);
                    }
                    break;

                case DataType.Signed:
                    if (result = int.TryParse(value, out intValue))
                    {
                        result = InternalPublic.CheckSigned(intValue, this.Length);
                    }
                    break;

                case DataType.Hex:
                    if (result = uint.TryParse(value, NumberStyles.HexNumber, null, out uintValue))
                    {
                        result = InternalPublic.CheckUnsigned(uintValue, this.Length);
                    }
                    break;

                case DataType.Float:
                    switch (this.Length)
                    {
                        case 1:
                        case 2:
                        case 4:
                            {
                                float floatValue = 0.0F;
                                result = float.TryParse(value, out floatValue);
                            }
                            break;

                        case 8:
                            {
                                double doubleValue = 0.0;
                                result = double.TryParse(value, out doubleValue);
                            }
                            break;

                        default:
                            result = false;
                            break;
                    }
                    break;

                case DataType.String:
                case DataType.LongOrLatitude:


                    result = (value.Length <= this.Length);
                    break;

                case DataType.Longtitude://2009.9.30
                    //result = (value.Length <= this.Length);
                    if ((value.StartsWith("E") || (value.StartsWith("W"))) && (value.Length > 1))
                    {
                        value = value.Substring(1);
                        try
                        {
                            if ((double.Parse(value) >= 0) && (double.Parse(value) <= 180) && (value.Length <= this.Length))
                            {
                                result = true;
                            }
                            else
                            {
                                result = false;
                            }
                        }
                        catch
                        {
                            result = false;
                        }
                    }
                    else
                    {
                        result = false;
                    }
                    break;

                case DataType.Latitude://2009.9.30
                    //result = (value.Length <= this.Length);
                    if (((value.StartsWith("S")) || (value.StartsWith("N"))) && (value.Length > 1))
                    {
                        value = value.Substring(1);
                        try
                        {

                            if ((double.Parse(value) >= 0) && (double.Parse(value) <= 90) && (value.Length <= this.Length))
                            {
                                result = true;
                            }
                            else
                            {
                                result = false;
                            }
                        }
                        catch
                        {
                            result = false;
                        }
                    }
                    else
                    {
                        result = false;
                    }
                    break;

                case DataType.Date:
                    {
                        result = true;
                        // if (value.Length != 7)
                        if (value.Length != 14)
                        {
                            result = false;
                        }
                        string strTemp = value;

                        foreach (char c in strTemp)
                        {
                            if (result)
                            {
                                //result &= (char.IsDigit(c) || (c == '+'));
                                result &= (char.IsDigit(c));
                            }
                            else
                            {
                                break;
                            }
                        }

                    }
                    break;

                case DataType.Phone:
                    {

                        result = (value.Length <= this.Length);
                        string strTemp = value;
                        //huabengao 2012.10.29
                        //增加电话号码以+号开头的长度限制，号码数字不能超过20位
                        if (value.StartsWith("+"))
                        {
                            result = true;
                            if (value.Length > 1 && value.Length <= this.Length)
                            {
                                strTemp = value.Substring(1);
                            }
                        }

                        foreach (char c in strTemp)
                        {
                            if (result)
                            {
                                //result &= (char.IsDigit(c) || (c == '+'));
                                result &= (char.IsDigit(c));
                            }
                            else
                            {
                                break;
                            }
                        }

                        //if (result)
                        //{
                        //    //result &= (value.LastIndexOf('+') == 0);
                        //    result |= (value.LastIndexOf('+') == 0);
                        //}



                        //foreach (char c in value)
                        //{
                        //    if (result)
                        //    {
                        //        //result &= (char.IsDigit(c) || (c == '+'));
                        //        result &= (char.IsDigit(c));
                        //    }
                        //    else
                        //    {
                        //        break;
                        //    }
                        //}
                    }
                    break;

                case DataType.Bytes:
                    if (this.Length > 0)
                    {
                        result = true;

                        if (value.Length != (this.Length * 3 + 1))
                        {
                            result = false;
                        }
                        else
                        {
                            for (int n = 0; n < value.Length; n++)
                            {
                                if (n == 0)
                                {
                                    if (value[n] != '{')
                                    {
                                        result = false;
                                        break;
                                    }
                                }
                                else
                                    if (n == (value.Length - 1))
                                    {
                                        if (value[n] != '}')
                                        {
                                            result = false;
                                            break;
                                        }
                                    }
                                    else
                                        if ((n % 3) == 0)
                                        {
                                            if (value[n] != '.')
                                            {
                                                result = false;
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            if (!char.IsDigit(value[n]) && !((value[n] >= 'A') && (value[n] <= 'F')))
                                            {
                                                result = false;
                                                break;
                                            }
                                        }
                            }
                        }
                    }
                    else
                    {
                        result = (value == "{}");
                    }
                    break;

                case DataType.Bit:
                    result = ((value == "0") || (value == "1"));
                    break;

                case DataType.IPv4:
                case DataType.IPv6:
                    {
                        result = true;

                        string[] IPn = value.Split('.');

                        if (IPn.Length != Length)
                        {
                            result = false;
                        }
                        else
                        {
                            byte byteValue = byte.MaxValue;

                            foreach (string s in IPn)
                            {
                                if (!byte.TryParse(s.Trim(), out byteValue))
                                {
                                    result = false;
                                }
                            }
                        }
                    }
                    break;

                case DataType.Special:
                    break;

                default:
                    break;
            }

            return result;
        }

        // 本函数假定value与本对象的类型是匹配的，否则可能抛掷异常
        // 参数EnumObjectTable对于非枚举类型提供null即可
        public byte[] GetBytes(string value)
        {
            byte[] result = new byte[0];

            // 预处理，简化后续分析过程
            switch (this.Type)
            {
                case DataType.Unsigned:
                case DataType.Att:
                case DataType.Signed:
                    {
                        // 将scale/offset作用的值转换为存储值
                        if ((this.Scale != 1) || (this.Offset != 0))
                        {
                            double doubleValue = double.MaxValue;

                            if (double.TryParse(value, out doubleValue))
                            {
                                value = ((Int64)(doubleValue * this.Scale + this.Offset)).ToString();
                            }
                            else
                            {
                                value = "<invalid>"; // 后续分析作为无效处理
                            }
                        }
                    }
                    break;

                case DataType.Enum:
                    {
                        EnumParser enumParser = new EnumParser(EnumObjects);

                        uint enumKey = 0;

                        enumParser.GetEnumKey(this.EnumName, value, out enumKey);

                        value = enumKey.ToString();
                    }
                    break;

                case DataType.Hex:
                    {
                        uint hexValue = uint.Parse(value, NumberStyles.HexNumber);

                        value = hexValue.ToString();
                    }
                    break;

                default:
                    break;
            }

            switch (this.Type)
            {
                case DataType.Unsigned:
                case DataType.Att:
                case DataType.Hex:
                case DataType.Enum:
                case DataType.Bit:
                    switch (this.Length)
                    {
                        case 1:
                            result = new byte[] { byte.Parse(value) };
                            break;

                        case 2:
                            result = BitConverter.GetBytes(ushort.Parse(value));
                            break;

                        case 4:
                            result = BitConverter.GetBytes(uint.Parse(value));
                            break;

                        default:
                            break;
                    }
                    break;

                case DataType.Signed:
                    switch (this.Length)
                    {
                        case 1:
                            result = new byte[] { (byte)sbyte.Parse(value) };
                            break;

                        case 2:
                            result = BitConverter.GetBytes(short.Parse(value));
                            break;

                        case 4:
                            result = BitConverter.GetBytes(int.Parse(value));
                            break;

                        default:
                            break;
                    }
                    break;

                case DataType.Float:
                    switch (this.Length)
                    {
                        case 1:
                        case 2:
                        case 4:
                            result = BitConverter.GetBytes(float.Parse(value));
                            break;

                        case 8:
                            result = BitConverter.GetBytes(double.Parse(value));
                            break;

                        default:
                            break;
                    }
                    break;

                case DataType.String:
                case DataType.LongOrLatitude:
                case DataType.Longtitude:// 2009.9.30
                case DataType.Latitude:// 2009.9.30
                case DataType.Phone:
                    {
                        result = new byte[this.Length];

                        byte[] temp = Encoding.ASCII.GetBytes(value);

                        for (int n = 0; n < result.Length; n++)
                        {
                            result[n] = (n < temp.Length) ? temp[n] : (byte)0;
                        }
                    }
                    break;

                case DataType.Date:
                    {
                        result = new byte[this.Length];

                        byte[] temp = Encoding.ASCII.GetBytes(value);

                        for (int n = 0; n < result.Length; n++)
                        {
                            result[n] = (n < temp.Length) ? temp[n] : (byte)0;
                        }
                    }
                    break;

                //case DataType.Longtitude:// 2009.9.30
                //    {
                //        result = new byte[this.Length];

                //        byte[] temp = Encoding.ASCII.GetBytes(value);

                //        for (int n = 0; n < result.Length; n++)
                //        {
                //            result[n] = (n < temp.Length) ? temp[n] : (byte)0;
                //        }
                //    }
                //    break;
                //case DataType.Latitude:// 2009.9.30
                //    {
                //        result = new byte[this.Length];

                //        byte[] temp = Encoding.ASCII.GetBytes(value);

                //        for (int n = 0; n < result.Length; n++)
                //        {
                //            result[n] = (n < temp.Length) ? temp[n] : (byte)0;
                //        }
                //    }
                //    break;

                case DataType.Bytes:
                    if (this.Length > 0)
                    {
                        result = new byte[this.Length];

                        for (int n = 0; n < result.Length; n++)
                        {
                            result[n] = byte.Parse(value.Substring(n * 3 + 1, 2), NumberStyles.HexNumber);
                        }
                    }
                    else
                    {
                        result = new byte[0];
                    }
                    break;

                case DataType.IPv4:
                case DataType.IPv6:
                    {
                        string[] split = value.Split('.');

                        result = new byte[this.Length];

                        for (int n = 0; n < result.Length; n++)
                        {
                            result[n] = byte.Parse(split[n].Trim());
                        }
                    }
                    break;

                case DataType.Special:
                    break;

                default:
                    break;
            }

            return result;
        }

        // 本函数假定bytes与本对象的类型是匹配的，否则可能抛掷异常
        // 参数EnumObjectTable对于非枚举类型提供null即可
        public string GetString(byte[] data)
        {
            string result = string.Empty;

            if (data.Length < this.Length)
            {
                return result;
            }

            uint uintValue = uint.MaxValue;
            int intValue = int.MaxValue;

            // 预处理，简化后续分析过程
            switch (this.Type)
            {
                case DataType.Unsigned:
                case DataType.Att://2009.9.27
                case DataType.Hex:
                case DataType.Enum:
                case DataType.Bit:
                    switch (this.Length)
                    {
                        case 1:
                            uintValue = (uint)data[0];
                            break;

                        case 2:
                            uintValue = (uint)BitConverter.ToUInt16(data, 0);
                            break;

                        case 4:
                            uintValue = BitConverter.ToUInt32(data, 0);
                            break;

                        default:
                            break;
                    }
                    break;

                case DataType.Signed:
                    switch (this.Length)
                    {
                        case 1:
                            intValue = (int)((sbyte)data[0]);
                            break;

                        case 2:
                            intValue = (int)BitConverter.ToInt16(data, 0);
                            break;

                        case 4:
                            intValue = BitConverter.ToInt32(data, 0);
                            break;

                        default:
                            break;
                    }
                    break;

                default:
                    break;
            }

            switch (this.Type)
            {
                case DataType.Unsigned:
                case DataType.Att:
                    if ((this.Scale != 1) || (this.Offset != 0))
                    {
                        if (this.Scale != 1)
                        {
                            result = (((double)uintValue - (double)this.Offset) / (double)this.Scale).ToString();
                        }
                        else
                        {
                            result = ((Int64)uintValue - (Int64)this.Offset).ToString();
                        }
                    }
                    else
                    {
                        result = uintValue.ToString();
                    }
                    break;

                case DataType.Signed:
                    if ((this.Scale != 1) || (this.Offset != 0))
                    {
                        if (this.Scale != 1)
                        {
                            result = (((double)intValue - (double)this.Offset) / (double)this.Scale).ToString();
                        }
                        else
                        {
                            result = ((Int64)intValue - (Int64)this.Offset).ToString();
                        }
                    }
                    else
                    {
                        result = intValue.ToString();
                    }
                    break;

                case DataType.Hex:
                    result = string.Format("{0:X}", uintValue);
                    break;

                case DataType.Float:
                    switch (this.Length)
                    {
                        case 1:
                        case 2:
                        case 4:
                            result = BitConverter.ToSingle(data, 0).ToString();
                            break;

                        case 8:
                            result = BitConverter.ToDouble(data, 0).ToString();
                            break;

                        default:
                            break;
                    }
                    break;

                case DataType.Enum:
                    {
                        EnumParser enumParser = new EnumParser(EnumObjects);

                        enumParser.GetEnumValue(this.EnumName, uintValue, out result);
                    }
                    break;

                case DataType.String:
                case DataType.LongOrLatitude:
                case DataType.Longtitude:// 2009.9.30
                case DataType.Latitude:// 2009.9.30
                case DataType.Phone:
                    result = Encoding.ASCII.GetString(data).Trim();
                    break;
                case DataType.Date: // 2009.11.18
                    result = Encoding.ASCII.GetString(data).Trim();
                    break;

                case DataType.Bit:
                    result = (uintValue != 0) ? "1" : "0";
                    break;

                case DataType.IPv4:
                    result = string.Format("{0}.{1}.{2}.{3}",
                        data[0], data[1], data[2], data[3]);
                    break;

                case DataType.IPv6:
                    result = string.Format("{0}.{1}.{2}.{3}.{4}.{5}",
                        data[0], data[1], data[2], data[3], data[4], data[5]);
                    break;

                case DataType.Bytes:
                    if (this.Length > 0)
                    {
                        StringBuilder build = new StringBuilder(this.Length * 3 + 1);

                        build.Append('{');

                        foreach (byte element in data)
                        {
                            build.AppendFormat("{0:X2}.", element);
                        }

                        build.Remove(build.Length - 1, 1);
                        build.Append('}');

                        result = build.ToString();
                    }
                    else
                    {
                        result = "{}";
                    }
                    break;

                case DataType.Special:
                    break;

                default:
                    break;
            }

            return result;
        }

        public string GetTypeNotes()
        {
            string notes = "Type: Error";

            switch (this.Type)
            {
                case DataType.Unsigned:
                case DataType.Att:
                    switch (this.Length)
                    {
                        case 1: notes = string.Format("Type: byte; {0}~{1}", byte.MinValue, byte.MaxValue); break;
                        case 2: notes = string.Format("Type: ushort; {0}~{1}", ushort.MinValue, ushort.MaxValue); break;
                        case 4: notes = string.Format("Type: uint; {0}~{1}", uint.MinValue, uint.MaxValue); break;

                        default: break;
                    }

                    if ((this.Scale != 1) || (this.Offset != 0))
                    {
                        notes += string.Format("; scale={0}; offset={1}", this.Scale, this.Offset);
                    }
                    break;

                case DataType.Signed:
                    switch (this.Length)
                    {
                        case 1: notes = string.Format("sbyte: {0}~{1}", sbyte.MinValue, sbyte.MaxValue); break;
                        case 2: notes = string.Format("short: {0}~{1}", short.MinValue, short.MaxValue); break;
                        case 4: notes = string.Format("int: {0}~{1}", int.MinValue, int.MaxValue); break;

                        default: break;
                    }

                    if ((this.Scale != 1) || (this.Offset != 0))
                    {
                        notes += string.Format("; scale={0}; offset={1}", this.Scale, this.Offset);
                    }
                    break;

                case DataType.Enum:
                    switch (this.Length)
                    {
                        case 1: notes = "Type: enum(byte)"; break;
                        case 2: notes = "Type: enum(ushort)"; break;
                        case 4: notes = "Type: enum(uint)"; break;

                        default: break;
                    }
                    break;

                case DataType.Hex:
                    switch (this.Length)
                    {
                        case 1: notes = string.Format("Type: byte(hex mode); {0:X2}~{1:X2}", byte.MinValue, byte.MaxValue); break;
                        case 2: notes = string.Format("Type: ushort(hex mode); {0:X4}~{1:X4}", ushort.MinValue, ushort.MaxValue); break;
                        case 4: notes = string.Format("Type: uint(hex mode); {0:X8}~{1:X8}", uint.MinValue, uint.MaxValue); break;

                        default: break;
                    }
                    break;

                case DataType.Float:
                    switch (this.Length)
                    {
                        case 1: notes = string.Format("Type: float; {0}~{1}", byte.MinValue, byte.MaxValue); break;
                        case 2: notes = string.Format("Type: float; {0}~{1}", ushort.MinValue, ushort.MaxValue); break;
                        case 4: notes = string.Format("Type: float; {0}~{1}", float.MinValue, float.MaxValue); break;
                        case 8: notes = string.Format("Type: double; {0}~{1}", double.MinValue, double.MaxValue); break;

                        default: break;
                    }
                    break;

                case DataType.LongOrLatitude:
                    notes = string.Format("Type: {0}; Max. Length={1}", "longitude or latitude(ASCII)", this.Length);
                    break;

                case DataType.Longtitude:
                    notes = string.Format("Type: {0}; Max. Length={1}, Must start with 'E' or 'W', e.g. 'E103.12345678' ", "longitude", this.Length);
                    break;
                case DataType.Latitude:
                    notes = string.Format("Type: {0}; Max. Length={1}, Must start with 'N' or 'S', e.g. 'N23.12345678'", "latitude", this.Length);
                    break;

                case DataType.String:
                    notes = string.Format("Type: {0}; Max. Length={1}", "string(ASCII)", this.Length);
                    break;

                case DataType.Phone:
                    notes = string.Format("Type: {0}; Max. Length={1}; {2}", "phone", this.Length, "number string, may start with plus(+)");
                    break;

                case DataType.Date: // 2009.11.18 时间
                    notes = string.Format("Type: Date; e.g. '20091201235959'");
                    break;

                case DataType.Bit:
                    notes = "Type: bit; 0 or 1";
                    break;

                case DataType.IPv4:
                    notes = "Type: IP address(IPv4); such as 127.0.0.1";
                    break;

                case DataType.IPv6:
                    notes = "Type: IP address(IPv6); such as 127.0.0.0.0.1";
                    break;

                case DataType.Bytes:
                    notes = string.Format("Type: {0}; Length={1}; {2}", "byte stream", this.Length, "0x01,0x10,0xFF formatted as {01.10.FF}");
                    break;

                case DataType.Special:
                    notes = "Type: special; please refer to other document";
                    break;

                default:
                    notes = "Type: Unknown";
                    break;
            }

            return notes;
        }
    }

    // 根据指定的取值范围字符串进行相关分析
    public class RangeParser
    {
        private string sDataType;
        private string sDataRange;

        private EnumObject[] EnumObjects;

        public RangeParser(string dataType, string dataRange, EnumObject[] objects)
        {
            this.sDataType = (dataType != null) ? dataType.Trim() : string.Empty;
            this.sDataRange = (dataRange != null) ? dataRange.Trim() : string.Empty;
            this.EnumObjects = (objects != null) ? objects : new EnumObject[0];
        }

        // 判断value是否是type和range限定下的有效值
        //public bool TryParse(string value)
        public bool TryParse(string value, string deviceType) // 2009.10.21 添加设备类型区分Att
        {

            TypeParser typeParser = new TypeParser(this.sDataType, this.EnumObjects);
            if (typeParser.Type == DataType.Att)
            {
                if (deviceType == "5")
                {
                    //this.sDataRange = "Attenuation:0~20";
                    this.sDataRange = "Attenuation:0~31";
                }
                else
                {
                    this.sDataRange = "Attenuation:0~31";
                }
            }//////////////////////////////////////////////////
            if (typeParser.Type == DataType.Longtitude)
            {
                this.sDataRange = "0~180";
            }//////////////////////////////////////////////////
            if (typeParser.Type == DataType.Latitude)
            {
                this.sDataRange = "0~90";
            }//////////////////////////////////////////////////
            if (!typeParser.TryParse(value))
            {
                return false;
            }

            if (string.IsNullOrEmpty(this.sDataRange))
            {
                return true;
            }

            string[] betweens = this.sDataRange.Split(';', ',', '\r', '\n');

            bool result = true;

            // 预处理
            if (typeParser.Type == DataType.Enum)
            {
                EnumParser enumParser = new EnumParser(EnumObjects);

                uint enumKey = 0;

                enumParser.GetEnumKey(typeParser.EnumName, value, out enumKey);

                value = enumKey.ToString();
            }

            switch (typeParser.Type)
            {
                case DataType.Unsigned:

                case DataType.Signed:
                case DataType.Bit:
                case DataType.Hex:
                case DataType.Float:
                case DataType.Enum:
                    {
                        double doubleValue = double.MaxValue;

                        if (typeParser.Type != DataType.Hex)
                        {
                            doubleValue = double.Parse(value);
                        }
                        else
                        {
                            uint uintValue = uint.Parse(value, NumberStyles.HexNumber);
                            doubleValue = (double)uintValue;
                        }

                        result = false;

                        foreach (string between in betweens)
                        {
                            try
                            {
                                if (InternalPublic.IsBetween(doubleValue, between))
                                {
                                    result = true;
                                }
                            }
                            catch (Exception)
                            {
                                // do nothing
                            }
                        }
                    }
                    break;
                case DataType.Att://####################################################################
                    if (deviceType == "5")
                    {
                        //if ((uint.Parse(value) >= 0) && (uint.Parse(value) <= 20))
                        if ((uint.Parse(value) >= 0) && (uint.Parse(value) <= 31))
                        {
                            result = true;
                        }
                        else
                        {
                            result = false;
                        }
                    }
                    else
                    {
                        if ((uint.Parse(value) >= 0) && (uint.Parse(value) <= 31))
                        {
                            result = true;
                        }
                        else
                        {
                            result = false;
                        }
                    }
                    break;

                case DataType.String:
                case DataType.Phone:
                case DataType.LongOrLatitude:
                case DataType.Longtitude:// 2009.9.30
                case DataType.Latitude:// 2009.9.30
                    result = (!string.IsNullOrEmpty(value) || (this.sDataRange.IndexOf("nonempty") < 0));
                    break;
                case DataType.Date:
                    result = (!string.IsNullOrEmpty(value) || (this.sDataRange.IndexOf("nonempty") < 0));
                    break;

                case DataType.Bytes:
                    {
                        result = true;

                        byte byteValue = byte.MinValue;

                        int bytesLength = typeParser.Length;

                        for (int n = 0; n < bytesLength; n++)
                        {
                            result = false;

                            byteValue = byte.Parse(value.Substring(n * 3 + 1, 2), NumberStyles.HexNumber);

                            foreach (string between in betweens)
                            {
                                try
                                {
                                    if (InternalPublic.IsBetween((double)byteValue, between))
                                    {
                                        result = true;
                                    }
                                }
                                catch (Exception)
                                {
                                    // do nothing
                                }
                            }

                            if (!result)
                            {
                                break;
                            }
                        }
                    }
                    break;

                case DataType.IPv4:
                case DataType.IPv6:
                case DataType.Special:
                    break;

                default:
                    break;
            }

            return result;
        }

        // 2010.10.14
        public bool TryParse(string value, string deviceType, string fieldName) // 2009.10.21 添加设备类型区分Att
        {

            if (fieldName.Equals("Site ID"))
            {
                if (value.Trim().Equals("0"))
                {
                    this.sDataRange = string.Format("Device No.:1~{0}", UInt32.MaxValue);
                }
            }

            TypeParser typeParser = new TypeParser(this.sDataType, this.EnumObjects);
            if (typeParser.Type == DataType.Att)
            {
                if (deviceType == "5")
                {
                    //this.sDataRange = "Attenuation:0~20";
                    this.sDataRange = "Attenuation:0~31";
                }
                else
                {
                    this.sDataRange = "Attenuation:0~31";
                }
            }//////////////////////////////////////////////////
            if (typeParser.Type == DataType.Longtitude)
            {
                this.sDataRange = "0~180";
            }//////////////////////////////////////////////////
            if (typeParser.Type == DataType.Latitude)
            {
                this.sDataRange = "0~90";
            }//////////////////////////////////////////////////
            if (!typeParser.TryParse(value))
            {
                return false;
            }
            //2010.10.14
            if (fieldName == "DL PA VSWR Limit")
            {
                this.sDataRange = "1.5~2.5, MUST have one decimal place";
            }
            //if (fieldName.Contains("DownLink SWR Threshold"))
            //{
            //    this.sDataRange = "1.5~2.5, MUST have one decimal place";
            //}
            if (string.IsNullOrEmpty(this.sDataRange))
            {
                return true;
            }

            string[] betweens = this.sDataRange.Split(';', ',', '\r', '\n');

            bool result = true;

            // 预处理
            if (typeParser.Type == DataType.Enum)
            {
                EnumParser enumParser = new EnumParser(EnumObjects);

                uint enumKey = 0;

                enumParser.GetEnumKey(typeParser.EnumName, value, out enumKey);

                value = enumKey.ToString();
            }

            switch (typeParser.Type)
            {
                case DataType.Unsigned:

                case DataType.Signed:
                case DataType.Bit:
                case DataType.Hex:
                case DataType.Float:
                case DataType.Enum:
                    {

                        //2010.10.14
                        //if ((fieldName == "DL PA VSWR Limit")||(fieldName == "DownLink SWR Threshold"))
                        if (fieldName == "DL PA VSWR Limit")
                        {
                            //this.sDataRange = "1.5~2.5";
                            this.sDataRange = "1.5~2.5, MUST have one decimal place";


                            double doubleValue = double.MaxValue;

                            try
                            {
                                if (Regex.IsMatch(value.Trim(), @"^[0-9]+(\.[0-9]{1})?$"))
                                {
                                    if (double.TryParse(value, out doubleValue))
                                    {
                                        if ((doubleValue - 1.4 > 0) && (doubleValue - 2.6 < 0))
                                        {
                                            result = true;
                                        }
                                        else
                                        {
                                            result = false;
                                        }
                                    }
                                }
                                else
                                {
                                    result = false;
                                }


                            }
                            catch (Exception)
                            {
                                // do nothing
                            }
                        }
                        else
                        {
                            double doubleValue = double.MaxValue;

                            if (typeParser.Type != DataType.Hex)
                            {
                                doubleValue = double.Parse(value);
                            }
                            else
                            {
                                uint uintValue = uint.Parse(value, NumberStyles.HexNumber);
                                doubleValue = (double)uintValue;
                            }

                            result = false;

                            foreach (string between in betweens)
                            {
                                try
                                {
                                    if (InternalPublic.IsBetween(doubleValue, between))
                                    {
                                        result = true;
                                    }
                                }
                                catch (Exception)
                                {
                                    // do nothing
                                }
                            }
                        }
                    }
                    break;
                case DataType.Att://####################################################################
                    if (deviceType == "5")
                    {
                        if ((uint.Parse(value) >= 0) && (uint.Parse(value) <= 31))
                        {
                            result = true;
                        }
                        else
                        {
                            result = false;
                        }
                    }
                    else
                    {
                        if ((uint.Parse(value) >= 0) && (uint.Parse(value) <= 31))
                        {
                            result = true;
                        }
                        else
                        {
                            result = false;
                        }
                    }
                    break;

                case DataType.String:
                case DataType.Phone:
                case DataType.LongOrLatitude:
                case DataType.Longtitude:// 2009.9.30
                case DataType.Latitude:// 2009.9.30
                    result = (!string.IsNullOrEmpty(value) || (this.sDataRange.IndexOf("nonempty") < 0));
                    break;
                case DataType.Date:
                    result = (!string.IsNullOrEmpty(value) || (this.sDataRange.IndexOf("nonempty") < 0));
                    break;

                case DataType.Bytes:
                    {
                        result = true;

                        byte byteValue = byte.MinValue;

                        int bytesLength = typeParser.Length;

                        for (int n = 0; n < bytesLength; n++)
                        {
                            result = false;

                            byteValue = byte.Parse(value.Substring(n * 3 + 1, 2), NumberStyles.HexNumber);

                            foreach (string between in betweens)
                            {
                                try
                                {
                                    if (InternalPublic.IsBetween((double)byteValue, between))
                                    {
                                        result = true;
                                    }
                                }
                                catch (Exception)
                                {
                                    // do nothing
                                }
                            }

                            if (!result)
                            {
                                break;
                            }
                        }
                    }
                    break;

                case DataType.IPv4:
                case DataType.IPv6:
                case DataType.Special:
                    break;

                default:
                    break;
            }

            return result;
        }

        public string GetRangeNotes(bool rangeOnly)
        {
            string rangeNotes = string.Empty;

            if (string.IsNullOrEmpty(this.sDataRange))
            {
                rangeNotes = "Range: no limit";
            }
            else
            {
                rangeNotes = "Range: " + this.sDataRange;
            }

            string notes = string.Empty;

            if (rangeOnly)
            {
                notes = rangeNotes;
            }
            else
            {
                TypeParser typeParser = new TypeParser(this.sDataType, this.EnumObjects);

                notes = string.Format("{0}\r\n{1}", typeParser.GetTypeNotes(), rangeNotes);
            }

            return notes;
        }
    }

    public class EnumObjectList : ObservableCollection<EnumObject>
    {
        public EnumObjectList()
        {
            CreateEnumObjectList();
        }

        private void CreateEnumObjectList()
        {
            Add(new EnumObject("YesNo", 0x00, "No"));
            Add(new EnumObject("YesNo", 0x01, "Yes"));
            Add(new EnumObject("CommunicationMode", 0x01, "Short Message"));
            Add(new EnumObject("CommunicationMode", 0x02, "Mobile Data(CSD)"));
            Add(new EnumObject("CommunicationMode", 0x03, "GPRS"));
            Add(new EnumObject("CommunicationMode", 0x04, "CDMA"));
            Add(new EnumObject("CommunicationMode", 0x05, "UDP"));
            Add(new EnumObject("EnableDisable", 0x00, "Disable"));
            Add(new EnumObject("EnableDisable", 0x01, "Enbale"));
            Add(new EnumObject("AlarmNormal", 0x00, "Normal"));
            Add(new EnumObject("AlarmNormal", 0x01, "Alarm"));
            Add(new EnumObject("OnOff", 0x00, "Off"));
            Add(new EnumObject("OnOff", 0x01, "On"));
        }
    }
}
