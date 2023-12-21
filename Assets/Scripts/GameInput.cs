// #define DEBUG

using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// Handles game input to Events, and action map rebinding 
public class GameInput : MonoBehaviour
{
    public static GameInput Instance { get; private set; }
    private InputActionRebindingExtensions.RebindingOperation m_RebindOperation;
    
    public event Action OnPlayerMenuToggle;
    public event Action OnPauseToggle;
    public event Action OnInteract;
    
    // the short is for status of charged abilities: 0 for started, 1 for performed, -1 for canceled
    public event Action<int, short> OnAbilityCast;
    public event Action OnDebugConsoleToggle;
    public event Action OnDebugConsoleExecute;
    
    public enum Bindings
    {
        Menu,
        MoveUp,
        MoveDown,
        MoveLeft,
        MoveRight,
        Interact,
        Dash,
        LightAttack,
        ChargedAttack,
        Ability1,
        Ability2,
        Ability3,
    }

    private GameInputActions gameInputActions;

    private void Awake()
    {
        gameInputActions = new GameInputActions();
        
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        // enable all maps
        gameInputActions.Player.Enable();
        gameInputActions.Menu.Enable();
        gameInputActions.UI.Enable();
        gameInputActions.Debug.Enable();
        
        // debug
        gameInputActions.Debug.ToggleDebug.performed += ToggleDebugConsole;
        gameInputActions.Debug.Execute.performed += ExecuteDebugConsole;

        // handle subscriptions
        gameInputActions.UI.Menu.performed += PlayerMenu_performed;
        gameInputActions.Menu.Pause.performed += Pause_performed;
        
        gameInputActions.Player.LightAttack.performed += Attack1_performed;
        gameInputActions.Player.ChargedAttack.performed += ChargedAttack_performed;
        gameInputActions.Player.ChargedAttack.canceled += ChargedAttack_canceled;
            
        gameInputActions.Player.Interact.performed += Interact_performed;
        gameInputActions.Player.Dash.performed += Dash_performed;
        
        gameInputActions.Player.Ability1.performed += Ability1_performed;
        gameInputActions.Player.Ability2.performed += Ability2_performed;
        gameInputActions.Player.Ability3.performed += Ability3_performed;
        
        LoadUserRebinds();
    }

    private void Start()
    {
        // game states
        GameEventsManager.Instance.GameStateEvents.OnPauseToggle += TogglePause;
        GameEventsManager.Instance.GameStateEvents.OnLoadToggle += ToggleLoad;
        GameEventsManager.Instance.GameStateEvents.OnUIToggle += ToggleUI;
        GameEventsManager.Instance.GameStateEvents.OnMenuToggle += ToggleMenu;
    }

    private void OnDisable()
    {
        SaveUserRebinds();
        
        // debug
        gameInputActions.Debug.ToggleDebug.performed -= ToggleDebugConsole;
        gameInputActions.Debug.Execute.performed -= ExecuteDebugConsole;
        
        gameInputActions.UI.Menu.performed -= PlayerMenu_performed;
        gameInputActions.Menu.Pause.performed -= Pause_performed;
        
        gameInputActions.Player.LightAttack.performed -= Attack1_performed;
        gameInputActions.Player.ChargedAttack.performed -= ChargedAttack_performed;
        gameInputActions.Player.ChargedAttack.canceled -= ChargedAttack_canceled;
        
        gameInputActions.Player.Interact.performed -= Interact_performed;
        gameInputActions.Player.Dash.performed -= Dash_performed;
        
        gameInputActions.Player.Ability1.performed -= Ability1_performed;
        gameInputActions.Player.Ability2.performed -= Ability2_performed;
        gameInputActions.Player.Ability3.performed -= Ability3_performed;
        
        // game states
        GameEventsManager.Instance.GameStateEvents.OnPauseToggle -= TogglePause;
        GameEventsManager.Instance.GameStateEvents.OnLoadToggle -= ToggleLoad;
        GameEventsManager.Instance.GameStateEvents.OnUIToggle -= ToggleUI;
        GameEventsManager.Instance.GameStateEvents.OnMenuToggle -= ToggleMenu;

        // dispose all maps
        gameInputActions.Dispose();
    }

