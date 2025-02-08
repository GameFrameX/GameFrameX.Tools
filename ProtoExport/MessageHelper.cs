using System.Text.RegularExpressions;

namespace GameFrameX.ProtoExport;

public static partial class MessageHelper
{
    // 正则表达式匹配enums
    private const string EnumPattern = @"enum\s+(\w+)\s+\{([^}]*)\}";

    // 正则表达式匹配messages
    private const string MessagePattern = @"message\s+(\w+)\s*\{\s*([^}]+)\s*\}";
    private const string CommentPattern = @"//([^\n]*)\n\s*(enum|message)\s+(\w+)\s*{";
    private const string StartPattern = @"option start = (\d+);";
    private const string ModulePattern = @"option module = (-?\d+);";
    private const string PackagePattern = @"package (\w+);";


    public static MessageInfoList Parse(string proto, string fileName, string filePath, bool isGenerateErrorCode)
    {
        var packageMatch = Regex.Match(proto, PackagePattern, RegexOptions.Singleline);

        if (!packageMatch.Success)
        {
            Console.WriteLine("Package not found");
            throw new Exception("Package not found==>example: package {" + fileName + "};");
        }

        MessageInfoList messageInfo = new MessageInfoList
        {
            OutputPath = Path.Combine(filePath, fileName),
            ModuleName = packageMatch.Groups[1].Value,
            FileName = fileName,
        };

        // 使用正则表达式提取module
        Match moduleMatch = Regex.Match(proto, ModulePattern, RegexOptions.Singleline);
        if (moduleMatch.Success)
        {
            if (short.TryParse(moduleMatch.Groups[1].Value, out var value))
            {
                messageInfo.Module = value;
            }
            else
            {
                Console.WriteLine("Module range error");
                throw new ArgumentOutOfRangeException("module", $"Module range error==>module > {short.MinValue} and module < {short.MaxValue}");
            }
        }
        else
        {
            Console.WriteLine("Module not found");
            throw new Exception("Module not found==>example: option module = 100");
        }

        var packageName = packageMatch.Groups[1].Value;
        Console.WriteLine($"Package: {packageName} => Module: {moduleMatch.Groups[1].Value}");
        // 使用正则表达式提取枚举类型
        ParseEnum(proto, packageName, messageInfo.Infos);

        // 使用正则表达式提取消息类型
        ParseMessage(proto, packageName, messageInfo.Infos, isGenerateErrorCode);

        ParseComment(proto, packageName, messageInfo.Infos);

        // 消息码排序配对
        MessageIdHandler(messageInfo.Infos, 10);
        return messageInfo;
    }

    private static void MessageIdHandler(List<MessageInfo> operationCodeInfos, int start)
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

    private static void ParseComment(string proto, string packageName, List<MessageInfo> operationCodeInfos)
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

    private static void ParseEnum(string proto, string packageName, List<MessageInfo> codes)
    {
        MatchCollection enumMatches = Regex.Matches(proto, EnumPattern, RegexOptions.Singleline);
        foreach (Match match in enumMatches)
        {
            MessageInfo info = new MessageInfo(true);
            codes.Add(info);
            string blockName = match.Groups[1].Value;
            if (!Utility.IsCamelCase(blockName))
            {
                throw new Exception($"[{packageName}] 包的 [{blockName}] 枚举名称必须遵守 [Upper Camel Case 命名规则]\n");
            }

            info.Name = blockName;
            // Console.WriteLine("Enum Name: " + match.Groups[1].Value);
            // Console.WriteLine("Contents: " + match.Groups[2].Value);
            var blockContent = match.Groups[2].Value.Trim();
            foreach (var line in blockContent.Split(new string[] { "\r", "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries))
            {
                MessageMember field = new MessageMember();
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
                        var name = fieldSplit[0].Trim();
                        if (!Utility.IsCamelCase(name))
                        {
                            throw new Exception($"[{packageName}] 包的 {name} 枚举字段名称必须遵守 [Upper Camel Case 命名规则]\n");
                        }

                        field.Type = name;
                        field.Members = int.Parse(fieldSplit[1].Replace(";", "").Trim());
                    }
                }
            }
        }
    }

    private static void ParseMessage(string proto, string packageName, List<MessageInfo> codes, bool isGenerateErrorCode = false)
    {
        MatchCollection messageMatches = Regex.Matches(proto, MessagePattern, RegexOptions.Singleline);
        foreach (Match match in messageMatches)
        {
            string messageName = match.Groups[1].Value;
            var blockContent = match.Groups[2].Value.Trim();
            MessageInfo info = new MessageInfo();
            codes.Add(info);
            if (!Utility.IsCamelCase(messageName))
            {
                throw new Exception($"[{packageName}] 包的 [{messageName}] 消息名称必须遵守 [Upper Camel Case 命名规则]\n");
            }

            info.Name = messageName;
            foreach (var line in blockContent.Split(new string[] { "\r", "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries))
            {
                MessageMember field = new MessageMember();
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

                            var name = fieldSplitStrings[2].Trim();

                            if (!Utility.IsCamelCase(name))
                            {
                                throw new Exception($"[{packageName}] 包的 [{messageName}] 消息的 [{name}] 字段名称必须遵守 [Upper Camel Case 命名规则]\n");
                            }

                            field.Name = name;
                        }
                        else if (fieldSplitStrings.Length > 1)
                        {
                            field.Type = Utility.ConvertType(fieldSplitStrings[0].Trim());
                            var name = fieldSplitStrings[1].Trim();
                            if (!Utility.IsCamelCase(name))
                            {
                                throw new Exception($"[{packageName}] 包的 [{messageName}] 消息的 [{name}] 字段名称必须遵守 [Upper Camel Case 命名规则]\n");
                            }

                            field.Name = name;
                        }
                    }
                }
            }

            if (isGenerateErrorCode && info.IsResponse && !info.IsNotify)
            {
                MessageMember field = new MessageMember();
                field.Description = "返回的错误码";
                field.Name = "ErrorCode";
                field.Type = Utility.ConvertType("int32");
                field.Members = 888;
                info.Fields.Add(field);
            }
        }
    }
}