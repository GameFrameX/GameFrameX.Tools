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

这是一个用于将Proto协议文件转换为 `Server/Unity/TypeScript/Godot` 代码的工具。

# Docker

预构建的 Docker 镜像支持 `linux/amd64` 和 `linux/arm64` 架构。

**Docker Hub**

```bash
docker pull gameframex/gameframex-tools:latest
```

**GitHub Container Registry (GHCR)**

```bash
docker pull ghcr.io/gameframex/gameframex.tools:latest
```

**使用示例**

```bash
docker run --rm \
  -v /path/to/protos:/protos \
  -v /path/to/output:/output \
  gameframex/gameframex-tools:latest \
  --mode server --inputpath /protos --outputpath /output --namespaceName GameFrameX.Proto.Proto
```

# Proto 协议规范

本工具对 `.proto` 文件格式有特定要求，请遵循以下规则以确保正确生成代码。

## 文件格式要求

```protobuf
syntax = "proto3";     // 必须声明：仅支持 proto3
package Basic;
option module = 10;    // 必须定义：模块 ID

// 请求心跳
message ReqHeartBeat
{
    int64 Timestamp = 1; // 时间戳
}
```

## 消息命名规则

- **请求消息**：必须以 `Req` 开头（如 `ReqLogin`、`ReqHeartBeat`）
- **响应消息**：必须以 `Resp` 开头（如 `RespLogin`）
- **通知消息**：必须以 `Notify` 开头（如 `NotifyBagInfoChanged`）
- 消息名称、字段名称、枚举名称和枚举值必须使用 **UpperCamelCase**（大驼峰）命名

## 模块 ID 规则

通过 `option module = <id>;` 定义模块 ID：

| ID 范围 | 用途 |
|---------|------|
| `0` ~ `32767` | 客户端-服务端通讯 |
| `-32768` ~ `-1` | 服务端-服务端通讯 |

## 字段编号规则

- 消息字段编号必须**小于 800**（大于等于 800 为系统预留，会导致解析异常）
- `ErrorCode` 是响应消息中的保留字段名，请勿手动定义。`Resp` 消息会自动生成 `ErrorCode` 字段

## 限制事项

- **不支持嵌套类型**：不允许在 message 内部嵌套 `message`、`enum` 或任何自定义类型
- **不支持 RPC 定义**：proto 文件中不允许使用 RPC 服务定义
- **仅支持 proto3**：必须声明 `syntax = "proto3";`，不支持 proto2

## 注释规范

- 在 message 和 enum 定义**上方**添加注释：

```protobuf
// 请求心跳
message ReqHeartBeat
{
    int64 Timestamp = 1;
}
```

- 在字段行**末尾**添加行内注释：

```protobuf
// 玩家信息
message PlayerInfo
{
    int64 Id = 1;         // 角色ID
    string Name = 2;      // 角色名
    uint32 Level = 3;     // 角色等级
    int32 State = 4;      // 角色状态
}
```

