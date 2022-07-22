using Data;
using WebAPI;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controla el panel de tienda del juego
/// </summary>
public class PanelMall : Panel
{
    #region Fields and properties

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

    private bool allItemsLoaded; //Determina si ya se han cargado todos los objetos disponibles de la tienda
    private bool isLoadingNewItems; //Determina si se encuentra cargando más objetos de la tienda
    private PageBody page; //Página actual de los objetos de tienda cargados
    private int counter; //Contador de paginas de objetos mostrados
    
    private const float DistanceToRecalcVisibility = 400.0f; //Distancia para recargar la visibilidad de los objetos de la tienda
    private const float DistanceMarginForLoad = 600.0f; //Distancia para iniciar cargado de objetos
    private float lastPos = Mathf.Infinity; //Última posición donde se encuentra el objeto arrastrable

    #endregion

    #region Unity Methods

    /// <summary>
    /// Se ejecuta cuando el panel ha sido iniciado por primera vez en escena configurando el panel y los
    /// objetos disponibles para comprar en la tienda
    /// </summary>
    private void Start()
    {
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
                        child.GetComponent<MallObjectButton>().DestroyImage();
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
        UpdateCoinsText();
        exitButton.onClick.AddListener(() => { ACBSingleton.Instance.UpdateGameData(); Close(); });
        counter = 1;
        allItemsLoaded = false;
        PageBody page = new PageBody() { page = counter };
        WebProcedure.Instance.GetPostMall(JsonConvert.SerializeObject(page), OnSuccessLoadingMallData, OnFailedLoadingMallData);
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
            page = new PageBody() { page = counter };
            WebProcedure.Instance.GetPostMall(JsonConvert.SerializeObject(page), OnSuccessLoadingMoreItems, (error) => { isLoadingNewItems = false; });
        }
    }

    /// <summary>
    /// Método que se ejecuta cuando los objetos de tienda han sido satisfactoriamente cargados
    /// </summary>
    /// <param name="obj">Datos de los objetos de la tienda</param>
    private void OnSuccessLoadingMallData(DataSnapshot obj)
    {
        Debug.Log(obj.RawJson);
        MallContainerData mallData = new MallContainerData();
        JsonConvert.PopulateObject(obj.RawJson, mallData);

        mallProductsContainer.sizeDelta = new Vector2(mallProductsContainer.sizeDelta.x, mallProductPrefab.GetComponent<LayoutElement>().preferredHeight * mallData.mallData.mallItems.Count);

        if(mallData.mallData.mallItems.Count <= 0)
        {
            SetSpinnerNewState(false);
            return;
        }

        for (int i = 0; i < mallData.mallData.mallItems.Count; i++)
        {
            GameObject productButton = Instantiate(mallProductPrefab, mallProductsContainer);
            productButton.GetComponent<MallObjectButton>().SetupMallButton(mallData.mallData.mallItems[i], UpdateCoinsText);
        }
    }

    /// <summary>
    /// Método que se ejecuta cuando una página de objetos de la tienda ha sido satisfactoriamente cargados
    /// </summary>
    /// <param name="obj">Datos de los objetos de la tienda</param>
    private void OnSuccessLoadingMoreItems(DataSnapshot obj)
    {
        Debug.Log(obj.RawJson);
        MallContainerData mallData = new MallContainerData();
        JsonConvert.PopulateObject(obj.RawJson, mallData);

        mallProductsContainer.sizeDelta += new Vector2(0, mallProductPrefab.GetComponent<LayoutElement>().preferredHeight * mallData.mallData.mallItems.Count);

        if (mallData.mallData.mallItems.Count <= 0)
        {
            allItemsLoaded = true;
        }

        for (int i = 0; i < mallData.mallData.mallItems.Count; i++)
        {
            GameObject productButton = Instantiate(mallProductPrefab, mallProductsContainer);
            productButton.GetComponent<MallObjectButton>().SetupMallButton(mallData.mallData.mallItems[i], UpdateCoinsText);
        }

        isLoadingNewItems = false;
        SetSpinnerNewState(false);
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
    /// Actualiza las monedas que tiene el jugador
    /// </summary>
    private void UpdateCoinsText() => coinAmount.text = Mathf.Clamp(ACBSingleton.Instance.AccountData.statsData.coinsBalance, 0, limit).ToString();

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

    #endregion
}
