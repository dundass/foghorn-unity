using UnityEngine;

public class PlaySound : MonoBehaviour, IEffect
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip audioClip;

    public void Apply(GameObject target)
    {
        audioSource.PlayOneShot(audioClip);
    }
}
