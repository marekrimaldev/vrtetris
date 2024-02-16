using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace VRTetris
{
    public class PieceSpawn : MonoBehaviour
    {
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private TimeVisualization _timer;
        [SerializeField] private Transform _spawnRadius;

        private Piece _spawnedPiece;
        
        public bool IsEmpty => _spawnedPiece == null;

        private void OnEnable()
        {
            // TODO
            _timer.OnTimesUp += () => { Debug.Log("Time is up"); };
        }

        private void Update()
        {
            if(_spawnedPiece != null)
            {
                LookAtClosestHand();
            }
        }

        public Piece SpawnPiece(Piece piecePrefab)
        {
            _spawnedPiece = InstantiateNextPiece(piecePrefab);
            _spawnedPiece.OnPieceGrabbed += OnPieceGrabbedListener;

            _timer.StartTimer(PieceSpawner.Instance.GrabLimit);

            return _spawnedPiece;
        }

        public void SetInteractability(bool val)
        {
            if (_spawnedPiece != null)
                _spawnedPiece.SetInteractability(val);
        }

        private Piece InstantiateNextPiece(Piece piecePrefab)
        {
            Piece piece = Instantiate(piecePrefab, _spawnPoint.position, _spawnPoint.rotation);
            piece.transform.localScale = Vector3.one * PieceSpawner.PieceScale;
            piece.transform.parent = transform;

            for (int i = 0; i < piece.Cubes.Length; i++)
            {
                piece.Cubes[i].localScale = Vector3.one * PieceSpawner.CubeInnerScale;
            }

            piece.gameObject.SetActive(false);
            StartCoroutine(ActivatePieceOnHandExitSpawnRadius(piece));

            return piece;
        }

        private IEnumerator ActivatePieceOnHandExitSpawnRadius(Piece piece)
        {
            while (IsHandInsideSpawnRadius())
            {
                yield return null;
            }

            piece.gameObject.SetActive(true);
        }

        private bool IsHandInsideSpawnRadius()
        {
            float leftHandDist = Vector3.Distance(transform.position, VRPlayer.Instance.LeftHand.HandPosition); 
            float rightHandDist = Vector3.Distance(transform.position, VRPlayer.Instance.RightHand.HandPosition);

            float radius = _spawnRadius.lossyScale.x / 2;
            return leftHandDist < radius || rightHandDist < radius;
        }

        private void LookAtClosestHand()
        {
            Vector3 leftHandPos = VRPlayer.Instance.LeftHand.HandPosition;
            Vector3 rightHandPos = VRPlayer.Instance.RightHand.HandPosition;

            float leftHandDist = Vector3.Distance(transform.position, leftHandPos);
            float rightHandDist = Vector3.Distance(transform.position, rightHandPos);

            bool leftHandCloser = leftHandDist < rightHandDist;
            Vector3 pos = leftHandCloser ? VRPlayer.Instance.LeftHand.HandPosition : VRPlayer.Instance.RightHand.HandPosition;

            Vector3 dir = (pos - transform.position).normalized;
            float rot = -Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            // We do not want to rotate the piece when the hand is inside the spawn radius
            float radius = _spawnRadius.lossyScale.x / 2;
            if (leftHandCloser && leftHandDist > radius)
            {
                _spawnedPiece.transform.localRotation = Quaternion.Euler(rot - 90, 0f, 0f);
            }
            else if (!leftHandCloser && rightHandDist > radius)
            {
                _spawnedPiece.transform.localRotation = Quaternion.Euler(rot - 90, 0f, 0f);
            }
        }

        private void OnPieceGrabbedListener(Piece piece)
        {
            piece.OnPieceGrabbed -= OnPieceGrabbedListener;

            piece.transform.parent = null;
            _timer.StopTimer();
            _spawnedPiece = null;
        }
    }
}
