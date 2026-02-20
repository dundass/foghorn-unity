using UnityEngine;

[CreateAssetMenu(fileName = "New PlaySound Effect", menuName = "Effects/PlaySound")]
public class PlaySound : IEffect
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip audioClip;

    public override void Apply(GameObject target)
    {
        audioSource.PlayOneShot(audioClip);
    }
}
