using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;
using Vector3 = UnityEngine.Vector3;

namespace DefaultNamespace.Relics
{
    public abstract class AbstractRelic : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private const string TEXTPREFAB = "TextPrefab";
        private const string DESCRIPTIONPREFAB = "OnHoverDescriptionDisplay"; 
        
        [SerializeField] protected bool _resetEveryTurn;
        [SerializeField] protected bool _resetEveryCombat;
        [SerializeField] protected bool _displayCounter;
        [SerializeField] private string _description;

        private Image _image;

        private TMP_Text _descriptionTMP;
        private const float DESCRIPTIONFONTSIZE = 24f;
        
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

        protected string Description
        {
            get => _description;
            set
            {
                _description = value;
                _descriptionTMP.text = value;
            }
        }

        private float _activateScaleMultiplier = 1.25f;
        private float _shrinkingScaleMultiplier = 1.01f;

        private Coroutine _shrinkingCoroutine;

        protected void Awake()
        {
            _image = gameObject.GetComponent<Image>();

            _counterText = InstantiateText(new Vector2(1, 0), COUNTERTEXTFONTSIZE);
            if (!_displayCounter) _counterText.gameObject.SetActive(false);

            // _descriptionTMP = Instantiate(Resources.Load<GameObject>(DESCRIPTIONPREFAB), transform).transform
            //     .GetChild(0).GetComponent<TMP_Text>();
            _descriptionTMP = InstantiateText(new Vector2(0.5f, 0), DESCRIPTIONFONTSIZE);
            _descriptionTMP.verticalAlignment = VerticalAlignmentOptions.Top;
            _descriptionTMP.gameObject.SetActive(false);
            Description = _description;
        }

        private TMP_Text InstantiateText(Vector2 anchor, float fontsize)
        {
            var res = Instantiate(Resources.Load<TMP_Text>(TEXTPREFAB), transform);
            res.rectTransform.anchorMin = res.rectTransform.anchorMax = anchor;
            res.fontSize = fontsize;
            return res;
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

        
        public void OnPointerEnter(PointerEventData eventData)
        {
            _descriptionTMP.gameObject.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _descriptionTMP.gameObject.SetActive(false);
        }
    }
}