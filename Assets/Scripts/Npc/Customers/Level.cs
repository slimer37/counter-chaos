using UnityEngine;

namespace Npc.Customers
{
    public class Level : MonoBehaviour
    {
        static Transform[] finishPoints;

        public static Vector3 GetFinishPoint() => finishPoints[Random.Range(0, finishPoints.Length)].position;

        void Awake()
        {
            var finishGOs = GameObject.FindGameObjectsWithTag("Finish");
            finishPoints = new Transform[finishGOs.Length];
            for (var i = 0; i < finishGOs.Length; i++)
                finishPoints[i] = finishGOs[i].transform;
        }
    }
}
