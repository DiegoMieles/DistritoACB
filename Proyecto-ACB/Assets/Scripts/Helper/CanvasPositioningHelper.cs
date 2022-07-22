using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Clase con utilidades relacionadas a componentes de UI
/// </summary>
public static class CanvasPositioningHelper
{
    /// <summary>
    /// Determina la posici�n global de un objeto que se encuentra en una posici�n local, por ejemplo un panel con menos tama�o
    /// </summary>
    /// <param name="objectLocalTransform">Componente de transformaci�n local del objeto</param>
    /// <param name="convertGlobalObjectTransform">Componente de transformaci�n global</param>
    /// <returns></returns>
    public static Vector2 GetGlobalCanvasRectTransformPosition(this RectTransform objectLocalTransform, RectTransform convertGlobalObjectTransform)
    {
        Vector3[] corners = new Vector3[4];

        objectLocalTransform.GetWorldCorners(corners);

        List<float> xCoords = new List<float>();
        List<float> yCoords = new List<float>();

        for (int i = 0; i < 4; i++)
        {
            Vector3 possibleCornerPos = convertGlobalObjectTransform.InverseTransformPoint(corners[i]);

            if (!xCoords.Contains(possibleCornerPos.x))
                xCoords.Add(possibleCornerPos.x);

            if (!yCoords.Contains(possibleCornerPos.y))
                yCoords.Add(possibleCornerPos.y);
        } 
        
        return new Vector2((xCoords[0] + xCoords[1]) / 2, (yCoords[0] + yCoords[1]) / 2);
    }
}
