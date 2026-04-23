namespace GameFrameX.ProtoExport;

public static class ProtoBufMessageHandler
{
    private static IProtoGenerateHelper _protoGenerateHelper;

    public static void Start(LauncherOptions launcherOptions, ModeType modeType)
    {
        var outputDirectoryInfo = new DirectoryInfo(launcherOptions.OutputPath);
        if (outputDirectoryInfo.Exists)
        {
            outputDirectoryInfo.Delete(true);
        }

        outputDirectoryInfo.Create();

        launcherOptions.OutputPath = outputDirectoryInfo.FullName;

        var types = typeof(IProtoGenerateHelper).Assembly.GetTypes();
        foreach (var type in types)
        {
            var attrs = type.GetCustomAttributes(typeof(ModeAttribute), true);
            if (attrs?.Length > 0 && (attrs[0] is ModeAttribute modeAttribute) && modeAttribute.Mode == modeType)
            {
                _protoGenerateHelper = (IProtoGenerateHelper)Activator.CreateInstance(type);
                break;
            }
        }

        var files = Directory.GetFiles(launcherOptions.InputPath, "*.proto", SearchOption.AllDirectories);

        var messageInfoLists = new List<MessageInfoList>(files.Length);
        foreach (var file in files)
        {
            var fileName = Path.GetFileNameWithoutExtension(file);
            var operationCodeInfo = MessageHelper.Parse(File.ReadAllText(file), fileName, launcherOptions.OutputPath, launcherOptions.IsGenerateErrorCode);
            messageInfoLists.Add(operationCodeInfo);
            switch (modeType)
            {
                case ModeType.Server:
                {
                    _protoGenerateHelper?.Run(operationCodeInfo, launcherOptions.OutputPath, launcherOptions.NamespaceName);
                }
                    break;
                case ModeType.Unity:
                {
                    if (fileName.EndsWith("-s") || fileName.EndsWith("_s"))
                    {
                        continue;
                    }

                    _protoGenerateHelper?.Run(operationCodeInfo, launcherOptions.OutputPath, launcherOptions.NamespaceName);
                }
                    break;
                case ModeType.TypeScript:
                {
                    if (fileName.EndsWith("-s") || fileName.EndsWith("_s"))
                    {
                        continue;
                    }

                    _protoGenerateHelper?.Run(operationCodeInfo, launcherOptions.OutputPath, Path.GetFileNameWithoutExtension(file));
                }
                    break;
                case ModeType.Godot:
                {
                    if (fileName.EndsWith("-s") || fileName.EndsWith("_s"))
                    {
                        continue;
                    }

                    _protoGenerateHelper?.Run(operationCodeInfo, launcherOptions.OutputPath, launcherOptions.NamespaceName);
                }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        _protoGenerateHelper?.Post(messageInfoLists, launcherOptions);
    }
}