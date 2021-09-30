using UnityEngine;

namespace Player
{
    public class WeaponScript : MonoBehaviour
    {
        [SerializeField] private TopDownCharacterController _player;
        [SerializeField] private GameObject _weapon;

        private void FixedUpdate()
        {
            switch (_player.gameObject.layer)
            {
                case 20:
                    _weapon.gameObject.layer = 20;
                    _weapon.GetComponent<SpriteRenderer>().sortingLayerName = "Layer 1";
                    break;
                case 21:
                    _weapon.gameObject.layer = 21;
                    _weapon.GetComponent<SpriteRenderer>().sortingLayerName = "Layer 2";
                    break;
                case 22:
                    _weapon.gameObject.layer = 22;
                    _weapon.GetComponent<SpriteRenderer>().sortingLayerName = "Layer 3";
                    break;
            }

            _weapon.GetComponent<SpriteRenderer>().sortingOrder = 3;
        }
    }
}