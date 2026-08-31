using System;
using System.Collections.Generic;
using UnityEngine;

public class TableManager : MonoBehaviour
{
    // 테이블 매니저 인스펙터 사용
    [SerializeField] private List<Table> tables = new();
    private Queue<CustomerStateManager> waitingQueue = new();
    public event Action SetTableDone;
    public IReadOnlyList<Table> Tables => tables;
    public int WaitingCount => waitingQueue.Count;
    public int UsableSeatCount
    {
        get
        {
            int count = 0;

            for (int i = 0; i < tables.Count; i++)
            {
                int level = GameManager.Instance.Upgrade.RuntimeLevel.Get((FacilityType)i);

                if (level <= 0)
                    continue;

                count++;

                if (level > 2)
                    count++;
            }

            return count;
        }
    }

    private void Start()
    {
        SetTable();
    }

    private void SetTable()
    {
        for (int i = 0; i < tables.Count; i++)
        {
            FacilityType table = (FacilityType)i;
            if(GameManager.Instance.Upgrade.RuntimeLevel.Get(table) < 1)
            {
                tables[i].gameObject.SetActive(false);
            }
            if (GameManager.Instance.Upgrade.RuntimeLevel.Get(table) > 2)
            {
                tables[i].OpenLeftSeat();
            }
        }
        SetTableDone?.Invoke();
    }

    public Table FindEmptyTable()
    {
        for (int i = 0; i < tables.Count; i++)
        {
            FacilityType tableType = (FacilityType)i;

            if (GameManager.Instance.Upgrade.RuntimeLevel.Get(tableType) == 0)
                continue;

            Table table = tables[i];

            if (table.HasEmptySeat())
            {
                return table;
            }
        }
        return null;
    }

    public Table FindTable(CustomerStateManager customer)
    {
        foreach (Table table in tables)
        {
            if (table.HasCustomer(customer))
            {
                return table;
            }
        }
        return null;
    }

    public void AddWaitingCustomer(CustomerStateManager customer)
    {
        waitingQueue.Enqueue(customer);
    }

    public bool TryGetSeatForWaitingCustomer()
    {
        if (waitingQueue.Count == 0)
            return false;

        CustomerStateManager customer = waitingQueue.Peek();

        Table table = FindEmptyTable();

        if (table != null)
        {
            waitingQueue.Dequeue();

            Transform seat = table.ReserveSeat(customer);

            customer.AssignTable(table, seat);

            customer.ChangeState(
                new CustomerMoveToTableState(
                    customer,
                    customer.AiMove,
                    seat
                )
            );

            return true;
        }
        return false;
    }

    public bool IsThereAnyWaiting()
    {
        if (waitingQueue.Count > 0)
        {
            return true;
        }
        else return false;
    }
}
