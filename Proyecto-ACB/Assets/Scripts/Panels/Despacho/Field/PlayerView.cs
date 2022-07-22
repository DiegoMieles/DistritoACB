using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controla la vista del jugador
/// </summary>
public abstract class PlayerView : MonoBehaviour
{
    #region Fields and Properties

    [Space(5)]
    [Header("Avatar part references")]
    [SerializeField] [Tooltip("Imagen del fondo del avatar")]
    protected Image backGround;
    [SerializeField] [Tooltip("Imagen de cuerpo")]
    protected Image bodyForm;
    [SerializeField] [Tooltip("Imagen de camisilla")]
    protected Image shirtDefault;
    [SerializeField] [Tooltip("Imagen de la forma de la cara")]
    protected Image faceForm;
    [SerializeField] [Tooltip("Imagen de las orejas")]
    protected Image ears;
    [SerializeField] [Tooltip("Imagen de los ojos")]
    protected Image eyes;
    [SerializeField] [Tooltip("Imagen de la nariz")]
    protected Image nose;
    [SerializeField] [Tooltip("Imagen de la boca")]
    protected Image mouth;
    [SerializeField] [Tooltip("Imagen de las cejas")]
    protected Image eyebrow;
    [SerializeField] [Tooltip("Imagen del cabello frontal")]
    protected Image hair;
    [SerializeField] [Tooltip("Imagen del cabello reverso")]
    protected Image hairBack;
    [SerializeField] [Tooltip("Imagen del accesorio de ojos")]
    protected Image eyesAccesory;
    [SerializeField] [Tooltip("Imagen del accesorio de cabeza")]
    protected Image headAccesory;
    [SerializeField] [Tooltip("Imagen del accesorio de cuerpo")]
    protected Image bodyAccesory;
    [SerializeField] [Tooltip("Imagen del accesorio de brazos")]
    protected Image armAccesory;

    protected const int DEFAULTHAIRCOLOR = 12; //Color de cabello por defecto
    protected const int DEFAULTSKINCOLOR = 0; //Color de piel por defecto
    protected const int DEFAULTBODYFORM = 136; //Forma del cuerpo por defecto
    protected const int DEFAULTFACEFORM = 42; //Forma de la cara por defecto
    protected const int DEFAULTEYES = 72; //Ojos por defecto
    protected const int DEFAULTEAR = 130; //Orejas por defecto
    protected const int DEFAULTNOSE = 110; //Nariz por defecto
    protected const int DEFAULTMOUTH = 22; //Boca por defecto
    protected const int DEFAULTEYEBROW = 52; //Cejas por defecto
    protected const int DEFAULTHAIRSTYLE = 148; //Estilo de cabello por defecto

    #endregion

    #region Protected Methods

    /// <summary>
    /// Actualiza la vista del jugador
    /// </summary>
    protected abstract void UpdateView();

    /// <summary>
    /// Asigna color al avatar en un atributo en específico
    /// </summary>
    /// <param name="typeColor">Tipo de color</param>
    /// <param name="color">Color a asignar</param>
    protected void SetColorToAvatar(TypeColor typeColor, Color color)
    {
        if (typeColor == TypeColor.Body)
        {
            faceForm.color = color;
            bodyForm.color = color;
            nose.color = color;
            ears.color = color;
        }

        if (typeColor == TypeColor.hair)
        {
            hair.color = color;
            hairBack.color = color;
            eyebrow.color = color;
        }
    }

    /// <summary>
    /// Asigna sprite por valor y carga internamente la vista del jugador
    /// </summary>
    /// <param name="loadedBackendData">id cargado de backend</param>
    /// <param name="defaultData">Dato por defecto</param>
    /// <param name="imageSpriteReference">Imagen del item a asignar</param>
    protected void SetSpriteByValue(int loadedBackendData, int defaultData, Image imageSpriteReference)
    {
        int ResourceValue = loadedBackendData <= 15 ? defaultData : loadedBackendData;
        Sprite testSprite = Resources.Load<Sprite>("SpriteItem/" + ResourceValue.ToString());
        imageSpriteReference.sprite = testSprite;
    }

    /// <summary>
    /// Ajusta la camisilla de acuerdo al tamaño del cuerpo
    /// </summary>
    /// <param name="bodyFormId">Id de la forma del cuerpo</param>
    /// <param name="underShirtReference">Lista de camisillas disponibles</param>
    protected void SetUndershirt(int bodyFormId, List<UnderShirtReferenceData> underShirtReference)
    {
        for (int i = 0; i < underShirtReference.Count; i++)
        {
            UnderShirtReferenceData actualUnderShirtData = underShirtReference[i];
            for (int j = 0; j < actualUnderShirtData.bodyReferences.Count; j++)
            {
                if (actualUnderShirtData.bodyReferences[j] == bodyFormId)
                {
                    shirtDefault.sprite = actualUnderShirtData.underShirtSprite;
                    return;
                }
            }
        }
    }

    #endregion
}
