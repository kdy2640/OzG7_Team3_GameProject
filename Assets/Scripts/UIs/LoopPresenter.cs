using UnityEngine;

[DisallowMultipleComponent]
public sealed class LoopPresenter : MonoBehaviour
{
    private const int GridSize = 3;

    [SerializeField] private Vector2 moveDirection = Vector2.left;
    [SerializeField, Min(0f)] private float moveSpeed = 100f;

    private RectTransform[] backgrounds;
    private Vector2 loopSize;
    private Vector2 halfLoopSize;

    private void Awake()
    {
        backgrounds = new RectTransform[transform.childCount];

        for (int i = 0; i < backgrounds.Length; i++)
        {
            backgrounds[i] = (RectTransform)transform.GetChild(i);
        }

        Vector2 backgroundSize = backgrounds[0].rect.size;
        loopSize = backgroundSize * GridSize;
        halfLoopSize = loopSize * 0.5f;
    }

    private void Update()
    {
        Vector2 movement = moveDirection.normalized * moveSpeed * Time.deltaTime;

        for (int i = 0; i < backgrounds.Length; i++)
        {
            Vector2 position = backgrounds[i].anchoredPosition + movement;

            if (position.x > halfLoopSize.x)
            {
                position.x -= loopSize.x;
            }
            else if (position.x < -halfLoopSize.x)
            {
                position.x += loopSize.x;
            }

            if (position.y > halfLoopSize.y)
            {
                position.y -= loopSize.y;
            }
            else if (position.y < -halfLoopSize.y)
            {
                position.y += loopSize.y;
            }

            backgrounds[i].anchoredPosition = position;
        }
    }
}
