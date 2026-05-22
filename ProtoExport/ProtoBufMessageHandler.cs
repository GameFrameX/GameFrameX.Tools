namespace GameFrameX.ProtoExport;

public static class ProtoBufMessageHandler
{
    public static void Start(LauncherOptions launcherOptions, ModeType modeType)
    {
        // 先验证输入参数，再删除输出目录
        if (string.IsNullOrWhiteSpace(launcherOptions.InputPath) || !Directory.Exists(launcherOptions.InputPath))
        {
            throw new DirectoryNotFoundException($"协议文件路径不存在: {launcherOptions.InputPath}");
        }

        IProtoGenerateHelper protoGenerateHelper = null;
        var types = typeof(IProtoGenerateHelper).Assembly.GetTypes();
        foreach (var type in types)
        {
            var attrs = type.GetCustomAttributes(typeof(ModeAttribute), true);
            if (attrs?.Length > 0 && (attrs[0] is ModeAttribute modeAttribute) && modeAttribute.Mode == modeType)
            {
                protoGenerateHelper = (IProtoGenerateHelper)Activator.CreateInstance(type);
                break;
            }
        }

        if (protoGenerateHelper == null)
        {
            throw new NotSupportedException($"不支持的模式类型: {modeType}。当前支持的模式: {string.Join(", ", Enum.GetNames<ModeType>())}");
        }

        protoGenerateHelper.Init(launcherOptions);

        // 参数验证通过后再清理并创建输出目录
        var outputDirectoryInfo = new DirectoryInfo(launcherOptions.OutputPath);
        if (outputDirectoryInfo.Exists)
        {
            outputDirectoryInfo.Delete(true);
        }

        outputDirectoryInfo.Create();

        launcherOptions.OutputPath = outputDirectoryInfo.FullName;

        var files = Directory.GetFiles(launcherOptions.InputPath, "*.proto", SearchOption.AllDirectories);

        var messageInfoLists = new List<MessageInfoList>(files.Length);

        foreach (var file in files)
        {
            var fileName = Path.GetFileNameWithoutExtension(file);

            // 跳过服务器内部协议文件（_s/-s 后缀），客户端构建不处理
            var isServerOnly = fileName.EndsWith("-s") || fileName.EndsWith("_s");
            if (!launcherOptions.IsServer && isServerOnly)
            {
                continue;
            }

            var operationCodeInfo = MessageHelper.Parse(File.ReadAllText(file), fileName, launcherOptions.OutputPath, launcherOptions.IsGenerateErrorCode);

            if (launcherOptions.CommentValidation != CommentValidationLevel.None)
            {
                CommentValidator.Validate(operationCodeInfo, launcherOptions.CommentValidation);
            }

            messageInfoLists.Add(operationCodeInfo);

            protoGenerateHelper.Run(operationCodeInfo, launcherOptions.OutputPath, launcherOptions.NamespaceName);
        }

        protoGenerateHelper.Post(messageInfoLists, launcherOptions);
    }
}
