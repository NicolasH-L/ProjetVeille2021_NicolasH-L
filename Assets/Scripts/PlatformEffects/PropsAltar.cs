using System.Collections.Generic;
using UnityEngine;

//when something get into the alta, make the runes glow
namespace PlatformEffects
{
    public class PropsAltar : MonoBehaviour
    {
        public List<SpriteRenderer> runes;
        public float lerpSpeed;
        private Color _curColor;
        private Color _targetColor;

        private void OnTriggerEnter2D(Collider2D other)
        {
            _targetColor = new Color(1, 1, 1, 1);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            _targetColor = new Color(1, 1, 1, 0);
        }

        private void Update()
        {
            _curColor = Color.Lerp(_curColor, _targetColor, lerpSpeed * Time.deltaTime);
            foreach (var r in runes)
                r.color = _curColor;
        }
    }
}