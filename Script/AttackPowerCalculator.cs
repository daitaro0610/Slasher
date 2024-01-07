using UnityEngine;

public static class AttackPowerCalculator
{
    //最小値から最大値までの値をランダムに
    private const float MIN_RANDOM_MULTIPLIER = 0.8f;   //最小値
    private const float MAX_RANDOM_MULTIPLIER = 1.2f;   //最大値

    //デフォルトの値を何倍した物を返却するのかどうか
    private const int DEFALUT_VALUE_MULTIPLIER = 100;

    public static int Attack(int attackValue)
    {
        float attackPowor = attackValue * DEFALUT_VALUE_MULTIPLIER;
        float multiplier = Random.Range(MIN_RANDOM_MULTIPLIER, MAX_RANDOM_MULTIPLIER);
        attackPowor *= multiplier;

        //小数点以下切り捨てで行う
        return (int)attackPowor;
    }
}
