using System;
using UnityEngine.SceneManagement;

/// <summary>
/// The scene loader. Maintains static variables for the current loading scene, scene information,
/// and any onSceneLoaded callbacks.
/// Provides single function entrypoint for Scene loading through Loader.Load().
/// </summary>
public static class Loader 
{
    private static string LoadScene = GameScene.LoadingScene.ToString();
    public const GameScene FirstScene = GameScene.BedrockPlains;

    public static SceneInfo TargetSceneInfoObj;
    public static string TargetScene;

    public static Action SceneLoadedCallback; 
    
    /// Loads a GameScene. Returns true upon successful loading.
    public static bool Load(GameScene nextScene, Action callback=null)
    {
        SceneLoadedCallback = callback;
        
        TargetScene = nextScene.ToString();
        TargetSceneInfoObj = StaticInfoObjects.GetSceneInfo(nextScene);
        
        switch (TargetSceneInfoObj.Type)
        {
            case SceneType.Single: case SceneType.Parent:
                return SceneTransitioner.Instance.LoadSceneSingle(LoadScene);
            case SceneType.Child:
                return SceneTransitioner.Instance.LoadSceneChild(TargetScene);
            default:
                return false;
        }
    }
    
    // overloading: load using string scene name
    public static bool Load(string nextScene, Action callback=null)
    {
        return Load(StaticInfoObjects.ToGameScene(nextScene), callback);
    }
    
    public static bool Load(Scene nextScene, Action callback=null)
    {
        return Load(nextScene.name, callback);
    }

    public static void Respawn()
    {
        Load(TargetScene, Player.Instance.Respawn);
    }
}
