using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using GameFrameX.ProtoExport.Persistence;
using ProtoExporterGUI.Models;
using Xunit;

namespace ProtoExporterGUI.Tests;

/// <summary>
/// LockPanelData 的契约：
/// 1) lock 文件不存在（或路径未设置）时状态为 NotFound，不抛异常；
/// 2) 合法 lock 文件解析为 Found，模块 / 消息数 / 墓碑数计数正确；
/// 3) 损坏或不兼容的 lock 文件归入 Failed，错误信息可展示；
/// 4) 时间戳格式化遵守传入 format，缺失时间返回 null。
/// </summary>
public class LockPanelDataTests
{
    private static string TempLockPath()
    {
        return Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".lock.json");
    }

    private static string WriteLock(MessageIdLock lockData)
    {
        var path = TempLockPath();
        File.WriteAllText(path, MessageIdLockStore.SaveToString(lockData));
        return path;
    }

    private static MessageIdLock SampleLock()
    {
        var lockData = MessageIdLock.CreateEmpty();
        var bag = new ModuleEntry
        {
            ModuleName = "Game.Bag",
        };
        bag.Messages["ReqUseItem"] = 1;
        bag.Messages["RespUseItem"] = 2;
        bag.Retired["ReqDropItem"] = 3;
        lockData.Modules["2"] = bag;

        var player = new ModuleEntry
        {
            ModuleName = "Game.Player",
        };
        player.Messages["ReqLogin"] = 1;
        lockData.Modules["1"] = player;

        return lockData;
    }

    /// <summary>
    /// Lock不存在_状态为未找到且不抛异常：lock 文件不存在时 Observe 状态为未找到（NotFound），且不抛异常。
    /// </summary>
    [Fact]
    public void LockFileMissing_StateNotFoundAndNoThrow()
    {
        var path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName(), "no-such.lock.json");
        var data = LockPanelData.Observe(path);

        Assert.Equal(LockPanelData.LoadState.NotFound, data.State);
        Assert.Null(data.LastWriteTime);
        Assert.Empty(data.Modules);
        Assert.Null(data.ErrorMessage);
    }

    /// <summary>
    /// 路径为空_同样视为未找到：lock 路径为空时同样视为未找到（NotFound）。
    /// </summary>
    [Fact]
    public void EmptyPath_TreatedAsNotFound()
    {
        var data = LockPanelData.Observe(null);

        Assert.Equal(LockPanelData.LoadState.NotFound, data.State);
        Assert.Empty(data.Modules);
    }

    /// <summary>
    /// 合法lock_模块消息墓碑计数正确：合法 lock 文件解析为 Found，模块 / 消息 / 墓碑（Retired）计数正确。
    /// </summary>
    [Fact]
    public void ValidLock_ModuleMessageRetiredCountsCorrect()
    {
        var path = WriteLock(SampleLock());
        try
        {
            var data = LockPanelData.Observe(path);

            Assert.Equal(LockPanelData.LoadState.Found, data.State);
            // SortedDictionary 按 key 字典序:1 在前,2 在后
            Assert.Equal(2, data.Modules.Count);
            Assert.Equal("1", data.Modules[0].ModuleKey);
            Assert.Equal("Game.Player", data.Modules[0].ModuleName);
            Assert.Equal(1, data.Modules[0].MessageCount);
            Assert.Equal(0, data.Modules[0].RetiredCount);

            Assert.Equal("2", data.Modules[1].ModuleKey);
            Assert.Equal("Game.Bag", data.Modules[1].ModuleName);
            Assert.Equal(2, data.Modules[1].MessageCount);
            Assert.Equal(1, data.Modules[1].RetiredCount);
        }
        finally
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }

    /// <summary>
    /// 损坏lock_归入失败状态并携带错误信息：损坏的 lock 文件归入失败状态（Failed）并携带可展示的错误信息。
    /// </summary>
    [Fact]
    public void CorruptedLock_ClassifiedAsFailedWithErrorMessage()
    {
        var path = TempLockPath();
        File.WriteAllText(path, "{ this is not valid json");
        try
        {
            var data = LockPanelData.Observe(path);

            Assert.Equal(LockPanelData.LoadState.Failed, data.State);
            Assert.Empty(data.Modules);
            Assert.False(string.IsNullOrEmpty(data.ErrorMessage));
        }
        finally
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }

    /// <summary>
    /// 时间戳_按指定格式输出：时间戳按指定 format 输出。
    /// </summary>
    [Fact]
    public void Timestamp_FormattedWithGivenFormat()
    {
        var path = WriteLock(SampleLock());
        try
        {
            var data = LockPanelData.Observe(path);

            Assert.Equal(LockPanelData.LoadState.Found, data.State);
            var formatted = data.FormatLastWriteTime("yyyy-MM-dd HH:mm:ss");
            Assert.Matches(@"^\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}$", formatted);
        }
        finally
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }

    /// <summary>
    /// 时间戳缺失_格式化返回null：时间戳缺失时格式化返回 null。
    /// </summary>
    [Fact]
    public void MissingTimestamp_FormatReturnsNull()
    {
        var data = LockPanelData.Observe(null);

        Assert.Null(data.FormatLastWriteTime("yyyy-MM-dd"));
    }
}
