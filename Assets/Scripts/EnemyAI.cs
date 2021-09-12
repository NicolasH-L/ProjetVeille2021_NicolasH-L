using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform target;
    private const float SPEED = 200f;
    private const float NEXT_WAYPOINT = 3F;
    private Path _path;
    private int _currentWaypoint = 0;
    private bool _reachedEndPath = false;
    private Seeker _seeker;
    private Rigidbody2D _rigidbody2D;

    void Start()
    {
        _seeker = GetComponent<Seeker>();
        _rigidbody2D = GetComponent<Rigidbody2D>();

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
        if (_path == null)
            return;

        if (_currentWaypoint <= _path.vectorPath.Count)
            _reachedEndPath = true;
        else
            _reachedEndPath = false;

        Vector2 direction = ((Vector2)_path.vectorPath[_currentWaypoint] - _rigidbody2D.position).normalized;
        Vector2 force = direction * SPEED * Time.deltaTime;
        _rigidbody2D.AddForce(force);
        float distance = Vector2.Distance(_rigidbody2D.position, _path.vectorPath[_currentWaypoint]);
        if (distance < NEXT_WAYPOINT)
        {
            _currentWaypoint++;
        }
    }
}