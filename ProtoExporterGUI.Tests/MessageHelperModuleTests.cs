using GameFrameX.ProtoExport;
using Xunit;

namespace ProtoExporterGUI.Tests;

/// <summary>
/// MessageHelper 模块 ID 三态解析的契约：
/// 1) 文件名前缀 _&lt;模块ID&gt;_ 优先，option module 声明兜底；
/// 2) 两者都有必须一致，不一致抛 FormatException（绝不静默取其一）；
/// 3) 文件名以 _+数字 开头但缺第二个下划线（如 _0010Basic）视为命名格式错误；
/// 4) 都没有时报 Module not found；
/// 5) ModuleSource 记录实际来源（FileName / Option）。
/// </summary>
/// <remarks>
/// Parse 内部写 ExportLogger.WriteLine（进程级静态委托），与 ExportLoggerTests 同 collection 串行执行，
/// 避免并行时日志写入对方捕获列表造成交叉失败。
/// </remarks>
[Collection("ExportLogger")]
public class MessageHelperModuleTests
{
    private const string ProtoWithModule10 = @"syntax = ""proto3"";

package Test;

option module = 10;

// 测试请求
message ReqDemo
{
  string Value = 1;
}
";

    [Fact]
    public void 文件名前缀与option一致_取文件名来源()
    {
        var info = MessageHelper.Parse(ProtoWithModule10, "_0010_Basic", "out", false);

        Assert.Equal(10, info.Module);
        Assert.Equal(MessageInfoList.ModuleSourceKind.FileName, info.ModuleSource);
        // module 解析不影响消息解析主流程
        Assert.Single(info.Infos);
    }

    [Fact]
    public void 带路径与扩展名的文件名_仍取前缀()
    {
        var info = MessageHelper.Parse(ProtoWithModule10, "Protobuf/_0010_Basic.proto", "out", false);

        Assert.Equal(10, info.Module);
        Assert.Equal(MessageInfoList.ModuleSourceKind.FileName, info.ModuleSource);
    }

    [Fact]
    public void 仅文件名前缀_省略option_用文件名()
    {
        var proto = ProtoWithModule10.Replace("option module = 10;", string.Empty);

        var info = MessageHelper.Parse(proto, "_0010_Basic", "out", false);

        Assert.Equal(10, info.Module);
        Assert.Equal(MessageInfoList.ModuleSourceKind.FileName, info.ModuleSource);
    }

    [Fact]
    public void 文件名前缀与option不一致_报错并携带两个值()
    {
        var proto = ProtoWithModule10.Replace("option module = 10;", "option module = 20;");

        var ex = Assert.Throws<FormatException>(() => MessageHelper.Parse(proto, "_0010_Basic", "out", false));

        // 断言取资源文案本体（与被测方同 culture，自洽），数字/文件名为字面量、语言无关
        Assert.Equal(string.Format(Loc.Err_ModuleMismatch, "_0010_Basic", 10, 20), ex.Message);
        Assert.Contains("_0010_Basic", ex.Message);
    }

    [Fact]
    public void 无前缀文件名_option兜底()
    {
        var info = MessageHelper.Parse(ProtoWithModule10, "Basic", "out", false);

        Assert.Equal(10, info.Module);
        Assert.Equal(MessageInfoList.ModuleSourceKind.Option, info.ModuleSource);
    }

    [Fact]
    public void 无前缀且无option_报ModuleNotFound()
    {
        var proto = ProtoWithModule10.Replace("option module = 10;", string.Empty);

        var ex = Assert.Throws<Exception>(() => MessageHelper.Parse(proto, "Basic", "out", false));

        Assert.Equal(Loc.Err_ModuleNotFound, ex.Message);
    }

    [Fact]
    public void 缺第二个下划线的疑似前缀_报格式错误()
    {
        var ex = Assert.Throws<FormatException>(() => MessageHelper.Parse(ProtoWithModule10, "_0010Basic", "out", false));

        Assert.Equal(string.Format(Loc.Err_ModuleFileNameFormat, "_0010Basic"), ex.Message);
    }

    [Fact]
    public void 连字符作分隔符_同样取前缀()
    {
        var info = MessageHelper.Parse(ProtoWithModule10, "_0010-Basic", "out", false);

        Assert.Equal(10, info.Module);
        Assert.Equal(MessageInfoList.ModuleSourceKind.FileName, info.ModuleSource);
    }

    [Fact]
    public void 负数前缀_剥离前导零得到负模块()
    {
        var proto = ProtoWithModule10.Replace("option module = 10;", "option module = -120;");

        var info = MessageHelper.Parse(proto, "_-0120_Inner_Social", "out", false);

        Assert.Equal(-120, info.Module);
        Assert.Equal(MessageInfoList.ModuleSourceKind.FileName, info.ModuleSource);
    }

    [Fact]
    public void 文件名模块超出short范围_报错()
    {
        Assert.Throws<FormatException>(() => MessageHelper.Parse(ProtoWithModule10, "_99999_Overflow", "out", false));
    }

    [Fact]
    public void option模块超出short范围_报错()
    {
        var proto = ProtoWithModule10.Replace("option module = 10;", "option module = 99999;");

        Assert.Throws<FormatException>(() => MessageHelper.Parse(proto, "Basic", "out", false));
    }
}
