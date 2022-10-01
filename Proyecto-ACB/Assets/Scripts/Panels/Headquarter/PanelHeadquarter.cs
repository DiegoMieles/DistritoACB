using Data;
using WebAPI;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

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
    [SerializeField] [Tooltip("Clase que controla la apertura de nuevos paneles a mostrar")]
    private PanelOpener panelOpener;
    [SerializeField] [Tooltip("Prefab del panel de sede central en versión HTML")]
    private GameObject headquarterHTMLPanel;
    [SerializeField] [Tooltip("Prefab del elemento donde se ve el ranking del jugador")]
    private GameObject playerRankingViewPrefab;
    [SerializeField] [Tooltip("Objeto que contiene los datos de los jugadores dentro del ranking")]
    private RectTransform containerRectTransform;
    [SerializeField] [Tooltip("Vista del jugador dentro del ranking")]
    private PlayerRankingView playerView;
    [SerializeField] [Tooltip("Spinner de carga")]
    private GameObject spinner;
    [SerializeField]
    [Tooltip("Botón de la liga clasica")]
    private Button classicLeagueButton;
    [SerializeField]
    [Tooltip("Botón de la liga actual")]
    private Button actualLeagueButton;
    [SerializeField]
    [Tooltip("Lista de puntos de la liga actual")]
    private List<HeadquarterContainerData.RankingUserData> actualLeaguePoints;
    [SerializeField]
    [Tooltip("Lista de puntos de la liga clasica")]
    private List<HeadquarterContainerData.RankingUserData> classicLeaguePoints;   
    [SerializeField]
    [Tooltip("Lista de puntos de la liga actual")]
    private List<HeadquarterContainerData.RankingUserData> playerActualLeaguePoints;
    [SerializeField]
    [Tooltip("Lista de puntos de la liga clasica")]
    private List<HeadquarterContainerData.RankingUserData> playerClassicLeaguePoints;
    [Tooltip("true si se ha seleccionado la liga clasica")]
    private bool isClassicLeague = false;
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
    /// <summary>
    /// Selecciona una liga desde el botón de rankings
    /// </summary>
    public void SelectLeague(bool isClassic)
    {
        isClassicLeague = isClassic;
        actualLeagueButton.GetComponent<Image>().color = new Color(1,1,1, isClassicLeague ? 0.5f : 1f);
        classicLeagueButton.GetComponent<Image>().color = new Color(1, 1, 1, !isClassicLeague ? 0.5f : 1f);
        PopulateLeague();
    }
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
        classicLeaguePoints.Clear();
        actualLeaguePoints.Clear();
        Debug.Log(obj.RawJson);
        HeadquarterContainerData hqContainerData = new HeadquarterContainerData();
        JsonConvert.PopulateObject(obj.RawJson, hqContainerData);

        if(hqContainerData != null)
        {
            classicLeaguePoints = hqContainerData.rankingClassic;
            actualLeaguePoints = hqContainerData.rankingCurrent;
            playerClassicLeaguePoints = hqContainerData.currentUserClassic;
            playerActualLeaguePoints= hqContainerData.currentUserCurrent;
        }
      

  
        PopulateLeague();
       /* if(hqContainerData.rankingUsers.Count > 0)
        {
            for (int i = 0; i < hqContainerData.rankingUsers.Count; i++)
            {
                GameObject playerView = Instantiate(playerRankingViewPrefab, containerRectTransform);
                playerView.GetComponent<PlayerRankingView>().ShowRankingView(hqContainerData.rankingUsers[i]);
            }
        }
       */
        spinner.SetActive(false);
        knowAppButton.onClick.AddListener(() => { OpenHTMLPanel(hqContainerData.headQuartersURL); });
    }
    /// <summary>
    /// llena el tablero de información de las ligas
    /// </summary>
    public void PopulateLeague()
    {
        foreach(Transform child in containerRectTransform)
        {
            Destroy(child.gameObject);
        }
        List<HeadquarterContainerData.RankingUserData> leagueToPopulate = isClassicLeague ? classicLeaguePoints : actualLeaguePoints;
        if(playerClassicLeaguePoints.Count >0 && playerActualLeaguePoints.Count > 0) playerView.ShowRankingView(isClassicLeague? playerClassicLeaguePoints[0] : playerActualLeaguePoints[0]);
        if (leagueToPopulate.Count > 0)
        {
            for (int i = 0; i < leagueToPopulate.Count; i++)
            {
                GameObject playerView = Instantiate(playerRankingViewPrefab, containerRectTransform);
                playerView.GetComponent<PlayerRankingView>().ShowRankingView(leagueToPopulate[i]);
            }
        }
        containerRectTransform.sizeDelta = new Vector2(containerRectTransform.sizeDelta.x, playerRankingViewPrefab.GetComponent<LayoutElement>().preferredHeight * (leagueToPopulate.Count + 1));
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
