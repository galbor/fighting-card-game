using UnityEngine;

namespace DefaultNamespace
{
    /**
     * when on map
     */
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D _body;
        [SerializeField] private KeyCode _up;
        [SerializeField] private KeyCode _down;
        [SerializeField] private KeyCode _left;
        [SerializeField] private KeyCode _right;
        [SerializeField] private float _speed;


        private void Update()
        {
            Move();
        }

        private void Move()
        {
            Vector2 direction = Vector2.zero;
            if (Input.GetKey(_up))
                direction += Vector2.up;
            if (Input.GetKey(_down))
                direction += Vector2.down;
            if (Input.GetKey(_left))
                direction += Vector2.left;
            if (Input.GetKey(_right))
                direction += Vector2.right;
            
            _body.velocity = direction * _speed;
        }
    }
}