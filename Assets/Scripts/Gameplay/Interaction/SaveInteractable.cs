using System;
using UnityEngine;

/// Checkpoint object class
public class SaveInteractable : InteractableObject
{
    public override void Interact()
    {
        interactionsOccured++;
        interactableIndicator.Stop();
        onInteractIndicator.Restart();
        CheckpointUI.Instance.Show();
    }

    public override bool CanInteract()
    {
        return !disabled;
    }
}