<div align="center">

![GameFrameX Logo](https://download.alianblank.com/gameframex/gameframex_logo_320.png)

# GameFrameX.Tools

[![Version](https://img.shields.io/github/v/release/GameFrameX/GameFrameX.Tools?label=version&color=green)](https://github.com/GameFrameX/GameFrameX.Tools/releases)
[![License](https://img.shields.io/badge/license-MIT+Apache%202.0-orange.svg)](LICENSE)
[![Documentation](https://img.shields.io/badge/docs-gameframex-brightgreen.svg)](https://gameframex.doc.alianblank.com)

**獨立遊戲前後端一體化解決方案 · 獨立遊戲開發者的圓夢大使**

[📖 文檔](https://gameframex.doc.alianblank.com) • [💬 QQ群: 467608841](https://qm.qq.com/cgi-bin/qm/qr?k=sYFd1nv6m2KZIWFLorZ5pBR0AE5ZhbuL&jump_from=webapi&authKey=oCu+uoL3n35fT5SEt7iLgGtROPxh31n/rHUxRlp0w1f+j38W4tKBuWyRH3KEdwHN)

---

🌐 **語言**: [English](README.md) | [简体中文](README.zh-CN.md) | **繁體中文** | [日本語](README.ja.md) | [한국어](README.ko.md)

---

</div>

# ProtoExport 工具

這是一個用於將Proto協議文件轉換為 `Server/Unity/TypeScript/Godot` 程式碼的工具。

# 參數解析

以下是此工具命令列參數的詳細說明：

`--mode`
此參數用於指定執行模式。有效值包括 `Server`, `Unity`, `TypeScript`, 或 `Godot` 中的任何一個。

`--inputpath`
此參數用於指定.proto協議檔案的路徑。程式將掃描該路徑下所有以.proto結尾的檔案。

`--outputpath`
此參數用於指定輸出檔案的儲存路徑。

`--namespaceName`
此參數用於指定命名空間。在TypeScript模式中此參數無效。在Godot模式中，生成的程式碼始終使用 `GameFrameX.Network.Runtime` 命名空間。如果不想設定命名空間，此參數可以傳空值。

## 命令列範例

下面的命令範例展示了如何將Proto協議文件轉換為Server程式碼：

```
--mode server --inputpath ./../../../../../Protobuf --outputpath ./../../../../../Server/GameFrameX.Proto/Proto --namespaceName GameFrameX.Proto.Proto
```

在上述命令範例中：

- `--mode server` 表示設定執行模式為 Server。
- `--inputpath ./../../../../../Protobuf` 表示.proto協議檔案的路徑為 `./../../../../../Protobuf`。
- `--outputpath ./../../../../../Server/GameFrameX.Proto/Proto` 表示輸出檔案的儲存路徑為 `./../../../../../Server/GameFrameX.Proto/Proto`。
- `--namespaceName GameFrameX.Proto.Proto` 表示命名空間設定為 `GameFrameX.Proto.Proto`。

更改命令列參數，可以根據實際需求轉換合適的程式碼。

### Godot 模式範例

下面的命令範例展示了如何將Proto協議文件轉換為Godot C#程式碼：

```
--mode godot --inputpath ./../../../../../Protobuf --outputpath ./../../../../../Godot/Proto --namespaceName Hotfix.Proto
```

在上述命令範例中：

- `--mode godot` 表示設定執行模式為 Godot。
- `--inputpath ./../../../../../Protobuf` 表示.proto協議檔案的路徑為 `./../../../../../Protobuf`。
- `--outputpath ./../../../../../Godot/Proto` 表示輸出檔案的儲存路徑為 `./../../../../../Godot/Proto`。
- `--namespaceName Hotfix.Proto` 表示命名空間設定為 `Hotfix.Proto`。服務端專屬的proto檔案（以 `-s` 或 `_s` 結尾）會被自動跳過。
