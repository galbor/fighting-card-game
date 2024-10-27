using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Vector3 = UnityEngine.Vector3;

namespace DefaultNamespace.Relics
{
    public abstract class AbstractRelic : MonoBehaviour
    {
        private const string TEXTPREFAB = "TextPrefab";
        
        [SerializeField] protected bool _resetEveryTurn;
        [SerializeField] protected bool _resetEveryCombat;
        [SerializeField] protected bool _displayCounter;

        private Image _image;
        
        private TMP_Text _counterText;
        private const float COUNTERTEXTFONTSIZE = 16f;
        private int _counter;
        protected int _counterCycleLength = Int32.MaxValue;
        protected int Counter
        {
            get => _counter;
            set
            {
                _counter = value;
                _counterText.text = value.ToString();
            }
        }

        private float _activateScaleMultiplier = 1.25f;
        private float _shrinkingScaleMultiplier = 1.01f;

        private Coroutine _shrinkingCoroutine;

        protected void Awake()
        {
            _image = gameObject.GetComponent<Image>();
            
            _counterText = Instantiate(Resources.Load<TMP_Text>(TEXTPREFAB), transform);
            _counterText.rectTransform.anchorMin = _counterText.rectTransform.anchorMax = new Vector2(1, 0);
            _counterText.fontSize = COUNTERTEXTFONTSIZE;
            if (!_displayCounter) _counterText.gameObject.SetActive(false);
        }

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

        protected void SetUsable(bool usable)
        {
            _image.color = usable ? Color.white : Color.gray;
        }

        protected int IncrementCounter(object obj = null)
        {
            return Counter = (Counter + 1) % _counterCycleLength;
        }

        protected void ResetCounter(object obj = null)
        {
            Counter = 0;
        }
    }
}