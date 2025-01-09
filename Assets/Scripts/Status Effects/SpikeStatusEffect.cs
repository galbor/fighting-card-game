using DefaultNamespace;
using Managers;

namespace DefaultNamespace.StatusEffects
{
    public class SpikeStatusEffect : BodyPartStatusEffect
    {
        protected SpikeStatusEffect() : base() { }
        
        protected new void Awake()
        {
            base.Awake();
            _eventActionDict.Add(EventManager.EVENT__HIT, inflictSpikeDamage);
            _description = "When attacked, deal X damage to attacking body part";
        }
        
        
        private void inflictSpikeDamage(object obj)
        {
            var attack = (AttackStruct)obj;
            if (attack.GetHealthBar(false) == BodyPart)
                attack.GetHealthBar(true).RemoveHealth(Number);
        }
    }
}