using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class UIManager: MonoBehaviour
{
    public static UIManager Instance;

    /// **explicitly** managed in UI scripts
    /// Used to determine which context [esc] leads to
    public static GameObject CurrentContext;
    
    [Header("Title text")]
    [SerializeField] private TextMeshProUGUI sceneTitle;
    [SerializeField] private CanvasGroup mainCanvas;
    [SerializeField] private AnimationCurve titleIntensityCurve;
    [SerializeField] private AnimationCurve UIIntensityCurve;
    private float titleAnimationTime = 3;

    [SerializeField] private GameObject deathText;
    
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
        DisableDeathText();
    }

    public void DisableDeathText() => deathText.gameObject.SetActive(false);
    public void EnableDeathText() => deathText.gameObject.SetActive(true);
    
    public void DisplaySceneName(string name)
    {
        sceneTitle.text = name;
        StartCoroutine(DisplaySceneNameCoroutine());
    }
    
    private IEnumerator DisplaySceneNameCoroutine()
    {
        GameEventsManager.Instance.GameStateEvents.MenuToggle(true);
        
        sceneTitle.gameObject.SetActive(false);
        mainCanvas.alpha = 0;
        yield return new WaitForSeconds(1);
        
        AudioManager.Instance.PlaySceneBegins();
        
        sceneTitle.gameObject.SetActive(true);
        float time = 0;
        while (time < 1)
        {
            sceneTitle.alpha = Mathf.Lerp(1, 0, titleIntensityCurve.Evaluate(time));
            time += Time.deltaTime / titleAnimationTime;
            yield return null;
        }
        sceneTitle.gameObject.SetActive(false);
        
        time = 0;
        while (time < 1)
        {
            mainCanvas.alpha = Mathf.Lerp(0, 1, UIIntensityCurve.Evaluate(time));
            time += Time.deltaTime;
            yield return null;
        }
        mainCanvas.alpha = 1;
        
        GameEventsManager.Instance.GameStateEvents.MenuToggle(false);
    }

}