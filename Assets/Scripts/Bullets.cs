using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullets : MonoBehaviour
{
    private const float Speed = 2f;
    private Transform _player;
    private Vector2 _target;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _target = new Vector2(_player.position.x, _player.position.y);
    }

    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, _target, Speed * Time.deltaTime);
        if (transform.position.Equals(_target))
        {
            Destroy(gameObject);
        }
    }
}