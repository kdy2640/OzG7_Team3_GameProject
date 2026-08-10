# 수확 씬 개편 설계안

## 문서 목적

이 문서는 수확 씬의 카메라와 식생 렌더링, 수확 판정 구조를 대규모 식생에 맞게 개편하기 위한 방향을 기록한다.

현재는 개편 전 설계 검토 단계이며, 이 문서의 내용이 구현되어 있다는 뜻은 아니다.

## 핵심 방향

- 기존 쿼터뷰 카메라를 더 낮은 숄더뷰에 가까운 시점으로 변경한다.
- 평면 전체에 많은 수의 식생을 배치한다.
- 식생 렌더링에는 GPU Resident Drawer를 사용한다.
- 식생 GameObject에는 개별 Collider와 수확 로직 컴포넌트를 두지 않는다.
- 식생의 수확 판정과 상태는 2D 청크 데이터로 관리한다.
- 커터가 닿을 수 있는 주변 청크만 검색하여 수확을 처리한다.

## "데이터 기반 식생"의 의미

식생을 GameObject 없이 렌더링한다는 뜻이 아니다.

각 식생은 씬에 다음 요소를 가진 GameObject로 존재한다.

- `Transform`
- `MeshFilter`
- `MeshRenderer`

GPU Resident Drawer는 이 GameObject들을 배칭하여 렌더링한다.

반면 다음 요소는 식생 GameObject마다 두지 않는다.

- `Collider`
- `Rigidbody`
- `HPHandler`
- `HarvestActor`
- `HarvestPresenter`
- 개별 수확 판정용 `MonoBehaviour`

위치, 체력, 활성 상태 같은 게임플레이 정보는 별도의 청크 데이터가 관리한다. 즉, 렌더링 표현은 GameObject가 담당하고 수확 규칙은 데이터가 담당한다.

## 현재 구조와 변경 이유

현재 `HarvestSpawner`는 지정된 영역에 `HarvestActor` 프리팹을 반복 생성한다.

- 기본 코드 설정은 9×9 영역에 60개이다.
- 현재 `HarvestScene` 설정은 25×25 영역에 300개이다.
- 각 수확 대상은 HP, Presenter, Mover 등의 컴포넌트를 가진다.
- 시각 프리팹도 초기화 과정에서 별도로 생성한다.
- `CropCutter.OnTriggerStay`가 식생 Collider를 감지하여 피해를 준다.

식생 수가 수천 개 이상으로 증가하면 렌더링뿐 아니라 다음 비용도 함께 증가한다.

- GameObject와 MonoBehaviour 생성 및 관리
- Collider 등록과 Physics broadphase
- Trigger 이벤트 전달
- 컴포넌트 검색과 개별 체력 처리

개편 구조는 식생 Collider와 개별 로직 컴포넌트를 제거하여 이 비용을 줄인다. GPU Resident Drawer는 렌더 제출 비용을 줄이고, 청크 기반 판정은 게임플레이 계산 범위를 줄인다. 두 최적화는 서로 다른 비용을 담당한다.

관련 현재 코드:

- [`HarvestSpawner.cs`](../Assets/Scripts/Harvest/HarvestSpawner.cs)
- [`CropCutter.cs`](../Assets/Scripts/Harvest/Actor/CropCutter.cs)
- [`HarvestActor.cs`](../Assets/Scripts/Harvest/Actor/HarvestActor.cs)
- [`TopViewCameraController.cs`](../Assets/Scripts/Harvest/TopViewCameraController.cs)
- [`HarvestScene.unity`](../Assets/Scenes/InGame/HarvestScene.unity)

## 식생 GameObject 구성

정적 식생 GameObject는 렌더링에 필요한 최소 컴포넌트만 가진다.

```text
Vegetation GameObject
├─ Transform
├─ MeshFilter
└─ MeshRenderer
```

식생은 생성된 뒤 이동하지 않는 것을 기본 전제로 한다. 생성 시 식생의 위치와 청크 인덱스를 계산하여 데이터에 저장한다.

수확 시에는 대응하는 `Renderer`를 끄거나 GameObject를 비활성화한다. GPU Resident Drawer 갱신은 실제로 수확된 개체에만 발생해야 한다.

## 청크 데이터

평면을 XZ 기준의 정사각형 청크로 나눈다. 각 청크는 자신에게 포함된 식생 항목을 보관한다.

