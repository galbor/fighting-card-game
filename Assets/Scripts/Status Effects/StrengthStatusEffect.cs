using DefaultNamespace.StatusEffects;

namespace Assets.Scripts.Status_Effects
{
    public class StrengthStatusEffect : BodyPartStatusEffect
    {
        private StrengthStatusEffect()
        {
            _description = "Deals X extra damage when attacking";
        }

        public override int Number
        {
            get => base.Number;
            set
            {
                base.Number = value;
                if (BodyPart == null) return;
                BodyPart.HitDamageDealtAddition = value;
            }
        }
    }
}