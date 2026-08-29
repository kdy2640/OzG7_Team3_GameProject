# GameManager 구조와 매니저별 책임

이 문서는 `GameManager`가 제공하는 매니저의 책임과 연결 관계를 설명한다. 공개 메서드의 세부 구현은 각 소스 파일을 기준으로 확인한다.

## 전체 구조

`GameManager` 프리팹은 메인 씬에서 생성된 뒤 `DontDestroyOnLoad`로 유지된다. 루트에는 게임 데이터의 상태를 소유하는 매니저가 있고, 자식 오브젝트에는 장면이나 기능 단위 매니저가 배치되어 있다.

```text
GameManager
├─ GameManager
├─ StockManager
├─ UpgradeManager
├─ MarketManager
├─ UtilityManager
│  ├─ SceneController
│  ├─ SaveManager
│  ├─ AudioManager
│  └─ TutorialManager
├─ HarvestManager
└─ ServiceManager
```

`CookingManager`는 `MonoBehaviour`가 아니다. `StockManager`가 필요할 때 생성하고 재고 조작 함수를 전달한다.

## GameManager

소스: [`GameManager.cs`](../Assets/Scripts/Managers/Core/GameManager.cs)

`GameManager`는 전역 진입점과 매니저 참조를 제공한다. 게임 규칙이나 데이터를 직접 처리하지 않는다.

`Awake`에서 수행하는 작업은 다음과 같다.

1. 기존 `Instance`가 있으면 새 오브젝트를 제거한다.
2. 자신을 `Instance`에 등록한다.
3. 루트 오브젝트를 `DontDestroyOnLoad` 대상으로 지정한다.
4. 루트와 자식 오브젝트에서 매니저 컴포넌트를 찾는다.
5. `StockManager.CookingManager`를 받아 `CookingManager` 프로퍼티에 보관한다.

제공하는 참조는 다음과 같다.

| 프로퍼티 | 참조 대상 |
|---|---|
| `StockManager` | 재화와 재고 |
| `CookingManager` | 조리 가능 수량 계산과 조리 실행 |
| `Scene` | 씬 전환 |
| `Upgrade` | 업그레이드 상태와 런타임 레벨·스탯 |
| `Market` | 영업일·단계·시장 레벨·레벨 미션 |
| `Harvest` | 수확 루프 |
| `Service` | 영업 루프 |
| `AudioManager` | BGM과 SFX |
| `Tutorial` | 튜토리얼 진행 상태 |
| `Save` | 저장과 불러오기 |

## StockManager

소스: [`StockManager.cs`](../Assets/Scripts/Managers/Core/StockManager.cs)

`StockManager`는 플레이어가 보유한 재화, 식재료, 완성 요리를 관리한다. 실제 상태는 직렬화된 `StockData`에 저장한다.

### 외부 조회

`StockData` 프로퍼티는 `IReadableStockData`를 반환한다. UI와 다른 시스템은 이 인터페이스를 통해 현재 재화와 목록을 읽는다.

### 재화 처리

- `AddCurrency`는 0 이상의 정수만 받는다.
- `CanConsumeCurrency`는 잔액을 변경하지 않고 지불 가능 여부를 검사한다.
- `TryConsumeCurrency`는 검사에 성공한 경우에만 재화를 차감한다.
- 재화는 0보다 작아지지 않으며 `int.MaxValue`를 넘지 않는다.

### 식재료 처리

- `AddGrocery`는 단일 항목과 목록 입력을 지원한다.
- 같은 `GroceryType`이 이미 있으면 기존 수량에 합산한다.
- 목록에 null이나 음수 수량이 하나라도 있으면 전체 추가를 취소한다.
- `CanConsumeGrocery`는 같은 종류가 여러 번 요청된 경우 합계 수량을 검사한다.
- `TryConsumeGrocery`는 모든 항목을 소비할 수 있을 때 재고를 차감한다.

### 요리 처리

요리 재고도 식재료와 같은 방식으로 검사하고 소비한다. `StockManager`에서 요리를 추가하는 메서드는 private이며 `CookingManager`에 델리게이트로 전달된다. 외부에서는 `CookingManager.AddCookedDish`를 통해서만 완성 요리를 추가한다.

