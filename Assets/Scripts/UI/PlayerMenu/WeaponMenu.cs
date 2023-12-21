using System;
using UnityEngine;

public class WeaponMenu : UI
{
    public static WeaponMenu Instance { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        Instance = this;
    }

    private void Start()
    {
        Hide();
    }
}