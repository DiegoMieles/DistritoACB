using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using WebAPI;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controla los datos y el funcionamiento de una carta de jugador
/// </summary>
public class PlayerCard : MonoBehaviour
{
    #region Fields and properties

    [Header("General components")]
    [SerializeField] [Tooltip("Botón de girar la carta para ver reserso o parte delantera")]
    private Button flipCardButton;
    [SerializeField] [Tooltip("Espacio de arrastre (o scroll) de la carta en la parte trasera")]
    private ScrollRect cardScroll;
    [SerializeField] [Tooltip("Vista frontal de la carta")]
    private GameObject frontCardView;
    [SerializeField] [Tooltip("Vista trasera de la carta")]
    private GameObject backCardView;
    [SerializeField] [Tooltip("Clase que controla la apertura de nuevos paneles a mostrar")]
    private PanelOpener panelOpener;
    [SerializeField] [Tooltip("Prefab del panel donde se ven los videos que contiene la carta")]
    private GameObject videoPanelPrefab;
    [SerializeField]
    [Tooltip("borde de las cartas de la liga actual")]
    private Sprite actualborderCard;
    [SerializeField]
    [Tooltip("borde de las cartas de la liga clásica")]
    private Sprite clasicborderCard;
    [SerializeField]
    [Tooltip("botón de la liga actual")]
    private List<Image> cardBorders;

    [Space(5)]
    [Header("Front Card components")]
    [SerializeField] [Tooltip("Imagen del jugador en la vista frontal de la carta")]
    private Image frontCardImage;

    [Space(5)]
    [Header("Back Card components")]
    [SerializeField] [Tooltip("Imagen de la rareza de la carta")]
    private Image rarityImage;
    [SerializeField] [Tooltip("Texto de la rareza de la carta")]
    private Text rarityText;
    [SerializeField] [Tooltip("Texto de victorias de la carta")]
    private Text victoriesText;
    [SerializeField] [Tooltip("Lista de textos que tienen el nombre de la carta")]
    private List<Text> cardNameTexts;
    [SerializeField] [Tooltip("Imagen de miniatura del reverso de la carta")]
    private Image picImage;
    [SerializeField] [Tooltip("Texto con el valor de efectividad de los tiros de 2 (anteriormente tiros de 3) a nivel base")]
    private Text threePointerBase;
    [SerializeField] [Tooltip("Texto con el valor de efectividad de los tiros de 2 (anteriormente tiros de 3) con potenciador")]
    private Text threePointerWithBoost;
    [SerializeField] [Tooltip("Texto con el valor de efectividad de los rebotes a nivel base")]
    private Text reboundBase;
    [SerializeField] [Tooltip("Texto con el valor de efectividad de los rebotes con potenciador")]
    private Text reboundWithBoost;
    [SerializeField] [Tooltip("Texto con el valor de efectividad de los tiros libres a nivel base")]
    private Text freeThrowBase;
    [SerializeField] [Tooltip("Texto con el valor de efectividad de los tiros libres con potenciador")]
    private Text freeThrowWithBoost;
    [SerializeField] [Tooltip("Texto con el valor de efectividad de las asistencias a nivel base")]
    private Text assistsBase;
    [SerializeField] [Tooltip("Texto con el valor de efectividad de las asistencias con potenciador")]
    private Text assistsWithBoost;
    [SerializeField] [Tooltip("Texto con el valor de efectividad del puntaje base")]
    private Text scoreBase;
    [SerializeField] [Tooltip("Texto con el valor de efectividad del puntaje con potenciador")]
    private Text scoreWithBoost;
    [SerializeField] [Tooltip("Imagen del fondo donde se determina si el jugador ha sufrido una lesión")]
    private Image injuryBackground;
    [SerializeField] [Tooltip("Texto donde se muestra si el jugador ha sufrido una lesión")]
    private Text injuryText;
    [SerializeField] [Tooltip("Texto con el nombre del jugador")]
    private Text playerName;
    [SerializeField] [Tooltip("Imagen de la liga a la que pertence el jugador")]
    private Image imageLeague;
    [SerializeField] [Tooltip("Botón de cerrar panel")]
    private Text backButtonText;
    [SerializeField] [Tooltip("Botón que se encuentra en el reverso de la carta y elimina o añade un jugador al equipo competitivo")]
    private Button backCardPartButton;
    [SerializeField] [Tooltip("Objeto donde se muestra si un jugador está herido")]
    private GameObject isInjure;
    [SerializeField] [Tooltip("Objeto donde se muestra que un jugador se encuentra en el equipo competitivo")]
    private GameObject isTeam;
    [SerializeField] [Tooltip("Objeto donde se muestra si la carta tiene un potenciador puesto")]
    private GameObject isBooster;
    [SerializeField] [Tooltip("Potenciador de tiros dobles (antes triples)")]
    private CardPowerUp triplesCard;
    [SerializeField] [Tooltip("Potenciador de rebotes")]
    private CardPowerUp reboundsCard;
    [SerializeField] [Tooltip("Potenciador de tiros libres")]
    private CardPowerUp freeShotsCard;
    [SerializeField] [Tooltip("Potenciador de asistencias")]
    private CardPowerUp assistsCard;
    [SerializeField] [Tooltip("Potenciador del puntaje")]
    private CardPowerUp pointsCard;
    [SerializeField] [Tooltip("Botón donde se selecciona si ver un video del jugador")]
    private Button viewVideoButton;
    [SerializeField] [Tooltip("Determina si la parte reversa de la carta puede ser arrastrada (o drageada)")]
    private GameObject SwipeText;

