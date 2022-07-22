using UnityEngine;

public static class RectTransformExtensions
{
    /// <summary>
    /// Checks if this RectTransform is overlaping the other RectTransform in the world space [Z Neutral]
    /// </summary>
    /// <param name="overlaping"></param>
    /// The object overlaping
    /// <param name="overlaped"></param>
    /// the object being overlaped
    /// <returns></returns>
    public static bool WorldSpaceOverlaps(this RectTransform overlaping, RectTransform overlaped)
    {
        Vector3[] aux = new Vector3[4]; //Cache
        //Get worldSpace corners and  then creates a Rect considering the first and the third values are diagonal
        overlaping.GetWorldCorners(aux);
        Rect overlapingRect = new Rect(aux[0], (aux[2] - aux[0]));
 
        //Reapeats for the other RectTranform
        overlaped.GetWorldCorners(aux);
        Rect overlapedRect = new Rect(aux[0], (aux[2] - aux[0]));
 
        //Use Rect.Overlaps to do the necessary calculations
        return (overlapedRect.Overlaps(overlapingRect, true));
    }
}
