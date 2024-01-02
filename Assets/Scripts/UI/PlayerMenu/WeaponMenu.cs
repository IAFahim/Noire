using System;
using UnityEngine;

public class WeaponMenu : UI
{
    public static WeaponMenu Instance { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        
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
        Hide(false);
    }
}