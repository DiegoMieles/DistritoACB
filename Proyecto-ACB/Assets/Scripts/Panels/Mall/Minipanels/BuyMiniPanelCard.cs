using System.Collections.Generic;
using Data;
using WebAPI;
using System;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;

/// <summary>
/// Controla el minipanel específico de compra de cartas y highlights
/// </summary>
public class BuyMiniPanelCard : BuyMiniPanel
{
    #region Fields and properties

    [Header("Card UI components")]
    [SerializeField] [Tooltip("Botón de girar la carta para ver reserso o parte delantera")]
    private Button flipCardButton;
    [SerializeField] [Tooltip("Vista frontal de la carta")]
    private GameObject frontCardView;
    [SerializeField] [Tooltip("Vista trasera de la carta")]
    private GameObject backCardView;
    [SerializeField] [Tooltip("Vista trasera del highlight")]
    private GameObject backHighlightView;
    [SerializeField] [Tooltip("Lista de marcos de la carta")]
    private List<Image> frameImages;

    [Space(5)]
    [Header("Front Card components")]
    [SerializeField] [Tooltip("Imagen frontal de la carta")]
    private Image frontCardImage;

    [Space(5)]
    [Header("Back Player Card components")]
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

    [Space(5)]
    [Header("Back Highlight Card components")]
    [SerializeField] [Tooltip("Texto con el nombre del highlight")]
    private Text highlightName;
    [SerializeField] [Tooltip("Fecha del highlight")]
    private Text highlightDate;
    [SerializeField] [Tooltip("Texto de descripción del highlight")]
    private Text highlightDescription;
    [SerializeField] [Tooltip("Imagen de la liga del highlight")]
    private Image leagueImage;
    [SerializeField]
    [Tooltip("Imagen de fondo de la parte de atrás del highlight")]
    private Image highlightBackBackground;

    [Header("Other components")]
    [SerializeField] [Tooltip("Spinner de carga")]
    private GameObject spinner;
    [SerializeField] [Tooltip("Marco del highliight")]
    private Sprite highlightFrame;
    [SerializeField] [Tooltip("Marco de la carta del jugador")]
    private Sprite tokenFrame;
    [SerializeField]
    [Tooltip("objeto que muestra que tiene un potenciador aplicado")]
    private GameObject boosterFlag;
    [SerializeField]
    [Tooltip("objeto que muestra que tiene heridas ")]
    private GameObject injuredFlag;
    [SerializeField]
    [Tooltip("panel padre que contiene este minipanel")]
    private PanelJumbleSaleBuyConfirmation panelBuyConfirmation;

    private GameObject actualBackPart; //Parte reversa de la carta actual
    private bool publishedByPlayer = true;
    private bool isHighlight ;
    private bool isJumbleSale ;
    #endregion

    #region Public Methods

