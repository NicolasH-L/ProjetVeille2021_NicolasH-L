using System;
using UnityEngine;

public class EnemyDetection : MonoBehaviour
{
    [SerializeField] private GameObject gameObject;
    private const float Speed = 1F;
    private Transform _target;

    private void Update()
    {
        float step = Speed * Time.deltaTime;
        if (_target != null)
            transform.position = Vector2.MoveTowards(transform.position, _target.position, step);
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