    #region TOGGLES
    
    // toggles UI and player functions. called when inside pause menu.
    private void TogglePause(bool paused)
    {
        if (paused)
        {
            gameInputActions.UI.Disable();
            gameInputActions.Player.Disable();
        }
        else
        {
            gameInputActions.UI.Enable();
            gameInputActions.Player.Enable();
        }
    }

    // toggles all input. called when loading scenes
    private void ToggleLoad(bool finished)
    {
        if (finished)
        {
            gameInputActions.Player.Enable();
            gameInputActions.Menu.Enable();
            gameInputActions.UI.Enable();
        }
        else
        {
            gameInputActions.Player.Disable();
            gameInputActions.Menu.Disable();
            gameInputActions.UI.Disable();
        }
    }
    
    public void ToggleAll(bool enable)
    {
        if (enable)
        {
            gameInputActions.Enable();
        }
        else
        {
            gameInputActions.Disable();
        }
    }
    
    // toggles player commands. called when in dialogues or UIs
    private void ToggleUI(bool isToggled)
    {
        if (isToggled)
        {
            gameInputActions.Player.Disable();
        }
        else
        {
            gameInputActions.Player.Enable();
        }
    }
    
    // toggles [esc] and [m]
    private void ToggleMenu(bool isToggled)
    {
        if (isToggled)
        {
            gameInputActions.Menu.Disable();
            gameInputActions.UI.Disable();
        }
        else
        {
            gameInputActions.Menu.Enable();
            gameInputActions.UI.Enable();
        }
    }

    
    // toggles the debug console. Disables all.
    private bool debug = false;
    private void OnToggleDebugConsole()
    {
        debug = !debug;
        OnDebugConsoleToggle?.Invoke();
        ToggleLoad(!debug);
    }
    #endregion

    #region INVOKE EVENTS
    
    private void ToggleDebugConsole(InputAction.CallbackContext obj) => OnToggleDebugConsole();
    
    private void ExecuteDebugConsole(InputAction.CallbackContext obj) => OnDebugConsoleExecute?.Invoke();
    
    private void PlayerMenu_performed(InputAction.CallbackContext obj) => OnPlayerMenuToggle?.Invoke();
    
    private void Pause_performed(InputAction.CallbackContext obj) => OnPauseToggle?.Invoke();

    private void Interact_performed(InputAction.CallbackContext obj) => OnInteract?.Invoke();
    
    private void Attack1_performed(InputAction.CallbackContext obj)
    {
        OnAbilityCast?.Invoke(0, 0);
    }
    
    private void Dash_performed(InputAction.CallbackContext obj)
    {
        OnAbilityCast?.Invoke(1, 0);
    }

    private void Ability1_performed(InputAction.CallbackContext obj)
    {
        OnAbilityCast?.Invoke(2, 0);
    }
    
    private void Ability2_performed(InputAction.CallbackContext obj)
    {
        OnAbilityCast?.Invoke(3, 0);
    }
    
    private void Ability3_performed(InputAction.CallbackContext obj)
    {
        OnAbilityCast?.Invoke(4, 0);
    }

    private void ChargedAttack_performed(InputAction.CallbackContext obj)
    {
        OnAbilityCast?.Invoke(5, 0);
    }

    private void ChargedAttack_canceled(InputAction.CallbackContext obj)
    {
        OnAbilityCast?.Invoke(5, -1);
    }
    
    public float GetZoomVal()
    {
        return gameInputActions.Player.CameraZoom.ReadValue<float>();
    }