    /// <summary>
    /// Activa el minipanel a nivel general
    /// </summary>
    /// <param name="productSprite">Imagen del producto a comprar</param>
    /// <param name="itemData">Datos del producto</param>
    /// <param name="description">Descripción del producto</param>
    /// <param name="onFailedLoading">Acción que se ejecuta cuando la carga del producto falla</param>
    public override void ShowMiniPanel(Sprite productSprite, MallContainerData.MallData.MallItems itemData, string description, Action onFailedLoading)
    {
        base.ShowMiniPanel(productSprite, itemData, description, onFailedLoading);

        frontCardView.SetActive(true);
        backCardView.SetActive(false);
        backHighlightView.SetActive(false);
        flipCardButton.interactable = false;

        if (frameImages != null && frameImages.Count > 0)
            frameImages.ForEach(frame => frame.sprite = itemData.elementType == ItemType.TOKEN ? tokenFrame : highlightFrame);

        actualBackPart = itemData.elementType == ItemType.TOKEN ? backCardView : backHighlightView;
        flipCardButton.onClick.AddListener(FlipCardView);

        if(itemData.random)
        {
            WebProcedure.Instance.GetSprite(itemData.path_img, (obj) => { frontCardImage.sprite = obj; flipCardButton.interactable = true; spinner.gameObject.SetActive(false); }, (error) => { });
            return;
        }

        if (itemData.elementType == ItemType.TOKEN)
        {
            MallContainerData.MallData.MallItemsToken cardData = new MallContainerData.MallData.MallItemsToken();
            WebProcedure.Instance.GetPostMallElement(itemData.id.ToString(), (obj) =>
            {
                Debug.Log($"Card data = { obj.RawJson}");
                JsonConvert.PopulateObject(obj.RawJson, cardData);
                SetCardData(cardData); 
            },
            (error) => { ACBSingleton.Instance.AlertPanel.SetupPanel(error.Message, "", false, onFailedLoading); });
        }
        else if(itemData.elementType == ItemType.HIGTHLIGHT)
        {
            MallContainerData.MallData.MallItemsHight highlightData = new MallContainerData.MallData.MallItemsHight();
            WebProcedure.Instance.GetPostMallElement(itemData.id.ToString(), (obj) =>
            {
                Debug.Log($"Highlight data = { obj.RawJson}");
                JsonConvert.PopulateObject(obj.RawJson, highlightData);
                SetHighlightData(highlightData);
            },
            (error) => { ACBSingleton.Instance.AlertPanel.SetupPanel(error.Message, "", false, onFailedLoading); });
        }
        else
            ACBSingleton.Instance.AlertPanel.SetupPanel("Esta carta no tiene datos, por favor selecciona otra carta", string.Empty, false, null);
    }
    /// <summary>
    /// Activa el minipanel a nivel general
    /// </summary>
    /// <param name="productSprite">Imagen del producto a comprar</param>
    /// <param name="itemData">Datos del producto</param>
    /// <param name="description">Descripción del producto</param>
    /// <param name="onFailedLoading">Acción que se ejecuta cuando la carga del producto falla</param>
    public override void ShowMiniPanel(Sprite productSprite, JumbleSaleResult.JumbleItems itemData, string description, Action onFailedLoading)
    {
        base.ShowMiniPanel(productSprite, itemData, description, onFailedLoading);
        isJumbleSale = true;
           publishedByPlayer = itemData.seller_user_id == WebProcedure.Instance.accessData.user;
        frontCardView.SetActive(true);
        backCardView.SetActive(false);
        backHighlightView.SetActive(false);
        flipCardButton.interactable = false;
      
        if (frameImages != null && frameImages.Count > 0)
            frameImages.ForEach(frame => frame.sprite = itemData.item_type == "TOKENCARD" ? tokenFrame : highlightFrame);

        actualBackPart = itemData.item_type == "TOKENCARD" ? backCardView : backHighlightView;
        flipCardButton.onClick.AddListener(FlipCardView);

  

        if (itemData.item_type == "TOKENCARD")
        {
            // SetCardData(itemData);
            injuredFlag.SetActive(itemData.is_injured );
            boosterFlag.SetActive(itemData.is_booster );
            JumbleSaleResult.JumbleItemData cardData = new JumbleSaleResult.JumbleItemData();
            WebProcedure.Instance.GetJumbleSaleInfoItem(itemData.id.ToString(), (obj) =>
            {
                Debug.Log($"Card data = { obj.RawJson}");
                JsonConvert.PopulateObject(obj.RawJson, cardData);
                cardData.data.name = itemData.name;
                cardData.data.description = itemData.description;
                SetCardData(cardData);
            },
            (error) => { ACBSingleton.Instance.AlertPanel.SetupPanel(error.Message, "", false, onFailedLoading); });
        }
        else if(itemData.item_type == "TOKENHIGTHLIGHT")
        {
            isHighlight = true;
            titleText.transform.parent.gameObject.SetActive(!publishedByPlayer);
            JumbleSaleResult.JumbleItemData cardData = new JumbleSaleResult.JumbleItemData();
            WebProcedure.Instance.GetJumbleSaleInfoItem(itemData.id.ToString(), (obj) =>
            {
                Debug.Log($"Card data = { obj.RawJson}");
                JsonConvert.PopulateObject(obj.RawJson, cardData);
                cardData.data.description = itemData.description;
                SetHighlightData(cardData);
            },
            (error) => { ACBSingleton.Instance.AlertPanel.SetupPanel(error.Message, "", false, onFailedLoading); });
        }
        else
            ACBSingleton.Instance.AlertPanel.SetupPanel("Esta carta no tiene datos, por favor selecciona otra carta", string.Empty, false, null);
    }

    #endregion

    #region Inner Methods

