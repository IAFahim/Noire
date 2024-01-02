using UnityEngine;

public class KeepOnLoad : MonoBehaviour
{
    public static KeepOnLoad Instance;

    [SerializeField] public GameObject managers;
    [SerializeField] private GameObject[] allChildren;
    
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

    public void EnableOnlyManager()
    {
        foreach (var child in allChildren)
            child.SetActive(false);
        
        managers.SetActive(true);
    }

    public void EnableAll()
    {
        foreach (var child in allChildren)
            child.SetActive(true);
    }
}