using System;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

namespace Audio
{
    // Loosely based on code from https://github.com/UnityTechnologies/open-project-1
    [Serializable]
    public class AudioClipGroup
    {
        public AudioClip[] clips;
        public SequenceMode mode = SequenceMode.Random;
        public float volume = 1;
        public float volumeVariation;
        public float pitch = 1;
        public float pitchVariation;
        public AudioMixerGroup output;

        int lastClip;

        public float GetVolume() => volume + Random.Range(-volumeVariation, volumeVariation);
        public float GetPitch() => pitch + Random.Range(-pitchVariation, pitchVariation);

        public AudioClip GetNextClip()
        {
            return mode switch
            {
                SequenceMode.Random => clips[Random.Range(0, clips.Length)],
                SequenceMode.Ordered => clips[lastClip++ % clips.Length],
                _ => throw new Exception("Mode not recognized.")
            };
        }
    }
    
    public enum SequenceMode
    {
        Random,
        Ordered
    }
}
