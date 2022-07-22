using UnityEngine;

/// <summary>
/// Controla el tipo de distribución asignados a los botones
/// </summary>
public class TypeLayoutController : MonoBehaviour
{
    [Header("Main Components")]
    [SerializeField] [Tooltip("Distribución de los items del avatar")]
    private AvatarLayoutItems layoutItems;
    [SerializeField] [Tooltip("Tipo de distribución de categoría de botones")]
    private TypeLayoutCategory typeLayout;

    public AvatarLayoutItems LayoutItems => layoutItems;
    public TypeLayoutCategory TypeLayout => typeLayout;

    /// <summary>
    /// Muestra el tipo de distribución
    /// </summary>
    public void EnableLayout()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Oculta el tipo de distribución
    /// </summary>
    public void DisableLayout()
    {
        gameObject.SetActive(false);
    }
}
