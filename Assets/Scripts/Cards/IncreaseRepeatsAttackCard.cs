using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using cards;
using DefaultNamespace.Utility;
using Managers;
using UnityEditor;
using UnityEngine;

namespace cards
{
    [CreateAssetMenu(menuName = "Cards/Increase Repeats Attack Card")]
    public class IncreaseRepeatsAttackCard : BasicAttackCard
    {
        protected int _repeats = 0;
        private string _eventName = EventManager.EVENT__KILL_ENEMY;

        protected IncreaseRepeatsAttackCard() : base()
        { 
            _repeats = 0;
        }

        public override void Play(Person user, List<Person.BodyPartEnum> attacking_parts, Person target, List<Person.BodyPartEnum> affected_parts)
        {
            int cur_repeats = _repeats;
            EventManager.Instance.StartListening(_eventName, IncrementRepeats);

            for (int i = 0; i < cur_repeats+1; i++)
            {
                base.Play(user, attacking_parts, target, affected_parts);
            }


            EventManager.Instance.StopListening(_eventName, IncrementRepeats);
        }

        private void IncrementRepeats(object obj)
        {
            _repeats++;
        }

        protected override string GenerateThisDefaultUnformattedDescription()
        {
            StringBuilder res = new StringBuilder( base.GenerateThisDefaultUnformattedDescription());

            res.Append("Repeat for each time this killed an enemy (currently {repeat} times).\n");
            return res.ToString();
        }

        protected override string FormatDescription(string unformattedStr)
        {
            string res = base.FormatDescription(unformattedStr);
            return MyUtils.ReplaceAllBrackets(res, "repeat", _repeats.ToString());
        }
    }
}