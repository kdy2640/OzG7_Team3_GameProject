using UnityEngine;

public class CleaningButton : MonoBehaviour
{
    private ServerList serverList;
    private Dirty dirty;

    private bool isClicked;

    private void OnEnable()
    {
        serverList = FindFirstObjectByType<ServerList>();
        dirty = GetComponentInParent<Dirty>();
        isClicked = false;
    }
    public void OnClick()
    {
        if(serverList.TryAllocClean(dirty))
        {
            isClicked = true;
            Destroy(this.gameObject);
        }
        else
        {
            // 서버 바쁨 메시지 
        }
    }
}
