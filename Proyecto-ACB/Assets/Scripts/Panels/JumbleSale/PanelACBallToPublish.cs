using Data;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using WebAPI;
using System.Linq;

/// <summary>
/// Controlador principal del panel donde se muestran las ACBall que tiene un jugador
/// </summary>
public class PanelACBallToPublish : Panel
{
    #region Fields and properties
    //Evento que se dispara al completar la publicación en el mercadillo
    public event PanelConfirmPublish.VoidDelegate OnConfirmedPublish;
    [Header("Panel Components")]
    [SerializeField] [Tooltip("Button de cerrar/esconder el panel ACBall")]
    private Button closePanelButton;
    [SerializeField] [Tooltip("Prefab del botón donde se muestra la ACBall con su descripción")]
    private GameObject acballButtonPrefab;
    [SerializeField] [Tooltip("Referencia al layout encargado de mostrar los botones apilados de ACBall")]
    private LayoutElement acballButtonLayoutReference;
    [SerializeField] [Tooltip("Referencia al objeto que contiene los botones de ACBall")]
    private RectTransform acballButtonContainer;
    [SerializeField] [Tooltip("Referencia principal al espacio de scroll donde se encuentran los botones")]
    private ScrollRect scroll;
    [SerializeField] [Tooltip("Esta clase contiene los datos principales de las ACBall del usuario, los cuales se actualizan al cargar en backend")]
    private AcbBallContainer.ACBallsToSell acballsContainer = new AcbBallContainer.ACBallsToSell();
    [SerializeField] [Tooltip("Botón para publicar el item seleccionado")]
    private Button publishButton ;
    
    [SerializeField] [Tooltip("encargado de abrir los páneles")]
    private PanelOpener panelOpener;
    [SerializeField] [Tooltip("prefab del panel de confirmación")]
    private GameObject confirmPanelPrefab;

    private bool allAcballsLoaded; //Determina si el listado de ACBalls del usuario está cargado
    private bool isLoadingNewACBalls; //Determina si está cargando más ACBalls del usuario
    private PageBody page; //Clase que contiene los datos principales de una página de ACBalls
    private int counter; //Entero que determina cual ha sido la última página de ACBalls cargada
    [SerializeField, TextArea]
    [Tooltip("Texto que se muestra cuando la traida de datos es fallida")]
    private string textFail;
    [SerializeField]
    [Tooltip("Texto de no acballs")]
    private Text textNoACBalls;
    #endregion

    #region Unity Methods

