using Managers;
using UnityEngine;

namespace DefaultNamespace
{
    public class RoomChoice : MonoBehaviour
    {
        [SerializeField] private Room _room;
        
        
        public void SetRoom()
        {
            RoomManager.Instance.SetRoom(_room);
            PlayerTurn.Instance.StartRound();
            StateManager.Instance.RemoveState();
            gameObject.SetActive(false);
        }
    }
}