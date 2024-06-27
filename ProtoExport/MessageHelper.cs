using System.Text.RegularExpressions;

namespace GameFrameX.ProtoExport;

public static class MessageHelper
{
    // 正则表达式匹配enums
    private const string EnumPattern = @"enum\s+(\w+)\s+\{([^}]*)\}";

    // 正则表达式匹配messages
    private const string MessagePattern = @"message\s+(\w+)\s*\{\s*([^}]+)\s*\}";
    private const string CommentPattern = @"//([^\n]*)\n\s*(enum|message)\s+(\w+)\s*{";
    private const string StartPattern = @"option start = (\d+);";
    private const string ModulePattern = @"option module = (\d+);";
    private const string PackagePattern = @"package (\w+);";


    public static OperationCodeInfoList Parse(string proto, string fileName, string filePath)
    {
        var packageMatch = Regex.Match(proto, PackagePattern, RegexOptions.Singleline);

        if (packageMatch.Success)
        {
            Console.WriteLine($"Package: {packageMatch.Groups[1].Value}");
            var packageName = packageMatch.Groups[1].Value.Trim();
            if (packageName != fileName)
            {
                Console.WriteLine("PackageName and fileName do not match");
                throw new Exception("PackageName and fileName do not match==>example: package {" + fileName + "};");
            }
        }
        else
        {
            Console.WriteLine("Package not found");
            throw new Exception("Package not found==>example: package {" + fileName + "};");
        }

        OperationCodeInfoList operationCodeInfo = new OperationCodeInfoList
        {
            OutputPath = Path.Combine(filePath, fileName)
        };

        // 使用正则表达式提取start
        Match startMatch = Regex.Match(proto, StartPattern, RegexOptions.Singleline);
        if (startMatch.Success)
        {
            Console.WriteLine($"Start: {startMatch.Groups[1].Value}");
            operationCodeInfo.Start = int.Parse(startMatch.Groups[1].Value);
        }
        else
        {
            Console.WriteLine("Start not found");
            throw new Exception("Start not found==>example: option start = 100");
        }

        // 使用正则表达式提取module
        Match moduleMatch = Regex.Match(proto, ModulePattern, RegexOptions.Singleline);
        if (moduleMatch.Success)
        {
            Console.WriteLine($"Module: {moduleMatch.Groups[1].Value}");
            if (ushort.TryParse(moduleMatch.Groups[1].Value, out var value))
            {
                operationCodeInfo.Module = value;
            }
            else
            {
                Console.WriteLine("Module range error");
                throw new ArgumentOutOfRangeException("module", "Module range error==>module >= 0 and module <= 65535");
            }
        }
        else
        {
            Console.WriteLine("Module not found");
            throw new Exception("Module not found==>example: option module = 100");
        }

        // 使用正则表达式提取枚举类型
        ParseEnum(proto, operationCodeInfo.OperationCodeInfos);

        // 使用正则表达式提取消息类型
        ParseMessage(proto, operationCodeInfo.OperationCodeInfos);

        ParseComment(proto, operationCodeInfo.OperationCodeInfos);

        // 消息码排序配对
        MessageIdHandler(operationCodeInfo.OperationCodeInfos, operationCodeInfo.Start);
        return operationCodeInfo;
    }

    private static void MessageIdHandler(List<OperationCodeInfo> operationCodeInfos, int start)
    {
        foreach (var operationCodeInfo in operationCodeInfos)
        {
            if (operationCodeInfo.IsMessage)
            {
                if (operationCodeInfo.Opcode > 0)
                {
                    continue;
                }

                operationCodeInfo.Opcode = start;
                // if (operationCodeInfo.IsRequest)
                // {
                //     operationCodeInfo.ResponseMessage = FindResponse(operationCodeInfos, operationCodeInfo.MessageName);
                //     if (operationCodeInfo.ResponseMessage != null)
                //     {
                //         operationCodeInfo.ResponseMessage.Opcode = operationCodeInfo.Opcode;
                //     }
                // }

                start++;
            }
        }
    }

    private static OperationCodeInfo? FindResponse(List<OperationCodeInfo> operationCodeInfos, string messageName)
    {
        foreach (var operationCodeInfo in operationCodeInfos)
        {
            if (operationCodeInfo is { IsMessage: true, IsResponse: true } && operationCodeInfo.MessageName == messageName)
            {
                return operationCodeInfo;
            }
        }

        return default;
    }

