using System.Collections.Generic;
using System.IO;
using GameFrameX.ProtoExport;
using Newtonsoft.Json;

namespace ToolGUI.Models;

public class SettingData
{
    Dictionary<string, LauncherOptions> Options { get; set; }

    public SettingData()
    {
        Options = new Dictionary<string, LauncherOptions>();
        Options.Add("Server", new LauncherOptions { Mode = ModeType.Server.ToString(), IsGenerateErrorCode = true, NamespaceName = "GameFrameX.Proto.Proto", OutputPath = "./../../../../../Server/GameFrameX.Proto/Proto", InputPath = "./../../../../../Protobuf" });
        Options.Add("Unity", new LauncherOptions { Mode = ModeType.Unity.ToString(), IsGenerateErrorCode = true, NamespaceName = "Hotfix.Proto", OutputPath = "./../../../../../Unity/Assets/Hotfix/Proto", InputPath = "./../../../../../Protobuf" });
        Options.Add("TypeScript", new LauncherOptions { Mode = ModeType.TypeScript.ToString(), IsGenerateErrorCode = true, NamespaceName = "", OutputPath = "./../../../../../Laya/src/gameframex/protobuf", InputPath = "./../../../../../Protobuf" });
    }

    public static SettingData Instance { get; } = new SettingData();

    public static LauncherOptions GetOptions(string mode)
    {
        if (string.IsNullOrWhiteSpace(mode))
        {
            return default;
        }

        return Instance.Options.GetValueOrDefault(mode);
    }

    public const string SettingPath = "Setting.json";

    public static void LoadSetting()
    {
        if (File.Exists(SettingPath))
        {
            var json = File.ReadAllText(SettingPath);
            Instance.Options = JsonConvert.DeserializeObject<Dictionary<string, LauncherOptions>>(json);
        }
    }

    public static void SaveSetting()
    {
        File.WriteAllText(SettingPath, JsonConvert.SerializeObject(Instance.Options, Formatting.Indented));
    }
}