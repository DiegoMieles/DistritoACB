using System.Collections.Generic;
using Data;
using WebAPI;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controlador de la carta de recompensa de misión
/// </summary>
public class MissionCardReward : MonoBehaviour
{
    #region Fields and properties

    [Header("Card UI components")]
    [SerializeField] [Tooltip("Botón de girar la carta para ver reserso o parte delantera")]
    private Button flipCardButton;
    [SerializeField] [Tooltip("Vista frontal de la carta")]
    private GameObject frontCardView;
    [SerializeField] [Tooltip("Vista trasera de la cartade jugador")]
    private GameObject backCardView;
    [SerializeField] [Tooltip("Vista trasera de la cartade highlight")]
    private GameObject backHighlightView;
    [SerializeField] [Tooltip("Listado de imagenes de frames")]
    private List<Image> frameImages;

    [Space(5)]
    [Header("Front Card components")]
    [SerializeField] [Tooltip("Imagen del jugador en la vista frontal de la carta")]
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
    [SerializeField] [Tooltip("Texto con la descripción del highlight")]
    private Text highlightDescription;
    [SerializeField] [Tooltip("Imagen de la liga del highlight")]
    private Image leagueImage;

    [Header("Other components")]
    [SerializeField] [Tooltip("Spinner de carga del panel")]
    private GameObject spinner;
    [SerializeField] [Tooltip("Recuadro del highlight")]
    private Sprite highlightFrame;
    [SerializeField] [Tooltip("Recuadro de la carta")]
    private Sprite tokenFrame;

    private GameObject actualBackPart; //Parte trasera de la carta

    #endregion

    #region Public Methods

    /// <summary>
    /// Muestra la carta con sus datos traidos desde backend
    /// </summary>
    /// <param name="itemData">Datos de la carta</param>
    public void ShowCard(MissionRewardData.RewardData itemData)
    {
        Debug.Log("Loading card");
        frontCardView.SetActive(true);
        frontCardImage.sprite = null;
        backCardView.SetActive(false);
        backHighlightView.SetActive(false);
        flipCardButton.onClick.RemoveAllListeners();
        actualBackPart = itemData.rewardType == ItemType.TOKEN ? backCardView : backHighlightView;
        flipCardButton.onClick.AddListener(FlipCardView);

        if(frameImages != null && frameImages.Count > 0)
            frameImages.ForEach(frame => frame.sprite = itemData.rewardType == ItemType.TOKEN ? tokenFrame : highlightFrame);

        if (itemData.rewardType == ItemType.TOKEN)
        {
            MissionRewardData.RewardData.RewardItemsToken cardData = (MissionRewardData.RewardData.RewardItemsToken)itemData.ChangeType(itemData.rewardType);
            SetCardData(cardData);
        }
        else if (itemData.rewardType == ItemType.HIGTHLIGHT)
        {
            MissionRewardData.RewardData.RewardItemsHighlight highlightData = (MissionRewardData.RewardData.RewardItemsHighlight)itemData.ChangeType(itemData.rewardType);
            SetHighlightData(highlightData);
        }
        else
            ACBSingleton.Instance.AlertPanel.SetupPanel("Esta carta no tiene datos, por favor selecciona otra carta", string.Empty, false, null);
    }

    #endregion

    #region Inner Methods

    /// <summary>
    /// Asigna los datos de la carta
    /// </summary>
    /// <param name="cardData">Datos de la carta</param>
    private void SetCardData(MissionRewardData.RewardData.RewardItemsToken cardData)
    {
        WebProcedure.Instance.GetSprite(cardData.element.pathImgFront, (obj) => { 
            frontCardImage.gameObject.SetActive(true);
            frontCardImage.sprite = obj;
            spinner.gameObject.SetActive(false); }, 
            (error) => { });

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
        injuryText.text = cardData.element.daysOrTextInjured;

    }

    /// <summary>
    ///  Muestra el highlight con sus datos traidos desde backend
    /// </summary>
    /// <param name="highlightData">Datos de highlight</param>
    private void SetHighlightData(MissionRewardData.RewardData.RewardItemsHighlight highlightData)
    {
        WebProcedure.Instance.GetSprite(highlightData.element.pathImg, (obj) => 
        {
            frontCardImage.gameObject.SetActive(true);
            frontCardImage.sprite = obj;
            spinner.gameObject.SetActive(false);
        }, 
        (error) => { });

        highlightName.text = highlightData.element.name;
        highlightDescription.text = highlightData.element.description;
        WebProcedure.Instance.GetSprite(highlightData.element.pathImgThumbnail, (obj) => { leagueImage.sprite = obj; }, (error) => { });
    }

    /// <summary>
    /// Voltea la carta
    /// </summary>
    private void FlipCardView()
    {
        bool cachedState = frontCardView.activeInHierarchy;
        frontCardView.SetActive(actualBackPart.activeInHierarchy);
        actualBackPart.SetActive(cachedState);
    }

    /// <summary>
    /// Asigna la rareza a la carta
    /// </summary>
    /// <param name="imageRarity">Tipo rareza de la carta/highlight</param>
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
