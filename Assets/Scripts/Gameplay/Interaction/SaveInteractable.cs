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
        DataPersistenceManager.Instance.GameData.LastCheckPointScene = Loader.TargetScene;
        DataPersistenceManager.Instance.GameData.LastCheckPointPosition = transform.position;
    }

    public override bool CanInteract()
    {
        return !disabled;
    }
}