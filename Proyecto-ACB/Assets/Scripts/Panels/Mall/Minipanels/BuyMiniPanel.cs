using Data;
using WebAPI;
using System;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using TMPro;

/// <summary>
/// Controlador de minipanel general de la tienda que controla los productos más básicos
/// </summary>
public class BuyMiniPanel : MonoBehaviour
{
    #region Fields and properties

    [Header("Mini panel UI components")]
    [SerializeField] [Tooltip("Imagen del producto")]
    protected Image productImage;
    [SerializeField] [Tooltip("Texto del título a mostrar en el minipanel")]
    protected Text titleText;
    [SerializeField] [Tooltip("Texto de descripción del minipanel")]
    protected Text descriptionText;
    [SerializeField]
    [Tooltip("Texto de la fecha de publicación")]
    protected Text publicationDate;
    [SerializeField]
    [Tooltip("Texto del costo de la publicación")]
    protected TextMeshProUGUI ownedItemPrice;

    [Space(5)]
    [Header("Title text according item type")]
    [SerializeField] [Tooltip("Texto que se asigna al titulo si el producto es una ACBall")]
    private string acballTitleText;
    [SerializeField] [Tooltip("Texto que se asigna al titulo si el producto es una carta")]
    private string cardTitleText;
    [SerializeField] [Tooltip("Texto que se asigna al titulo si el producto es un objeto de avatar")]
    private string skinTitleText;
    [SerializeField] [Tooltip("Texto que se asigna al titulo si el producto es un potenciador")]
    private string boosterTitleText;
    [SerializeField] [Tooltip("Texto que se asigna al titulo si el producto es una carta highlight")]
    private string highlightTitleText;
    [Header("Random text")]
    [SerializeField] [Tooltip("Texto que se asigna al titulo si el producto es una ACBall aleatoria")]
    private string acballRandomTitleText;
    [SerializeField] [Tooltip("Texto que se asigna al titulo si el producto es una carta aleatoria")]
    private string cardRandomTitleText;
    [SerializeField] [Tooltip("Texto que se asigna al titulo si el producto es un objeto de avatar aleatorio")]
    private string skinRandomTitleText;
    [SerializeField] [Tooltip("Texto que se asigna al titulo si el producto es un potenciador aleatorio")]
    private string boosterRandomTitleText;
    [SerializeField] [Tooltip("Texto que se asigna al titulo si el producto es una carta highlight aleatoria")]
    private string highlightRandomTitleText;
    [SerializeField]
    [Tooltip("Texto que se asigna al titulo si el producto del mercadillo es del jugador ")]
    private string jumbleSaleOwnedPublicationTitleText;

    #endregion

    #region Public Methods

    /// <summary>
    /// Activa el minipanel a nivel general
    /// </summary>
    /// <param name="productSprite">Imagen del producto a comprar</param>
    /// <param name="itemData">Datos del producto</param>
    /// <param name="description">Descripción del producto</param>
    /// <param name="onFailedLoading">Acción que se ejecuta cuando la carga del producto falla</param>
    public virtual void ShowMiniPanel(Sprite productSprite, MallContainerData.MallData.MallItems itemData, string description, Action onFailedLoading)
    {
        gameObject.SetActive(true);

        if (productImage != null)
        {
            if(!itemData.random)
            {
                switch(itemData.elementType)
                {
                    case ItemType.BOOSTER:
                        MallContainerData.MallData.MallItemsBooster boosterData = new MallContainerData.MallData.MallItemsBooster();

                        WebProcedure.Instance.GetPostMallElement(itemData.id.ToString(), (obj) =>
                        {
                            Debug.Log($"Booster data = { obj.RawJson}");
                            JsonConvert.PopulateObject(obj.RawJson, boosterData);
                            if(boosterData.element.path_img == null)
                            {
                                productImage.sprite = productSprite;
                            }
                            else
                            {
                                WebProcedure.Instance.GetSprite(boosterData.element.path_img, OnSuccessLoading, (error) => { });
                            }
                        },
                        (error) => { ACBSingleton.Instance.AlertPanel.SetupPanel(error.Message, "", false, onFailedLoading); });

                        break;

                    case ItemType.SKIN:

                        MallContainerData.MallData.MallItemsAvatar skinData = new MallContainerData.MallData.MallItemsAvatar();

                        WebProcedure.Instance.GetPostMallElement(itemData.id.ToString(), (obj) =>
                        {
                            Debug.Log($"Skin data = { obj.RawJson}");
                            JsonConvert.PopulateObject(obj.RawJson, skinData);
                            WebProcedure.Instance.GetSprite(skinData.element.img_show, OnSuccessLoading, (error) => { });
                        },
                        (error) => { ACBSingleton.Instance.AlertPanel.SetupPanel(error.Message, "", false, onFailedLoading); });

                        break;

                    default:
                        productImage.sprite = productSprite;
                        break;
                }
            }
            else
                productImage.sprite = productSprite;
        }

        titleText.text = TitleTextFromItemType(itemData.elementType, itemData.random);

        if (!string.IsNullOrEmpty(description) && descriptionText != null && itemData.elementType != ItemType.ACBALL)
            descriptionText.text = description;
    }

