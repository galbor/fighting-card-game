using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace.Utility;
using TMPro;
using UnityEngine;
using cards;

namespace Managers
{
    public class PlayerTurn : Singleton<PlayerTurn>
    {
        [SerializeField] private TMP_Text _energyText; //energyText's parent should be the image
        
        [SerializeField] private KeyCode _endTurnKey = KeyCode.Return;
        [SerializeField] private KeyCode _undo = KeyCode.Z;
        
        [SerializeField] private KeyCode _selectCard1 = KeyCode.Alpha1;
        [SerializeField] private KeyCode _selectCard2 = KeyCode.Alpha2;
        [SerializeField] private KeyCode _selectCard3 = KeyCode.Alpha3;
        [SerializeField] private KeyCode _selectCard4 = KeyCode.Alpha4;
        [SerializeField] private KeyCode _selectCard5 = KeyCode.Alpha5;
        [SerializeField] private KeyCode _selectCard6 = KeyCode.Alpha6;
        [SerializeField] private KeyCode _selectCard7 = KeyCode.Alpha7;
        [SerializeField] private KeyCode _selectCard8 = KeyCode.Alpha8;
        [SerializeField] private KeyCode _selectCard9 = KeyCode.Alpha9;
        [SerializeField] private KeyCode _selectCard10 = KeyCode.Alpha0;

        [SerializeField] private KeyCode _selectEnemy1 = KeyCode.Alpha1;
        [SerializeField] private KeyCode _selectEnemy2 = KeyCode.Alpha2;
        [SerializeField] private KeyCode _selectEnemy3 = KeyCode.Alpha3;
        [SerializeField] private KeyCode _selectEnemy4 = KeyCode.Alpha4;

        [SerializeField] private KeyCode _selectRightArm = KeyCode.E;
        [SerializeField] private KeyCode _selectLeftArm = KeyCode.Q;
        [SerializeField] private KeyCode _selectRightLeg = KeyCode.D;
        [SerializeField] private KeyCode _selectLeftLeg = KeyCode.A;
        [SerializeField] private KeyCode _selectHead = KeyCode.W;
        [SerializeField] private KeyCode _selectTorso = KeyCode.S;

        private int _maxHandSize = 10;
        private int _basicHandSize = 5;

        private Enemy[] _enemies;

        private Person _playerPerson;
        
        private int _selectedCard;
        private int _selectedEnemy;
        private Person.BodyPartEnum _selectedAffectedBodyPart;
        private List<Person.BodyPartEnum> _selectedBodyParts;
        
        private int _defaultEnergy;
        private int _energy = 10; // assuming there aren't cards with >10 energy
        
        private List<BasicCard> _hand;

        private List<BasicCard> _deck;
        private Queue<BasicCard> _drawPile;
        private Queue<BasicCard> _discardPile;
        private List<BasicCard> _exhaustPile;
        
        public List<BasicCard> DiscardPile { get => _discardPile.ToList(); }

        public List<BasicCard> DrawPile { get =>_drawPile.ToList(); }
        public List<BasicCard> ExhaustPile { get => _exhaustPile; }
        
        public Dictionary<Person.BodyPartEnum, KeyCode> _BodyPartKeyCodes;

        protected PlayerTurn() { }

        private void Awake()
        {
            // GetDeck();
            _hand = new List<BasicCard>();
            _drawPile = new Queue<BasicCard>();
            _exhaustPile = new List<BasicCard>();
            _enemies = Array.Empty<Enemy>();
            

            SetBodyPartKeyCodes();
            enabled = false;
        }
        
        public int Energy
        {
            get => _energy;
            set //used to be private
            {
                _energy = value;
                _energyText.text = _energy.ToString();
                HandDisplayManager.Instance.SetEnergyCostColors();
            }
        }
        

        public void SetParameters(int defaultEnergy, int maxHandSize, int basicHandSize, Person playerPerson)
        {
            _defaultEnergy = defaultEnergy;
            _maxHandSize = maxHandSize;
            _basicHandSize = basicHandSize;
            _playerPerson = playerPerson;
        }

        public void StartRound()
        {
            _hand = new List<BasicCard>();
            _drawPile = new Queue<BasicCard>();
            _discardPile = new Queue<BasicCard>();
            _exhaustPile = new List<BasicCard>();

            foreach (var card in _deck)
            {
                _discardPile.Enqueue(card);
            }
            ShuffleDiscardPile();
            
            _playerPerson.RemoveAllDefense();
            _playerPerson.RemoveAllStatusEffects();
            
            EventManager.Instance.TriggerEvent(EventManager.EVENT__START_COMBAT, null);

            GetEnemies();
            StartTurn();
        }

