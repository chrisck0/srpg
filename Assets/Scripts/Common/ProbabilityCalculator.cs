using UnityEngine;

public static class ProbabilityCalculator
{
    public static bool AttackProbability(int turn, UnitStat ownerStat, WeaponData weaponData, UnitStat targetStat)
    {
        float probability = ((float)ownerStat.UnitData.Accuracy / 100) * (1 - (float)targetStat.UnitData.Avoid / 100);

        int seed = ownerStat.UnitData.Id * targetStat.UnitData.Id 
            + ownerStat.Health + ownerStat.Energy
            + weaponData.Attack + weaponData.EnergyCost
            + targetStat.Health + targetStat.Energy
            + turn;

        System.Random rand = new System.Random(seed);
        float randomValue = (float)rand.NextDouble();
        Debug.Log(randomValue);
        return (randomValue <= probability);
    }
}
