using System.Collections.Generic;
using UnityEngine;

public class TableManager : MonoBehaviour
{
    // 테이블 매니저 인스펙터 사용
    [SerializeField] private List<Table> tables = new();
    private Queue<CustomerStateManager> waitingQueue = new();

    public IReadOnlyList<Table> Tables => tables;

    public Table FindEmptyTable()
    {
        foreach (Table table in tables)
        {
            if (table.HasEmptySeat())
            {
                return table;
            }
        }
        Debug.Log("빈 자리 찾기 실패");
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
        Debug.Log("현재 대기 수 : " + waitingQueue.Count);
    }

    public void GetSeatForWaitingCustomer()
    {
        if (waitingQueue.Count == 0)
            return;

        CustomerStateManager customer = waitingQueue.Peek();

        Table table = FindEmptyTable();

        if (table != null)
        {
            waitingQueue.Dequeue();

            Transform seat = table.ReserveSeat(customer);
            Debug.Log("Seat : " + seat);

            customer.AssignTable(table, seat);

            customer.ChangeState(
                new CustomerMoveToTableState(
                    customer,
                    customer.AiMove,
                    seat
                )
            );
        }
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
