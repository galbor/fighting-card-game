﻿using DefaultNamespace.StatusEffects;
using DefaultNamespace.Utility;
using Managers;
using UnityEngine;

namespace Relics
{
    /**
     * inflicts _bleed bleed on the attacker body part when the player attacks with _bodyPart
     */
    public class SpikedGauntlet : AbstractRelic
    {
        [SerializeField] private Person.BodyPartEnum _bodyPart;
        [SerializeField] private int _bleed;
        
        protected new void Awake()
        {
            base.Awake();
            
            EventManager.Instance.StartListening(EventManager.EVENT__START_COMBAT, GiveKnife);
            
            Description = MyUtils.ReplaceAllBrackets(Description, "bleed", _bleed.ToString());
        }

        /**
         * gives the knife status effect, which inflicts bleed when attacking
         */
        private void GiveKnife(object obj)
        {
            Player.Instance.Person.GetHealthBar(_bodyPart).AddStatusEffect(typeof(KnifeStatusEffect), _bleed);
            Activate();
        }
    }
}