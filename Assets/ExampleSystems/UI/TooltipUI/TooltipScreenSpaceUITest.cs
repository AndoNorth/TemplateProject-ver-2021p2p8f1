using UnityEngine;

public class TooltipScreenSpaceUITest : MonoBehaviour
{
    Vector3 position = Vector3.zero;
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
        }
        position = GeneralUtility.GetMouseWorldPosition();
        System.Func<string> GetTooltipTextFunc = () =>
        {
            string clickPositionString = "MousePosition: (" + position.x + "," + position.y + ")"
            + "\n";
            return clickPositionString;
        };
        TooltipScreenSpaceUI.ShowTooltip_Static(GetTooltipTextFunc);
    }
}
