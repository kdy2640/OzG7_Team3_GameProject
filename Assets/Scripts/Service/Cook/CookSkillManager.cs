using System.Collections.Generic;
using System.Diagnostics;

public class CookSkillManager
{
    private List<Cooker> cookers;
    public void Initialize(List<Cooker> cookers)
    {
        this.cookers = cookers;
    }
    public void SkillApply(int index)
    {
        int serchIndex = index + 4;


        switch (index)
        {
            case 0:
                if (GameManager.Instance.Upgrade.RuntimeLevel.Get((EmployeeType)serchIndex) >= 3)
                {
                    AutoCook(cookers[index]);
                }
                if (GameManager.Instance.Upgrade.RuntimeLevel.Get((EmployeeType)serchIndex) >= 5)
                {
                    //TipChanceUp();
                }
                return;
            case 1:
                AutoCook(cookers[index]);
                if (GameManager.Instance.Upgrade.RuntimeLevel.Get((EmployeeType)serchIndex) >= 3)
                {
                    //CustomerEatSpeedUp();
                }
                if (GameManager.Instance.Upgrade.RuntimeLevel.Get((EmployeeType)serchIndex) >= 5)
                {
                    //TipChanceUp();
                }
                return;
            case 2:
                //CustomerEatSpeedUp();
                if (GameManager.Instance.Upgrade.RuntimeLevel.Get((EmployeeType)serchIndex) >= 3)
                {
                    AutoCook(cookers[index]);
                }
                if (GameManager.Instance.Upgrade.RuntimeLevel.Get((EmployeeType)serchIndex) >= 5)
                {
                    //TipChanceUp();
                }
                return;
            default: return;
        }
    }

    private void AutoCook(Cooker cooker)
    {
        cooker.AutoCook();
    }
}
