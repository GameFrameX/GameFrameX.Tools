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
        var skippedCount = 0;

        foreach (var file in files)
        {
            var fileName = Path.GetFileNameWithoutExtension(file);

            // 客户端构建跳过服务器内部协议文件（_s/-s 后缀），仅服务器导出
            var isServerOnly = fileName.EndsWith("-s") || fileName.EndsWith("_s");
            if (!launcherOptions.IsServer && isServerOnly)
            {
                Console.WriteLine($"[SKIP] 客户端构建跳过服务器内部协议文件（_s/-s 后缀）: {fileName}");
                skippedCount++;
                continue;
            }

            var operationCodeInfo = MessageHelper.Parse(File.ReadAllText(file), fileName, launcherOptions.OutputPath, launcherOptions.IsGenerateErrorCode);

            // 客户端构建跳过模块 id 小于 0 的内部协议（如 Inner*），仅服务器导出
            if (!launcherOptions.IsServer && operationCodeInfo.Module < 0)
            {
                Console.WriteLine($"[SKIP] 客户端构建跳过内部协议（moduleId={operationCodeInfo.Module} < 0）: {fileName}");
                skippedCount++;
                continue;
            }

            if (launcherOptions.CommentValidation != CommentValidationLevel.None)
            {
                CommentValidator.Validate(operationCodeInfo, launcherOptions.CommentValidation);
            }

            messageInfoLists.Add(operationCodeInfo);

            protoGenerateHelper.Run(operationCodeInfo, launcherOptions.OutputPath, launcherOptions.NamespaceName);
        }

        Console.WriteLine($"协议扫描完成: 共发现 {files.Length} 个 .proto 文件，导出 {messageInfoLists.Count} 个，跳过 {skippedCount} 个（模式: {(launcherOptions.IsServer ? "服务器" : "客户端")}）");

        protoGenerateHelper.Post(messageInfoLists, launcherOptions);
    }
}
