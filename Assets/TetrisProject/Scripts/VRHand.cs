using UnityEngine;

namespace VRTetris
{
    public class VRHand : MonoBehaviour
    {
        [SerializeField] private Transform _handCenter;

        public Vector3 HandPosition => _handCenter.position;
    }
}