식생 항목에 필요한 최소 데이터는 다음과 같다.

```text
VegetationEntry
- position
- harvestType
- hp
- alive
- renderer 또는 gameObject 참조
```

식생이 정적이라면 반복 판정 중 `Transform.position`을 계속 읽지 않고, 생성 시 저장한 `position`을 사용한다. `Transform`은 씬 표현을 위해 존재하지만 공간 판정의 원본 데이터로 매번 접근하지 않는다.

청크 크기는 고정값으로 결정하되, 커터 크기가 증가해도 특정 3×3 청크만 검사한다고 가정해서는 안 된다. 실제 커터 판정 범위가 겹치는 최소·최대 청크 좌표를 매번 계산해야 한다.

## 커터 판정

커터의 현재 중심점만 기준으로 원형 거리 검사를 하면 이동 속도가 빠를 때 프레임 사이의 식생을 건너뛸 수 있다.

따라서 이전 물리 프레임 위치부터 현재 위치까지 커터가 이동한 영역을 검사한다.

```text
이전 커터 위치와 현재 커터 위치
            ↓
이동 구간을 포함하는 swept 영역 계산
            ↓
영역과 겹치는 청크 좌표 계산
            ↓
해당 청크의 살아 있는 식생만 후보로 선택
            ↓
후보에 대한 정확한 거리 또는 커터 형상 판정
            ↓
피해 적용 → 사망 처리 → 보상 지급 → 렌더 비활성화
```

구현 시 지켜야 할 조건은 다음과 같다.

- 거리 비교에는 가능한 경우 제곱 거리를 사용한다.
- 이미 수확된 항목은 후보 검사에서 제외한다.
- 체력이 0이 되는 순간 `alive`를 먼저 변경하여 중복 보상을 막는다.
- 여러 커터가 같은 항목을 처리해도 사망과 보상은 한 번만 발생해야 한다.
- 한 식생을 처리할 때마다 전체 청크나 전체 필드를 다시 구성하지 않는다.

현재 수확 규칙은 작물 HP 3, 커터 피해 1, 피해 간격 0.25초이다. 이 규칙을 유지한다면 데이터 항목에 다음 피해 가능 시각 또는 이에 준하는 피해 누적 상태가 필요하다. 새 개편에서 즉시 절단 방식으로 바꿀 경우에는 별도의 게임 규칙 변경으로 명시한다.

커터가 작물을 자르는 동안 트랙터 이동 속도를 낮추는 기존 동작도 유지 여부를 결정해야 한다. 유지한다면 해당 프레임에 유효한 식생 후보가 하나 이상 있었는지를 커터의 절단 상태로 사용할 수 있다.

## 정적 작물과 이동 대상 분리

현재 수확 타입에는 정적 작물뿐 아니라 닭, 소, 양처럼 이동하는 대상도 포함된다.

정적 작물은 이 문서의 식생 청크 구조를 사용한다. 이동 대상은 위치가 지속적으로 바뀌므로 다음 중 별도 방식으로 관리한다.

- 기존 Actor GameObject 유지
- 개수 제한이 있는 오브젝트 풀 사용
- 필요할 경우 이동 대상 전용 공간 인덱스 사용

이동 대상을 정적 식생 청크에 그대로 넣고 매 프레임 청크 소속을 변경하는 구조는 기본안에 포함하지 않는다.

## GPU Resident Drawer

GPU Resident Drawer는 `MeshRenderer` GameObject를 `BatchRendererGroup` 기반 GPU 인스턴싱으로 렌더링한다. 이 설계에서는 식생 GameObject가 렌더 표현으로 남아 있으므로 GPU Resident Drawer를 사용할 수 있다.

현재 PC 렌더 설정은 다음 조건을 이미 만족한다.

- URP 사용
- SRP Batcher 활성화
- Forward+ 렌더링 경로
- 식생 머티리얼 `M_Gradient`가 `Universal Render Pipeline/Lit` 사용

현재 GPU Resident Drawer 자체는 비활성화 상태이므로 구현 시 활성화와 실제 배칭 여부를 확인해야 한다.

주의사항:

