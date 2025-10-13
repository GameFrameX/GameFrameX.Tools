using GameFrameX.Foundation.Options;

namespace GameFrameX.ProtoExport
{
    internal static class Program
    {
        private static int Main(string[] args)
        {
            try
            {
                var launcherOptions = new OptionsBuilder<LauncherOptions>(args).Build();
                if (launcherOptions == null)
                {
                    Console.WriteLine("参数错误，解析失败");
                    throw new Exception("参数错误，解析失败");
                }

                if (!Enum.TryParse<ModeType>(launcherOptions.Mode, true, out var modeType))
                {
                    Console.WriteLine("不支持的运行模式");
                    throw new Exception("不支持的运行模式");
                }

                ProtoBufMessageHandler.Start(launcherOptions, modeType);
                Console.WriteLine("导出成功,请查看日志");
            }
            catch (Exception e)
            {
                Console.WriteLine("导出失败,请检查错误信息");
                Console.WriteLine(e);
                Console.ReadKey();
                throw;
            }

            return 0;
        }
    }
}