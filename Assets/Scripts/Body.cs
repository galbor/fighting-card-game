using TMPro;
using UnityEngine;

public class Body : MonoBehaviour
{
    [SerializeField] public HealthBar[] _healthBars;
    [SerializeField] public TMP_Text[] _bodyPartTexts;
    [SerializeField] public TMP_Text _enemyNumber;
    [SerializeField] public PlannedAttackDisplay _plannedAttackDisplay;
}