# GameFrameX.Tools

# ProtoExport 工具

这是一个用于将Proto协议文件转换为 `Server/Unity/TypeScript` 代码的工具。

# 参数解析

以下是此工具命令行参数的详细说明：

`-m, --mode`    
此参数用于指定运行模式。有效值包括 `Server`, `Unity`, 或 `TypeScript` 中的任何一个。

`-i, --inputpath`    
此参数用于指定.proto协议文件的路径。程序将扫描该路径下所有以.proto结尾的文件。

`-o, --outputpath`    
此参数用于指定输出文件的保存路径。

`-n, --namespaceName`   
此参数用于指定命名空间。在TypeScript模式中此参数无效。如果不想设定命名空间，此参数可以传空值。

## 命令行示例

下面的命令示例展示了如何将Proto协议文件转换为Server代码：

```
-m server -i ./../../../../../Protobuf -o ./../../../../../Server/GameFrameX.Proto/Proto -n GameFrameX.Proto.Proto
```

在上述命令示例中：

- `-m server` 表示设置运行模式为 Server。
- `-i ./../../../../../Protobuf` 表示.proto协议文件的路径为 `./../../../../../Protobuf`。
- `-o ./../../../../../Server/GameFrameX.Proto/Proto` 表示输出文件的保存路径为 `./../../../../../Server/GameFrameX.Proto/Proto`。
- `-n GameFrameX.Proto.Proto` 表示命名空间设定为 `GameFrameX.Proto.Proto`。

更改命令行参数，可以根据实际需求转换合适的代码。