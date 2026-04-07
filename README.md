# Qsight

🚀 QSight Client (WinUI 3)
QSight는 로컬 시스템의 보안 위협을 실시간으로 감지하고 분석하는 보안 플랫폼의 클라이언트 에이전트입니다.

저는 WinUI 3 기반의 데스크톱 프론트엔드 설계와 백엔드 서비스 계층의 통합을 담당하여, 복잡한 보안 데이터를 사용자에게 직관적으로 전달하는 환경을 구축했습니다.

🛠️ 주요 담당 역할 (Front-end)
1. WinUI 3 기반의 반응형 대시보드 설계
실시간 상태 모니터링: 에이전트의 스캔 진행률(ProgressBar), 현재 분석 중인 파일 경로, 서버 연결 상태를 한눈에 파악할 수 있는 대시보드를 구현했습니다.

사용자 경험 최적화:

UnknownBox: 자동 분석이 필요한 미확인 파일을 별도 관리하는 UI 섹션 구현.

QueueList: 현재 대기 중인 스캔 작업 목록을 실시간 피드백으로 제공.

커스텀 컨트롤 활용: ListView와 DataTemplate을 활용하여 가독성 높은 데이터 바인딩 구조를 설계했습니다.

2. 서비스 레이어 통합 및 아키텍처 고도화 (Facade Pattern)
AgentService 중앙 집중화: 분산되어 있던 스캔 엔진 제어, IPC 메시지 처리, 상태 관리 로직을 AgentService로 통합하여 관리 포인트의 단일화를 달성했습니다.

이벤트 기반 UI 업데이트: 스캔 시작, 완료, 진행률 변경 등 엔진의 상태 변화를 Action 이벤트를 통해 UI에 실시간으로 반영하는 구조를 설계했습니다.

3. 고성능 비동기 처리 및 안정성 확보
스레드 안전성(Thread-Safety): DispatcherQueue를 활용하여 백그라운드 스캔 작업과 UI 스레드 간의 충돌을 방지하고, 비동기 작업 중 발생할 수 있는 메모리 접근 위반(Access Violation)을 방어하는 로직을 구축했습니다.

REST API 연동: HttpClient를 활용하여 보안 분석 서버와 통신하고, 대시보드 요약 데이터를 비동기(async/await)로 호출하여 UI 프리징 현상을 제거했습니다.

4. IPC(Inter-Process Communication) 연동
Named Pipes 통신: 셸 확장(Shell Extension) 등 외부 프로세스로부터 전달되는 스캔 명령을 수신하고 처리하는 메시지 핸들러(HandleIPC)를 구현했습니다.

💻 Technical Stack
Language: C# (.NET 8)

Framework: WinUI 3 (Windows App SDK)

Tools: Git, Visual Studio 2022

🔍 핵심 코드 기여 (Highlights)
비동기 대시보드 로딩: 서버 상태 체크 및 통계 데이터를 안전하게 업데이트하는 로직 구현.

실시간 큐 관리: ConcurrentQueue를 활용한 멀티스레드 환경에서의 안정적인 스캔 작업 스케줄링.

UI/UX 리팩토링: Grid와 StackPanel을 활용한 체계적인 레이아웃 분리 및 시각적 구분선 적용.
