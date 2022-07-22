using System.Collections.Generic;
using UnityEngine;
using Data;

/// <summary>
/// Referencia de los colores que puede tener un item dentro del juego
/// </summary>
[System.Serializable]
public class ItemReferencesColor
{
    public int id;
    public ItemType type;
    public Color icon;

    /// <summary>
    /// Constructor de la referencia del color de un item
    /// </summary>
    /// <param name="id">Id del color</param>
    /// <param name="type">Tipo de item al que se le quiere aplicar color</param>
    /// <param name="icon">Dato del color en específico</param>
    public ItemReferencesColor(int id, ItemType type, Color icon)
    {
        this.id = id;
        this.type = type;
        this.icon = icon;
    }
}

/// <summary>
/// Referencia de camisillas disponibles para asignar al avatar de acuerdo a la complexión de cuerpo
/// </summary>
[System.Serializable]
public class UnderShirtReferenceData
{
    public Sprite underShirtSprite;
    public List<int> bodyReferences;
    public string undershirtSizeName;
}

/// <summary>
/// Datos extra del avatar como los colores disponibles de cabello, de piel y las camisillas puestas
/// </summary>
[CreateAssetMenu(fileName = "ScriptableAvatarExtraData", menuName = "ScriptableObjects/ScriptableColorData", order = 4)]
public class AvatarExtraData : ScriptableObject
{
    public List<ItemReferencesColor> skinColor;
    public List<ItemReferencesColor> hairColors;
    public List<UnderShirtReferenceData> underShirts;
}
