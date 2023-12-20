using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Button))]
public class ButtonUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    private Button button;
    
    [Header("Text")]
    [SerializeField] private Color textColorTransparent;
    [SerializeField] private Color textColorOpaque;
    [SerializeField] public TextMeshProUGUI buttonText;
    
    [Header("Text Fade Animation")]
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
    
    [Header("Indicators and Animations")]
    [SerializeField] private CanvasGroup leftIndicator;
    [SerializeField] private CanvasGroup rightIndicator;
    [SerializeField] private RectTransform leftHorizontalLine;
    [SerializeField] private RectTransform rightHorizontalLine;
    [SerializeField] private RectTransform leftVerticalLine;
    [SerializeField] private RectTransform rightVerticalLine;
    [SerializeField] private float maxLineWidthH = 400f;
    [SerializeField] private float maxLineWidthV = 100f;
    private Vector2 origLeftSizeH;
    private Vector2 origRightSizeH;
    private Vector2 origLeftSizeV;
    private Vector2 origRightSizeV;
    private bool canTriggerIndicatorAnimations;

    private void Awake()
    {
        button = GetComponent<Button>();
        canTriggerIndicatorAnimations = leftIndicator; // only trigger indicat or if left indicator is not null
    }

    private void Start()
    {
        initialScale = transform.localScale;
        endScale = initialScale * scaleAmount;

        if (canTriggerIndicatorAnimations)
        {
            origLeftSizeH = leftHorizontalLine.sizeDelta;
            origRightSizeH = rightHorizontalLine.sizeDelta;
            origLeftSizeV = leftVerticalLine.sizeDelta;
            origRightSizeV = rightVerticalLine.sizeDelta;
        }

        SetIndicatorAlphas(0);
        button.onClick.AddListener(() =>
        {
            transform.localScale = initialScale;
            AudioManager.Instance.PlayOnClick();
            if (canTriggerIndicatorAnimations)
                StartCoroutine(ScaleLinesOnClick());
        });
    }

    public void Disable(bool setTransparent = true)
    {
        button.interactable = false;
        if(setTransparent)
            buttonText.color = textColorTransparent;
        
        if (scaleOnSelect != null)
            StopCoroutine(scaleOnSelect);
        if (textAlphaCycleOnSelect != null)
            StopCoroutine(textAlphaCycleOnSelect);
    }

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
    
    IEnumerator InvokeCall(Action call, Action immediateCall)
    {
        immediateCall?.Invoke();
        yield return new WaitForSeconds(.2f);
        call?.Invoke();
    }

    public void SetText(string text)
    {
        buttonText.text = text;
    }

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
        float t = 0;
        
        var randomMaxLineWidthL = Random.Range(maxLineWidthH, maxLineWidthH + 30);
        var randomMaxLineWidthR = Random.Range(maxLineWidthH, maxLineWidthH + 30);
        var randomMaxLineHeightL = Random.Range(maxLineWidthV, maxLineWidthV + 30);
        var randomMaxLineHeightR = Random.Range(maxLineWidthV, maxLineWidthV + 30);

        var hlh = leftHorizontalLine.rect.height;
        var hrh = rightHorizontalLine.rect.height;

        var vlw = leftVerticalLine.rect.width;
        var vrw = rightVerticalLine.rect.width;

        var animTime = UI.animationTime - 0.05f;
        
        while (t < animTime)
        {
            t += Time.deltaTime;
            float eval = t / animTime;
            
            // horizontal lines scale
            leftHorizontalLine.sizeDelta = new Vector2(
                Mathf.Lerp(leftHorizontalLine.rect.width, randomMaxLineWidthL, eval), 
                hlh);
            rightHorizontalLine.sizeDelta = new Vector2(
                Mathf.Lerp(rightHorizontalLine.rect.width, randomMaxLineWidthR, eval), 
                hrh);
            
            // vertical lines scale
            leftVerticalLine.sizeDelta = new Vector2(
                vlw, 
                Mathf.Lerp(leftVerticalLine.rect.height, randomMaxLineHeightL, eval));
            rightVerticalLine.sizeDelta = new Vector2(
                vrw, 
                Mathf.Lerp(rightVerticalLine.rect.height, randomMaxLineHeightR, eval));
            
            yield return null;
        }
        
        SetIndicatorAlphas(0);
    }

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
        
        ResetIndicators();
        
        if (scaledownOnDeSelect != null)
        {
            StopCoroutine(scaledownOnDeSelect);
            scaledownOnDeSelect = null;
        }

        scaleOnSelect = StartCoroutine(ScaleSelection(true));
        textAlphaCycleOnSelect = StartCoroutine(TextAlphaCycle());
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (!button.interactable || scaledownOnDeSelect != null)
            return;
        
        if (scaleOnSelect != null)
        {
            StopCoroutine(scaleOnSelect);
            scaleOnSelect = null;
        }

        scaledownOnDeSelect = StartCoroutine(ScaleSelection(false));

        if (textAlphaCycleOnSelect != null)
        {
            StopCoroutine(textAlphaCycleOnSelect);
            buttonText.alpha = 1;
        }
    }

    private void SetIndicatorAlphas(float alpha)
    {
        if (canTriggerIndicatorAnimations)
        {
            leftIndicator.alpha = alpha;
            rightIndicator.alpha = alpha;
        }
    }

    private void ResetIndicators()
    {
        if (canTriggerIndicatorAnimations)
        {
            leftHorizontalLine.sizeDelta = origLeftSizeH;
            rightHorizontalLine.sizeDelta = origRightSizeH;
            leftVerticalLine.sizeDelta = origLeftSizeV;
            rightVerticalLine.sizeDelta = origRightSizeV;
        }
    }
}