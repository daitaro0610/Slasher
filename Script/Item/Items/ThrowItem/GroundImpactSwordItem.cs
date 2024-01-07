using UnityEngine;
using KD;

public class GroundImpactSwordItem : MonoBehaviour
{
    private CharacterAttack m_Attack;

    private void Awake()
    {
        m_Attack = GetComponent<CharacterAttack>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        if(Physics.Raycast(ray,out RaycastHit hit))
        {
            EffectManager.instance.PlayPosition(EffectName.GroundInpact, hit.point);
            m_Attack.SetCollider(0).Attack();
        }
        AudioManager.instance.PlaySEFromObjectPosition(SoundName.GroundImpact,gameObject);
        Destroy(gameObject);
    }
}
