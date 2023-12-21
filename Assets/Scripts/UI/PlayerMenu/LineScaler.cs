using System;
using System.Collections;
using System.Linq;
using TetraCreations.Attributes;
using UnityEngine;

public class LineScaler : MonoBehaviour
{
    // REQUIRED: all these lines have height=2. We scaling width only!
    [SerializeField] private bool scaleAllChildren;
    [SerializeField] private RectTransform[] lines;
    
    private Vector2[] finalSizes;
    private Coroutine scaleLinesCoroutine;
    private readonly Vector2 initialSize = new (0, 2);
    private float animTime;
    
    private void Awake()
    {
        animTime = UI.animationTime / 2.5f;
    }

    private void Start()
    {
        if (scaleAllChildren)
            lines = GetComponentsInChildren<RectTransform>();
        
        finalSizes = lines.Select(x => x.sizeDelta).ToArray();
    }

    public void Animate(bool exit, bool delayed=false)
    {
        if (scaleLinesCoroutine != null)
            StopCoroutine(scaleLinesCoroutine);
        scaleLinesCoroutine = StartCoroutine(ScaleLines(exit, delayed));
    }

    private IEnumerator ScaleLines(bool exit, bool delay)
    {
        if (delay)
        {
            foreach (var rt in lines)
            {
                rt.sizeDelta = initialSize;
            }
            yield return new WaitForSeconds(animTime);
        }

        float t = 0;

        while (t < animTime)
        {
            t += Time.deltaTime;
            var eval = t / animTime;

            for (int i = 0; i < lines.Length; i++)
            {
                if (!exit) // entry animation
                    lines[i].sizeDelta = Vector2.Lerp(initialSize, finalSizes[i], eval);
                else
                    lines[i].sizeDelta = Vector2.Lerp(finalSizes[i], initialSize, eval);
            }
            
            yield return null;
        }

        scaleLinesCoroutine = null;
    }
}