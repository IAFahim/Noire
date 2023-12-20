using UnityEngine;
using Ink.Runtime;
using UnityEngine.UI;

public class InteractableExamine : InteractableObject
{
    [TextArea(3, 5)] [SerializeField] private string examineText; 
    [SerializeField] private Texture2D examineImage;
    
    public override void Interact()
    {
        onInteractIndicator.Play();
        interactionsOccured++;
        ExamineUI.Instance.Display(examineText, examineImage);
        FinishInteract();
    }
}