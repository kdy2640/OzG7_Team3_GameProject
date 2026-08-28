using UnityEngine;

public class Seat : MonoBehaviour
{
    // 참조 없는 gizmo용
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.2f);
    }
}
