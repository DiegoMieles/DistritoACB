using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

/// <summary>
/// Enumerador con el tipo de atributo f�sico del avatar al cual aplicar color
/// </summary>
public enum TypeColor
{
    None,
    Body,
    hair
}

/// <summary>
/// Controlador del bot�n que se encarga de asignar datos del funcionamiento y los colores que se deben mostrar
/// </summary>
public class AvatarColorButton : MonoBehaviour
{
    [Header("Main Components")]
    [SerializeField] [Tooltip("Dato del color a mostrar en el bot�n")]
    private ItemReferencesColor data;
    [SerializeField] [Tooltip("Categor�a de selecci�n de avatar")]
    private CategorySelection category;
    [SerializeField] [Tooltip("Contorno al rededor del bot�n la haber sido seleccionado")]
    private UICircle selectedColorButton;
    [SerializeField] [Tooltip("Bot�n de color del avatar")]
    private Button button;
    [SerializeField] [Tooltip("Objeto de selecci�n de imagen")]
    private GameObject selectImage;

    [Tooltip("Tipo de atributo al que se le debe aplicar color")]
    public TypeColor typeColor;

    public ItemReferencesColor Data => data;
    public UICircle SelectedColorButton => selectedColorButton;

    /// <summary>
    /// Configura los datos del bot�n de color
    /// </summary>
    /// <param name="category">Categor�a espec�fica del avatar</param>
    /// <param name="data">Datos del color del bot�n</param>
    /// <param name="customClickAction">Acci�n que se ejecuta al seleccionar el bot�n</param>
    public void Init(CategorySelection category, ItemReferencesColor data, Action customClickAction)
    {
        this.category = category;
        this.data = data;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(delegate {
            OnColorClick();
            customClickAction?.Invoke();
        });
    }

    /// <summary>
    /// Pinta atributo del avatar con el color del bot�n
    /// </summary>
    public void OnColorClick()
    {
        category.OnClickColorButton(data, typeColor);
        selectImage.SetActive(true);
    }

    /// <summary>
    /// Desactiva el color
    /// </summary>
    public void DesactiveColor()
    {
        selectImage.SetActive(false);
    }
}
