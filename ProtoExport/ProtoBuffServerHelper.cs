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
                    sb.AppendLine($"    /// <summary>");
                    sb.AppendLine($"    /// {operationCodeInfo.Description}");
                    sb.AppendLine($"    /// </summary>");
                    sb.AppendLine($"    [System.ComponentModel.Description(\"{operationCodeInfo.Description}\")]");
                    sb.AppendLine($"    public enum {operationCodeInfo.Name}");
                    sb.AppendLine("    {");
                    for (var index = 0; index < operationCodeInfo.Fields.Count; index++)
                    {
                        var operationField = operationCodeInfo.Fields[index];
                        if (string.IsNullOrEmpty(operationField.Type))
                        {
                            continue;
                        }
                        sb.AppendLine($"        /// <summary>");
                        sb.AppendLine($"        /// {operationField.Description}");
                        sb.AppendLine($"        /// </summary>");
                        sb.AppendLine($"        [System.ComponentModel.Description(\"{operationField.Description}\")]");
                        sb.AppendLine($"        {operationField.Type} = {operationField.Members},");
                        if (index < operationCodeInfo.Fields.Count - 1)
                        {
                            sb.AppendLine();
                        }
                    }

                    sb.AppendLine("    }");
                    sb.AppendLine();
                }
                else
                {
                    sb.AppendLine($"    /// <summary>");
                    sb.AppendLine($"    /// {operationCodeInfo.Description}");
                    sb.AppendLine($"    /// </summary>");
                    sb.AppendLine($"    [ProtoContract]");
                    sb.AppendLine($"    [System.ComponentModel.Description(\"{operationCodeInfo.Description}\")]");
                    if (string.IsNullOrEmpty(operationCodeInfo.ParentClass))
                    {
                        sb.AppendLine($"    public sealed class {operationCodeInfo.Name}");
                    }
                    else
                    {
                        sb.AppendLine($"    [MessageTypeHandler({(messageInfoList.Module << 16) + operationCodeInfo.Opcode})]");
                        sb.AppendLine($"    public sealed class {operationCodeInfo.Name} : MessageObject, {operationCodeInfo.ParentClass}");
                    }

                    sb.AppendLine("    {");
                    for (var index = 0; index < operationCodeInfo.Fields.Count; index++)
                    {
                        var operationField = operationCodeInfo.Fields[index];
                        if (!operationField.IsValid)
                        {
                            continue;
                        }

                        sb.AppendLine($"        /// <summary>");
                        sb.AppendLine($"        /// {operationField.Description}");
                        sb.AppendLine($"        /// </summary>");
                        sb.AppendLine($"        [ProtoMember({operationField.Members})]");
                        sb.AppendLine($"        [System.ComponentModel.Description(\"{operationField.Description}\")]");
                        if (operationField.IsRepeated)
                        {
                            sb.AppendLine($"        public List<{operationField.Type}> {operationField.Name} {{ get; set; }} = new List<{operationField.Type}>();");
                        }
                        else
                        {
                            string defaultValue = string.Empty;
                            if (operationField.IsKv)
                            {
                                defaultValue = $" = new {operationField.Type}();";
                                sb.AppendLine($"        [ProtoMap(DisableMap = true)]");
                            }

                            sb.AppendLine($"        public {operationField.Type} {operationField.Name} {{ get; set; }}{defaultValue}");
                        }

                        if (index < operationCodeInfo.Fields.Count - 1)
                        {
                            sb.AppendLine();
                        }
                    }

                    sb.AppendLine("    }");
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