### 변경 알림과 저장

재고가 실제로 변경되면 구독자에게 알림을 보낸다. UI는 `SubscribeStockDataChange`로 등록하고 사용이 끝나면 `UnsubscribeStockDataChange`를 호출해야 한다.

`CreateStockSaveData`는 현재 목록을 새 객체로 복사한다. `LoadStockSaveData`는 저장값에서 새 `StockData`를 만들고 잘못된 enum 값과 음수 수량을 보정한 뒤 변경 알림을 보낸다.

## CookingManager

소스: [`CookingManager.cs`](../Assets/Scripts/Managers/Service/CookingManager.cs)

`CookingManager`는 조리 규칙만 처리하는 일반 C# 객체다. 재고를 직접 보유하지 않고 `StockManager`가 전달한 함수로 수량을 조회하고 변경한다.

- `CalculateCookableAmount`는 `DishDataDB`에서 레시피를 찾고 재료별 보유량으로 최대 조리 수량을 계산한다.
- 같은 식재료가 레시피에 여러 번 있으면 요구 수량을 합산한다.
- 레시피가 없거나 재료 목록이 비어 있으면 조리 가능 수량은 0이다.
- `CanCook`는 최대 조리 수량이 1 이상인지 반환한다.
- `TryCook`는 레시피의 모든 식재료를 소비하고 조리 시작 성공 여부만 반환한다.
- `AddCookedDish`는 조리가 완료된 시점에 해당 요리를 1개 재고에 추가한다.

조리 관련 기능은 `GameManager.Instance.CookingManager`를 통해 호출한다. 현재 영업 씬은 `TryCook`로 재료를 먼저 차감하고, 조리 연출이 끝났을 때 `AddCookedDish`를 호출하는 2단계 흐름을 사용한다.

## UpgradeManager

소스: [`UpgradeManager.cs`](../Assets/Scripts/Managers/Core/UpgradeManager.cs)

`UpgradeManager`는 플레이어가 획득한 업그레이드 레벨, 타입별 런타임 레벨, 계산된 런타임 스탯을 관리한다.

### 업그레이드 상태

직렬화된 `upgradeStates` 목록은 저장 대상이 되는 상태 원본이다. `upgradeStateMap`은 업그레이드 ID로 상태를 찾고, 타입별 맵은 `HarvestUpgradeType`, `DishType`, `EmployeeType`, `FacilityType`으로 상태를 찾는다.

`Awake`에서 null 데이터, 빈 ID, 중복 ID를 제거한 뒤 맵을 구성한다. `GetState`에 아직 등록되지 않은 데이터가 전달되면 레벨 0 상태를 새로 만든다.

### 구매 처리

`TryUpgrade`는 다음 순서로 처리한다.

1. 업그레이드 상태를 찾거나 생성한다.
2. `GetUpgradeAvailability`로 최대 레벨, 시장 레벨 조건, 보유 재화 또는 요리 강화 재료를 검사한다.
3. `StockManager`에서 현재 비용 또는 요리 강화 재료를 차감한다.
4. 레벨을 1 올린다.
5. `RuntimeStat`과 `RuntimeLevel`을 다시 계산한다.
6. `SubscribeUpgradeChanged`로 등록된 구독자에게 변경을 알린다.

다음 레벨의 비용은 `UpgradeDataSO.requiredCosts`의 `targetUpgradeLevel - 1` 인덱스를 사용한다. 비용 목록에 해당 레벨이 없으면 데이터 오류로 처리하고 구매할 수 없다.
다음 레벨의 시장 레벨 제한은 `UpgradeDataSO.requiredMarketLevel`의 `targetUpgradeLevel - 1` 인덱스를 사용한다. 조건 목록에 해당 레벨이 없으면 구매할 수 없다.

요리 강화 재료는 `DishUpgradeDataSO.requiredIngredients`의 `targetUpgradeLevel - 1` 인덱스를 사용한다. Unity의 중첩 리스트 직렬화 제한 때문에 각 레벨의 `List<GroceryAmount>`는 `GroceryRequirement`가 감싼다. 해당 레벨의 데이터가 없으면 데이터 오류로 처리하고 강화할 수 없다.

