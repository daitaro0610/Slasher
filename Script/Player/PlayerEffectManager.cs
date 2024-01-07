using UnityEngine;
using KD;

public class PlayerEffectManager : MonoBehaviour
{
    [SerializeField]
    private GameObject m_BaseGeneratePoint;

    [SerializeField]
    private Vector3[] m_RotationOffsetDatas;

    private void FastSlash(int rotationIndex)
       => EffectManager.instance.Play(EffectName.FastSlash, m_BaseGeneratePoint,rotation:m_RotationOffsetDatas[rotationIndex]);

    private void HorizonSlash(int rotationIndex)
       => EffectManager.instance.Play(EffectName.HorizonSlash, m_BaseGeneratePoint, rotation: m_RotationOffsetDatas[rotationIndex]);

    private void FastHorizonSlash(int rotationIndex)
        => EffectManager.instance.Play(EffectName.FastHorizonSlash, m_BaseGeneratePoint, rotation: m_RotationOffsetDatas[rotationIndex]);

    private void VarticalSlash(int rotationIndex)
        => EffectManager.instance.Play(EffectName.VarticalSlash, m_BaseGeneratePoint, rotation: m_RotationOffsetDatas[rotationIndex]);

    private void FastVarticalSlash(int rotationIndex)
        => EffectManager.instance.Play(EffectName.FastVarticalSlash, m_BaseGeneratePoint, rotation: m_RotationOffsetDatas[rotationIndex]);

    private void DownSlash(int rotationIndex)
        => EffectManager.instance.Play(EffectName.DownSlash, m_BaseGeneratePoint, rotation: m_RotationOffsetDatas[rotationIndex]);

    private void RoundHorizon(int rotationIndex)
        => EffectManager.instance.Play(EffectName.RoundHorizon, m_BaseGeneratePoint, rotation: m_RotationOffsetDatas[rotationIndex]);

    private void Prick(int rotationIndex)
        => EffectManager.instance.Play(EffectName.Prick, m_BaseGeneratePoint, rotation: m_RotationOffsetDatas[rotationIndex]);

    private void StrikeSlash(int rotationIndex)
        => EffectManager.instance.PlayPosition(EffectName.StrikeSlash, m_BaseGeneratePoint.transform.position, rotation:transform.eulerAngles + m_RotationOffsetDatas[rotationIndex]);

    private void StrongHorizonSlash(int rotationIndex)
        => EffectManager.instance.Play(EffectName.StrongHorizonSlash, m_BaseGeneratePoint, rotation: m_RotationOffsetDatas[rotationIndex]);

    private void SpecialSlashFirst(int rotationIndex)
        => EffectManager.instance.Play(EffectName.SpecialSlashFirst, m_BaseGeneratePoint, rotation: m_RotationOffsetDatas[rotationIndex]);

    private void SpecialSlashSecond(int rotationIndex)
        => EffectManager.instance.Play(EffectName.SpecialSlashSecond, m_BaseGeneratePoint, rotation: m_RotationOffsetDatas[rotationIndex]);

    private void CounterSlash(int rotationIndex)
        => EffectManager.instance.Play(EffectName.CounterSlash, m_BaseGeneratePoint, rotation: m_RotationOffsetDatas[rotationIndex]);
}

