<div align="center">

![GameFrameX Logo](https://download.alianblank.com/gameframex/gameframex_logo_320.png)

# GameFrameX.Tools

[![Version](https://img.shields.io/github/v/release/GameFrameX/GameFrameX.Tools?label=version&color=green)](https://github.com/GameFrameX/GameFrameX.Tools/releases)
[![License](https://img.shields.io/badge/license-MIT+Apache%202.0-orange.svg)](LICENSE)
[![Documentation](https://img.shields.io/badge/docs-gameframex-brightgreen.svg)](https://gameframex.doc.alianblank.com)

**インディゲーム開発者向けオールインワンソリューション · インディ開発者の夢を支援**

[📖 ドキュメント](https://gameframex.doc.alianblank.com) • [💬 QQグループ: 467608841](https://qm.qq.com/cgi-bin/qm/qr?k=sYFd1nv6m2KZIWFLorZ5pBR0AE5ZhbuL&jump_from=webapi&authKey=oCu+uoL3n35fT5SEt7iLgGtROPxh31n/rHUxRlp0w1f+j38W4tKBuWyRH3KEdwHN)

---

🌐 **言語**: [English](README.md) | [简体中文](README.zh-CN.md) | [繁體中文](README.zh-TW.md) | **日本語** | [한국어](README.ko.md)

---

</div>

# ProtoExport ツール

言語別エクスポートの Proto プロトコルコード生成ツールです。C#、TypeScript に対応、C++ と Lua は開発中です。

# Docker

`linux/amd64` および `linux/arm64` 用の Docker イメージが提供されています。

**Docker Hub**

```bash
docker pull gameframex/gameframex-tools:latest
```

**GitHub Container Registry (GHCR)**

```bash
docker pull ghcr.io/gameframex/gameframex.tools:latest
```

**使用例**

```bash
docker run --rm \
  -v /path/to/protos:/protos \
  -v /path/to/output:/output \
  gameframex/gameframex-tools:latest \
  --mode csharp --isServer true --usingStatements "using System|using ProtoBuf|using System.Collections.Generic|using GameFrameX.NetWork.Abstractions|using GameFrameX.NetWork.Messages" --isGenerateDescription true --inputPath /protos --outputPath /output --namespaceName GameFrameX.Proto.Proto
```

# Proto プロトコル仕様

本ツールは `.proto` ファイルの書式について特定の要件があります。正しくコードを生成するために、以下のルールに従ってください。

## ファイルフォーマット要件

```protobuf
syntax = "proto3";     // 必須：proto3 のみサポート
package Basic;
option module = 10;    // 必須：モジュール ID の定義

// ハートビートリクエスト
message ReqHeartBeat
{
    int64 Timestamp = 1; // タイムスタンプ
}
```

## メッセージ命名規則

- **リクエストメッセージ**：`Req` で始まる必要があります（例：`ReqLogin`、`ReqHeartBeat`）
- **レスポンスメッセージ**：`Resp` で始まる必要があります（例：`RespLogin`）
- **通知メッセージ**：`Notify` で始まる必要があります（例：`NotifyBagInfoChanged`）
- メッセージ名、フィールド名、enum名、enum値はすべて **UpperCamelCase**（アッパーキャメルケース）を使用

## モジュール ID ルール

`option module = <id>;` でモジュール ID を定義します：

| ID 範囲 | 用途 |
|---------|------|
| `0` ~ `32767` | クライアント-サーバー間通信 |
| `-32768` ~ `-1` | サーバー-サーバー間通信 |

## フィールド番号ルール

- メッセージのフィールド番号は **800 未満**である必要があります（800 以上はシステム予約であり、パースエラーの原因になります）
- `ErrorCode` はレスポンスメッセージの予約フィールド名です。手動で定義しないでください。`Resp` メッセージには自動的に `ErrorCode` フィールドが生成されます

## 制限事項

- **ネスト型非対応**：message 内部での `message`、`enum`、その他カスタム型のネストはサポートされていません
- **RPC 定義非対応**：proto ファイル内での RPC サービス定義はサポートされていません
- **proto3 のみ対応**：`syntax = "proto3";` の宣言が必須です。proto2 はサポートされていません

## コメント規約

- message と enum 定義の**上**にコメントを追加：

```protobuf
// ハートビートリクエスト
message ReqHeartBeat
{
    int64 Timestamp = 1;
}
```

- フィールド行の**末尾**にインラインコメントを追加：

```protobuf
// プレイヤー情報
message PlayerInfo
{
    int64 Id = 1;         // プレイヤーID
    string Name = 2;      // プレイヤー名
    uint32 Level = 3;     // プレイヤーレベル
    int32 State = 4;      // プレイヤー状態
}
```

完全なプロトコル仕様については [通信プロトコル仕様](https://gameframex.doc.alianblank.com/zh-CN/protobuf/require.html) と [注意事項](https://gameframex.doc.alianblank.com/zh-CN/protobuf/note.html) のドキュメントを参照してください。

## サンプル Proto ファイル

[TestProtos/](TestProtos/) ディレクトリには、主要なパターンを網羅したサンプル proto ファイルが含まれています：

| ファイル | パターン | モジュール ID |
|----------|----------|--------------|
| `heartbeat.proto` | 基本的な Req/Resp | `1`（クライアント-サーバー） |
| `player.proto` | Req/Resp/Notify + enum + map | `2`（クライアント-サーバー） |
| `bag.proto` | enum + repeated + map + Notify | `3`（クライアント-サーバー） |
| `admin-s.proto` | サーバー専用プロトコル（`-s` 接尾辞） | `99`（クライアント-サーバー） |
| `server-internal-s.proto` | サーバー間通信（負のモジュール ID） | `-1`（サーバー-サーバー） |

---

# パラメータ解説

## コアパラメータ

| パラメータ | 必須 | デフォルト | 説明 |
|-----------|------|-----------|------|
| `--mode` | はい | - | 言語モード：`csharp`、`typescript`、`cpp`、`lua` |
| `--inputPath` | はい | - | `.proto` ファイルのディレクトリパス |
| `--outputPath` | はい | - | 生成ファイルの出力パス |
| `--namespaceName` | いいえ | `""` | 生成コードの名前空間（C# のみ有効、TypeScript では無視） |
| `--isGenerateErrorCode` | いいえ | `true` | レスポンスメッセージに `ErrorCode` フィールドを自動生成するか |

## C# 専用パラメータ

| パラメータ | 必須 | デフォルト | 説明 |
|-----------|------|-----------|------|
| `--usingStatements` | いいえ | `""` | using 文を `\|` で区切って指定（例：`"using System\|using ProtoBuf\|using System.Collections.Generic"`） |
| `--isGenerateDescription` | いいえ | `false` | `[System.ComponentModel.Description]` 属性を生成するか |
| `--isServer` | いいえ | `false` | サーバー専用 proto ファイル（`-s` または `_s` で終わるファイル）を含めるか |

## TypeScript 専用パラメータ

| パラメータ | 必須 | デフォルト | 説明 |
|-----------|------|-----------|------|
| `--importPath` | いいえ | `"../network/"` | 生成される import 文のパスプレフィックス |
| `--isGenerateDescription` | いいえ | `false` | JSDoc スタイルのコメントを生成するか |

## その他のパラメータ

| パラメータ | 必須 | デフォルト | 説明 |
|-----------|------|-----------|------|
| `--isGenerateErrorCodeExcelFile` | いいえ | `true` | エラーコード Excel ファイルを生成するか |
| `--errorCodeExcelFilePath` | いいえ | `""` | エラーコード Excel ファイルのカスタムパス |

---

# モード詳細と例

| モード | 出力言語 | ファイル拡張子 | 説明 |
|--------|---------|--------------|------|
| `csharp` | C# | `.cs` | Server、Unity、Godot、Stride、Flax などに対応 |
| `typescript` | TypeScript | `.ts` | LayaAir、Cocos Creator、Phaser などに対応 |
| `cpp` | C++ | `.h`/`.cpp` | Unreal Engine などに対応（未実装） |
| `lua` | Lua | `.lua` | Defold、Solar2D などに対応（未実装） |

## C# モード

`[ProtoContract]` / `[ProtoMember]` 属性付きの C# コードを生成します。すべての動作は CLI パラメータで制御され、エンジン固有のハードコードはありません。

### サーバーエクスポート

サーバー用の using 文、`[Description]` 属性付きのコードを生成し、サーバー専用 proto ファイルを含めます。

**ローカル実行：**

```bash
dotnet ProtoExport.dll \
  --mode csharp \
  --isServer true \
  --usingStatements "using System|using ProtoBuf|using System.Collections.Generic|using GameFrameX.NetWork.Abstractions|using GameFrameX.NetWork.Messages" \
  --isGenerateDescription true \
  --inputPath ./../../../../../Protobuf \
  --outputPath ./../../../../../Server/GameFrameX.Proto/Proto \
  --namespaceName GameFrameX.Proto.Proto \
  --isGenerateErrorCode true
```

**Docker 実行：**

```bash
docker run --rm \
  -v ./Protobuf:/protos \
  -v ./Server/GameFrameX.Proto/Proto:/output \
  gameframex/gameframex-tools:latest \
  --mode csharp --isServer true \
  --usingStatements "using System|using ProtoBuf|using System.Collections.Generic|using GameFrameX.NetWork.Abstractions|using GameFrameX.NetWork.Messages" \
  --isGenerateDescription true \
  --inputPath /protos --outputPath /output --namespaceName GameFrameX.Proto.Proto
```

### Unity エクスポート

Unity 用の using 文付きコードを生成し、サーバー専用 proto ファイルは自動的にスキップされます。

```bash
dotnet ProtoExport.dll \
  --mode csharp \
  --usingStatements "using System|using ProtoBuf|using System.Collections.Generic|using GameFrameX.Network.Runtime" \
  --inputPath ./../../../../../Protobuf \
  --outputPath ./../../../../../Unity/Assets/Hotfix/Proto \
  --namespaceName Hotfix.Proto \
  --isGenerateErrorCode true
```

### Godot エクスポート

Unity と同様ですが、Godot 固有の名前空間を使用します。

```bash
dotnet ProtoExport.dll \
  --mode csharp \
  --usingStatements "using System|using ProtoBuf|using System.Collections.Generic|using GameFrameX.Network.Runtime" \
  --inputPath ./../../../../../Protobuf \
  --outputPath ./../../../../../Godot/Proto \
  --namespaceName Proto \
  --isGenerateErrorCode true
```

## TypeScript モード

`export namespace`、`export class`、`export enum` を含む `.ts` ファイルと、集約ファイル `ProtoMessageRegister.ts` を生成します。サーバー専用 proto ファイルは自動的にスキップされます。

### デフォルト import パス

```bash
dotnet ProtoExport.dll \
  --mode typescript \
  --inputPath ./../../../../../Protobuf \
  --outputPath ./../../../../../Laya/src/gameframex/protobuf \
  --isGenerateErrorCode true
```

### カスタム import パス

```bash
dotnet ProtoExport.dll \
  --mode typescript \
  --importPath "./lib/network/" \
  --inputPath ./../../../../../Protobuf \
  --outputPath ./../../../../../CocosCreator/assets/scripts/protobuf \
  --isGenerateErrorCode true
```

**Docker 実行：**

```bash
docker run --rm \
  -v ./Protobuf:/protos \
  -v ./Laya/src/gameframex/protobuf:/output \
  gameframex/gameframex-tools:latest \
  --mode typescript --inputPath /protos --outputPath /output
```

## C++ モード（開発中）

```bash
dotnet ProtoExport.dll \
  --mode cpp \
  --inputPath ./../../../../../Protobuf \
  --outputPath ./../../../../../Unreal/Source/Proto
```

## Lua モード（開発中）

```bash
dotnet ProtoExport.dll \
  --mode lua \
  --inputPath ./../../../../../Protobuf \
  --outputPath ./../../../../../Defold/scripts/protobuf
```

---

# クイックエクスポートスクリプト

`Protobuf/` ディレクトリにプリセットのエクスポートスクリプトが用意されています：

| スクリプト | 説明 |
|-----------|------|
| `Proto2CsExport_Server.sh/.bat` | C# サーバー用コードのエクスポート |
| `Proto2CsExport_Client.sh/.bat` | C# Unity クライアント用コードのエクスポート |
| `Proto2TsExport.sh/.bat` | TypeScript コードのエクスポート |

---

# Docker パスマッピング

Docker 使用時のパスマッピングは以下の通りです：

- `-v <ホストパス>:<コンテナパス>` でホストのディレクトリをコンテナにマウントします
- `--inputPath` と `--outputPath` には**コンテナ内パス**（例：`/protos`、`/output`）を指定します（ホストパスではありません）

```bash
# 例：ホスト ./my-protos -> コンテナ /protos
docker run --rm \
  -v $(pwd)/my-protos:/protos \
  -v $(pwd)/my-output:/output \
  gameframex/gameframex-tools:latest \
  --mode csharp --isServer true \
  --usingStatements "using System|using ProtoBuf|using System.Collections.Generic|using GameFrameX.NetWork.Abstractions|using GameFrameX.NetWork.Messages" \
  --isGenerateDescription true \
  --inputPath /protos --outputPath /output --namespaceName GameFrameX.Proto.Proto
```