        // Update is called once per frame
        void Update()
        {
            //TODO turn ends too many times. do a check that turn isn't switching.
            if (Input.GetKeyDown(_endTurnKey)) EndTurn();

            if (Input.GetKeyDown(_undo))
            {
                 ResetAction();
            }
        }

        IEnumerator SelectCard()
        {
            int index = -1;
            while (true)
            {
                yield return null;
                if (Input.GetKeyDown(_selectCard1)) index = 0;
                else if (Input.GetKeyDown(_selectCard2)) index = 1;
                else if (Input.GetKeyDown(_selectCard3)) index = 2;
                else if (Input.GetKeyDown(_selectCard4)) index = 3;
                else if (Input.GetKeyDown(_selectCard5)) index = 4;
                else if (Input.GetKeyDown(_selectCard6)) index = 5;
                else if (Input.GetKeyDown(_selectCard7)) index = 6;
                else if (Input.GetKeyDown(_selectCard8)) index = 7;
                else if (Input.GetKeyDown(_selectCard9)) index = 8;
                else if (Input.GetKeyDown(_selectCard10)) index = 9;
                else index = -1;
                if (index != -1)
                {
                    if (index >= _hand.Count) continue;
                    if (_hand[index].Cost > Energy) continue;
                    _selectedCard = index;
                    break;
                }
            }
            HandDisplayManager.Instance.ChooseCard(index);

            StartCoroutine(SelectEnemy());
        }

        IEnumerator SelectEnemy()
        {
            int selectedEnemy = -1;
            _selectedEnemy = 0;
            if (_hand[_selectedCard].SingleEnemyTarget && _enemies.Length > 1)
            {
                ForEachEnemy(x => x.Person.SetEnemyNumberActive(true));
                while (true)
                {
                    yield return null;
                    if (Input.GetKeyDown(_selectEnemy1)) selectedEnemy = 0;
                    else if (Input.GetKeyDown(_selectEnemy2)) selectedEnemy = 1;
                    else if (Input.GetKeyDown(_selectEnemy3)) selectedEnemy = 2;
                    else if (Input.GetKeyDown(_selectEnemy4)) selectedEnemy = 3;
                    else selectedEnemy = -1;
                    if (selectedEnemy != -1)
                    {
                        if (selectedEnemy >= _enemies.Length) continue;
                        _selectedEnemy = selectedEnemy;
                        break;
                    }
                }
                ForEachEnemy(x => x.Person.SetEnemyNumberActive(false));
            }

            StartCoroutine(SelectBodyParts());
        }


        IEnumerator SelectBodyParts()
        {
            BasicCard.CardChoiceEnum[] cardChoices = _hand[_selectedCard].CardChoices;
            _selectedBodyParts = new List<Person.BodyPartEnum>();

            Person targetPerson = _hand[_selectedCard].ChoiceOnEnemy ? _enemies[_selectedEnemy].Person : _playerPerson;

            for (int i = 0; i < cardChoices.Length; i++)
            {
                Person.BodyPartEnum selectedBodyPart = BasicCard.GetBodyPart(cardChoices[i]);

                if (selectedBodyPart != Person.BodyPartEnum.NONE)
                {
                    _selectedBodyParts.Add(selectedBodyPart);
                    continue;
                }

                targetPerson.HighlightBodyParts(cardChoices[i]);
                while (true)
                {
                    yield return null;
                    selectedBodyPart = GetPressedBodyPart();
                    if (selectedBodyPart == Person.BodyPartEnum.NONE) continue;
                    if (cardChoices[i] == BasicCard.CardChoiceEnum.ARM)
                    {
                        if (Person.GetSide(selectedBodyPart) == Person.SideEnum.LEFT)
                            selectedBodyPart = Person.BodyPartEnum.LEFT_ARM;
                        else if (Person.GetSide(selectedBodyPart) == Person.SideEnum.RIGHT)
                            selectedBodyPart = Person.BodyPartEnum.RIGHT_ARM;
                    }
                    else if (cardChoices[i] == BasicCard.CardChoiceEnum.LEG)
                    {
                        if (Person.GetSide(selectedBodyPart) == Person.SideEnum.LEFT)
                            selectedBodyPart = Person.BodyPartEnum.LEFT_LEG;
                        else if (Person.GetSide(selectedBodyPart) == Person.SideEnum.RIGHT)
                            selectedBodyPart = Person.BodyPartEnum.RIGHT_LEG;
                    }
                    else if (cardChoices[i] == BasicCard.CardChoiceEnum.UPPER_BODY)
                    {
                        if (selectedBodyPart == Person.BodyPartEnum.RIGHT_LEG)
                            selectedBodyPart = Person.BodyPartEnum.RIGHT_ARM;
                        else if (selectedBodyPart == Person.BodyPartEnum.LEFT_LEG)
                            selectedBodyPart = Person.BodyPartEnum.LEFT_ARM;
                    }

                    break;
                }
                _selectedBodyParts.Add(selectedBodyPart);

            }

            PlayCard(_selectedCard);
            ResetAction();
            
            RoomManager.Instance.CheckRoomWin();
        }
        
