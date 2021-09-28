using System;
using System.Collections;
using Pathfinding;
using UnityEngine;

public class SpecialEnemy : MonoBehaviour
{
    private const float Speed = 100f;
    private const float NextWaypoint = 3F;
    private const float SpriteScale = 0.2751575f;
    private const float ResetDelay = 0.5f;
    private const int StartingHealthPoint = 200;
    private const string WeaponTag = "Weapon";
    [SerializeField] private Transform target;
    [SerializeField] private Transform spriteBoss;
    private Path _path;
    private Seeker _seeker;
    private Rigidbody2D _rigidbody2D;
    private int _currentWaypoint;
    private int _healthPoint;
    private bool _isHit;
    private bool _reachedEndPath;

    void Start()
    {
        if (GameManager.GameManagerInstance == null) return;
        _healthPoint = StartingHealthPoint;
        _seeker = GetComponent<Seeker>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _currentWaypoint = 0;
        _reachedEndPath = false;
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

        if (_currentWaypoint >= _path.vectorPath.Count)
        {
            _reachedEndPath = true;
            return;
        }

        _reachedEndPath = false;
        Vector2 direction = ((Vector2)_path.vectorPath[_currentWaypoint] - _rigidbody2D.position).normalized;
        Vector2 force = direction * Speed * Time.deltaTime;
        _rigidbody2D.AddForce(force);
        float distance = Vector2.Distance(_rigidbody2D.position, _path.vectorPath[_currentWaypoint]);
        if (distance < NextWaypoint)
            _currentWaypoint++;

        if (_rigidbody2D.velocity.x >= 0.01f)
            spriteBoss.localScale = new Vector3(SpriteScale, SpriteScale, SpriteScale);
        else if (_rigidbody2D.velocity.x <= -0.01f)
            spriteBoss.localScale = new Vector3(-SpriteScale, SpriteScale, SpriteScale);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag(WeaponTag) || GameManager.GameManagerInstance == null || _isHit)
            return;
        TakeDamage(GameManager.GameManagerInstance.GetPlayerDamage());
    }

    private void TakeDamage(int damage)
    {
        _isHit = true;
        if (_healthPoint - damage <= 0)
        {
            Destroy(GetComponent<Rigidbody2D>());
            Destroy(GetComponent<Collider2D>());
            Destroy(GetComponent<SpriteRenderer>());
            Destroy(gameObject);
            return;
        }

        _healthPoint -= damage;
        StartCoroutine(ResetHit());
    }

    private IEnumerator ResetHit()
    {
        yield return new WaitForSeconds(ResetDelay);
        _isHit = false;
    }
}