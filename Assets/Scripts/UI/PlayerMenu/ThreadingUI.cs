using System;
using UnityEngine;

public class ThreadingUI : UI
{
    public static ThreadingUI Instance { get; private set; }
    
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