    public Vector3 GetMovementVectorNormalized()
    {
        Vector2 readVal = gameInputActions.Player.Move.ReadValue<Vector2>();
        return new Vector3(readVal.x, 0, readVal.y).normalized;
    }

    public bool IsShiftModifierOn()
    {
        return gameInputActions.Player.ShiftModifier.IsPressed();
    }
    #endregion

    #region BINDINGS
    
    public string GetBindingText(Bindings binding)
    {
        switch (binding)
        {
            default:
            case Bindings.MoveUp:
                return gameInputActions.Player.Move.bindings[1].ToDisplayString();
            case Bindings.MoveDown:
                return gameInputActions.Player.Move.bindings[2].ToDisplayString();
            case Bindings.MoveLeft:
                return gameInputActions.Player.Move.bindings[3].ToDisplayString();
            case Bindings.MoveRight:
                return gameInputActions.Player.Move.bindings[4].ToDisplayString();
            case Bindings.LightAttack:
                return gameInputActions.Player.LightAttack.bindings[0].ToDisplayString();
            case Bindings.ChargedAttack:
                return gameInputActions.Player.ChargedAttack.bindings[0].ToDisplayString();
            case Bindings.Dash:
                return gameInputActions.Player.Dash.bindings[0].ToDisplayString();
            case Bindings.Interact:
                return gameInputActions.Player.Interact.bindings[0].ToDisplayString();
            case Bindings.Ability1:
                return gameInputActions.Player.Ability1.bindings[0].ToDisplayString();
            case Bindings.Ability2:
                return gameInputActions.Player.Ability2.bindings[0].ToDisplayString();
            case Bindings.Ability3:
                return gameInputActions.Player.Ability3.bindings[0].ToDisplayString();
            case Bindings.Menu:
                return gameInputActions.UI.Menu.bindings[0].ToDisplayString();
        }
    }

    /// Finds the input action and binding index associated with the `binding` 
    private void FindActionAndIndex(Bindings binding, out InputAction inputAction, out int bindingIndex)
    {
        switch (binding)
        {
            default:
            case Bindings.MoveUp:
                inputAction = gameInputActions.Player.Move;
                bindingIndex = 1;
                break;
            case Bindings.MoveDown:
                inputAction = gameInputActions.Player.Move;
                bindingIndex = 2;
                break;
            case Bindings.MoveLeft:
                inputAction = gameInputActions.Player.Move;
                bindingIndex = 3;
                break;
            case Bindings.MoveRight:
                inputAction = gameInputActions.Player.Move;
                bindingIndex = 4;
                break;
            case Bindings.LightAttack:
                inputAction = gameInputActions.Player.LightAttack;
                bindingIndex = 0;
                break;
            case Bindings.ChargedAttack:
                inputAction = gameInputActions.Player.ChargedAttack;
                bindingIndex = 0;
                break;
            case Bindings.Dash:
                inputAction = gameInputActions.Player.Dash;
                bindingIndex = 0;
                break;
            case Bindings.Interact:
                inputAction = gameInputActions.Player.Interact;
                bindingIndex = 0;
                break;
            case Bindings.Ability1:
                inputAction = gameInputActions.Player.Ability1;
                bindingIndex = 0;
                break;
            case Bindings.Ability2:
                inputAction = gameInputActions.Player.Ability2;
                bindingIndex = 0;
                break;
            case Bindings.Ability3:
                inputAction = gameInputActions.Player.Ability3;
                bindingIndex = 0;
                break;
        }

    }
    
    /// Rebinds a `binding` interactively by specifying two callbacks upon state change of the operation 
    /// <param name="binding">The binding</param>
    /// <param name="onActionRebound">Called upon operation is canceled or completed</param>
    /// <param name="onDuplicateFound">Called upon duplicate found</param>
    /// <param name="onInvalidBinding">Called upon finding invalid bindings (mouse is not valid for example)</param>
    public void RebindBinding(
        Bindings binding, 
        Action onActionRebound, 
        Action onDuplicateFound, 
        Action onInvalidBinding)
    {
        FindActionAndIndex(binding, out InputAction inputAction, out int bindingIndex);
        InteractiveRebind(inputAction, bindingIndex, onActionRebound, onDuplicateFound, onInvalidBinding);
    }

