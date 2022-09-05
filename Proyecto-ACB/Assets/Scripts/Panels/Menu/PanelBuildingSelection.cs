using Data;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WebAPI;

/// <summary>
/// Clase que almacena los datos principales de un edificio
/// </summary>
public class BuildingCachedData
{
    [Tooltip("Acción que se ejecuta al cerrar el edificio")]
    public Action buildingGoBackAction;
    [Tooltip("Datos del edificio")]
    public BuildingData previousBuildingData;
    [Tooltip("Panel del edificio a cerrar")]
    public Panel buildingPanel;

    /// <summary>
    /// Constructor que almacena los datos del edificio
    /// </summary>
    /// <param name="previousBuildingData"></param>
    public BuildingCachedData(BuildingData previousBuildingData)
    {
        this.previousBuildingData = previousBuildingData;
    }

    /// <summary>
    /// Se añade el panel del edificio a cerrar
    /// </summary>
    /// <param name="buildingPanel">Panel a cerrar</param>
    public void AddPanelToClose(Panel buildingPanel) => this.buildingPanel = buildingPanel;

    /// <summary>
    /// Añade una acción que se ejecuta al cerrar el edificio
    /// </summary>
    /// <param name="buildingGoBackAction"></param>
    public void AddGoBackAction(Action buildingGoBackAction) => this.buildingGoBackAction = buildingGoBackAction;

}

/// <summary>
/// Controla el panel de selección de edificio
/// </summary>
public class PanelBuildingSelection : Panel
{

    #region Fields and Properties

    [Header("Panels References")]
    [SerializeField] [Tooltip("Clase que controla la apertura de nuevos paneles a mostrar")]
    private PanelOpener panelOpener;
    [SerializeField] [Tooltip("Prefab del panel de información del edificio seleccionado")]
    private GameObject buildingInfoPanelPrefab;
    [SerializeField] [Tooltip("Prefab del panel de avatar")]
    private GameObject avatarPanelPrefab;
    [SerializeField] [Tooltip("Prefab del panel de ACBall")]
    private GameObject acballPanelPrefab;
    [SerializeField] [Tooltip("Texto de cantidad de ACBalls que tiene el jugador")]
    private Text ACBallsAmountText;
    [SerializeField] [Tooltip("Límite de valor de las ACBalls a nivel gráfico")]
    private float limitAcball;
    [SerializeField] [Tooltip("Límite de valor de las ACBCoins a nivel gráfico")]
    private float limitCoin;
    [SerializeField] [Tooltip("Texto de cantidad de ACBalls")]
    private Text ACBCoinsAmountText;
   
    [SerializeField] [Tooltip("Burbuja que aparece en las ACBall")]
    private GameObject acballBubble;

    [Space(5)]
    [Header("Building Option components")]
    [SerializeField] [Tooltip("Ícono de edificio seleccionado")]
    private Image buildingIcon;
    [SerializeField] [Tooltip("Objeto que contiene el menú de opciones al seleccionar un edificio")]
    private GameObject buildingOptions;
    [SerializeField] [Tooltip("Botón de entrar al edificio")]
    private Button buildingEnterButton;
    [SerializeField] [Tooltip("Botón de información del edificio")]
    private Button buildingInfoButton;
    [SerializeField] [Tooltip("Botón de avatar del jugador")]
    private Button avatarButton;
    [SerializeField] [Tooltip("Vista del avatarn del jugador")]
    private AvatarImageView lowerAvatarView;
    [SerializeField] [Tooltip("Texto del nombre del edificio")]
    private Text buildingNameText;

    [Space(5)]
    [Header("Inner building components")]
    [SerializeField] [Tooltip("Lista de componentes UI del edificio")]
    private List<GameObject> innerBuildingsUIComponents;
    [SerializeField] [Tooltip("Ícono del edificio")]
    public Image innerBuildingIcon;
    [SerializeField] [Tooltip("Texto del edificio")]
    public Text innerBuildingName;
    [SerializeField] [Tooltip("Botón de volver a un panel anterior")]
    public Button goBackButton;
    [SerializeField] [Tooltip("Vista principal de la ciudad")]
    private GameObject mainMenuWorldView;

    [Space(5)]
    [Header("Other UI components")]
    [SerializeField] [Tooltip("Botón de ACBall")]
    private Button acballButton;
    [SerializeField] [Tooltip("Imagen del fondo del panel")]
    private GameObject background;

    private BuildingData cachedBuildingData; //Datos del edificio actual
    private BuildingData cachedBuildingMapData; //Datos del escenario actual (Ciudad/Pabellón)
    private GameObject cachedInnerBuildingObject; //Objeto del edificio actual
    private MissionsData.MissionItemData cachedMissionData; //Data de la misión actual
    
