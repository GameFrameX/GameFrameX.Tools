namespace GameFrameX.ProtoExport
{
    internal static class TypeMapper
    {
        public static string ToCSharp(string type)
        {
            string typeCs = "";
            switch (type)
            {
                case "int16":
                    typeCs = "short";
                    break;
                case "uint16":
                    typeCs = "ushort";
                    break;
                case "int32":
                case "sint32":
                case "sfixed32":
                    typeCs = "int";
                    break;
                case "uint32":
                case "fixed32":
                    typeCs = "uint";
                    break;
                case "int64":
                case "sint64":
                case "sfixed64":
                    typeCs = "long";
                    break;
                case "uint64":
                case "fixed64":
                    typeCs = "ulong";
                    break;
                case "bytes":
                    typeCs = "byte[]";
                    break;
                case "string":
                    typeCs = "string";
                    break;
                case "bool":
                    typeCs = "bool";
                    break;
                case "double":
                    typeCs = "double";
                    break;
                case "float":
                    typeCs = "float";
                    break;
                default:
                    if (type.StartsWith("map<"))
                    {
                        var typeMap = type.Replace("map", string.Empty).Replace("<", string.Empty).Replace(">", string.Empty).Split(',');
                        if (typeMap.Length == 2)
                        {
                            typeCs = $"Dictionary<{ToCSharp(typeMap[0])}, {ToCSharp(typeMap[1])}>";
                            break;
                        }
                    }

                    typeCs = type;
                    break;
            }

            return typeCs;
        }

        public static string ToTypeScript(string type)
        {
            string typeTs = "";
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
                    typeTs = "number";
                    break;
                case "uint32":
                case "fixed32":
                    typeTs = "uint";
                    break;
                case "string":
                    typeTs = "string";
                    break;
                case "bool":
                    typeTs = "boolean";
                    break;
                case "byte[]":
                    typeTs = "Uint8Array";
                    break;
                default:
                    if (type.StartsWith("Dictionary<"))
                    {
                        var typeMap = type.Replace("Dictionary", string.Empty).Replace("<", string.Empty).Replace(">", string.Empty).Split(',');
                        if (typeMap.Length == 2)
                        {
                            typeTs = $"Map<{ToTypeScript(typeMap[0].Trim())}, {ToTypeScript(typeMap[1].Trim())}>";
                            break;
                        }
                    }

                    typeTs = type;
                    break;
            }

            return typeTs;
        }

        public static string ToTypeScriptFromProto(string type)
        {
            switch (type)
            {
                case "int16":
                case "uint16":
                case "int32":
                case "sint32":
                case "sfixed32":
                case "uint32":
                case "fixed32":
                case "int64":
                case "sint64":
                case "sfixed64":
                case "uint64":
                case "fixed64":
                case "double":
                case "float":
                    return "number";
                case "string":
                    return "string";
                case "bool":
                    return "boolean";
                case "bytes":
                    return "Uint8Array";
                default:
                    if (type.StartsWith("map<"))
                    {
                        var typeMap = type.Replace("map", string.Empty).Replace("<", string.Empty).Replace(">", string.Empty).Split(',');
                        if (typeMap.Length == 2)
                        {
                            return $"Map<{ToTypeScriptFromProto(typeMap[0].Trim())}, {ToTypeScriptFromProto(typeMap[1].Trim())}>";
                        }
                    }

                    return type;
            }
        }

        public static string ToCpp(string type)
        {
            throw new NotImplementedException("C++ type mapping is not yet implemented.");
        }

        public static string ToLua(string type)
        {
            throw new NotImplementedException("Lua type mapping is not yet implemented.");
        }
    }
}
