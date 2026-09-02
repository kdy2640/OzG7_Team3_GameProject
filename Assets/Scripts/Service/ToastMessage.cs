using System.Collections;
using TMPro;
using UnityEngine;

public class ToastMessage : MonoBehaviour
{
    [SerializeField] GameObject messageImg;
    [SerializeField] TMP_Text tmpText;
    [SerializeField] float duration = 2f;

    public void ShowMessage(string message)
    {
        StopAllCoroutines();

        tmpText.text = message;
        messageImg.gameObject.SetActive(true);

        StartCoroutine(HideMessageCo());
    }

    private IEnumerator HideMessageCo()
    {
        yield return new WaitForSeconds(duration);

        messageImg.SetActive(false);
    }
}
