using System;

[Serializable]
public abstract class MissionCondition
{
    public abstract bool IsSatisfied();
    public abstract override string ToString();
}
