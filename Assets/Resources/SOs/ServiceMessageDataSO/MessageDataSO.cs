using UnityEngine;

public enum MessageType
{
    Welcome,
    Hungry,
    Angry,
    Happy,
    Count
}

[CreateAssetMenu(menuName = "Game/MessageDataSO")]
public sealed class MessageDataSO : ScriptableObject
{
    [SerializeField] private MessageType messageType = MessageType.Count;
    [SerializeField, TextArea] private string message;

    public MessageType MessageType => messageType;
    public string Message => message;
}