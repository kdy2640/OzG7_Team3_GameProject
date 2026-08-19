public sealed class SalesResultBuilder
{
    private SalesResultData currentResult;

    public void Reset(int yesterdaySales)
    {
        currentResult = new SalesResultData
        {
            yesterdaySales = yesterdaySales > 0 ? yesterdaySales : 0
        };
    }

    public void RecordCustomer()
    {
        if (currentResult == null)
            return;

        currentResult.customerMax++;
    }

    public void RecordDishSale(DishType dishType, int salesAmount)
    {
        if (currentResult == null || salesAmount <= 0)
            return;

        currentResult.customerReceived++;
        currentResult.todaySales += salesAmount;

        SalesResultData.MenuSalesData menuSales =
            currentResult.menuSales.Find(data => data.dishType == dishType);

        if (menuSales == null)
        {
            currentResult.menuSales.Add(new SalesResultData.MenuSalesData
            {
                dishType = dishType,
                salesAmount = salesAmount
            });
            return;
        }

        menuSales.salesAmount += salesAmount;
    }

    public void RecordTip(int tipAmount)
    {
        if (currentResult == null || tipAmount <= 0)
            return;

        currentResult.tipSales += tipAmount;
    }

    public SalesResultData Build()
    {
        SalesResultData result = currentResult;
        currentResult = null;
        return result;
    }
}
