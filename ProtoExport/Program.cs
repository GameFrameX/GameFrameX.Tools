using CommandLine;

namespace GameFrameX.ProtoExport
{
    internal static class Program
    {
        private static int Main(string[] args)
        {
            var launcherOptions = Parser.Default.ParseArguments<LauncherOptions>(args).Value;
            if (launcherOptions == null)
            {
                Console.WriteLine("参数错误，解析失败");
                return 1;
            }

            if (!Enum.TryParse<ModeType>(launcherOptions.Mode, true, out var modeType))
            {
                Console.WriteLine("不支持的运行模式");
                return 1;
            }

            ProtoBufMessageHandler.Start(launcherOptions, modeType);

            Console.WriteLine("导出成功，按任意键退出");
            Console.ReadKey();
            return 0;
        }
    }
}