# 담당자별 GameManager 사용 시나리오

이 문서는 기능 담당자가 `GameManager`와 관련 DB를 통해 데이터를 조회하고 변경을 요청하는 표준 흐름을 설명한다. 매니저별 책임과 전체 구조는 [GameManager 구조 문서](./GameManagerArchitecture.md)를 참고한다.

## 업그레이드 담당자

업그레이드 담당 코드는 `Resources`에서 SO를 직접 로드하지 않고 `UpgradeDataDB`를 통해 타겟에 맞는 SO를 조회한다.

```csharp
HarvestUpgradeDataSO harvestData =
    UpgradeDataDB.GetData(HarvestUpgradeType.SawSpeed);

DishUpgradeDataSO dishData =
    UpgradeDataDB.GetData(DishType.MeatOnigiri);

EmployeeUpgradeDataSO employeeData =
    UpgradeDataDB.GetData(EmployeeType.Harvester_1);

FacilityUpgradeDataSO facilityData =
    UpgradeDataDB.GetData(FacilityType.Table_1);
```

SO ID를 알고 있는 경우에는 `UpgradeDataDB.GetData(string id)`를 사용한다. 단, 일반 게임플레이 코드에서는 타입별 enum 오버로드를 우선한다. `GetData`는 데이터가 없으면 경고를 남기고 `null`을 반환하므로 결과를 검사한다.

현재 레벨과 다음 비용, 구매 가능 상태는 다음과 같이 조회한다.

```csharp
UpgradeManager upgrade = GameManager.Instance.Upgrade;
EmployeeUpgradeDataSO data =
    UpgradeDataDB.GetData(EmployeeType.Harvester_1);

if (data == null)
    return;

int currentLevel = upgrade.RuntimeLevel.Get(data.TargetEmployee);
int nextCost = data.GetCosts(currentLevel);
UpgradeAvailability availability =
    upgrade.GetUpgradeAvailability(data);
```

`UpgradeAvailability.Available`이라고 해도 UI 측에서 돈을 직접 차감하거나 레벨을 수정하지 않는다. 실제 업그레이드는 `TryUpgrade`로 요청한다.

```csharp
if (GameManager.Instance.Upgrade.TryUpgrade(data))
{
    // 성공: 돈 차감, 레벨 상승,
    // RuntimeStat·RuntimeLevel 재계산까지 완료된 상태
}
```

`TryUpgrade`는 데이터, 최대 레벨, 시장 레벨 제한, 보유 재화를 다시 검사한다. 따라서 UI의 선행 검사가 통과했더라도 최종 성공 여부는 `TryUpgrade`의 반환값을 사용한다.

업그레이드 후 UI를 갱신해야 하면 `SubscribeUpgradeChanged`를 구독하고, 해당 UI가 사용되지 않을 때 `UnsubscribeUpgradeChanged`로 구독을 해제한다.

## 영업 담당자

영업 코드에서는 어떤 레벨이 필요한지에 따라 조회 경로를 구분한다.

```csharp
// 현재 시장 레벨
int marketLevel = GameManager.Instance.Market.MarketData.CurrentLevel;

// 직원·시설·요리의 현재 업그레이드 레벨
RuntimeLevel levels = GameManager.Instance.Upgrade.RuntimeLevel;
int employeeLevel = levels.Get(EmployeeType.Server_1);
int facilityLevel = levels.Get(FacilityType.Table_1);
int dishLevel = levels.Get(DishType.MeatOnigiri);

// 시설 업그레이드로 계산된 영업 스탯
float customerCount = GameManager.Instance.Upgrade.RuntimeStat.Service
    .Get(ServiceStatType.CustomerCount);
```

시장 레벨은 콘텐츠 해금 조건이고, `RuntimeLevel`은 각 업그레이드 타겟의 성장 레벨이다. `RuntimeStat.Service`는 그 레벨들을 계산한 실제 영업 적용값이다.

고객의 결제가 확정될 때는 보유 재화를 다음과 같이 증가시킨다.

```csharp
DishDataSO dishData = DishDataDB.GetData(orderedDish);

if (dishData != null)
    GameManager.Instance.StockManager.AddCurrency(dishData.Cost);
```

`orderedDish`는 결제가 완료된 주문의 `DishType`이다. 현재는 `CustomerEatState.FinishEating`이 같은 흐름으로 재화를 증가시킨다.

`StockManager.AddCurrency`만 호출하면 플레이어의 실제 돈과 재고 변경 알림만 갱신된다. `MarketData.TotalIncome`은 자동으로 늘어나지 않는다.

영업 중에는 영업 담당 객체가 해당 영업의 수익을 별도로 누적한다. 영업이 끝났을 때 그 합계를 `TotalIncome`에 한 번만 반영한다.

```csharp
private int earnedIncomeThisService;

private void CompletePayment(int earnedCurrency)
{
    GameManager.Instance.StockManager.AddCurrency(earnedCurrency);
    earnedIncomeThisService += earnedCurrency;
}

private void CompleteService()
{
    MarketData marketData = GameManager.Instance.Market.MarketData;
    marketData.TotalIncome += earnedIncomeThisService;
    earnedIncomeThisService = 0;
}
```

이 두 값의 책임은 다르다.

- `StockManager.StockData.Currency`: 즉시 사용할 수 있는 현재 보유 재화
- `MarketData.TotalIncome`: 레벨 미션과 진행도에 사용하는 누적 영업 수입

영업 종료 처리는 `ServiceEventType.LoopEnded`를 구독해 실행할 수 있다. `ServiceManager.EndLoop`는 `LoopEnded` 구독자를 먼저 호출한 뒤 `Hub`로 전환한다.
