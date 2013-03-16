using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nms_datatype_lib;

namespace nms_usercontrol_libs.src
{
    public class DataGridModel : ObservableCollection<SiteModel>
    {
        public DataGridModel()
        {
           
        }

        public DataGridModel(int multiplier)
        {
            
        }

        public void CreateGenericSiteModelData(SiteModel siteModel)
        {
            Add(siteModel);
        }

        public void CreateGenericSiteModelData(int _id, bool _only, ushort _moid, int _length, string _name, string _unit, string _type)
        {
            Add(new SiteModel(_id, _only, _moid, _length, _name, _unit, _type));
        }

        public void CreateGenericSiteModelData(int _id, bool _only, ushort _moid, int _length, string _name, string _unit, string _type, object _tag)
        {
            Add(new SiteModel(_id, _only, _moid, _length, _name, _unit, _type, _tag));
        }
    }

    public class SiteModel : INotifyPropertyChanged, ICloneable, IEditableObject
    {
        #region Private Fields
        private SiteModel copy;
        private int id;
        private int length;
        private bool select;
        private bool only;
        private ushort moid;
        private string name;       
        private string query;
        private string status;
        private string unit;
        private string type;
        private byte[] data;
        private object tag;
        private object _value;
        #endregion

        #region Constructors
        public SiteModel()
        {
        }

        public SiteModel(ushort _moid, int _length, byte[] _data)
        {
            this.moid = _moid;
            this.length = _length;
            this.data = new byte[_length];

            for (int n = 0; n < _length; n++)
            {
                data[n] = _data[n];
            }
        }

        public SiteModel(int _id, bool _only, ushort _moid, int _length, string _name, string _unit, string _type)
        {
            this.id = _id;
            this.only = _only;
            this.name = _name;
            this.moid = _moid;
            this.unit = _unit;
            this.type = _type;
            this.tag = null;
            this._value = null;
            this.length = _length;
            this.query = string.Empty;
            this.data = new byte[_length];
        }

        public SiteModel(int _id, bool _only, ushort _moid, int _length, string _name,string _unit, string _type, object _tag)
        {
            this.id = _id;
            this.only = _only;
            this.name = _name;
            this.moid = _moid;
            this.unit = _unit;
            this.type = _type;
            this.tag = _tag;
            this._value = null;
            this.length = _length;
            this.query = string.Empty;
            this.data = new byte[_length];
        }
        #endregion

        #region Public Members
        public override string ToString()
        {
            return string.Format("moid: 0x{0:X4}--name: {1}", this.moid, this,name);
        }

        public void CopyFrom(SiteModel other)
        {
            this.id = other.id;
            this.length = other.length;
            this.select = other.select;
            this.only = other.only;
            this.moid = other.moid;
            this.name = other.name;
            this._value = other._value;
            this.query = other.query;
            this.status = other.status;
            this.unit = other.unit;
            this.type = other.type;
            this.tag = other.tag;
        }
        #endregion

        #region Public Properties
        public bool Select
        {
            get { return select; }
            set
            {
                select = value;
                OnPropertyChanged("Select");
            }
        }

        public ushort Moid
        {
            get { return moid; }
            set
            {
                moid = value;
                OnPropertyChanged("Moid");
            }
        }

        public int Length
        {
            get { return length; }
        }

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }

        public string Unit
        {
            get { return unit; }
            set
            {
                unit = value;
                OnPropertyChanged("Unit");
            }
        }

        public string Status
        {
            get { return status; }
            set
            {
                status = value;
                OnPropertyChanged("Status");
            }
        }

        public string Value
        {
            get { return _value.ToString(); }
            set
            {
                _value = value;
                ToBytes();
                OnPropertyChanged("Value");
            }
        }

        public string Query
        {
            get { return query; }
            set
            {
                query = value;
                OnPropertyChanged("Query");
            }
        }

        public object Tag
        {
            get { return tag; }
            set { tag = value; }
        }

