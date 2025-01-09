using DefaultNamespace;
using Managers;

namespace DefaultNamespace.StatusEffects
{
    public class KnifeStatusEffect : BodyPartStatusEffect
    {
        protected KnifeStatusEffect() : base() { }
        
        protected new void Awake()
        {
            base.Awake();
            _eventActionDict.Add(EventManager.EVENT__HIT, InflictBleed);
            _description = "When attacking, Apply X bleed";
        }

        private void InflictBleed(object obj)
        {
            var attack = (AttackStruct)obj;
            if (attack.GetHealthBar(true) == BodyPart)
                attack.GetHealthBar(false).AddStatusEffect(typeof(BleedStatusEffect), Number);
        }
    }
}