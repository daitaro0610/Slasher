/// <summary>
/// ���b�̍d���ɂ���ă_���[�W�̗ʂ����炷
/// </summary>
public static class DamageCalculator
{
    public static int Calculate(int armor,int damageValue)
    {
        float damageF = damageValue;
        damageF *= 1 - ((float)armor / 100);
        damageValue = (int)damageF;
        return damageValue;
    }
}
