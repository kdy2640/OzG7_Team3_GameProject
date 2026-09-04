using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ServiceWarningMessage : MonoBehaviour
{
    public void ShowMessage(MessageType messageType)
    {

        List<MessageDataSO> messages = MessageDataDB.GetData(messageType);

        MessageDataSO content =
            messages[Random.Range(0, messages.Count)];

        GameManager.Instance.Utility.Toast.Show(content.Message);
    }
}
