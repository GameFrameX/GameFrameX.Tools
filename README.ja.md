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

## コマンドライン例

以下のコマンド例は、Proto プロトコルファイルを Server コードに変換する方法を示しています：

```
--mode server --inputpath ./../../../../../Protobuf --outputpath ./../../../../../Server/GameFrameX.Proto/Proto --namespaceName GameFrameX.Proto.Proto
```

上記のコマンド例では：

- `--mode server` は実行モードを Server に設定します。
- `--inputpath ./../../../../../Protobuf` は .proto プロトコルファイルのパスを `./../../../../../Protobuf` に設定します。
- `--outputpath ./../../../../../Server/GameFrameX.Proto/Proto` は出力ファイルのパスを `./../../../../../Server/GameFrameX.Proto/Proto` に設定します。
- `--namespaceName GameFrameX.Proto.Proto` は名前空間を `GameFrameX.Proto.Proto` に設定します。

コマンドラインパラメータを変更することで、実際のニーズに合わせたコードを生成できます。

### Godot モードの例

以下のコマンド例は、Proto プロトコルファイルを Godot C# コードに変換する方法を示しています：

```
--mode godot --inputpath ./../../../../../Protobuf --outputpath ./../../../../../Godot/Proto --namespaceName Hotfix.Proto
```

上記のコマンド例では：

- `--mode godot` は実行モードを Godot に設定します。
- `--inputpath ./../../../../../Protobuf` は .proto プロトコルファイルのパスを `./../../../../../Protobuf` に設定します。
- `--outputpath ./../../../../../Godot/Proto` は出力ファイルのパスを `./../../../../../Godot/Proto` に設定します。
- `--namespaceName Hotfix.Proto` は名前空間を `Hotfix.Proto` に設定します。サーバー専用の proto ファイル（`-s` または `_s` で終わるもの）は自動的にスキップされます。