    [Space(5)]
    [Header("Alert texts")]
    [SerializeField] [Tooltip("Texto de eliminar carta del equipo competitivo")]
    private string deleteCardText;
    [SerializeField] [Tooltip("Texto de agregar carta al equipo competitivo")]
    private string addCardText;
    [SerializeField] [Tooltip("Spinner de carga")]
    private GameObject Spinner;

    private bool inited; //Determina si la carta ya se encuentra configurada por primera vez
    private TokenItemData tokenData; //Datos de la carta traidos desde backend cuando se miran desde el despacho

    private ChallengeAcceptedData.CardChallengeData.CardItems cardData; //Datos de la carta traidos desde backend cuando se juega un desafio

    private bool doFillAnimation; //Determina si una carta debe hacer efecto de barrido (esto pasa únicamente cuando es arrastrado un token)
    private Action onBoosterAdded; //Acción ejecutada cuando un potenciador ha sido añadido a la carta

    public TokenItemData TokenData => tokenData;
    public Action OnBoosterAdded => onBoosterAdded;


    #endregion

    /// <summary>
    /// Se ejecuta cuando la carta ha sido iniciada por primera vez en escena, configurando los botones de video y de giro de la carta
    /// </summary>
    private void Start()
    {
        flipCardButton.onClick.AddListener(FlipCardView);
        viewVideoButton.onClick.AddListener(OpenVideoPanel);
        flipCardButton.interactable = false;
    }

    #region Public Methods

    /// <summary>
    /// Configura la carta desde el equipo competitivo
    /// </summary>
    /// <param name="tokenData">Datos de la carta</param>
    /// <param name="onClickBackPartCardButton">Acción que se ejecuta al dar click en el botón del reverso de la carta</param>
    /// <param name="canScroll">Determina si la carta se puede arrastrar en la parte reversa</param>
    /// <param name="isDeletingAction">Determina si la carta tiene la opción de eliminarse del equipo competitivo</param>
    /// <param name="doFillAnimation">Determina si se hace efecto de barrido de la carta</param>
    /// <param name="change">Determina si la carta se debe mostrar desde la parte frontal en caso de que su último estado fuera el reverso</param>
    /// <param name="onBoosterAdded">Acción que se ejecuta cuando un potenciador se ha añadido a la carta</param>
    public void SetupCardData(TokenItemData tokenData, Action onClickBackPartCardButton, bool canScroll = true, bool isDeletingAction = true, bool doFillAnimation = false, bool change = true, Action onBoosterAdded = null)
    {
        bool isActualLeague = false;
        PanelTeamCompetitivo addTeamPanel = FindObjectOfType<PanelTeamCompetitivo>(true);
        if(addTeamPanel)isActualLeague = addTeamPanel.isActualLeague;
        cardBorders.ForEach(t => t.sprite = isActualLeague ? actualborderCard:clasicborderCard );
        this.tokenData = tokenData;
        this.doFillAnimation = doFillAnimation;
        this.onBoosterAdded = onBoosterAdded;
        LoadFrontCardPart(change);
        cardScroll.enabled = canScroll;

        LoadBackCardPart();
        backButtonText.text = isDeletingAction ? deleteCardText : addCardText;
        if (!inited)
        {
            backCardPartButton.onClick.AddListener(() => { onClickBackPartCardButton?.Invoke(); });
            inited = true;
        }

        LoadBoosters();
        viewVideoButton.interactable = false;

        if (tokenData.videos != null && tokenData.videos.Count > 0)
        {
            viewVideoButton.interactable = true;
        }
    }

