﻿using System.Text;

namespace GameFrameX.ProtoExport
{
    /// <summary>
    /// 生成ProtoBuf 协议文件
    /// </summary>
    [Mode(ModeType.Server)]
    internal class ProtoBuffServerHelper : IProtoGenerateHelper
    {
        public void Run(MessageInfoList messageInfoList, string outputPath, string namespaceName = "Hotfix")
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("using System;");
            sb.AppendLine("using ProtoBuf;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using GameFrameX.NetWork.Abstractions;");
            sb.AppendLine("using GameFrameX.NetWork.Messages;");


            sb.AppendLine();
            sb.AppendLine($"namespace {namespaceName}");
            sb.AppendLine("{");

            foreach (var operationCodeInfo in messageInfoList.Infos)
            {
                if (operationCodeInfo.IsEnum)
                {
                    sb.AppendLine($"\t/// <summary>");
                    sb.AppendLine($"\t/// {operationCodeInfo.Description}");
                    sb.AppendLine($"\t/// </summary>");
                    sb.AppendLine($"\tpublic enum {operationCodeInfo.Name}");
                    sb.AppendLine("\t{");
                    foreach (var operationField in operationCodeInfo.Fields)
                    {
                        sb.AppendLine($"\t\t/// <summary>");
                        sb.AppendLine($"\t\t/// {operationField.Description}");
                        sb.AppendLine($"\t\t/// </summary>");
                        sb.AppendLine($"\t\t{operationField.Type} = {operationField.Members}, ");
                    }

                    sb.AppendLine("\t}");
                    sb.AppendLine();
                }
                else
                {
                    sb.AppendLine($"\t/// <summary>");
                    sb.AppendLine($"\t/// {operationCodeInfo.Description}");
                    sb.AppendLine($"\t/// </summary>");
                    sb.AppendLine($"\t[ProtoContract]");
                    if (string.IsNullOrEmpty(operationCodeInfo.ParentClass))
                    {
                        sb.AppendLine($"\tpublic sealed class {operationCodeInfo.Name}");
                    }
                    else
                    {
                        sb.AppendLine($"\t[MessageTypeHandler({(messageInfoList.Module << 16) + operationCodeInfo.Opcode})]");
                        sb.AppendLine($"\tpublic sealed class {operationCodeInfo.Name} : MessageObject, {operationCodeInfo.ParentClass}");
                    }

                    sb.AppendLine("\t{");
                    foreach (var operationField in operationCodeInfo.Fields)
                    {
                        if (!operationField.IsValid)
                        {
                            continue;
                        }

                        sb.AppendLine($"\t\t/// <summary>");
                        sb.AppendLine($"\t\t/// {operationField.Description}");
                        sb.AppendLine($"\t\t/// </summary>");
                        sb.AppendLine($"\t\t[ProtoMember({operationField.Members})]");
                        if (operationField.IsRepeated)
                        {
                            sb.AppendLine($"\t\tpublic List<{operationField.Type}> {operationField.Name} = new List<{operationField.Type}>();");
                        }
                        else
                        {
                            string defaultValue = string.Empty;
                            if (operationField.IsKv)
                            {
                                defaultValue = $" = new {operationField.Type}();";
                            }

                            sb.AppendLine($"\t\tpublic {operationField.Type} {operationField.Name} {{ get; set; }}{defaultValue}");
                        }

                        sb.AppendLine();
                    }

                    sb.AppendLine("\t}");
                    sb.AppendLine();
                }
            }

            sb.Append("}");
            sb.AppendLine();

            File.WriteAllText(messageInfoList.OutputPath + ".cs", sb.ToString(), Encoding.UTF8);
        }

        public void Post(List<MessageInfoList> operationCodeInfo, string launcherOptionsOutputPath)
        {
            
        }
    }
}