using System;
using System.Collections.Generic;
using System.Linq;
using Managers;
using DefaultNamespace.StatusEffects;
using DefaultNamespace.UI;
using cards;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.VisualScripting;

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
        public HealthBar _healthBar;
        public TMP_Text _letterDisplay;
        public BodyPartEnum _BodyPartEnum;
        public Sprite _sprite;

        public BodyPart(BodyPartEnum part, HealthBar healthBar, int health, TMP_Text letterDisplay)
        {
            _BodyPartEnum = part;
            _healthBar = healthBar;
            _healthBar.MaxHealth = health;
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
            _bodyParts[(int)bodyPart]._healthBar.MaxHealth = (int)(HealthValues[bodyPart] * _maxHealth);
        });
    }
    
    public void HighlightBodyParts(BasicCard.CardChoiceEnum targetType)
    {
        var partsLst = Enum.GetValues(typeof(BodyPartEnum)).OfType<BodyPartEnum>()
                                    .Where(part => part != BodyPartEnum.NONE);
        Dictionary<BodyPartEnum, KeyCode> dict = PlayerTurn.Instance._BodyPartKeyCodes;
        Func<BodyPartEnum, String> func = (x => dict[x].ToString());

        switch (targetType)
        {
            case BasicCard.CardChoiceEnum.BODY_PART:
                break;
            case BasicCard.CardChoiceEnum.LEG:
            case BasicCard.CardChoiceEnum.ARM:
                partsLst = new List<BodyPartEnum> { BodyPartEnum.RIGHT_ARM, BodyPartEnum.LEFT_ARM };
                break;
            case BasicCard.CardChoiceEnum.UPPER_BODY:
                partsLst = partsLst.Where(part =>
                    part != BodyPartEnum.LEFT_LEG && part != BodyPartEnum.RIGHT_LEG);
                break;
            default: //case pre selected
                func = (x => "");
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
            var protectingParts = _curProtections.Where(pair => pair.Value == bodyPart).Select(pair => pair.Key).ToList();
            var maxInvincibility = protectingParts.Max(bodypart => GetHealthBar(bodypart).Invincibility);
            bodyPart = protectingParts.
                Where(bodypart => GetHealthBar(bodypart).Invincibility == maxInvincibility).
                OrderByDescending(bodypart => GetHealthBar(bodypart).Defense).First();
        }

        //TakeHitDamage() returns true if health > 0
        if (!GetHealthBar(bodyPart).TakeHitDamage(damage))
        {
            RemoveProtection(bodyPart);
        }
        CheckAlive();

        return bodyPart;
    }
    
    public void Bleed(BodyPartEnum bodyPart, int amt)
    {
        if (amt == 0) return;
        _bodyParts[(int)bodyPart]._healthBar.AddStatusEffect(typeof(BleedStatusEffect), amt);
    }
    
    public void Heal(BodyPartEnum bodyPart, int heal)
    {
        _bodyParts[(int)bodyPart]._healthBar.AddHealth(heal);
    }

    /**
     * heals person and spreads it between body parts
     * algorithm: first come first serve
     */
    public void Heal(int heal)
    {
        ForEachBodyPart(bodyPartEnum =>
        {
            var bodyPart = _bodyParts[(int)bodyPartEnum];
            int amt = Math.Min(bodyPart._healthBar.MaxHealth - bodyPart._healthBar.Health, heal);
            heal -= amt;
            bodyPart._healthBar.AddHealth(amt);
            if (heal <= 0) return;
        });
    }

    /**
     * heals person and gives each body part 'heal' amount
     */
    public void HealAll(int heal)
    {
        ForEachBodyPart(bodyPartEnum =>
        {
            var bodyPart = _bodyParts[(int)bodyPartEnum];
            bodyPart._healthBar.AddHealth(heal);
        });
    }
    
    public void Defend(BodyPartEnum bodyPart, int defense)
    {
        _bodyParts[(int)bodyPart]._healthBar.AddDefense(defense);
    }

    /**
     * calculates how much damage an attack from the given body part with the given base_damage would deal
     * rounds up
     * @return the damage
     */
    public int GetAttackDamage(BodyPartEnum bodyPart, int base_damage)
    {
        var healthBar = _bodyParts[(int)bodyPart]._healthBar;
        base_damage += healthBar.HitDamageDealtAddition;
        return (int)Math.Ceiling((float)base_damage * healthBar.HitDamageDealtMultiplier * healthBar.Health / healthBar.MaxHealth);
    }
    public int GetAttackBleed(BodyPartEnum bodyPart, int base_bleed)
    {
        return base_bleed;
    }

    public bool IsAlive()
    {
        return _bodyParts[(int)BodyPartEnum.HEAD]._healthBar.IsAlive() && _bodyParts[(int)BodyPartEnum.TORSO]._healthBar.IsAlive();
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
        RemoveAllStatusEffects();
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
                if (_bodyParts[(int)bodyPart]._healthBar.Health <= 0)
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
        ForEachBodyPart(bodyPart => _bodyParts[(int)bodyPart]._healthBar.Defense = 0);
    }

    public void RemoveAllStatusEffects()
    {
        ForEachBodyPart(x =>
        {
            _bodyParts[(int)x]._healthBar.RemoveAllStatusEffects();
        });
    } 

    private void DisplayProtection(BodyPartEnum guard, BodyPartEnum protectedPart)
    {
        if (guard == BodyPartEnum.NONE) return;
        
        Sprite image = protectedPart == BodyPartEnum.NONE ? null : _bodyParts[(int)protectedPart]._sprite;
        
        _bodyParts[(int)guard]._healthBar.SetBlockImage(image);
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
        return _bodyParts[(int)bodyPart]._healthBar;
    }

    /**
     * displays the given sprites
     */
    public void DisplayPlannedAttack(BodyPartEnum attackingPart, BodyPartEnum affectedPart, int damage)
    {
        _body._plannedAttackDisplay.gameObject.SetActive(true);
        _body._plannedAttackDisplay.SetAttackingPart(GetBodyPartSprite(attackingPart));
        _body._plannedAttackDisplay.SetAffectedPart(Player.Instance.Person.GetBodyPartSprite(affectedPart));

        var plannedAttackDamage = GetAttackDamage(attackingPart, damage);
        var hitDamage = Player.Instance.Person.GetHealthBar(affectedPart).GetHitDamage(plannedAttackDamage);
        _body._plannedAttackDisplay.SetDamage(hitDamage);
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