完整的协议规范请参考 [通讯协议规范](https://gameframex.doc.alianblank.com/zh-CN/protobuf/require.html) 和 [注意事项](https://gameframex.doc.alianblank.com/zh-CN/protobuf/note.html) 文档。

## 示例 Proto 文件

[TestProtos/](TestProtos/) 目录下提供了覆盖所有主要模式的示例 proto 文件：

| 文件 | 模式 | 模块 ID |
|------|------|---------|
| `heartbeat.proto` | 基础 Req/Resp | `1`（客户端-服务端） |
| `player.proto` | Req/Resp/Notify + enum + map | `2`（客户端-服务端） |
| `bag.proto` | enum + repeated + map + Notify | `3`（客户端-服务端） |
| `admin-s.proto` | 服务端专属协议（`-s` 后缀） | `99`（客户端-服务端） |
| `server-internal-s.proto` | 服务端间通讯（负值模块 ID） | `-1`（服务端-服务端） |

---

# 参数解析

以下是此工具命令行参数的详细说明：

`--mode`
此参数用于指定运行模式。有效值包括 `Server`, `Unity`, `TypeScript`, 或 `Godot` 中的任何一个。

`--inputpath`
此参数用于指定.proto协议文件的路径。程序将扫描该路径下所有以.proto结尾的文件。

`--outputpath`
此参数用于指定输出文件的保存路径。

`--namespaceName`
此参数用于指定命名空间。在TypeScript模式中此参数无效。在Godot模式中，生成的代码始终使用 `GameFrameX.Network.Runtime` 命名空间。如果不想设定命名空间，此参数可以传空值。

## 模式详情与示例

| 模式 | 输出语言 | 命名空间行为 | 服务端专属 Proto |
|------|---------|-------------|-----------------|
| `Server` | C# | 使用 `--namespaceName` 的值 | 包含 |
| `Unity` | C# | 使用 `--namespaceName` 的值 | 跳过（以 `-s` 或 `_s` 结尾的文件） |
| `TypeScript` | TypeScript (.ts) | `--namespaceName` 无效 | 跳过（以 `-s` 或 `_s` 结尾的文件） |
| `Godot` | C# | 固定使用 `GameFrameX.Network.Runtime` | 跳过（以 `-s` 或 `_s` 结尾的文件） |

### Server 模式

生成带有 `[System.ComponentModel.Description]` 特性的 C# 代码，包含服务端专属 proto 文件。

**本地运行：**

```bash
dotnet run --project ProtoExport -- \
  --mode server \
  --inputpath ./Protobuf \
  --outputpath ./Server/GameFrameX.Proto/Proto \
  --namespaceName GameFrameX.Proto.Proto
```

**Docker 运行：**

```bash
docker run --rm \
  -v ./Protobuf:/protos \
  -v ./Server/GameFrameX.Proto/Proto:/output \
  gameframex/gameframex-tools:latest \
  --mode server --inputpath /protos --outputpath /output --namespaceName GameFrameX.Proto.Proto
```

### Unity 模式

生成使用 `GameFrameX.Network.Runtime` 命名空间的 C# 代码（不含 `[Description]` 特性），自动跳过服务端专属 proto 文件。

**本地运行：**

```bash
dotnet run --project ProtoExport -- \
  --mode unity \
  --inputpath ./Protobuf \
  --outputpath ./Unity/Assets/Hotfix/Proto \
  --namespaceName Hotfix.Proto
```

**Docker 运行：**

```bash
docker run --rm \
  -v ./Protobuf:/protos \
  -v ./Unity/Assets/Hotfix/Proto:/output \
  gameframex/gameframex-tools:latest \
  --mode unity --inputpath /protos --outputpath /output --namespaceName Hotfix.Proto
```

### TypeScript 模式

生成包含 `export namespace`、`export class` 和 `export enum` 的 `.ts` 文件，并生成聚合文件 `ProtoMessageRegister.ts`。`--namespaceName` 参数在此模式下无效。自动跳过服务端专属 proto 文件。

**本地运行：**

```bash
dotnet run --project ProtoExport -- \
  --mode typescript \
  --inputpath ./Protobuf \
  --outputpath ./Laya/src/gameframex/protobuf
```

**Docker 运行：**

```bash
docker run --rm \
  -v ./Protobuf:/protos \
  -v ./Laya/src/gameframex/protobuf:/output \
  gameframex/gameframex-tools:latest \
  --mode typescript --inputpath /protos --outputpath /output
```

### Godot 模式

生成固定使用 `GameFrameX.Network.Runtime` 命名空间的 C# 代码（`--namespaceName` 参数会被忽略）。自动跳过服务端专属 proto 文件。

**本地运行：**

```bash
dotnet run --project ProtoExport -- \
  --mode godot \
  --inputpath ./Protobuf \
  --outputpath ./Godot/Proto
```

**Docker 运行：**

```bash
docker run --rm \
  -v ./Protobuf:/protos \
  -v ./Godot/Proto:/output \
  gameframex/gameframex-tools:latest \
  --mode godot --inputpath /protos --outputpath /output
```

## Docker 路径映射说明

使用 Docker 时，路径映射规则如下：

- `-v <宿主机路径>:<容器内路径>` 将宿主机目录挂载到容器中
- `--inputpath` 和 `--outputpath` 必须填写**容器内路径**（如 `/protos`、`/output`），而非宿主机路径

```bash
# 示例：宿主机 ./my-protos -> 容器内 /protos
docker run --rm \
  -v $(pwd)/my-protos:/protos \    # 宿主机路径 : 容器内路径
  -v $(pwd)/my-output:/output \    # 宿主机路径 : 容器内路径
  gameframex/gameframex-tools:latest \
  --mode server --inputpath /protos --outputpath /output --namespaceName GameFrameX.Proto.Proto
```
