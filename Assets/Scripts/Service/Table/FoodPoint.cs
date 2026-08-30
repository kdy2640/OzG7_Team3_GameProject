using UnityEngine;

public class FoodPoint : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.orange;
        Gizmos.DrawSphere(transform.position, 0.2f);
    }
}
