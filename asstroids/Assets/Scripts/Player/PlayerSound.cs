using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerSound : MonoBehaviour
{
    [SerializeField] private AudioClip[] footsteps;
    [SerializeField] private float footstepsVolume = 0.8f;
    [SerializeField] private AudioClip[] throws;
    [SerializeField] private AudioClip[] grab;
    [SerializeField] private AudioClip jump;
    [SerializeField] private AudioClip land;
    [SerializeField] private AudioClip jetpackusing;
    [SerializeField] private AudioClip jetpackempty;
    [SerializeField] private AudioClip jetpackrefilling;
    [SerializeField] private AudioClip jetpackrefilled;

    PlatformerCollision plat;
    AudioSource audioSource;

    AudioSource as_jetpackusing;
    AudioSource as_jetpackrefilling;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        plat = GetComponent<PlatformerCollision>();
        as_jetpackusing = gameObject.AddComponent<AudioSource>();
        as_jetpackrefilling = gameObject.AddComponent<AudioSource>();

        as_jetpackusing.spatialBlend = 1;
        as_jetpackusing.clip = jetpackusing;
        as_jetpackusing.loop = true;

        as_jetpackrefilling.spatialBlend = 1;
        as_jetpackrefilling.clip = jetpackrefilling;
        as_jetpackrefilling.loop = true;
    }

    public void PlayRandomFootstep()
    {
        if (!plat.Grounded)
            return;

        var x = Internal_GetRandomSoundFromArray(footsteps);
        if (x != null)
            audioSource.PlayOneShot(x, footstepsVolume);
    }

    public void PlayJetpackUsingSound()
    {
        if(!as_jetpackusing.isPlaying)
            as_jetpackusing.Play();
    }

    public void PauseJetpackUsingSound()
    {
        if (as_jetpackusing.isPlaying)
            as_jetpackusing.Pause();
    }

    public void PlayJetpackRefillingSound()
    {
        if (!as_jetpackrefilling.isPlaying)
            as_jetpackrefilling.Play();
    }

    public void PauseJetpackRefillingSound()
    {
        if (as_jetpackrefilling.isPlaying)
            as_jetpackrefilling.Pause();
    }

    public void PlayJetpackRefilledSound()
    {
        audioSource.PlayOneShot(jetpackrefilled, 0.6f);
    }

    public void PlayJetpackEmpty()
    {
        audioSource.PlayOneShot(jetpackempty);
    }

    public void PlayThrowSound()
    {
        var x = Internal_GetRandomSoundFromArray(throws);
        if (x != null)
            audioSource.PlayOneShot(x, 0.6f);
    }

    public void PlayGrabSound()
    {
        var x = Internal_GetRandomSoundFromArray(grab);
        if (x != null)
            audioSource.PlayOneShot(x);
    }

    public void PlayJumpSound()
    {
        if (jump && plat.Grounded)
            audioSource.PlayOneShot(jump);
    }

    public void PlayLandSound()
    {
        if (land)
        {
            audioSource.pitch = 1.5f;
            audioSource.PlayOneShot(land, 0.5f);
            audioSource.pitch = 1.0f;
        }
    }

    private AudioClip Internal_GetRandomSoundFromArray(AudioClip[] array)
    {
        if (array.Length == 0)
            return null;

        int num = Random.Range(0, array.Length);
        return array[num];
    }
}
