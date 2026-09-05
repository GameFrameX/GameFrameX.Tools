using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using GameFrameX.ProtoExport;
using Xunit;

namespace ProtoExporterGUI.Tests;

/// <summary>
/// 资源 key 完整性守护：ProtoExport 与 GUI 的中英两套 resx 必须保持同一 key 集合，
/// 防止新增文案只写了一侧、运行时回退成 key 本身（<see cref="Loc.Get"/> 的兜底行为）。
/// </summary>
public class LocalizationParityTests
{
    public static IEnumerable<object[]> ResourceManifests()
    {
        yield return new object[] { "GameFrameX.ProtoExport.Strings", typeof(Loc).Assembly };
        yield return new object[] { "ProtoExporterGUI.Resources.Strings", typeof(ProtoExporterGUI.Models.LockModuleRow).Assembly };
    }

    /// <summary>
    /// 中英资源 key 集合一致
    /// </summary>
    [Theory]
    [MemberData(nameof(ResourceManifests))]
    public void ChineseAndEnglishResourceKeys_HaveIdenticalSets(string baseName, System.Reflection.Assembly assembly)
    {
        var manager = new ResourceManager(baseName, assembly);

        var neutralKeys = new HashSet<string>(StringComparer.Ordinal);
        using (var set = manager.GetResourceSet(CultureInfo.InvariantCulture, true, true))
        {
            foreach (System.Collections.DictionaryEntry entry in set)
            {
                neutralKeys.Add((string)entry.Key);
            }
        }

        var enKeys = new HashSet<string>(StringComparer.Ordinal);
        using (var set = manager.GetResourceSet(CultureInfo.GetCultureInfo("en"), true, true))
        {
            foreach (System.Collections.DictionaryEntry entry in set)
            {
                enKeys.Add((string)entry.Key);
            }
        }

        Assert.True(neutralKeys.SetEquals(enKeys),
            $"resx key 不一致 [{baseName}]：仅中文有 {{{string.Join(", ", neutralKeys.Except(enKeys))}}}，仅英文有 {{{string.Join(", ", enKeys.Except(neutralKeys))}}}");
    }
}
