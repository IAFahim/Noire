using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathMenuUI : UI
{
    [SerializeField] private Button respawnButton;
    [SerializeField] private Button mainMenuButton;

    private void Awake()
    {
        // TODO: respawn at last checkpoint
        // respawnButton.onClick.AddListener(() =>
        // {
        //     Loader.Load(DataPersistenceManager.Instance.CurrentScene);
        // });
        
        mainMenuButton.onClick.AddListener(() =>
        {
            Loader.Load(GameScene.MainMenuScene);
        });
    }

}
