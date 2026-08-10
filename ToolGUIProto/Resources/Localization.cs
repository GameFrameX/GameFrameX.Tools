using System;
using System.ComponentModel;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace ToolGUI.Resources;

/// <summary>
/// 运行时本地化服务。封装 resx 资源读取与 Culture 切换，
/// 并通过 <see cref="PropertyChanged"/> 通知 XAML 绑定刷新所有本地化文本。
/// </summary>
/// <remarks>
/// 用法：XAML 绑定到 <c>Localization.Instance.ExportType</c> 等属性；
/// 切换语言时调用 <see cref="SetCulture"/>，触发 PropertyChanged 让全部绑定重新求值。
///
/// 设计选择：用单例 + 索引属性而非 DynamicResource，因为本工具字符串数量少、
/// 且需要在 C# 代码（ExportLogger 消息）中复用同一份资源，单例访问更直接。
/// </remarks>
public sealed class Localization : INotifyPropertyChanged
{
    /// <summary>支持的语言（与 resx 附属程序集对应）。</summary>
    public static readonly (string Code, string Display)[] SupportedCultures =
    {
        ("zh-CN", "中文"),
        ("en", "English"),
    };

    // 资源根名必须匹配 SDK 生成的嵌入资源名：<RootNamespace>.<resx 相对路径去掉扩展名>。
    // ToolGUIProto 默认 RootNamespace=ToolGUIProto，resx 在 Resources/Strings.resx，
    // 故实际嵌入名为 ToolGUIProto.Resources.Strings（见 MissingManifestResourceException 错误）。
    private static readonly ResourceManager Manager =
        new("ToolGUIProto.Resources.Strings", typeof(Localization).Assembly);

    /// <summary>全局单例，XAML 与代码共用。</summary>
    public static Localization Instance { get; } = new Localization();

    private Localization() { }

    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// 切换 UI Culture 并通知所有绑定刷新。
    /// 无效或缺失的 culture 静默回退到默认 resx（中性资源）。
    /// </summary>
    public void SetCulture(string cultureCode)
    {
        if (string.IsNullOrWhiteSpace(cultureCode))
        {
            return;
        }
        try
        {
            var culture = CultureInfo.GetCultureInfo(cultureCode);
            CultureInfo.DefaultThreadCurrentUICulture = culture;
            // 用一个空属性名通知，触发所有绑定重新求值（最简单的全量刷新）
            OnPropertyChanged(string.Empty);
        }
        catch (CultureNotFoundException)
        {
            // 静默忽略：保持当前语言不变。
        }
    }

    /// <summary>
    /// 取本地化字符串。找不到时返回 key 本身（便于发现遗漏的翻译条目）。
    /// </summary>
    public string this[string key]
    {
        get
        {
            var value = Manager.GetString(key, CultureInfo.CurrentUICulture);
            return string.IsNullOrEmpty(value) ? key : value;
        }
    }

    // 强类型访问器：供 XAML 绑定（x:Static 无法索引器，需要具名属性）。
    public string WindowTitle => this[nameof(WindowTitle)];
    public string ExportType => this[nameof(ExportType)];
    public string Namespace => this[nameof(Namespace)];
    public string InputPath => this[nameof(InputPath)];
    public string OutputPath => this[nameof(OutputPath)];
    public string UsingStatements => this[nameof(UsingStatements)];
    public string ImportPath => this[nameof(ImportPath)];
    public string RequireComments => this[nameof(RequireComments)];
    public string GenerateErrorCode => this[nameof(GenerateErrorCode)];
    public string GenerateDescription => this[nameof(GenerateDescription)];
    public string IsServer => this[nameof(IsServer)];
    public string ServerModeHint => this[nameof(ServerModeHint)];
    public string Export => this[nameof(Export)];
    public string Help => this[nameof(Help)];
    public string Browse => this[nameof(Browse)];
    public string Language => this[nameof(Language)];
    public string PickInputFolder => this[nameof(PickInputFolder)];
    public string PickOutputFolder => this[nameof(PickOutputFolder)];
    public string ErrUnsupportedMode => this[nameof(ErrUnsupportedMode)];
    public string ErrInputPathEmpty => this[nameof(ErrInputPathEmpty)];
    public string ErrOutputPathEmpty => this[nameof(ErrOutputPathEmpty)];
    public string ErrNamespaceEmpty => this[nameof(ErrNamespaceEmpty)];
    public string ExportSuccess => this[nameof(ExportSuccess)];
    public string ExportFailed => this[nameof(ExportFailed)];
    public string HelpOpenFailed => this[nameof(HelpOpenFailed)];

    private void OnPropertyChanged([CallerMemberName] string name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