`UpgradeAvailability`는 구매 가능 여부를 `Available`, `InvalidData`, `MaxLevel`, `MarketLevelLocked`, `InsufficientCurrency`, `InsufficientIngredients`로 구분한다.

### RuntimeStat과 RuntimeLevel

`StatCalculator`는 모든 업그레이드 상태를 순회해 `RuntimeStat`을 새로 만든다.

- `RuntimeStat.Harvest`는 `HarvestUpgradeDataSO`의 변경치를 누적한다. 현재 항목은 톱 크기·개수·속도·날카로움과 트럭 속도·용량·연료다.
- `RuntimeStat.Service`는 `FacilityUpgradeDataSO`의 영업 스탯 변경치를 누적한다. 현재 항목은 `CustomerCount`다.
- 변경치는 `Add`, `Multiply`, `Max` 방식을 사용하고, 설정값에 현재 업그레이드 레벨을 곱해 적용한다.

`RuntimeLevel`은 스탯 계산과 별개로 수확·요리·직원·시설 타입별 현재 업그레이드 레벨을 보관한다. UI와 레벨 미션은 `RuntimeLevel.Get(...)`으로 레벨을 조회한다.

### 저장

업그레이드는 SO 참조 대신 ID와 레벨을 저장한다. 불러올 때 `UpgradeDataDB`로 ID에 맞는 SO를 다시 찾고 레벨을 0부터 `maxLevel` 사이로 제한한다. 업그레이드 ID를 변경했다면 `UpgradeDataDB.idMigrationMap`에 이전 ID와 새 ID를 등록해야 한다.

## MarketManager

소스: [`MarketManager.cs`](../Assets/Scripts/Managers/Core/MarketManager.cs)

`MarketManager`는 영업일 진행 상태, 시장 레벨 데이터, 오늘의 맛, 레벨 미션을 관리한다.

### MarketData

실제 진행 상태는 [`MarketData.cs`](../Assets/Scripts/Markets/MarketData.cs)에 보관한다.

- `CurrentBusinessDay`: 현재 영업일
- `CurrentPhase`: `Morning`, `Afternoon`, `Night` 중 현재 단계
- `CurrentLevel`: 현재 시장 레벨
- `TotalIncome`: 누적 수입
- `SelectedDishes`: 다음 영업에서 사용할 선택 메뉴의 읽기 전용 목록

`SelectDish`와 `DeselectDish`가 메뉴 목록을 변경한다. 각 프로퍼티와 메뉴가 실제로 변경되면 `MarketData` 내부 이벤트가 발생하고, `MarketManager`가 이를 `SubscribeMarketDataChanged`의 구독자에게 전달한다.

`MoveToNextPhase`는 `Morning → Afternoon → Night → 다음 영업일 Morning`으로 진행한다. `TodayTaste`는 `CurrentBusinessDay % TasteType.Count`로 결정한다.

### 시장 레벨과 미션

`LevelRefresh`는 현재 시장 레벨을 기준으로 다음 데이터를 교체한다.

- `LevelDataDB`: CSV에서 메뉴 선택 제한 `MaxDishLimit`과 목표 수입 `IncomeGoal`을 읽는다.
- `LevelMissionGroupDB`: 현재 레벨의 `LevelMissionGroupSO`를 읽는다.
- `LevelMissionChecker.CurrentStage`: 미션을 앞에서부터 검사해 첫 미달성 인덱스를 반환하며, 전부 달성했으면 미션 개수를 반환한다.

현재 미션 조건은 누적 수입과 요리·시설·직원·수확 업그레이드 레벨을 지원한다. 업그레이드 조건은 `UpgradeManager.RuntimeLevel`을 조회한다.

`LevelRefresh`는 `Start`, 시장 저장 데이터 로드, 시장 저장 데이터 리셋 시점에 호출된다. `MarketData.CurrentLevel`을 직접 변경하는 것만으로는 `LevelData`와 미션 그룹이 자동 갱신되지 않는다.

