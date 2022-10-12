using Data;
using WebAPI;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controla el panel de sede central
/// </summary>
public class PanelHeadquarter : Panel
{
    #region Fields and Properties

    [Header("Panel components")]
    [SerializeField] [Tooltip("Botón que se encarga del cerrado del panel")]
    private Button goBackButton;
    [SerializeField] [Tooltip("Botón de conocer app")]
    private Button knowAppButton;
    [SerializeField]
    [Tooltip("Botón para ver más jugadores del ranking")]
    private Button showMorePlayersButton;
    [SerializeField] [Tooltip("Clase que controla la apertura de nuevos paneles a mostrar")]
    private PanelOpener panelOpener;
    [SerializeField] [Tooltip("Prefab del panel de sede central en versión HTML")]
    private GameObject headquarterHTMLPanel;
    [SerializeField] [Tooltip("Prefab del elemento donde se ve el ranking del jugador")]
    private GameObject playerRankingViewPrefab;
    [SerializeField]
    [Tooltip("Prefab del elemento donde se ve el ranking histórico del jugador")]
    private GameObject playerRankingViewHistoricPrefab;
    [SerializeField] [Tooltip("Objeto que contiene los datos de los jugadores dentro del ranking")]
    private RectTransform containerRectTransform;
    [SerializeField]
    [Tooltip("Objeto que contiene los datos de los jugadores dentro del ranking")]
    private RectTransform containerHistoricRectTransform;
    [SerializeField] [Tooltip("Vista del jugador dentro del ranking")]
    private PlayerRankingView playerView;
    [SerializeField] [Tooltip("Spinner de carga")]
    private GameObject spinner;
    [SerializeField]
    [Tooltip("Scrollview de los puntajes")]
    private GameObject scrollViewPuntajes;
    [SerializeField]
    [Tooltip("Scrollview de los puntajes historicos")]
    private GameObject scrollViewHistoric;

    [Header("Data management")]
    [SerializeField] [Tooltip("Máxima cantidad de entradas de ranking a mostrar")]
    private int rowsToShowInRanking;

    #endregion

    #region Unity Methods

    /// <summary>
    /// Se ejecuta cuando el panel ha sido iniciado por primera vez en escena
    /// </summary>
    private void Start()
    {
        SetupPanel();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Configura el panel de ranking y el funcionamiento de los botones del panel
    /// </summary>
    public void SetupPanel()
    {
        goBackButton.onClick.AddListener(() => { Close(); ACBSingleton.Instance.UpdateGameData(); });
        RankingBody body = new RankingBody() { rowQuantity = rowsToShowInRanking };
        WebProcedure.Instance.GetRankingList(JsonConvert.SerializeObject(body), OnSuccessLoadingRanking, OnFailedLoadingRanking);
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Abre panel HTML
    /// </summary>
    /// <param name="htmlUrl">Url de la página a cargar</param>
    private void OpenHTMLPanel(string htmlUrl)
    {
        panelOpener.popupPrefab = headquarterHTMLPanel;
        panelOpener.OpenPopup();
        panelOpener.popup.GetComponent<PanelHeadquarterHTML>().OpenPanel(htmlUrl);
    }

    /// <summary>
    /// Método que se ejecuta cuando los datos de ranking han sido traidos de backend exitosamente, mostrando el
    /// ranking y los datos de los jugadores
    /// </summary>
    /// <param name="obj">Datos del ranking</param>
    private void OnSuccessLoadingRanking(DataSnapshot obj)
    {
        Debug.Log(obj.RawJson);
        HeadquarterContainerData hqContainerData = new HeadquarterContainerData();
        JsonConvert.PopulateObject(obj.RawJson, hqContainerData);

        containerRectTransform.sizeDelta = new Vector2(containerRectTransform.sizeDelta.x, playerRankingViewPrefab.GetComponent<LayoutElement>().preferredHeight * (hqContainerData.currentUser.Count + hqContainerData.rankingUsers.Count));

        playerView.ShowRankingView(hqContainerData.currentUser[0]);

        if(hqContainerData.rankingUsers.Count > 0)
        {
            for (int i = 0; i < hqContainerData.rankingUsers.Count; i++)
            {
                GameObject playerView = Instantiate(playerRankingViewPrefab, containerRectTransform);
                playerView.GetComponent<PlayerRankingView>().ShowRankingView(hqContainerData.rankingUsers[i]);
            }
        }

        spinner.SetActive(false);
        knowAppButton.onClick.AddListener(() => { OpenHTMLPanel(hqContainerData.headQuartersURL); });
    }
    
    /// <summary>
    /// Método que se ejecuta cuando el ranking no ha podido ser cargado exitosamente
    /// </summary>
    /// <param name="obj">Datos de error devueltos por backend</param>
    private void OnFailedLoadingRanking(WebError obj)
    {
        ACBSingleton.Instance.AlertPanel.SetupPanel("No se ha podido cargar el ranking correctamente", "", false, Close);
        spinner.SetActive(false);
    }
    #endregion
}
