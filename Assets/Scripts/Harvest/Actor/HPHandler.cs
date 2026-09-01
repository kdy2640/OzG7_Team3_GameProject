using System;
using UnityEngine;

public class HPHandler : MonoBehaviour
{
    [SerializeField] public float maxHp;
    public float nowHp { get; private set; }

    private Action<float> OnHPUpdate;
    private Action OnDied;

    public void Init(int _maxHP)
    { 
        nowHp = _maxHP;
        OnHPUpdate?.Invoke(nowHp);
    } 

    public void TakeHeal(float heal)
    {
        nowHp = Mathf.Min(nowHp + heal, maxHp);
        OnHPUpdate?.Invoke(nowHp);
    }
    public void TakeDamage(float damage)
    {
        nowHp = Mathf.Max(nowHp - damage, 0);
        OnHPUpdate?.Invoke(nowHp);
        if (nowHp == 0)
        {
            OnDie();
            return;
        }
    }
    public void SubscribeHPUpdate(Action<float> ev)
    {
        OnHPUpdate += ev;
    }
    public void UnSubscribeHPUpdate(Action<float> ev)
    {
        OnHPUpdate -= ev;
    }
    public void SubscribeDying(Action ev)
    {
        OnDied += ev;
    }
    public void UnSubscribeDying(Action ev)
    {
        OnDied -= ev;
    }
    private void OnDie()
    {
        OnDied?.Invoke();
    }
}