    /// <summary>
    /// Configura la carta desde un desafio que se está desarrollando
    /// </summary>
    /// <param name="cardData">Datos de la carta traidos desde backend</param>
    /// <param name="onBoosterAdded">Acción que se ejecuta cuando un potenciador se ha añadido a la carta</param>
    public void SetupCardData(ChallengeAcceptedData.CardChallengeData.CardItems cardData, Action onBoosterAdded = null)
    {
        cardBorders.ForEach(t => t.sprite = cardData.is_clasic ? clasicborderCard: actualborderCard );
        this.cardData = cardData;
        this.onBoosterAdded = onBoosterAdded;
        LoadFrontCardPart();
        cardScroll.enabled = false;
        viewVideoButton.interactable = false;
        LoadBackCardPart();
    }

    /// <summary>
    /// Determina si se muestra el botón del reverso de la carta para añadir o eliminar el equipo competitivo
    /// </summary>
    /// <param name="state">Determinar si se muestra o no el botón</param>
    public void EnableBackCardButton(bool state) => backCardPartButton.interactable = state;

    #endregion

    #region Inner Methods

    /// <summary>
    /// Configura la vista frontal de la carta
    /// </summary>
    /// <param name="change">Determina si la carta se debe forzar a mostrarse desde la vista frontal en caso de estar en reverso</param>
    private void LoadFrontCardPart(bool change = true)
    {
        if (change)
        {
            frontCardView.SetActive(true);
            backCardView.SetActive(false);
        }
        
        if (tokenData != null)
        {
            WebProcedure.Instance.GetSprite(tokenData?.pathImgFront, OnSuccess, (failed) => { Debug.Log("Failed loading front image"); });
            flipCardButton.interactable = !doFillAnimation;
            frontCardImage.fillAmount = doFillAnimation ? 0f : 1f;
        }
        else if(cardData != null)
        {
            WebProcedure.Instance.GetSprite(cardData?.pathImgFront, OnSuccess, (failed) => { Debug.Log("Failed loading front image"); });
        }
    }

    /// <summary>
    /// Configura la vista y los datos de la parte reversa de la carta
    /// </summary>
    private void LoadBackCardPart()
    {
        if(tokenData != null)
        {
            SetRarityImage(tokenData.rarity);
            victoriesText.text = tokenData.victories;
            cardNameTexts.ForEach(card => card.text = tokenData.name);
            WebProcedure.Instance.GetSprite(tokenData.pathThumbnail, (obj) => { picImage.sprite = obj; }, (failed) => { Debug.Log("Failed loading thumbnail image"); });
            threePointerBase.text = tokenData.st_triples;
            threePointerWithBoost.text = tokenData.triples;
            reboundBase.text = tokenData.st_rebounds;
            reboundWithBoost.text = tokenData.rebounds;
            freeThrowBase.text = tokenData.st_freeshots;
            freeThrowWithBoost.text = tokenData.freeshots;
            assistsBase.text = tokenData.st_assists;
            assistsWithBoost.text = tokenData.assits;
            scoreBase.text = tokenData.st_points;
            scoreWithBoost.text = tokenData.points;
            injuryBackground.color = tokenData.isInjured ? Color.red : Color.black;
            injuryText.text = tokenData.daysOrTextInjured;
            playerName.text = tokenData.name;

            isInjure.gameObject?.SetActive(tokenData.isInjured);
            isBooster.gameObject?.SetActive(tokenData.isBooster);
            isTeam.gameObject?.SetActive(tokenData.isTeam);
            WebProcedure.Instance.GetSprite(tokenData.pathImgCol, (obj) => { imageLeague.sprite = obj; }, (failed) => { Debug.Log("Failed loading col image"); });
            var backimg = backCardView.GetComponent<Image>();
            backimg.color= Color.white;
            WebProcedure.Instance.GetSprite(tokenData.pathImgBack, (obj) => { backimg.sprite = obj; }, (failed) => { Debug.Log("Failed loading back image"); });
        }
        else if(cardData != null)
        {
            SetRarityImage(cardData.rarity);
            victoriesText.text = cardData.victories;
            cardNameTexts.ForEach(card => card.text = cardData.name);
            WebProcedure.Instance.GetSprite(cardData.pathThumbnail, (obj) => { picImage.sprite = obj; }, (failed) => { Debug.Log("Failed loading thumbnail image"); });
            threePointerBase.text = cardData.st_triples;
            threePointerWithBoost.text = cardData.triples;
            reboundBase.text = cardData.st_rebounds;
            reboundWithBoost.text = cardData.rebounds;
            freeThrowBase.text = cardData.st_freeshots;
            freeThrowWithBoost.text = cardData.freeshots;
            assistsBase.text = cardData.st_assists;
            assistsWithBoost.text = cardData.assits;
            scoreBase.text = cardData.st_points;
            scoreWithBoost.text = cardData.points;
            injuryBackground.color = cardData.isInjured ? Color.red : Color.black;
            injuryText.text = cardData.daysOrTextInjured;
            playerName.text = cardData.name;

            isInjure.gameObject?.SetActive(cardData.isInjured);
            isBooster.gameObject?.SetActive(cardData.isBooster);
            isTeam.gameObject?.SetActive(cardData.isTeam);
            WebProcedure.Instance.GetSprite(cardData.pathImgCol, (obj) => { imageLeague.sprite = obj; }, (failed) => { Debug.Log("Failed loading col image"); });
            var backimg = backCardView.GetComponent<Image>();
            backimg.color= Color.white;
            WebProcedure.Instance.GetSprite(cardData.pathImgBack, (obj) => {    backimg.sprite = obj; }, (failed) => { Debug.Log("Failed loading back image"); });
        }

        SwipeText.SetActive(tokenData != null);
    }

