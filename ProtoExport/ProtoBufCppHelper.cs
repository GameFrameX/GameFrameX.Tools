namespace GameFrameX.ProtoExport;

[Mode(ModeType.Cpp)]
internal class ProtoBufCppHelper : IProtoGenerateHelper
{
    public void Init(LauncherOptions launcherOptions)
    {
    }

    public void Run(MessageInfoList messageInfoList, string outputPath, string namespaceName = "GFXHotfix")
    {
        throw new NotImplementedException("C++ code generation is not yet implemented.");
    }

    public void Post(List<MessageInfoList> operationCodeInfo, LauncherOptions launcherOptions)
    {
    }
}
