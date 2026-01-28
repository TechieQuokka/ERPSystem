# ERP System

C# .NET 기반 ERP(Enterprise Resource Planning) 시스템입니다.

## 프로젝트 구조

```
├── ERP.Application/      # 애플리케이션 레이어
├── ERP.Domain/           # 도메인 레이어
├── ERP.Infrastructure/   # 인프라 레이어
├── ERP.WebAPI/           # Web API
└── ERPSystem.sln         # 솔루션 파일
```

## 아키텍처

Clean Architecture 패턴을 따릅니다.

## 요구사항

- .NET 6.0+
- Visual Studio 2022

## 빌드 및 실행

1. `ERPSystem.sln` 파일을 Visual Studio에서 엽니다
2. 솔루션을 빌드합니다
3. ERP.WebAPI를 시작 프로젝트로 설정 후 실행합니다

## License

MIT License