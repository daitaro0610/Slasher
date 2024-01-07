using KD;

public class KatanaSeManager
{
    public void PlayNormalAttack()
        => AudioManager.instance.PlaySE(KatanaAudioName.NormalAttack,1);

    public void PlayCounterAttack()
        => AudioManager.instance.PlaySE(KatanaAudioName.CounterAttack,1);

    public void PlayHeavyAttack()
        => AudioManager.instance.PlaySE(KatanaAudioName.HeavyAttack,1);

    public void PlaySheathingSword()
        => AudioManager.instance.PlaySE(KatanaAudioName.SheathingSword,1);

    public void PlayDrawingSword()
        => AudioManager.instance.PlaySE(KatanaAudioName.DrawSword,1);
}
