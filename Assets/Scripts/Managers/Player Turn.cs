using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerTurn : Singleton<PlayerTurn>
{
    [SerializeField] private Person _player;
    [SerializeField] private HandDisplayManager _handDisplayManager;
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
    
    private int _selectedCard;
    private int _selectedEnemy;
    private Person.BodyPartEnum _selectedAffectedBodyPart;
    private List<Person.BodyPartEnum> _selectedAttackerBodyParts;
    
    private int _defaultEnergy;
    private int _energy;
    
    private List<BasicCard> _hand;

    private Queue<BasicCard> _drawPile;
    private Queue<BasicCard> _discardPile;

    public int Energy
    {
        get => _energy;
        private set
        {
            _energy = value;
            _energyText.Text = _energy.ToString();
            _handDisplayManager.SetEnergyCostColors();
        }
    }
    
    public Person Player {get => _player; }

    public void Init(int defaultEnergy, int maxHandSize, int basicHandSize, List<BasicCard> deck)
    {
        _defaultEnergy = defaultEnergy;
        _maxHandSize = maxHandSize;
        _basicHandSize = basicHandSize;
        _hand = new List<BasicCard>();
        _drawPile = new Queue<BasicCard>();
        _discardPile = new Queue<BasicCard>();
        
        foreach (var Card in deck)
        {
            _discardPile.Enqueue(Card);
        }
        ShuffleDiscardPile();

        SetBodyPartKeyCodes();
        
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

    //TODO display buttons
    IEnumerator SelectCard()
    {
        _handDisplayManager.DisplayHand();
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
        _handDisplayManager.ChooseCard(index);

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
                Player.HighlightBodyParts(BasicCard.TargetTypeEnum.SIDE);
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

        
        Player.HighlightBodyParts(BasicCard.TargetTypeEnum.PRE_SELECTED);
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
        _hand[index].Play(_player, _selectedAttackerBodyParts, _enemies[_selectedEnemy].Person, _selectedAffectedBodyPart);
        DiscardCard(index);
        return true;
    }

    public void StartTurn()
    {
        Energy = _defaultEnergy;
        Player.SetProtectionDefault();
        DrawHand();
        ResetAction();
    }
    
    public void EndTurn()
    {
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

    private void ResetAction()
    {
        StopAllCoroutines();
        
        _enemies.ToList().ForEach(enemy => enemy.Person.HighlightBodyParts(BasicAttackCard.TargetTypeEnum.PRE_SELECTED));
        Player.HighlightBodyParts(BasicCard.TargetTypeEnum.PRE_SELECTED);
        
        StartCoroutine(SelectCard());
    }
    
    public bool DrawCard()
    {
        if (!DrawCardNoDisplay()) return false;
        _handDisplayManager.SetHand(_hand);
        return true;
    }

    private bool DrawCardNoDisplay()
    {
        if (_hand.Count >= _maxHandSize) return false;
        BasicCard card = GetCardFromDeck();
        if (card == null) return false;
        _hand.Add(card);
        return true;
    }

    /**
     * @return The number of cards drawn
     */
    private int DrawHand()
    {
        for (int i = 0; i < _basicHandSize; i++)
        {
            if (!DrawCardNoDisplay()) return i;
        }
        _handDisplayManager.SetHand(_hand);

        return _basicHandSize;
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
        _handDisplayManager.SetHand(_hand);
        return true;
    }

    private bool DiscardCardNoDisplay(int index)
    {
        if (_hand[index] == null) return false;
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
        _handDisplayManager.SetHand(_hand);

        return res;
    }
    
    //shuffle discard pile into draw pile
    private void ShuffleDiscardPile()
    {
        BasicCard[] tmp_array = new BasicCard[_discardPile.Count];
        _discardPile.CopyTo(tmp_array, 0);
        _discardPile.Clear();
        //Fisher-Yates shuffle
        for (int i = 0; i < tmp_array.Length-1; i++)
        {
            int j = Random.Range(i, tmp_array.Length-1);
            (tmp_array[i], tmp_array[j]) = (tmp_array[j], tmp_array[i]);
        }
        
        for (int i = 0; i<tmp_array.Length; i++)
        {
            _drawPile.Enqueue(tmp_array[i]);
        }
    }

    //TODO get enemies from somewhere
    private Enemy[] GetEnemies()
    {
        return _enemies = RoomManager.Instance.Enemies;
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
        EventManagerScript.Instance._BodyPartKeyCodes ??= new Dictionary<Person.BodyPartEnum, KeyCode>(); //if null then equals
        
        Dictionary<Person.BodyPartEnum, KeyCode> dict = EventManagerScript.Instance._BodyPartKeyCodes;
        dict[Person.BodyPartEnum.HEAD] = _selectHead;
        dict[Person.BodyPartEnum.TORSO] = _selectTorso;
        dict[Person.BodyPartEnum.LEFT_ARM] = _selectLeftArm;
        dict[Person.BodyPartEnum.RIGHT_ARM] = _selectRightArm;
        dict[Person.BodyPartEnum.LEFT_LEG] = _selectLeftLeg;
        dict[Person.BodyPartEnum.RIGHT_LEG] = _selectRightLeg;
    }
}
