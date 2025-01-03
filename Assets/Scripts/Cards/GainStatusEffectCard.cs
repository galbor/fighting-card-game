﻿using System.Collections.Generic;
using System.Text;
using DefaultNamespace.StatusEffects;
using DefaultNamespace.UI;
using DefaultNamespace.Utility;
using UnityEngine;

namespace cards
{
    [CreateAssetMenu(menuName = "Cards/Gain Status Effect Card")]
    public class GainStatusEffectCard : ApplyStatusEffect
    {

        protected GainStatusEffectCard()
        {
            _defaultDescriptionFormat = "Gain {0} stacks of {1} on your {2}.\n";
        }

        protected override void AddStatusEffect(Person user, HealthBar attacking_part, Person target,
            Person.BodyPartEnum affected_part)
        {
            attacking_part.AddStatusEffect(BodyPartStatusEffect.GetTypeOfStatusType(_statusEffect), _amt);
        }

        protected override string FormattedSingleAttackerTypeDescription(AttackerTypeEnum attackerType)
        {
            return string.Format(_defaultDescriptionFormat,
                    _amt, GetStatusName(), attackerType.ToString());
        }


    }
}