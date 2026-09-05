using System.Collections.Generic;
using ProtoExporterGUI.Models;
using Xunit;

namespace ProtoExporterGUI.Tests;

/// <summary>
/// ModuleSourceParser 的契约：
/// 1) 能从导出器输出的 module 来源日志行收集 module → source 映射，中英文行格式均兼容
///    （导出器文案随 UI culture 切换：Package X =&gt; Module N (from s) / 包 X =&gt; 模块 N（来源 s））；
/// 2) 同一模块多次出现时，以后出现的为准（最后一次导出生效）；
/// 3) 无匹配行 / null 行 / null 集合返回空字典，不抛异常。
/// </summary>
public class ModuleSourceParserTests
{
    [Fact]
    public void 英文来源行_收集到模块与来源()
    {
        var map = ModuleSourceParser.Collect(new[]
        {
            "Package Basic => Module 10 (from fileName)",
            "Package ServerInternal => Module -1 (from option)",
        });

        Assert.Equal(2, map.Count);
        Assert.Equal("fileName", map[10]);
        Assert.Equal("option", map[-1]);
    }

    [Fact]
    public void 中文来源行_收集到模块与来源()
    {
        var map = ModuleSourceParser.Collect(new[]
        {
            "包 Basic => 模块 10（来源 fileName）",
            "包 ServerInternal => 模块 -1（来源 option）",
        });

        Assert.Equal(2, map.Count);
        Assert.Equal("fileName", map[10]);
        Assert.Equal("option", map[-1]);
    }

    [Fact]
    public void 旧版冒号格式行_同样兼容()
    {
        var map = ModuleSourceParser.Collect(new[]
        {
            "Package: Basic => Module: 10 (from fileName)",
        });

        Assert.Single(map);
        Assert.Equal("fileName", map[10]);
    }

    [Fact]
    public void 同模块重复出现_以后出现的为准()
    {
        var map = ModuleSourceParser.Collect(new[]
        {
            "Package Basic => Module 10 (from option)",
            "包 Basic => 模块 10（来源 fileName）",
        });

        Assert.Single(map);
        Assert.Equal("fileName", map[10]);
    }

    [Fact]
    public void 普通日志行_跳过不收集()
    {
        var map = ModuleSourceParser.Collect(new[]
        {
            "协议扫描完成: 共发现 5 个 .proto 文件，导出 5 个，跳过 0 个（模式: 服务器）",
            "Proto scan completed: found 5 .proto files, exported 5, skipped 0 (mode: server)",
            "[SKIP] client build skips internal proto (moduleId=-1 < 0): _-0120_Inner_Social",
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
