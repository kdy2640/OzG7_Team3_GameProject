using UnityEngine;

public abstract class AnimalStateBase : MonoBehaviour
{
    protected AnimalStateController Controller { get; private set; }

    public abstract void StateStart();
    public abstract void StateEnd();

    public void Init(AnimalStateController controller)
    {
        Controller = controller;
        enabled = false;
    }
}
