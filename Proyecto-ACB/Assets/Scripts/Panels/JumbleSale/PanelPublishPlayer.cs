using Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using WebAPI;

public class PanelPublishPlayer : Panel
{
    #region Fields and properties
    //Evento que se dispara al completar la publicación en el mercadillo
    public event PanelConfirmPublish.VoidDelegate OnConfirmedPublish;
    [Header("Panel main components")]
    [SerializeField] [Tooltip("Determina si es una colección")]
    private bool isColeccion;
    [SerializeField] [Tooltip("Título del panel")]
    private Text textTitle;
    [SerializeField] [Tooltip("Contenedor de los objetos arrastrables")]
    private GridLayoutGroup gridLayoutGroupCards;
    [SerializeField] [Tooltip("Contenedor de los objetos arrastrables")]
    private ToggleGroup toggleGroup;
    [SerializeField] [Tooltip("Botón de añadir jugador al equipo competitivo")]
    private Button addPlayerButton;
    [SerializeField] [Tooltip("Objeto arrastrable con datos de la carta")]
    private PanelTokenItemToggle panelCardItem;
    [SerializeField] [Tooltip("Se ejecuta cuando la traida de datos de liga es fallida")]
    private UnityEvent onFailed;
    [SerializeField] [Tooltip("Datos del contenedor de los objetos arrastrables")]
    private List<TokenItemData> tokenContainer = new List<TokenItemData>();
    [SerializeField] [Tooltip("Datos del contenedor de hightlights de los objetos arrastrables")]
    private SubCollectionHiglightDataToPublish tokenHighlightsContainer = new SubCollectionHiglightDataToPublish();
    [SerializeField] [Tooltip("Primer título de alerta para añadir carta al equipo competitivo")]
    private string alertAdds = "¿Quieres añadir el token a tu Equipo Competitivo?";
    [SerializeField] [Tooltip("Segundo título de alerta para añadir carta al equipo competitivo")]
    private string alertFailAdds = "Elige el token que quieres añadir a tu Equipo.";
    [SerializeField]
    [Tooltip("Objeto encargado de abrir nuevos páneles")]
    private PanelOpener panelOpener;
    [SerializeField]
    [Tooltip("Prefab del panel de selección de precio del token")]
    private GameObject panelPublish;
    [Space(10)]
    [Header("Dragable reference")]
    [SerializeField] [Tooltip("Posición final de objetos arrastrables")]
    private GameObject dragableTargetPosition;
    [SerializeField] [Tooltip("Panel de carta del jugador")]
    private PlayerCard playerCard;
    [SerializeField] [Tooltip("Spinner de carga")]
    private GameObject Spinner;

    private PanelTokenItemToggle draggedToogle;
    private int dragablesCount; //Contador de elementos arrastrables
    private int cardId; //Id de la carta
    private string title; //Texto de título del panel

    #endregion

    #region Unity Methods

    /// <summary>
    /// Suscribe acción de cerrar panel del equipo competitivo
    /// </summary>
    private void OnEnable()
    {
        PanelTeamCompetitivo.OnClose += Close;
    }

    /// <summary>
    /// Desuscribe el evento de cerrar panel al destruirse el objeto
    /// </summary>
    private void OnDestroy()
    {
        PanelTeamCompetitivo.OnClose -= Close;
    }

