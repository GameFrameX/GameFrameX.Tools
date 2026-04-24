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

Proto プロトコルファイルを `Server/Unity/TypeScript/Godot` コードに変換するツールです。

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
  --mode server --inputpath /protos --outputpath /output --namespaceName GameFrameX.Proto.Proto
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

---

# パラメータ解説

このツールのコマンドラインパラメータの詳細説明は以下の通りです。

`--mode`
実行モードを指定します。有効な値は `Server`, `Unity`, `TypeScript`, または `Godot` のいずれかです。

`--inputpath`
.proto プロトコルファイルのパスを指定します。プログラムは指定されたパス以下のすべての .proto ファイルをスキャンします。

`--outputpath`
生成されたファイルの出力パスを指定します。

`--namespaceName`
名前空間を指定します。TypeScript モードではこのパラメータは無効です。Godot モードでは、生成されたコードは常に `GameFrameX.Network.Runtime` 名前空間を使用します。名前空間を設定しない場合は空の値を渡してください。

## モード詳細と例

| モード | 出力言語 | 名前空間の動作 | サーバー専用 Proto |
|--------|---------|--------------|-------------------|
| `Server` | C# | `--namespaceName` の値を使用 | 含む |
| `Unity` | C# | `--namespaceName` の値を使用 | スキップ（`-s` または `_s` で終わるファイル） |
| `TypeScript` | TypeScript (.ts) | `--namespaceName` は無効 | スキップ（`-s` または `_s` で終わるファイル） |
| `Godot` | C# | 常に `GameFrameX.Network.Runtime` を使用 | スキップ（`-s` または `_s` で終わるファイル） |

### Server モード

`[System.ComponentModel.Description]` 属性付きの C# コードを生成します。サーバー専用 proto ファイルを含みます。

**ローカル実行：**

```bash
dotnet run --project ProtoExport -- \
  --mode server \
  --inputpath ./Protobuf \
  --outputpath ./Server/GameFrameX.Proto/Proto \
  --namespaceName GameFrameX.Proto.Proto
```

**Docker 実行：**

```bash
docker run --rm \
  -v ./Protobuf:/protos \
  -v ./Server/GameFrameX.Proto/Proto:/output \
  gameframex/gameframex-tools:latest \
  --mode server --inputpath /protos --outputpath /output --namespaceName GameFrameX.Proto.Proto
```

### Unity モード

`GameFrameX.Network.Runtime` 名前空間を使用する C# コードを生成します（`[Description]` 属性なし）。サーバー専用 proto ファイルは自動的にスキップされます。

**ローカル実行：**

```bash
dotnet run --project ProtoExport -- \
  --mode unity \
  --inputpath ./Protobuf \
  --outputpath ./Unity/Assets/Hotfix/Proto \
  --namespaceName Hotfix.Proto
```

**Docker 実行：**

```bash
docker run --rm \
  -v ./Protobuf:/protos \
  -v ./Unity/Assets/Hotfix/Proto:/output \
  gameframex/gameframex-tools:latest \
  --mode unity --inputpath /protos --outputpath /output --namespaceName Hotfix.Proto
```

### TypeScript モード

`export namespace`、`export class`、`export enum` を含む `.ts` ファイルと、集約ファイル `ProtoMessageRegister.ts` を生成します。このモードでは `--namespaceName` パラメータは無効です。サーバー専用 proto ファイルは自動的にスキップされます。

**ローカル実行：**

```bash
dotnet run --project ProtoExport -- \
  --mode typescript \
  --inputpath ./Protobuf \
  --outputpath ./Laya/src/gameframex/protobuf
```

**Docker 実行：**

```bash
docker run --rm \
  -v ./Protobuf:/protos \
  -v ./Laya/src/gameframex/protobuf:/output \
  gameframex/gameframex-tools:latest \
  --mode typescript --inputpath /protos --outputpath /output
```

### Godot モード

固定で `GameFrameX.Network.Runtime` 名前空間を使用する C# コードを生成します（`--namespaceName` パラメータは無視されます）。サーバー専用 proto ファイルは自動的にスキップされます。

**ローカル実行：**

```bash
dotnet run --project ProtoExport -- \
  --mode godot \
  --inputpath ./Protobuf \
  --outputpath ./Godot/Proto
```

**Docker 実行：**

```bash
docker run --rm \
  -v ./Protobuf:/protos \
  -v ./Godot/Proto:/output \
  gameframex/gameframex-tools:latest \
  --mode godot --inputpath /protos --outputpath /output
```

## Docker パスマッピング

Docker 使用時のパスマッピングは以下の通りです：

- `-v <ホストパス>:<コンテナパス>` でホストのディレクトリをコンテナにマウントします
- `--inputpath` と `--outputpath` には**コンテナ内パス**（例：`/protos`、`/output`）を指定します（ホストパスではありません）

```bash
# 例：ホスト ./my-protos -> コンテナ /protos
docker run --rm \
  -v $(pwd)/my-protos:/protos \     # ホストパス : コンテナパス
  -v $(pwd)/my-output:/output \     # ホストパス : コンテナパス
  gameframex/gameframex-tools:latest \
  --mode server --inputpath /protos --outputpath /output --namespaceName GameFrameX.Proto.Proto
```
