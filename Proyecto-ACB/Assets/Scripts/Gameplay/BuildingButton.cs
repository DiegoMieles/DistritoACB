using UnityEngine;
using UnityEngine.UI;
using Coffee.UIExtensions;
using UnityEngine.EventSystems;

/// <summary>
/// Estructura que almacena los datos principales de un edificio
/// </summary>
[System.Serializable]
public class BuildingData
{
    [Tooltip("Panel o vista que el edificio abre al ser seleccionado")]
    public GameObject panelToOpen;
    [Tooltip("Determina si se debe carga un panel como prefab o si se debe activar un objeto existente")]
    public bool panelIsPrefab = true;
    [Tooltip("Determina si el edificio tiene información adicional, por ejemplo, descripción")]
    public bool panelHasInfoToDisplay = false;
    [Tooltip("Determina si los datos del edificio deben permanecer almacenados en memoria al abrir otro edificio")]
    public bool storePanelAsPrevious = false;

    [Header("Probably info will be set in scriptable objects")]
    [Tooltip("Título con el nombre del edificio")]
    public string infoTitle;
    [Tooltip("Título con la dedscripción del edificio")]
    public string infoDescription;
    [Tooltip("Ícono del edificio")]
    public Sprite buildingIcon;
}

/// <summary>
/// Clase controla los datos del edificio y su comportamiento como si se tratase de un botón
/// </summary>
public class BuildingButton : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [Header("Building Components")]
    [SerializeField] [Tooltip("Contorno que se muestra al rededor del edificio")]
    protected Outline selectionOutline;
    [SerializeField] [Tooltip("Efecto de barrido que se muestra encima de la imagen del edificio")]
    protected UIShiny shiny;
    [SerializeField] [Tooltip("Componente botón del edificio")]
    private Button buildingButton;
    [Space(5)]
    [Header("Building Configuration")]
    [SerializeField] [Tooltip("Datos generales del edificio")]
    private BuildingData buildingData;
    [SerializeField] [Tooltip("Id del edificio")]
    private string buildingId;
    [SerializeField] [Tooltip("Referencia de la luz que ilumina el edificio al estar de noche")]
    private GameObject lightObject;

    public BuildingData BuildingData => buildingData;
    public Outline SelectionOutline => selectionOutline;
    public UIShiny Shiny => shiny;
    public string BuildingId => buildingId;

    #region Unity Methods

    /// <summary>
    /// Se ejecuta cuando se activa el objeto, mostrando el contorno del edificio
    /// </summary>
    private void OnBecameVisible()
    {
        selectionOutline.enabled = true;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Configura el botón del edificio a nivel lógico y visual
    /// </summary>
    /// <param name="outlineColor">Color del contorno del edificio</param>
    public void SetupButton(Color outlineColor)
    {
        buildingButton.image.alphaHitTestMinimumThreshold = 0.5f;
        buildingButton.onClick.AddListener(OpenBuildingOptions);
        selectionOutline.effectColor = outlineColor;
        selectionOutline.enabled = false;
        shiny.Stop();
    }

    /// <summary>
    /// Se asignan los datos principales del edificio
    /// </summary>
    /// <param name="backendBuildData">Datos del edificio traidos desde backend</param>
    /// <param name="isNight">Determina si es de noche para mostrar la luz del edificio</param>
    public void SetBuildingData(Data.MainMenuData.BuildData backendBuildData, bool isNight)
    {
        if (!string.IsNullOrEmpty(backendBuildData.title))
            buildingData.infoTitle = backendBuildData.title;

        if (!string.IsNullOrEmpty(backendBuildData.info))
            buildingData.infoDescription = backendBuildData.info;

        if(lightObject != null)
            lightObject.SetActive(isNight);
    }

    #endregion

    #region Protected Methods
    
    /// <summary>
    /// Carga los datos del edificio al panel de opciones que se muestra al seleccionar un edificio
    /// </summary>
    protected virtual void OpenBuildingOptions() => ACBSingleton.Instance.PanelBuildingSelection.ShowPanelOptions(buildingData);

    #endregion

    #region ISelectHandler Implementation

    /// <summary>
    /// Muestra el contorno y efecto de barrido al seleccionar un edificio
    /// </summary>
    /// <param name="eventData"></param>
    public void OnSelect(BaseEventData eventData)
    {
        selectionOutline.enabled = true;
        shiny.Play();
    }
    #endregion

    #region IDeselectHandler Implementation

    /// <summary>
    /// Oculta el contorno y efecto de barrido al dejar de selecionar un edificio
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDeselect(BaseEventData eventData)
    {
        selectionOutline.enabled = false;
        shiny.Stop();
    }

    #endregion
}
