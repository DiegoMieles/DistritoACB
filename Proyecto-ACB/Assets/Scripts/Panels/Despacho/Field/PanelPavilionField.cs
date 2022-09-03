using Data;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using WebAPI;

/// <summary>
/// Controla el panel de desafio en la cancha del pabellón
/// </summary>
public class PanelPavilionField : Panel
{
    #region Fields and properties

    [Header("Panel components")]
    [SerializeField] [Tooltip("Botón que se encarga del cerrado del panel")]
    private Button goBackButton;
    [SerializeField] [Tooltip("Contenedor de los desafios de la cancha")]
    private RectTransform fieldDataContainer;
    [SerializeField] [Tooltip("Prefab del botón donde se muestran los desafios completados por el jugador")]
    private GameObject playerFieldPrefab;
    [SerializeField] [Tooltip("Acción que se ejecuta al no poder traer los datos de desafio desde backend")]
    private UnityEvent onFailed;
    [SerializeField] [Tooltip("Texto que se muestra cuando el jugador no ha realizado desafios")]
    private Text  textNoChallenges;
    [SerializeField,TextArea] [Tooltip("Texto que se muestra cuando el jugador no ha realizado desafios")]
    private string textFail;
    [SerializeField] [Tooltip("Referencia principal al scroll contenedor de los desafios de cancha")]
    private ScrollRect itemsScroll;

    private bool allItemsAreLoaded;
    private bool isLoadingNewItems;
    private int counter;

    
    private const float DistanceToRecalcVisibility = 400.0f; //Distancia para recargar la visibilidad de los objetos de la tienda
    private const float DistanceMarginForLoad = 600.0f; //Distancia para iniciar cargado de objetos
    private float lastPos = Mathf.Infinity; //�ltima posici�n donde se encuentra el objeto arrastrable

    [SerializeField]
    [Tooltip("botón de la liga actual")]
    private Button actualLeagueButton;
    [SerializeField]
    [Tooltip("botón de la liga clásica")]
    private Button clasicLeagueButton;
    [Tooltip("se está mostrando la liga clásica? ")]
    public  bool isclasicLeague;
    #endregion

    #region Unity Methods

    /// <summary>
    /// Al crear el panel dentro del juego carga los datos de los desafios
    /// </summary>
    private void OnEnable()
    {
        SwitchLEague(false);
        goBackButton.onClick.AddListener(() => { ACBSingleton.Instance.PanelBuildingSelection.ResetCachedMapData(); Close(); });
        counter = 1;
    }

    #endregion

    #region Inner Methods
    /// <summary>
    /// Cambia la liga que se va a mostraar
    /// </summary>
    public void SwitchLEague(bool isClasic)
    {
        
        if (isClasic != isclasicLeague) counter = 1;
        isclasicLeague = isClasic;
        if (isclasicLeague) UpdatePavilionViewClasicLeague(); else UpdatePavilionViewActualLeague();
        clasicLeagueButton.image.color = isclasicLeague ? new Color(1f, 1f, 1f, 1f) : new Color(1f, 1f, 1f, 0.5f);
        actualLeagueButton.image.color = !isclasicLeague ? new Color(1f, 1f, 1f, 1f) : new Color(1f, 1f, 1f, 0.5f);
    }

    /// <summary>
    /// Carga los datos de desafios llamando a backend de la liga actual
    /// </summary>
    private void UpdatePavilionViewActualLeague()
    {
      
        itemsScroll.onValueChanged.AddListener(OnScrollMove);

        itemsScroll.onValueChanged.AddListener((newValue) => {
            if (Mathf.Abs(lastPos - this.itemsScroll.content.transform.localPosition.y) >= DistanceToRecalcVisibility)
            {
                lastPos = this.itemsScroll.content.transform.localPosition.y;
 
                RectTransform scrollTransform = this.itemsScroll.GetComponent<RectTransform>();
                float checkRectMinY = scrollTransform.rect.yMin - DistanceMarginForLoad;
                float checkRectMaxY = scrollTransform.rect.yMax + DistanceMarginForLoad;
 
                foreach (Transform child in itemsScroll.content) {
                    RectTransform childTransform = child.GetComponent<RectTransform>();
                    Vector3 positionInWord = childTransform.parent.TransformPoint(childTransform.localPosition);
                    Vector3 positionInScroll = scrollTransform.InverseTransformPoint(positionInWord);
                    float childMinY = positionInScroll.y + childTransform.rect.yMin;
                    float childMaxY = positionInScroll.y + childTransform.rect.yMax;
 
                    if (childMaxY >= checkRectMinY && childMinY <= checkRectMaxY)
                    {
                        var challlenge = child.GetComponent<ChallengeFieldButton>();
                        challlenge.LoadImage();
                        challlenge.LoadImageAvatar();
                        Debug.Log("LoadImage");
                    } 
                    else
                    {
                        var challlenge = child.GetComponent<ChallengeFieldButton>();
                        challlenge.DestroyImage();
                        challlenge.DestroyImageAvatar();
                      Debug.Log("DestroyImage");
                    }
                }
            }
        });


        isLoadingNewItems = true;
        allItemsAreLoaded = false;
        PageBody initialPage = new PageBody() { page = counter };
        WebProcedure.Instance.GetChallengesCancha(JsonConvert.SerializeObject(initialPage), OnSuccess, OnFailed,!isclasicLeague);
    }

    
    private void OnScrollMove(Vector2 actualPosition)
    {
        if (actualPosition.y <= 0.1f && !allItemsAreLoaded && !isLoadingNewItems)
        {
            counter++;
            SetNewSpinnerActiveState(true);
            isLoadingNewItems = true;
            SwitchLEague(isclasicLeague);
        }
    }

