using UnityEngine;

namespace Enemy
{
    public class Bullets : MonoBehaviour
    {
        [SerializeField] private float speed;
        private Transform _player;
        private CircleCollider2D _bulletCollider;
        private Vector2 _target;

        private void Start()
        {
            _bulletCollider = GetComponent<CircleCollider2D>();
            _player = GameObject.FindGameObjectWithTag("Player").transform;
            _target = new Vector2(_player.position.x, _player.position.y);
        }

        void Update()
        {
            transform.position = Vector2.MoveTowards(transform.position, _target, speed * Time.deltaTime);
            if (!transform.position.Equals(_target)) return;
            Destroy(_bulletCollider);
            Destroy(gameObject);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Props") || other.gameObject.CompareTag("Weapon"))
            {
                Destroy(gameObject);
                Destroy(this);
            }
        }
    }
}