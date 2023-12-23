using System.Collections;
using TMPro;
using UnityEngine;

public class DeathMenuUI : UI
{
    [SerializeField] private ButtonUI respawnButton;
    [SerializeField] private ButtonUI mainMenuButton;

    private void Start()
    {
        canvasGroup.alpha = 0;
        Show();
        
        mainMenuButton.AddListener(() => Loader.Load(GameScene.MainMenuScene));
        respawnButton.AddListener(Loader.Respawn);
    }
}
