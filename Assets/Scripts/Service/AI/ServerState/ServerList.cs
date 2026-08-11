
using System.Collections.Generic;
using UnityEngine;

public class ServerList : MonoBehaviour
{
    [SerializeField] private int serverCount= 4;
    [SerializeField] private ServerStateManager serverPrefab;
    [SerializeField] private List<Transform> serverSpots = new();

    private List<ServerStateManager> servers = new();
    


    private void OnEnable()
    {
        CreateServers();
    }

    private void CreateServers()
    {
        for(int i = 0; i< serverCount; i++)
        {
            ServerStateManager server = Instantiate(serverPrefab, transform);
            server.SetServerSpot(serverSpots[i]);
            servers.Add(server);
        }
    }

    public bool TryAllocServe(DishType dish, CustomerStateManager customer)
    {
        foreach (ServerStateManager server in servers)
        {
            if(server.IsBusy)
            {
                continue;
            }
            else
            {
                server.SetServerDish(dish, customer);
                return true;
            }
        }
        return false;
    }

    private void Clear()
    {
        foreach(ServerStateManager server in servers)
        {
            Destroy(server.gameObject);
        }

        servers.Clear();
    }

    private void OnDisable()
    {
        Clear();
    }
}