    /// <summary>
    /// Asigna que ningún objeto está siendo arrastrado
    /// </summary>
    private void Start()
    {
        draggedToogle = null;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Carga los datos de la carta
    /// </summary>
    /// <param name="cardId">Id de la carta</param>
    /// <param name="title">Título de la colección</param>
    public void CallInfo(int cardId, string title,bool isJumbleSale = false)
    {
        OnToggleValueChanged(true);
        textTitle.text = title;
        this.title = title;
        this.cardId = cardId;
        var cardbody = JsonConvert.SerializeObject(new BodyToken(){ card_id = cardId}) ;

        if (isColeccion)
        {
            WebProcedure.Instance.GetCardsUserToSell(cardbody,snapshot =>
            {
                Debug.Log(snapshot.RawJson);
                tokenContainer = new List<TokenItemData>();
    JsonConvert.PopulateObject(snapshot.RawJson, tokenContainer);
                addPlayerButton?.onClick.AddListener(TryToPublish);

                if (gridLayoutGroupCards.transform.childCount > 0)
                {
                    for (int i = 0; i < gridLayoutGroupCards.transform.childCount; i++)
                    {
                        Destroy(gridLayoutGroupCards.transform.GetChild(i).gameObject);
                    }
                }

                if (tokenContainer != null && tokenContainer.Count > 0)
                {
                    dragablesCount = 0;
                    foreach (var transactionData in tokenContainer)
                    {
                        var prefab= Instantiate(panelCardItem, gridLayoutGroupCards.transform);

                        if (dragableTargetPosition != null)
                        {
                            gridLayoutGroupCards.GetComponent<RectTransform>().sizeDelta = new Vector2((prefab.GetComponent<RectTransform>().sizeDelta.x + 80) * tokenContainer.Count, gridLayoutGroupCards.GetComponent<RectTransform>().sizeDelta.y);
                            prefab.GetComponent<RectTransform>().anchoredPosition = new Vector2(((prefab.GetComponent<RectTransform>().sizeDelta.x + 80) * dragablesCount) + (prefab.GetComponent<RectTransform>().sizeDelta.x / 2) + 80, 0);
                            prefab.ShowInfo(transactionData, dragableTargetPosition, () => { UpdatePlayerCardView(transactionData, prefab); }, null, OnToggleValueChanged);
                            dragablesCount++;
                        }
                        else
                            prefab.ShowInfo(transactionData, null, null, () => { CallInfo(cardId, title); }, OnToggleValueChanged);

                        if (toggleGroup)
                        {
                            prefab.toggle.group = toggleGroup;  
                        }
                    } 
                }
                else
                {
                    ClosedSpinner();
                }
            
            }, error =>
            {
                onFailed.Invoke();
                ClosedSpinner();
            });
        }
        else
        {
             cardbody = JsonConvert.SerializeObject(new BodyTokenHighlight() { highlight_id = cardId });
            WebProcedure.Instance.GetHighlightsToSell(cardbody,snapshot =>
            {
                Debug.Log(snapshot.RawJson);
                tokenHighlightsContainer = new SubCollectionHiglightDataToPublish();
                JsonConvert.PopulateObject(snapshot.RawJson, tokenHighlightsContainer);
                addPlayerButton?.onClick.AddListener(TryToPublish);

                if (gridLayoutGroupCards.transform.childCount > 0)
                {
                    for(int i = 0; i < gridLayoutGroupCards.transform.childCount; i++)
                    {
                        Destroy(gridLayoutGroupCards.transform.GetChild(i).gameObject);
                    }
                }

                if (tokenHighlightsContainer != null)
                {
                    dragablesCount = 0;
                    foreach (var transactionData in tokenHighlightsContainer.items)
                    {
                        var prefab= Instantiate(panelCardItem, gridLayoutGroupCards.transform);

                        if (dragableTargetPosition != null)
                        {
                            gridLayoutGroupCards.GetComponent<RectTransform>().sizeDelta = new Vector2((prefab.GetComponent<RectTransform>().sizeDelta.x + 80) * tokenHighlightsContainer.items.Count, gridLayoutGroupCards.GetComponent<RectTransform>().sizeDelta.y);
                            prefab.GetComponent<RectTransform>().anchoredPosition = new Vector2(((prefab.GetComponent<RectTransform>().sizeDelta.x + 80) * dragablesCount) + (prefab.GetComponent<RectTransform>().sizeDelta.x / 2) + 80, 0);
                            prefab.ShowInfo(transactionData, dragableTargetPosition, null,null,OnToggleValueChanged);
                            dragablesCount++;
                        }
                        else
                            prefab.ShowInfo(transactionData, null, null, null ,OnToggleValueChanged);

                        if (toggleGroup)
                        {
                            prefab.toggle.group = toggleGroup;  
                        }
                    }
                    ClosedSpinner();
                }
                else
                {
                    ClosedSpinner();
                }
            
            }, error =>
            {
                onFailed.Invoke();
                ClosedSpinner();
            });
        }
    }

    /// <summary>
    /// Añadir carta al equipo competitivo
    /// </summary>
    public void TryToPublish()
    {
        if (GetSelectedToggle())
        {
            PanelTokenItemToggle toggleToken = GetSelectedToggle().GetComponent<PanelTokenItemToggle>();
            if(toggleToken != null )
            {
                if (toggleToken.currentHighLight.id != 0) ShowDialogConfirmation(toggleToken.currentHighLight);
                else ShowDialogConfirmation(toggleToken.currentToken);
            }
        }
    }
    /// <summary>
    /// Muestra el panel de confirmación para publicar el item en el mercadillo
    /// </summary>
    /// <param name="itemData">Datos de la skin</param>
    public void ShowDialogConfirmation(TokenItemData itemData)
    {
        panelOpener.popupPrefab = panelPublish;
        panelOpener.OpenPopup();
        panelOpener.popup.GetComponent<PanelConfirmPublish>().Populate(itemData);
        panelOpener.popup.GetComponent<PanelConfirmPublish>().OnConfirmedPublish += () => { OnConfirmedPublish?.Invoke(); Close(); };
    } 
    /// <summary>
    /// Muestra el panel de confirmación para publicar el highlight en el mercadillo
    /// </summary>
    /// <param name="itemData">Datos de la skin</param>
    public void ShowDialogConfirmation(HighLightData.HigthlightItems highlight)
    {
        panelOpener.popupPrefab = panelPublish;
        panelOpener.OpenPopup();
        panelOpener.popup.GetComponent<PanelConfirmPublish>().Populate(highlight);
        panelOpener.popup.GetComponent<PanelConfirmPublish>().OnConfirmedPublish += () => { OnConfirmedPublish?.Invoke(); Close(); };
    }



    /// <summary>
    /// Obtiene el objeto arrastrable seleccionado
    /// </summary>
    /// <returns>Referencia del objeto arrastrable</returns>
    private Toggle GetSelectedToggle()
    {
        var toggles = gridLayoutGroupCards.transform.GetComponentsInChildren<Toggle>();
        return toggles.FirstOrDefault(t => t.isOn);
    }

    /// <summary>
    /// Actualiza vista de las caetas
    /// </summary>
    /// <param name="cardNewData">Datos de la carta</param>
    /// <param name="draggedToogle">Objeto arrastrable</param>
    private void UpdatePlayerCardView(TokenItemData cardNewData, PanelTokenItemToggle draggedToogle)
    {
        if (this.draggedToogle != null)
            this.draggedToogle.Dragable.ResetDragable();

        Action actionView = null;
        if (!cardNewData.isTeam)
            actionView = null;

        playerCard.SetupCardData(cardNewData, actionView, true, cardNewData.isTeam, true, true, () => { CallInfo(cardId, title); });
        gridLayoutGroupCards.CalculateLayoutInputHorizontal();
        
        this.draggedToogle = draggedToogle;
    }

    /// <summary>
    /// Oculta el spinner de carga
    /// </summary>
    private void ClosedSpinner()
    {
        Spinner.gameObject.SetActive(false);
    }
    /// <summary>
    /// called when a toggle item change his value
    /// </summary>
    private void OnToggleValueChanged(bool status)
    {
        addPlayerButton.interactable = GetSelectedToggle();
    }
    #endregion
}
