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
    /// <summary>
    /// Secciones del panel de rankings
    /// </summary>
    public enum RankingSections {ClassicLeague,ActualLeague,Missions,Cards,PlayerTokens};
    /// <summary>
    /// Escalas de tiempo de rankings
    /// </summary>
    private enum TimeScales {Season,Trimester,Historic };
    /// <summary>
    /// Seccion actual seleccionada en el panel de rankings
    /// </summary>
    private RankingSections actualSection;
    /// <summary>
    /// Escala actual de tiempo de los rankings
    /// </summary>
    private TimeScales actualTimeScale;
    [SerializeField]
    [Tooltip("Panel donde se muestran los puntajes")]
    private GameObject panelScores;
    [Header("Panel components")]
    [SerializeField] [Tooltip("Bot?n que se encarga del cerrado del panel")]
    private Button goBackButton;
    [SerializeField] [Tooltip("Bot?n de conocer app")]
    private Button knowAppButton;
    [SerializeField]
    [Tooltip("Bot?n para ver m?s jugadores del ranking")]
    private Button showMorePlayersButton;
    [SerializeField] [Tooltip("Clase que controla la apertura de nuevos paneles a mostrar")]
    private PanelOpener panelOpener;
    [SerializeField] [Tooltip("Prefab del panel de sede central en versi?n HTML")]
    private GameObject headquarterHTMLPanel;
    [SerializeField] [Tooltip("Prefab del elemento donde se ve el ranking del jugador")]
    private GameObject playerRankingViewPrefab;
    [SerializeField]
    [Tooltip("Prefab del elemento donde se ve el ranking de la cartas")]
    private GameObject playerRankingTokenViewPrefab;
    [SerializeField]
    [Tooltip("Prefab del elemento donde se ve el ranking hist?rico del jugador")]
    private GameObject playerRankingViewHistoricPrefab;
    [SerializeField]
    [Tooltip("Prefab del elemento donde se ve el ranking hist?rico de los tokens")]
    private GameObject playerRankingViewHistoricTokenPrefab;
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
    [SerializeField]
    [Tooltip("Layout del icono de la liga")]
    private GameObject LayoutLeagueIcon;
    [SerializeField]
    [Tooltip("Imagen de la liga")]
    private Image imageLeagueIcon;
    [SerializeField]
    [Tooltip("sprite de la liga Actual")]
    private Sprite spriteActualLeagueIcon;
    [SerializeField]
    [Tooltip("sprite de la liga Clasica")]
    private Sprite spriteClasicLeagueIcon;
    [SerializeField]
    [Tooltip("Boton de puntajes del trimeste")]
    private Button trimesterButton;
    [SerializeField]
    [Tooltip("Boton de puntajes de la temporada")]
    private Button seasonButton;
    [SerializeField]
    [Tooltip("Boton de puntajes historicos")]
    private Button historicButton;
    [SerializeField]
    [Tooltip("Boton para cargar mas jugadores")]
    private Button loadMoreButton;
    [Header("Data management")]
    [SerializeField] [Tooltip("M?xima cantidad de entradas de ranking a mostrar")]
    private int rowsToShowInRanking;
    [SerializeField]
    [Tooltip("Visualizador de la carta")]
    private PlayerCard cardViewer;
    [SerializeField]
    [Tooltip("Texto del encabezado en los paneles de las secciones")]
    private Text headerTitleText;
    //true cuando el endpoint para cargar los puntos ha entregado un resultado
    private bool allItemsLoaded;
    //numero de paginacion
    private int counter;
    // numero total de paginas que tiene la consulta
    private int maxPages;
    //ha cargado mas jugadores
    private bool hasLoadedMorePlayer;

    #endregion

    #region Unity Methods

    /// <summary>
    /// Se ejecuta cuando el panel ha sido iniciado por primera vez en escena
    /// </summary>
    private void Start()
    {
        knowAppButton.onClick.AddListener(() => { OpenHTMLPanel(ACBSingleton.Instance.GameData.headQuartersURL); });
        
        //  SetupPanel();
        scrollViewPuntajes.GetComponentInChildren<ScrollRect>().onValueChanged.AddListener(OnScrollContent);
        goBackButton.onClick.AddListener(() => { Close(); ACBSingleton.Instance.UpdateGameData(); });
    }

    #endregion

    public void ShowCard(TokenItemData tokenData)
    {
        cardViewer.transform.parent.gameObject.SetActive(true);
        cardViewer.SetupCardData(tokenData,()=> { });
    }
    #region Public Methods



    #endregion

    #region Private Methods

    /// <summary>
    /// Abre panel HTML
    /// </summary>
    /// <param name="htmlUrl">Url de la p?gina a cargar</param>
    private void OpenHTMLPanel(string htmlUrl)
    {
        panelOpener.popupPrefab = headquarterHTMLPanel;
        panelOpener.OpenPopup();
        panelOpener.popup.GetComponent<PanelHeadquarterHTML>().OpenPanel(htmlUrl);
    }

    /// <summary>
    /// M?todo que se ejecuta cuando los datos de ranking han sido traidos de backend exitosamente, mostrando el
    /// ranking dependiendo de la seccion seleccionada
    /// </summary>
    /// <param name="obj">Datos del ranking</param>
    private void OnSuccessLoadingRanking(DataSnapshot obj)
    {
    
        HeadquarterContainerData hqContainerData = new HeadquarterContainerData();
        switch (actualSection)
        {
            default:  // actual y Classic League , missiones y cards collections
                Debug.Log(obj.RawJson);
              
                JsonConvert.PopulateObject(obj.RawJson, hqContainerData);


                maxPages = hqContainerData.total_pages;
                if (hqContainerData.current_user != null) playerView.ShowRankingView(hqContainerData.current_user,actualSection);

                if (hqContainerData.items.Count > 0)
                {
                    for (int i = 0; i < hqContainerData.items.Count; i++)
                    {
                        GameObject playerView = Instantiate(playerRankingViewPrefab, containerRectTransform);
                        playerView.GetComponent<PlayerRankingView>().ShowRankingView(hqContainerData.items[i], actualSection);
                    }
                }
                //significa que es un historico
                if(hqContainerData.data.Count > 0)
                {
                    if(hqContainerData.best_leauge.Count > 0)
                    {
                        GameObject playerView = Instantiate( playerRankingViewHistoricPrefab, containerRectTransform);
                        playerView.GetComponent<PanelSeasonRanking>().ShowRankingSeasonView(hqContainerData.best_leauge, "Mejores Temporada 21-22");
                    }
                    foreach (HeadquarterContainerData.DataSeason dataSeason in hqContainerData.data)
                    {
                        string yearText = "";
                        string quarterText = "";
                        List<HeadquarterContainerData.Season> season = new List<HeadquarterContainerData.Season>();
                        if (dataSeason.SEASON_2020.Count > 0) { season = dataSeason.SEASON_2020; yearText = "20"; }
                        if (dataSeason.SEASON_2021.Count > 0) { season = dataSeason.SEASON_2021; yearText = "21"; }
                        if (dataSeason.SEASON_2022.Count > 0) { season = dataSeason.SEASON_2022; yearText = "22"; }
                        if (dataSeason.SEASON_2023.Count > 0) { season = dataSeason.SEASON_2023; yearText = "23"; }
                        if (dataSeason.SEASON_2024.Count > 0) { season = dataSeason.SEASON_2024; yearText = "24"; }
                        if (dataSeason.SEASON_2025.Count > 0) { season = dataSeason.SEASON_2025; yearText = "25"; }
                        if (season.Count == 0) continue;
                        List<HeadquarterContainerData.RankingUserData> quarter = new List <HeadquarterContainerData.RankingUserData>();
                        foreach (HeadquarterContainerData.Season quarterSeason in season)
                        {
                            if (quarterSeason.ENE_MAR.Count > 0) { quarter = quarterSeason.ENE_MAR; quarterText = "ene-mar "; }
                            if (quarterSeason.ABR_JUN.Count > 0) {quarter = quarterSeason.ABR_JUN;quarterText = "abr-jun "; }
                            if (quarterSeason.JUL_SEP.Count > 0){ quarter = quarterSeason.JUL_SEP; quarterText = "jul-sep "; }
                            if (quarterSeason.OCT_DEC.Count > 0) {quarter = quarterSeason.OCT_DEC; quarterText = "oct-dic "; }
                            if (quarter.Count == 0) continue;
                            GameObject playerView = Instantiate(playerRankingViewHistoricPrefab, containerRectTransform);
                            playerView.GetComponent<PanelSeasonRanking>().ShowRankingSeasonView(quarter, "Mejores " + quarterText + yearText,actualSection);
                        }
                            
                    }
                }
                if(!string.IsNullOrEmpty( hqContainerData.headQuartersURL))
                {
                    knowAppButton.onClick.RemoveAllListeners();
                    knowAppButton.onClick.AddListener(() => { OpenHTMLPanel(hqContainerData.headQuartersURL); });
                }
                break;
            
         case RankingSections.PlayerTokens:
                 Debug.Log(obj.RawJson);
                TokenDataRankingContainer tokenDataRanking = new TokenDataRankingContainer();
                JsonConvert.PopulateObject(obj.RawJson, tokenDataRanking);
                maxPages = tokenDataRanking.total_pages;
                if (tokenDataRanking.items.Count > 0)
                {
                    for (int i = 0; i < tokenDataRanking.items.Count; i++)
                    {
                        GameObject playerView = Instantiate(playerRankingTokenViewPrefab, containerRectTransform);
                        playerView.GetComponent<PlayerRankingViewToken>().ShowRankingView(tokenDataRanking.items[i]);
                    }
                }
                //significa que es un historico
                if (tokenDataRanking.data.Count > 0)
                {
                    /*if (tokenDataRanking.best_leauge.Count > 0)
                    {
                        GameObject playerView = Instantiate(playerRankingViewHistoricTokenPrefab, containerRectTransform);
                        playerView.GetComponent<PanelSeasonRanking>().ShowRankingSeasonView(hqContainerData.best_leauge, "Mejores Temporada 21-22");
                    }*/
                    foreach (TokenDataRankingContainer.DataSeason dataSeason in tokenDataRanking.data)
                    {
                        string yearText = "";
                        string quarterText = "";
                        List<TokenDataRankingContainer.Season> season = new List<TokenDataRankingContainer.Season>();
                        if (dataSeason.SEASON_2020.Count > 0) { season = dataSeason.SEASON_2020; yearText = "20"; }
                        if (dataSeason.SEASON_2021.Count > 0) { season = dataSeason.SEASON_2021; yearText = "21"; }
                        if (dataSeason.SEASON_2022.Count > 0) { season = dataSeason.SEASON_2022; yearText = "22"; }
                        if (dataSeason.SEASON_2023.Count > 0) { season = dataSeason.SEASON_2023; yearText = "23"; }
                        if (dataSeason.SEASON_2024.Count > 0) { season = dataSeason.SEASON_2024; yearText = "24"; }
                        if (dataSeason.SEASON_2025.Count > 0) { season = dataSeason.SEASON_2025; yearText = "25"; }
                        if (season.Count == 0) continue;
                        List<TokenDataRanking> quarter = new List<TokenDataRanking>();
                        foreach (TokenDataRankingContainer.Season quarterSeason in season)
                        {
                            if (quarterSeason.ENE_MAR.Count > 0) { quarter = quarterSeason.ENE_MAR; quarterText = "ene-mar "; }
                            if (quarterSeason.ABR_JUN.Count > 0) { quarter = quarterSeason.ABR_JUN; quarterText = "abr-jun "; }
                            if (quarterSeason.JUL_SEP.Count > 0) { quarter = quarterSeason.JUL_SEP; quarterText = "jul-sep "; }
                            if (quarterSeason.OCT_DEC.Count > 0) { quarter = quarterSeason.OCT_DEC; quarterText = "oct-dic "; }
                            if (quarter.Count == 0) continue;
                            GameObject playerView = Instantiate(playerRankingViewHistoricTokenPrefab, containerRectTransform);
                            playerView.GetComponent<PanelSeasonRanking>().ShowRankingSeasonView(quarter, "Mejores " + quarterText + yearText);
                        }

                    }
                }
                break;
             
        }
        containerRectTransform.sizeDelta = new Vector2(containerRectTransform.sizeDelta.x, (actualTimeScale == TimeScales.Historic ? (actualSection == RankingSections.PlayerTokens ? playerRankingViewHistoricTokenPrefab.GetComponent<LayoutElement>().preferredHeight : playerRankingViewHistoricPrefab.GetComponent<LayoutElement>().preferredHeight ): (actualSection == RankingSections.PlayerTokens ? playerRankingTokenViewPrefab.GetComponent<LayoutElement>().preferredHeight : playerRankingViewPrefab.GetComponent<LayoutElement>().preferredHeight)) * (1 + containerRectTransform.childCount));
        allItemsLoaded = true;
                spinner.SetActive(false);

    }

    /// <summary>
    /// M?todo que se ejecuta cuando el ranking no ha podido ser cargado exitosamente
    /// </summary>
    /// <param name="obj">Datos de error devueltos por backend</param>
    private void OnFailedLoadingRanking(WebError obj)
    {
        ACBSingleton.Instance.AlertPanel.SetupPanel("No se ha podido cargar el ranking correctamente", "", false, Close);
        spinner.SetActive(false);
    }
    /// <summary>
    /// Carga la tabla de posiciones de la liga clásica
    /// </summary>
    public void ClassicLeaguePressed()
    {
        headerTitleText.text = "Puntos";
        actualSection = RankingSections.ClassicLeague;
               panelScores.SetActive(true);
          
        imageLeagueIcon.sprite = spriteClasicLeagueIcon;
        imageLeagueIcon.transform.parent.GetComponent<Image>().color = Color.white;
        LoadSeasonPoints();
      }
    public void ActualLeaguePressed()
    {
        headerTitleText.text = "Puntos";
        actualSection = RankingSections.ActualLeague;
        panelScores.SetActive(true);
     
        imageLeagueIcon.sprite = spriteActualLeagueIcon;
        imageLeagueIcon.transform.parent.GetComponent<Image>().color = Color.black;
        LoadSeasonPoints();
    }
    public void MissionsPressed()
    {
        headerTitleText.text = "Misiones";
        actualSection = RankingSections.Missions;
        panelScores.SetActive(true);
        LoadSeasonPoints();
    }
    public void CardsPressed()
    {
        headerTitleText.text = "Cards";
        actualSection = RankingSections.Cards;
        panelScores.SetActive(true);
        LoadSeasonPoints();
    }
    public void PlayerTokensPressed()
    {
        headerTitleText.text = "Mejores player token";
        actualSection = RankingSections.PlayerTokens;
        panelScores.SetActive(true);
        LoadSeasonPoints();
    }

    public void LoadSeasonPoints()
    {
        LayoutLeagueIcon.gameObject.SetActive(actualSection == RankingSections.ClassicLeague || actualSection == RankingSections.ActualLeague);
        playerView.gameObject.SetActive(actualSection != RankingSections.PlayerTokens);
        hasLoadedMorePlayer = false;
        loadMoreButton.transform.GetChild(0).GetComponentInChildren<Text>().text = "Ver competidores";
        actualTimeScale = TimeScales.Season;
        counter = 1;
        allItemsLoaded = false;
        spinner.SetActive(true);
        foreach (Transform child in containerRectTransform)
        {
            Destroy(child.gameObject);
        }
        containerRectTransform.sizeDelta = new Vector2(containerRectTransform.sizeDelta.x, 0f);
        seasonButton.transform.GetChild(1).gameObject.SetActive(true);
        trimesterButton.transform.GetChild(1).gameObject.SetActive(false);
        historicButton.transform.GetChild(1).gameObject.SetActive(false);
        loadMoreButton.gameObject.SetActive(actualSection != RankingSections.PlayerTokens);
        RankingBody body = new RankingBody() { page = counter, num_items = rowsToShowInRanking };
        switch (actualSection)
        {
            case RankingSections.ClassicLeague:
                WebProcedure.Instance.GetRankingsPoints(JsonConvert.SerializeObject(body), false, OnSuccessLoadingRanking, OnFailedLoadingRanking);
                break;
            case RankingSections.ActualLeague:
                WebProcedure.Instance.GetRankingsPoints(JsonConvert.SerializeObject(body), true , OnSuccessLoadingRanking, OnFailedLoadingRanking);
                break;
            case RankingSections.Missions:
                WebProcedure.Instance.GetRankingsMissionsPoints(JsonConvert.SerializeObject(body), OnSuccessLoadingRanking, OnFailedLoadingRanking);

                break;
            case RankingSections.Cards:
                WebProcedure.Instance.GetRankingsTokens(JsonConvert.SerializeObject(body), OnSuccessLoadingRanking, OnFailedLoadingRanking);
                break;
            case RankingSections.PlayerTokens:
                WebProcedure.Instance.GetAllRankingTokenCardsVictories(JsonConvert.SerializeObject(body), OnSuccessLoadingRanking, OnFailedLoadingRanking);
                break;
        }
        

    }

    public void LoadTrimesterPoints()
    {
        counter = 1;
        LayoutLeagueIcon.gameObject.SetActive(actualSection == RankingSections.ClassicLeague || actualSection == RankingSections.ActualLeague);
        playerView.gameObject.SetActive(actualSection != RankingSections.PlayerTokens);
        hasLoadedMorePlayer = false;
        loadMoreButton.transform.GetChild(0).GetComponentInChildren<Text>().text = "Ver competidores";
        actualTimeScale = TimeScales.Trimester;
        spinner.SetActive(true);
        foreach (Transform child in containerRectTransform)
        {
            Destroy(child.gameObject);
        }
        containerRectTransform.sizeDelta = new Vector2(containerRectTransform.sizeDelta.x, 0f);
        seasonButton.transform.GetChild(1).gameObject.SetActive(false);
        trimesterButton.transform.GetChild(1).gameObject.SetActive(true);
        historicButton.transform.GetChild(1).gameObject.SetActive(false);
        loadMoreButton.gameObject.SetActive(actualSection != RankingSections.PlayerTokens);
        RankingBody body = new RankingBody() { page = counter, num_items = rowsToShowInRanking };
        switch (actualSection)
        {
            case RankingSections.ClassicLeague:
             WebProcedure.Instance.GetRankingsQuarterPoints(JsonConvert.SerializeObject(body), false, OnSuccessLoadingRanking, OnFailedLoadingRanking);
                break;
            case RankingSections.ActualLeague:
             WebProcedure.Instance.GetRankingsQuarterPoints(JsonConvert.SerializeObject(body), true, OnSuccessLoadingRanking, OnFailedLoadingRanking);
                break;
            case RankingSections.Missions:
                WebProcedure.Instance.GetRankingsMissionsPointsQuarter(JsonConvert.SerializeObject(body), OnSuccessLoadingRanking, OnFailedLoadingRanking);
                break;
            case RankingSections.Cards:
                WebProcedure.Instance.GetRankingsTokensQuarter(JsonConvert.SerializeObject(body), OnSuccessLoadingRanking, OnFailedLoadingRanking);
                break;
            case RankingSections.PlayerTokens:
                WebProcedure.Instance.GetAllRankingTokenCardsOfLastQaurter(JsonConvert.SerializeObject(body), OnSuccessLoadingRanking, OnFailedLoadingRanking);
                break; 
        }
    }
    public void LoadHistoricPoints()
    {
        LayoutLeagueIcon.gameObject.SetActive(actualSection == RankingSections.ClassicLeague || actualSection == RankingSections.ActualLeague);
        playerView.gameObject.SetActive(false);
        hasLoadedMorePlayer = false;
        actualTimeScale = TimeScales.Historic;
        spinner.SetActive(true);
        foreach (Transform child in containerRectTransform)
        {
            Destroy(child.gameObject);
        }
        containerRectTransform.sizeDelta = new Vector2(containerRectTransform.sizeDelta.x, 0f);
        seasonButton.transform.GetChild(1).gameObject.SetActive(false);
        trimesterButton.transform.GetChild(1).gameObject.SetActive(false);
        historicButton.transform.GetChild(1).gameObject.SetActive(true);
        loadMoreButton.gameObject.SetActive(false);
        RankingBody body = new RankingBody() { page = counter, num_items = rowsToShowInRanking };
        switch (actualSection)
        {
            case RankingSections.ClassicLeague:
                WebProcedure.Instance.GetHistoricPoints( false, OnSuccessLoadingRanking, OnFailedLoadingRanking);
                break;
            case RankingSections.ActualLeague:
                WebProcedure.Instance.GetHistoricPoints( true, OnSuccessLoadingRanking, OnFailedLoadingRanking);
                break;
            case RankingSections.Missions:
                WebProcedure.Instance.GetHistoricMissions( OnSuccessLoadingRanking, OnFailedLoadingRanking);
                break;
            case RankingSections.Cards:
                WebProcedure.Instance.GetAllHistoricOfTokens(OnSuccessLoadingRanking, OnFailedLoadingRanking);
                break;
            case RankingSections.PlayerTokens:
                WebProcedure.Instance.GetAllHistoricOfVictories(OnSuccessLoadingRanking, OnFailedLoadingRanking);
                break;
        }
    }
    public void LoadMorePlayers()
    {
        hasLoadedMorePlayer = !hasLoadedMorePlayer;
        loadMoreButton.transform.GetChild(0).GetComponentInChildren<Text>().text = hasLoadedMorePlayer ? "Volver arriba": "Ver competidores";
        playerView.gameObject.SetActive(!hasLoadedMorePlayer);
        if (!hasLoadedMorePlayer)
        {
            if (actualTimeScale == TimeScales.Season)
                LoadSeasonPoints();
            else
                LoadTrimesterPoints();
            return;
        }
        counter = 1;
        allItemsLoaded = false;
        spinner.SetActive(true);
        foreach (Transform child in containerRectTransform)
        {
            Destroy(child.gameObject);
        }
        containerRectTransform.sizeDelta = new Vector2(containerRectTransform.sizeDelta.x, 0f);
        switch (actualSection)
        {
            case RankingSections.ClassicLeague:
                if (actualTimeScale == TimeScales.Season) WebProcedure.Instance.GetRankingCompetitors(false, OnSuccessLoadingRanking, OnFailedLoadingRanking);
                     else WebProcedure.Instance.GetCompetitorsLastQuarter(false, OnSuccessLoadingRanking, OnFailedLoadingRanking);
                break;
            case RankingSections.ActualLeague:
                if (actualTimeScale == TimeScales.Season) WebProcedure.Instance.GetRankingCompetitors(true, OnSuccessLoadingRanking, OnFailedLoadingRanking);
                   else WebProcedure.Instance.GetCompetitorsLastQuarter(true, OnSuccessLoadingRanking, OnFailedLoadingRanking);
                break;
            case RankingSections.Missions:
                if (actualTimeScale == TimeScales.Season) WebProcedure.Instance.GetCompetitorsMissionsQuarters( OnSuccessLoadingRanking, OnFailedLoadingRanking);
                else WebProcedure.Instance.GetCompetitorsMissions(OnSuccessLoadingRanking, OnFailedLoadingRanking);
                break;
            case RankingSections.Cards:
                if (actualTimeScale == TimeScales.Season) WebProcedure.Instance.GetTokenCompetitors(OnSuccessLoadingRanking, OnFailedLoadingRanking);
               else  WebProcedure.Instance.GetTokenCompetitorsLastQuarter(OnSuccessLoadingRanking, OnFailedLoadingRanking);
                break; 
            case RankingSections.PlayerTokens:
                break;
        }
    }
    /// <summary>
    /// M?todo que se ejecuta cada vez que el jugador se mueve entre el cat?logo
    /// </summary>
    /// <param name="scrollNormalizedPos">Posici?n normalizada del objeto arrastrable</param>
    private void OnScrollContent(Vector2 scrollNormalizedPos)
    {
        if (scrollNormalizedPos.y <= 0f && allItemsLoaded && containerRectTransform.childCount > 0 && counter < maxPages)
        {
            spinner.SetActive(true);
            allItemsLoaded = false;
            counter++;
            RankingBody body = new RankingBody() { page = counter , num_items = rowsToShowInRanking};
            switch (actualSection)
            {
                case RankingSections.ClassicLeague:
                    if (actualTimeScale == TimeScales.Trimester)
                        WebProcedure.Instance.GetRankingsQuarterPoints(JsonConvert.SerializeObject(body), false, OnSuccessLoadingRanking, OnFailedLoadingRanking);
                    else
                    WebProcedure.Instance.GetRankingsPoints(JsonConvert.SerializeObject(body), false, OnSuccessLoadingRanking, OnFailedLoadingRanking);

                    break;
                case RankingSections.ActualLeague:
                    if (actualTimeScale == TimeScales.Trimester)
                        WebProcedure.Instance.GetRankingsQuarterPoints(JsonConvert.SerializeObject(body), true, OnSuccessLoadingRanking, OnFailedLoadingRanking);
                    else
                        WebProcedure.Instance.GetRankingsPoints(JsonConvert.SerializeObject(body), true, OnSuccessLoadingRanking, OnFailedLoadingRanking);
                    break;
                case RankingSections.Missions:
                    if (actualTimeScale == TimeScales.Trimester)
                        WebProcedure.Instance.GetRankingsMissionsPointsQuarter(JsonConvert.SerializeObject(body), OnSuccessLoadingRanking, OnFailedLoadingRanking);
                    else
                    WebProcedure.Instance.GetRankingsMissionsPoints(JsonConvert.SerializeObject(body), OnSuccessLoadingRanking, OnFailedLoadingRanking);
                    break;
                case RankingSections.Cards:
                    if (actualTimeScale == TimeScales.Trimester)
                        WebProcedure.Instance.GetRankingsTokensQuarter(JsonConvert.SerializeObject(body), OnSuccessLoadingRanking, OnFailedLoadingRanking);
                    else
                        WebProcedure.Instance.GetRankingsTokens(JsonConvert.SerializeObject(body), OnSuccessLoadingRanking, OnFailedLoadingRanking);
                    break;
                case RankingSections.PlayerTokens:
                    if (actualTimeScale == TimeScales.Trimester)
                        WebProcedure.Instance.GetAllRankingTokenCardsOfLastQaurter(JsonConvert.SerializeObject(body), OnSuccessLoadingRanking, OnFailedLoadingRanking);
                    else
                        WebProcedure.Instance.GetAllRankingTokenCardsVictories(JsonConvert.SerializeObject(body), OnSuccessLoadingRanking, OnFailedLoadingRanking);
                    break;
            }

        }
    }
    

    #endregion
}