    public Stack<BuildingCachedData> cachedBuildingsStack {get; private set; } //Pila de edificios visitados dentro de otros
    private BuildingCachedData buildingCachedData; //Datos almacenados de edificio

    #endregion

    #region Unity Methods

    /// <summary>
    /// Se ejecuta cuando el panel ha sido iniciado por primera vez en escena y muestra los datos básicos del jugador
    /// </summary>
    private void Start()
    {
        goBackButton.onClick.AddListener(GoToPreviousPanel);
        goBackButton.gameObject.SetActive(false);
        cachedBuildingsStack = new Stack<BuildingCachedData>();
        avatarButton.onClick.AddListener(OpenAvatarPanel);
        acballButton.onClick.AddListener(OpenACBallPanel);
    }

    /// <summary>
    /// Se ejecuta constantemente actualizando los datos de los recursos que tiene el jugador
    /// </summary>
    private void Update()
    {
        ACBallsAmountText.text = Mathf.Clamp(ACBSingleton.Instance.AccountData.statsData.acballsBalance, 0, limitAcball) .ToString();
        ACBCoinsAmountText.text = Mathf.Clamp(ACBSingleton.Instance.AccountData.statsData.coinsBalance, 0, limitCoin).ToString();
        acballBubble.SetActive(ACBSingleton.Instance.AccountData.statsData.acballsNotification);
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Muestra el minipanel de opciones al seleccionar un edificio
    /// </summary>
    /// <param name="buildingData">Datos del edificio seleccionado</param>
    public void ShowPanelOptions(BuildingData buildingData)
    {
        if (buildingOptions.activeInHierarchy)
            return;

        if(buildingData.panelIsPrefab && buildingData.storePanelAsPrevious)
            buildingCachedData = new BuildingCachedData(cachedBuildingData);

        cachedMissionData = null;
        buildingOptions.SetActive(true);
        if (ACBSingleton.Instance.GameData.mainMenuData.buildInfoData.Find(x => x.title == buildingData.infoTitle).id != "Edf-Cajero" && ACBSingleton.Instance.GameData.mainMenuData.buildInfoData.Find(x => x.title == buildingData.infoTitle).id != "Edf-Cartelera")
        {
            cachedBuildingData = buildingData;
        }

        buildingNameText.text = buildingData.infoTitle;
        buildingIcon.sprite = buildingData.buildingIcon;
        
        background.SetActive(true);

        if (buildingData.panelToOpen != null)
        {
            panelOpener.popupPrefab = buildingData.panelToOpen;

            if (buildingData.panelIsPrefab)
                buildingEnterButton.onClick.AddListener(()=> { OpenBuildingPrefabPanel(buildingData.panelToOpen, buildingData.panelHasInfoToDisplay); });
            else
            {
                cachedInnerBuildingObject = buildingData.panelToOpen;
                buildingEnterButton.onClick.AddListener(() => OpenNewExistingMap(true));
            }
        }

        buildingEnterButton.transform.parent.gameObject.SetActive(buildingData.panelToOpen != null);
        buildingInfoButton.onClick.AddListener(OpenInfobuildingPanel);
    }

    /// <summary>
    /// Allmacena los datos de la mision actual en caso de seleccionarse una marquesina
    /// </summary>
    /// <param name="missionData">Datos de la misión actual</param>
    public void SetMissionData(MissionsData.MissionItemData missionData)
    {
        cachedMissionData = missionData;
    }

    /// <summary>
    /// Oculta el panel de opciones
    /// </summary>
    public void HidePanelOptions()
    {
        if (!buildingOptions.activeInHierarchy)
            return;

        background.SetActive(false);
        buildingOptions.SetActive(false);
        buildingEnterButton.onClick.RemoveAllListeners();
        buildingInfoButton.onClick.RemoveAllListeners();
        cachedMissionData = null;
    }

    /// <summary>
    /// Verifica si es la primera vez que el jugador entra al juego, de ser así abre
    /// automáticamente el panel de avatar
    /// </summary>
    /// <param name="isFirstTimeInGame">Determina si es la primera vez que el jugador entra al juego</param>
    public void OpenFirstTimeAvatarPanel(bool isFirstTimeInGame)
    {
        gameObject.SetActive(true);

        if (isFirstTimeInGame)
        {
            OpenAvatarPanel();
        }

        lowerAvatarView.UpdateView();
    }

    /// <summary>
    /// Abre el panel de ACBall
    /// </summary>
    public void OpenACBallPanel()
    {
        panelOpener.popupPrefab = acballPanelPrefab;
        panelOpener.OpenPopup();
        panelOpener.popup.GetComponent<PanelACBall>().LoadACBallsData();
        HidePanelOptions();
    }

    #endregion

    #region Inner Methods

    /// <summary>
    /// Abre el prefab del panel del edificio seleccionado
    /// </summary>
    private void OpenBuildingPrefabPanel(GameObject prefabToOpen,bool panelHasInfoToDisplay)
    {
        Debug.Log(prefabToOpen.name);
        if (WebProcedure.Instance.IsUserNull() && !prefabToOpen.GetComponent<PanelHeadquarter>())
        {
            ACBSingleton.Instance.NoUserInfo();
        }
        else
        {
            panelOpener.popupPrefab = prefabToOpen;
            panelOpener.OpenPopup();

            if(cachedMissionData != null)
                panelOpener.popup.GetComponent<PanelMision>().ShowInfo(cachedMissionData);

            if (panelHasInfoToDisplay)
            {
                buildingCachedData.AddPanelToClose(panelOpener.popup.GetComponent<Panel>());
                SetNewGoBackAction(buildingCachedData.buildingPanel.Close);
            }

            HidePanelOptions();
        }

    }

    /// <summary>
    /// Activa o desactiva la vista dentro de un edificio (Actualmente aplica para pabellón)
    /// </summary>
    /// <param name="state">Estado de activación de la vista del edificio</param>
    private void OpenNewExistingMap(bool state)
    {
        if (!WebProcedure.Instance.IsUserNull())
        {
            cachedInnerBuildingObject.SetActive(state);
            mainMenuWorldView.SetActive(!state);

            if (cachedBuildingData.panelHasInfoToDisplay && state)
            {
                buildingCachedData = new BuildingCachedData(null);
                cachedBuildingMapData = cachedBuildingData;
                SetNewGoBackAction(() => { OpenNewExistingMap(false); });
            }

            HidePanelOptions();
        }
        else
        {
            ACBSingleton.Instance.NoUserInfo();
        }
  
    }

    /// <summary>
    /// Asigna acciones a ejecutar cuando el jugador decide volver a un edificio previo
    /// </summary>
    /// <param name="newActionOnGoBack">Acción a ejecutar al salir del edificio</param>
    private void SetNewGoBackAction(Action newActionOnGoBack)
    {
        if (cachedBuildingsStack.Count == 0)
        {
            goBackButton.gameObject.SetActive(true);
            innerBuildingsUIComponents.ForEach(component => component.SetActive(true));
        }

        innerBuildingIcon.sprite = cachedBuildingData.buildingIcon;
        innerBuildingName.text = cachedBuildingData.infoTitle;

        buildingCachedData.AddGoBackAction(newActionOnGoBack);
        cachedBuildingsStack.Push(buildingCachedData);
    }

    /// <summary>
    /// Abre panel donde se muestra la información y descripción de un edificio
    /// </summary>
    private void OpenInfobuildingPanel()
    {
        panelOpener.popupPrefab = buildingInfoPanelPrefab;
        panelOpener.OpenPopup();
        panelOpener.popup.GetComponent<PanelBuildingInfo>().SetBuildingInfo(cachedBuildingData);
        HidePanelOptions();
    }

    /// <summary>
    /// Abre el panel de avatar
    /// </summary>
    private void OpenAvatarPanel()
    {
        if (!WebProcedure.Instance.IsUserNull())
        {
            panelOpener.popupPrefab = avatarPanelPrefab;
            panelOpener.OpenPopup();
            panelOpener.popup.GetComponent<PanelAvatar>().ActivePanel(lowerAvatarView.UpdateView);
        }
        else
        {
         ACBSingleton.Instance.NoUserInfo();
        }
  
    }

    /// <summary>
    /// Regresar al panel previo y actualiza los datos del edificio donde se encuentra el jugador
    /// </summary>
    private void GoToPreviousPanel()
    {
        if (cachedBuildingsStack.Count <= 0)
            return;

        if(cachedBuildingsStack.Count == 1)
        {
            innerBuildingsUIComponents.ForEach(component => component.SetActive(false));
            ACBSingleton.Instance.UpdateGameData();
        }

        BuildingCachedData lastSavedData = cachedBuildingsStack.Pop();
        innerBuildingIcon.sprite = lastSavedData.previousBuildingData != null ? lastSavedData.previousBuildingData.buildingIcon : null;
        innerBuildingName.text = lastSavedData.previousBuildingData != null ? lastSavedData.previousBuildingData.infoTitle : string.Empty;
        Debug.Log(innerBuildingName.text);
        lastSavedData.buildingGoBackAction?.Invoke();

        if(lastSavedData.previousBuildingData != null)
            cachedBuildingData = lastSavedData.previousBuildingData;
    }

    /// <summary>
    /// Resetea los datos del mapa principal
    /// </summary>
    public void ResetCachedMapData() => cachedBuildingData = cachedBuildingMapData;

    #endregion
}
