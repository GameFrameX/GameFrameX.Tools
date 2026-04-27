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

# Proto 協議規範

本工具對 `.proto` 檔案格式有特定要求，請遵循以下規則以確保正確生成程式碼。

## 檔案格式要求

```protobuf
syntax = "proto3";     // 必須宣告：僅支援 proto3
package Basic;
option module = 10;    // 必須定義：模組 ID

// 請求心跳
message ReqHeartBeat
{
    int64 Timestamp = 1; // 時間戳
}
```

## 訊息命名規則

- **請求訊息**：必須以 `Req` 開頭（如 `ReqLogin`、`ReqHeartBeat`）
- **回應訊息**：必須以 `Resp` 開頭（如 `RespLogin`）
- **通知訊息**：必須以 `Notify` 開頭（如 `NotifyBagInfoChanged`）
- 訊息名稱、欄位名稱、列舉名稱和列舉值必須使用 **UpperCamelCase**（大駝峰）命名

## 模組 ID 規則

透過 `option module = <id>;` 定義模組 ID：

| ID 範圍 | 用途 |
|---------|------|
| `0` ~ `32767` | 用戶端-伺服器端通訊 |
| `-32768` ~ `-1` | 伺服器端-伺服器端通訊 |

## 欄位編號規則

- 訊息欄位編號必須**小於 800**（大於等於 800 為系統保留，會導致解析異常）
- `ErrorCode` 是回應訊息中的保留欄位名稱，請勿手動定義。`Resp` 訊息會自動生成 `ErrorCode` 欄位

## 限制事項

- **不支援巢狀型別**：不允許在 message 內部巢狀 `message`、`enum` 或任何自訂型別
- **不支援 RPC 定義**：proto 檔案中不允許使用 RPC 服務定義
- **僅支援 proto3**：必須宣告 `syntax = "proto3";`，不支援 proto2

## 註解規範

- 在 message 和 enum 定義**上方**新增註解：

```protobuf
// 請求心跳
message ReqHeartBeat
{
    int64 Timestamp = 1;
}
```

- 在欄位行**末尾**新增行內註解：

```protobuf
// 玩家資訊
message PlayerInfo
{
    int64 Id = 1;         // 角色ID
    string Name = 2;      // 角色名
    uint32 Level = 3;     // 角色等級
    int32 State = 4;      // 角色狀態
}
```

完整的協議規範請參考 [通訊協議規範](https://gameframex.doc.alianblank.com/zh-CN/protobuf/require.html) 和 [注意事項](https://gameframex.doc.alianblank.com/zh-CN/protobuf/note.html) 文件。

## 範例 Proto 檔案

[TestProtos/](TestProtos/) 目錄下提供了覆蓋所有主要模式的範例 proto 檔案：

| 檔案 | 模式 | 模組 ID |
|------|------|---------|
| `heartbeat.proto` | 基礎 Req/Resp | `1`（用戶端-伺服器端） |
| `player.proto` | Req/Resp/Notify + enum + map | `2`（用戶端-伺服器端） |
| `bag.proto` | enum + repeated + map + Notify | `3`（用戶端-伺服器端） |
| `admin-s.proto` | 伺服器端專屬協議（`-s` 後綴） | `99`（用戶端-伺服器端） |
| `server-internal-s.proto` | 伺服器端間通訊（負值模組 ID） | `-1`（伺服器端-伺服器端） |

---

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
