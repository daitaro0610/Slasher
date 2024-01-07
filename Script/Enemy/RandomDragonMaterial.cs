using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomDragonMaterial : MonoBehaviour
{
    [SerializeField]
    private Material[] m_DragonMaterials;

    //�A�^�b�`���ꂽ�����_���ȃ}�e���A����K�p����
    private void Awake()
    {
        int rand = Random.Range(0, m_DragonMaterials.Length);
        GetComponent<SkinnedMeshRenderer>().material = m_DragonMaterials[rand];
    }
}
