using System;
using UnityEngine;

namespace Audio
{
    [CreateAssetMenu(menuName = "Events/Audio Channel")]
    public class AudioChannel : ScriptableObject
    {
        public event Action<AudioClipGroup, Vector3> OnAudioRequested;

        public void RequestAudio(AudioClipGroup group, Vector3 position) => OnAudioRequested?.Invoke(group, position);
    }
}
