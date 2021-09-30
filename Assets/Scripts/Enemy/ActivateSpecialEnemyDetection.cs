using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateSpecialEnemyDetection : MonoBehaviour
{
    private const string PlayerTag = "Player";
    [SerializeField] private SpecialEnemy specialEnemy;

    private void Start()
    {
        if (GameManager.GameManagerInstance == null) return;
        specialEnemy.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag(PlayerTag)) return;
        specialEnemy.enabled = true;
    }
}
