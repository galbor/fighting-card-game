using System.Collections;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace DefaultNamespace.Relics
{
    public abstract class AbstractRelic : MonoBehaviour
    {
        [SerializeField] protected bool _resetEveryTurn;
        [SerializeField] protected bool _resetEveryCombat;   
        
        private float _activateScaleMultiplier = 1.25f;
        private float _shrinkingScaleMultiplier = 1.01f;

        private Coroutine _shrinkingCoroutine;

        protected virtual void Activate()
        {
            if (_shrinkingCoroutine != null) 
                StopCoroutine(_shrinkingCoroutine);
            transform.localScale *= _activateScaleMultiplier;
            _shrinkingCoroutine = StartCoroutine(ReduceSize());
        }

        IEnumerator ReduceSize()
        {
            while (transform.localScale.x > 1f)
            {
                transform.localScale /= _shrinkingScaleMultiplier;
                yield return null;
            }

            transform.localScale = Vector3.one;
        }
    }
}