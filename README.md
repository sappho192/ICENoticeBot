# ICENoticeBot

## 문서 정보
* 작성자: 김태인 (sappho192@gmail.com)
* 프로그램 개발자: 김태인
* 최종 문서 수정일: 2019년 10월 6일
* 문서 요약: ASP.Net Core에 기반한 텔레그램 챗봇입니다. 인하대학교 정보통신공학과 졸업프로젝트 공지사항 정보를 제공합니다.

## 제공하는 기능
**체크 되어 있지 않으면 예정사항입니다.**

- [x] 새로운 공지사항이 있을 시 사용자에게 알림
- [ ] 공지사항 목록을 보여줌
- [ ] 특정 공지사항의 내용을 보여줌
- [ ] 특정 공지사항의 파일만 제공해줌

## 개발 환경
* 언어: C#, HTML, JSON
* 프레임워크: ASP.Net Core 2.1.* **SDK** [다운로드](https://dotnet.microsoft.com/download/dotnet-core/2.1)
* 버전 관리: Git
* 통신 규격: HTTP(REST 통신)
* 개발 툴: Visual Studio 또는 Visual Studio Code [다운로드](https://visualstudio.microsoft.com/ko/downloads/)

### 종속성 (NuGet 기반)
* HtmlAgilityPack : HTML 문서 처리
* Newtonsoft.Json : JSON 데이터 처리

## 실행 환경
* Dotnet Core 2.1 **런타임** [다운로드](https://dotnet.microsoft.com/download/dotnet-core/2.1)
* 위의 런타임을 설치할 수 있는 운영 체제

# 프로젝트 코드 구조
중요한 파일들만 언급했습니다.

```
root
├── Program.cs: 프로그램의 시작점
├── Startup.cs: 웹 서버의 시작점
├── TelegramSettings.DEFAULT.json: 텔레그램과 관련한 설정을 담아둔 파일의 *예시*
├── (TelegramSettings.json): TelegramSettings.DEFAULT.json을 참고하여 *직접 생성해야 합니다*.
├── (noticeHeaders.json): 공지사항 정보들을 담아두는 파일 (프로그램 최초 실행시 자동생성됨)
├── (subscribedUsers.json): 알림을 받기로 한 사용자들을 담아두는 파일 (프로그램 최초 실행시 자동생성됨)
├── (lastUpdateId.json): 가장 마지막으로 처리한 메시지 정보를 담아두는 파일 (프로그램 최초 실행시 자동생성됨)
├── /Properties
│   └── Constants.cs: 전역으로 이용하는 문자열 등의 값들을 모아놓은 클래스
└── /Core
    ├── CommandProcessor.cs: 수신된 사용자 메시지를 처리하는 클래스
    ├── CommandReceiver.cs: 사용자 메시지를 수신하는 클래스 
    ├── NoticeCrawler.cs: 공지사항 정보를 수집하는 클래스
    ├── NoticeUpdatedEvent.cs: 새로운 공지사항이 있을 때 이를 알려주는 이벤트 클래스
    └── UserManager.cs: 새로운 공지사항이 있을때 알림을 받기로 한 사용자들을 관리하는 클래스
└── /Model
    └── ArticleHeader.cs: 공지사항의 내용을 제외한 정보(제목, 작성일, 첨부파일 존재여부 등)를 담는 클래스
└── /Controllers
    └── ValuesController.cs: 웹 요청을 처리하는 클래스
└── /Util
    ├── Synchronizer.cs: 비동기 메소드를 동기로 실행하게 도와주는 클래스 (웹페이지 요청 용도)
    └── UniqueQueue.cs: 중복 삽입이 금지된 Queue 자료구조 (메시지 저장 및 처리 용도)
```


## 개발 시 주의사항
* Nuget 패키지 관리자가 설치되어 있지 않으면 종속 라이브러리들이 복원되지 않으니 Visual Studio Installer로 먼저 설치해주세요.
* 솔루션을 최초로 열었을 때는 솔루션 **탐색기->Nuget 패키지 복원** 을 실행해주세요.
* TelegramSettings.DEFAULT.json을 참고하여 TelegramSettings.json을 *직접 생성해야 프로그램을 실행할 수 있습니다*.
아래에 있는 APIKey에 대응되는 값에 실제 텔레그램 API키를 기입한 후 TelegramSettings.json으로 저장해주세요.
```
{
  "APIKey": "botNNNNNNNNN:MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM"
}
```
