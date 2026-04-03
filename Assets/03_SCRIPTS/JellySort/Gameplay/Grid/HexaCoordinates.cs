using System;
using UnityEngine;

namespace JellySort.Gameplay.Grid
{
    [Serializable]
    public struct HexaCoordinates : IEquatable<HexaCoordinates>
    {
        [SerializeField] private int _q;
        [SerializeField] private int _r;

        public int Q => _q;
        public int R => _r;
        public int S => -_q - _r;

        public HexaCoordinates(int q, int r)
        {
            _q = q;
            _r = r;
        }

        public HexaCoordinates(int q, int r, int s)
        {
            if (q + r + s != 0)
                throw new ArgumentException("q + r + s must be 0 for cube coordinates.");
            _q = q;
            _r = r;
        }
        
        public static HexaCoordinates FromOffset(int col, int row)
        {
            int q = col;
            int r = row;
            return new HexaCoordinates(q, r);
        }

        public Vector2Int ToOffset()
        {
            return new Vector2Int(Q, R);
        }

        public Vector3 ToWorldPosition(float radius = 1f)
        {
            float height = 2f * radius;
            float width = Mathf.Sqrt(3f) * radius;
            
            Vector3 qDirection = Quaternion.Euler(0, 60, 0) * Vector3.right;
            Vector3 rDirection = Vector3.back;
            Vector3 sDirection = Quaternion.Euler(0, 120, 0) * Vector3.right;

            Vector3 spawnPosition = 
                rDirection * R * height * 1.5f +
                qDirection * Q * width +
                sDirection * S * width;

            return spawnPosition;
        }

        public bool Equals(HexaCoordinates other)
        {
            return _q == other._q && _r == other._r;
        }

        public override bool Equals(object obj)
        {
            return obj is HexaCoordinates other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_q * 397) ^ _r;
            }
        }

        public override string ToString() => $"({Q}, {R}, {S})";

        public static bool operator ==(HexaCoordinates left, HexaCoordinates right) => left.Equals(right);
        public static bool operator !=(HexaCoordinates left, HexaCoordinates right) => !left.Equals(right);
    }
}
