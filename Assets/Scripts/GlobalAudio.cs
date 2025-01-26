namespace SpiritLevel
{
    using UnityEngine;
    using UnityEngine.Audio;

    public class GlobalAudio : Singleton<GlobalAudio>
    {
        [SerializeField]
        private AudioSource audioSource, audioSourceTwo;


        internal void PlayAudioResource(AudioResource audioResource)
        {
            Debug.Log("[GlobalAudio] Play resource: " + audioResource.name);

            if (!audioSource.isPlaying)
            {
                audioSource.resource = audioResource;
                audioSource.Play();
            }
            else
            {
                audioSourceTwo.resource = audioResource;
                audioSourceTwo.Play();
            }
        }
    }
}