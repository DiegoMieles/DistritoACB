using Data;
using WebAPI;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
/// <summary>
/// Controla el panel de tienda del juego
/// </summary>
public class PanelJumbleSale : Panel
{
    #region Fields and properties
    /// <summary>
    /// Filtros de items del mercadillo en el mismo orden con el que se encuentran en el filterContainer
    /// </summary>
    public enum Filters {All,Mine, Potenciators, Skins, Highlights,Cards, ACBalls};
    /// <summary>
    /// Orden de prioridad con el que se van a posicionar los items en el contenedor
    /// </summary>
    public enum OrderFilters {Date, Higher, Lower };
    /// <summary>
    /// conjunto de filtros a aplicar al momento de enviar la petición de los items del mercadillo
    /// </summary>
    public List<Filters> appliedFilters { get; private set; } = new List<Filters>() { Filters.ACBalls,Filters.Cards, Filters.Skins, Filters.Potenciators, Filters.Highlights,Filters.Mine,Filters.All};
    /// <summary>
    /// orden a aplicar al  enviar la petición de los items del mercadillo
    /// </summary>
    public OrderFilters appliedOrderFilter { get; private set; } = OrderFilters.Date;
    [Header("Panel Components")]
    [SerializeField] [Tooltip("Texto con la cantidad de monedas que tiene el jugador")]
    private Text coinAmount;
    [SerializeField] [Tooltip("Cantidad de ACBCoins máxima a mostrar a nivel gráfico")]
    private float limit;
    [SerializeField] [Tooltip("Prefab del panel de confirmación de compra")]
    private GameObject mallProductPrefab;
    [SerializeField] [Tooltip("Objeto que contiene el listado de productos de la tienda")]
    private RectTransform mallProductsContainer;
    [SerializeField] [Tooltip("Botón que se encarga del cerrado del panel")]
    private Button exitButton;
    [SerializeField] [Tooltip("Area arrastrable donde se encuentran los objetos disponibles para compra en la tienda")]
    private ScrollRect scroll;
    [SerializeField] [Tooltip("Contenedor de los botones de los filtros ")]
    private GameObject filterButtonsContainer;
    [SerializeField] [Tooltip("Contenedor de los botones de los tipos de orden ")]
    private GameObject orderButtonsContainer;
    [SerializeField]
    [Tooltip("Textos de descripción de cada tipo de filtro")]
    private string filterDateText, filterHigherText, filterLowerText;
    [SerializeField]
    [Tooltip("componente texto del botón del filtro de orden")]
    private Text filterAppliedText;
    [SerializeField]
    [Tooltip("Clase que controla la apertura de nuevos paneles a mostrar")]
    private PanelOpener panelOpener;
    [SerializeField]
    [Tooltip("Prefab del panel de colecciones de cartas")]
    private GameObject cardCollectionsPrefab;
    [SerializeField]
    [Tooltip("Prefab del panel de los potenciadores del jugador")]
    private GameObject panelPotenciatorsPrefab;
    [SerializeField]
    [Tooltip("Prefab del panel de las skins del jugador")]
    private GameObject panelSkinsPrefab;
    [SerializeField]
    [Tooltip("Prefab del panel de las Acball del jugador")]
    private GameObject panelAcballPrefab;
    [SerializeField]
    [Tooltip("Prefab del panel de las highlights del jugador")]
    private GameObject panelHighlightsPrefab;
    [SerializeField]
    [Tooltip("Panel donde se selecciona el tipo de item a publicar")]
    private GameObject layoutPublish;
    [SerializeField]
    [Tooltip("campo de búsqueda del mercadillo")]
    private InputField searchBarInputField;

    private bool allItemsLoaded; //Determina si ya se han cargado todos los objetos disponibles de la tienda
    private bool isLoadingNewItems; //Determina si se encuentra cargando más objetos de la tienda
    private PageBody page; //Página actual de los objetos de tienda cargados
    private int counter; //Contador de paginas de objetos mostrados
    