        public byte[] Data
        {
            get { return data; }
        }

        public string Type
        {
            get { return type; }
        }
        #endregion

        #region Private Members
        /// <summary>
        /// DataType转换为枚举类型
        /// </summary>
        /// <param name="strValue"></param>
        /// <returns></returns>
        private string DataTypeToEnumName(string strValue)
        {
            int nStartIndex = strValue.IndexOf("<");
            int nEndIndex = strValue.IndexOf(">");

            return strValue.Substring(nStartIndex + 1, (nEndIndex - nStartIndex - 1));
        }
        // 0~9 => '0'~'9'; 10~15 => 'A'~'F'
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

        // '0'~'9' => 0~9; 'A'~'F' => 10~15
        private bool CharToHalfByte(char input, out byte output)
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

        // 字节拆分(16进制显示)，输出第1个字符(hiChar)和第2个字符(loChar)
        public void SplitByte(byte data, out char hiChar, out char loChar)
        {
            byte hiHalfByte = (byte)((data >> 4) & 0x0F);
            byte loHalfByte = (byte)((data >> 0) & 0x0F);

            HalfByteToChar(hiHalfByte, out hiChar);
            HalfByteToChar(loHalfByte, out loChar);
        }

        // 字节拆分的反向处理
        public bool CombineByte(char hiChar, char loChar, out byte data)
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
        /// GetDateBytes
        /// </summary>
        /// <param name="strDate"></param>
        /// <returns></returns>
        private byte[] GetDateBytes(String strDate)
        {
            char[] chars = strDate.ToCharArray();
            List<byte> byteList = new List<byte>();

            for (int i = 0; i < chars.Length; i += 2)
            {
                byte byteData;
                CombineByte(chars[i], chars[i + 1], out byteData);
                byteList.Add(byteData);
            }
            return byteList.ToArray();
        }
        /// <summary>
        /// GetIPToByte
        /// </summary>
        /// <param name="strIPv4"></param>
        /// <returns></returns>
        private byte[] GetIPToByte(string strIPv4)
        {
            byte[] result = new byte[4];

            int index = 0;

            string[] IPn = strIPv4.Split('.');

            byte byteValue = byte.MaxValue;

            foreach (string s in IPn)
            {
                if (byte.TryParse(s.Trim(), out byteValue))
                {
                    result[index] = byteValue;
                    index++;
                }
            }

            return result;
        }

        private void ToBytes()
        {
            int xResult = 0;
            uint uResult = 0;
            float fResult = 0.0f;

            byte[] datas = null;
            EnumObjectList enumObjectList = new EnumObjectList();
            EnumParser parser = new EnumParser(enumObjectList.ToArray());
            TypeParser typeParser = new TypeParser(type, enumObjectList.ToArray());

            switch (typeParser.Type)
            {
                case DataType.Enum:
                    string EnumName = DataTypeToEnumName(type);

                    uint nKey = 0;
                    if (parser.GetEnumKey(EnumName, Value, out nKey))
                    {
                        data[0] = (byte)nKey;
                    }
                    break;

                case DataType.String:
                case DataType.LongOrLatitude:
                case DataType.Longtitude:
                case DataType.Latitude:
                case DataType.Phone:
                    datas = Encoding.ASCII.GetBytes(Value);
                    length = (datas.Length < length) ? datas.Length : length;
                    Array.ConstrainedCopy(datas, 0, data, 0, length);
                    break;

                case DataType.Date:
                    datas = GetDateBytes(Value);
                    length = (datas.Length < length) ? datas.Length : length;
                    Array.ConstrainedCopy(datas, 0, data, 0, length);
                    break;

                case DataType.Unsigned:
                    if (uint.TryParse(Value, out uResult))
                    {
                        datas = BitConverter.GetBytes(uResult);
                        Array.ConstrainedCopy(datas, 0, data, 0, length);
                    }
                    break;

                case DataType.IPv4:
                    data = GetIPToByte(Value);
                    break;

                case DataType.Att:
                    if (uint.TryParse(Value, out uResult))
                    {
                        datas = BitConverter.GetBytes(uResult);
                        Array.ConstrainedCopy(datas, 0, data, 0, length);
                    }
                    break;
                case DataType.Signed:
                    if (int.TryParse(Value, out xResult))
                    {
                        datas = BitConverter.GetBytes(xResult);
                        Array.ConstrainedCopy(datas, 0, data, 0, length);
                    }
                    break;

                case DataType.Unknown:     //在此定义为频率类型                   
                    break;

                case DataType.Bytes:
                    int index = 0;
                    byte nByte = 0;
                    string tmpString = Value.Trim(new char[2] { '{', '}' });
                    string[] tmpBytes = tmpString.Split(new char[1] { '.' });
                    foreach (string str in tmpBytes)
                    {
                        nByte = Convert.ToByte(str, 16);
                        data[index++] = nByte;
                    }
                    break;

                //参数是浮点型数据类型
                case DataType.Float:                   
                    if (float.TryParse(Value, out fResult))
                    {
                        fResult = fResult * 10;
                        uResult = (uint)(fResult);
                        datas = BitConverter.GetBytes(uResult);
                        Array.ConstrainedCopy(datas, 0, data, 0, length);
                    }
                    break;

                default:
                    break;
            }
        }

