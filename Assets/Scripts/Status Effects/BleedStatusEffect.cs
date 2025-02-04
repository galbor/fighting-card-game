﻿using System.Collections.Generic;
using Managers;
using UnityEngine;
using UnityEngine.Events;

namespace DefaultNamespace.StatusEffects
{
    public class BleedStatusEffect : BodyPartStatusEffect
    {
        protected BleedStatusEffect() : base() { }

        protected new void Awake()
        {
            base.Awake();
            _eventActionDict.Add(EventManager.EVENT__END_TURN, TakeBleedDamage);
            _description = "At the end of the turn, deals X damage and removes 1 stack";
        }
        
        private void TakeBleedDamage(object obj)
        {
            BodyPart.RemoveHealth(Number);
            Number -= 1;
        }
    }
}