using System.Collections.Generic;
using System.IO;
using GameFrameX.ProtoExport;
using GameFrameX.ProtoExport.Persistence;
using Xunit;

namespace ProtoExporterGUI.Tests;

/// <summary>
/// LockSeedGenerator 的契约：
/// 1) SeedFromCurrentOpcodes 必须把当前 Opcode 1:1 落到 Messages，不重新分配；
/// 2) 调用一次后，再走 Coordinator.AssignAndPersist，老号纹丝不动；
/// 3) Opcode <= 0 必须报错（防止误传空 proto 集合）。
/// </summary>
public class LockSeedGeneratorTests
{
    private static MessageInfoList ListWithOpcodes(short module, params (string name, int opcode)[] entries)
    {
        var list = new MessageInfoList { Module = module, ModuleName = "M" };
        foreach (var (name, opcode) in entries)
        {
            list.Infos.Add(new MessageInfo { Name = name, Opcode = opcode });
        }

        return list;
    }

    /// <summary>
    /// Seed 冻结当前 Opcode 作为起点，不重新分配。
    /// </summary>
    [Fact]
    public void Seed_FreezesCurrentOpcodesAsBaseline()
    {
        var path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".lock.json");
        try
        {
            var lists = new[]
            {
                ListWithOpcodes(1, ("ReqLogin", 10), ("RespLogin", 11)),
                ListWithOpcodes(2, ("ReqOpen", 10)),
            };

            var result = LockSeedGenerator.SeedFromCurrentOpcodes(path, lists);
            Assert.Equal(2, result.ModuleCount);
            Assert.Equal(3, result.NewlyAssignedCount);

            var lockData = MessageIdLockStore.Load(path);
            Assert.Equal(10, lockData.Modules["1"].Messages["ReqLogin"]);
            Assert.Equal(11, lockData.Modules["1"].Messages["RespLogin"]);
            Assert.Equal(10, lockData.Modules["2"].Messages["ReqOpen"]);
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
    /// Seed 后的 lock 喂给 Coordinator —— 老号必须保持不变；新增消息续号。
    /// </summary>
    [Fact]
    public void SeedThenCoordinator_OldSubIdsUnchanged()
    {
        var path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".lock.json");
        try
        {
            // Seed：模拟迁移
            LockSeedGenerator.SeedFromCurrentOpcodes(path, new[]
            {
                ListWithOpcodes(1, ("ReqLogin", 10), ("RespLogin", 11)),
            });

            // 再走 Coordinator，新消息续号
            var lists = new[] { ListWithOpcodes(1, ("ReqLogin", 0), ("RespLogin", 0), ("ReqLogout", 0)) };
            MessageIdCoordinator.AssignAndPersist(path, lists);

            Assert.Equal(10, lists[0].Infos[0].Opcode); // 沿用
            Assert.Equal(11, lists[0].Infos[1].Opcode); // 沿用
            Assert.Equal(12, lists[0].Infos[2].Opcode); // 新增
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
    /// Seed 时 Opcode 非法（&lt;= 0）报错，防止误传空 proto 集合。
    /// </summary>
    [Fact]
    public void Seed_InvalidOpcode_Throws()
    {
        var path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".lock.json");
        try
        {
            var lists = new[] { ListWithOpcodes(1, ("ReqA", 0)) };
            Assert.Throws<InvalidDataException>(() => LockSeedGenerator.SeedFromCurrentOpcodes(path, lists));
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
    /// Seed 时 Opcode 超出 SubId 上限报错。
    /// </summary>
    [Fact]
    public void Seed_OpcodeExceedsRange_Throws()
    {
        var path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".lock.json");
        try
        {
            var lists = new[] { ListWithOpcodes(1, ("ReqA", 0x10000)) };
            Assert.Throws<InvalidDataException>(() => LockSeedGenerator.SeedFromCurrentOpcodes(path, lists));
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
    /// FormatLockForDisplay 输出等价于序列化输出。
    /// </summary>
    [Fact]
    public void FormatLockForDisplay_EqualsSerializedOutput()
    {
        var path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".lock.json");
        try
        {
            LockSeedGenerator.SeedFromCurrentOpcodes(path, new[]
            {
                ListWithOpcodes(1, ("ReqA", 10), ("ReqB", 11)),
            });

            var fromFile = MessageIdLockStore.Load(path);
            var formatted = LockSeedGenerator.FormatLockForDisplay(fromFile);
            var raw = MessageIdLockStore.SaveToString(fromFile);

            Assert.Equal(raw, formatted);
        }
        finally
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}