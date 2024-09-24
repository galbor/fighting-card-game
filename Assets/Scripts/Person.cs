using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Assertions;

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
        public TextCarrier _letterDisplay;
        public BodyPartEnum _BodyPartEnum;
        public Sprite _sprite;

        public BodyPart(BodyPartEnum part, HealthBar healthBar, int health, TextCarrier letterDisplay)
        {
            _BodyPartEnum = part;
            _HealthBar = healthBar;
            _HealthBar.SetMaxHealth(health);
            _letterDisplay = letterDisplay;
            _sprite = _letterDisplay.gameObject.GetComponent<SpriteRenderer>().sprite;
        }
    }

    [SerializeField] private GameObject _healthBarPrefab;
    private Transform _healthBarParent;
    [SerializeField] private Carrier[] _healthBarCarriers;
    [SerializeField] private TextCarrier[] _bodyPartTextCarriers;

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
        _healthBarParent = EventManagerScript.Instance._HealthBarParent;
        _bodyParts = new BodyPart[Enum.GetNames(typeof(BodyPartEnum)).Length];
        foreach (BodyPartEnum bodyPart in Enum.GetValues(typeof(BodyPartEnum)))
        {
            if (bodyPart == BodyPartEnum.NONE) continue;
            GameObject healthBar = Instantiate(_healthBarPrefab, _healthBarParent);
            HealthBar healthBarScript = healthBar.GetComponent<HealthBar>();
            _healthBarCarriers[(int)bodyPart].SetDisplay(healthBar.GetComponent<RectTransform>());
            _bodyParts[(int)bodyPart] = new BodyPart(bodyPart, healthBarScript,
                (int)(HealthValues[bodyPart] * _maxHealth), _bodyPartTextCarriers[(int)bodyPart]);
        }
        
        SetProtectionDefault();
    }

    public void SetMaxHealth(int maxHealth)
    {
        _maxHealth = maxHealth;
        if (_bodyParts == null) return;
        foreach (BodyPartEnum bodyPart in Enum.GetValues(typeof(BodyPartEnum)))
        {
            if (bodyPart == BodyPartEnum.NONE) continue;
            _bodyParts[(int)bodyPart]._HealthBar.SetMaxHealth((int)(HealthValues[bodyPart] * _maxHealth));
        }
    }
    
    public void HighlightBodyParts(BasicAttackCard.TargetTypeEnum targetType)
    {
        if (targetType == BasicAttackCard.TargetTypeEnum.PRE_SELECTED)
        {
            foreach (BodyPartEnum bodyPartEnum in Enum.GetValues(typeof(BodyPartEnum)))
            {
                if (bodyPartEnum == BodyPartEnum.NONE) continue;
                _bodyParts[(int)bodyPartEnum]._letterDisplay.Text = "";
            }

            return;
        }

        Dictionary<Person.BodyPartEnum, KeyCode> dict = EventManagerScript.Instance._BodyPartKeyCodes;

        if (targetType == BasicAttackCard.TargetTypeEnum.BODY_PART)
        {
            foreach (BodyPartEnum bodyPartEnum in Enum.GetValues(typeof(BodyPartEnum)))
            {
                if (bodyPartEnum == BodyPartEnum.NONE) continue;
                _bodyParts[(int)bodyPartEnum]._letterDisplay.Text = dict[bodyPartEnum].ToString();
            }

            return;
        }
        
        if (targetType == BasicAttackCard.TargetTypeEnum.UPPER_BODY)
        {
            foreach (BodyPartEnum bodyPartEnum in Enum.GetValues(typeof(BodyPartEnum)))
            {
                if (bodyPartEnum == BodyPartEnum.NONE || bodyPartEnum == BodyPartEnum.RIGHT_LEG || bodyPartEnum == BodyPartEnum.LEFT_LEG) continue;
                _bodyParts[(int)bodyPartEnum]._letterDisplay.Text = dict[bodyPartEnum].ToString();
            }

            return;
        }
        
        // targetType == BasicCard.TargetTypeEnum.SIDE
        _bodyParts[(int)BodyPartEnum.LEFT_ARM]._letterDisplay.Text = dict[BodyPartEnum.LEFT_ARM].ToString();
        _bodyParts[(int)BodyPartEnum.RIGHT_ARM]._letterDisplay.Text = dict[BodyPartEnum.RIGHT_ARM].ToString();
        return;
    }
    
    public void TakeDamage(BodyPartEnum bodyPart, int damage)
    {
        if (_curProtections.Values.Contains(bodyPart))
        {
            bodyPart = _curProtections.First(pair => pair.Value == bodyPart).Key;
        }

        //RemoveHealth returns true if health > 0
        if (!_bodyParts[(int)bodyPart]._HealthBar.RemoveHealth(damage))
        {
            RemoveProtection(bodyPart);
        }
        if (!IsAlive()) Die();
    }
    
    public void Bleed(BodyPartEnum bodyPart, int bleed)
    {
        _bodyParts[(int)bodyPart]._HealthBar.AddBleed(bleed);
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
        return _bodyParts[(int)BodyPartEnum.HEAD]._HealthBar.Health > 0;
    }

    private void Die()
    {
        //TODO
        return;
    }
    
    //
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
        
        BodyPartEnum protectedPart;
        foreach (BodyPartEnum bodypart in BodyPartEnum.GetValues(typeof(BodyPartEnum)))
        {
            if (_curProtections.ContainsKey(bodypart))
            {
                protectedPart = _curProtections[bodypart];
                if (_bodyParts[(int)bodypart]._HealthBar.Health <= 0)
                {
                    RemoveProtection(bodypart);
                    protectedPart = BodyPartEnum.NONE;
                }
            }
            else protectedPart =  BodyPartEnum.NONE;
            DisplayProtection(bodypart, protectedPart);
        }
    }

    private void DisplayProtection(BodyPartEnum guard, BodyPartEnum protectedPart)
    {
        if (guard == BodyPartEnum.NONE) return;
        
        Sprite image = protectedPart == BodyPartEnum.NONE ? null : _bodyParts[(int)protectedPart]._sprite;
        
        _bodyParts[(int)guard]._HealthBar.SetBlockImage(image);
    }
    
    public int MaxHealth => _maxHealth;
}
