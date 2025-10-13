using GameFrameX.Foundation.Options.Attributes;

namespace GameFrameX.ProtoExport;

public sealed class LauncherOptions
{
    /// <summary>
    /// 协议文件路径
    /// </summary>
    [Option("inputPath", Required = true, Description = "协议文件路径")]
    public string InputPath { get; set; }

    /// <summary>
    /// 运行模式
    /// </summary>
    [Option("mode", Required = true, Description = "运行模式")]
    public string Mode { get; set; }

    /// <summary>
    /// 文件路径
    /// </summary>
    [Option("outputPath", Required = true, Description = "文件路径")]
    public string OutputPath { get; set; }

    /// <summary>
    /// 命名空间
    /// </summary>
    [Option("namespaceName", Required = true, Description = "命名空间")]
    public string NamespaceName { get; set; }

    /// <summary>
    /// 是否生成错误码
    /// </summary>
    [Option("isGenerateErrorCode", Required = false, DefaultValue = true, Description = "是否生成错误码")]
    public bool IsGenerateErrorCode { get; set; }

    /// <summary>
    /// 是否生成错误码Excel文件
    /// </summary>
    [Option("isGenerateErrorCodeExcelFile", Required = false, DefaultValue = true, Description = "是否生成错误码Excel文件")]
    public bool IsGenerateErrorCodeExcelFile { get; set; }

    /// <summary>
    /// 错误码Excel文件路径
    /// </summary>
    [Option("errorCodeExcelFilePath", Required = false, Description = "错误码Excel文件路径")]
    public string ErrorCodeExcelFilePath { get; set; }
}