    /// <summary>
    /// Este método se ejecuta una vez se activa el panel, en este caso carga el evento del botón de cerrado de panel
    /// y carga el evento del scroll cada vez que es movido
    /// </summary>
    private void OnEnable()
    {
        publishButton.onClick.AddListener(PublishClick);
        closePanelButton.onClick.AddListener(() => { ACBSingleton.Instance.UpdateGameData(); Close(); });
        scroll.onValueChanged.AddListener(OnScrollContent);
        OnToggleValueChanged();
        LoadACBallsData();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Hace la carga inicial de los datos de ACBall trayendo los datos de las ACBalls del usuario desde backend
    /// </summary>
    public void LoadACBallsData()
    {
        counter = 1;
        allAcballsLoaded = false;
        page = new PageBody() { page = counter };
        WebProcedure.Instance.GetACBallsToSell(JsonConvert.SerializeObject(page), OnSuccess, OnFailed);
    }

    #endregion

    #region Inner Methods

    /// <summary>
    /// Método llamado cada vez que se mueve el scroll que al llegar al final del scroll
    /// Determina si hay más ACBalls del usuario a cargar, teniendo en cuenta en qué 
    /// página se encuentra el usuario
    /// </summary>
    /// <param name="scrollNormalizedPos">Posición actual del jugador en el scroll</param>
    private void OnScrollContent(Vector2 scrollNormalizedPos)
    {
        if (scrollNormalizedPos.y <= 0.1f && !allAcballsLoaded && !isLoadingNewACBalls)
        {
           // counter++;
            SetSpinnerNewState(true);
            isLoadingNewACBalls = true;
            page = new PageBody() { page = counter };
          //  WebProcedure.Instance.GetACBallsToSell(JsonConvert.SerializeObject(page), OnSuccessLoadingMoreACBalls, (error) => { isLoadingNewACBalls = false; textNoACBalls.text = textFail; });
        }
    }

    /// <summary>
    /// Método que se llama al traer de forma satisfactoria los datos de las ACBall del usuario a nivel general
    /// </summary>
    /// <param name="obj">Clase con los datos traidos por backend</param>
    private void OnSuccess(DataSnapshot obj)
    {
        textNoACBalls.text = "";
        Debug.Log(obj.RawJson);
        acballsContainer = new AcbBallContainer.ACBallsToSell();
        JsonConvert.PopulateObject(obj.RawJson, acballsContainer);
        JsonConvert.PopulateObject(obj.RawJson, ACBSingleton.Instance.AccountData.statsData);

        if(acballButtonContainer.childCount > 0)
        {
            foreach (Transform child in acballButtonContainer.transform)
            {
                Destroy(child.gameObject);
            }
        }
        
        if(acballButtonContainer == null || acballsContainer.items.Count <= 0)
        {
            textNoACBalls.text = textFail;
            SetSpinnerNewState(false);
            return;
        }

        acballButtonContainer.sizeDelta = new Vector2(acballButtonContainer.sizeDelta.x, acballsContainer.items.Count * acballButtonLayoutReference.preferredHeight);
        foreach (var acballItems in acballsContainer.items)
        {
            var acballButton = Instantiate(acballButtonPrefab, acballButtonContainer);
            acballButton.GetComponent<ACBallButton>().SetupButton(acballItems, LoadACBallsData, LoadACBallsData, () => { acballButtonContainer.anchoredPosition = Vector2.zero; });
            acballButton.GetComponent<Toggle>().onValueChanged.AddListener(OnToggleValueChanged);
            acballButton.GetComponent<Toggle>().group = acballButtonContainer.GetComponent<ToggleGroup>();
        }

    }

    /// <summary>
    /// Método llamado cada vez que se han traido los datos de una página de ACBalls satisfactoriamente
    /// </summary>
    /// <param name="obj">Clase con los datos de la página de ACBalls traidos de backend</param>
    private void OnSuccessLoadingMoreACBalls(DataSnapshot obj)
    {
        Debug.Log(obj.RawJson);
        acballsContainer = new AcbBallContainer.ACBallsToSell();
        JsonConvert.PopulateObject(obj.RawJson, acballsContainer);

        if(acballsContainer.items.Count <= 0)
        {
            allAcballsLoaded = true;
        }

        acballButtonContainer.sizeDelta += new Vector2(0, acballsContainer.items.Count * acballButtonLayoutReference.preferredHeight);
        foreach (var acballItems in acballsContainer.items)
        {
            var acballButton = Instantiate(acballButtonPrefab, acballButtonContainer);
            acballButton.GetComponent<ACBallButton>().SetupButton(acballItems, LoadACBallsData, LoadACBallsData, () => { acballButtonContainer.anchoredPosition = Vector2.zero; });
            acballButton.GetComponent<Toggle>().onValueChanged.AddListener(OnToggleValueChanged);
            acballButton.GetComponent<Toggle>().group = acballButtonContainer.GetComponent<ToggleGroup>();
        }

        isLoadingNewACBalls = false;
        SetSpinnerNewState(false);
    }

    /// <summary>
    /// Método que se llama cada vez que backend falla trayendo los datos
    /// </summary>
    /// <param name="obj">Clase con los datos de error que devuelve backend</param>
    private void OnFailed(WebError obj)
    {
        textNoACBalls.text = textFail;
        ACBSingleton.Instance.AlertPanel.SetupPanel("Hubo un error cargando los datos de las acballs, por favor intenta nuevamente", "", false, Close);
    }

    /// <summary>
    /// Activa o desactiva el spinner de carga del panel
    /// </summary>
    /// <param name="state">Estado que determina si se muestra o se oculta el spinner</param>
    private void SetSpinnerNewState(bool state)
    {
        GameObject spinner = GameObject.Find("Spinner_ACBall");
        for (int i = 0; i < spinner.transform.childCount; i++)
        {
            spinner.transform.GetChild(i).gameObject.SetActive(state);
        }
    }
    /// <summary>
    /// Obtiene el objeto arrastrable seleccionado
    /// </summary>
    /// <returns>Referencia del objeto arrastrable</returns>
    private Toggle GetSelectedToggle()
    {
        var toggles = acballButtonContainer.GetComponentsInChildren<Toggle>();
        return toggles.FirstOrDefault(t => t.isOn);
    }
    /// <summary>
    /// called when a toggle item change his value
    /// </summary>
    private void OnToggleValueChanged(bool status = true)
    {
        publishButton.interactable = GetSelectedToggle();
    }
    /// <summary>
    /// Muestra el panel de confirmación para publicar el item en el mercadillo
    /// </summary>
    /// <param name="itemData">Datos de la ACBall</param>
    public void ShowDialogConfirmation(AcbBallContainer.ACBallsToSell.AcBallsItems ACBallData)
    {
        panelOpener.popupPrefab = confirmPanelPrefab;
        panelOpener.OpenPopup();
        panelOpener.popup.GetComponent<PanelConfirmPublish>().Populate(ACBallData);
        panelOpener.popup.GetComponent<PanelConfirmPublish>().OnConfirmedPublish += () => { OnConfirmedPublish?.Invoke(); Close(); };
    }
    /// <summary>
    /// Intenta Abrir el panel de confirmación para publicar
    /// </summary>
    public void PublishClick()
    {
        if (GetSelectedToggle())
        {
            ShowDialogConfirmation(GetSelectedToggle().GetComponent<ACBallButton>().acballMarketItemData);
        }
    }
    #endregion
}
