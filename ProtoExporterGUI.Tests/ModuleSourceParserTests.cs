using System.Collections.Generic;
using ProtoExporterGUI.Models;
using Xunit;

namespace ProtoExporterGUI.Tests;

/// <summary>
/// ModuleSourceParser 的契约：
/// 1) 能从导出器输出的「Package: X => Module: N (from fileName|option)」日志行收集 module → source 映射；
/// 2) 同一模块多次出现时，以后出现的为准（最后一次导出生效）；
/// 3) 无匹配行 / null 行 / null 集合返回空字典，不抛异常。
/// </summary>
public class ModuleSourceParserTests
{
    [Fact]
    public void 标准来源行_收集到模块与来源()
    {
        var map = ModuleSourceParser.Collect(new[]
        {
            "Package: Basic => Module: 10 (from fileName)",
            "Package: ServerInternal => Module: -1 (from option)",
        });

        Assert.Equal(2, map.Count);
        Assert.Equal("fileName", map[10]);
        Assert.Equal("option", map[-1]);
    }

    [Fact]
    public void 同模块重复出现_以后出现的为准()
    {
        var map = ModuleSourceParser.Collect(new[]
        {
            "Package: Basic => Module: 10 (from option)",
            "Package: Basic => Module: 10 (from fileName)",
        });

        Assert.Single(map);
        Assert.Equal("fileName", map[10]);
    }

    [Fact]
    public void 普通日志行_跳过不收集()
    {
        var map = ModuleSourceParser.Collect(new[]
        {
            "协议扫描完成: 共发现 5 个 .proto 文件",
            "[SKIP] 客户端构建跳过内部协议（moduleId=-1 < 0）: _-0120_Inner_Social",
            "导出成功",
        });

        Assert.Empty(map);
    }

    [Fact]
    public void null行与null集合_返回空字典不抛异常()
    {
        Assert.Empty(ModuleSourceParser.Collect(null));
        Assert.Empty(ModuleSourceParser.Collect(new string[] { null, string.Empty }));
    }
}
