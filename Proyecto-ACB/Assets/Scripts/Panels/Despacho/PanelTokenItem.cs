using Data;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using WebAPI;

/// <summary>
/// Controlador de objeto que representa la carta
/// </summary>
public class PanelTokenItem : MonoBehaviour
{

    #region Fields and Properties

    [Header("Token View components")]
    [SerializeField] [Tooltip("Imagen miniatura de la carta")]
    private Image imageThumbnail;
    [SerializeField] [Tooltip("Imagen de la carta en el equipo competitivo")]
    private GameObject team;
    [SerializeField] [Tooltip("Imagen de potenciador activo en la carta")]
    public GameObject booster;
    [SerializeField] [Tooltip("Imagen de carta con lesi�n")]
    private GameObject injured;
    [SerializeField] [Tooltip("Punto central de la carta")]
    private GameObject pivot;
    [SerializeField] [Tooltip("Se ejecuta cuando la traida de datos es fallida")]
    private UnityEvent onFailed;
    [SerializeField] [Tooltip("Datos del objeto")]
    private TokenItemData currentToken;

    [Space(5)]
    [Header("Panel to open components")]
    [SerializeField] [Tooltip("Bot�n del objeto")]
    private Button tokenButton;
    [SerializeField] [Tooltip("Clase que controla la apertura de nuevos paneles a mostrar")]
    private PanelOpener panelOpener;
    [SerializeField] [Tooltip("Prefab de carta del equipo competitivo")]
    private GameObject competitiveCardViewPrefab;

    [Space(5)]
    [Header("Card variables")]
    [SerializeField] [Tooltip("Determina si el token puede hacer uso de las funcionalidades de bot�n")]
    private bool isTokenButton = true;

    private ChallengeAcceptedData.CardChallengeData.CardItems cardData; //Datos de la carta del objeto
    private Action onCardGoBack; //Acci�n que se ejecuta al cerrar el panel de la carta

    #endregion

    #region Unity Methods

    /// <summary>
    /// Configura que el objeto al ser seleccionado abra la vista de la carta
    /// </summary>
    private void Start()
    {
        tokenButton?.onClick.AddListener(OpenCardView);
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Muestra la informaci�n de la carta en el objeto
    /// </summary>
    /// <param name="tokendata">Datos del objeto arrastrable</param>
    /// <param name="onCardGoBack">Acci�n que se ejecuta al cerrar el panel de la carta</param>
    /// <param name="spinner">Spinner de carga</param>
    public void ShowInfo(TokenItemData tokendata, Action onCardGoBack, GameObject spinner)
    {
        currentToken = tokendata;
        this.onCardGoBack = onCardGoBack;

        if(team)
            team.SetActive(currentToken.isTeam);
        if(booster)
            booster.SetActive(currentToken.isBooster);
        if(injured)
            injured.SetActive(currentToken.isInjured);

        if(tokenButton != null)
            tokenButton.interactable = true;
        
        if (!string.IsNullOrEmpty(currentToken.pathThumbnail))
        {
            WebProcedure.Instance.GetSprite(currentToken.pathThumbnail, sprite =>
            {
                if (imageThumbnail)
                {
                    imageThumbnail.sprite = sprite;
                    imageThumbnail.gameObject.SetActive(true);
                    spinner.SetActive(false);

                }

            }, error =>
            {
                onFailed.Invoke();
                spinner.SetActive(false);
            } );
        }
       
    }

    /// <summary>
    /// Muestra la informaci�n de la carta en el objeto
    /// </summary>
    /// <param name="cardData">Datos de la carta</param>
    public void ShowInfo(ChallengeAcceptedData.CardChallengeData.CardItems cardData)
    {
        this.cardData = cardData;

        if (team)
            team.SetActive(cardData.isTeam);
        if (booster)
            booster.SetActive(cardData.isBooster);
        if (injured)
            injured.SetActive(cardData.isInjured);

        if (tokenButton != null)
            tokenButton.interactable = true;

        if (!string.IsNullOrEmpty(cardData.pathThumbnail))
        {
            WebProcedure.Instance.GetSprite(cardData.pathThumbnail, sprite =>
            {
                if (imageThumbnail)
                {
                    imageThumbnail.sprite = sprite;
                    imageThumbnail.gameObject.SetActive(true);
                    
                }

            }, error =>
            {
                onFailed.Invoke();
            });
        }
    }

    /// <summary>
    /// Resetea el objeto
    /// </summary>
    public void ResetToken()
    {
       if(imageThumbnail) imageThumbnail.gameObject.SetActive(false);

        if(tokenButton != null)
            tokenButton.interactable = false;
        
       if(team != null) team.SetActive(false);
        if (booster != null) booster.SetActive(false);
        if (injured != null) injured.SetActive(false);
    }
    
    /// <summary>
    /// Muestra el objeto centrado
    /// </summary>
    /// <param name="show">Determina si se debe mostrar el objeto</param>
    public void ShowPivot(bool show)
    {
        pivot.gameObject.SetActive(show);
    }

    #endregion

    #region Inner Methods

    /// <summary>
    /// Abre el panel de vista de la carta
    /// </summary>
    private void OpenCardView()
    {
        panelOpener.popupPrefab = competitiveCardViewPrefab;
        panelOpener.OpenPopup();
        if(isTokenButton)
            panelOpener.popup.GetComponent<PanelCardCompetitiveTeam>().SetCardData(currentToken, onCardGoBack);
        else
            panelOpener.popup.GetComponent<PanelCardCompetitiveTeam>().SetCardData(cardData);
    }

    #endregion

}
