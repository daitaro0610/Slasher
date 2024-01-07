using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace KD
{
    public class EventManager : MonoBehaviour
    {
        public static EventManager instance;

        private EventSystem m_Event;
        public EventSystem current => m_Event;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

            //EventSystem‚ðŽæ“¾‚·‚é
            m_Event = FindAnyObjectByType<EventSystem>();
        }
    }
}