using Assets.Scripts.Status_Effects;
using DefaultNamespace.Utility;
using Managers;
using UnityEngine;

namespace Relics
{
    class BoxingGlove : AbstractRelic
    {
        [SerializeField] private int _strength;

        protected new void Awake()
        {
            base.Awake();

            EventManager.Instance.StartListening(EventManager.EVENT__START_COMBAT, Strengthen);

            Description = MyUtils.ReplaceAllBrackets(Description, "strength", _strength.ToString());
        }

        /**
         * gives the knife status effect, which inflicts bleed when attacking
         */
        private void Strengthen(object obj)
        {
            Player.Instance.Person.GetHealthBar(Person.BodyPartEnum.LEFT_ARM).AddStatusEffect(typeof(StrengthStatusEffect), _strength);
            Player.Instance.Person.GetHealthBar(Person.BodyPartEnum.RIGHT_ARM).AddStatusEffect(typeof(StrengthStatusEffect), _strength);
            Player.Instance.Person.GetHealthBar(Person.BodyPartEnum.LEFT_LEG).AddStatusEffect(typeof(StrengthStatusEffect), -_strength);
            Player.Instance.Person.GetHealthBar(Person.BodyPartEnum.RIGHT_LEG).AddStatusEffect(typeof(StrengthStatusEffect), -_strength);
            Activate();
        }
    }
}