    private void InteractiveRebind(
        InputAction action, 
        int bindingIndex, 
        Action onActionRebound, 
        Action onDuplicateFound,
        Action onInvalidBinding)
    {
        m_RebindOperation?.Cancel();

        void CleanUp()
        {
            m_RebindOperation?.Dispose();
            m_RebindOperation = null;
        }
        
        m_RebindOperation = action.PerformInteractiveRebinding(bindingIndex)
            .WithCancelingThrough("<Keyboard>/escape")
            .WithTimeout(10)
            .OnCancel(operation =>
            {
                onActionRebound();
                CleanUp();
            })
            .OnComplete(operation =>
            {
                if (!CheckForInvalidBindings(action, bindingIndex))
                {
                    action.RemoveBindingOverride(bindingIndex);
                    onInvalidBinding();
                    CleanUp();
                    InteractiveRebind(action, bindingIndex, onActionRebound, onDuplicateFound, onInvalidBinding);
                }
                else if (CheckForDuplicateBindings(action, bindingIndex))
                {
                    action.RemoveBindingOverride(bindingIndex);
                    onDuplicateFound();
                    CleanUp();
                    InteractiveRebind(action, bindingIndex, onActionRebound, onDuplicateFound, onInvalidBinding);
                }
                else
                {
                    onActionRebound();
                    CleanUp();
                }
            })
            .Start();
    }

    /// checks if the binding contains `mouse`, which is invalid
    private bool CheckForInvalidBindings(InputAction action, int bindingIndex)
    {
        InputBinding newBinding = action.bindings[bindingIndex];
        if (newBinding.effectivePath.Contains("Mouse"))
        {
#if DEBUG
            Debug.Log("Invalid binding Found" + newBinding.effectivePath);
#endif
            return false;
        }

        return true;
    }
    
    /// Checks if the binding overwrites any existing ones
    /// <param name="action">The input action</param>
    /// <param name="bindingIndex">The binding index of the particular key</param>
    private bool CheckForDuplicateBindings(InputAction action, int bindingIndex)
    {
        InputBinding newBinding = action.bindings[bindingIndex];
        
        foreach (InputBinding oldBinding in action.actionMap.bindings)
        {
            if (oldBinding.action == newBinding.action)
                continue;
            if (oldBinding.effectivePath == newBinding.effectivePath)
            {
#if DEBUG
                Debug.Log("Duplicate Binding Found" + newBinding.effectivePath);
#endif
                return true;
            }
        }

        return false;
    }

    public void ResetAllBindings()
    {
        gameInputActions.RemoveAllBindingOverrides();
    }

    /// Resets the binding associated with the action and binding index.
    /// Swaps out any duplicate bindings found. 
    public void ResetBinding(InputAction action, int bindingIndex)
    {
        InputBinding newBinding = action.bindings[bindingIndex];
        string oldOverridePath = newBinding.overridePath;
        
        action.RemoveBindingOverride(bindingIndex);

        foreach (var otherAction in action.actionMap.actions)
        {
            if (otherAction == action)
                continue;

            for (int i = 0; i < otherAction.bindings.Count; i++)
            {
                InputBinding binding = otherAction.bindings[i];
                if(binding.overridePath == newBinding.path)
                    otherAction.ApplyBindingOverride(i, oldOverridePath);
            }
        }
    }
    
    void SaveUserRebinds()
    {
        var rebinds = gameInputActions.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString("rebinds", rebinds);
    }
 
    void LoadUserRebinds()
    {
        var rebinds = PlayerPrefs.GetString("rebinds");
        gameInputActions.LoadBindingOverridesFromJson(rebinds);
    }
    #endregion
}