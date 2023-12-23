using System;
using TMPro;
using UnityEngine;

public class WarningText : UI
{
    public static WarningText Instance;

    [SerializeField] private TextMeshProUGUI displayText;

    protected override void Awake()
    {
        base.Awake();
        Instance = this;
    }

    private void Start()
    {
        Hide();
    }

    public void ShowPopup(float hideDelay, string text)
    {
        // TODO: play warning sound
        displayText.text = text;
        ForceShow();
        HideAfterDelay(hideDelay);
    }
}