        private bool PlayCard(int index)
        {
            if (index < 0 || index >= _hand.Count) return false;
            if (_hand[index].Cost > Energy)
                return false;
            Energy -= _hand[index].Cost;

            var enemy = _enemies[_selectedEnemy].Person;
            
            if (_hand[index].ChoiceOnEnemy)
                _hand[index].Play(_playerPerson, _hand[index].PreSelectedChoices.ToList(), enemy, _selectedBodyParts);
            else
                _hand[index].Play(_playerPerson, _selectedBodyParts, enemy, _hand[index].PreSelectedChoices.ToList());

            EventManager.Instance.TriggerEvent(EventManager.EVENT__PLAY_CARD, this);
            
            if (_hand[index].Exhaust)
                ExhaustCard(index);
            else
                DiscardCard(index);
            
            return true;
        }

        public void StartTurn()
        {
            Energy = _defaultEnergy;
            _playerPerson.SetProtectionDefault();
            
            EventManager.Instance.TriggerEvent(EventManager.EVENT__START_TURN, null);
            
            DrawHand();
            ResetAction();
        }
        
        public void EndTurn()
        {
            EventManager.Instance.TriggerEvent(EventManager.EVENT__END_TURN, null);
            if (RoomManager.Instance.CheckRoomWin()) return; //if All enemies are dead, doesn't start new turn

            /**
             * exhausts all ethereal cards in hand
             */
            _hand.ToList().Select((card, i) => new { Value = card, Index = i })
                .Where(pair => pair.Value.Ethereal)
                .ToList().ForEach(pair => ExhaustCardNoDisplay(pair.Index));
            
            DiscardHand();
            EnemiesAttack();
            if (RoomManager.Instance.CheckRoomWin()) return; //spike damage happens after EnemiesAttack()
            StartTurn();
        }

        private void EnemiesAttack()
        {
            ForEachEnemy(enemy =>
            {
                enemy.Person.SetProtectionDefault();
                enemy.Attack();
            });
        }

        public void StopAction()
        {
            StopAllCoroutines();
            
            ForEachEnemy(enemy =>
            {
                enemy.Person.SetEnemyNumberActive(false);
                enemy.Person.HighlightBodyParts(BasicCard.CardChoiceEnum.LEFT_ARM); //LEFT_ARM means no selection
            });
            _playerPerson.HighlightBodyParts(BasicCard.CardChoiceEnum.LEFT_ARM);
        }
        
        public void ResetAction()
        {
            StopAction();
            HandDisplayManager.Instance.DisplayHand();
            
            StartCoroutine(SelectCard());
        }
        
        public bool DrawCard()
        {
            if (!DrawCardNoDisplay()) return false;
            HandDisplayManager.Instance.SetHand(_hand);
            return true;
        }

        private bool DrawCardNoDisplay()
        {
            if (_hand.Count >= _maxHandSize) return false;
            BasicCard card = GetCardFromDeck();
            if (card == null) return false;
            _hand.Add(card);
            
            EventManager.Instance.TriggerEvent(EventManager.EVENT__DRAW_CARD, card);
            
            return true;
        }

        /**
         * @return The number of cards drawn
         */
        private int DrawHand()
        {
            int handSize;
            for (handSize = 0; handSize < _basicHandSize; handSize++)
            {
                if (!DrawCardNoDisplay()) break;
            }
            HandDisplayManager.Instance.SetHand(_hand);

            return handSize;
        }

        private BasicCard GetCardFromDeck()
        {
            if (_drawPile.Count == 0)
            {
                ShuffleDiscardPile();
            }
            if (_drawPile.Count == 0)
            {
                return null;
            }
            return _drawPile.Dequeue();
        }

