using System.Collections.Generic;
using UnityEngine;

public static class MessageDataDB
{
    private const string LoadPath = "SOs/ServiceMessageDataSO";

    private static Dictionary<MessageType, List<MessageDataSO>> messageDataMap;

    public static List<MessageDataSO> GetData(MessageType messageType)
    {
        Initialize();

        if (!messageDataMap.TryGetValue(messageType, out List<MessageDataSO> data))
        {
            return null;
        }

        return data;
    }

    private static void Initialize()
    {
        if (messageDataMap != null)
        {
            return;
        }


        messageDataMap = new Dictionary<MessageType, List<MessageDataSO>>();

        MessageDataSO[] resources =
            Resources.LoadAll<MessageDataSO>(LoadPath);
        Debug.Log("resources.Length : " + resources.Length);

        foreach (MessageDataSO data in resources)
        {
            if (data == null)
                continue;


            if (!messageDataMap.ContainsKey(data.MessageType))
            {
                messageDataMap.Add(
                    data.MessageType,
                    new List<MessageDataSO>());
            }

            messageDataMap[data.MessageType].Add(data);
        }

    }
}