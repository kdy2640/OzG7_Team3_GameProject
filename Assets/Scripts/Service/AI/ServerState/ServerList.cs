using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ServerList : MonoBehaviour
{
    private static readonly EmployeeType[] ServerTypes =
    {
        EmployeeType.Server_1,
        EmployeeType.Server_2,
        EmployeeType.Server_3,
        EmployeeType.Server_4
    };

    [SerializeField] private ServerStateManager serverPrefab;
    [SerializeField] private List<Transform> serverSpots = new();
    [SerializeField] private AccelerationButton accelerationButton;
    [SerializeField] private float accelDuration;
    private ServerSkillManager skillManager = new();

    private List<ServerStateManager> servers = new();
    private bool acceled = false;
    public bool Acceled => acceled;

    private void OnEnable()
    {
        CreateServers();
    }

    private void Update()
    {
        if(!acceled)
        {
            return;
        }

    }

    private void CreateServers()
    {
        for (int i = 0; i < ServerTypes.Length; i++)
        {
            if (GameManager.Instance.Upgrade.RuntimeLevel.Get(ServerTypes[i]) > 0)
            {
                ServerStateManager server = Instantiate(serverPrefab, transform.position, Quaternion.identity);
                server.SetServerSpot(serverSpots[i]);
                server.SetLevel((EmployeeType)i);
                servers.Add(server);
            }
            else
            {
                servers.Add(null);
            }
        }

        skillManager.SkillUpdate(servers);
    }

    


    public bool TryAllocServe(DishType dish, CustomerStateManager customer)
    {
        foreach (ServerStateManager server in servers)
        {
            if (server == null) continue;
            if (server.IsBusy)
            {
                continue;
            }
            else
            {
                server.IsBusy = true;
                server.SetServerDish(dish, customer);
                GameManager.Instance.Utility.Audio.PlaySFX(SFXType.Service_ServerVoice);
                return true;
            }
        }
        return false;
    }

    public bool TryAllocCatch(CustomerStateManager customer, out ServerStateManager catcher)
    {
        foreach (ServerStateManager server in servers)
        {
            if (server == null) continue;
            if (server.IsBusy)
            {
                continue;
            }
            else
            {
                server.IsBusy = true;
                server.SetCustomer(customer);
                server.ChangeState(new ServerCatchRunnerState(server));
                catcher = server;
                GameManager.Instance.Utility.Audio.PlaySFX(SFXType.Service_ServerVoice);
                return true;
            }
        }
        catcher = null;
        return false;
    }

    public bool TryAllocClean(Dirty dirty)
    {
        foreach (ServerStateManager server in servers)
        {
            if (server == null) continue;
            if (server.IsBusy)
            {
                continue;
            }
            else
            {
                server.IsBusy = true;
                server.ChangeState(new ServerMoveToCleanState(server, dirty));
                GameManager.Instance.Utility.Audio.PlaySFX(SFXType.Service_ServerVoice);
                return true;
            }
        }
        return false;
    }



    private void OnDisable()
    {
        servers.Clear();
    }

    public void Acceleration(float percentage)
    {
        foreach (ServerStateManager server in servers)
        {
            if (server == null) continue;
            server.AiMove.SetSpeed(server.Speed * (1+ percentage));
            server.Animator.speed = 2f;
        }
    }

    public void Deceleration()
    {
        foreach (ServerStateManager server in servers)
        {
            if (server == null) continue;
            server.AiMove.SetSpeed(server.Speed);
            server.Animator.speed = 1f;
        }
    }
}
