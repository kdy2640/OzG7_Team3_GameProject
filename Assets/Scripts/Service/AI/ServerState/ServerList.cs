using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ServerList : MonoBehaviour
{
    [SerializeField] private int serverCount = 4;
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
        for (int i = 0; i < serverCount; i++)
        {
            ServerStateManager server = Instantiate(serverPrefab, transform.position, Quaternion.identity);
            server.SetServerSpot(serverSpots[i]);
            server.SetLevel((EmployeeType)i);
            servers.Add(server);
        }

        skillManager.SkillUpdate(servers);
    }

    


    public bool TryAllocServe(DishType dish, CustomerStateManager customer)
    {
        foreach (ServerStateManager server in servers)
        {
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

    

    private void OnDisable()
    {
        servers.Clear();
    }

    private void Acceleration()
    {
        acceled = true;

        foreach(ServerStateManager server in servers)
        {
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
            server.AiMove.Deceleration();
            server.Animator.speed = 1f;
        }
    }
}