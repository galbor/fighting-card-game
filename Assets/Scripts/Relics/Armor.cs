﻿using DefaultNamespace.Utility;
using Managers;
using UnityEngine;

namespace Relics
{
    /**
     * adds armor every turn/combat start
     */
    public class Armor : AbstractRelic
    {
        [SerializeField] private Person.BodyPartEnum _bodyPart;
        [SerializeField] private int _amt;

        protected new void Awake()
        {
            base.Awake();
            string eventTrigger = _resetEveryTurn ? EventManager.EVENT__START_TURN : EventManager.EVENT__START_COMBAT;
            EventManager.Instance.StartListening(eventTrigger, obj => Activate());
            
            Description = MyUtils.ReplaceAllBrackets(Description, "block", _amt.ToString());
        }

        protected override void Activate()
        {
            base.Activate();
            Player.Instance.Person.GetHealthBar(_bodyPart).AddDefense(_amt);
        }
    }
}