    private const float DistanceToRecalcVisibility = 400.0f; //Distancia para recargar la visibilidad de los objetos de la tienda
    private const float DistanceMarginForLoad = 600.0f; //Distancia para iniciar cargado de objetos
    private float lastPos = Mathf.Infinity; //Última posición donde se encuentra el objeto arrastrable
    private string researchWord; //Palabra que se usará para buscar entre el mercadillo

    #endregion

    #region Unity Methods

    /// <summary>
    /// Se ejecuta cuando el panel ha sido iniciado por primera vez en escena configurando el panel y los
    /// objetos disponibles para comprar en la tienda
    /// </summary>
    private void Start()
    {
        searchBarInputField.onValueChanged.AddListener(OnSearchBarChanged);
        SetupPanel();
        scroll.onValueChanged.AddListener(OnScrollContent);
        
        scroll.onValueChanged.AddListener((newValue) => {
            if (Mathf.Abs(lastPos - this.scroll.content.transform.localPosition.y) >= DistanceToRecalcVisibility)
            {
                lastPos = this.scroll.content.transform.localPosition.y;
 
                RectTransform scrollTransform = this.scroll.GetComponent<RectTransform>();
                float checkRectMinY = scrollTransform.rect.yMin - DistanceMarginForLoad;
                float checkRectMaxY = scrollTransform.rect.yMax + DistanceMarginForLoad;
 
                foreach (Transform child in this.scroll.content) {
                    RectTransform childTransform = child.GetComponent<RectTransform>();
                    Vector3 positionInWord = childTransform.parent.TransformPoint(childTransform.localPosition);
                    Vector3 positionInScroll = scrollTransform.InverseTransformPoint(positionInWord);
                    float childMinY = positionInScroll.y + childTransform.rect.yMin;
                    float childMaxY = positionInScroll.y + childTransform.rect.yMax;
 
                    if (childMaxY >= checkRectMinY && childMinY <= checkRectMaxY)
                    {
                      child.GetComponent<MallObjectButton>().LoadImage();
                    } 
                    else
                    {
                      //  child.GetComponent<MallObjectButton>().DestroyImage();
                    }
                }
            }
        });
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Configura el panel con los objetos que se pueden comprar y sus datos
    /// </summary>
    public void SetupPanel()
    {
        exitButton.onClick.AddListener(() => { ACBSingleton.Instance.UpdateGameData(); Close(); });
        counter = 1;
        allItemsLoaded = false;
        List<string> filters = new List<string>();
        foreach (Filters filter in appliedFilters)
        {
            string data = "";
            switch (filter)
            {
                case Filters.ACBalls:
                    data = "ACBALL";
                    break;
                case Filters.Cards:
                    data = "TOKENCARD";
                    break;
                case Filters.Highlights:
                    data = "TOKENHIGTHLIGHT";
                    break;
                case Filters.Potenciators:
                    data = "BOOSTER";
                    break;
                case Filters.Skins:
                    data = "SKIN";
                    break;
            }
            if (!string.IsNullOrEmpty(data))
                filters.Add(data);
        }
        List<string[]> order = new List<string[]>();
        switch (appliedOrderFilter)
        {
            case OrderFilters.Date:
                order.Add(new string[] { "publication_date", "DESC" });
                break;
            case OrderFilters.Higher:
                order.Add(new string[] { "price", "DESC" });
                break;
            case OrderFilters.Lower:
                order.Add(new string[] { "price", "ASC" });
                break;
        }
        SetSpinnerNewState(true);
        JumbleSaleRequest page = new JumbleSaleRequest() { page = counter, num_items = 20, types = filters.ToArray(), order = order, user_id = "", query = researchWord };
        WebProcedure.Instance.GetJumbleSaleItems(JsonConvert.SerializeObject(page), OnSuccessLoadingMallData, OnFailedLoadingMallData);
        UpdateFilterButtons();
       
    }

    /// <summary>
    /// Agrega un nuevo filtro a la lista de filtros a aplicar en el display de los items del mercadillo , si el filtro ya existe entonces lo elimina de la lista de filtros
    /// </summary>
    /// <param name="filterEnumIndex">index del Filtro a agregar </param>
    public void SetNewFilter(int filterEnumIndex)
    {
        Filters newFilter = (Filters)filterEnumIndex;
        if (!appliedFilters.Contains(newFilter))
        {
            appliedFilters.Add(newFilter);
      
        }
        else
        {
            if(newFilter != Filters.All || (newFilter == Filters.All) && appliedFilters.Contains(Filters.Mine))
            {
                appliedFilters.Remove(newFilter);
            }
            if (newFilter == Filters.Mine   && !appliedFilters.Contains(Filters.All))
            {
                appliedFilters.Remove(newFilter);
                SetNewFilter((int)Filters.All);
            }
        }
        UpdateFilterButtons();
    }

    /// <summary>
    /// Setea un nuevo orden de prioridad en la muestra de items del mercadillo
    /// </summary>
    /// <param name="orderEnumIndex">index del orden a setear </param>
    public void SetOrderFilter(int orderEnumIndex)
    {
        appliedOrderFilter = (OrderFilters)orderEnumIndex;
        UpdateFilterButtons();
    }
    /// <summary>
    /// Actualiza los botones de filtros, Enciende la imagen que opaca los botones de los filtros no seleccionados
    /// </summary>
    public void UpdateFilterButtons()
    {
        foreach (Transform child in filterButtonsContainer.transform)
        {
            child.GetChild(0).gameObject.SetActive(!appliedFilters.Contains((Filters)(child.GetSiblingIndex())));
        }
        foreach (Transform child in orderButtonsContainer.transform)
        {
            Animator animator = child.GetComponent<Animator>();
            if ((OrderFilters)child.GetSiblingIndex() == appliedOrderFilter && animator != null)
            {
                animator.SetBool("Selected",true);
            }
            else if (animator != null)
            {
                animator.SetBool("Selected", false);
            }

        }
        string filterDescription = "";
        switch(appliedOrderFilter)
        {
            case OrderFilters.Date:
                filterDescription = filterDateText;
                break;
            case OrderFilters.Higher:
                filterDescription = filterHigherText;
                break;
            case OrderFilters.Lower:
                filterDescription = filterLowerText;
                break;
        }
        filterAppliedText.text = filterDescription;
    }
    #endregion

    #region Private Methods


    /// <summary>
    /// Método que se ejecuta cada vez que el jugador se mueve entre el católogo
    /// </summary>
    /// <param name="scrollNormalizedPos">Posición normalizada del objeto arrastrable</param>
    private void OnScrollContent(Vector2 scrollNormalizedPos)
    {
        if (scrollNormalizedPos.y <= 0.1f && !allItemsLoaded && !isLoadingNewItems)
        {
            counter++;
            SetSpinnerNewState(true);
            isLoadingNewItems = true;
            List<string> filters = new List<string>() ;
            foreach(Filters filter in appliedFilters)
            {
                string data = "";
                switch(filter)
                {
                    case Filters.ACBalls:
                        data = "ACBALL";
                        break;
                    case Filters.Cards:
                        data = "TOKENCARD";
                        break;
                    case Filters.Highlights:
                        data = "TOKENHIGTHLIGHT";
                        break;
                    case Filters.Potenciators:
                        data = "BOOSTER";
                        break;
                    case Filters.Skins:
                        data = "SKIN";
                        break;
                }
                if(!string.IsNullOrEmpty( data))
                filters.Add(data);
            }
            List<string[]> order = new List<string[]>();
            switch (appliedOrderFilter)
            {
                case OrderFilters.Date:
                    order.Add(new string[] { "publication_date", "DESC" });
                    break;
                case OrderFilters.Higher:
                    order.Add(new string[] { "price", "DESC" });
                    break;
                case OrderFilters.Lower:
                    order.Add(new string[] { "price", "ASC" });
                    break;
            }
            JumbleSaleRequest page = new JumbleSaleRequest() { page = counter, num_items = 20, types = filters.ToArray(), order = order,user_id = "", query = researchWord };
            WebProcedure.Instance.GetJumbleSaleItems(JsonConvert.SerializeObject(page), OnSuccessScrollLoadingMallData, OnFailedLoadingMallData);
        }
    }


    /// <summary>
    /// Método que se ejecuta cuando una página de objetos de la tienda ha sido satisfactoriamente cargados
    /// </summary>
    /// <param name="obj">Datos de los objetos de la tienda</param>
    private void OnSuccessLoadingMallData(DataSnapshot obj)
    {

        Debug.Log(obj.RawJson);
        JumbleSaleResult mallData = new JumbleSaleResult();
        JsonConvert.PopulateObject(obj.RawJson, mallData);
        if(mallData != null )
        {
           if(mallData.balance > 0) coinAmount.text = mallData.balance.ToString();
            if (mallData.items.Count <= 0)
            {
                allItemsLoaded = true;
            }
            for (int i = mallProductsContainer.childCount - 1; i >= 0; i--)
            {
                Destroy(mallProductsContainer.GetChild(i).gameObject);
            }
            InstanciateJumbleSaleItems(mallData.items);
        }
        isLoadingNewItems = false;
        SetSpinnerNewState(false);
    }
        /// <summary>
    /// Método que se ejecuta cuando una página de objetos de la tienda ha sido satisfactoriamente cargados al escrollear la pagina
    /// </summary>
    /// <param name="obj">Datos de los objetos de la tienda</param>
    private void OnSuccessScrollLoadingMallData(DataSnapshot obj)
    {
        Debug.Log(obj.RawJson);
        JumbleSaleResult mallData = new JumbleSaleResult();
        JsonConvert.PopulateObject(obj.RawJson, mallData);
        if(mallData != null && mallData.total_pages > 0)
        {
            if (mallData.items.Count <= 0)
            {
                allItemsLoaded = true;
            }
            InstanciateJumbleSaleItems(mallData.items);
        }
        isLoadingNewItems = false;
        SetSpinnerNewState(false);
    }

    public void InstanciateJumbleSaleItems(List<JumbleSaleResult.JumbleItems> jumbleItems)
    {
        mallProductsContainer.sizeDelta += new Vector2(0, mallProductPrefab.GetComponent<LayoutElement>().preferredHeight * jumbleItems.Count);
        for (int i = 0; i < jumbleItems.Count; i++)
        {
            if (!appliedFilters.Contains(Filters.Mine) && jumbleItems[i].seller_user_id == WebProcedure.Instance.accessData.user) continue;
            if (!appliedFilters.Contains(Filters.All) && jumbleItems[i].seller_user_id != WebProcedure.Instance.accessData.user) continue;
            GameObject productButton = Instantiate(mallProductPrefab, mallProductsContainer);
            productButton.GetComponent<JumbleSaleObjectButton>().SetupMallButton(jumbleItems[i], () => { ACBSingleton.Instance.UpdateGameData(); SetupPanel(); }, SetupPanel);
        }

    }

    /// <summary>
    /// Método que se ejecuta cuando no se han podido traer los objetos de tienda
    /// </summary>
    /// <param name="obj">Clase con los datos de error</param>
    private void OnFailedLoadingMallData(WebError obj)
    {
        Debug.Log(obj.Message);
        SetSpinnerNewState(false);
    }



    /// <summary>
    /// Asigna el valor de activación del spinner de carga
    /// </summary>
    /// <param name="state">Estado de activación del spinner carga</param>
    private void SetSpinnerNewState(bool state)
    {
        GameObject spinner = GameObject.Find("Spinner_mall");

        for (int i = 0; i < spinner.transform.childCount; i++)
        {
            spinner.transform.GetChild(i).gameObject.SetActive(state);
        }
    }
    /// <summary>
    /// Abre el panel de colecciones de cartas del jugador
    /// </summary>
    public void OpenCardsCollection()
    {
        panelOpener.popupPrefab = cardCollectionsPrefab;
        panelOpener.OpenPopup();
        panelOpener.popup.GetComponent<Panels.PanelCollectionsToPublish>().OnConfirmedPublish += () => { layoutPublish.SetActive(false); SetupPanel(); };
    }  
    /// <summary>
    /// Abre el panel de potenciadores del jugador
    /// </summary>
    public void OpenPotenciators()
    {
        panelOpener.popupPrefab = panelPotenciatorsPrefab;
        panelOpener.OpenPopup();
        panelOpener.popup.GetComponent<PanelBoostersToSale>().OnConfirmedPublish += () => { layoutPublish.SetActive(false); SetupPanel(); };
    }
    /// <summary>
    /// Abre el panel de skins que tiene el jugador
    /// </summary>
    public void OpenSkins()
    {
        panelOpener.popupPrefab = panelSkinsPrefab;
        panelOpener.OpenPopup(); 
        panelOpener.popup.GetComponent<PanelSkinsToPublish>().OnConfirmedPublish += () => { layoutPublish.SetActive(false); SetupPanel(); };
    }
    /// <summary>
    /// Abre el panel de ACBalls que tiene el jugador
    /// </summary>
    public void OpenAcball()
    {
        panelOpener.popupPrefab = panelAcballPrefab;
        panelOpener.OpenPopup();
        panelOpener.popup.GetComponent<PanelACBallToPublish>().LoadACBallsData();
        panelOpener.popup.GetComponent<PanelACBallToPublish>().OnConfirmedPublish += () => { layoutPublish.SetActive(false); SetupPanel(); };
    }
    /// <summary>
    /// Abre el panel de highlights que tiene el jugador para vender 
    /// </summary>
    public void OpenHighlights()
    {
        panelOpener.popupPrefab = panelHighlightsPrefab;
        panelOpener.OpenPopup();
        panelOpener.popup.GetComponent<Panels.PanelCollectionsToPublish>().OnConfirmedPublish += () => { layoutPublish.SetActive(false); SetupPanel(); };
    }
    /// <summary>
    /// se dispara cuando el input field recibe un nuevo valor 
    /// </summary>
    public void OnSearchBarChanged(string researchWord)
    {
        StopAllCoroutines();
        StartCoroutine(TimertoSearch(researchWord, 0.75f));

    }
    /// <summary>
    /// Temporizador que luego de x segundos si el jugador no está escribiendo , hace una búsqueda de los items del mercadillo
    /// </summary>
    /// <returns></returns>
    IEnumerator TimertoSearch(string m_researchWord,float time)
    {
        researchWord = m_researchWord;
       yield return new WaitForSeconds(time);
        counter = 1;
        List<string> filters = new List<string>();
            foreach (Filters filter in appliedFilters)
            {
                string data = "";
                switch (filter)
                {
                    case Filters.ACBalls:
                        data = "ACBALL";
                        break;
                    case Filters.Cards:
                        data = "TOKENCARD";
                        break;
                    case Filters.Highlights:
                        data = "TOKENHIGTHLIGHT";
                        break;
                    case Filters.Potenciators:
                        data = "BOOSTER";
                        break;
                    case Filters.Skins:
                        data = "SKIN";
                        break;
                }
                if (!string.IsNullOrEmpty(data))
                    filters.Add(data);
            }
            List<string[]> order = new List<string[]>();
            switch (appliedOrderFilter)
            {
                case OrderFilters.Date:
                    order.Add(new string[] { "publication_date", "DESC" });
                    break;
                case OrderFilters.Higher:
                    order.Add(new string[] { "price", "DESC" });
                    break;
                case OrderFilters.Lower:
                    order.Add(new string[] { "price", "ASC" });
                    break;
            }
            JumbleSaleRequest page = new JumbleSaleRequest() { page = counter, num_items = 20, types = filters.ToArray(), order = order, user_id = "", query = researchWord };
        SetSpinnerNewState(true);
        WebProcedure.Instance.GetJumbleSaleItems(JsonConvert.SerializeObject(page), OnSuccessLoadingMallData, OnFailedLoadingMallData);
        
    }
    #endregion
}