### 저장

`MarketSaveData`는 영업일, 현재 단계, 시장 레벨, 누적 수입, 선택 메뉴를 저장한다. 기존 `currentEXP` 저장값은 `totalIncome`이 0인 경우 누적 수입으로 이관한다.

## SceneController

소스: [`SceneController.cs`](../Assets/Scripts/Scenes/SceneController.cs)

`SceneController`는 `SceneType`과 `SceneBase` 구현체를 연결하고 씬 전환 순서를 관리한다.

현재 등록된 씬은 `Main`, `Hub`, `Harvest`, `Service`다. `MainScene`은 `GameManager`와 저장 데이터를 초기화한 뒤 자동으로 `HubScene`으로 전환하는 부트스트랩 씬이다.

`ChangeScene`은 같은 씬으로의 일반 전환을 무시한다. `RestartScene`은 현재 씬과 요청한 씬이 같을 때만 다시 로드한다. 전환 중에는 `isChangingScene`이 새 요청을 차단한다.

씬 전환 순서는 다음과 같다.

```text
현재 SceneBase.Exit
→ SceneManager.LoadSceneAsync
→ 다음 SceneBase.PrepareBeforeReveal
→ 다음 SceneBase.Enter
```

`HarvestScene`과 `ServiceScene`은 이 훅을 사용해 해당 게임 루프를 준비하고 시작하거나 종료한다.

## HarvestManager

소스: [`HarvestManager.cs`](../Assets/Scripts/Managers/Harvest/HarvestManager.cs)

`HarvestManager`는 수확 씬의 제한 시간과 루프 상태를 관리한다.

- `PrepareReveal`은 타이머를 기본 20초로 초기화하고 구독자에게 시간을 전달한다.
- `StartLoop`는 현재 씬이 `Harvest`일 때 루프를 시작하고 `LoopStarted` 이벤트를 발생시킨다.
- `Update`는 실행 중인 타이머를 감소시키고 매 프레임 남은 시간을 전달한다.
- 타이머가 0 이하가 되면 `EndLoop`를 호출한다.
- `EndLoop`는 실행 상태를 해제하고 `LoopEnded` 이벤트를 발생시킨 뒤 `Hub`로 전환한다.
- `Restart`는 `SceneController`에 수확 씬 재시작을 요청한다.

시간 표시는 `SubscribeTick`으로 연결한다. 게임 규칙 객체는 `HarvestEventManager`의 이벤트를 구독한다.

`HarvestEventType`에는 `BeforeLoopStarted`, `Pause`, `UnPause`가 정의되어 있지만 현재 `HarvestManager`는 이 이벤트를 발생시키지 않는다.

## ServiceManager

소스: [`ServiceManager.cs`](../Assets/Scripts/Managers/Service/ServiceManager.cs)

`ServiceManager`는 영업 씬의 제한 시간과 루프 상태를 관리한다. 타이머 처리와 공개 API는 `HarvestManager`와 같은 형태다.

- `PrepareReveal`은 타이머를 기본 20초로 초기화하고 구독자에게 시간을 전달한다.
- `StartLoop`는 현재 씬이 `Service`일 때 루프를 시작한다.
- `Update`는 남은 시간을 갱신한다.
- `EndLoop`는 루프를 종료하고 `LoopEnded` 이벤트를 발생시킨 뒤 `Hub`로 전환한다.
- `Restart`는 영업 씬 재시작을 요청한다.

영업 규칙 객체는 `ServiceEventManager`를 통해 이벤트를 주고받는다. `BeforeLoopStarted`, `Pause`, `UnPause` 이벤트는 현재 발생하지 않는다.

## AudioManager

소스: [`AudioManager.cs`](../Assets/Scripts/Managers/Utility/AudioManager.cs)

`AudioManager`는 BGM 한 채널과 여러 SFX 채널을 관리한다.

`Awake`에서 필요한 `AudioSource`를 생성하고 Inspector의 클립 목록을 enum 기반 딕셔너리로 변환한다. SFX 소스는 기본 20개이며 큐를 순환해 재생 중이 아닌 소스를 선택한다. 모든 소스가 사용 중이면 새 SFX 요청을 무시한다.

