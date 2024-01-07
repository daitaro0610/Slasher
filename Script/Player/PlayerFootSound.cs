using KD;
using UnityEngine;

public class PlayerFootSound : MonoBehaviour
{
    [SerializeField]
    private float m_FootSoundVolume = 0.5f;

    [SerializeField]
    private GameObject[] m_FootPlayPositions;

    public void PlayFootSound(int index)
    {
        if (index >= m_FootPlayPositions.Length) index = m_FootPlayPositions.Length - 1;

        GameObject playObj = m_FootPlayPositions[index];

        int rand = Random.Range(0, 3);
        switch (rand)
        {
            case 0:
                AudioManager.instance.PlaySEFromObjectPosition(SoundName.Foot1, playObj, volume: m_FootSoundVolume) ;
                break;
            case 1:
                AudioManager.instance.PlaySEFromObjectPosition(SoundName.Foot2, playObj, volume: m_FootSoundVolume);
                break;
            case 2:
                AudioManager.instance.PlaySEFromObjectPosition(SoundName.Foot2, playObj, volume: m_FootSoundVolume);
                break;
        }
    }
}
