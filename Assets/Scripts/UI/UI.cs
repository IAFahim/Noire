using System;
using System.Collections;
using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.Events;


/// <summary>
/// THE UI baseclass. Provides useful functions for transitioning.
/// <remarks>Only use this on parent MonoBehaviours that needs to fade in/out!
/// Otherwise it will come with unnecessary overhead</remarks>
/// 
/// Inheritors have access to
/// <code>Show: {ForceShow, ShowAfterDelay}</code>
/// Calls Activate() on the object, enables it, (or the container object if alternativeGameObject is toggled on),
/// and begins the Fade transition.
/// <code>Hide: {ForceHide, HideAfterDelay}</code>
/// Calls Deactivate() on the object, disables it, and begins the Fade out transition
/// <code>Init</code>
/// Obtains the canvasGroup and rectTransform components from the gameObject.
/// 
/// NOTE: Inheritors should selectively implement the following event methods:
/// <code>Activate</code> Called before Show()'s fade transitions. Can be toggled off with activate=false.
/// <code>Deactivate</code> Called before Hide()'s fade transitions. Can be toggled off with deactivate=false.
/// <code>LateActivate</code> Called after Show()'s fade transitions. Always Triggered.
/// <code>LateDeactivate</code> Called after Hide()'s fade transitions. Always Triggered.
/// 
/// </summary>

[RequireComponent(typeof(CanvasGroup))]
public class UI : MonoBehaviour
{
    [SerializeField] protected GameObject containerGameObject;
    protected CanvasGroup canvasGroup;
    protected RectTransform rectTransform;
    protected bool alternativeGameObject = false;
    public static readonly float animationTime = .6f;
    
    protected Coroutine fadeCoroutine;
    protected Coroutine delayedShowCoroutine;
    protected Coroutine delayedHideCoroutine;

    private bool CanAnimate => fadeCoroutine == null;
    
    protected virtual void Activate() { }
    protected virtual void Deactivate() { }
    protected virtual void LateActivate() { }
    protected virtual void LateDeactivate() { }

    protected virtual void Awake()
    {
        Init();
    }

    /// Shows an UI element by fading. If another fade coroutine is going at the same time,
    /// this operation is canceled, and Activate will not be called.
    public bool Show(bool activate=true)
    {
        if (CanAnimate)
        {
            Display(true);
            if(activate)
                Activate();
            fadeCoroutine = StartCoroutine(Fade(1));
            return true;
        }

        return false;
    }

    /// Shows an UI element by fading. Stops and overwrites any concurrent coroutines.
    public void ForceShow(bool activate=true)
    {
        if(fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);
        
        Display(true);
        if(activate)
            Activate();
        fadeCoroutine = StartCoroutine(Fade(1));
    }
    
    /// Shows an UI element by fading after `delay`
    /// Will refresh delay time if multiple calls are made before showing.
    /// <remarks>
    /// Does not work after a Hide() call,
    /// since Hide() disables the object and therefore stops all related coroutines.
    /// </remarks>
    public void ShowAfterDelay(float delay, bool activate=true, bool force=false)
    {
        if(delayedShowCoroutine != null)
            StopCoroutine(delayedShowCoroutine);
        Display(true);
        delayedShowCoroutine = StartCoroutine(DelayAndShow(delay, activate, force));
    }

    IEnumerator DelayAndShow(float delay, bool activate, bool force)
    {
        yield return new WaitForSeconds(delay);
        if (force)
            ForceShow(activate);
        else
            Show(activate);
    }
    
    /// Hides an UI element by fading. If another fade coroutine is going at the same time,
    /// this operation is canceled, and Deactivate will not be called.
    public bool Hide(bool deactivate=true)
    {
        if (CanAnimate && gameObject.activeSelf)
        {
            if(deactivate)
                Deactivate();
            fadeCoroutine = StartCoroutine(Fade(0));
            return true;
        }

        return false;
    }

    /// Hides an UI element by fading. Stops and overwrites any concurrent coroutines.
    public void ForceHide(bool deactivate=true)
    {
        if(fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);
        
        if(deactivate)
            Deactivate();
        if (gameObject.activeSelf)
            fadeCoroutine = StartCoroutine(Fade(0));
    }

    /// Hides an UI element by fading after `delay`
    /// Will refresh delay time if multiple calls are made before hiding.
    public void HideAfterDelay(float delay, bool activate=true, bool force=false)
    {
        if(delayedHideCoroutine != null)
            StopCoroutine(delayedHideCoroutine);
        delayedHideCoroutine = StartCoroutine(DelayAndHide(delay, activate, force));
    }

    IEnumerator DelayAndHide(float delay, bool activate, bool force)
    {
        yield return new WaitForSeconds(delay);
        if (force)
            ForceHide(activate);
        else
            Hide(activate);
    }
    
    private const float EPS = 0.03f;
    /// Fades the canvasGroup given an ending alpha, and evaluates it along the FADE_ANIM_CURVE. 
    /// <param name="end">The ending alpha</param>
    protected IEnumerator Fade(float end)
    {
        float init = canvasGroup.alpha;
        bool isHide = end == 0;
        
        if (Mathf.Abs(init - end) < EPS)
            goto animationEnd;
        
        float t = 0;
        while (t < animationTime)
        {
            t += Time.deltaTime;
            float eval = t / animationTime;

            canvasGroup.alpha = Mathf.Lerp(
                init, 
                end,
                isHide
                    ? StaticInfoObjects.Instance.FADEOUT.Evaluate(eval)
                    : StaticInfoObjects.Instance.FADEIN.Evaluate(eval)
            );
            yield return null;
        }
        
        animationEnd:
        {
            canvasGroup.alpha = end;

            if (isHide)
            {
                Display(false);
                LateDeactivate();
            }
            else
            {
                LateActivate();
            }

            fadeCoroutine = null;
        }
    }
    
    private void Display(bool active)
    {
        if (alternativeGameObject)
            containerGameObject.SetActive(active);
        else
            gameObject.SetActive(active);
    }

    protected void Init()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
    }
}