using UnityEngine;

public static class ProbabilityCalculator
{
    private static int cnt;

    public static bool AttackProbability(int cnt, UnitStat ownerStat, WeaponData weaponData, UnitStat targetStat)
    {

        float probability = ((float)ownerStat.UnitData.Accuracy / 100) * (1 - (float)targetStat.UnitData.Avoid / 100);

        // 시드 고정 (턴, 아군 순서 추가)
        int seed = ownerStat.Health + ownerStat.Energy + weaponData.Attack + weaponData.EnergyCost + targetStat.Health + targetStat.Energy;

        Random.InitState(seed);
        float randomValue = Random.Range(0f, 1f);
        Debug.Log(randomValue);
        return (randomValue >= 1 - probability);
    }
}
