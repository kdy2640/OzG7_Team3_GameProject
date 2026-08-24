using UnityEngine;

public class CleaningButton : MonoBehaviour
{
    private ServerList serverList;
    private Dirty dirty;
    private void OnEnable()
    {
        serverList = FindFirstObjectByType<ServerList>();
        dirty = GetComponentInParent<Dirty>();
    }
    public void OnClick()
    {
        if(serverList.TryAllocClean(dirty))
        {
            Destroy(this.gameObject);
        }
        else
        {
            // 서버 바쁨 메시지 
        }
    }
}
