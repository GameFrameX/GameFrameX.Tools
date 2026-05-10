using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using GameFrameX.ProtoExport;
using ToolGUI.Models;

namespace ToolGUI.Views;

public partial class MainWindow : Window
{
    StringWriter stringWriter;
    DispatcherTimer timer;

    public MainWindow()
    {
        InitializeComponent();
        Width = 450;
        Height = 400;
        MaxWidth = 450;
        stringWriter = new StringWriter();
        Console.SetOut(stringWriter);
        timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(100),
        };
        timer.Tick += Timer_Tick;
        SettingData.LoadSetting();
        var options = SettingData.GetOptions(this.Mode.SelectionBoxItem?.ToString());
        if (options != null)
        {
            this.InputPath.Text = options.InputPath;
            this.OutputPath.Text = options.OutputPath;
            this.NameSpace.Text = options.NamespaceName;
            this.IsGenerateErrorCode.IsChecked = options.IsGenerateErrorCode;
        }
    }

    private void Timer_Tick(object sender, EventArgs e)
    {
        var output = stringWriter.ToString();
        ErrorLog.Text = output;
    }

    private async void Button_OnClick(object sender, RoutedEventArgs e)
    {
        stringWriter.GetStringBuilder().Clear();
        timer.Start();

        var modeName = this.Mode.SelectionBoxItem?.ToString();
        var savedOptions = SettingData.GetOptions(modeName);
        if (savedOptions == null)
        {
            Console.WriteLine("不支持的运行模式");
            return;
        }

        LauncherOptions launcherOptions = new LauncherOptions
        {
            Mode = savedOptions.Mode,
            UsingStatements = savedOptions.UsingStatements,
            IsGenerateDescription = savedOptions.IsGenerateDescription,
            IsServer = savedOptions.IsServer,
            InputPath = this.InputPath.Text,
            OutputPath = this.OutputPath.Text,
            NamespaceName = this.NameSpace.Text,
            IsGenerateErrorCode = Convert.ToBoolean(this.IsGenerateErrorCode.IsChecked),
        };

        if (!Enum.TryParse<ModeType>(launcherOptions.Mode, true, out var modeType))
        {
            Console.WriteLine("不支持的运行模式");
            return;
        }

        if (string.IsNullOrWhiteSpace(launcherOptions.InputPath))
        {
            Console.WriteLine("协议文件路径不能为空");
            return;
        }

        if (string.IsNullOrWhiteSpace(launcherOptions.OutputPath))
        {
            Console.WriteLine("导出路径不能为空");
            return;
        }

        if (string.IsNullOrWhiteSpace(launcherOptions.NamespaceName))
        {
            Console.WriteLine("命名空间不能为空");
            return;
        }

        #region Save

        savedOptions.InputPath = launcherOptions.InputPath;
        savedOptions.OutputPath = launcherOptions.OutputPath;
        savedOptions.NamespaceName = launcherOptions.NamespaceName;
        savedOptions.IsGenerateErrorCode = launcherOptions.IsGenerateErrorCode;
        SettingData.SaveSetting();

        #endregion

        ProtoBufMessageHandler.Start(launcherOptions, modeType);
        Console.WriteLine("导出成功");
        await Task.Delay(500);
        timer.Stop();
    }

    private void Mode_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var options = SettingData.GetOptions(this.Mode?.SelectionBoxItem?.ToString());
        if (options != null)
        {
            this.InputPath.Text = options.InputPath;
            this.OutputPath.Text = options.OutputPath;
            this.NameSpace.Text = options.NamespaceName;
            this.IsGenerateErrorCode.IsChecked = options.IsGenerateErrorCode;
        }
    }

    private void HelpButton_OnClick(object sender, RoutedEventArgs e)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = "https://gameframex.doc.alianblank.com/tools/proto/launcher-params.html",
            UseShellExecute = true
        });
    }
}
