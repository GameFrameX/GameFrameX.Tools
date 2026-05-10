namespace GameFrameX.ProtoExport;

/// <summary>
/// C# 目标平台的配置
/// </summary>
public sealed record CSharpTargetConfig(
    string[] UsingStatements,
    bool ShouldGenerateDescription
);

/// <summary>
/// 目标平台配置注册表
/// </summary>
public static class TargetConfigRegistry
{
    private static readonly CSharpTargetConfig DefaultCSharp = new(
        ["using System;", "using ProtoBuf;", "using System.Collections.Generic;", "using GameFrameX.Network.Runtime;"],
        false
    );

    private static readonly Dictionary<TargetType, CSharpTargetConfig> CSharpConfigs = new()
    {
        [TargetType.Server] = new(
            ["using System;", "using ProtoBuf;", "using System.Collections.Generic;", "using GameFrameX.NetWork.Abstractions;", "using GameFrameX.NetWork.Messages;"],
            true
        ),
        [TargetType.Unity] = new(
            ["using System;", "using ProtoBuf;", "using System.Collections.Generic;", "using GameFrameX.Network.Runtime;"],
            false
        ),
        [TargetType.Godot] = new(
            ["using System;", "using ProtoBuf;", "using System.Collections.Generic;", "using GameFrameX.Network.Runtime;"],
            false
        ),
        [TargetType.Stride] = new(
            ["using System;", "using ProtoBuf;", "using System.Collections.Generic;", "using GameFrameX.Network.Runtime;"],
            false
        ),
        [TargetType.Flax] = new(
            ["using System;", "using ProtoBuf;", "using System.Collections.Generic;", "using GameFrameX.Network.Runtime;"],
            false
        ),
    };

    public static CSharpTargetConfig GetCSharpConfig(TargetType target)
        => CSharpConfigs.GetValueOrDefault(target, DefaultCSharp);
}
