﻿using UnityEngine;

namespace Player
{
    public class CameraFollow : MonoBehaviour
    {
        public Transform target;
        public float lerpSpeed = 1.0f;
        private Vector3 _offset;
        private Vector3 _targetPos;

        private void Start()
        {
            if (target == null) return;
            _offset = transform.position - target.position;
        }

        private void Update()
        {
            if (target == null) return;
            _targetPos = target.position + _offset;
            transform.position = Vector3.Lerp(transform.position, _targetPos, lerpSpeed * Time.deltaTime);
        }
    }
}
