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
    private float timer;
    private bool acceled = false;

    private void OnEnable()
    {
        CreateServers();
        accelerationButton.OnClicked += Acceleration;
    }

    private void Update()
    {
        if(!acceled)
        {
            return;
        }

        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            Deceleration();
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
                server.SetServerDish(dish, customer);
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
                server.SetCustomer(customer);
                server.ChangeState(new ServerCatchRunnerState(server));
                catcher = server;
                return true;
            }
        }
        catcher = null;
        return false;
    }



    private void OnDisable()
    {
        servers.Clear();
    }

    private void Acceleration()
    {
        acceled = true;
        
        foreach (ServerStateManager server in servers)
        {
            if (server == null) continue;
            server.AiMove.Acceleration();
            server.Animator.speed = 2f;
        }

        timer = accelDuration;
    }

    private void Deceleration()
    {
        acceled = false;

        foreach (ServerStateManager server in servers)
        {
            if (server == null) continue;
            server.AiMove.Deceleration();
            server.Animator.speed = 1f;
        }
    }
}