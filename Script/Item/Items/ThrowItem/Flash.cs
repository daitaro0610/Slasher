using System.Collections.Generic;
using UnityEngine;
using KD.Tweening;
using KD;

public class Flash : MonoBehaviour
{
    private Light m_Light;

    [SerializeField]
    private float m_Intensity; //光の強さ

    [SerializeField]
    private float m_LightDelay;//光り続ける時間

    [SerializeField]
    private float m_Duration;  //光る時間と消える時間

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

        //閃光玉の音を鳴らす
        AudioManager.instance.PlaySEFromObjectPosition(SoundName.Flash,gameObject);

        //フラッシュ用のTween処理
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
    /// 敵がライトの範囲内にいるかどうかの判定
    /// </summary>
    private void CheckEnemyInLight()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, m_Light.range, m_DetectionLayer);

        List<GameObject> flashTargetList = new();

        foreach (Collider collider in hitColliders)
        {
            //同じオブジェクトの場合はもうダメージ処理を行わない
            if (flashTargetList.Contains(collider.transform.root.gameObject)) continue;

            flashTargetList.Add(collider.transform.root.gameObject);
        }

        foreach (GameObject enemyObj in flashTargetList)
        {
            //敵側で正面にあるかどうか判定し、正面にある場合は混乱する
            EnemyControllerBase enemy = enemyObj.GetComponent<EnemyControllerBase>();
            
            if (enemy != null)
                enemy.CheckFlashInFront(transform.position);
        }
    }
}