        private string DisplayDatetime(byte[] input)
        {
            byte[] temp = input;
            string result = string.Empty;

            foreach (byte b in temp)
            {
                int data = int.Parse(b.ToString(), System.Globalization.NumberStyles.Integer);
                string strData = Convert.ToString(data, 16);
                string strTemp = string.Empty;

                if (data < 10)
                {
                    strTemp = string.Format("0{0}", strData);
                }
                else
                {
                    strTemp = strData;
                }

                result += strTemp;
            }

            string display;

            display = string.Format("{0}-{1}-{2} {3}:{4}:{5}", result.Substring(0, 4), result.Substring(4, 2), result.Substring(6, 2), result.Substring(8, 2), result.Substring(10, 2), result.Substring(12, 2));
            return display;
        }
        private string GetByteToIP(byte[] input)
        {
            string strIP = string.Empty;
            strIP = string.Format("{0}.{1}.{2}.{3}", input[0], input[1], input[2], input[3]);
            return strIP;
        }

        private string BytesToString()
        {
            EnumObjectList enumObjectList = new EnumObjectList();
            EnumParser parser = new EnumParser(enumObjectList.ToArray());
            TypeParser typeParser = new TypeParser(type, enumObjectList.ToArray());

            uint nByte = 0;
            byte[] nBytes = new byte[4];
            string strValue = string.Empty;

            switch (typeParser.Type)
            {
                case DataType.Enum:
                    nByte = data[0];
                    parser.GetEnumValue(DataTypeToEnumName(type), nByte, out strValue);
                    break;

                case DataType.Phone:
                case DataType.LongOrLatitude:
                case DataType.Longtitude:
                case DataType.Latitude:    
                case DataType.String:
                    strValue = Encoding.ASCII.GetString(data);
                    strValue = strValue.Replace("\0", "");
                    break;

                case DataType.Date: 
                    strValue = DisplayDatetime(data);
                    strValue = strValue.Replace("\0", "");
                    break;

                case DataType.Unsigned:
                    {
                        uint nValue = 0;
                        switch (length)
                        {
                            case 4:
                                nValue = BitConverter.ToUInt32(data, 0);
                                strValue = nValue.ToString();
                                break;
                            case 2:
                                nValue = (uint)BitConverter.ToUInt16(data, 0);
                                strValue = nValue.ToString();
                                break;
                            case 1:
                                nValue = (uint)data[0];
                                strValue = nValue.ToString();
                                break;
                        }
                        strValue = strValue.Replace("\0", "");
                    }
                    break;

                case DataType.IPv4:
                    strValue = Encoding.ASCII.GetString(data);
                    strValue = GetByteToIP(data);

                    strValue = strValue.Replace("\0", "");
                    break;
                case DataType.Att:
                    {
                        uint nValue = 0;
                        switch (length)
                        {
                            case 4:
                                nValue = BitConverter.ToUInt32(data, 0);
                                break;
                            case 2:
                                nValue = (uint)BitConverter.ToUInt16(data, 0);
                                break;
                            case 1:
                                nValue = (uint)data[0];
                                break;
                        }
                        strValue = nValue.ToString();
                        strValue = strValue.Replace("\0", "");
                    }
                    break;

                case DataType.Signed:
                    {
                        int nValue = 0;
                     
                        switch (length)
                        {
                            case 4:
                                nValue = BitConverter.ToInt32(data, 0);
                                break;
                            case 2:
                                nValue = (int)BitConverter.ToInt16(data, 0);
                                break;
                            case 1:
                                nValue = int.Parse(SByte.Parse(Convert.ToString(data[0], 16), System.Globalization.NumberStyles.HexNumber).ToString());//2009.9.28 signed 
                                break;
                        }

                        if ((moid == 0x0502)
                        ||  (moid == 0x0503)
                        ||  (moid == 0x0505)
                        ||  (moid == 0x050D)
                        ||  (moid == 0x0A70)
                        ||  (moid == 0x0A71))
                        {
                            if (nValue == 125) //7D
                            {
                                strValue = "--";
                            }
                            else if (nValue == 126) //7E 
                            {
                                strValue = "Low";
                            }
                            else if (nValue == 127) //7F
                            {
                                strValue = "High";
                            }
                            else
                            {
                                strValue = nValue.ToString();
                            }
                        }
                        else
                        {
                            strValue = nValue.ToString();
                        }
                       
                        strValue = strValue.Replace("\0", "");
                    }
                    break;
                case DataType.Unknown:
                    short chNum = BitConverter.ToInt16(data, 0);
                    strValue = chNum.ToString();
                    strValue = strValue.Replace("\0", "");
                    break;

                case DataType.Bytes:
                    {
                        foreach (byte b in data)
                        {
                            if (b < 0x10)
                            {
                                strValue += string.Format("0{0:X2}.", Convert.ToString(b, 16).ToUpper());
                            }
                            else
                            {
                                strValue += string.Format("{0:X2}.", Convert.ToString(b, 16).ToUpper());
                            }
                        }
                        strValue = strValue.TrimEnd(new char[1] { '.' });
                        strValue = strValue.Replace("\0", "");
                        strValue = "{" + strValue + "}";
                    }
                    break;

                case DataType.Float:
                    float fResult = 0.0F;
                    switch (length)
                    {
                        case 1:
                            fResult = (float)data[0];
                            fResult = fResult / 10;
                            strValue = fResult.ToString();
                            break;
                        case 2:
                            fResult = (float)BitConverter.ToUInt16(data, 0);
                            fResult = fResult / 10;
                            strValue = fResult.ToString();
                            break;
                        case 4:
                            fResult = (float)BitConverter.ToUInt16(data, 0);
                            fResult = fResult / 10;
                            strValue = fResult.ToString();
                            break;
                        case 8:
                            break;
                        default:
                            break;
                    }
                    break;

                default:
                    break;
            }

            return strValue;
        }
        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion INotifyPropertyChanged

        #region ICloneable
        public object Clone()
        {
            SiteModel siteModel = (SiteModel)this.MemberwiseClone();

            return siteModel;
        }
        #endregion ICloneable

        #region IEditableObject Members
        public void BeginEdit()
        {
            if (this.copy == null)
            {
                this.copy = new SiteModel();
            }

            this.copy = (SiteModel)Clone();
        }

        public void CancelEdit()
        {
            this.CopyFrom(this.copy);
        }

        public void EndEdit()
        {
            this.copy = null;
        }
        #endregion
    }
}
