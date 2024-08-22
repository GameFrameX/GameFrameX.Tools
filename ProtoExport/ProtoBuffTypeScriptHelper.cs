using System.Text;

namespace GameFrameX.ProtoExport
{
    /// <summary>
    /// 生成ProtoBuf 协议文件
    /// </summary>
    [Mode(ModeType.TypeScript)]
    internal class ProtoBuffTypeScriptHelper : IProtoGenerateHelper
    {
        public void Run(MessageInfoList messageInfoList, string outputPath, string namespaceName)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("import IRequestMessage from \"../network/IRequestMessage\";\n");
            sb.Append("import IResponseMessage from \"../network/IResponseMessage\";\n");
            sb.Append("import INotifyMessage from \"../network/INotifyMessage\";\n");
            sb.Append("import IHeartBeatMessage from \"../network/IHeartBeatMessage\";\n");
            sb.Append("import MessageObject from \"../network/MessageObject\";\n");
            sb.Append("import ProtoMessageHelper from \"../network/ProtoMessageHelper\";\n");
            sb.AppendLine();
            sb.Append($"export namespace {messageInfoList.ModuleName} {'{'}\n");

            foreach (var operationCodeInfo in messageInfoList.Infos)
            {
                if (operationCodeInfo.IsEnum)
                {
                    sb.Append($"\t/// <summary>\n");
                    sb.Append($"\t/// {operationCodeInfo.Description}\n");
                    sb.Append($"\t/// </summary>\n");
                    sb.AppendLine($"\texport enum {operationCodeInfo.Name}");
                    sb.AppendLine("\t{");
                    foreach (var operationField in operationCodeInfo.Fields)
                    {
                        sb.Append($"\t\t/// <summary>\n");
                        sb.Append($"\t\t/// {operationField.Description}\n");
                        sb.Append($"\t\t/// </summary>\n");
                        sb.Append($"\t\t{operationField.Type} = {operationField.Members}, \n");
                    }

                    sb.AppendLine("\t}\n");
                }
                else
                {
                    sb.Append($"\t/// <summary>\n");
                    sb.Append($"\t/// {operationCodeInfo.Description}\n");
                    sb.Append($"\t/// </summary>\n");
                    // if (operationCodeInfo.IsRequest)
                    // {
                    //     sb.Append($"\t@RegisterRequestMessageClass({operationCodeInfo.Opcode})\n");
                    // }
                    // else
                    // {
                    //     sb.Append($"\t@RegisterResponseMessageClass({operationCodeInfo.Opcode})\n");
                    // }

                    if (string.IsNullOrEmpty(operationCodeInfo.ParentClass))
                    {
                        sb.AppendLine($"\texport class {operationCodeInfo.Name} {'{'}");
                    }
                    else
                    {
                        // sb.AppendLine($"\t[MessageTypeHandler({operationCodeInfo.Opcode})]");
                        sb.AppendLine($"\texport class {operationCodeInfo.Name} extends MessageObject implements {operationCodeInfo.ParentClass} {'{'}");
                        sb.AppendLine();

                        sb.Append($"\t\tpublic static register(): void{'{'}\n");
                        if (operationCodeInfo.IsRequest)
                        {
                            sb.Append($"\t\t\tProtoMessageHelper.registerReqMessage('{messageInfoList.ModuleName}.{operationCodeInfo.Name}', {(messageInfoList.Module << 16) + operationCodeInfo.Opcode});\n");
                        }
                        else
                        {
                            sb.Append($"\t\t\tProtoMessageHelper.registerRespMessage('{messageInfoList.ModuleName}.{operationCodeInfo.Name}', {(messageInfoList.Module << 16) + operationCodeInfo.Opcode});\n");
                        }

                        sb.Append($"\t\t{'}'}\n");
                        
                        
                        sb.Append($"\t\tpublic get PackageName(): string{'{'}\n");
                        sb.Append($"\t\t\treturn '{messageInfoList.ModuleName}.{operationCodeInfo.Name}';\n");
                        sb.Append($"\t\t{'}'}\n");

                        sb.AppendLine();
                    }

                    foreach (var operationField in operationCodeInfo.Fields)
                    {
                        if (!operationField.IsValid)
                        {
                            continue;
                        }

                        if (operationField.IsRepeated)
                        {
                            sb.Append($"\t\t/// <summary>\n");
                            sb.Append($"\t\t/// {operationField.Description}\n");
                            sb.Append($"\t\t/// </summary>\n");
                            sb.Append($"\t\t{operationField.Name}:{ConvertType(operationField.Type)};\n\n");
                        }
                        else
                        {
                            sb.Append($"\t\t/// <summary>\n");
                            sb.Append($"\t\t/// {operationField.Description}\n");
                            sb.Append($"\t\t/// </summary>\n");
                            sb.Append($"\t\t{operationField.Name}:{ConvertType(operationField.Type)};\n\n");
                        }
                    }


                    sb.AppendLine("\t}\n");
                }
            }

            sb.Append("}\n");

            File.WriteAllText(messageInfoList.OutputPath + ".ts", sb.ToString(), Encoding.UTF8);
        }

        static string ConvertType(string type)
        {
            string typeCs = "";
            switch (type)
            {
                case "short":
                case "ushort":
                case "int":
                case "uint":
                case "long":
                case "ulong":
                case "double":
                case "float":
                    typeCs = "number";
                    break;
                case "uint32":
                case "fixed32":
                    typeCs = "uint";
                    break;
                case "string":
                    typeCs = "string";
                    break;
                case "bool":
                    typeCs = "boolean";
                    break;
                default:
                    if (type.StartsWith("Dictionary<"))
                    {
                        var typeMap = type.Replace("Dictionary", string.Empty).Replace("<", string.Empty).Replace(">", string.Empty).Split(',');
                        if (typeMap.Length == 2)
                        {
                            typeCs = $"Map<{ConvertType(typeMap[0].Trim())}, {ConvertType(typeMap[1].Trim())}>";
                            break;
                        }
                    }

                    typeCs = type;
                    break;
            }

            return typeCs;
        }


        public void Post(List<MessageInfoList> operationCodeInfo, string launcherOptionsOutputPath)
        {
            StringBuilder stringBuilder = new StringBuilder();

            foreach (var messageInfoList in operationCodeInfo)
            {
                stringBuilder.AppendLine($"import {{ {messageInfoList.ModuleName} }} from \"./{messageInfoList.ModuleName}_{messageInfoList.Module}\";");
            }

            stringBuilder.AppendLine();
            stringBuilder.AppendLine("export default class ProtoMessageRegister {\n    public static register(): void{");

            foreach (var messageInfoList in operationCodeInfo)
            {
                foreach (var messageInfo in messageInfoList.Infos)
                {
                    if (!string.IsNullOrEmpty(messageInfo.ParentClass))
                    {
                        stringBuilder.AppendLine($"\t\t{messageInfoList.ModuleName}.{messageInfo.Name}.register();");
                    }
                }
            }

            stringBuilder.AppendLine("    }\n}");
            File.WriteAllText(launcherOptionsOutputPath + "/ProtoMessageRegister.ts", stringBuilder.ToString(), Encoding.UTF8);
        }
    }
}