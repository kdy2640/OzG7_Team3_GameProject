using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ToastMessage : MonoBehaviour
{
    [SerializeField] GameObject messageImg;
    [SerializeField] TMP_Text tmpText;
    [SerializeField] float duration = 2f;


    public void ShowMessage(MessageType messageType)
    {

        List<MessageDataSO> messages = MessageDataDB.GetData(messageType);

        MessageDataSO content =
            messages[Random.Range(0, messages.Count)];

        tmpText.text = content.Message;

        messageImg.SetActive(true);

        StartCoroutine(HideMessageCo());
    }

    private IEnumerator HideMessageCo()
    {
        yield return new WaitForSeconds(duration);

        messageImg.SetActive(false);
    }
}
