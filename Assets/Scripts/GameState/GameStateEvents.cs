using System;

public class GameStateEvents
{
    public event Action<bool> OnPauseToggle;
    public void PauseToggle(bool paused) => OnPauseToggle?.Invoke(paused);

    public event Action<bool> OnLoadToggle;
    public void LoadToggle(bool finished) => OnLoadToggle?.Invoke(finished);
    
    public event Action<bool, bool> OnUIToggle;
    public void UIToggle(bool isToggled, bool triggerBlur=true) => OnUIToggle?.Invoke(isToggled, triggerBlur);
    
    public event Action<bool> OnMenuToggle;
    public void MenuToggle(bool isToggled) => OnMenuToggle?.Invoke(isToggled);
}