### BGM

- `PlayBGM`은 `BGMType`에 해당하는 클립을 재생한다.
- 같은 클립이 이미 재생 중이면 다시 시작하지 않는다.
- `StopBGM`, `PauseBGM`, `ResumeBGM`으로 상태를 제어한다.

### SFX

- `PlaySFX`는 클립별 최소 재생 간격과 최대 동시 재생 수를 검사한다.
- `PlaySFXRandomPitch`는 지정한 범위에서 피치를 변경한다.
- 클립 재생 시간이 지나면 코루틴이 동시 재생 수를 감소시킨다.

### 볼륨과 저장

마스터, BGM, SFX 볼륨은 0부터 1 사이로 제한한다. 실제 BGM 볼륨에는 클립 볼륨과 BGM·마스터 설정을 함께 적용한다. 저장 데이터에는 세 볼륨 값을 기록한다.

## TutorialManager

소스: [`TutorialManager.cs`](../Assets/Scripts/Managers/Utility/TutorialManager.cs)

`TutorialManager`는 `TutorialType`별 완료 여부를 딕셔너리에 저장한다. 현재 등록된 튜토리얼은 `BeforeHarvest`다.

- `GetTutorialProgressed`는 지정한 튜토리얼의 완료 여부를 반환한다.
- `ResolveTutorial`은 해당 값을 완료 상태로 바꾼다.
- `Initialize`와 `ResetTutorialSaveData`는 모든 값을 false로 초기화한다.

저장할 때 enum 이름과 완료 여부를 기록한다. 불러올 때 문자열을 `TutorialType`으로 변환하며 존재하지 않는 값은 무시한다.

## SaveManager

소스: [`SaveManager.cs`](../Assets/Scripts/Managers/Utility/SaveManager.cs)

`SaveManager`는 각 매니저의 저장 데이터를 하나의 `GameSaveData`로 모아 JSON 파일로 관리한다.

저장 경로는 `Application.persistentDataPath/save.json`이다. 파일 이름은 Inspector의 `saveFileName`으로 변경할 수 있다.

### 호출 시점

- `Start`에서 저장 파일이 있으면 자동으로 불러온다.
- `OnApplicationQuit`에서 현재 상태를 저장한다.
- `SaveGame`과 `LoadGame`은 외부에서 직접 호출할 수 있다.

### 저장 범위

| 데이터 | 담당 매니저 |
|---|---|
| 업그레이드 ID와 레벨 | `UpgradeManager` |
| 튜토리얼 완료 여부 | `TutorialManager` |
| 오디오 볼륨 | `AudioManager` |
| 재화와 재고 | `StockManager` |
| 영업일·단계·시장 레벨·누적 수입·선택 메뉴 | `MarketManager` |

불러오기는 업그레이드, 튜토리얼, 오디오, 재고, 시장 순서로 적용한다. 업그레이드 로드 시 `RuntimeStat`과 `RuntimeLevel`이 먼저 재계산되고, 시장 로드 시 현재 시장 레벨의 `LevelData`와 미션 그룹이 갱신된다.

`ResetSave`는 각 매니저를 기본 상태로 초기화한 뒤 저장 파일을 삭제한다. `DeleteSave`는 파일만 삭제하며 현재 메모리 상태는 유지한다.

## 생명주기

Unity는 같은 프레임에 실행되는 서로 다른 컴포넌트의 `Awake`와 `Start` 순서를 보장하지 않는다. 현재 구조는 다음 시점에 작업을 나눈다.

### Awake

- `GameManager`: 싱글턴 등록과 매니저 참조 수집
- `UpgradeManager`: 상태 맵과 최초 `RuntimeStat`·`RuntimeLevel` 계산
- `MarketManager`: `MarketData` 이벤트 연결과 레벨 미션 검사기 준비
- `HarvestManager`, `ServiceManager`: 이벤트 관리자 생성
- `AudioManager`: 오디오 소스와 클립 딕셔너리 준비
- `TutorialManager`: 진행 상태 초기화
- `SceneController`: `SceneType`과 `SceneBase` 연결

