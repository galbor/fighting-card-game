using System;
using System.Collections.Generic;
using Managers;
using System.Text;
using UnityEngine;

namespace cards
{
    [CreateAssetMenu(menuName = "Cards/Basic Card")]
    public class BasicCard : ScriptableObject
    {
        [SerializeField] private string _name = "";
        [SerializeField] private string _description = "";
        [SerializeField] Sprite _image;
        [SerializeField] int _cost = 1;
        [SerializeField] int _draw = 0;

        [SerializeField] protected bool _singleEnemyTarget = true;
        [SerializeField] protected CardChoiceEnum[] _choiceArr = Array.Empty<CardChoiceEnum>();
        [SerializeField] protected Person.BodyPartEnum[] _preSelectedArr = Array.Empty<Person.BodyPartEnum>();
        protected bool _choiceOnEnemy = true;

        [SerializeField] private bool _exhaust = false;
        [SerializeField] private bool _ethereal = false;

        [SerializeField] private List<BasicCard> _cardsToPlay;
        
        public bool Exhaust { get => _exhaust; }
        public bool Ethereal { get => _ethereal; }

        public CardChoiceEnum[] CardChoices
        {
            get => _choiceArr;
            protected set => _choiceArr = value;
        }

        public Person.BodyPartEnum[] PreSelectedChoices
        {
            get => _preSelectedArr;
            protected set => _preSelectedArr = value;
        }

        public bool SingleEnemyTarget { get => _singleEnemyTarget; }
        public bool ChoiceOnEnemy { get => _choiceOnEnemy; }

        //choice made with the card
        public enum CardChoiceEnum
        {
            ARM,
            RIGHT_ARM,
            LEFT_ARM,
            LEG,
            RIGHT_LEG,
            LEFT_LEG,
            HEAD,
            TORSO,
            UPPER_BODY,
            BODY_PART
        }

        public static Person.BodyPartEnum GetBodyPart(CardChoiceEnum attackerType)
        {
            switch (attackerType)
            {
                case CardChoiceEnum.RIGHT_ARM:
                    return Person.BodyPartEnum.RIGHT_ARM;
                case CardChoiceEnum.LEFT_ARM:
                    return Person.BodyPartEnum.LEFT_ARM;
                case CardChoiceEnum.RIGHT_LEG:
                    return Person.BodyPartEnum.RIGHT_LEG;
                case CardChoiceEnum.LEFT_LEG:
                    return Person.BodyPartEnum.LEFT_LEG;
                case CardChoiceEnum.HEAD:
                    return Person.BodyPartEnum.HEAD;
                case CardChoiceEnum.TORSO:
                    return Person.BodyPartEnum.TORSO;
                case CardChoiceEnum.LEG:
                    return Person.BodyPartEnum.NONE;
                case CardChoiceEnum.ARM:
                    return Person.BodyPartEnum.NONE;
                case CardChoiceEnum.UPPER_BODY:
                    return Person.BodyPartEnum.NONE;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected void Awake()
        {
            _cardsToPlay ??= new List<BasicCard>();
        }

        protected string _displayDescription;

        public string DisplayDescription
        {
            get
            {
                UpdateDescription();
                return _displayDescription;
            }
        }

        public Sprite Image
        {
            get => _image;
        }

        public string Name
        {
            get => _name;
        }

        public int Cost
        {
            get => _cost;
        }

        public virtual void Play(Person user, List<Person.BodyPartEnum> attacking_parts, Person target,
            List<Person.BodyPartEnum> affected_parts)
        {
            PlayerTurn playerTurn = PlayerTurn.Instance;
            for (int i = 0; i < _draw; i++)
            {
                if (!playerTurn.DrawCard()) break;
            }
            PlayExtraCards(user, attacking_parts, target, affected_parts);
        }

        /**
         * plays all cards in the _cardsToPlay list
         */
        private void PlayExtraCards(Person user, List<Person.BodyPartEnum> attacking_parts, Person target,
            List<Person.BodyPartEnum> affected_parts)
        {
            _cardsToPlay.ForEach(card => card.Play(user, attacking_parts, target, affected_parts));
        }

        /**
         * either generates description or takes the description from the _description field if it's filled
         */
        protected virtual string GetThisDescription()
        {
            if (_description == "") return GenerateThisDescription();
            return _description.Replace("\\n", "\n");
        }

        /**
         * creates a description from known parameters of the card and its class
         */
        protected virtual string GenerateThisDescription()
        {
            StringBuilder res = new StringBuilder();
            if (Exhaust) res.Append("Exhaust.\n");
            if (Ethereal) res.Append("Ethereal.\n");
            if (_draw > 0) res.AppendFormat("Draw {0} cards.\n", _draw);

            return res.ToString();
        }

        protected virtual void UpdateDescription()
        {
            // _displayDescription = _description.Replace("\\n", "\n");
            _displayDescription = GenerateDescription();
        }

        /**
         * generates description from this card's description and the cards it plays' descriptions2
         */
        private String GenerateDescription()
        {
            var res = new StringBuilder(GetThisDescription());
            foreach (var card in _cardsToPlay)
            {
                res.Append(card.GetThisDescription());
            }

            return res.ToString();
        }
    }
}