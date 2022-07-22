using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controla el panel de información del edificio
/// </summary>
public class PanelBuildingInfo : Panel
{
    [Header("Panel components")]
    [SerializeField] [Tooltip("Clase que controla la apertura de nuevos paneles a mostrar")]
    private PanelOpener opener;
    [SerializeField] [Tooltip("Botón que se encarga del cerrado del panel")]
    private Button closeButton;
    [SerializeField] [Tooltip("Icono del edificio")]
    private Image buildingImage;
    [SerializeField] [Tooltip("Texto de descripción del edificio")]
    private Text buildingDescription;

    #region Unity Methods

    /// <summary>
    /// Se ejecuta cuando el panel ha sido iniciado por primera vez en escena configurando el botón de salir del panel
    /// </summary>
    private void Start()
    {
        closeButton.onClick.AddListener(Close);
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Asigna la información del edificio en el panel
    /// </summary>
    /// <param name="buildingData">Datos del edificio</param>
    public void SetBuildingInfo(BuildingData buildingData)
    {
        buildingImage.sprite = buildingData.buildingIcon;
        buildingDescription.text = buildingData.infoDescription;
        opener.popupPrefab = gameObject;
    }

    #endregion
}
