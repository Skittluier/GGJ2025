namespace SpiritLevel
{
    using UnityEngine;
    using UnityEngine.Audio;

    public class GlobalAudio : Singleton<GlobalAudio>
    {
        [SerializeField]
        private AudioSource audioSource;


        internal void PlayAudioResource(AudioResource audioResource)
        {
            Debug.Log("[GlobalAudio] Play resource: " + audioResource.name);

            audioSource.resource = audioResource;
            audioSource.Play();
        }
    }
}