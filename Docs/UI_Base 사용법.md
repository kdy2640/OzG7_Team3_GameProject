# UI_Base 사용법

이 문서는 Hub의 State UI 프리팹에 UI 컴포넌트를 추가하고 State 스크립트에서 연결하는 방법을 설명한다.

## 동작 구조

`HubCanvasController.Awake()`는 `BuildViewLookup()`에서 등록된 모든 State 프리팹을 생성한다. 각 인스턴스를 비활성화한 뒤 `UI_Base.Init(owner)`를 호출한다. `Init`은 `Owner`를 저장하고 `OnInit()`을 한 번 실행한다.

State 스크립트는 `OnInit()`에서 컴포넌트를 바인딩하고 필요한 의존성과 내부 이벤트를 연결한다. `Start()`에는 다른 객체의 `Awake()` 완료가 필요한 초기 작업을 작성한다. `Init()`과 `OnInit()`은 직접 호출하지 않는다.

State 인스턴스는 생성 직후 비활성화된다. 해당 State의 `Start()`는 처음 활성화될 때 실행된다.

`Bind<T>()`는 State 프리팹의 모든 자식에서 enum 멤버와 이름이 같은 오브젝트를 찾는다. 찾은 오브젝트에 붙은 `T` 컴포넌트를 저장한다. 비활성화된 자식도 검색한다.

## 컴포넌트 추가 절차

예시는 `UI_MenuManagement`에 음식 조리량을 설정하는 `Slider`를 추가하는 경우다.

1. `UI_MenuManagement.prefab`을 연다.
2. Slider가 붙은 자식 오브젝트를 만들고 이름을 `CookingAmountSlider`로 지정한다.
3. `UI_MenuManagement.cs`에 컴포넌트 타입별 enum을 추가한다.
4. `OnInit()`에서 `Bind<Slider>()`를 호출한다.
5. `OnInit()`에서 `GetUI<Slider>()`로 컴포넌트를 가져와 초기값과 이벤트를 설정한다.

```csharp
using System.Collections;
using UnityEngine.UI;

public sealed class UI_MenuManagement : UI_Base
{
    private enum Sliders
    {
        CookingAmountSlider
    }

    private Slider cookingAmountSlider;

    protected override void OnInit()
    {
        Bind<Slider>(typeof(Sliders));
        cookingAmountSlider =
            GetUI<Slider>((int)Sliders.CookingAmountSlider);

        if (cookingAmountSlider == null)
        {
            return;
        }

        cookingAmountSlider.wholeNumbers = true;
        cookingAmountSlider.minValue = 0f;
        cookingAmountSlider.value = 0f;
        cookingAmountSlider.onValueChanged.AddListener(
            OnCookingAmountChanged);
    }

    private void Start()
    {
        // 다른 객체의 Awake 완료 후 필요한 초기 작업을 작성합니다.
    }

    private void OnCookingAmountChanged(float value)
    {
        // 선택한 음식의 조리량 표시를 갱신합니다.
    }

    protected override IEnumerator OnShow()
    {
        // 화면을 표시할 때 갱신할 값과 등장 연출을 작성합니다.
        yield break;
    }

    protected override IEnumerator OnHide()
    {
        // 화면을 숨기기 전 정리할 값과 퇴장 연출을 작성합니다.
        yield break;
    }
}
```

프리팹 자식 이름과 enum 멤버 이름은 대소문자까지 같아야 한다. `CookingAmountSlider` 오브젝트 자체에 `Slider`가 붙어 있어야 한다.

## 여러 타입 연결

컴포넌트 타입마다 enum을 나눈다. enum 이름은 팀에서 찾기 쉬운 복수형을 사용한다.

```csharp
private enum Buttons
{
    ConfirmButton,
    CancelButton
}

private enum Texts
{
    PriceText,
    DescriptionText
}

private enum Images
{
    ItemIcon
}

protected override void OnInit()
{
    Bind<Button>(typeof(Buttons));
    Bind<TextMeshProUGUI>(typeof(Texts));
    Bind<Image>(typeof(Images));
    GetButton((int)Buttons.ConfirmButton)
        ?.onClick.AddListener(OnConfirm);

    GetTextMeshPro((int)Texts.PriceText).text = "0";
    GetImage((int)Images.ItemIcon).enabled = false;
}

private void Start()
{
    // 다른 객체의 Awake 완료 후 필요한 초기 작업을 작성합니다.
}
```

`UI_Base`는 다음 접근 메서드를 제공한다.

| 타입 | 접근 메서드 |
| --- | --- |
| `Text` | `GetText(index)` |
| `Button` | `GetButton(index)` |
| `Image` | `GetImage(index)` |
| `GameObject` | `GetGameObject(index)` |
| `TextMeshProUGUI` | `GetTextMeshPro(index)` |

`Slider`, `Toggle`, 팀에서 만든 컴포넌트는 `GetUI<T>(index)`로 가져온다.

```csharp
private enum ItemSlots
{
    MainItemSlot,
    MaterialItemSlot
}

protected override void OnInit()
{
    Bind<UI_ItemSlot>(typeof(ItemSlots));
    UI_ItemSlot mainSlot =
        GetUI<UI_ItemSlot>((int)ItemSlots.MainItemSlot);

    mainSlot?.Init();
}

private void Start()
{
    // 다른 객체의 Awake 완료 후 필요한 초기 작업을 작성합니다.
}
```

enum에는 숫자를 직접 지정하지 않는다. 첫 멤버부터 `0, 1, 2` 순서가 유지되어야 enum 값을 배열 인덱스로 사용할 수 있다.

