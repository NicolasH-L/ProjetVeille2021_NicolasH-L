using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetection : MonoBehaviour
{
    [SerializeField] private GameObject gameObject;
    [SerializeField] private GameObject projectile;
    private const float Speed = 1F;
    private const float TimeBetweenShots = 2f;
    private Transform _target;
    private bool _hasShot;

    private void Start()
    {
        _hasShot = false;
    }

    private void Update()
    {
        float step = Speed * Time.deltaTime;
        if (_target != null)
        {
            transform.position = Vector2.MoveTowards(transform.position, _target.position, step);
            if (_hasShot) return;
            StartCoroutine(DelayNextShot());
        }
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