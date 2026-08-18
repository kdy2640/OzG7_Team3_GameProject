using System.Collections.Generic;
using UnityEngine;

public sealed class ServiceSessionResultBuilder
{
    public ServiceResultData CreateResult(int customerReceived, int customerMax,
        IReadOnlyList<MenuSalesResultData> menuSales, int tipAmount)
    {
        ServiceResultData result = new ServiceResultData
        {
            customerReceived = Mathf.Max(0, customerReceived),
            customerMax = Mathf.Max(0, customerMax),
            tipResult = CalculateTip(tipAmount)
        };

        if (menuSales != null)
        {
            foreach (MenuSalesResultData menuResult in menuSales)
            {
                if (menuResult == null)
                    continue;

                result.menuSales.Add(new MenuSalesResultData
                {
                    dishType = menuResult.dishType,
                    menuIcon = menuResult.menuIcon,
                    salesAmount = Mathf.Max(0, menuResult.salesAmount)
                });
            }
        }

        return result;
    }

    private int CalculateTip(int tipAmount)
    {
        if (GameManager.Instance == null || GameManager.Instance.Upgrade == null)
        {
            return 0;
        }

        FacilityUpgradeDataSO tipTableData = UpgradeDataDB.GetData(FacilityType.Decor_6);

        if (tipTableData == null)
            return 0;

        UpgradeState tipTableState = GameManager.Instance.Upgrade.GetState(tipTableData);

        if (tipTableState == null || tipTableState.level <= 0)
            return 0;

        return Mathf.Max(0, tipAmount);
    }
}