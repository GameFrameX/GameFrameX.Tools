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

        Console.WriteLine($"Package: {packageMatch.Groups[1].Value} => Module: {moduleMatch.Groups[1].Value}");
        // 使用正则表达式提取枚举类型
        ParseEnum(proto, messageInfo.Infos);

        // 使用正则表达式提取消息类型
        ParseMessage(proto, messageInfo.Infos, isGenerateErrorCode);

        ParseComment(proto, messageInfo.Infos);

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

    private static void ParseComment(string proto, List<MessageInfo> operationCodeInfos)
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

    private static void ParseEnum(string proto, List<MessageInfo> codes)
    {
        MatchCollection enumMatches = Regex.Matches(proto, EnumPattern, RegexOptions.Singleline);
        foreach (Match match in enumMatches)
        {
            MessageInfo info = new MessageInfo(true);
            codes.Add(info);
            string blockName = match.Groups[1].Value;
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
                        field.Type = fieldSplit[0].Trim();
                        field.Members = int.Parse(fieldSplit[1].Replace(";", "").Trim());
                    }
                }
            }
        }
    }

    private static void ParseMessage(string proto, List<MessageInfo> codes, bool isGenerateErrorCode = false)
    {
        MatchCollection messageMatches = Regex.Matches(proto, MessagePattern, RegexOptions.Singleline);
        foreach (Match match in messageMatches)
        {
            string messageName = match.Groups[1].Value;
            // Console.WriteLine("Message Name: " + match.Groups[1].Value);
            // Console.WriteLine("Contents: " + match.Groups[2].Value);
            var blockContent = match.Groups[2].Value.Trim();
            MessageInfo info = new MessageInfo();
            codes.Add(info);
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

    [GeneratedRegex(ModulePattern, RegexOptions.Singleline)]
    private static partial Regex MyRegex();
}