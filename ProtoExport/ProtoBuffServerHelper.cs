using System.Text;

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
            sb.AddTemplateHeader();

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
                    sb.AppendLine($"\t[System.ComponentModel.Description(\"{operationCodeInfo.Description}\")]");
                    sb.AppendLine($"\tpublic enum {operationCodeInfo.Name}");
                    sb.AppendLine("\t{");
                    for (var index = 0; index < operationCodeInfo.Fields.Count; index++)
                    {
                        var operationField = operationCodeInfo.Fields[index];
                        if (string.IsNullOrEmpty(operationField.Type))
                        {
                            continue;
                        }

                        sb.AppendLine($"\t\t/// <summary>");
                        sb.AppendLine($"\t\t/// {operationField.Description}");
                        sb.AppendLine($"\t\t/// </summary>");
                        sb.AppendLine($"\t\t[System.ComponentModel.Description(\"{operationField.Description}\")]");
                        sb.AppendLine($"\t\t{operationField.Type} = {operationField.Members},");
                        if (index < operationCodeInfo.Fields.Count - 1)
                        {
                            sb.AppendLine();
                        }
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
                    sb.AppendLine($"\t[System.ComponentModel.Description(\"{operationCodeInfo.Description}\")]");
                    if (string.IsNullOrEmpty(operationCodeInfo.ParentClass))
                    {
                        sb.AppendLine($"\tpublic sealed class {operationCodeInfo.Name}");
                    }
                    else
                    {
                        sb.AppendLine($"\t[MessageTypeHandler((({messageInfoList.Module}) << 16) + {operationCodeInfo.Opcode})]");
                        sb.AppendLine($"\tpublic sealed class {operationCodeInfo.Name} : MessageObject, {operationCodeInfo.ParentClass}");
                    }

                    sb.AppendLine("\t{");
                    for (var index = 0; index < operationCodeInfo.Fields.Count; index++)
                    {
                        var operationField = operationCodeInfo.Fields[index];
                        if (!operationField.IsValid)
                        {
                            continue;
                        }

                        sb.AppendLine($"\t\t/// <summary>");
                        sb.AppendLine($"\t\t/// {operationField.Description}");
                        sb.AppendLine($"\t\t/// </summary>");
                        sb.AppendLine($"\t\t[ProtoMember({operationField.Members})]");
                        sb.AppendLine($"\t\t[System.ComponentModel.Description(\"{operationField.Description}\")]");
                        if (operationField.IsRepeated)
                        {
                            sb.AppendLine($"\t\tpublic List<{operationField.Type}> {operationField.Name} {{ get; set; }} = new List<{operationField.Type}>();");
                        }
                        else
                        {
                            string defaultValue = string.Empty;
                            if (operationField.IsKv)
                            {
                                defaultValue = $" = new {operationField.Type}();";
                                sb.AppendLine($"\t\t[ProtoMap(DisableMap = true)]");
                            }

                            sb.AppendLine($"\t\tpublic {operationField.Type} {operationField.Name} {{ get; set; }}{defaultValue}");
                        }

                        if (index < operationCodeInfo.Fields.Count - 1)
                        {
                            sb.AppendLine();
                        }
                    }

                    if (!string.IsNullOrEmpty(operationCodeInfo.ParentClass))
                    {
                        sb.AppendLine();
                        sb.AppendLine("\t\tpublic override void Clear()");
                        sb.AppendLine("\t\t{");
                        for (var index = 0; index < operationCodeInfo.Fields.Count; index++)
                        {
                            var operationField = operationCodeInfo.Fields[index];
                            if (!operationField.IsValid)
                            {
                                continue;
                            }

                            if (operationField.IsRepeated)
                            {
                                sb.AppendLine($"\t\t\t{operationField.Name}.Clear();");
                            }
                            else
                            {
                                if (operationField.IsKv)
                                {
                                    sb.AppendLine($"\t\t\t{operationField.Name}.Clear();");
                                }
                                else
                                {
                                    sb.AppendLine($"\t\t\t{operationField.Name} = default;");
                                }
                            }
                        }

                        sb.AppendLine("\t\t}");
                    }

                    sb.AppendLine("\t}");
                    sb.AppendLine();
                }
            }

            sb.Append("}");
            sb.AppendLine();
            Console.WriteLine($"Generate File:{messageInfoList.OutputPath + ".cs"}");
            File.WriteAllText(messageInfoList.OutputPath + ".cs", sb.ToString(), Encoding.UTF8);
        }

        public void Post(List<MessageInfoList> operationCodeInfo, LauncherOptions launcherOptions)
        {
        }
    }
}