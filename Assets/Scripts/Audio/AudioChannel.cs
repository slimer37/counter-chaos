using System;
using UnityEngine;

namespace Audio
{
    [CreateAssetMenu(menuName = "Events/Audio Channel")]
    public class AudioChannel : ScriptableObject
    {
        public event Action<AudioClipGroup, Vector3, float> OnAudioRequested;

        public void RequestAudio(AudioClipGroup group,
            Vector3 position = default,
            float spatialBlend = 1) =>
            OnAudioRequested?.Invoke(group, position, spatialBlend);
    }
}
