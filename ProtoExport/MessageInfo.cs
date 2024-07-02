namespace GameFrameX.ProtoExport
{
    public class MessageInfoList
    {
        /// <summary>
        /// 消息模块ID
        /// </summary>
        public ushort Module { get; set; }

        /// <summary>
        /// 消息列表
        /// </summary>
        public List<MessageInfo> Infos { get; set; } = new List<MessageInfo>(32);

        /// <summary>
        /// 输出路径
        /// </summary>
        public string OutputPath { get; set; }
    }

    /// <summary>
    /// 消息码信息
    /// </summary>
    public class MessageInfo
    {
        private string _name;

        public MessageInfo(bool isEnum = false) : this()
        {
            IsEnum = isEnum;
        }

        private MessageInfo()
        {
            Fields = new List<MessageMember>();
            Description = string.Empty;
        }


        /// <summary>
        /// 是否是请求
        /// </summary>
        public bool IsRequest { get; private set; }

        /// <summary>
        /// 是否是响应
        /// </summary>
        public bool IsResponse { get; private set; }

        /// <summary>
        /// 是否是通知
        /// </summary>
        public bool IsNotify { get; private set; }

        /// <summary>
        /// 是否是心跳
        /// </summary>
        public bool IsHeartbeat { get; private set; }

        /// <summary>
        /// 是否是消息
        /// </summary>
        public bool IsMessage => IsRequest || IsResponse || IsNotify;

        /// <summary>
        /// 父类
        /// </summary>
        public string ParentClass
        {
            get
            {
                if (IsEnum)
                {
                    return string.Empty;
                }

                string parentClass = string.Empty;
                if (IsRequest)
                {
                    parentClass = "IRequestMessage";
                }
                else if (IsResponse)
                {
                    parentClass = "IResponseMessage";
                }
                else if (IsNotify)
                {
                    parentClass = "INotifyMessage";
                }

                if (IsHeartbeat && !string.IsNullOrEmpty(parentClass))
                {
                    parentClass += ", IHeartBeatMessage";
                }

                return parentClass;
            }
        }


        /// <summary>
        /// 消息名称，用于请求和相应配对
        /// </summary>
        public string MessageName { get; private set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                IsRequest = Name.StartsWith("Req") || Name.StartsWith("C2S_");
                IsNotify = Name.StartsWith("Notify");
                IsHeartbeat = Name.Contains("Heartbeat", StringComparison.OrdinalIgnoreCase);
                IsResponse = Name.StartsWith("Resp") || Name.StartsWith("S2C_") || IsNotify || IsHeartbeat;
                if (IsRequest)
                {
                    if (Name.StartsWith("Req"))
                    {
                        MessageName = Name[3..];
                    }
                    else if (Name.StartsWith("C2S_"))
                    {
                        MessageName = Name[4..];
                    }
                }
                else
                {
                    if (Name.StartsWith("Resp") || Name.StartsWith("S2C_"))
                    {
                        MessageName = Name[4..];
                    }
                }
            }
        }

        /// <summary>
        /// 操作码
        /// </summary>
        public int Opcode { get; set; }

        /// <summary>
        /// 是否是枚举
        /// </summary>
        public bool IsEnum { get; set; }

        /// <summary>
        /// 字段
        /// </summary>
        public List<MessageMember> Fields { get; set; }

        /// <summary>
        /// 注释
        /// </summary>
        public string Description { get; set; }
    }

    public class MessageMember
    {
        private int _members;

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 是否有效
        /// </summary>
        public bool IsValid
        {
            get { return !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Type); }
        }

        /// <summary>
        /// 字段类型
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 注释
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// 成员编码
        /// </summary>
        public int Members
        {
            get => _members;
            set
            {
                _members = value;
                if (value >= 800 && Name != "ErrorCode")
                {
                    throw new Exception("成员编码不能大于800");
                }
            }
        }

        /// <summary>
        /// 是否是重复
        /// </summary>
        public bool IsRepeated { get; set; }

        /// <summary>
        /// 是否是kv键值对
        /// </summary>
        public bool IsKv { get; set; }
    }
}