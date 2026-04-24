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

# Proto Protocol Specification

This tool has specific requirements for `.proto` file formatting. Please follow the rules below to ensure correct code generation.

## File Format Requirements

```protobuf
syntax = "proto3";     // Required: only proto3 is supported
package Basic;
option module = 10;    // Required: module ID must be defined

// Request heartbeat
message ReqHeartBeat
{
    int64 Timestamp = 1; // Timestamp
}
```

## Message Naming Rules

- **Request messages**: Must start with `Req` (e.g., `ReqLogin`, `ReqHeartBeat`)
- **Response messages**: Must start with `Resp` (e.g., `RespLogin`)
- **Notification messages**: Must start with `Notify` (e.g., `NotifyBagInfoChanged`)
- All message names, field names, enum names, and enum values must use **UpperCamelCase**

## Module ID Rules

Module ID is defined via `option module = <id>;`:

| ID Range | Purpose |
|----------|---------|
| `0` ~ `32767` | Client-Server communication |
| `-32768` ~ `-1` | Server-Server communication |

## Field Numbering Rules

- Message field numbers must be **less than 800** (values >= 800 are system-reserved and will cause parse errors)
- `ErrorCode` is a reserved field name in response messages — do not define it manually. `Resp` messages automatically generate an `ErrorCode` field

## Restrictions

- **No nested types**: Nesting of `message`, `enum`, or any custom type inside another message is not supported
- **No RPC definitions**: RPC service definitions in proto files are not supported
- **Only proto3**: `syntax = "proto3";` is required; proto2 is not supported

## Comment Standards

- Add a comment line **above** message and enum definitions:

```protobuf
// Request heartbeat
message ReqHeartBeat
{
    int64 Timestamp = 1;
}
```

- Add **inline** comments at the end of field lines:

```protobuf
// Player information
message PlayerInfo
{
    int64 Id = 1;         // Player ID
    string Name = 2;      // Player name
    uint32 Level = 3;     // Player level
    int32 State = 4;      // Player state
}
```

For the complete protocol specification, see the [Protocol Requirements](https://gameframex.doc.alianblank.com/en-US/protobuf/require.html) and [Notes](https://gameframex.doc.alianblank.com/en-US/protobuf/note.html) documentation.

---

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

## Mode Details and Examples

| Mode | Output Language | Namespace Behavior | Server-only Protos |
|------|----------------|-------------------|--------------------|
| `Server` | C# | Uses `--namespaceName` value | Included |
| `Unity` | C# | Uses `--namespaceName` value | Skipped (files ending with `-s` or `_s`) |
| `TypeScript` | TypeScript (.ts) | `--namespaceName` has no effect | Skipped (files ending with `-s` or `_s`) |
| `Godot` | C# | Always uses `GameFrameX.Network.Runtime` | Skipped (files ending with `-s` or `_s`) |

### Server Mode

Generates C# code with `[System.ComponentModel.Description]` attributes. Server-only proto files are included.

**Local:**

```bash
dotnet run --project ProtoExport -- \
  --mode server \
  --inputpath ./Protobuf \
  --outputpath ./Server/GameFrameX.Proto/Proto \
  --namespaceName GameFrameX.Proto.Proto
```

**Docker:**

```bash
docker run --rm \
  -v ./Protobuf:/protos \
  -v ./Server/GameFrameX.Proto/Proto:/output \
  gameframex/gameframex-tools:latest \
  --mode server --inputpath /protos --outputpath /output --namespaceName GameFrameX.Proto.Proto
```

### Unity Mode

Generates C# code using the `GameFrameX.Network.Runtime` namespace (without `[Description]` attributes). Server-only proto files are automatically skipped.

**Local:**

```bash
dotnet run --project ProtoExport -- \
  --mode unity \
  --inputpath ./Protobuf \
  --outputpath ./Unity/Assets/Hotfix/Proto \
  --namespaceName Hotfix.Proto
```

**Docker:**

```bash
docker run --rm \
  -v ./Protobuf:/protos \
  -v ./Unity/Assets/Hotfix/Proto:/output \
  gameframex/gameframex-tools:latest \
  --mode unity --inputpath /protos --outputpath /output --namespaceName Hotfix.Proto
```

### TypeScript Mode

Generates `.ts` files with `export namespace`, `export class`, and `export enum`, plus an aggregated `ProtoMessageRegister.ts` file. The `--namespaceName` parameter has no effect in this mode. Server-only proto files are automatically skipped.

**Local:**

```bash
dotnet run --project ProtoExport -- \
  --mode typescript \
  --inputpath ./Protobuf \
  --outputpath ./Laya/src/gameframex/protobuf
```

**Docker:**

```bash
docker run --rm \
  -v ./Protobuf:/protos \
  -v ./Laya/src/gameframex/protobuf:/output \
  gameframex/gameframex-tools:latest \
  --mode typescript --inputpath /protos --outputpath /output
```

### Godot Mode

Generates C# code with the fixed `GameFrameX.Network.Runtime` namespace (the `--namespaceName` parameter is ignored). Server-only proto files are automatically skipped.

**Local:**

```bash
dotnet run --project ProtoExport -- \
  --mode godot \
  --inputpath ./Protobuf \
  --outputpath ./Godot/Proto
```

**Docker:**

```bash
docker run --rm \
  -v ./Protobuf:/protos \
  -v ./Godot/Proto:/output \
  gameframex/gameframex-tools:latest \
  --mode godot --inputpath /protos --outputpath /output
```

## Docker Path Mapping

When using Docker, paths are mapped as follows:

- `-v <host-path>:<container-path>` mounts a host directory into the container
- `--inputpath` and `--outputpath` must reference the **container-side** paths (e.g. `/protos`, `/output`), not the host paths

```bash
# Example: host ./my-protos -> container /protos
docker run --rm \
  -v $(pwd)/my-protos:/protos \   # host path : container path
  -v $(pwd)/my-output:/output \   # host path : container path
  gameframex/gameframex-tools:latest \
  --mode server --inputpath /protos --outputpath /output --namespaceName GameFrameX.Proto.Proto
```
