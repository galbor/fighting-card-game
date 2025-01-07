using DefaultNamespace.StatusEffects;
using DefaultNamespace.UI;
using UnityEngine;

namespace cards
{
    [CreateAssetMenu(menuName = "Cards/Gain Status Effect Card")]
    public class GainStatusEffectCard : ApplyStatusEffect
    {

        protected GainStatusEffectCard()
        {
            _choiceOnEnemy = false;
            _defaultUnformattedDescription = "Gain {amt} stacks of {effect} on your {part}.\n"; ;
        }

        protected override void AddStatusEffect(Person user, HealthBar attacking_part, Person target,
            Person.BodyPartEnum affected_part)
        {
            attacking_part.AddStatusEffect(BodyPartStatusEffect.GetTypeOfStatusType(_statusEffect), _amt);
        }
    }
}