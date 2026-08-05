# 서비스 씬 계약

서비스 씬은 메뉴·재료·완성 요리·돈을 새로 만들거나 별도로 저장하지 않습니다. `GameManager`가 제공하는 기존 매니저를 사용하며, 재고의 단일 원본은 `StockManager`입니다.

## 무엇을 사용할까

| 목적 | 접근 경로 | 계약 |
|---|---|---|
| 보유 재료·요리·돈 조회 | `GameManager.Instance.StockManager.StockData` | 읽기 전용 재고 원본을 제공합니다. |
| 재고·돈 변경 | `GameManager.Instance.StockManager` | 검증 후 차감·추가하고 변경 알림을 발생시킵니다. |
| 조리 | `GameManager.Instance.CookingManager` | 레시피 확인, 재료 차감, 완성 요리 추가를 한 번에 처리합니다. |
| 이번 영업 메뉴 | `GameManager.Instance.Market.Data.SelectedDishes` | 서비스에서 제공할 메뉴 목록입니다. |
| 이름·가격·레시피 | `DishDataDB.TryGetData()` | `DishDataSO` 정적 정의를 조회합니다. |
| 영업 시간·시작·종료 | `GameManager.Instance.Service` | 서비스 씬 생명주기만 담당합니다. |

`DishType`/`GroceryType`은 종류 식별자이고, `DishAmount`/`GroceryAmount`는 기존 API에 종류와 수량을 전달하는 요청값입니다. 이들은 별도 인벤토리가 아닙니다.

## 진행 흐름

```text
서비스 진입
 → Market.Data.SelectedDishes로 메뉴 구성
 → CookingManager.TryCook(dishType)
    (재료 차감 + 완성 요리 재고 추가)
 → StockManager.TryConsumeDish(new DishAmount(dishType, 1)) 성공 후 서빙
 → 식사 완료 시 DishDataSO.Cost를 StockManager.AddCurrency()로 지급
 → 팁은 요리와 독립된 별도 조건·연출에서 AddCurrency()로 지급
```

- 조리는 서비스 씬 안에서만 합니다. 가능 여부 표시는 `CanCook()` 또는 `CalculateCookableAmount()`를 사용하고, 실제 조리는 반드시 `TryCook()`으로 요청합니다.
- 서빙은 완성 요리 차감에 성공한 경우에만 성립합니다. 기본 매출은 조리·서빙 시점이 아니라 식사 완료 시 지급합니다.
- 팁은 `DishDataSO.Cost`, 조리 성공, 요리 재고와 결합하지 않습니다.
- 재고 UI는 `StockData`를 읽고 `SubscribeStockDataChange()`를 구독합니다. 비활성화·파괴 시 반드시 구독을 해제합니다.
- 타이머와 영업 시작·종료 반응은 `ServiceManager.SubscribeTick()`과 `ServiceManager.Events`를 사용합니다.

## 금지 사항

- 서비스 씬 전용 `Inventory`, `IngredientInventory`, `DishInventory`를 생성하지 않습니다.
- 재료·완성 요리 수량 또는 선택 메뉴를 별도 리스트나 딕셔너리에 복제하지 않습니다.
- `StockData`, `MarketData`, `CookingManager`를 직접 생성하지 않습니다.
- 서비스 코드에서 재료 차감과 완성 요리 추가를 따로 구현하지 않습니다.
- 조리·서빙 성공 시 기본 매출 또는 팁을 지급하지 않습니다.

단, API 요청을 위한 `new DishAmount(...)`와 `new GroceryAmount(...)` 생성은 허용합니다.
