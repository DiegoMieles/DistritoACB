using Data;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using WebAPI;

/// <summary>
/// Controlador principal del panel donde se muestran las ACBall que tiene un jugador
/// </summary>
public class PanelACBall : Panel
{
    #region Fields and properties

    [Header("Panel Components")]
    [SerializeField] [Tooltip("Button de cerrar/esconder el panel ACBall")]
    private Button closePanelButton;
    [SerializeField] [Tooltip("Prefab del bot�n donde se muestra la ACBall con su descripci�n")]
    private GameObject acballButtonPrefab;
    [SerializeField] [Tooltip("Referencia al layout encargado de mostrar los botones apilados de ACBall")]
    private LayoutElement acballButtonLayoutReference;
    [SerializeField] [Tooltip("Referencia al objeto que contiene los botones de ACBall")]
    private RectTransform acballButtonContainer;
    [SerializeField] [Tooltip("Referencia principal al espacio de scroll donde se encuentran los botones")]
    private ScrollRect scroll;
    [SerializeField] [Tooltip("Esta clase contiene los datos principales de las ACBall del usuario, los cuales se actualizan al cargar en backend")]
    private AcbBallContainer acballsContainer = new AcbBallContainer();

    private bool allAcballsLoaded; //Determina si el listado de ACBalls del usuario est� cargado
    private bool isLoadingNewACBalls; //Determina si est� cargando m�s ACBalls del usuario
    private PageBody page; //Clase que contiene los datos principales de una p�gina de ACBalls
    private int counter; //Entero que determina cual ha sido la �ltima p�gina de ACBalls cargada

    #endregion

    #region Unity Methods

    /// <summary>
    /// Este m�todo se ejecuta una vez se activa el panel, en este caso carga el evento del bot�n de cerrado de panel
    /// y carga el evento del scroll cada vez que es movido
    /// </summary>
    private void OnEnable()
    {
        closePanelButton.onClick.AddListener(() => { ACBSingleton.Instance.UpdateGameData(); Close(); });
        scroll.onValueChanged.AddListener(OnScrollContent);
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
        WebProcedure.Instance.GetACBallList(JsonConvert.SerializeObject(page), OnSuccess, OnFailed);
    }

    #endregion

    #region Inner Methods

    /// <summary>
    /// M�todo llamado cada vez que se mueve el scroll que al llegar al final del scroll
    /// Determina si hay m�s ACBalls del usuario a cargar, teniendo en cuenta en qu� 
    /// p�gina se encuentra el usuario
    /// </summary>
    /// <param name="scrollNormalizedPos">Posici�n actual del jugador en el scroll</param>
    private void OnScrollContent(Vector2 scrollNormalizedPos)
    {
        if (scrollNormalizedPos.y <= 0.1f && !allAcballsLoaded && !isLoadingNewACBalls)
        {
            counter++;
            SetSpinnerNewState(true);
            isLoadingNewACBalls = true;
            page = new PageBody() { page = counter };
            WebProcedure.Instance.GetACBallList(JsonConvert.SerializeObject(page), OnSuccessLoadingMoreACBalls, (error) => { isLoadingNewACBalls = false; });
        }
    }

    /// <summary>
    /// M�todo que se llama al traer de forma satisfactoria los datos de las ACBall del usuario a nivel general
    /// </summary>
    /// <param name="obj">Clase con los datos traidos por backend</param>
    private void OnSuccess(DataSnapshot obj)
    {
        Debug.Log(obj.RawJson);
        acballsContainer = new AcbBallContainer();
        JsonConvert.PopulateObject(obj.RawJson, acballsContainer);
        JsonConvert.PopulateObject(obj.RawJson, ACBSingleton.Instance.AccountData.statsData);

        if(acballButtonContainer.childCount > 0)
        {
            foreach (Transform child in acballButtonContainer.transform)
            {
                Destroy(child.gameObject);
            }
        }
        
        if(acballsContainer.acballsData.acballsItems.Count <= 0)
        {
            SetSpinnerNewState(false);
            return;
        }

        acballButtonContainer.sizeDelta = new Vector2(acballButtonContainer.sizeDelta.x, acballsContainer.acballsData.acballsItems.Count * acballButtonLayoutReference.preferredHeight);
        foreach (var acballItems in acballsContainer.acballsData.acballsItems)
        {
            var acballButton = Instantiate(acballButtonPrefab, acballButtonContainer);
            acballButton.GetComponent<ACBallButton>().SetupButton(acballItems, LoadACBallsData, LoadACBallsData, () => { acballButtonContainer.anchoredPosition = Vector2.zero; });
        }

    }

    /// <summary>
    /// M�todo llamado cada vez que se han traido los datos de una p�gina de ACBalls satisfactoriamente
    /// </summary>
    /// <param name="obj">Clase con los datos de la p�gina de ACBalls traidos de backend</param>
    private void OnSuccessLoadingMoreACBalls(DataSnapshot obj)
    {
        Debug.Log(obj.RawJson);
        acballsContainer = new AcbBallContainer();
        JsonConvert.PopulateObject(obj.RawJson, acballsContainer);

        if(acballsContainer.acballsData.acballsItems.Count <= 0)
        {
            allAcballsLoaded = true;
        }

        acballButtonContainer.sizeDelta += new Vector2(0, acballsContainer.acballsData.acballsItems.Count * acballButtonLayoutReference.preferredHeight);
        foreach (var acballItems in acballsContainer.acballsData.acballsItems)
        {
            var acballButton = Instantiate(acballButtonPrefab, acballButtonContainer);
            acballButton.GetComponent<ACBallButton>().SetupButton(acballItems, LoadACBallsData, LoadACBallsData, () => { acballButtonContainer.anchoredPosition = Vector2.zero; });
        }

        isLoadingNewACBalls = false;
        SetSpinnerNewState(false);
    }

    /// <summary>
    /// M�todo que se llama cada vez que backend falla trayendo los datos
    /// </summary>
    /// <param name="obj">Clase con los datos de error que devuelve backend</param>
    private void OnFailed(WebError obj)
    {
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

    #endregion
}
