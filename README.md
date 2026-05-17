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

A language-oriented tool for converting Proto protocol files into multi-language code. Supports C#, TypeScript, C++ (planned), and Lua (planned).

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
  --mode csharp --isServer true --usingStatements "using System|using ProtoBuf|using System.Collections.Generic|using GameFrameX.NetWork.Abstractions|using GameFrameX.NetWork.Messages" --isGenerateDescription true --inputPath /protos --outputPath /output --namespaceName GameFrameX.Proto.Proto
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

## Example Proto Files

The [TestProtos/](TestProtos/) directory contains example proto files covering all major patterns:

| File | Pattern | Module ID |
|------|---------|-----------|
| `heartbeat.proto` | Basic Req/Resp | `1` (client-server) |
| `player.proto` | Req/Resp/Notify + enum + map | `2` (client-server) |
| `bag.proto` | enum + repeated + map + Notify | `3` (client-server) |
| `admin-s.proto` | Server-only proto (`-s` suffix) | `99` (client-server) |
| `server-internal-s.proto` | Server-server communication (negative module ID) | `-1` (server-server) |

---

# Parameter Reference

## Core Parameters

| Parameter | Required | Default | Description |
|-----------|----------|---------|-------------|
| `--mode` | Yes | - | Language mode: `csharp`, `typescript`, `cpp`, `lua` |
| `--inputPath` | Yes | - | Path to the `.proto` files directory |
| `--outputPath` | Yes | - | Output path for generated files |
| `--namespaceName` | No | `""` | Namespace for generated code (C# only, ignored by TypeScript) |
| `--isGenerateErrorCode` | No | `true` | Whether to auto-generate `ErrorCode` field in response messages |

## C# Parameters

| Parameter | Required | Default | Description |
|-----------|----------|---------|-------------|
| `--usingStatements` | No | `""` | Using statements separated by `\|` (e.g., `"using System\|using ProtoBuf\|using System.Collections.Generic"`) |
| `--isGenerateDescription` | No | `false` | Whether to generate `[System.ComponentModel.Description]` attributes |
| `--isServer` | No | `false` | Whether to include server-only proto files (files ending with `-s` or `_s`) |

## TypeScript Parameters

| Parameter | Required | Default | Description |
|-----------|----------|---------|-------------|
| `--importPath` | No | `"../network/"` | Import path prefix for generated import statements |
| `--isGenerateDescription` | No | `false` | Whether to generate JSDoc-style comments |

## Legacy Parameters

| Parameter | Required | Default | Description |
|-----------|----------|---------|-------------|
| `--isGenerateErrorCodeExcelFile` | No | `true` | Whether to generate error code Excel file |
| `--errorCodeExcelFilePath` | No | `""` | Custom path for error code Excel file |

---

# Mode Details and Examples

| Mode | Output Language | File Extension | Description |
|------|----------------|----------------|-------------|
| `csharp` | C# | `.cs` | For Server, Unity, Godot, Stride, Flax, etc. |
| `typescript` | TypeScript | `.ts` | For LayaAir, Cocos Creator, Phaser, etc. |
| `cpp` | C++ | `.h` | For Unreal Engine, etc. |
| `lua` | Lua | `.lua` | For Defold, Solar2D, Dora SSR, etc. |
| `go` | Go | `.go` | For Go game servers, etc. |

## C# Mode

Generates C# code with `[ProtoContract]` / `[ProtoMember]` attributes. All behavior is controlled via CLI parameters — no hardcoded engine-specific logic.

### Server Export

Generates code with server-specific using statements, `[Description]` attributes, and includes server-only proto files.

**Local:**

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

**Docker:**

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

### Unity Export

Generates code with Unity-specific using statements. Server-only proto files are automatically skipped.

```bash
dotnet ProtoExport.dll \
  --mode csharp \
  --usingStatements "using System|using ProtoBuf|using System.Collections.Generic|using GameFrameX.Network.Runtime" \
  --inputPath ./../../../../../Protobuf \
  --outputPath ./../../../../../Unity/Assets/Hotfix/Proto \
  --namespaceName Hotfix.Proto \
  --isGenerateErrorCode true
```

### Godot Export

Same as Unity but with Godot-specific namespace.

```bash
dotnet ProtoExport.dll \
  --mode csharp \
  --usingStatements "using System|using ProtoBuf|using System.Collections.Generic|using GameFrameX.Network.Runtime" \
  --inputPath ./../../../../../Protobuf \
  --outputPath ./../../../../../Godot/Proto \
  --namespaceName Proto \
  --isGenerateErrorCode true
```

## TypeScript Mode

Generates `.ts` files with `export namespace`, `export class`, and `export enum`, plus an aggregated `ProtoMessageRegister.ts` file. Server-only proto files are automatically skipped.

### Default Import Path

```bash
dotnet ProtoExport.dll \
  --mode typescript \
  --inputPath ./../../../../../Protobuf \
  --outputPath ./../../../../../Laya/src/gameframex/protobuf \
  --isGenerateErrorCode true
```

### Custom Import Path

```bash
dotnet ProtoExport.dll \
  --mode typescript \
  --importPath "./lib/network/" \
  --inputPath ./../../../../../Protobuf \
  --outputPath ./../../../../../CocosCreator/assets/scripts/protobuf \
  --isGenerateErrorCode true
```

**Docker:**

```bash
docker run --rm \
  -v ./Protobuf:/protos \
  -v ./Laya/src/gameframex/protobuf:/output \
  gameframex/gameframex-tools:latest \
  --mode typescript --inputPath /protos --outputPath /output
```

## C++ Mode

Generates C++ header files with `#pragma once`, namespace, `enum class`, and class definitions. Classes with `MessageObject` base include `MESSAGE_ID` and `Clear()` method.

```bash
dotnet ProtoExport.dll \
  --mode cpp \
  --usingStatements "#include <cstdint>|#include <string>|#include <vector>|#include <unordered_map>" \
  --inputPath ./../../../../../Protobuf \
  --outputPath ./../../../../../Unreal/Source/Proto \
  --namespaceName GameFrameX.Proto
```

## Lua Mode

Generates `.lua` files with LuaDoc (EmmyLua) type annotations and module-based message definitions. Includes a `ProtoMessageRegister.lua` aggregate file.

```bash
dotnet ProtoExport.dll \
  --mode lua \
  --importPath "./network/" \
  --inputPath ./../../../../../Protobuf \
  --outputPath ./../../../../../Defold/scripts/protobuf
```

**Docker:**

```bash
docker run --rm \
  -v ./Protobuf:/protos \
  -v ./Defold/scripts/protobuf:/output \
  gameframex/gameframex-tools:latest \
  --mode lua --importPath "./network/" --inputPath /protos --outputPath /output
```

## Go Mode

Generates Go struct definitions with protobuf tags, enum type definitions, and a `message_register.go` aggregate file. Uses `--namespaceName` as the Go package name (last segment if dot-separated).

```bash
dotnet ProtoExport.dll \
  --mode go \
  --usingStatements "google.golang.org/protobuf/runtime/protoimpl" \
  --inputPath ./../../../../../Protobuf \
  --outputPath ./../../../../../GoServer/proto \
  --namespaceName proto
```

**Docker:**

```bash
docker run --rm \
  -v ./Protobuf:/protos \
  -v ./GoServer/proto:/output \
  gameframex/gameframex-tools:latest \
  --mode go --inputPath /protos --outputPath /output --namespaceName proto
```

---

# Quick Export Scripts

Pre-built scripts are available in the `Protobuf/` directory:

| Script | Description |
|--------|-------------|
| `Proto2CsExport_Server.sh/.bat` | Export C# for Server |
| `Proto2CsExport_Client.sh/.bat` | Export C# for Unity Client |
| `Proto2TsExport.sh/.bat` | Export TypeScript |
| `Proto2CppExport.sh/.bat` | Export C++ |
| `Proto2LuaExport.sh/.bat` | Export Lua |
| `Proto2GoExport.sh/.bat` | Export Go |

---

# Docker Path Mapping

When using Docker, paths are mapped as follows:

- `-v <host-path>:<container-path>` mounts a host directory into the container
- `--inputPath` and `--outputPath` must reference the **container-side** paths (e.g. `/protos`, `/output`), not the host paths

```bash
# Example: host ./my-protos -> container /protos
docker run --rm \
  -v $(pwd)/my-protos:/protos \
  -v $(pwd)/my-output:/output \
  gameframex/gameframex-tools:latest \
  --mode csharp --isServer true \
  --usingStatements "using System|using ProtoBuf|using System.Collections.Generic|using GameFrameX.NetWork.Abstractions|using GameFrameX.NetWork.Messages" \
  --isGenerateDescription true \
  --inputPath /protos --outputPath /output --namespaceName GameFrameX.Proto.Proto
```
