using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Pool;

namespace Audio
{
    public class SfxReceiver : MonoBehaviour
    {
        [SerializeField] AudioChannel channel;
        [SerializeField] int poolSize;
        [SerializeField] AudioMixerGroup defaultMixerGroup;
        
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

        void OnAudioRequested(AudioClipGroup group, Vector3 position, float spatialBlend)
        {
            pool.Get(out var s);
            s.spatialBlend = spatialBlend;
            s.transform.position = position;
            s.outputAudioMixerGroup = group.output ? group.output : defaultMixerGroup;
            
            var clip = group.GetNextClip();
            s.pitch = group.GetPitch();
            s.volume = group.GetVolume();
            s.PlayOneShot(clip);
            
            pool.Release(s);
        }
    }
}