    /// <summary>
    /// Activa el minipanel a nivel general
    /// </summary>
    /// <param name="productSprite">Imagen del producto a comprar del mercadillo</param>
    /// <param name="itemData">Datos del producto</param>
    /// <param name="description">Descripción del producto</param>
    /// <param name="onFailedLoading">Acción que se ejecuta cuando la carga del producto falla</param>
    public virtual void ShowMiniPanel(Sprite productSprite, JumbleSaleResult.JumbleItems itemData, string description, Action onFailedLoading)
    {
        gameObject.SetActive(true);
        if (!string.IsNullOrEmpty(description) && descriptionText != null)
        {
            descriptionText.text = itemData.item_type == "TOKENHIGTHLIGHT" ? itemData.name : description;
        }

        if (itemData.seller_user_id == WebProcedure.Instance.accessData.user)
        {
            if(descriptionText != null)descriptionText.transform.parent.gameObject.SetActive(true);
             if(descriptionText != null)descriptionText.resizeTextForBestFit = itemData.item_type != "TOKENHIGTHLIGHT";
            if (publicationDate)
            {
                publicationDate.gameObject.SetActive(true);
                publicationDate.text = itemData.publication_date;
            }
            if(ownedItemPrice)
            {
                ownedItemPrice.transform.parent.gameObject.SetActive(true);
                ownedItemPrice.text = itemData.item_type == "TOKENHIGTHLIGHT" || itemData.item_type == "TOKENCARD" ? "<sprite=3>"  + itemData.price : itemData.price + "<sprite=4>";
            }
        }
        else
        {
            if (descriptionText != null) descriptionText.transform.parent.gameObject.SetActive(itemData.item_type != "TOKENHIGTHLIGHT" && itemData.item_type != "TOKENCARD" );
        }
        if (productImage != null)
        {

            switch (itemData.item_type)
            {
                case "BOOSTER":
                    if (productImage != null)
                        productImage.sprite = productSprite;
                    titleText.transform.parent.gameObject.SetActive(itemData.seller_user_id != WebProcedure.Instance.accessData.user);

                    break;

                case "SKIN":
                    if (productImage != null)
                        productImage.sprite = productSprite;
                    titleText.transform.parent.gameObject.SetActive(itemData.seller_user_id != WebProcedure.Instance.accessData.user);
                    break;

                default:
                    productImage.sprite = productSprite;
                    break;
            }

        }
        titleText.text = TitleTextFromItemType(itemData.item_type, itemData.seller_user_id == WebProcedure.Instance.accessData.user);
       
          
    }

    #endregion

    #region Inner Methods

    /// <summary>
    /// Asigna el texto a mostrar dependiendo el tipo de producto a comprar
    /// </summary>
    /// <param name="title">Tipo de item</param>
    /// <param name="isRandom">Determina si el objeto es aleatorio</param>
    /// <returns></returns>
    private string TitleTextFromItemType(ItemType title, bool isRandom)
    {
        string returnString = string.Empty;
        switch(title)
        {
            case ItemType.ACBALL:
                if (!isRandom)
                    returnString = acballTitleText;
                else
                    returnString = acballRandomTitleText;
                break;

            case ItemType.TOKEN:
                if (!isRandom)
                    returnString = cardTitleText;
                else
                    returnString = cardRandomTitleText;
                break;

            case ItemType.SKIN:
                if (!isRandom)
                    returnString = skinTitleText;
                else
                    returnString = skinRandomTitleText;
                break;

            case ItemType.BOOSTER:
                if (!isRandom)
                    returnString = boosterTitleText;
                else
                    returnString = boosterRandomTitleText;
                break;

            case ItemType.HIGTHLIGHT:
                if (!isRandom)
                    returnString = highlightTitleText;
                else
                    returnString = highlightRandomTitleText;
                break;
        }

        return returnString;
    }
    /// <summary>
    /// Asigna el texto a mostrar dependiendo el tipo de producto a comprar EN EL MERCADILLO
    /// </summary>
    /// <param name="title">Tipo de item</param>
    /// <param name="isOwnedByPlayer">Determina si el objeto fué publicado por el jugador</param>
    /// <returns></returns>
    private string TitleTextFromItemType(string title , bool isOwnedByPlayer)
        {
            string returnString = string.Empty;
        if (isOwnedByPlayer)
            return  jumbleSaleOwnedPublicationTitleText;

          
            switch (title)
            {
                case "ACBALL":
                        returnString = acballTitleText;
                break;
                case "TOKENCARD":
                        returnString = cardTitleText;
                    break;

                case "SKIN":
                        returnString = skinTitleText;
                    break;

                case  "BOOSTER":
                        returnString = boosterTitleText;
                    break;

                case "TOKENHIGTHLIGHT":
                        returnString = highlightTitleText;
                    break;
            }

            return returnString;
        }
        /// <summary>
        /// Este método se ejecuta cuando la imagen del producto se carga exitosamente
        /// </summary>
        /// <param name="obj">Imagen del producto</param>
        private void OnSuccessLoading(Sprite obj)
    {
        if (productImage != null)
            productImage.sprite = obj;
    }

    #endregion
}
