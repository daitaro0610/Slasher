using UnityEngine;

public static class AttackPowerCalculator
{
    //�ŏ��l����ő�l�܂ł̒l�������_����
    private const float MIN_RANDOM_MULTIPLIER = 0.8f;   //�ŏ��l
    private const float MAX_RANDOM_MULTIPLIER = 1.2f;   //�ő�l

    //�f�t�H���g�̒l�����{��������ԋp����̂��ǂ���
    private const int DEFALUT_VALUE_MULTIPLIER = 100;

    public static int Attack(int attackValue)
    {
        float attackPowor = attackValue * DEFALUT_VALUE_MULTIPLIER;
        float multiplier = Random.Range(MIN_RANDOM_MULTIPLIER, MAX_RANDOM_MULTIPLIER);
        attackPowor *= multiplier;

        //�����_�ȉ��؂�̂Ăōs��
        return (int)attackPowor;
    }
}