    private static void ParseComment(string proto, List<OperationCodeInfo> operationCodeInfos)
    {
        MatchCollection enumMatches = Regex.Matches(proto, CommentPattern, RegexOptions.Singleline);
        foreach (Match match in enumMatches)
        {
            if (match.Groups.Count > 3)
            {
                var comment = match.Groups[1].Value;
                var type = match.Groups[3].Value;
                foreach (var operationCodeInfo in operationCodeInfos)
                {
                    if (operationCodeInfo.Name == type)
                    {
                        operationCodeInfo.Description = comment.Trim();
                        break;
                    }
                }
            }
        }
    }

    private static void ParseEnum(string proto, List<OperationCodeInfo> codes)
    {
        MatchCollection enumMatches = Regex.Matches(proto, EnumPattern, RegexOptions.Singleline);
        foreach (Match match in enumMatches)
        {
            OperationCodeInfo info = new OperationCodeInfo(true);
            codes.Add(info);
            string blockName = match.Groups[1].Value;
            info.Name = blockName;
            // Console.WriteLine("Enum Name: " + match.Groups[1].Value);
            // Console.WriteLine("Contents: " + match.Groups[2].Value);
            var blockContent = match.Groups[2].Value.Trim();
            foreach (var line in blockContent.Split(new string[] { "\r", "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries))
            {
                OperationField field = new OperationField();
                info.Fields.Add(field);
                // 解析注释
                var lineSplit = line.Split("//", StringSplitOptions.RemoveEmptyEntries);
                if (lineSplit.Length > 1)
                {
                    // 有注释
                    field.Description = lineSplit[1].Trim();
                }

                if (lineSplit.Length > 0)
                {
                    var fieldType = lineSplit[0].Trim().Trim(';');
                    var fieldSplit = fieldType.Split('=', StringSplitOptions.RemoveEmptyEntries);
                    if (fieldSplit.Length > 1)
                    {
                        field.Type = fieldSplit[0].Trim();
                        field.Members = int.Parse(fieldSplit[1].Replace(";", "").Trim());
                    }
                }
            }
        }
    }

    private static void ParseMessage(string proto, List<OperationCodeInfo> codes)
    {
        MatchCollection messageMatches = Regex.Matches(proto, MessagePattern, RegexOptions.Singleline);
        foreach (Match match in messageMatches)
        {
            string messageName = match.Groups[1].Value;
            // Console.WriteLine("Message Name: " + match.Groups[1].Value);
            // Console.WriteLine("Contents: " + match.Groups[2].Value);
            var blockContent = match.Groups[2].Value.Trim();
            OperationCodeInfo info = new OperationCodeInfo();
            codes.Add(info);
            info.Name = messageName;
            foreach (var line in blockContent.Split(new string[] { "\r", "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries))
            {
                OperationField field = new OperationField();
                info.Fields.Add(field);
                // 解析注释
                var lineSplit = line.Split("//", StringSplitOptions.RemoveEmptyEntries);
                if (lineSplit.Length > 1)
                {
                    // 有注释
                    field.Description = lineSplit[1].Trim();
                }

                // 字段
                if (lineSplit.Length > 0)
                {
                    var fieldSplit = lineSplit[0].Split('=', StringSplitOptions.RemoveEmptyEntries);
                    if (fieldSplit.Length > 1)
                    {
                        field.Members = int.Parse(fieldSplit[1].Replace(";", "").Trim());
                    }

                    if (fieldSplit.Length > 0)
                    {
                        var fieldSplitStrings = fieldSplit[0].Split(Utility.splitChars, StringSplitOptions.RemoveEmptyEntries);
                        if (fieldSplitStrings.Length > 2)
                        {
                            var key = fieldSplitStrings[0].Trim();
                            if (key.Trim().StartsWith("repeated"))
                            {
                                field.IsRepeated = true;
                                field.Type = Utility.ConvertType(fieldSplitStrings[1].Trim());
                            }
                            else
                            {
                                field.Type = Utility.ConvertType(key + fieldSplitStrings[1].Trim());
                                if (key.Trim().StartsWith("map"))
                                {
                                    field.IsKv = true;
                                }
                            }


                            field.Name = fieldSplitStrings[2].Trim();
                        }
                        else if (fieldSplitStrings.Length > 1)
                        {
                            field.Type = Utility.ConvertType(fieldSplitStrings[0].Trim());
                            field.Name = fieldSplitStrings[1].Trim();
                        }
                    }
                }
            }

            if (info.IsResponse && !info.IsNotify)
            {
                OperationField field = new OperationField();
                field.Description = "返回的错误码";
                field.Name = "ErrorCode";
                field.Type = "int";
                field.Members = 888;
                info.Fields.Add(field);
            }
        }
    }
}