- 동일 메시와 동일 머티리얼을 공유하는 식생이 많을수록 효과가 크다.
- 개별 식생에 `MaterialPropertyBlock`을 사용하지 않는다.
- 식생마다 고유 머티리얼 인스턴스를 만들지 않는다.
- GPU Resident Drawer는 Physics나 MonoBehaviour 비용을 줄이지 않는다.
- 드로우콜이 감소해도 버텍스, 픽셀, 그림자 렌더링 비용은 남는다.
- 실제 적용 여부는 Frame Debugger의 `Hybrid Batch Group`과 Profiler로 확인한다.

현재 PC 설정:

- [`PC_RPAsset.asset`](../Assets/Setting/Settings/PC_RPAsset.asset)
- [`PC_Renderer.asset`](../Assets/Setting/Settings/PC_Renderer.asset)

모바일 렌더러는 현재 일반 Forward 경로이며 Android 그래픽 API에는 OpenGL ES도 포함되어 있다. GPU Resident Drawer는 Forward+ 및 Compute Shader 지원 환경이 필요하고 OpenGL ES에서는 동작하지 않는다. 모바일이 최종 목표 플랫폼이라면 렌더 파이프라인과 그래픽 API 변경을 먼저 확정하지 말고 실제 기기에서 성능을 비교한다.

## 카메라 개편

현재 수확 씬 카메라는 Orthographic 쿼터뷰이다.

숄더뷰에 가까운 느낌을 만들기 위한 기본 방향은 다음과 같다.

- Perspective 카메라 사용
- 기존보다 낮은 높이와 낮은 pitch 사용
- 트랙터 뒤쪽에 카메라 배치
- 필요하면 좌우 어깨 오프셋 추가
- 이동 입력을 카메라 기준 방향으로 변환

낮은 카메라는 지평선 방향으로 더 많은 식생을 화면에 포함한다. 따라서 카메라 변경은 다음 항목과 함께 검증한다.

- 식생 렌더 거리
- 식생 그림자 활성 여부와 그림자 거리
- LOD 필요 여부
- 전방 식생이 플레이어와 커터를 가리는 문제
- 카메라 뒤쪽 청크의 렌더링 제외 여부

GPU Occlusion Culling은 평평하고 개방된 필드에서 항상 이득이라는 보장이 없다. 기본 전제로 활성화하지 않고, 최종 식생 밀도와 카메라에서 측정한 뒤 결정한다.

## 구현 순서 제안

1. 새 카메라 구도를 임시 값으로 구성한다.
2. 최종 후보 식생 메시와 머티리얼로 목표 밀도 테스트 필드를 만든다.
3. 식생에서 Collider와 개별 수확 컴포넌트를 제거한다.
4. 식생 생성 시 청크와 식생 데이터를 함께 구성한다.
5. 커터의 swept 영역 기반 청크 검색과 수확 처리를 구현한다.
6. 수확 결과와 기존 보상 시스템을 연결한다.
7. GPU Resident Drawer를 활성화하고 실제 배칭 여부를 확인한다.
8. 렌더 거리, 그림자, 청크 크기를 대상 기기 프로파일링 결과로 조정한다.
9. 정적 작물과 이동 동물의 생성 및 처리 경로를 분리한다.

## 검증 기준

에디터 Game View의 FPS만으로 판정하지 않는다. PC 빌드 또는 대상 모바일 기기에서 다음 항목을 비교한다.

- 목표 식생 수에서 Main Thread 시간
- Physics 시간
- Render Thread 시간
- GPU Frame Time
- Batches와 SetPass Calls
- 수확 중 GC Alloc
- 한 번에 많은 식생을 자를 때의 프레임 스파이크
- 고속 이동 시 절단 누락 여부
- 여러 커터가 겹칠 때 중복 보상 여부
- 식생 비활성화 후 GPU Resident Drawer 배치 갱신 비용

테스트 밀도는 임의의 한 수치만 사용하지 않고, 낮음·목표·목표 초과의 세 단계로 측정한다.

## 구현 전 확정할 항목

- 최종 주 대상 플랫폼이 PC인지 모바일인지
- 한 필드의 목표 식생 수와 최대 식생 수
- 필드 크기와 식생 평균 간격
- 작물 HP와 반복 피해 규칙을 유지할지 여부
- 수확된 식생의 재생성 또는 리스폰 여부
- 최대 커터 크기와 최대 커터 개수
- 정적 작물과 이동 동물의 목표 개수 비율
- 식생 그림자와 LOD 정책

