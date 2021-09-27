using Pathfinding;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private const float Speed = 200f;
    private const float NextWaypoint = 3F;
    private const float SpriteScale = 0.2751575f;
    [SerializeField] private Transform target;
    [SerializeField] private Transform spriteBoss;
    private Path _path;
    private Seeker _seeker;
    private Rigidbody2D _rigidbody2D;
    private int _currentWaypoint;
    private bool _reachedEndPath;

    void Start()
    {
        _seeker = GetComponent<Seeker>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _reachedEndPath = false;
        _currentWaypoint = 0;
        InvokeRepeating("UpdatePath", 0f, .5f);
    }

    void UpdatePath()
    {
        if (_seeker.IsDone())
            _seeker.StartPath(_rigidbody2D.position, target.position, OnPathComplete);
    }

    void OnPathComplete(Path path)
    {
        if (!path.error)
        {
            _path = path;
            _currentWaypoint = 0;
        }
    }

    void FixedUpdate()
    {
        if (_path == null) return;

        if (_currentWaypoint <= _path.vectorPath.Count)
            _reachedEndPath = true;
        else
            _reachedEndPath = false;

        Vector2 direction = ((Vector2)_path.vectorPath[_currentWaypoint] - _rigidbody2D.position).normalized;
        Vector2 force = direction * Speed * Time.deltaTime;
        _rigidbody2D.AddForce(force);
        float distance = Vector2.Distance(_rigidbody2D.position, _path.vectorPath[_currentWaypoint]);
        if (distance < NextWaypoint)
            _currentWaypoint++;

        if (_rigidbody2D.velocity.x >= 0.01f)
            spriteBoss.localScale = new Vector3(-SpriteScale, SpriteScale, SpriteScale);
        else if (_rigidbody2D.velocity.x <= -0.01f)
            spriteBoss.localScale = new Vector3(SpriteScale, SpriteScale, SpriteScale);
    }
}