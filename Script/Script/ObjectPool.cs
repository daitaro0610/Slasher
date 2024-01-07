using System.Collections.Generic;
using UnityEngine;

namespace KD.ObjectPool
{
    public class ObjectPool<T> where T : Component
    {
        private readonly Queue<GameObject> m_PoolObject;
        private readonly Transform m_Parent;
        private readonly GameObject m_InstanceObject;

        private const int DEFAULT_POOL_SIZE = 20;

        public ObjectPool(GameObject instanceObject, Transform parent = null)
        {
            m_PoolObject = new();
            m_Parent = (parent == null) ? new GameObject("ObjectPoolParent").transform : parent;
            m_InstanceObject = instanceObject;

            for (int i = 0; i < DEFAULT_POOL_SIZE; i++)
            {
                CreateObject();
            }
        }

        private void CreateObject()
        {
            var createObject = Object.Instantiate(m_InstanceObject, m_Parent);
            m_PoolObject.Enqueue(createObject);
            createObject.SetActive(false);
        }

        public T Get()
        {
            if (m_PoolObject.Count == 0)
            {
                Debugger.Log("’Ç‰Á¶¬");
                CreateObject();
            }

            var getObject = m_PoolObject.Dequeue();
            getObject.SetActive(true);
            return getObject.GetComponent<T>();
        }

        public void Release(T releaseObject)
        {
            if (releaseObject.transform.parent != m_Parent)
                releaseObject.transform.parent = m_Parent;
            releaseObject.transform.position = Vector3.zero;
            releaseObject.gameObject.SetActive(false);
            m_PoolObject.Enqueue(releaseObject.gameObject);
        }
    }
}