        public bool DiscardCard(int index)
        {
            if (!DiscardCardNoDisplay(index)) return false;
            HandDisplayManager.Instance.SetHand(_hand);
            return true;
        }

        private bool DiscardCardNoDisplay(int index)
        {
            if (_hand[index] == null) return false;
            
            EventManager.Instance.TriggerEvent(EventManager.EVENT__DISCARD_CARD, _hand[index]);
            
            _discardPile.Enqueue(_hand[index]);
            _hand.RemoveAt(index);
            
            return true;
        }

        /**
         * exhausts card from hand
         */
        public void ExhaustCard(int index)
        {
            ExhaustCardNoDisplay(index);
            HandDisplayManager.Instance.SetHand(_hand);
        }
        
        private void ExhaustCardNoDisplay(int index)
        {
            EventManager.Instance.TriggerEvent(EventManager.EVENT__DISCARD_CARD, _hand[index]);
            
            _exhaustPile.Add(_hand[index]);
            _hand.RemoveAt(index);
        }

        /**
         * @return The number of cards discarded
         */
        private int DiscardHand()
        {
            int res = 0;
            for (int i = _hand.Count-1; i >=0; i--)
            {
                if (DiscardCardNoDisplay(i)) res++;
            }
            HandDisplayManager.Instance.SetHand(_hand);

            return res;
        }
        
        //shuffle discard pile into draw pile
        private void ShuffleDiscardPile()
        {
            BasicCard[] tmp_array = new BasicCard[_discardPile.Count];
            _discardPile.CopyTo(tmp_array, 0);
            _discardPile.Clear();
            MyUtils.ShuffleArrayInPlace(tmp_array);
            
            for (int i = 0; i<tmp_array.Length; i++)
            {
                _drawPile.Enqueue(tmp_array[i]);
            }
        }
        
        public Enemy[] GetEnemies()
        {
            return GetEnemies(RoomManager.Instance);
        }

        public Enemy[] GetEnemies(RoomManager roomManager)
        {
            return _enemies = roomManager.Enemies;
        }

        private Person.BodyPartEnum GetPressedBodyPart()
        {
            if (Input.GetKeyDown(_selectRightArm)) return Person.BodyPartEnum.RIGHT_ARM;
            if (Input.GetKeyDown(_selectLeftArm)) return Person.BodyPartEnum.LEFT_ARM;
            if (Input.GetKeyDown(_selectRightLeg)) return Person.BodyPartEnum.RIGHT_LEG;
            if (Input.GetKeyDown(_selectLeftLeg)) return Person.BodyPartEnum.LEFT_LEG;
            if (Input.GetKeyDown(_selectHead)) return Person.BodyPartEnum.HEAD;
            if (Input.GetKeyDown(_selectTorso)) return Person.BodyPartEnum.TORSO;
            return Person.BodyPartEnum.NONE;
        }

        private void SetBodyPartKeyCodes()
        {
            _BodyPartKeyCodes = new Dictionary<Person.BodyPartEnum, KeyCode>
            {
                {Person.BodyPartEnum.HEAD, _selectHead},
                {Person.BodyPartEnum.TORSO, _selectTorso},
                {Person.BodyPartEnum.LEFT_ARM, _selectLeftArm},
                {Person.BodyPartEnum.RIGHT_ARM, _selectRightArm},
                {Person.BodyPartEnum.LEFT_LEG, _selectLeftLeg},
                {Person.BodyPartEnum.RIGHT_LEG, _selectRightLeg}
            };
        }

        
        //Copies deck from Player
        public void ObtainDeck()
        {
            var deck = Player.Instance.Deck;
            _deck = new List<BasicCard>();
            deck.ForEach(x => _deck.Add(x));
        }

        /**
         * I write too much _enemies.ToList.ForEach(...)
         */
        private void ForEachEnemy(Action<Enemy> action)
        {
            foreach (var enemy in _enemies)
            {
                action(enemy);
            }
        }

        /**
         * like SetActive
         */
        public void ShowCombat(bool show)
        {
            SetActivePersons(show);
            _energyText.transform.parent.gameObject.SetActive(show);
            if (show) HandDisplayManager.Instance.DisplayHand();
            else HandDisplayManager.Instance.HideHand();
        }
        
        private void SetActivePersons(bool active)
        {
            _playerPerson.gameObject.SetActive(active);
            ForEachEnemy(x=>x.Person.gameObject.SetActive(active));
        }
    }
}