using System.Collections.Generic;
using UnityEngine;
using KD.Tweening;
using KD;

public class Flash : MonoBehaviour
{
    private Light m_Light;

    [SerializeField]
    private float m_Intensity; //���̋���

    [SerializeField]
    private float m_LightDelay;//���葱���鎞��

    [SerializeField]
    private float m_Duration;  //���鎞�ԂƏ����鎞��

    [SerializeField]
    private LayerMask m_DetectionLayer;


    private void Awake()
    {
        m_Light = GetComponent<Light>();
    }

    private void Start()
    {
        OnFlash();
    }

    private void OnFlash()
    {
        CheckEnemyInLight();

        //�M���ʂ̉���炷
        AudioManager.instance.PlaySEFromObjectPosition(SoundName.Flash,gameObject);

        //�t���b�V���p��Tween����
        KDTweener.To(() => m_Light.intensity,
            (x) => m_Light.intensity = x,
            m_Intensity,
            m_Duration).OnComplete(() =>
            {
                KDTweener.To(() => m_Light.intensity,
            (x) => m_Light.intensity = x,
            0f,
            m_Duration).SetDelay(m_LightDelay).OnComplete(() =>
            {
                Destroy(gameObject);
            });
            });
    }


    /// <summary>
    /// �G�����C�g�͈͓̔��ɂ��邩�ǂ����̔���
    /// </summary>
    private void CheckEnemyInLight()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, m_Light.range, m_DetectionLayer);

        List<GameObject> flashTargetList = new();

        foreach (Collider collider in hitColliders)
        {
            //�����I�u�W�F�N�g�̏ꍇ�͂����_���[�W�������s��Ȃ�
            if (flashTargetList.Contains(collider.transform.root.gameObject)) continue;

            flashTargetList.Add(collider.transform.root.gameObject);
        }

        foreach (GameObject enemyObj in flashTargetList)
        {
            //�G���Ő��ʂɂ��邩�ǂ������肵�A���ʂɂ���ꍇ�͍�������
            EnemyControllerBase enemy = enemyObj.GetComponent<EnemyControllerBase>();
            
            if (enemy != null)
                enemy.CheckFlashInFront(transform.position);
        }
    }
}