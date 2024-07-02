using CommandLine;

namespace GameFrameX.ProtoExport;

public sealed class LauncherOptions
{
    [Option("inputPath", Required = true, HelpText = "协议文件路径")]
    public string InputPath { get; set; }

    [Option("mode", Required = true, HelpText = "运行模式")]
    public string Mode { get; set; }

    [Option("outputPath", Required = true, HelpText = "文件路径")]
    public string OutputPath { get; set; }

    [Option("namespaceName", Required = true, HelpText = "命名空间")]
    public string NamespaceName { get; set; }

    [Option("isGenerateErrorCode", Required = false, Default = true, HelpText = "是否生成错误码")]
    public bool IsGenerateErrorCode { get; set; }
}