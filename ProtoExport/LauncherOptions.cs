using CommandLine;

namespace GameFrameX.ProtoExport;

public sealed class LauncherOptions
{
    [Option('i', "inputPath", Required = true, HelpText = "协议文件路径")]
    public string InputPath { get; set; }

    [Option('m', "mode", Required = true, HelpText = "运行模式")]
    public string Mode { get; set; }

    [Option('o', "outputPath", Required = true, HelpText = "文件路径")]
    public string OutputPath { get; set; }

    [Option('n', "namespaceName", Required = true, HelpText = "命名空间")]
    public string NamespaceName { get; set; }
}