같은 State에서 같은 컴포넌트 타입을 두 번 `Bind`하면 마지막 바인딩이 이전 값을 교체한다. 같은 타입의 항목은 하나의 enum에 모은다.

## UI 이벤트 연결

`UI_EventHandler`가 붙은 객체는 `AddUIEvent()` 확장 메서드로 포인터 이벤트를 연결할 수 있다.

```csharp
private enum EventHandlers
{
    PreviewArea
}

protected override void OnInit()
{
    Bind<UI_EventHandler>(typeof(EventHandlers));
    UI_EventHandler previewArea =
        GetUI<UI_EventHandler>((int)EventHandlers.PreviewArea);

    previewArea?.AddUIEvent(OnPreviewClicked);
    previewArea?.AddUIEvent(
        OnPreviewEntered,
        UI_EventHandler.UIEvent.Enter);
}

private void Start()
{
    // 다른 객체의 Awake 완료 후 필요한 초기 작업을 작성합니다.
}
```

기본 이벤트는 `LClick`이다. 지정할 수 있는 이벤트는 `LClick`, `Enter`, `Exit`, `Hold`, `Deselect`다. `AddUIEvent()`는 `UI_EventHandler`를 자동으로 추가하지 않으므로 프리팹에 컴포넌트를 먼저 붙인다.

같은 Action을 같은 이벤트에 다시 추가하면 기존 연결을 제거한 뒤 한 번만 등록한다.

## 표시와 숨김

등장 연출이 필요하면 `OnShow()`를 재정의한다. `Show()`는 오브젝트를 활성화한 뒤 `OnShow()`가 끝날 때까지 기다린다.

```csharp
protected override IEnumerator OnShow()
{
    yield return panel
        .DOFade(1f, 0.2f)
        .WaitForCompletion();
}
```

퇴장 연출은 `OnHide()`에 작성한다. `Hide()`는 `OnHide()`가 끝난 뒤 오브젝트를 비활성화한다.

컴포넌트 바인딩, owner 주입, 같은 State 안의 이벤트 등록은 `OnInit()`에 둔다. 다른 객체의 `Awake()` 완료가 필요한 초기 작업은 `Start()`에 둔다. 화면을 열 때마다 갱신해야 하는 값은 `OnShow()`에서 설정한다.

## State 전이 버튼

`UI_HubStateButton`은 State 전이 버튼에 사용하는 컴포넌트다. 다른 UI 컴포넌트와 같은 방식으로 `Buttons` enum에 연결한다.

```csharp
private enum Buttons
{
    To_HubView
}

protected override void OnInit()
{
    Bind<UI_HubStateButton>(typeof(Buttons));
    GetUI<UI_HubStateButton>((int)Buttons.To_HubView)
        ?.Init(Owner);
}

private void Start()
{
    // 다른 객체의 Awake 완료 후 필요한 초기 작업을 작성합니다.
}
```

프리팹에서 버튼 오브젝트 이름을 `To_HubView`로 지정하고 `targetState`를 `HubView`로 설정한다.

## 프리팹 작업 규칙

- 같은 State 프리팹 안에서 바인딩 대상 이름을 중복해서 사용하지 않는다. 중복되면 계층에서 먼저 검색된 오브젝트가 연결된다.
- 자식 이름을 바꾸면 State 스크립트의 enum 멤버도 같은 작업에서 바꾼다.
- 컴포넌트를 제거하면 enum 멤버와 `GetUI` 사용 코드도 함께 제거한다.
- 기존 State 작업은 해당 State 프리팹과 State 스크립트 안에서 끝낸다.
- `UI_Base`, `UI_EventHandler`, 공용 버튼 프리팹, `HubCanvasController`, HubScene은 담당자와 조율한 뒤 수정한다.
- 새 State를 만들 때는 `HubCanvasState` enum과 `HubCanvasController.views`에도 등록한다.

## 오류 확인

`Bind`가 자식 이름을 찾지 못하면 해당 이름이 Console 경고에 표시된다. 이름, 대소문자, 프리팹 계층을 확인한다.

이름은 찾았지만 컴포넌트가 없으면 요청한 타입이 Console 경고에 표시된다. 이름이 붙은 오브젝트에 컴포넌트가 직접 연결됐는지 확인한다.

`GetUI` 전에 해당 타입을 `Bind`하지 않으면 바인딩되지 않은 타입이라는 경고가 출력된다.

## 작업 완료 체크리스트

- enum의 모든 멤버에 대응하는 자식 오브젝트가 하나씩 있다.
- 프리팹 자식 이름과 enum 멤버 이름의 대소문자가 같다.
- 바인딩 대상 이름을 중복해서 사용하지 않았다.
- 이름이 같은 자식 오브젝트에 요청한 컴포넌트가 직접 붙어 있다.
- enum에 숫자를 직접 지정하지 않았다.
- `OnInit()`에서 각 컴포넌트 타입을 `Bind`했다.
- 같은 컴포넌트 타입을 두 번 `Bind`하지 않았다.
- `Bind` 호출 뒤에 `GetUI`를 사용했다.
- owner 주입과 내부 이벤트 연결을 `OnInit()`에서 끝냈다.
- 다른 객체의 `Awake()` 완료가 필요한 작업만 `Start()`에 작성했다.
- 화면을 열 때마다 바뀌는 값은 `OnShow()`에서 갱신한다.
- 새 이벤트가 한 번만 실행된다.
- State를 닫았다 다시 열어도 값과 이벤트가 정상 동작한다.
- Console에 UI 바인딩 경고와 초기화 오류가 없다.
