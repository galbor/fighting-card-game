using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using DefaultNamespace.Managers;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerTurn : Singleton<PlayerTurn>
{
    [SerializeField] private TextCarrier _energyText;
    
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
    private List<Person.BodyPartEnum> _selectedAttackerBodyParts;
    
    private int _defaultEnergy;
    private int _energy;
    
    private List<BasicCard> _hand;

    private List<BasicCard> _deck;
    private Queue<BasicCard> _drawPile;
    private Queue<BasicCard> _discardPile;
    
    public Dictionary<Person.BodyPartEnum, KeyCode> _BodyPartKeyCodes;

    protected PlayerTurn()
    {
    }

    private void Awake()
    {
        // GetDeck();
        _hand = new List<BasicCard>();
        _drawPile = new Queue<BasicCard>();
        

        SetBodyPartKeyCodes();
    }
    
    public int Energy
    {
        get => _energy;
        set //used to be private
        {
            _energy = value;
            _energyText.Text = _energy.ToString();
            HandDisplayManager.Instance.SetEnergyCostColors();
        }
    }
    

    public void SetParameters(int defaultEnergy, int maxHandSize, int basicHandSize, Person playerPerson)
    {
        _defaultEnergy = defaultEnergy;
        _maxHandSize = maxHandSize;
        _basicHandSize = basicHandSize;
        _playerPerson = playerPerson;

        // StartRound();
    }

    public void StartRound()
    {
        DiscardHand();
        _drawPile = new Queue<BasicCard>();
        _discardPile = new Queue<BasicCard>();

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
        HandDisplayManager.Instance.DisplayHand();
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
        }

        StartCoroutine(SelectAttackerTypes());
    }

    IEnumerator SelectAttackerTypes()
    {
        BasicCard.AttackerTypeEnum[] attackerTypes = _hand[_selectedCard].AttackerType;
        _selectedAttackerBodyParts = new List<Person.BodyPartEnum>();

        int i = 0;
        while (_selectedAttackerBodyParts.Count < attackerTypes.Length)
        {
            if (BasicCard.GetBodyPart(attackerTypes[i]) == Person.BodyPartEnum.NONE)
            {
                _playerPerson.HighlightBodyParts(BasicCard.TargetTypeEnum.SIDE);
                while (true)
                {
                    var pressedSide = Person.GetSide(GetPressedBodyPart());
                    if (pressedSide == Person.SideEnum.RIGHT)
                    {
                        if (attackerTypes[i] == BasicCard.AttackerTypeEnum.ARM) _selectedAttackerBodyParts.Add(Person.BodyPartEnum.RIGHT_ARM);
                        else _selectedAttackerBodyParts.Add(Person.BodyPartEnum.RIGHT_LEG);
                        break;
                    }
                    if (pressedSide == Person.SideEnum.LEFT)
                    {
                        if (attackerTypes[i] == BasicCard.AttackerTypeEnum.ARM)
                            _selectedAttackerBodyParts.Add(Person.BodyPartEnum.LEFT_ARM);
                        else _selectedAttackerBodyParts.Add(Person.BodyPartEnum.LEFT_LEG);
                        break;
                    }
                    yield return null;
                }
            }
            else
            {
                _selectedAttackerBodyParts.Add(BasicCard.GetBodyPart(attackerTypes[i]));
            }
            
            i++;
        }

        
        _playerPerson.HighlightBodyParts(BasicCard.TargetTypeEnum.PRE_SELECTED);
        StartCoroutine(SelectAffectedPart());
    }

    IEnumerator SelectAffectedPart()
    {
        BasicAttackCard.TargetTypeEnum targetType = _hand[_selectedCard].TargetType;
        Person.BodyPartEnum selectedBodyPart = Person.BodyPartEnum.NONE;
        _enemies[_selectedEnemy].Person.HighlightBodyParts(targetType);
        if (targetType != BasicAttackCard.TargetTypeEnum.PRE_SELECTED)
        {
            while (true)
            {
                yield return null;
                selectedBodyPart = GetPressedBodyPart();
                if (selectedBodyPart == Person.BodyPartEnum.NONE) continue;
                if (targetType == BasicAttackCard.TargetTypeEnum.SIDE)
                {
                    if (Person.GetSide(selectedBodyPart) == Person.SideEnum.LEFT)
                    {
                        _selectedAffectedBodyPart = Person.BodyPartEnum.LEFT_ARM;
                    }
                    else
                    {
                        _selectedAffectedBodyPart = Person.BodyPartEnum.RIGHT_ARM;
                    }
                }
                else if (targetType == BasicAttackCard.TargetTypeEnum.UPPER_BODY)
                {
                    if (selectedBodyPart == Person.BodyPartEnum.RIGHT_LEG)
                        selectedBodyPart = Person.BodyPartEnum.RIGHT_ARM;
                    else if (selectedBodyPart == Person.BodyPartEnum.LEFT_LEG)
                        selectedBodyPart = Person.BodyPartEnum.LEFT_ARM;
                    _selectedAffectedBodyPart = selectedBodyPart;
                }
                else _selectedAffectedBodyPart = selectedBodyPart;

                break;
            }
        }
        else _selectedAffectedBodyPart = _hand[_selectedCard].PreSelectedTarget;
        PlayCard(_selectedCard);
        // StartCoroutine(SelectCard());
        ResetAction();
    }
    
    private bool PlayCard(int index)
    {
        if (index < 0 || index >= _hand.Count) return false;
        if (_hand[index].Cost > Energy)
            return false;
        Energy -= _hand[index].Cost;
        
        _hand[index].Play(Player.Instance.Person, _selectedAttackerBodyParts, _enemies[_selectedEnemy].Person, _selectedAffectedBodyPart);
        EventManager.Instance.TriggerEvent(EventManager.EVENT__PLAY_CARD, this);
        
        DiscardCard(index);
        
        RoomManager.Instance.CheckRoomWin();
        
        return true;
    }

    public void StartTurn()
    {
        Energy = _defaultEnergy;
        ShowEnergy(true);
        _playerPerson.SetProtectionDefault();
        
        EventManager.Instance.TriggerEvent(EventManager.EVENT__START_TURN, null);
        
        DrawHand();
        ResetAction();
    }
    
    public void EndTurn()
    {
        EventManager.Instance.TriggerEvent(EventManager.EVENT__END_TURN, null);
        if (RoomManager.Instance.CheckRoomWin()) return; //if All enemies are dead, doesn't start new turn
        
        DiscardHand();
        EnemiesAttack();
        StartTurn();
    }

    private void EnemiesAttack()
    {
        foreach (Enemy enemy in _enemies)
        {
            enemy.Person.SetProtectionDefault();
            enemy.Attack();
        }
    }

    public void StopAction()
    {
        StopAllCoroutines();
        
        _enemies.ToList().ForEach(enemy => enemy.Person.HighlightBodyParts(BasicAttackCard.TargetTypeEnum.PRE_SELECTED));
        _playerPerson.HighlightBodyParts(BasicCard.TargetTypeEnum.PRE_SELECTED);
    }
    
    private void ResetAction()
    {
        StopAction();
        
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

    private bool DiscardCard(int index)
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
    public void GetDeck()
    {
        var deck = Player.Instance.Deck;
        _deck = new List<BasicCard>();
        deck.ForEach(x => _deck.Add(x));
    }

    public void ShowEnergy(bool active)
    {
        _energyText.gameObject.SetActive(active);
    }
}
