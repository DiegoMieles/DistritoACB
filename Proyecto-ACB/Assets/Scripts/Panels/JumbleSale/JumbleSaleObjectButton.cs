using System;
using Data;
using WebAPI;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controla y asigna los datos de un objeto que se puede comprar en la tienda
/// </summary>
public class JumbleSaleObjectButton : MallObjectButton
{
    [SerializeField]
    [Tooltip("texto de la fecha de publicación ")]
    public Text publishDateText;
    [SerializeField]
    [Tooltip("color que tendrán las letras de las publicaciones posteadas por el jugador")]
    private Color publicationOwnedColor;
    [Header("Cards")]
    [SerializeField]
    [Tooltip("Imagen de un jugador")]
    private Image tokenCardImage;   
    [SerializeField]
    [Tooltip("Imagen de un Highlight")]
    private Image tokenHighlightCardImage;
    [Tooltip("Datos del objeto del mercadillo")]
    public JumbleSaleResult.JumbleItems jumbleSaleData;
    [SerializeField]
    [Tooltip("Imagen de potenciador activo en la carta")]
    private GameObject booster;
    [SerializeField]
    [Tooltip("Imagen de carta con lesión")]
    private GameObject injured;
    protected Action onSuccesDeletePublication; //Acción que se ejecuta cuando la compra se ha realizado exitosamente

    /// <summary>
    /// Configura el botón y los datos del objeto de la tienda
    /// </summary>
    /// <param name="itemData">Datos del objeto</param>
    /// <param name="onSuccesfulBuy">Acción que se ejecuta cuando la compra del objeto ha sido exitosa</param>
    /// <param name="onSuccesDeletePublication">Acción que se ejecuta cuando la el item se eliminó del mercadillo</param>
    /// 
    public new void SetupMallButton(JumbleSaleResult.JumbleItems itemData, Action onSuccesfulBuy,Action onSuccesDeletePublication)
    {
        buttonImage.gameObject.SetActive(false);
        tokenCardImage.transform.parent.gameObject.SetActive(false);
        tokenHighlightCardImage.transform.parent.gameObject.SetActive(false);
        if (itemData.seller_user_id == WebProcedure.Instance.accessData.user)
        {
            publishDateText.color = publicationOwnedColor;
            productName.color = publicationOwnedColor;
        }
       
        publishDateText.text = itemData.publication_date;
        this.jumbleSaleData = itemData;
        this.onSuccesfulBuy = onSuccesfulBuy;
        this.onSuccesDeletePublication = onSuccesDeletePublication;
        productName.text = itemData.item_type == "TOKENHIGTHLIGHT"? itemData.name : itemData.description;
        productCost.text = itemData.price;
        productButton.onClick.AddListener(OpenBuyPanel);
        booster.transform.parent.gameObject.SetActive(itemData.item_type == "TOKENCARD");
        booster.SetActive(itemData.is_booster);
        injured.SetActive(itemData.is_injured);
        WebProcedure.Instance.GetSprite(itemData.path_img, (Sprite obj) => { OnSuccessLoadingImage(itemData.item_type, obj); }, OnFailedLoadingImage);
    }
    /// <summary>
    /// Método que se ejecuta cuando backend carga la imagen del objeto de forma exitosa
    /// </summary>
    /// <param name="obj">Imagen del objeto</param>
    protected  void OnSuccessLoadingImage(string itemType,Sprite obj)
    {
        if(itemType == "TOKENCARD" && tokenCardImage )
        {
          
            tokenCardImage.transform.parent.gameObject.SetActive( true);
            tokenCardImage.sprite = obj;
        }
       else  if (itemType == "TOKENHIGTHLIGHT" && tokenHighlightCardImage )
        {
         
            tokenHighlightCardImage.transform.parent.gameObject.SetActive( true);
            tokenHighlightCardImage.sprite = obj;
        }
        else if(buttonImage)
        {
            buttonImage.gameObject.SetActive( true);
            buttonImage.sprite = obj;
        }

        DeactivateSpinner();

    }
    /// <summary>
    /// Abre el panel donde se confirma la compra del objeto
    /// </summary>
    protected override void OpenBuyPanel()
    {
        panelOpener.popupPrefab = panelBuyPrefab;
        panelOpener.OpenPopup();
        Sprite spriteImage = null;
        if (jumbleSaleData.item_type == "TOKENCARD")
        {

            spriteImage = tokenCardImage.sprite;
        }
        else if (jumbleSaleData.item_type == "TOKENHIGTHLIGHT")
        {
            tokenHighlightCardImage.transform.parent.gameObject.SetActive(true);
            spriteImage = tokenHighlightCardImage.sprite ;
        }
        else
        {
            buttonImage.gameObject.SetActive(true);
            spriteImage = buttonImage.sprite;
        }
        panelOpener.popup.GetComponent<PanelJumbleSaleBuyConfirmation>().SetupProductData(jumbleSaleData, spriteImage, onSuccesfulBuy, onSuccesDeletePublication);
    }
}
