using UnityEngine;

[CreateAssetMenu(fileName = "New OpenDialogue Effect", menuName = "Effects/OpenDialogue")]
public class OpenDialogue : IEffect
{
    //[SerializeField] private DialogueStage dialogueStage;

    public override void Apply(GameObject target)
    {
        // DialogueController.OpenDialogue(dialogueStage)
    }
}
