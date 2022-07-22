using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controla el cambio del tipo de categoría del atributo del avatar
/// </summary>
public class ChangeCategory : MonoBehaviour
{
    [Header("Main Components")]
    [SerializeField] [Tooltip("Tipo de categoría")]
    private TypeCategory typeCategory;
    [SerializeField] [Tooltip("Color del atributo")]
    private Color selectColor;
    [SerializeField] [Tooltip("Imagen del botón")]
    private Image imageButton;

    private AvatarEditor avatarEditor; //Editor de avatar

    public TypeCategory TypeCategory => typeCategory;

    /// <summary>
    /// Carga los datos del editor de avatar
    /// </summary>
    /// <param name="_avatarEditor">Editor de avatar</param>
    public void Init(AvatarEditor _avatarEditor)
    {
        avatarEditor = _avatarEditor;
    }

    /// <summary>
    /// Cambio de la categoría de los botones
    /// </summary>
    public void OnChangeCategory()
    {
        imageButton.color = selectColor;
        avatarEditor.OnSelectCategory(typeCategory);
    }

    /// <summary>
    /// Regresa a la imagen a su color original
    /// </summary>
    public void OnDesactiveCategory()
    {
        imageButton.color = Color.white;
    }
}
