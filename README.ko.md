<div align="center">

![GameFrameX Logo](https://download.alianblank.com/gameframex/gameframex_logo_320.png)

# GameFrameX.Tools

[![Version](https://img.shields.io/github/v/release/GameFrameX/GameFrameX.Tools?label=version&color=green)](https://github.com/GameFrameX/GameFrameX.Tools/releases)
[![License](https://img.shields.io/badge/license-MIT+Apache%202.0-orange.svg)](LICENSE)
[![Documentation](https://img.shields.io/badge/docs-gameframex-brightgreen.svg)](https://gameframex.doc.alianblank.com)

**인디 게임 개발자를 위한 올인원 솔루션 · 인디 개발자의 꿈을 실현**

[📖 문서](https://gameframex.doc.alianblank.com) • [💬 QQ 그룹: 467608841](https://qm.qq.com/cgi-bin/qm/qr?k=sYFd1nv6m2KZIWFLorZ5pBR0AE5ZhbuL&jump_from=webapi&authKey=oCu+uoL3n35fT5SEt7iLgGtROPxh31n/rHUxRlp0w1f+j38W4tKBuWyRH3KEdwHN)

---

🌐 **언어**: [English](README.md) | [简体中文](README.zh-CN.md) | [繁體中文](README.zh-TW.md) | [日本語](README.ja.md) | **한국어**

---

</div>

# ProtoExport 도구

Proto 프로토콜 파일을 `Server/Unity/TypeScript/Godot` 코드로 변환하는 도구입니다.

# Docker

`linux/amd64` 및 `linux/arm64` 용 Docker 이미지가 제공됩니다.

**Docker Hub**

```bash
docker pull gameframex/gameframex-tools:latest
```

**GitHub Container Registry (GHCR)**

```bash
docker pull ghcr.io/gameframex/gameframex.tools:latest
```

**사용 예시**

```bash
docker run --rm \
  -v /path/to/protos:/protos \
  -v /path/to/output:/output \
  gameframex/gameframex-tools:latest \
  --mode server --inputpath /protos --outputpath /output --namespaceName GameFrameX.Proto.Proto
```

# 매개변수 설명

이 도구의 명령줄 매개변수에 대한 자세한 설명은 다음과 같습니다.

`--mode`
실행 모드를 지정합니다. 유효한 값은 `Server`, `Unity`, `TypeScript` 또는 `Godot` 중 하나입니다.

`--inputpath`
.proto 프로토콜 파일의 경로를 지정합니다. 프로그램은 지정된 경로 아래의 모든 .proto 파일을 스캔합니다.

`--outputpath`
생성된 파일의 출력 경로를 지정합니다.

`--namespaceName`
네임스페이스를 지정합니다. TypeScript 모드에서는 이 매개변수가 적용되지 않습니다. Godot 모드에서는 생성된 코드가 항상 `GameFrameX.Network.Runtime` 네임스페이스를 사용합니다. 네임스페이스를 설정하지 않으려면 빈 값을 전달하세요.

## 모드 상세 및 예시

| 모드 | 출력 언어 | 네임스페이스 동작 | 서버 전용 Proto |
|------|----------|-----------------|----------------|
| `Server` | C# | `--namespaceName` 값 사용 | 포함 |
| `Unity` | C# | `--namespaceName` 값 사용 | 건너뜀 (`-s` 또는 `_s`로 끝나는 파일) |
| `TypeScript` | TypeScript (.ts) | `--namespaceName` 무효 | 건너뜀 (`-s` 또는 `_s`로 끝나는 파일) |
| `Godot` | C# | 항상 `GameFrameX.Network.Runtime` 사용 | 건너뜀 (`-s` 또는 `_s`로 끝나는 파일) |

### Server 모드

`[System.ComponentModel.Description]` 속성이 포함된 C# 코드를 생성합니다. 서버 전용 proto 파일이 포함됩니다.

**로컬 실행:**

```bash
dotnet run --project ProtoExport -- \
  --mode server \
  --inputpath ./Protobuf \
  --outputpath ./Server/GameFrameX.Proto/Proto \
  --namespaceName GameFrameX.Proto.Proto
```

**Docker 실행:**

```bash
docker run --rm \
  -v ./Protobuf:/protos \
  -v ./Server/GameFrameX.Proto/Proto:/output \
  gameframex/gameframex-tools:latest \
  --mode server --inputpath /protos --outputpath /output --namespaceName GameFrameX.Proto.Proto
```

### Unity 모드

`GameFrameX.Network.Runtime` 네임스페이스를 사용하는 C# 코드를 생성합니다 (`[Description]` 속성 없음). 서버 전용 proto 파일은 자동으로 건너뜁니다.

**로컬 실행:**

```bash
dotnet run --project ProtoExport -- \
  --mode unity \
  --inputpath ./Protobuf \
  --outputpath ./Unity/Assets/Hotfix/Proto \
  --namespaceName Hotfix.Proto
```

**Docker 실행:**

```bash
docker run --rm \
  -v ./Protobuf:/protos \
  -v ./Unity/Assets/Hotfix/Proto:/output \
  gameframex/gameframex-tools:latest \
  --mode unity --inputpath /protos --outputpath /output --namespaceName Hotfix.Proto
```

### TypeScript 모드

`export namespace`, `export class`, `export enum`이 포함된 `.ts` 파일과 집계 파일 `ProtoMessageRegister.ts`를 생성합니다. 이 모드에서는 `--namespaceName` 매개변수가 무효합니다. 서버 전용 proto 파일은 자동으로 건너뜁니다.

**로컬 실행:**

```bash
dotnet run --project ProtoExport -- \
  --mode typescript \
  --inputpath ./Protobuf \
  --outputpath ./Laya/src/gameframex/protobuf
```

**Docker 실행:**

```bash
docker run --rm \
  -v ./Protobuf:/protos \
  -v ./Laya/src/gameframex/protobuf:/output \
  gameframex/gameframex-tools:latest \
  --mode typescript --inputpath /protos --outputpath /output
```

### Godot 모드

고정적으로 `GameFrameX.Network.Runtime` 네임스페이스를 사용하는 C# 코드를 생성합니다 (`--namespaceName` 매개변수는 무시됩니다). 서버 전용 proto 파일은 자동으로 건너뜁니다.

**로컬 실행:**

```bash
dotnet run --project ProtoExport -- \
  --mode godot \
  --inputpath ./Protobuf \
  --outputpath ./Godot/Proto
```

**Docker 실행:**

```bash
docker run --rm \
  -v ./Protobuf:/protos \
  -v ./Godot/Proto:/output \
  gameframex/gameframex-tools:latest \
  --mode godot --inputpath /protos --outputpath /output
```

## Docker 경로 매핑

Docker 사용 시 경로 매핑 규칙은 다음과 같습니다:

- `-v <호스트 경로>:<컨테이너 경로>` 로 호스트 디렉토리를 컨테이너에 마운트합니다
- `--inputpath` 와 `--outputpath` 에는 **컨테이너 내부 경로** (예: `/protos`, `/output`)를 지정해야 합니다 (호스트 경로가 아님)

```bash
# 예시: 호스트 ./my-protos -> 컨테이너 /protos
docker run --rm \
  -v $(pwd)/my-protos:/protos \     # 호스트 경로 : 컨테이너 경로
  -v $(pwd)/my-output:/output \     # 호스트 경로 : 컨테이너 경로
  gameframex/gameframex-tools:latest \
  --mode server --inputpath /protos --outputpath /output --namespaceName GameFrameX.Proto.Proto
```
