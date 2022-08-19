using System;
using UnityEngine;

namespace Audio
{
    public class AudioCue : MonoBehaviour
    {
        [SerializeField] AudioClipGroup clipGroup;
        [SerializeField] AudioChannel sfxChannel;

        public void Play()
        {
            if (!sfxChannel) throw new NullReferenceException($"No {nameof(AudioChannel)} assigned.");
            sfxChannel.RequestAudio(clipGroup, transform.position);
        }
    }
}