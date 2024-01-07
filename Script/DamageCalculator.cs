/// <summary>
/// ‘•b‚Ìd‚³‚É‚æ‚Á‚Äƒ_ƒ[ƒW‚Ì—Ê‚ğŒ¸‚ç‚·
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
