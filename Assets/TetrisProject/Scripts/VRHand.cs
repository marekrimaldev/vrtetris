using UnityEngine;
using UnityEngine.InputSystem;

namespace VRTetris
{
    public class VRHand : MonoBehaviour
    {
        [SerializeField] private Transform _handCenter;
        [SerializeField] private InputActionProperty _flipPieceAction;

        public Vector3 HandPosition => _handCenter.position;

        public Piece HeldPiece { get; set; }

        private bool _pieceFlippedHold = false;
        private const float Threshold = 0.01f;

        private void Update()
        {
            if (HeldPiece != null)
            {
                DetectPieceFlip();
            }
        }

        private void DetectPieceFlip()
        {
            float leftWandValue = _flipPieceAction.action.ReadValue<float>();

            Debug.LogWarning("Value = " + leftWandValue);

            if (!_pieceFlippedHold && leftWandValue > Threshold)
            {
                HeldPiece.Flip();
                _pieceFlippedHold = true;
            }
            else if (leftWandValue < Threshold)
            {
                _pieceFlippedHold = false;
            }
        }
    }
}