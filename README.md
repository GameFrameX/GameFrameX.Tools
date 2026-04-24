<div align="center">

![GameFrameX Logo](https://download.alianblank.com/gameframex/gameframex_logo_320.png)

# GameFrameX.Tools

[![Version](https://img.shields.io/github/v/release/GameFrameX/GameFrameX.Tools?label=version&color=green)](https://github.com/GameFrameX/GameFrameX.Tools/releases)
[![License](https://img.shields.io/badge/license-MIT+Apache%202.0-orange.svg)](LICENSE)
[![Documentation](https://img.shields.io/badge/docs-gameframex-brightgreen.svg)](https://gameframex.doc.alianblank.com)

**All-in-One Solution for Indie Game Development · Empowering Indie Developers' Dreams**

[📖 Documentation](https://gameframex.doc.alianblank.com) • [💬 QQ Group: 467608841](https://qm.qq.com/cgi-bin/qm/qr?k=sYFd1nv6m2KZIWFLorZ5pBR0AE5ZhbuL&jump_from=webapi&authKey=oCu+uoL3n35fT5SEt7iLgGtROPxh31n/rHUxRlp0w1f+j38W4tKBuWyRH3KEdwHN)

---

🌐 **Language**: **English** | [简体中文](README.zh-CN.md) | [繁體中文](README.zh-TW.md) | [日本語](README.ja.md) | [한국어](README.ko.md)

---

</div>

# ProtoExport Tool

A tool for converting Proto protocol files into `Server/Unity/TypeScript/Godot` code.

# Docker

Pre-built Docker images are available for `linux/amd64` and `linux/arm64`.

**Docker Hub**

```bash
docker pull gameframex/gameframex-tools:latest
```

**GitHub Container Registry (GHCR)**

```bash
docker pull ghcr.io/gameframex/gameframex.tools:latest
```

**Usage**

```bash
docker run --rm \
  -v /path/to/protos:/protos \
  -v /path/to/output:/output \
  gameframex/gameframex-tools:latest \
  --mode server --inputpath /protos --outputpath /output --namespaceName GameFrameX.Proto.Proto
```

# Parameter Reference

Below is a detailed description of the command-line parameters for this tool.

`--mode`
This parameter specifies the run mode. Valid values include `Server`, `Unity`, `TypeScript`, or `Godot`.

`--inputpath`
This parameter specifies the path to the .proto protocol files. The program will scan all files ending with .proto under the specified path.

`--outputpath`
This parameter specifies the output path for the generated files.

`--namespaceName`
This parameter specifies the namespace. This parameter has no effect in TypeScript mode. In Godot mode, the generated code always uses `GameFrameX.Network.Runtime` namespace. If you do not want to set a namespace, pass an empty value.

## Command Line Example

The following command example demonstrates how to convert Proto protocol files into Server code:

```
--mode server --inputpath ./../../../../../Protobuf --outputpath ./../../../../../Server/GameFrameX.Proto/Proto --namespaceName GameFrameX.Proto.Proto
```

In the above example:

- `--mode server` sets the run mode to Server.
- `--inputpath ./../../../../../Protobuf` sets the .proto protocol file path to `./../../../../../Protobuf`.
- `--outputpath ./../../../../../Server/GameFrameX.Proto/Proto` sets the output file path to `./../../../../../Server/GameFrameX.Proto/Proto`.
- `--namespaceName GameFrameX.Proto.Proto` sets the namespace to `GameFrameX.Proto.Proto`.

You can adjust the command-line parameters to generate the appropriate code for your needs.

### Godot Mode Example

The following command example demonstrates how to convert Proto protocol files into Godot C# code:

```
--mode godot --inputpath ./../../../../../Protobuf --outputpath ./../../../../../Godot/Proto --namespaceName Hotfix.Proto
```

In the above example:

- `--mode godot` sets the run mode to Godot.
- `--inputpath ./../../../../../Protobuf` sets the .proto protocol file path to `./../../../../../Protobuf`.
- `--outputpath ./../../../../../Godot/Proto` sets the output file path to `./../../../../../Godot/Proto`.
- `--namespaceName Hotfix.Proto` sets the namespace to `Hotfix.Proto`. Server-only proto files (ending with `-s` or `_s`) are automatically skipped.
