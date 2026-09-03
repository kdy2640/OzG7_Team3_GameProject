using UnityEngine;

public sealed class GroceryArgs : PoolArgs
{
    public readonly Vector3 WorldPosition;
    public readonly GroceryType GroceryType;

    public GroceryArgs(Vector3 worldPosition, GroceryType groceryType)
    {
        WorldPosition = worldPosition;
        GroceryType = groceryType;
    }
}

public sealed class GroceryPooler : Pooler<GroceryPresenter>
{
}
