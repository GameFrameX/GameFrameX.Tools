namespace GameFrameX.ProtoExport;

[Mode(ModeType.Lua)]
internal class ProtoBufLuaHelper : IProtoGenerateHelper
{
    public void Init(LauncherOptions launcherOptions)
    {
    }

    public void Run(MessageInfoList messageInfoList, string outputPath, string namespaceName = "GFXHotfix")
    {
        throw new NotImplementedException("Lua code generation is not yet implemented.");
    }

    public void Post(List<MessageInfoList> operationCodeInfo, LauncherOptions launcherOptions)
    {
    }
}
