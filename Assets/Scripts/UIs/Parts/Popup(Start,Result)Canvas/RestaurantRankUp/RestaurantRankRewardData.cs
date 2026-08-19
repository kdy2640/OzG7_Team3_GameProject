using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class RestaurantRankRewardData
{
    [SerializeField] private int level;
    [SerializeField] private List<Sprite> newMenus = new();
    [SerializeField] private List<Sprite> newIngredients = new();
    [SerializeField] private List<string> newFunctions = new();

    public int Level => level;
    public IReadOnlyList<Sprite> NewMenus => newMenus;
    public IReadOnlyList<Sprite> NewIngredients => newIngredients;
    public IReadOnlyList<string> NewFunctions => newFunctions;
}
[CreateAssetMenu(fileName = "RestaurantRankRewardSO", menuName = "Game/Restaurant Rank Reward")]
public sealed class RestaurantRankRewardSO : ScriptableObject
{
    [SerializeField] private List<RestaurantRankRewardData> rewards = new();
    public RestaurantRankRewardData GetReward(int level)
    {
        for(int i = 0; i < rewards.Count; i++)
        {
            if (rewards[i] != null && rewards[i].Level == level)
                return rewards[i];
        }
        return null;
    }
}
