using UnityEngine;

public static class ProbabilityCalculator
{
    public static bool CalculateProbability(int turn, int totalPlayerUnits, int currentTurnCount,
        UnitStat ownerStat, WeaponData weaponData, UnitStat targetStat)
    {
        float probability = ((float)ownerStat.UnitData.Accuracy / 100) * (1 - (float)targetStat.UnitData.Avoid / 100);

        int seed = ownerStat.UnitData.Id * targetStat.UnitData.Id * weaponData.Id
            + ownerStat.Health + ownerStat.Energy
            + weaponData.Attack + weaponData.EnergyCost
            + targetStat.Health + targetStat.Energy
            + turn * totalPlayerUnits + currentTurnCount;

        System.Random rand = new System.Random(seed);
        float randomValue = (float)rand.NextDouble();
        return (randomValue <= probability);
    }

    public static float GetAttackProbability(UnitStat ownerStat, UnitStat targetStat)
    {
        return ((float)ownerStat.UnitData.Accuracy / 100) * (1 - (float)targetStat.UnitData.Avoid / 100);
    }
}
