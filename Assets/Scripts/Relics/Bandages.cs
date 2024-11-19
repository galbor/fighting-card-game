using DefaultNamespace.Utility;
using UnityEngine;
using Managers;

namespace Relics
{
    /**
     * Heals every combat/turn start
     */
    public class Bandages : AbstractRelic
    {
        [SerializeField] private Person.BodyPartEnum _bodyPart;
        [SerializeField] private int _amt;

        protected new void Awake()
        {
            base.Awake();
            string eventTrigger = _resetEveryTurn ? EventManager.EVENT__START_TURN : EventManager.EVENT__START_COMBAT;
            EventManager.Instance.StartListening(eventTrigger, obj => Activate());
            
            Description = MyUtils.ReplaceAllBrackets(Description, "health", _amt.ToString());
        }

        protected override void Activate()
        {
            base.Activate();
            Player.Instance.Person.GetHealthBar(_bodyPart).AddHealth(_amt);
        }
    }
}