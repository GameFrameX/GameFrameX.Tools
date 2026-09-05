using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ProtoExporterGUI.Models
{
    /// <summary>
    /// 从导出日志中提取各模块的 module 来源（文件名前缀 / option 声明）。
    /// </summary>
    /// <remarks>
    /// 导出器（ProtoExport.MessageHelper.Parse）解析每个 proto 文件后输出一行
    /// <c>Package: X => Module: 10 (from fileName)</c> 的日志。GUI 只做观测，
    /// 从日志读回 module → source 映射，与 lock 面板按模块号关联显示。
    /// 正则按数字与来源 token 捕获，不锚定中文文案细节。
    /// </remarks>
    public static class ModuleSourceParser
    {
        /// <summary>匹配导出器输出的 module 来源日志行。</summary>
        private static readonly Regex SourcePattern = new Regex(
            @"=>\s*Module:\s*(?<module>-?\d+)\s*\(from\s+(?<source>fileName|option)\)",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);

        /// <summary>
        /// 从多行日志中收集 module → source 映射。同一模块出现多次时以后出现的为准（最后一次导出生效）。
        /// 匹配不到任何行时返回空字典，不抛异常（含 null 行输入）。
        /// </summary>
        public static Dictionary<short, string> Collect(IEnumerable<string> logLines)
        {
            var result = new Dictionary<short, string>();
            if (logLines == null)
            {
                return result;
            }

            foreach (var line in logLines)
            {
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                var match = SourcePattern.Match(line);
                if (!match.Success)
                {
                    continue;
                }

                if (!short.TryParse(match.Groups["module"].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var module))
                {
                    continue;
                }

                result[module] = match.Groups["source"].Value;
            }

            return result;
        }
    }
}
