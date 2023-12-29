using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// class for storing all CONST fields such as static dictionaries, animation curves, etc
///
/// Also provides useful access functions to them
/// </summary>


public class StaticInfoObjects : MonoBehaviour
{
    public static StaticInfoObjects Instance { get; private set; }

    [SerializeField] public AnimationCurve FADEIN; // UI fade animation curves
    [SerializeField] public AnimationCurve FADEOUT; // UI fade animation curves
    
    [SerializeField] public AnimationCurve CA_DEATH_CURVE; // chromatic aberration curve
    [SerializeField] public AnimationCurve LD_DEATH_CURVE; // lens distortion curve
    
    [SerializeField] public AnimationCurve CA_QUICK_IMPULSE; // chromatic aberration curve
    [SerializeField] public AnimationCurve LD_QUICK_IMPULSE; // lens distortion curve

    [SerializeField] public AnimationCurve TEXT_ALPHA_CYCLE_CURVE; // button on hover alpha cycle
    
    // reverse mapping from string to GameScene enum
    private static Dictionary<string, GameScene> GAMESCENES = new();

    public static GameScene ToGameScene(string scene) => GAMESCENES[scene];
    public static GameScene ToGameScene(Scene scene) => GAMESCENES[scene.name];
    public static SceneInfo GetSceneInfo(GameScene scene) => LOADING_INFO[scene];
    public static bool Match(string scene1, GameScene scene2) => GAMESCENES.TryGetValue(scene1, out GameScene val) 
                                                          && val == scene2;

    public static bool Match(Scene scene1, GameScene scene2) => Match(scene1.name, scene2);

    public static SceneType GetSceneType(string scene) => LOADING_INFO[GAMESCENES[scene]].Type;
    
    public static Color GetVoronoiColor(DreamState dreamState) => VORONOI_INDICATOR[dreamState];
    
    private void Awake()
    {
        if (Instance != null) 
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        var allScenes = Enum.GetValues(typeof(GameScene));
        foreach (var scene in allScenes)
            GAMESCENES.Add(scene.ToString(), (GameScene)scene);
    }

    // maps scene -> (load type, load mode, initial position)
    private static readonly Dictionary<GameScene, SceneInfo> LOADING_INFO = new()
    {
        { GameScene.MainMenuScene, new SceneInfo(SceneType.Single, null) },
        { GameScene.LoadingScene, new SceneInfo(SceneType.Single, null) },
        
        { GameScene.BedrockPlains, new SceneInfo(SceneType.Single, null) },
        
        { GameScene.SoluraBase, new SceneInfo(SceneType.Parent, GameScene.SoluraEntry) },
        { GameScene.SoluraEntry, new SceneInfo(SceneType.Child, GameScene.SoluraBase) },
        { GameScene.SoluraCliffHouses, new SceneInfo(SceneType.Child, GameScene.SoluraBase) },
        { GameScene.SoluraDam, new SceneInfo(SceneType.Child, GameScene.SoluraBase)},
        { GameScene.SoluraBirthday, new SceneInfo(SceneType.Child, GameScene.SoluraBase)},
        
        { GameScene.TheShorelines, new SceneInfo(SceneType.Single, null) },
    };

    private static readonly Dictionary<DreamState, Color> VORONOI_INDICATOR = new()
    {
        { DreamState.Neutral, Color.black },
        { DreamState.Lucid, Color.cyan },
        { DreamState.Deep, Color.magenta },
    };
}