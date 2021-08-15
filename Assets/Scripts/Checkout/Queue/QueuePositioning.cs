using UnityEngine;

namespace Checkout
{
    public static class QueuePositioning
    {
        const int MaxCheckAngle = 180;
        const int CheckInterval = 1;

        const float CheckRadius = 0.5f;
        const float CheckHeightOffset = 0.5f;

        static readonly int ObstacleMask = ~LayerMask.GetMask("Player", "Customer", "Ground");

        static float currentSpacing;

        public static Vector3[] GenerateQueue(
            Vector3 queueStart, Vector3 startForward, float spacing, int length, int iterationLimit = 5)
        {
            if (length == 0) throw new System.ArgumentOutOfRangeException(nameof(length), "Queue length cannot be zero.");

            var positions = new Vector3[length];
            currentSpacing = spacing;

            positions[0] = queueStart;

            var succeeded = false;
            while (!succeeded)
                succeeded = GenerateFromFirst();
		
            return positions;
        
            bool GenerateFromFirst()
            {
                // On the second position and up...
                for (var i = 1; i < length; i++)
                {
                    Vector3 forward;
                    if (i == 1) forward = -startForward * spacing;
                    // Forward not multiplied since the distance between customers is always equal to IndividualOffset.
                    else forward = positions[i - 1] - positions[i - 2];

                    positions[i] = positions[i - 1] + forward;

                    // Check for obstacles.
                    if (!OverlapSphere(positions[i]) && !IsPathObstructed(positions[i - 1], positions[i]))
                        continue;

                    var pos = positions[i];
                    positions[i] = FindPositionAround(positions[i - 1], positions[i]);

                    // If the value didn't change, log a warning.
                    if (pos == positions[i])
                    {
#if UNITY_EDITOR
                        if (!Application.isPlaying)
                        {
                            Gizmos.color = Color.red;   
                            Gizmos.DrawWireSphere(pos + Vector3.up * CheckHeightOffset, CheckRadius);
                        }
#endif
                        Debug.LogWarning($"Couldn't find a suitable queue position at index {i}.");
			        
                        iterationLimit--;
                        if (iterationLimit > 0) return false;
                    }
                }

                return true;
            }
        }

        static bool OverlapSphere(Vector3 pos)
        {
            var collided = new Collider[1];
            pos += Vector3.up * CheckHeightOffset;
            return Physics.OverlapSphereNonAlloc(pos, CheckRadius, collided, ObstacleMask) > 0;
        }

        static bool IsPathObstructed(Vector3 from, Vector3 to)
        {
            from += Vector3.up * CheckHeightOffset;
            to += Vector3.up * CheckHeightOffset;
            var delta = (to - from).normalized;
            return Physics.Raycast(from, delta, currentSpacing, ObstacleMask);
        }

        static Vector3 FindPositionAround(Vector3 lastPosition, Vector3 position)
        {
            var forward = position - lastPosition;

            // Checks towards the left first if coin flip is heads.
            var side = Random.Range(0, 2) == 0 ? -1 : 1;
            var attempt = new Vector3();

            if (Spin(side) || Spin(-side)) return attempt;

            // If spinning in both directions failed, return the original value.
            return position;

            bool Spin(int multiplier)
            {
                for (var angle = 0; angle <= MaxCheckAngle / 2; angle += CheckInterval)
                {
                    attempt = lastPosition + Quaternion.Euler(0, multiplier * angle, 0) * forward;
                    if (!OverlapSphere(attempt) && !IsPathObstructed(lastPosition, attempt)) return true;
                }
                return false;
            }
        }
    }
}
