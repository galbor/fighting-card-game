using System;
using UnityEngine;
using UnityEngine.Events;

namespace DefaultNamespace.Utility
{
    /**
     * triggers an event when entering trigger
     */
    public class SeperateTrigger : MonoBehaviour
    {
        [SerializeField] private UnityEvent _event;

        private void OnTriggerEnter2D(Collider2D obj)
        {
            _event.Invoke();
        }
    }
}