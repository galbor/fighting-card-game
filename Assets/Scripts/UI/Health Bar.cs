using System;
using System.Collections.Generic;
using System.Reflection;
using DefaultNamespace.StatusEffects;
using Managers;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace DefaultNamespace.UI
{
    [RequireComponent(typeof(HealthBarUI))]
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private HealthBarUI _UIComponent;

        private List<BodyPartStatusEffect> _statusEffects;

        private int _maxHealth = 50;
        private int _currentHealth = 50;
        private int _defense = 0;
        private int _invincibility = 0;

        public int Defense
        {
            get => _defense;
            set => SetDefense(value);
        }

        /**
         * multiply by this when taking hit damage
         * damage rounded using Math.Round()
         */
        public float HitDamageTakenMultiplier {get; set;} = 1f;

        /**
         * multiply by this when dealing damage
         */
        public float HitDamageDealtMultiplier {get; set;} = 1f;

        /**
         * added to hit this health bar deals
         * goes before multiplying
         */
        public int HitDamageDealtAddition {get; set;} = 0;

        public int Health
        {
            get => _currentHealth;
            set => SetHealth(value);
        }

        public int MaxHealth
        {
            get => _maxHealth;
            set => SetMaxHealth(value);
        }

        public int Invincibility
        {
            get => _invincibility;
            set => _invincibility = Math.Max(value, 0);
        }

        void Awake()
        {
            _statusEffects = new List<BodyPartStatusEffect>();

            _UIComponent.Init(MaxHealth, _statusEffects);
            SetHealth(_maxHealth);
            _UIComponent.Health = Health;
        }

        private void OnDestroy()
        {
            RemoveAllStatusEffects();
        }

        public void SetBlockImage(Sprite sprite)
        {
            _UIComponent.SetBlockImage(sprite);
        }

        /**
         * Set the max health
         * the amount of max health added/removed will be added/removed to the current health
         * if removes too much health, sets health to 1
         * @param health The new max health value
         */
        public void SetMaxHealth(int maxHealth)
        {
            int change = maxHealth - _maxHealth;
            _maxHealth = maxHealth;

            _UIComponent.MaxHealth = maxHealth;

            if (!AddHealth(change)) //if dead
                SetHealth(1);
        }

        

        /**
            * Set the health
            * @param health The new health value
            * @return True if the object is still alive, false otherwise
            */
        public bool SetHealth(int health)
        {
            health = Mathf.Clamp(health, 0, _maxHealth);
            int healthChange = health - _currentHealth;
            _currentHealth = health;

            _UIComponent.Health = health;

            if (healthChange < 0) EventManager.Instance.TriggerEvent(EventManager.EVENT__REMOVE_HEALTH, -healthChange);
            if (!IsAlive()) RemoveAllStatusEffects();

            return IsAlive();
        }

        private void SetDefense(int defense)
        {
            _defense = defense;
            _UIComponent.SetDefense(defense);
        }

        /**
         *
         * @return The new damage value
         */
        public int ReduceDefense(int damage)
        {
            if (_defense == 0) return damage;

            int difference = damage - _defense;
            damage = Math.Max(difference, 0);
            SetDefense(Math.Max(-difference, 0));
            return damage;
        }


        public void AddDefense(int amt)
        {
            Defense += amt;
        }

        /**
        * Add health to the object
        * @param health The health to add
        * @return True if the object is still alive, false otherwise
        */
        public bool AddHealth(int health)
        {
            return SetHealth(_currentHealth + health);
        }

        /**
         * Remove health from the object
         * @param health The health to remove
         * @return True iff the object is still alive
         */
        public bool RemoveHealth(int damage)
        {
            if (Invincibility > 0)
            {
                Invincibility -= 1;
                return true;
            }

            damage = ReduceDefense(damage);
            return SetHealth(_currentHealth - damage);
        }

        /**
         * Same as RemoveHealth(int) but times HitDamageMultiplier
         * @return true iff the object is still alive
         */
        public bool TakeHitDamage(int damage)
        {
            return RemoveHealth(GetHitDamage(damage));
        }

        /**
         * @return how much damage a hit would do
         */
        public int GetHitDamage(int attackDamage)
        {
            return (int)Math.Round(attackDamage * HitDamageTakenMultiplier);
        }

        public bool IsAlive()
        {
            return Health > 0;
        }

        /**
         * adds a status effect
         * if already exists, adds amt to the existing one
         * if the sum with amt is 0, removes the status effect
         * @return amount at the end (and 0 if unsuccessful)
         */
        public int AddStatusEffect(Type TStatus, int amt,
            bool affectsDeadBodyParts = false)
        {
            if (!IsStatusEffect(TStatus) || (!affectsDeadBodyParts && !IsAlive())) return 0;
            BodyPartStatusEffect status = _statusEffects.Find(x => x.GetStatusType() == TStatus);
            int num = 0;
            if (status != null)
            {
                num = status.Number;
            }

            return SetStatusEffect(TStatus, num + amt, affectsDeadBodyParts);
        }

        /**
         * sets a status effect's amt
         * if type doesn't exist, creates it
         * if 0, removes it
         * @return amount at the end (and 0 if unsuccessful)
         */
        public int SetStatusEffect(Type TStatus, int amt,
            bool affectsDeadBodyParts = false)
        {
            if (!IsStatusEffect(TStatus) || (!affectsDeadBodyParts && !IsAlive())) return 0;
            BodyPartStatusEffect status = _statusEffects.Find(x => x.GetStatusType() == TStatus);
            if (status != null)
            {
                status.Number = amt; //if amt == 0, removes self
                return amt;
            }

            status = BodyPartStatusEffect.GetPool(TStatus).GetFromPool();
            status.BodyPart = this;
            _UIComponent.SetNewStatusPosition(status);
            _statusEffects.Add(status);
            status.Number = amt;
            status.OnFirstAdded();
            return amt;
        }



        /**
         * @return amount status had before
         */
        public int RemoveStatusEffect<TStatus>() where TStatus : BodyPartStatusEffect
        {
            BodyPartStatusEffect status = _statusEffects.Find(x => x.GetStatusType() == typeof(TStatus));
            if (status == null) return 0;
            return RemoveStatusEffect(status);
        }

        /**
         * removes status effect
         */
        public int RemoveStatusEffect(BodyPartStatusEffect status)
        {
            int prevNum = status.Number;
            if (status.Number != 0)
            {
                //[Number = 0] calls this function
                //It's vital for the number to become 0 before being removed (because of effects like Invincible, vulnerable)
                status.Number = 0;
                return prevNum;
            }

            int index = _statusEffects.IndexOf(status);
            _statusEffects.RemoveAt(index);
            BodyPartStatusEffect.GetPool(status).ReturnToPool(status);
            _UIComponent.RemoveStatusEffect(index, status);
            return 0;
            
        }

        

        /*
         * return amount removed
         */
        public int RemoveAllStatusEffects()
        {
            int sum = 0;
            while (_statusEffects.Count > 0)
            {
                sum += _statusEffects[^1].Number;
                _statusEffects[^1].Number = 0; //[Number = 0] removes self
            }
            _statusEffects.Clear();
            return sum;
        }


        /**
         * returns true iff 'type' strictly inherirts from BodyPartStatusEffect 
         */
        private bool IsStatusEffect(Type type)
        {
            return type.IsSubclassOf(typeof(BodyPartStatusEffect));
        }
    }
}