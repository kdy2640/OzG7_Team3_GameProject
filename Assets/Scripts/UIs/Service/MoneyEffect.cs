
using DG.Tweening;
using TMPro;
using UnityEngine;

public class MoneyEffect : MonoBehaviour
{
    [SerializeField] private TMP_Text amountText;
    [SerializeField] private float floatingDuration;

    private void OnEnable()
    {
        floatingDuration = 2.0f;
    }

    private void Update()
    {
        floatingDuration -= Time.deltaTime;
        if( floatingDuration <= 0 )
        {
            Destroy(gameObject);
        }
    }

    public void SetAmount(int amount)
    {
        amountText.text = "+ " + amount.ToString() + "!";

        transform.DOMoveY(transform.position.y + 1.0f, floatingDuration).SetEase(Ease.OutCubic).SetLink(gameObject);
        //amountText.DOFade(0f, floatingDuration).OnComplete(() => Destroy(gameObject));
    }
}
