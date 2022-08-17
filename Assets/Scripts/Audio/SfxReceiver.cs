using UnityEngine;
using UnityEngine.Pool;

namespace Audio
{
    public class SfxReceiver : MonoBehaviour
    {
        [SerializeField] AudioChannel channel;
        [SerializeField] int poolSize;
        
        ObjectPool<AudioSource> pool;

        void Awake()
        {
            pool = new ObjectPool<AudioSource>(CreateSource, defaultCapacity: poolSize);
        }

        void OnEnable() => channel.OnAudioRequested += OnAudioRequested;
        void OnDisable() => channel.OnAudioRequested -= OnAudioRequested;

        static AudioSource CreateSource()
        {
            var s = new GameObject("Audio Source").AddComponent<AudioSource>();
            s.spatialBlend = 1;
            return s;
        }

        void OnAudioRequested(AudioClipGroup group, Vector3 position)
        {
            pool.Get(out var s);
            s.transform.position = position;
            
            var clip = group.GetNextClip();
            s.pitch = group.GetPitch();
            s.volume = group.GetVolume();
            s.PlayOneShot(clip);
            
            pool.Release(s);
        }
    }
}
