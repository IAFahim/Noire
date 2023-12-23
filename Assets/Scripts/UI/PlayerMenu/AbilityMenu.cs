using System;
using UnityEngine;

public class AbilityMenu : UI
{
    public static AbilityMenu Instance { get; private set; }
    
    protected override void Awake()
    {
        base.Awake();
        Instance = this;
    }

    private void Start()
    { 
        Hide(false);
    }
}