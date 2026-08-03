using UnityEngine;

public class Table : MonoBehaviour
{
    [Header("Seat")]
    public Transform leftSeat;
    public Transform rightSeat;

    [Header("Serve Point")]
    public Transform leftServePoint;
    public Transform rightServePoint;

    public bool leftOccupied;
    public bool rightOccupied;
}
