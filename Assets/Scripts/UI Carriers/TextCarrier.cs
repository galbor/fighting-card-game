using System;
using TMPro;
using UnityEngine;

namespace DefaultNamespace
{
    public class TextCarrier : Carrier
    {
        private const string TEXTPREFAB = "UI/TextPrefab";
        
        [SerializeField] private String _text;
        [SerializeField] private int _fontSize = 0;
        private TMP_Text _textMeshPro;
        private Transform _parent;
        new void Awake()
        {
            _parent = EventManager.Instance._TextParent;
            if (_carriedSubject == null)
            {
                GameObject textPrefab = Instantiate(Resources.Load<GameObject>(TEXTPREFAB), _parent);
                _carriedSubject = textPrefab.GetComponent<RectTransform>();
            }
            _textMeshPro = _carriedSubject.GetComponent<TMP_Text>();
            Text = _text;
            if (_fontSize != 0)
            {
                _textMeshPro.fontSize = _fontSize;
            }

            base.Awake();
        }
        
        

        public string Text
        {
            get => _textMeshPro.text;
            set => _textMeshPro.text = value;
        }
        
        public float FontSize
        {
            get => _textMeshPro.fontSize;
            set => _textMeshPro.fontSize = value;
        }

        public Color Color
        {
            get => _textMeshPro.color;
            set => _textMeshPro.color = value;
        }
        
        public void SetAligntment(TextAlignmentOptions alignment)
        {
            _carriedSubject.sizeDelta = new Vector2(_carriedSubject.sizeDelta.x, 0);
            _textMeshPro.alignment = alignment;
        }

        public void SetWidth(float width)
        {
            _carriedSubject.sizeDelta = new Vector2(width, _carriedSubject.sizeDelta.y);
        }

        protected override void ChangeScale(float mult)
        {
            // base.ChangeScale(mult);
            FontSize *= mult;
        }
    }
}