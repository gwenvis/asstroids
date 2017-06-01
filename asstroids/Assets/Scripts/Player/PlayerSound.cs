using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerSound : MonoBehaviour
{
    [SerializeField] private AudioClip[] footsteps;
    [SerializeField] private float footstepsVolume = 1.0f;
    [SerializeField] private AudioClip[] throws;
    [SerializeField] private AudioClip[] grab;
    [SerializeField] private AudioClip jump;
    [SerializeField] private AudioClip land;

    PlatformerCollision plat;
    AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        plat = GetComponent<PlatformerCollision>();
    }

    public void PlayRandomFootstep()
    {
        if (!plat.Grounded)
            return;

        var x = Internal_GetRandomSoundFromArray(footsteps);
        if (x != null)
            audioSource.PlayOneShot(x, footstepsVolume);
    }

    public void PlayThrowSound()
    {
        var x = Internal_GetRandomSoundFromArray(throws);
        if (x != null)
            audioSource.PlayOneShot(x);
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
            audioSource.PlayOneShot(land);
    }

    private AudioClip Internal_GetRandomSoundFromArray(AudioClip[] array)
    {
        if (array.Length == 0)
            return null;

        int num = Random.Range(0, array.Length);
        return array[num];
    }
}
