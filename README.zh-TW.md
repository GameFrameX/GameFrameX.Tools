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

# Docker

預建構的 Docker 映像檔支援 `linux/amd64` 和 `linux/arm64` 架構。

**Docker Hub**

```bash
docker pull gameframex/gameframex-tools:latest
```

**GitHub Container Registry (GHCR)**

```bash
docker pull ghcr.io/gameframex/gameframex.tools:latest
```

**使用範例**

```bash
docker run --rm \
  -v /path/to/protos:/protos \
  -v /path/to/output:/output \
  gameframex/gameframex-tools:latest \
  --mode server --inputpath /protos --outputpath /output --namespaceName GameFrameX.Proto.Proto
```

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

## 模式詳情與範例

| 模式 | 輸出語言 | 命名空間行為 | 服務端專屬 Proto |
|------|---------|-------------|-----------------|
| `Server` | C# | 使用 `--namespaceName` 的值 | 包含 |
| `Unity` | C# | 使用 `--namespaceName` 的值 | 跳過（以 `-s` 或 `_s` 結尾的檔案） |
| `TypeScript` | TypeScript (.ts) | `--namespaceName` 無效 | 跳過（以 `-s` 或 `_s` 結尾的檔案） |
| `Godot` | C# | 固定使用 `GameFrameX.Network.Runtime` | 跳過（以 `-s` 或 `_s` 結尾的檔案） |

### Server 模式

生成帶有 `[System.ComponentModel.Description]` 特性的 C# 程式碼，包含服務端專屬 proto 檔案。

**本機執行：**

```bash
dotnet run --project ProtoExport -- \
  --mode server \
  --inputpath ./Protobuf \
  --outputpath ./Server/GameFrameX.Proto/Proto \
  --namespaceName GameFrameX.Proto.Proto
```

**Docker 執行：**

```bash
docker run --rm \
  -v ./Protobuf:/protos \
  -v ./Server/GameFrameX.Proto/Proto:/output \
  gameframex/gameframex-tools:latest \
  --mode server --inputpath /protos --outputpath /output --namespaceName GameFrameX.Proto.Proto
```

### Unity 模式

生成使用 `GameFrameX.Network.Runtime` 命名空間的 C# 程式碼（不含 `[Description]` 特性），自動跳過服務端專屬 proto 檔案。

**本機執行：**

```bash
dotnet run --project ProtoExport -- \
  --mode unity \
  --inputpath ./Protobuf \
  --outputpath ./Unity/Assets/Hotfix/Proto \
  --namespaceName Hotfix.Proto
```

**Docker 執行：**

```bash
docker run --rm \
  -v ./Protobuf:/protos \
  -v ./Unity/Assets/Hotfix/Proto:/output \
  gameframex/gameframex-tools:latest \
  --mode unity --inputpath /protos --outputpath /output --namespaceName Hotfix.Proto
```

### TypeScript 模式

生成包含 `export namespace`、`export class` 和 `export enum` 的 `.ts` 檔案，並生成聚合檔案 `ProtoMessageRegister.ts`。`--namespaceName` 參數在此模式下無效。自動跳過服務端專屬 proto 檔案。

**本機執行：**

```bash
dotnet run --project ProtoExport -- \
  --mode typescript \
  --inputpath ./Protobuf \
  --outputpath ./Laya/src/gameframex/protobuf
```

**Docker 執行：**

```bash
docker run --rm \
  -v ./Protobuf:/protos \
  -v ./Laya/src/gameframex/protobuf:/output \
  gameframex/gameframex-tools:latest \
  --mode typescript --inputpath /protos --outputpath /output
```

### Godot 模式

生成固定使用 `GameFrameX.Network.Runtime` 命名空間的 C# 程式碼（`--namespaceName` 參數會被忽略）。自動跳過服務端專屬 proto 檔案。

**本機執行：**

```bash
dotnet run --project ProtoExport -- \
  --mode godot \
  --inputpath ./Protobuf \
  --outputpath ./Godot/Proto
```

**Docker 執行：**

```bash
docker run --rm \
  -v ./Protobuf:/protos \
  -v ./Godot/Proto:/output \
  gameframex/gameframex-tools:latest \
  --mode godot --inputpath /protos --outputpath /output
```

## Docker 路徑映射說明

使用 Docker 時，路徑映射規則如下：

- `-v <宿主機路徑>:<容器內路徑>` 將宿主機目錄掛載到容器中
- `--inputpath` 和 `--outputpath` 必須填寫**容器內路徑**（如 `/protos`、`/output`），而非宿主機路徑

```bash
# 範例：宿主機 ./my-protos -> 容器內 /protos
docker run --rm \
  -v $(pwd)/my-protos:/protos \    # 宿主機路徑 : 容器內路徑
  -v $(pwd)/my-output:/output \    # 宿主機路徑 : 容器內路徑
  gameframex/gameframex-tools:latest \
  --mode server --inputpath /protos --outputpath /output --namespaceName GameFrameX.Proto.Proto
```
