<div align="center">

![GameFrameX Logo](https://download.alianblank.com/gameframex/gameframex_logo_320.png)

# GameFrameX.Tools

[![Version](https://img.shields.io/github/v/release/GameFrameX/GameFrameX.Tools?label=version&color=green)](https://github.com/GameFrameX/GameFrameX.Tools/releases)
[![License](https://img.shields.io/badge/license-MIT+Apache%202.0-orange.svg)](LICENSE)
[![Documentation](https://img.shields.io/badge/docs-gameframex-brightgreen.svg)](https://gameframex.doc.alianblank.com)

**独立游戏前后端一体化解决方案 · 独立游戏开发者的圆梦大使**

[📖 文档](https://gameframex.doc.alianblank.com) • [💬 QQ群: 467608841](https://qm.qq.com/cgi-bin/qm/qr?k=sYFd1nv6m2KZIWFLorZ5pBR0AE5ZhbuL&jump_from=webapi&authKey=oCu+uoL3n35fT5SEt7iLgGtROPxh31n/rHUxRlp0w1f+j38W4tKBuWyRH3KEdwHN)

---

🌐 **语言**: [English](README.md) | **简体中文** | [繁體中文](README.zh-TW.md) | [日本語](README.ja.md) | [한국어](README.ko.md)

---

</div>

# ProtoExport 工具

这是一个用于将Proto协议文件转换为 `Server/Unity/TypeScript` 代码的工具。

# 参数解析

以下是此工具命令行参数的详细说明：

`--mode`
此参数用于指定运行模式。有效值包括 `Server`, `Unity`, 或 `TypeScript` 中的任何一个。

`--inputpath`
此参数用于指定.proto协议文件的路径。程序将扫描该路径下所有以.proto结尾的文件。

`--outputpath`
此参数用于指定输出文件的保存路径。

`--namespaceName`
此参数用于指定命名空间。在TypeScript模式中此参数无效。如果不想设定命名空间，此参数可以传空值。

## 命令行示例

下面的命令示例展示了如何将Proto协议文件转换为Server代码：

```
--mode server --inputpath ./../../../../../Protobuf --outputpath ./../../../../../Server/GameFrameX.Proto/Proto --namespaceName GameFrameX.Proto.Proto
```

在上述命令示例中：

- `--mode server` 表示设置运行模式为 Server。
- `--inputpath ./../../../../../Protobuf` 表示.proto协议文件的路径为 `./../../../../../Protobuf`。
- `--outputpath ./../../../../../Server/GameFrameX.Proto/Proto` 表示输出文件的保存路径为 `./../../../../../Server/GameFrameX.Proto/Proto`。
- `--namespaceName GameFrameX.Proto.Proto` 表示命名空间设定为 `GameFrameX.Proto.Proto`。

更改命令行参数，可以根据实际需求转换合适的代码。
