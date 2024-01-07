using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashItem : MonoBehaviour
{
    [SerializeField]
    private GameObject m_Flash;

    [SerializeField]
    private Vector3 m_FlashOffset;
    private void OnCollisionEnter(Collision collision)
    {
        Instantiate(m_Flash, transform.position + m_FlashOffset, Quaternion.identity);
        Destroy(gameObject);
    }
}
