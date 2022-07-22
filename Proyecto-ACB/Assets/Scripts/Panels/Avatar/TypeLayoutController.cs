using UnityEngine;

/// <summary>
/// Controla el tipo de distribuci�n asignados a los botones
/// </summary>
public class TypeLayoutController : MonoBehaviour
{
    [Header("Main Components")]
    [SerializeField] [Tooltip("Distribuci�n de los items del avatar")]
    private AvatarLayoutItems layoutItems;
    [SerializeField] [Tooltip("Tipo de distribuci�n de categor�a de botones")]
    private TypeLayoutCategory typeLayout;

    public AvatarLayoutItems LayoutItems => layoutItems;
    public TypeLayoutCategory TypeLayout => typeLayout;

    /// <summary>
    /// Muestra el tipo de distribuci�n
    /// </summary>
    public void EnableLayout()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Oculta el tipo de distribuci�n
    /// </summary>
    public void DisableLayout()
    {
        gameObject.SetActive(false);
    }
}
