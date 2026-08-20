using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ServerSkillManager
{
    public void SkillUpdate(IReadOnlyList<ServerStateManager> servers)
    {
        if (servers[0] != null)
            Server1Update(servers[0]);
        if (servers[1] != null)
            Server2Update(servers[1]);
        if (servers[2] != null)
            Server3Update(servers[2]);
        if (servers[3] != null)
            Server4Update(servers[3]);
    }

    private void Server1Update(ServerStateManager server)
    {
        Debug.Log("server1 LV : " + server.Level);

        if(server == null)
        {
            return;
        }

        if(server.Level >= 3)
        {
            server.AutoServe();
        }

        if(server.Level >= 5)
        {
            server.customerChanged += server.CustomerEatSpeedUp;
        }
    }
    private void Server2Update(ServerStateManager server)
    {
        if (server == null)
        {
            return;
        }

        server.UpgradeBaseSpeed();

        if (server.Level >= 3)
        {
            server.AutoServe();
        }

        if (server.Level >= 5)
        {
            server.customerChanged += server.CustomerEatSpeedUp;
        }
    }
    private void Server3Update(ServerStateManager server)
    {
        if (server == null)
        {
            return;
        }
        server.AutoServe();

        if( server.Level >= 3)
        {
            server.WorkSpeedUp();
        }

        if (server.Level >= 5)
        {
            server.customerChanged += server.CustomerTipChanceUp;
        }
    }
    private void Server4Update(ServerStateManager server)
    {
        if (server == null)
        {
            return;
        }
        server.customerChanged += server.CustomerEatSpeedUp;

        if (server.Level >= 3)
        {
            server.UpgradeBaseSpeed();
        }

        if (server.Level >= 5)
        {
            server.customerChanged += server.CustomerTipChanceUp;
        }
    }
}
