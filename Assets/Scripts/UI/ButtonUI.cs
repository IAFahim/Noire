using System;
using System.Collections;
using TetraCreations.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

/// <summary>
/// The wrapper class on Unity Button.
/// Provides API for enable, disable, add/remove listeners.
/// Implements animations on select, hover, and click. 
/// </summary>

[RequireComponent(typeof(Button))]
public class ButtonUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    private Button button;
    
    [Header("Text")]
    [SerializeField] private Color textColorTransparent;
    [SerializeField] private Color textColorOpaque;
    [SerializeField] public TextMeshProUGUI buttonText;

    [Header("Text Fade Animation")] 
    private bool hasText;
    private readonly float textAlphaPeriod = 1.5f;
    private readonly float textAlphaMultiplier = 0.4f;
    private Coroutine textAlphaCycleOnSelect;
    
    [Header("Button Scale")]
    private readonly float scaleAmount = 1.1f;
    private readonly float scaleAnimationTime = .1f;
    private Coroutine scaleOnSelect;
    private Coroutine scaledownOnDeSelect;
    private Vector3 initialScale;
    private Vector3 endScale;
    
    [Header("Indicators")]
    [SerializeField] private CanvasGroup leftIndicator;
    [SerializeField] private CanvasGroup rightIndicator;
    
    [Header("Scale Line Animations")]
    [SerializeField] public bool scaleLines = true; // toggle enable scaleLine animations
    [DrawIf(nameof(scaleLines), true)] [SerializeField] private RectTransform leftHorizontalLine;
    [DrawIf(nameof(scaleLines), true)] [SerializeField] private RectTransform rightHorizontalLine;
    [DrawIf(nameof(scaleLines), true)] [SerializeField] private RectTransform leftVerticalLine;
    [DrawIf(nameof(scaleLines), true)] [SerializeField] private RectTransform rightVerticalLine;
    [DrawIf(nameof(scaleLines), true)] [SerializeField] private float WOffset = 30f;
    [DrawIf(nameof(scaleLines), true)] [SerializeField] private float HOffset = 30f;
    private Vector2 origLeftSizeH;
    private Vector2 origRightSizeH;
    private Vector2 origLeftSizeV;
    private Vector2 origRightSizeV;
    private bool canTriggerIndicatorAnimations;
    
    private void Awake()
    {
        button = GetComponent<Button>();
        canTriggerIndicatorAnimations = leftIndicator; // only trigger indicat or if left indicator is not null
        hasText = buttonText;
    }

    private void Start()
    {
        initialScale = transform.localScale;
        endScale = initialScale * scaleAmount;

        if (scaleLines)
        {
            origLeftSizeH = leftHorizontalLine.sizeDelta;
            origRightSizeH = rightHorizontalLine.sizeDelta;
            origLeftSizeV = leftVerticalLine.sizeDelta;
            origRightSizeV = rightVerticalLine.sizeDelta;
        }

        SetIndicatorAlphas(0);
        AddDefaultListeners();
    }
    
    #region API
    
    /// Disables a button.  
    /// <param name="setTransparent">whether to set the text transparent</param>
    public void Disable(bool setTransparent = true)
    {
        button.interactable = false;
        if(setTransparent)
            buttonText.color = textColorTransparent;
        
        if (scaleOnSelect != null)
            StopCoroutine(scaleOnSelect);
        if (hasText && textAlphaCycleOnSelect != null)
            StopCoroutine(textAlphaCycleOnSelect);
    }

    /// Toggles a button to be interactive, set its color to textColorOpaque 
    public void Enable()
    {
        button.interactable = true;
        buttonText.color = textColorOpaque;
        
        SetIndicatorAlphas(0);
    }
    
    /// Adds a listener to the Button. The Call is invoked after a slight delay.
    /// Can also specify another call which is triggered immediately on click.
    /// <param name="call">The delayed call</param>
    /// <param name="immediateCall">The immediate call</param>
    public void AddListener(Action call, Action immediateCall=null)
    {
        button.onClick.AddListener(() => StartCoroutine(InvokeCall(call, immediateCall)));
    }
    
    /// Removes all *added* listeners to the button.
    /// <remarks>Default listeners include line scale animations, onclick/hover audio, etc</remarks> 
    /// <param name="removeDefault">if true, EVERY listener will be removed</param>
    public void RemoveAllListeners(bool removeDefault=false)
    {
        button.onClick.RemoveAllListeners();
        if (!removeDefault)
            AddDefaultListeners();
    }

    /// Sets the button text
    /// <param name="text">The text to set to the button</param>
    public void SetText(string text)
    {
        buttonText.text = text;
    }
    
    #endregion API
    
    #region ANIMATION COROUTINES
    
    /// animates a scaled up effect on the button
    private IEnumerator ScaleSelection(bool startAnimation)
    {
        float time = 0;
        
        if (!startAnimation) // scale down animation
        {
            SetIndicatorAlphas(0);

            while (time < scaleAnimationTime)
            {
                time += Time.deltaTime;
                
                float eval = time / scaleAnimationTime;
                transform.localScale = Vector3.Lerp(transform.localScale, initialScale, eval);
                
                yield return null;
            }
            scaledownOnDeSelect = null;
        }
        else
        {
            SetIndicatorAlphas(1);
        
            while (time < scaleAnimationTime)
            {
                time += Time.deltaTime;
                
                float eval = time / scaleAnimationTime;
                transform.localScale = Vector3.Lerp(transform.localScale, endScale, eval);
                
                yield return null;
            }
            scaleOnSelect = null;
        }
    }

    private IEnumerator TextAlphaCycle()
    {
        float t = 0;
        while (true)
        {
            t += Time.deltaTime / textAlphaPeriod;
            t %= 1;
            buttonText.alpha = Mathf.Lerp(1, textAlphaMultiplier, StaticInfoObjects.Instance.TEXT_ALPHA_CYCLE_CURVE.Evaluate(t));
            yield return null;
        }
    }

    /// animates a scaled up effect on the width of the indicator lines on click
    private IEnumerator ScaleLinesOnClick()
    {
        if (!scaleLines)
            yield break;
        
        float t = 0;

        var rect1 = leftHorizontalLine.rect;
        var rect2 = rightHorizontalLine.rect;
        var rect3 = leftVerticalLine.rect;
        var rect4 = rightVerticalLine.rect;

        var rect1W = rect1.width;
        var rect1H = rect1.height;

        var rect2W = rect2.width;
        var rect2H = rect2.height;

        var rect3W = rect3.width;
        var rect3H = rect3.height;
        
        var rect4W = rect4.width;
        var rect4H = rect4.height;
        
        var rect1RandW = Random.Range(rect1W, rect1W + WOffset);
        var rect2RandW = Random.Range(rect2W, rect2W + WOffset);
        var rect3RandH = Random.Range(rect3H, rect3H + HOffset);
        var rect4RandH = Random.Range(rect4H, rect4H + HOffset);

        var animTime = UI.animationTime - 0.05f;
        
        while (t < animTime)
        {
            t += Time.deltaTime;
            float eval = t / animTime;
            
            // horizontal lines scale
            leftHorizontalLine.sizeDelta = new Vector2(
                Mathf.Lerp(leftHorizontalLine.rect.width, rect1RandW, eval), 
                rect1H);
            rightHorizontalLine.sizeDelta = new Vector2(
                Mathf.Lerp(rightHorizontalLine.rect.width, rect2RandW, eval), 
                rect2H);
            
            // vertical lines scale
            leftVerticalLine.sizeDelta = new Vector2(
                rect3W, 
                Mathf.Lerp(leftVerticalLine.rect.height, rect3RandH, eval));
            rightVerticalLine.sizeDelta = new Vector2(
                rect4W, 
                Mathf.Lerp(rightVerticalLine.rect.height, rect4RandH, eval));
            
            yield return null;
        }
        
        SetIndicatorAlphas(0);
    }
    
    #endregion ANIMATION COROUTINES
    
    #region IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button.interactable)
        {
            eventData.selectedObject = gameObject;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (button.interactable)
        {
            eventData.selectedObject = null;
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (!button.interactable || scaleOnSelect != null)
            return;
        
        ResetLines();
        DoSelect();
        if(hasText)
            textAlphaCycleOnSelect = StartCoroutine(TextAlphaCycle());
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (!button.interactable || scaledownOnDeSelect != null)
            return;
        
        DoDeselect();

        if (hasText && textAlphaCycleOnSelect != null)
        {
            StopCoroutine(textAlphaCycleOnSelect);
            buttonText.alpha = 1;
        }
    }

    private void DoSelect()
    {
        if (scaledownOnDeSelect != null)
        {
            StopCoroutine(scaledownOnDeSelect);
            scaledownOnDeSelect = null;
        }

        scaleOnSelect = StartCoroutine(ScaleSelection(true));
    }

    private void DoDeselect()
    {
        if (scaleOnSelect != null)
        {
            StopCoroutine(scaleOnSelect);
            scaleOnSelect = null;
        }

        scaledownOnDeSelect = StartCoroutine(ScaleSelection(false));
    }
    
    #endregion IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler

    #region HELPERS
    private void SetIndicatorAlphas(float alpha)
    {
        if (canTriggerIndicatorAnimations)
        {
            leftIndicator.alpha = alpha;
            rightIndicator.alpha = alpha;
        }
    }

    private void ResetLines()
    {
        if (scaleLines)
        {
            leftHorizontalLine.sizeDelta = origLeftSizeH;
            rightHorizontalLine.sizeDelta = origRightSizeH;
            leftVerticalLine.sizeDelta = origLeftSizeV;
            rightVerticalLine.sizeDelta = origRightSizeV;
        }
    }
    
    IEnumerator InvokeCall(Action call, Action immediateCall)
    {
        immediateCall?.Invoke();

        if (call != null)
        {
            yield return new WaitForSeconds(.2f);
            call.Invoke();
        }
    }
    
    private void AddDefaultListeners()
    {
        button.onClick.AddListener(() =>
        {
            transform.localScale = initialScale;
            AudioManager.Instance.PlayOnClick();
            if (canTriggerIndicatorAnimations)
                StartCoroutine(ScaleLinesOnClick());
        });
    }
    #endregion
}
