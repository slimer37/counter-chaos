using System;
using UnityEngine;

namespace Audio
{
    public class AudioCue : MonoBehaviour
    {
        [SerializeField] AudioClipGroup[] clipGroups;
        [SerializeField] AudioChannel sfxChannel;

        void Reset()
        {
            clipGroups = new[] { new AudioClipGroup() };
        }

        public void Play(int i)
        {
            if (!sfxChannel) throw new NullReferenceException($"No {nameof(AudioChannel)} assigned.");
            sfxChannel.RequestAudio(clipGroups[i], transform.position);
        }
    }
}