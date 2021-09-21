using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetection : MonoBehaviour
{
    [SerializeField] private GameObject projectile;
    private const float Speed = 1F;
    private const float TimeBetweenShots = 1f;
    private Transform _target;
    private bool _hasShot;
    private bool _isLeft;
    private bool _isRight;
    private SpriteRenderer test;

    private void Start()
    {
        _hasShot = false;
        _isLeft = true;
        _isRight = false;
        test = gameObject.GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (_target == null) return;
        float step = Speed * Time.deltaTime;
        transform.position = Vector2.MoveTowards(transform.position, _target.position, step);
        if (_target.position.x < gameObject.transform.position.x && _isRight)
        {
            test.flipX = false;
            _isLeft = true;
            _isRight = false;
        }
        else if (_target.position.x > gameObject.transform.position.x && _isLeft)
        {
            test.flipX = true;
            _isLeft = false;
            _isRight = true;
        }
        if (_hasShot) return;
        StartCoroutine(DelayNextShot());
    }

    private IEnumerator DelayNextShot()
    {
        _hasShot = true;
        Instantiate(projectile, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(TimeBetweenShots);
        _hasShot = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            _target = other.transform;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            _target = null;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Weapon"))
            Destroy(gameObject);
    }
}