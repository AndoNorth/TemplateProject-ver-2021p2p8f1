using UnityEngine;
using TMPro;
// this is a dynamically sized text tooltip
// can be called by any other script using a delegate which returns a string
// code monkey - https://www.youtube.com/watch?v=YUIohCXt_pc
public class TooltipScreenSpaceUI : MonoBehaviour
{
    /* creating an "instance" is where you declare a class as a single instance
     this means that there is and only is one instance of this class */
    public static TooltipScreenSpaceUI Instance { get; private set; }

    private System.Func<string> getTooltipTextFunc;

    private RectTransform backgroundRectTransform;
    private TextMeshProUGUI textMeshPro;
    private RectTransform rectTransform;
    private RectTransform canvasRectTransform;

    private void Awake()
    {
        Instance = this; // set the object with this script to be the only instance of this class

        backgroundRectTransform = transform.Find("textBackground").GetComponent<RectTransform>();
        textMeshPro = transform.Find("tooltipText").GetComponent<TextMeshProUGUI>();
        rectTransform = transform.GetComponent<RectTransform>();
        canvasRectTransform = transform.parent.GetComponent<RectTransform>();

        HideTooltip();
    }
    private void Update()
    {
        SetText(getTooltipTextFunc());
        SetTooltipPosition();
    }
    #region internalTooltipFunctions
    private void SetText(string tooltipText)
    {
        textMeshPro.SetText(tooltipText);
        textMeshPro.ForceMeshUpdate();

        Vector2 textSize = textMeshPro.GetRenderedValues(false);
        Vector2 textBackgroundPaddingSize = new Vector2(8, 8);

        backgroundRectTransform.sizeDelta = textSize + textBackgroundPaddingSize;
    }

    private Vector2 ReturnMousePositionOnCanvas()
    {
        return Input.mousePosition / canvasRectTransform.localScale.x;
    }

    private Vector2 ReturnRealignedTooltipFromOffCanvas(Vector2 anchoredPosition)
    {
        // if greater than the right edge
        if (anchoredPosition.x + backgroundRectTransform.rect.width > canvasRectTransform.rect.width)
        {
            anchoredPosition.x = canvasRectTransform.rect.width - backgroundRectTransform.rect.width;
        }
        // if greater than the top edge
        if (anchoredPosition.y + backgroundRectTransform.rect.height > canvasRectTransform.rect.height)
        {
            anchoredPosition.y = canvasRectTransform.rect.height - backgroundRectTransform.rect.height;
        }
        if (anchoredPosition.x < 0)
        {
            anchoredPosition.x = 0;
        }
        if (anchoredPosition.y < 0)
        {
            anchoredPosition.y = 0;
        }
        return anchoredPosition;
    }

    private void SetTooltipPosition()
    {
        rectTransform.anchoredPosition = ReturnRealignedTooltipFromOffCanvas(ReturnMousePositionOnCanvas());
    }

    private void ShowTooltip(string tooltipText)
    {
        ShowTooltip(() => tooltipText);
    }

    private void ShowTooltip(System.Func<string> getTooltipTextFunc)
    {
        this.getTooltipTextFunc = getTooltipTextFunc;
        gameObject.SetActive(true);
        SetText(getTooltipTextFunc());
    }

    private void HideTooltip()
    {
        gameObject.SetActive(false);
    }
    #endregion
    // for use outside of this class
    #region globalTooltipFunctions
    public static void ShowTooltip_Static(string tooltipText)
    {
        Instance.ShowTooltip(tooltipText);
    }

    public static void ShowTooltip_Static(System.Func<string> getTooltipTextFunc)
    {
        Instance.ShowTooltip(getTooltipTextFunc);
    }

    public static void HideTooltip_Static()
    {
        Instance.HideTooltip();
    }
    #endregion
}
