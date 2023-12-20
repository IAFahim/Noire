using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExamineUI : UI
{
    public static ExamineUI Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI examineText;
    [SerializeField] private RawImage examineImage;
    private RectTransform imgRT;
    
    [SerializeField] private ButtonUI closeButton;

    protected override void Awake()
    {
        Instance = this;
        canvasGroup = GetComponent<CanvasGroup>();
        imgRT = examineImage.GetComponent<RectTransform>();
    }

    private void Start()
    {
        gameObject.SetActive(false);
        closeButton.AddListener(() => Hide());
    }

    public void Display(string text, Texture2D image=null)
    {
        gameObject.SetActive(true);
        examineText.text = text;

        if (image)
        {
            examineImage.texture = image;
            imgRT.sizeDelta = new Vector2(image.width, image.height);
        }
        else
            examineImage.texture = null;

        Show();
        HideAfterDelay(10);
    }
}
