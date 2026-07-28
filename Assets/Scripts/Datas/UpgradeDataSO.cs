using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 업그레이드 노드를 구성하는 데이터.<br></br>
/// SO이며, 노드 자체를 의미한다긴 보단 노드 뒤에서 주고받는 데이터라고 생각하면 됨.<br></br>
/// 일단 값은 고정될 예정
/// </summary>
[CreateAssetMenu(menuName = "Game/UpgradeDataSO")]
public class UpgradeDataSO : ScriptableObject
{
    public string id;
    public string displayName;
    [field: SerializeField] public Sprite displayIcon;

    public int baseCost;
    public float costMultiplier = 1.2f;

    public int maxLevel = 1;
    public List<StatModifier> statModifiers;

    [SerializeField] private bool isTemporary;

    public bool IsTemporary => isTemporary;

    /// <summary>
    /// 다음 레벨에 따라 필요한 재료량을 뱉어주는 함수.
    /// </summary>
    /// <param name="level">업그레이드 레벨을 넣어주세요</param>
    /// <returns></returns>
    public int GetCosts(int level)
    {

        int scaledAmount = Mathf.RoundToInt(
            baseCost * Mathf.Pow(costMultiplier, level)
        ); 
         
        return scaledAmount;
    } 
}
 
