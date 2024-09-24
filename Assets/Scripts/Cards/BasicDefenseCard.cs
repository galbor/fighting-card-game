using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Basic Defense Card")]
public class BasicDefenseCard : BasicCard
{

    [SerializeField] private int _block;
    [SerializeField] private int _invincibility;
    // Start is called before the first frame update

    public override void Play(Person user, List<Person.BodyPartEnum> attacking_parts, Person target,
        Person.BodyPartEnum affected_part)
    {
        Block(user, attacking_parts, affected_part);
        base.Play(user, attacking_parts, target, affected_part);
    }

    private void Block(Person user, List<Person.BodyPartEnum> attacking_parts, Person.BodyPartEnum affected_part)
    {
        attacking_parts.All(x => {
            user.Defend(x, _block);
            if (affected_part != Person.BodyPartEnum.NONE) user.SetProtection(x, affected_part);
            return true;});
    }

    public override void UpdateDescription()
    {
        base.UpdateDescription();
        _displayDescription = ReplaceFirstOccurrence(_displayDescription, "block", _block.ToString());
        _displayDescription = ReplaceFirstOccurrence(_displayDescription, "invincibility", _invincibility.ToString());
    }
}
