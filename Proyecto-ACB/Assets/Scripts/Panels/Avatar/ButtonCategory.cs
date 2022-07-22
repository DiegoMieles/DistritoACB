using Data;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controla la categoria de los botones
/// </summary>
public class ButtonCategory : MonoBehaviour
{
    [Header("Category values")]
    [SerializeField] [Tooltip("Tipo de categor�a")]
    private TypeCategory category;
    [SerializeField] [Tooltip("Tipo de item")]
    private ItemType itemType;
    [SerializeField] [Tooltip("Tipo de vista de categor�a")]
    private TypeLayoutCategory layoutCategory;
    [SerializeField] [Tooltip("Determina si el bot�n a usar es nulo")]
    private bool useNullButton;
    [SerializeField] [Tooltip("Determina si los datos se deben cargar desde los datos internos de juego")]
    private bool loadFromGameData;

    [Space(10)]
    [Header("Button visual components")]
    [SerializeField] [Tooltip("Texto con el nombre de la categor�a del item")]
    private Text text;
    [SerializeField] [Tooltip("Bot�n de la categor�a")]
    private Button button;
    [SerializeField] [Tooltip("Color cuando el bot�n es seleccionado")]
    private Color colorEnable;
    [SerializeField] [Tooltip("Color cuando el bot�n no esta seleccionado")]
    private Color colorDisable;
    [SerializeField] [Tooltip("Imagen de selecci�n")]
    private Image selectionImage;

    private CategorySelection categorySelection; //Categor�a de selecci�n

    public TypeCategory Category => category;
    public ItemType ItemType => itemType;
    public TypeLayoutCategory LayoutCategory => layoutCategory;
    public bool UseNullButton => useNullButton;

    /// <summary>
    /// Configura el bot�n de categor�a
    /// </summary>
    /// <param name="_category">Categor�a del bot�n</param>
    public void Init(CategorySelection _category)
    {
        categorySelection = _category;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(delegate {
            categorySelection.SelectButton(itemType, layoutCategory, useNullButton, loadFromGameData);
        });

        gameObject.SetActive(true);
    }

    /// <summary>
    /// Activa bot�n
    /// </summary>
    public void EnableButton()
    {
        text.color = colorEnable;
        selectionImage.gameObject.SetActive(true);
        if(itemType == ItemType.ACBALL)
        {
            categorySelection.AvatarEditor.HideButtons(true);
        }
    }

    /// <summary>
    /// Deshabilita el bot�n
    /// </summary>
    public void DisableButton()
    {
        text.color = colorDisable;
        selectionImage.gameObject.SetActive(false);
        if(categorySelection && itemType == ItemType.ACBALL)
        {
            categorySelection.AvatarEditor.HideButtons(false);
        }
    }

    /// <summary>
    /// Desactiva el bot�n
    /// </summary>
    public void DesactiveButton()
    {
        gameObject.SetActive(false);
    }
}
