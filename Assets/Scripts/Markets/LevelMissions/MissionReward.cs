using System;

[Serializable]
public abstract class MissionReward
{
    public abstract bool TryGrant();
    public abstract override string ToString();
}
