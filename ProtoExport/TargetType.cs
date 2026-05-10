namespace GameFrameX.ProtoExport;

public enum TargetType
{
    /// <summary>
    /// 无特定目标（默认）
    /// </summary>
    None,

    // C# targets
    /// <summary>
    /// 服务器
    /// </summary>
    Server,
    /// <summary>
    /// Unity
    /// </summary>
    Unity,
    /// <summary>
    /// Godot
    /// </summary>
    Godot,
    /// <summary>
    /// Stride
    /// </summary>
    Stride,
    /// <summary>
    /// Flax Engine
    /// </summary>
    Flax,

    // TypeScript targets
    /// <summary>
    /// LayaAir
    /// </summary>
    LayaAir,
    /// <summary>
    /// Cocos Creator
    /// </summary>
    CocosCreator,
    /// <summary>
    /// Phaser
    /// </summary>
    Phaser,

    // C++ targets
    /// <summary>
    /// Unreal Engine
    /// </summary>
    Unreal,

    // Lua targets
    /// <summary>
    /// Defold
    /// </summary>
    Defold,
    /// <summary>
    /// Solar2D
    /// </summary>
    Solar2D,
    /// <summary>
    /// Dora SSR
    /// </summary>
    DoraSSR
}
