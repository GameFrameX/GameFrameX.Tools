using System.Globalization;
using System.Threading;
using ProtoExporterGUI.Resources;
using Xunit;

namespace ProtoExporterGUI.Tests;

public class LocalizationTests : IDisposable
{
    readonly CultureInfo _originalCurrentCulture = Thread.CurrentThread.CurrentUICulture;
    readonly CultureInfo _originalDefaultCulture = CultureInfo.DefaultThreadCurrentUICulture;

    public void Dispose()
    {
        Thread.CurrentThread.CurrentUICulture = _originalCurrentCulture;
        CultureInfo.DefaultThreadCurrentUICulture = _originalDefaultCulture;
    }

    /// <summary>
    /// WindowTitle 按当前语言显示友好名称。
    /// </summary>
    [Fact]
    public void WindowTitle_ShowsFriendlyNamePerCurrentCulture()
    {
        Localization.Instance.SetCulture("zh-CN");
        Assert.Equal("ProtoExporterGUI 协议导出工具", Localization.Instance.WindowTitle);

        Localization.Instance.SetCulture("en");
        Assert.Equal("ProtoExporterGUI", Localization.Instance.WindowTitle);
    }
}
