using System;
using DG.Tweening;
using UnityEngine;

namespace Animation
{
    public class BoxFlapAnimator : MonoBehaviour
    {
        [Serializable]
        struct FlapSet
        {
            public Transform[] transforms;
            public Vector3 openRotation;
            public float startTime;
            public float duration;
            [HideInInspector] public Quaternion[] openRotationQuaternions;
        }
    
        [SerializeField] FlapSet[] flapSets;

        Sequence openSequence;

        void Awake()
        {
            for (var i = 0; i < flapSets.Length; i++)
            {
                flapSets[i].openRotationQuaternions = new Quaternion[flapSets[i].transforms.Length];
                for (var j = 0; j < flapSets[i].transforms.Length; j++)
                    flapSets[i].openRotationQuaternions[j] =
                        Quaternion.Euler(flapSets[i].transforms[j].localEulerAngles + flapSets[i].openRotation);
            }
        
            openSequence = DOTween.Sequence();

            foreach (var flapSet in flapSets)
            {
                var tween = GetTween(0);
                openSequence.Insert(flapSet.startTime, tween);

                for (var i = 1; i < flapSet.transforms.Length; i++)
                    openSequence.Join(GetTween(i));
            
                Tween GetTween(int index) =>
                    flapSet.transforms[index].DOLocalRotateQuaternion(flapSet.openRotationQuaternions[index], flapSet.duration);
            }

            openSequence.SetAutoKill(false);
            openSequence.Pause();
        }

        void OnDestroy() => openSequence.Kill();

        public void Open() => Animate(true);
        public void Close() => Animate(false);

        void Animate(bool open)
        {
            if (open)
                openSequence.PlayForward();
            else
                openSequence.PlayBackwards();
        }
    }
}
