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

Proto 프로토콜 파일을 `Server/Unity/TypeScript` 코드로 변환하는 도구입니다.

# 매개변수 설명

이 도구의 명령줄 매개변수에 대한 자세한 설명은 다음과 같습니다.

`--mode`
실행 모드를 지정합니다. 유효한 값은 `Server`, `Unity` 또는 `TypeScript` 중 하나입니다.

`--inputpath`
.proto 프로토콜 파일의 경로를 지정합니다. 프로그램은 지정된 경로 아래의 모든 .proto 파일을 스캔합니다.

`--outputpath`
생성된 파일의 출력 경로를 지정합니다.

`--namespaceName`
네임스페이스를 지정합니다. TypeScript 모드에서는 이 매개변수가 적용되지 않습니다. 네임스페이스를 설정하지 않으려면 빈 값을 전달하세요.

## 명령줄 예시

다음 명령 예시는 Proto 프로토콜 파일을 Server 코드로 변환하는 방법을 보여줍니다:

```
--mode server --inputpath ./../../../../../Protobuf --outputpath ./../../../../../Server/GameFrameX.Proto/Proto --namespaceName GameFrameX.Proto.Proto
```

위의 명령 예시에서:

- `--mode server`은 실행 모드를 Server로 설정합니다.
- `--inputpath ./../../../../../Protobuf`는 .proto 프로토콜 파일 경로를 `./../../../../../Protobuf`로 설정합니다.
- `--outputpath ./../../../../../Server/GameFrameX.Proto/Proto`는 출력 파일 경로를 `./../../../../../Server/GameFrameX.Proto/Proto`로 설정합니다.
- `--namespaceName GameFrameX.Proto.Proto`는 네임스페이스를 `GameFrameX.Proto.Proto`로 설정합니다.

명령줄 매개변수를 조정하여 실제 필요에 맞는 코드를 생성할 수 있습니다.