### Start

- `UpgradeManager`: `StockManager` 참조 확보
- `MarketManager`: 현재 시장 레벨의 `LevelData`와 미션 그룹 로드
- `SaveManager`: 저장 파일 불러오기

다른 매니저가 준비됐다고 가정하는 로직은 `Start` 이후에 실행한다. 초기화 순서가 반드시 필요한 기능은 Script Execution Order에 기대지 말고 호출 관계를 코드에 드러낸다.

## 접근 규칙

- 런타임 매니저는 `GameManager.Instance`에서 가져온다.
- 재고 조회는 `StockManager.StockData`의 읽기 전용 인터페이스를 사용한다.
- 재고 변경은 `StockManager`의 공개 메서드로 요청한다.
- 조리는 `GameManager.Instance.CookingManager`의 `TryCook`로 재료를 소비하고, 완료 시 `AddCookedDish`로 결과물을 추가한다.
- 씬 전환은 `SceneController.ChangeScene` 또는 `RestartScene`으로 요청한다.
- 업그레이드 변경을 표시하는 객체는 `SubscribeUpgradeChanged`를 구독하고 사용이 끝나면 `UnsubscribeUpgradeChanged`로 해제한다.
- 시장 상태를 표시하는 객체는 `SubscribeMarketDataChanged`를 구독하고 사용이 끝나면 `UnsubscribeMarketDataChanged`로 해제한다.
- 재고 변경을 표시하는 UI는 재고 변경 이벤트를 구독하고 비활성화 또는 파괴 시점에 해제한다.
- 수확과 영업 규칙은 각 루프 매니저의 이벤트 관리자를 사용한다.

## 주의 사항

- `GameManager.Awake`는 필요한 컴포넌트가 프리팹에 존재한다고 가정한다. 특히 `StockManager`가 없으면 `CookingManager` 참조를 가져오는 과정에서 예외가 발생한다.
- `GetComponentInChildren`은 비활성 자식 오브젝트를 기본적으로 찾지 않는다. Utility, Harvest, Service 매니저 오브젝트는 GameManager 초기화 시 활성 상태여야 한다.
- `UpgradeManager`의 `stockManager`는 `Start`에서 할당된다. 그보다 먼저 `TryUpgrade`나 구매 가능 검사를 호출하면 `InvalidData`로 처리될 수 있다.
- 업그레이드 ID는 저장 파일의 키다. ID 중복은 허용되지 않으며 이름 변경 시 마이그레이션 항목이 필요하다.
- `UpgradeDataDB`는 `Resources/SOs/UpgradeDatas` 아래의 유형별 폴더를 읽는다. 폴더 이동 시 로드 경로를 함께 수정한다.
- `MarketData.CurrentLevel`을 변경하면 시장 변경 알림은 발생하지만 `LevelData`와 `LevelMissionGroup`은 자동 교체되지 않는다. 레벨 변경 흐름에서 `LevelRefresh`를 함께 호출해야 한다.
- `TodayTaste`는 `TasteType.Count`가 0보다 크다고 가정한다.
- `TryCook`는 재료를 소비해도 요리 재고를 즉시 추가하지 않는다. 조리 완료 흐름에서 `AddCookedDish`가 누락되면 재료만 소모된다.
- 새 `SceneType`을 추가하면 `SceneController`의 딕셔너리, `SceneBase` 구현체, Build Settings를 함께 갱신한다.
- 저장 파일 읽기와 쓰기의 예외 처리는 현재 구현되어 있지 않다. 손상된 JSON이나 파일 시스템 오류가 발생하면 호출이 중단될 수 있다.
- 자동 저장은 애플리케이션 종료 시점에만 실행된다. 진행 중 저장이 필요한 기능은 `SaveGame`을 직접 호출해야 한다.
- `DeleteSave` 호출 후 애플리케이션이 종료되면 `OnApplicationQuit`이 현재 메모리 상태를 다시 저장한다.
- 오디오 클립 목록에서 null 클립과 중복 enum 값은 딕셔너리에 등록되지 않는다.
