# 개인 기여와 코드 확인 순서

[저장소 소개로 돌아가기](../README.md)

## 프로토타입 시연 영상

- [InEarth4 UI(사용자 인터페이스) 시연 영상 보기](https://youtu.be/_SWBLqQ7XXQ)
- [InEarth4 버프 사용 시연 영상 보기](https://youtu.be/0IAS78Irmr0)

개발 당시 제작한 프로토타입 영상입니다. 최종 상용 버전이 아니며, 공개 저장소의 코드와 일부 차이가 있을 수 있습니다. 영상은 프로젝트의 동작을 소개하는 자료이며, 개인 기여와 공동 개발 범위는 별도 안내를 기준으로 확인해 주세요.

대표 개인 기여는 **휴대전화 패턴 잠금에서 착안한 스킬 입력의 기획과 전체 구현**입니다. 조작 아이디어를 노드 입력, 판정, 마나 소비와 화면 표시로 연결한 부분을 먼저 보시면 됩니다. 아래 함수가 들어 있는 `PlayManager.cs` 전체나 다른 팀 기능까지 단독 구현한 것으로 표시하지 않습니다.

## 구현 목표와 선택

미리 정의한 노드를 연결하는 순서를 스킬 사용 규칙으로 만들었습니다. 일반적인 손글씨·자유 궤적 인식이 아니라 3×3 노드의 방문 순서를 비교하는 시스템입니다.

입력 코드는 방문 순서와 연결선을 관리하고, 스킬별 패턴·비용·지속시간·효과 정보는 `ScriptableObject`(스크립터블 오브젝트, Unity의 데이터 자산)로 분리했습니다. 패턴 일치뿐 아니라 이미 등록된 스킬인지, 마나가 충분한지도 확인합니다.

## 3분 코드 확인 경로

아래 링크는 당시 보존 커밋에 고정되어 있어 이후 문서 수정으로 줄 위치가 달라지지 않습니다.

| 순서 | 바로 볼 함수 | 확인할 내용 |
| --- | --- | --- |
| 1 | [`skillData`와 `settingSkillCode`](https://github.com/fishbowl92/InEarth4/blob/1b3304b83aee43bfcb8a788b961bd5022c0453f2/Scriptable/skillData.cs#L5-L29) | 노드 배열과 비교 코드, 마나 비용 및 효과 데이터 |
| 2 | [`onMouseDownNode`](https://github.com/fishbowl92/InEarth4/blob/1b3304b83aee43bfcb8a788b961bd5022c0453f2/PlayManager.cs#L1125-L1139) | 입력 시작과 첫 노드 기록 |
| 3 | [`onMouseEnterNode`](https://github.com/fishbowl92/InEarth4/blob/1b3304b83aee43bfcb8a788b961bd5022c0453f2/PlayManager.cs#L1014-L1119) | 노드 진입, 일부 경로의 중간 노드 보완, 방문 순서 기록 |
| 4 | [`onMouseUpNode`, `useManaCost`](https://github.com/fishbowl92/InEarth4/blob/1b3304b83aee43bfcb8a788b961bd5022c0453f2/PlayManager.cs#L1140-L1184) | 패턴 검색 → 중복 등록·마나 검사 → 비용 차감과 스킬 목록 등록 |
| 5 | [연결선 갱신](https://github.com/fishbowl92/InEarth4/blob/1b3304b83aee43bfcb8a788b961bd5022c0453f2/PlayManager.cs#L354-L362) · [`trySetLineEdit`, `release`](https://github.com/fishbowl92/InEarth4/blob/1b3304b83aee43bfcb8a788b961bd5022c0453f2/PlayManager.cs#L1185-L1230) | 입력 중 화면 표시, 연결선 재사용과 입력 종료 정리 |

## 코드로 확인할 수 있는 결과

노드 입력부터 시각적 피드백, 사용 조건 검사와 스킬 등록까지 한 흐름으로 구현했습니다. 입력 종료 경로에서 방문 목록과 연결선을 정리하며, 스킬 패턴과 비용을 데이터로 구성할 수 있습니다.

다만 효과의 새 동작까지 모두 데이터만으로 추가된다는 뜻은 아닙니다. 새로운 종류의 효과에는 실행 코드가 필요할 수 있습니다. 전체 Unity 프로젝트가 없어 이번 정리에서 실제 모바일 조작이나 게임 실행을 다시 검증하지는 않았습니다.

## 현재 다시 구현한다면

패턴 판정과 입력·화면 표시를 분리해 빈 입력, 틀린 패턴, 마나 부족, 중복 등록, 중간 노드와 재방문 규칙을 자동 검사하겠습니다. 현재 문자열 비교는 한 자리 노드 번호를 전제로 하므로 노드 수 확장 시 식별 형식도 바꿔야 합니다.

투사체와 화면 객체의 재사용 등 다른 기능은 [전체 코드 흐름](./CODE_WALKTHROUGH.md)의 보조 자료로 두었습니다. 대표 개인 기여의 시작점은 위 패턴 입력 코드입니다.

[회고와 개선 방향](./RETROSPECTIVE.md)
