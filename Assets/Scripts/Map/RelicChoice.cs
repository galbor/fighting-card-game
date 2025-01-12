using System.Collections.Generic;
using Managers;
using Map;
using UnityEngine;

namespace Assets.Scripts.Map
{
    public class RelicChoice : MonoBehaviour
    {

        [SerializeField] private List<RelicRoom> relicRooms;

        // Use this for initialization
        void Start()
        {
            EventManager.Instance.StartListening(EventManager.EVENT__ADD_RELIC, CheckMadeChoice);
        }

        private void CheckMadeChoice(object obj = null)
        {
            if (relicRooms.Exists(r => r.Taken))
                DeactivateAllRooms();
        }


        private void DeactivateAllRooms()
        {
            relicRooms.ForEach(r =>
            {
                r.gameObject.SetActive(false);
            });
            gameObject.SetActive(false);

            EventManager.Instance.StopListening(EventManager.EVENT__ADD_RELIC, CheckMadeChoice);
        }
    }
}