    /// <summary>
    /// Asigna datos de carta del jugador
    /// </summary>
    /// <param name="cardData">Datos de la carta del jugador</param>
    private void SetCardData(MallContainerData.MallData.MallItemsToken cardData)
    {
        WebProcedure.Instance.GetSprite(cardData.element.pathImgFront, (obj) => { frontCardImage.sprite = obj; spinner.gameObject.SetActive(false); flipCardButton.interactable = true; }, (error) => { });
        SetRarityImage(cardData.element.rarity);
        victoriesText.text = cardData.element.victories;
        cardNameTexts.ForEach(card => card.text = cardData.element.name);
        WebProcedure.Instance.GetSprite(cardData.element.pathThumbnail, (obj) => { picImage.sprite = obj; }, (failed) => { Debug.Log("Failed loading thumbnail image"); });
        threePointerBase.text = cardData.element.st_triples;
        threePointerWithBoost.text = cardData.element.st_triples;
        reboundBase.text = cardData.element.st_rebounds;
        reboundWithBoost.text = cardData.element.st_rebounds;
        freeThrowBase.text = cardData.element.st_freeshots;
        freeThrowWithBoost.text = cardData.element.st_freeshots;
        assistsBase.text = cardData.element.st_assists;
        assistsWithBoost.text = cardData.element.st_assists;
        scoreBase.text = cardData.element.st_points;
        scoreWithBoost.text = cardData.element.st_points;
        injuryBackground.color = cardData.element.isInjured ? Color.red : Color.black;
        injuryBackground.color = Color.white;
        injuryText.text = cardData.element.textInjured;
        
    }
    /// <summary>
    /// Asigna datos de carta del jugador en el mercadillo
    /// </summary>
    /// <param name="cardData">Datos de la carta del jugador</param>
    private void SetCardData(JumbleSaleResult.JumbleItemData cardData)
    {
        WebProcedure.Instance.GetSprite(cardData.data.path_img_front, (obj) => { frontCardImage.sprite = obj; spinner.gameObject.SetActive(false); flipCardButton.interactable = true; }, (error) => { });
        SetRarityImage(cardData.data.rarity);
        victoriesText.text = cardData.data.victories;
        cardNameTexts.ForEach(card => card.text = cardData.data.name);
        WebProcedure.Instance.GetSprite(cardData.data.path_img_back, (obj) => { picImage.sprite = obj; }, (failed) => { Debug.Log("Failed loading thumbnail image"); });
        threePointerBase.text = cardData.data.st_triples;
        threePointerWithBoost.text = cardData.data.st_triples;
        reboundBase.text = cardData.data.st_rebounds;
        reboundWithBoost.text = cardData.data.st_rebounds;
        freeThrowBase.text = cardData.data.st_freeshots;
        freeThrowWithBoost.text = cardData.data.st_freeshots;
        assistsBase.text = cardData.data.st_assists;
        assistsWithBoost.text = cardData.data.st_assists;
        scoreBase.text = cardData.data.st_points;
        scoreWithBoost.text = cardData.data.st_points;
        injuryBackground.color = cardData.data.isInjured ? Color.red : Color.black;
        injuryBackground.color = Color.white;
        injuryText.text = cardData.data.textInjured;
        
    }

    /// <summary>
    /// Asigna datos de highlight a la carta
    /// </summary>
    /// <param name="highlightData">Datos de la carta tipo highlight</param>
    private void SetHighlightData(MallContainerData.MallData.MallItemsHight highlightData)
    {
        WebProcedure.Instance.GetSprite(highlightData.element.pathImg, (obj) => { frontCardImage.sprite = obj; spinner.gameObject.SetActive(false); flipCardButton.interactable = true; }, (error) => { });
        highlightName.text = highlightData.element.title;
        highlightDate.text = !string.IsNullOrEmpty(highlightData.postdate) ? "Creación " + highlightData.postdate.ToString() : "";
        highlightDescription.text = highlightData.element.description;
        WebProcedure.Instance.GetSprite(highlightData.element.pathImgCol, (obj) => { leagueImage.sprite = obj; }, (error) => { });
    }
        /// <summary>
    /// Asigna datos de highlight a la carta en el mercadillo
    /// </summary>
    /// <param name="highlightData">Datos de la carta tipo highlight</param>
    private void SetHighlightData(JumbleSaleResult.JumbleItemData highlightData)
    {
      
        WebProcedure.Instance.GetSprite(highlightData.data.path_img, (obj) => { frontCardImage.sprite = obj; spinner.gameObject.SetActive(false); flipCardButton.interactable = true; }, (error) => { });
        highlightName.text = highlightData.data.title;
        highlightDate.text = !string.IsNullOrEmpty(highlightData.data.created) ? "Creación " + highlightData.data.created.ToString() : "";
        //highlightDate.text = !string.IsNullOrEmpty(highlightData.data.) ? "Creación " + highlightData.publication_date : "";
        highlightDescription.text = highlightData.data.description;
        WebProcedure.Instance.GetSprite(highlightData.data.path_img_leauge, (obj) => { leagueImage.sprite = obj; }, (error) => { });
        if(highlightBackBackground) WebProcedure.Instance.GetSprite(highlightData.data.path_img_backcard, (obj) => { highlightBackBackground.sprite = obj; }, (error) => { });
    }

    /// <summary>
    /// Gira la vista de la carta
    /// </summary>
    private void FlipCardView()
    {
        bool cachedState = frontCardView.activeInHierarchy;
        frontCardView.SetActive(actualBackPart.activeInHierarchy);
        actualBackPart.SetActive(cachedState);
        if(isJumbleSale)
        {
            panelBuyConfirmation.costInfoLayout.gameObject.SetActive(!publishedByPlayer && !cachedState);
            panelBuyConfirmation.deleteButton.transform.parent.gameObject.SetActive(publishedByPlayer && !cachedState);
            titleText.transform.parent.gameObject.SetActive((!publishedByPlayer||!isHighlight) && !cachedState);
            descriptionText.transform.parent.gameObject.SetActive(!cachedState);
        }
       
    }

    /// <summary>
    /// Determina la rareza de la carta
    /// </summary>
    /// <param name="imageRarity">Rareza de la carta</param>
    private void SetRarityImage(TokenRarety imageRarity)
    {
        string path = "CardRarity/";
        switch (imageRarity)
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


    #endregion
}
