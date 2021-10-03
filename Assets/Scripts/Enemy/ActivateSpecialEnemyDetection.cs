using System;
using UnityEngine;

namespace Enemy
{
    public class ActivateSpecialEnemyDetection : MonoBehaviour
    {
        private const string PlayerTag = "Player";
        [SerializeField] private SpecialEnemy specialEnemy;
        private bool _isActivated;

        private void Start()
        {
            if (GameManager.GameManagerInstance == null) return;
            specialEnemy.enabled = false;
            _isActivated = false;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.gameObject.CompareTag(PlayerTag)) return;
            if (_isActivated == false)
            {
                specialEnemy.enabled = true;
                _isActivated = true;
            }
            else
            {
                specialEnemy.enabled = false;
                _isActivated = false;
            }
        }
    }
}