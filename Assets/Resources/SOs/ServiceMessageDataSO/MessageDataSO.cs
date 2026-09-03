using UnityEngine;

public enum MessageType
{
    cEat,
    cHungry,
    cCaught,
    cTip,
    cAngry,
    cGoToDrink,
    cDrink,
    cGoHome,
    cLateReceive,
    sServe,
    sGoToClean,
    sCatchRunner,
    sMoveToTable,
    sAfterSleep,
    sLateServe,
    wNoDish,
    wNoGrocery,
    wServerBusy,
    wFullStock,
    wFullQueue,
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