    /// <summary>
    /// Carga los datos de desafios llamando a backend de la liga clásica
    /// </summary>
    private void UpdatePavilionViewClasicLeague()
    {

       
        itemsScroll.onValueChanged.AddListener(OnScrollMove);

        itemsScroll.onValueChanged.AddListener((newValue) => {
            if (Mathf.Abs(lastPos - this.itemsScroll.content.transform.localPosition.y) >= DistanceToRecalcVisibility)
            {
                lastPos = this.itemsScroll.content.transform.localPosition.y;

                RectTransform scrollTransform = this.itemsScroll.GetComponent<RectTransform>();
                float checkRectMinY = scrollTransform.rect.yMin - DistanceMarginForLoad;
                float checkRectMaxY = scrollTransform.rect.yMax + DistanceMarginForLoad;

                foreach (Transform child in itemsScroll.content)
                {
                    RectTransform childTransform = child.GetComponent<RectTransform>();
                    Vector3 positionInWord = childTransform.parent.TransformPoint(childTransform.localPosition);
                    Vector3 positionInScroll = scrollTransform.InverseTransformPoint(positionInWord);
                    float childMinY = positionInScroll.y + childTransform.rect.yMin;
                    float childMaxY = positionInScroll.y + childTransform.rect.yMax;

                    if (childMaxY >= checkRectMinY && childMinY <= checkRectMaxY)
                    {
                        var challlenge = child.GetComponent<ChallengeFieldButton>();
                        challlenge.LoadImage();
                        challlenge.LoadImageAvatar();
                        Debug.Log("LoadImage");
                    }
                    else
                    {
                        var challlenge = child.GetComponent<ChallengeFieldButton>();
                        challlenge.DestroyImage();
                        challlenge.DestroyImageAvatar();
                        Debug.Log("DestroyImage");
                    }
                }
            }
        });


        isLoadingNewItems = true;
        allItemsAreLoaded = false;
        PageBody initialPage = new PageBody() { page = counter };
        WebProcedure.Instance.GetChallengesCancha(JsonConvert.SerializeObject(initialPage), OnSuccess, OnFailed,!isclasicLeague);
    }
    /// <summary>
    /// Método que se ejecuta cuando los desafios han sido correctamente cargados desde backend
    /// </summary>
    /// <param name="obj">Datos de los desafios</param>
    private void OnSuccess(DataSnapshot obj)
    {
        Debug.Log(obj.RawJson);
        ChallengesField challengesFields = new ChallengesField();
        JsonConvert.PopulateObject(obj.RawJson, challengesFields);

        if(challengesFields.page <= 1 && challengesFields.rowCount > 0)
            CheckChallenges(challengesFields);

        if (challengesFields.rowCount <= 0)
        {
            allItemsAreLoaded = true;
            SetNewSpinnerActiveState(false);
            return;
        }
        fieldDataContainer.sizeDelta = new Vector2(fieldDataContainer.sizeDelta.x, 0);
        foreach (Transform child in fieldDataContainer)
        {
            Destroy(child.gameObject);
        }
        fieldDataContainer.sizeDelta += new Vector2(0, playerFieldPrefab.GetComponent<LayoutElement>().preferredHeight * challengesFields.challengeData.challengeItems.Count);

        if (challengesFields.challengeData.challengeItems != null && challengesFields.challengeData.challengeItems.Count > 0)
        {
            foreach (var challengeData in challengesFields.challengeData.challengeItems)
            {
                GameObject prefab = Instantiate(playerFieldPrefab, fieldDataContainer.transform);
                prefab.GetComponent<ChallengeFieldButton>().SetupChallengeButton(challengeData);
            }
        }

        SetNewSpinnerActiveState(false);
        isLoadingNewItems = false;
    }

    /// <summary>
    /// Desactiva o activa el spinner de carga
    /// </summary>
    private void SetNewSpinnerActiveState(bool newState)
    {
        GameObject spinner = GameObject.Find("Spinner_cancha");
        for(int i=0; i<spinner.transform.childCount; i++)
        {
            spinner.transform.GetChild(i).gameObject.SetActive(newState);
        }
    }

    /// <summary>
    /// Método que se ejecuta cuando los desafios no han sido correctamente cargados desde backend
    /// </summary>
    /// <param name="obj">Clase con los datos del error</param>
    private void OnFailed(WebError obj)
    {
        isLoadingNewItems = false;
        onFailed?.Invoke();
        SetNewSpinnerActiveState(false);
    }

    /// <summary>
    /// Verifica que el jugador tenga desafios hechos previamente
    /// </summary>
    private void CheckChallenges(ChallengesField challengesFields)
    {
        if (challengesFields?.challengeData?.challengeItems?.Count == 0 )
        {
            textNoChallenges.text = textFail;
            SetNewSpinnerActiveState(false);
        }
        else
        {
            textNoChallenges.text = string.Empty;   
        }
    }

    
    #endregion

}
