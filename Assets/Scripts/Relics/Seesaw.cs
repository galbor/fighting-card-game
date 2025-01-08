using System;
using DefaultNamespace.StatusEffects;
using Managers;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Relics
{
    public class Seesaw : AbstractRelic
    {
        [SerializeField] private BodyPartStatusEffect.StatusType _statusType;
        private Boolean _leftSide = false;

        protected new void Awake()
        {
            base.Awake();
            EventManager.Instance.StartListening(EventManager.EVENT__START_COMBAT, ResetCounter);
            EventManager.Instance.StartListening(EventManager.EVENT__START_TURN, Activate);
        }

        public override void OnAddRelic()
        {
            base.OnAddRelic();
            _leftSide = Random.Range(0, 1) == 0;
        }

        protected override void Activate(object obj = null)
        {
            base.Activate(obj);
            _leftSide = !_leftSide;
            Person player = Player.Instance.Person;
            int sign = _leftSide ? 1 : -1;
            int amt = Counter * 2 + 1;
            var type = BodyPartStatusEffect.GetTypeOfStatusType(_statusType);

            player.GetHealthBar(Person.BodyPartEnum.LEFT_ARM).AddStatusEffect(type, amt * sign);
            player.GetHealthBar(Person.BodyPartEnum.LEFT_LEG).AddStatusEffect(type, amt * sign);
            player.GetHealthBar(Person.BodyPartEnum.RIGHT_ARM).AddStatusEffect(type, -amt * sign);
            player.GetHealthBar(Person.BodyPartEnum.RIGHT_LEG).AddStatusEffect(type, -amt * sign);

            IncrementCounter();
        }
    }
}