    /// <summary>
    /// Configura los botones de los potenciadores en el reverso de la carta
    /// </summary>
    private void LoadBoosters()
    {
        triplesCard.SetupPowerUpView(tokenData.isBoostTriples, this, tokenData.isTeam, tokenData.pt_triples);
        reboundsCard.SetupPowerUpView(tokenData.isBoostRebounds, this, tokenData.isTeam, tokenData.pt_rebounds);
        freeShotsCard.SetupPowerUpView(tokenData.isBoostFreeshots, this, tokenData.isTeam, tokenData.pt_freeshots);
        assistsCard.SetupPowerUpView(tokenData.isBoostAssists, this, tokenData.isTeam, tokenData.pt_assists);
        pointsCard.SetupPowerUpView(tokenData.isBoostPoints, this, tokenData.isTeam, tokenData.pt_points);
    }

    /// <summary>
    /// Gira la carta de una vista a otra
    /// </summary>
    private void FlipCardView()
    {
        bool cachedState = frontCardView.activeInHierarchy;
        frontCardView.SetActive(backCardView.activeInHierarchy);
        backCardView.SetActive(cachedState);
    }

    /// <summary>
    /// Muestra el panel de video
    /// </summary>
    private void OpenVideoPanel()
    {
        panelOpener.popupPrefab = videoPanelPrefab;
        panelOpener.OpenPopup();
        panelOpener.popup.GetComponent<PanelVideoList>().SetupVideos(tokenData.videos);
    }

    /// <summary>
    /// Determina el valor de rareza de la carta
    /// </summary>
    /// <param name="imageRarity">Valor de rareza de la carta a asignar</param>
    private void SetRarityImage(TokenRarety imageRarity)
    {
        string path = "CardRarity/";
        switch(imageRarity)
        {
            case TokenRarety.COMÚN:
                path += "Common";
                break;

            case TokenRarety.ÉPICA:
                path += "Epic";
                break;

            case TokenRarety.LEGENDARIA:
                path += "Legend";
                break;
        }

        rarityText.text = imageRarity.ToString().ToUpper();
        rarityImage.sprite = Resources.Load<Sprite>(path);
    }

    /// <summary>
    /// Método que se ejecuta cuando se pudo mostrar satisfactoriamente la imagen de la carta desde backend
    /// Cerrando el spinner de carga
    /// </summary>
    /// <param name="obj"></param>
    private void OnSuccess(Sprite obj)
    {
        frontCardImage.sprite = obj;
        if (doFillAnimation)
        {
            StopAllCoroutines();
            StartCoroutine(FillCard());
        }
        else
            flipCardButton.interactable = true;
        CloseSpinner();
    }

    /// <summary>
    /// Corrutina de efecto de barrido de la carta en la parte frontal
    /// </summary>
    /// <returns></returns>
    private IEnumerator FillCard()
    {
        for (float fill = 0; fill <= 1; fill += 0.075f)
        {
            frontCardImage.fillAmount = fill;
            yield return new WaitForSeconds(0.01f);
        }

        frontCardImage.fillAmount = 1f;
        flipCardButton.interactable = true;
    }

    /// <summary>
    /// Cierra el spinner de carga
    /// </summary>
    private void CloseSpinner()
    {
        Spinner.gameObject.SetActive(false);
    }

    #endregion

}
