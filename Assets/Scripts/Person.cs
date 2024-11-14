using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using DefaultNamespace.StatusEffects;
using DefaultNamespace.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Person : MonoBehaviour
{
    public enum BodyPartEnum
    {
        HEAD,
        TORSO,
        LEFT_ARM,
        RIGHT_ARM,
        LEFT_LEG,
        RIGHT_LEG,
        NONE
    }
    
    public enum SideEnum
    {
        LEFT,
        RIGHT,
        TORSO,
        HEAD,
        NONE
    }
    
    /**
     * @return The side of the body part
     */
    public static SideEnum GetSide(BodyPartEnum bodyPart)
    {
        switch (bodyPart)
        {
            case BodyPartEnum.HEAD:
                return SideEnum.HEAD;
            case BodyPartEnum.TORSO:
                return SideEnum.TORSO;
            case BodyPartEnum.LEFT_ARM:
            case BodyPartEnum.LEFT_LEG:
                return SideEnum.LEFT;
            case BodyPartEnum.RIGHT_ARM:
            case BodyPartEnum.RIGHT_LEG:
                return SideEnum.RIGHT;
            case BodyPartEnum.NONE:
                return SideEnum.NONE;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    private Dictionary<BodyPartEnum, double> HealthValues = new Dictionary<BodyPartEnum, double>
    {
        {BodyPartEnum.HEAD, 0.05},
        {BodyPartEnum.TORSO, 0.35},
        {BodyPartEnum.LEFT_ARM, 0.15},
        {BodyPartEnum.RIGHT_ARM, 0.15},
        {BodyPartEnum.LEFT_LEG, 0.15},
        {BodyPartEnum.RIGHT_LEG, 0.15}
    };
    
    private class BodyPart
    {
        public HealthBar _HealthBar;
        public TMP_Text _letterDisplay;
        public BodyPartEnum _BodyPartEnum;
        public Sprite _sprite;

        public BodyPart(BodyPartEnum part, HealthBar healthBar, int health, TMP_Text letterDisplay)
        {
            _BodyPartEnum = part;
            _HealthBar = healthBar;
            _HealthBar.MaxHealth = health;
            _letterDisplay = letterDisplay;
            _sprite = _letterDisplay.transform.parent.parent //a bodypart's grandchildren are the letterdisplay and the healthbar 
                .gameObject.GetComponent<Image>().sprite;
        }
    }

    [SerializeField] private Body _body;

    [SerializeField]
    private Dictionary<BodyPartEnum, BodyPartEnum> _defaultProtections = new Dictionary<BodyPartEnum, BodyPartEnum>() {
        { BodyPartEnum.LEFT_ARM , BodyPartEnum.HEAD},
        { BodyPartEnum.RIGHT_ARM ,BodyPartEnum.HEAD} };

    private Dictionary<BodyPartEnum, BodyPartEnum> _curProtections;
    private BodyPart[] _bodyParts;
    private int _maxHealth = 100;
    // private bool _initiated = false;
    public void Awake()
    {
        _bodyParts = new BodyPart[Enum.GetNames(typeof(BodyPartEnum)).Length];
        ForEachBodyPart(bodyPart =>
        {
            _bodyParts[(int)bodyPart] = new BodyPart(bodyPart,
                _body._healthBars[(int)bodyPart],
                (int)(HealthValues[bodyPart] * _maxHealth),
                _body._bodyPartTexts[(int)bodyPart]);
        });
        
        EventManager.Instance.StartListening(EventManager.EVENT__REMOVE_HEALTH,
            objAmtLost =>
            {
                int amtLost = (int)objAmtLost;
                CheckAlive();
            });
        
        SetProtectionDefault();
    }

    public void SetMaxHealth(int maxHealth)
    {
        _maxHealth = maxHealth;
        if (_bodyParts == null) return;
        ForEachBodyPart(bodyPart =>
        {
            _bodyParts[(int)bodyPart]._HealthBar.MaxHealth = (int)(HealthValues[bodyPart] * _maxHealth);
        });
    }
    
    public void HighlightBodyParts(BasicCard.TargetTypeEnum targetType)
    {
        var partsLst = Enum.GetValues(typeof(BodyPartEnum)).OfType<BodyPartEnum>()
                                    .Where(part => part != BodyPartEnum.NONE);
        Dictionary<BodyPartEnum, KeyCode> dict = PlayerTurn.Instance._BodyPartKeyCodes;
        Func<BodyPartEnum, String> func = (x => dict[x].ToString());

        switch (targetType)
        {
            case BasicCard.TargetTypeEnum.PRE_SELECTED:
                func = (x => "");
                break;
            case BasicCard.TargetTypeEnum.SIDE:
                partsLst = new List<BodyPartEnum> { BodyPartEnum.RIGHT_ARM, BodyPartEnum.LEFT_ARM };
                break;
            case BasicCard.TargetTypeEnum.UPPER_BODY:
                partsLst = partsLst.Where(part =>
                    part != BodyPartEnum.LEFT_LEG && part != BodyPartEnum.RIGHT_LEG);
                break;
            default: //case BODY_PART
                break;
        }

        partsLst.ToList().ForEach(part => _bodyParts[(int)part]._letterDisplay.text = func(part));
    }
    
    /**
     * deals damage to bodyPart, or to a bodyPart that protects it
     * returns the bodyPart that takes damage
     */
    public BodyPartEnum TakeDamage(BodyPartEnum bodyPart, int damage)
    {
        if (_curProtections.Values.Contains(bodyPart))
        {
            bodyPart = _curProtections.Where(pair => pair.Value == bodyPart).
                OrderByDescending(pair => GetHealthBar(pair.Key).Defense).First().Key;
        }

        //RemoveHealth returns true if health > 0
        if (!_bodyParts[(int)bodyPart]._HealthBar.RemoveHealth(damage))
        {
            RemoveProtection(bodyPart);
        }
        CheckAlive();

        return bodyPart;
    }
    
    public void Bleed(BodyPartEnum bodyPart, int amt)
    {
        if (amt == 0) return;
        _bodyParts[(int)bodyPart]._HealthBar.AddStatusEffect(BodyPartStatusEffect.StatusType.BLEED, amt);
    }
    
    public void Heal(BodyPartEnum bodyPart, int heal)
    {
        _bodyParts[(int)bodyPart]._HealthBar.AddHealth(heal);
    }
    
    public void Defend(BodyPartEnum bodyPart, int defense)
    {
        _bodyParts[(int)bodyPart]._HealthBar.AddDefense(defense);
    }


    public int GetAttackDamage(BodyPartEnum bodyPart, int base_damage)
    {
        return (int)Math.Ceiling((float)base_damage * _bodyParts[(int)bodyPart]._HealthBar.Health / _bodyParts[(int)bodyPart]._HealthBar.MaxHealth);
    }
    public int GetAttackBleed(BodyPartEnum bodyPart, int base_bleed)
    {
        return base_bleed;
    }

    public bool IsAlive()
    {
        return _bodyParts[(int)BodyPartEnum.HEAD]._HealthBar.IsAlive() && _bodyParts[(int)BodyPartEnum.TORSO]._HealthBar.IsAlive();
    }

    /**
     * if !IsAlive() dies
     */
    private void CheckAlive()
    {
        if (!IsAlive()) Die();
    }

    private void Die()
    {
        if (RoomManager.Instance.KillEnemy(this))
            EventManager.Instance.TriggerEvent(EventManager.EVENT__KILL_ENEMY, this);
        else 
            EventManager.Instance.TriggerEvent(EventManager.EVENT__PLAYER_DEATH, null);
        Destroy(gameObject); //perhaps undefined behavior for player death?
        // gameObject.SetActive(false);
    }
    
    //guard protects protectedPart
    public void SetProtection(BodyPartEnum guard, BodyPartEnum protectedPart)
    {
        _curProtections[guard] = protectedPart;
        DisplayProtection(guard, protectedPart);
    }

    public void RemoveProtection(BodyPartEnum guard)
    {
        _curProtections.Remove(guard);
        DisplayProtection(guard, BodyPartEnum.NONE);
    }

    public void SetProtectionDefault()
    {
        _curProtections = new Dictionary<BodyPartEnum, BodyPartEnum>(_defaultProtections);
        
        ForEachBodyPart(bodyPart =>
        {
            BodyPartEnum protectedPart;
            if (_curProtections.TryGetValue(bodyPart, out protectedPart))
            {
                if (_bodyParts[(int)bodyPart]._HealthBar.Health <= 0)
                {
                    RemoveProtection(bodyPart);
                    protectedPart = BodyPartEnum.NONE;
                }
            }
            else protectedPart =  BodyPartEnum.NONE;
            DisplayProtection(bodyPart, protectedPart);
        });
    }

    public void RemoveAllDefense()
    {
        ForEachBodyPart(bodyPart => _bodyParts[(int)bodyPart]._HealthBar.Defense = 0);
    }

    public void RemoveAllStatusEffects()
    {
        ForEachBodyPart(x =>
        {
            _bodyParts[(int)x]._HealthBar.RemoveAllStatusEffects();
        });
    } 

    private void DisplayProtection(BodyPartEnum guard, BodyPartEnum protectedPart)
    {
        if (guard == BodyPartEnum.NONE) return;
        
        Sprite image = protectedPart == BodyPartEnum.NONE ? null : _bodyParts[(int)protectedPart]._sprite;
        
        _bodyParts[(int)guard]._HealthBar.SetBlockImage(image);
    }
    
    public int MaxHealth => _maxHealth;

    /**
     * EnemyNumber is the number over the person
     */
    public void SetEnemyNumber(int number)
    {
        _body._enemyNumber.text = number.ToString();
    }

    /**
     * EnemyNumber is the number over the person
     */
    public void SetEnemyNumberActive(bool active)
    {
        _body._enemyNumber.gameObject.SetActive(active);
    }

    /**
     * Applies action to all bodyparts that aren't None
     */
    private void ForEachBodyPart(Action<BodyPartEnum> action)
    {
        foreach (BodyPartEnum bodyPart in BodyPartEnum.GetValues(typeof(BodyPartEnum)))
        {
            if (bodyPart == BodyPartEnum.NONE) continue;
            action(bodyPart);
        }
    }

    public HealthBar GetHealthBar(BodyPartEnum bodyPart)
    {
        return _bodyParts[(int)bodyPart]._HealthBar;
    }

    /**
     * displays the given sprites
     */
    public void DisplayPlannedAttack(BodyPartEnum attackingPart, BodyPartEnum affectedPart, int damage)
    {
        _body._plannedAttackDisplay.gameObject.SetActive(true);
        _body._plannedAttackDisplay.SetAttackingPart(GetBodyPartSprite(attackingPart));
        _body._plannedAttackDisplay.SetAffectedPart(Player.Instance.Person.GetBodyPartSprite(affectedPart));
        _body._plannedAttackDisplay.SetDamage(GetAttackDamage(attackingPart, damage));
    }

    public void HidePlannedAttack()
    {
        _body._plannedAttackDisplay.gameObject.SetActive(false);
    }

    private Sprite GetBodyPartSprite(BodyPartEnum bodyPart)
    {
        if (bodyPart == BodyPartEnum.NONE) return null;
        return GetHealthBar(bodyPart).transform.parent.parent.GetComponent<Image>().sprite; //the healthbar's parent's parent is the body part
    }
}
