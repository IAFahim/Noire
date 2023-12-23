using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

/// <summary>
/// A parent scene controller supports virtual implementations of the following functions:
/// <code>Init</code> Called in Awake()
/// <code>LateInit</code> Called in Start(), This is called after DataPersistence!
/// </summary>

public class ParentSceneController : MonoBehaviour
{
    [SerializeField] private string sceneName;
    
    [Header("Interactable Objects")]
    [SerializeField] protected InteractableObject[] unaffectedInteractableObjects;
    
    [Header("Audio")]
    [SerializeField] protected FMODUnity.EventReference bgmAudioEvent;
    
    protected List<InteractableObject> interactablesList;

    private void Awake()
    {
        SceneManager.sceneLoaded += FindAllInteractables;
        Init();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= FindAllInteractables;
    }

    private void Start()
    {
        foreach (var obj in unaffectedInteractableObjects)
            obj.Enable();
        UIManager.Instance.DisplaySceneName(sceneName);
        LateInit();
    }

    protected virtual void Init() { }

    protected virtual void LateInit() { }
    
    protected void ToggleAllInteractables(bool active)
    {
        if (active)
            foreach (var interactable in interactablesList)
                interactable.Enable();
        else
            foreach (var interactable in interactablesList)
                interactable.Disable();
    }
    
    protected void FindAllInteractables(Scene scene, LoadSceneMode mode)
    {
        interactablesList = FindObjectsOfType<MonoBehaviour>(true)
            .OfType<InteractableObject>()
            .Except(unaffectedInteractableObjects)
            .ToList();
    }
}