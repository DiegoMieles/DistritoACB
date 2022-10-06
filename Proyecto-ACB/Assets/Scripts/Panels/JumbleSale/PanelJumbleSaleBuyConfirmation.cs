using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Data;
using WebAPI;
using System;
using Newtonsoft.Json;
/// <summary>
/// panel de confirmaci?n de compra del mercadillo
/// </summary>
public class PanelJumbleSaleBuyConfirmation : MallBuyConfirmation
{
    protected Action OnDeletePublish; //Action que se ejecuta al eliminarse un producto ;
    protected JumbleSaleResult.JumbleItems itemData; //Clase con los datos del item que se est? comprando
    [SerializeField]
    [Tooltip("bot?n de eliminar publicaci?n")]
    public Button deleteButton;
    [SerializeField]
    [Tooltip("objeto que contiene la informaci?n de los costos de la publicaci?n")]
    public GameObject costInfoLayout;
    [SerializeField]
    [Tooltip("Primer t?tulo de alerta al eliminar un item")]
    private string alertDelete = "?Est?s seguro?";
    [SerializeField]
    [Tooltip("Descripci?n de alerta al eliminar un item")]
    private string alertDeleteDescription = "Esta oferta se eliminar? autom?ticamente";  
    [SerializeField][TextArea]
    [Tooltip("Descripci?n de alerta al eliminar un item")]
    private string confirmationBuyText ;
    [Header("Mini panels")]
    [SerializeField]
    [Tooltip("Minipanel que muestra la informaci?n de la acball publicado junto con lo que esta puede traer dentro")]
    protected BuyMiniPanelACBall acballMinipanelOwned;
    /// <summary>
    /// Configura los datos a mostrar del producto a comprar
    /// </summary>
    /// <param name="itemData">Clase con los datos del producto seleccionado para comprar</param>
    /// <param name="productSprite">Imagen del producto a comprar</param>
    /// <param name="onSuccessfulbuy">Acci?n que se ejecuta cuando un objeto se ha comprado satisfactoriamente</param>
    public void SetupProductData(JumbleSaleResult.JumbleItems itemData, Sprite productSprite, Action onSuccessfulbuy, Action OnDeletePublish)
    {
        if (itemData.seller_user_id == WebProcedure.Instance.accessData.user && deleteButton != null)
        {
            costInfoLayout.SetActive(false);
            deleteButton.transform.parent.gameObject.SetActive(true);
            deleteButton.onClick.AddListener(() => { DeletePublication(itemData); });
        }
        this.itemData = itemData;
        goBackButton.onClick.AddListener(Close);
        buyButton.onClick.AddListener(() => { CheckIfCanBuyItem(itemData); });
        productPrice.text = itemData.price.ToString();
        currentCoins.text = Mathf.Clamp(ACBSingleton.Instance.AccountData.statsData.coinsBalance, 0, limit).ToString();
        generalBuyMinipanel.gameObject.SetActive(false);
        cardBuyMinipanel.gameObject.SetActive(false);

        this.onSuccessfulbuy = onSuccessfulbuy;
        this.OnDeletePublish = OnDeletePublish;
        BuyMiniPanel minipanelToOpen = null;

        switch (itemData.item_type)
        {
            case "ACBALL":
                minipanelToOpen = itemData.seller_user_id == WebProcedure.Instance.accessData.user ? acballMinipanelOwned : acballMinipanel ;
                break;
            case "TOKENCARD":
                minipanelToOpen = cardBuyMinipanel;
                break;
            case "TOKENHIGTHLIGHT":
                minipanelToOpen = cardBuyMinipanel;
                break;
            default:
                minipanelToOpen = generalBuyMinipanel;
                break;
        }

        minipanelToOpen.gameObject.SetActive(true);
        minipanelToOpen.ShowMiniPanel(productSprite, itemData, itemData.description, Close);
    }
    /// <summary>
    /// M?todo que controla si el jugador puede comprar un item o no de acuerdo a la cantidad de ACBCoins que tenga
    /// </summary>
    /// <param name="itemData">Clase con los datos del producto que se quiere comprar</param>
    protected void CheckIfCanBuyItem(JumbleSaleResult.JumbleItems itemData)
    {
        spinner.gameObject.SetActive(true);
        float price = -1;
        float.TryParse(itemData.price.Split('.')[0], out price);
        if (price > -1 && price > ACBSingleton.Instance.AccountData.statsData.coinsBalance)
            ACBSingleton.Instance.AlertPanel.SetupPanel(notEnoughCurrencyAmountText, "", false, () => { spinner.gameObject.SetActive(false); });
        else
            ACBSingleton.Instance.AlertPanel.SetupPanel(enoughCurrencyAmountText, confirmationBuyText + itemData.price + " acbcoins", true, () => { BuyItem(itemData); }, () => { spinner.gameObject.SetActive(false); });
    }
    /// <summary>
    /// M?todo que se llama al presionar el bot?n de comprar producto, llamando a backend para verificar que la compra se pueda hacer
    /// </summary>
    /// <param name="itemData">Clase con los datos del producto que se quiere comprar</param>
    protected void BuyItem(JumbleSaleResult.JumbleItems itemData)
    {
        JumbleSaleResult.JumbleBuyRequest body = new JumbleSaleResult.JumbleBuyRequest() { item_id = itemData.id, user_id = WebProcedure.Instance.accessData.user, item_type = itemData.item_type };
        WebProcedure.Instance.BuyJumbleSaleItem(JsonConvert.SerializeObject(body), OnSuccessBuying, OnFailedBuying);
    }
    /// <summary>
    /// destruye la publicaci?n creada en el mercadillo y crea lo p?neles de confirmaci?n
    /// </summary>
    protected void DeletePublication(JumbleSaleResult.JumbleItems itemData)
    {
         ACBSingleton.Instance.AlertPanel.SetupPanel(alertDelete, alertDeleteDescription, true,()=> {
             JumbleSaleResult.JumbleDeleteItemRequest body = new JumbleSaleResult.JumbleDeleteItemRequest() { item_id = itemData.id, user_id = WebProcedure.Instance.accessData.user };
             WebProcedure.Instance.DeleteJumbleSaleItem(JsonConvert.SerializeObject(body), (DataSnapshot obj) => {
                 MissionAlreadyComplete error = new MissionAlreadyComplete();
                 try
                 {
                     JsonConvert.PopulateObject(obj.RawJson, error);
                     if (error.code == 400)
                     {
                         ACBSingleton.Instance.AlertPanel.SetupPanel(error.message, "", false, () => { OnDeletePublish?.Invoke(); Close(); });
                         return;
                     }
                 }
                 catch
                 {
                    
                 }
                 ACBSingleton.Instance.AlertPanel.SetupPanel("Oferta eliminada", "", false, () => { OnDeletePublish?.Invoke(); Close(); });
             }, (WebError obj) => {
                         Debug.LogError(obj); });
         });
       
       
    }
    /// <summary>
    /// M?todo que se ejecuta cuando la compra del producto ha sido exitosa en backend y actualiza las anal?ticas de firebase
    /// </summary>
    /// <param name="obj">Clase con los datos de la compra exitosa</param>
    protected override void OnSuccessBuying(DataSnapshot obj)
    {

        MissionAlreadyComplete error = new MissionAlreadyComplete();
        try
        {
            JsonConvert.PopulateObject(obj.RawJson, error);
            if (error.code != 200 && !string.IsNullOrEmpty( error.message))
            {
                ACBSingleton.Instance.AlertPanel.SetupPanel(error.message, "", false, () => { Close();  onSuccessfulbuy?.Invoke();});
                return;
            }
        }
        catch
        {}
        JumbleResult jumbleResult = new JumbleResult();
        JsonConvert.PopulateObject(obj.RawJson, jumbleResult);
        JsonConvert.PopulateObject(obj.RawJson, ACBSingleton.Instance.AccountData);
        ACBSingleton.Instance.AlertPanel.SetupPanel(onSuccessCode, "", false, () => { ACBSingleton.Instance.AccountData.statsData.coinsBalance = jumbleResult.balance; Close(); onSuccessfulbuy?.Invoke(); });
            Firebase.Analytics.Parameter param;
            switch (itemData.item_type)
            {
                case "ACBALL":
                    Debug.Log("Analytic buy_acball logged");
                    Firebase.Analytics.FirebaseAnalytics.LogEvent("buy_acball");
                    break;

                case "SKIN":

                   /* MallContainerData.MallData.MallItemsAvatar skinData = (MallContainerData.MallData.MallItemsAvatar)itemData.ChangeType(ItemType.SKIN);
                    Debug.Log("Analytic buy_skin logged");

                    switch (skinData.element.itemType)
                    {
                        case ItemType.ARMACCESORY:
                            param = new Firebase.Analytics.Parameter("type", ItemType.ARMACCESORY.ToString());
                            Firebase.Analytics.FirebaseAnalytics.LogEvent("buy_skin", param);
                            break;

                        case ItemType.BACKGROUNDACCESORY:
                            param = new Firebase.Analytics.Parameter("type", ItemType.BACKGROUNDACCESORY.ToString());
                            Firebase.Analytics.FirebaseAnalytics.LogEvent("buy_skin", param);
                            break;

                        case ItemType.BODYACCESORY:
                            param = new Firebase.Analytics.Parameter("type", ItemType.BODYACCESORY.ToString());
                            Firebase.Analytics.FirebaseAnalytics.LogEvent("buy_skin", param);
                            break;

                        case ItemType.EYEACCESORY:
                            param = new Firebase.Analytics.Parameter("type", ItemType.EYEACCESORY.ToString());
                            Firebase.Analytics.FirebaseAnalytics.LogEvent("buy_skin", param);
                            break;

                        case ItemType.FOREGROUNDACCESORY:
                            param = new Firebase.Analytics.Parameter("type", ItemType.FOREGROUNDACCESORY.ToString());
                            Firebase.Analytics.FirebaseAnalytics.LogEvent("buy_skin", param);
                            break;

                        case ItemType.HEADACCESORY:
                            param = new Firebase.Analytics.Parameter("type", ItemType.HEADACCESORY.ToString());
                            Firebase.Analytics.FirebaseAnalytics.LogEvent("buy_skin", param);
                            break;
                    }*/
                    break;

                case "TOKENCARD":
                    param = new Firebase.Analytics.Parameter("type", ItemType.TOKEN.ToString());
                    Debug.Log("Analytic buy_card logged");
                    Firebase.Analytics.FirebaseAnalytics.LogEvent("buy_card", param);
                    break;

                case "TOKENHIGTHLIGHT":
                    param = new Firebase.Analytics.Parameter("type", ItemType.HIGTHLIGHT.ToString());
                    Debug.Log("Analytic buy_card logged");
                    Firebase.Analytics.FirebaseAnalytics.LogEvent("buy_card", param);
                    break;

                case "BOOSTER":
                    /*allContainerData.MallData.MallItemsBooster boosterData = (MallContainerData.MallData.MallItemsBooster)itemData.ChangeType(ItemType.BOOSTER);
                    Debug.Log("Analytic buy_booster logged");

                    switch (boosterData.element.type)
                    {
                        case BoosterType.FREESHOTS:
                            param = new Firebase.Analytics.Parameter("type", BoosterType.FREESHOTS.ToString());
                            Firebase.Analytics.FirebaseAnalytics.LogEvent("buy_booster", param);
                            break;

                        case BoosterType.TRIPLES:
                            param = new Firebase.Analytics.Parameter("type", BoosterType.TRIPLES.ToString());
                            Firebase.Analytics.FirebaseAnalytics.LogEvent("buy_booster", param);
                            break;

                        case BoosterType.POINTS:
                            param = new Firebase.Analytics.Parameter("type", BoosterType.POINTS.ToString());
                            Firebase.Analytics.FirebaseAnalytics.LogEvent("buy_booster", param);
                            break;

                        case BoosterType.ASSISTS:
                            param = new Firebase.Analytics.Parameter("type", BoosterType.ASSISTS.ToString());
                            Firebase.Analytics.FirebaseAnalytics.LogEvent("buy_booster", param);
                            break;

                        case BoosterType.REBOUNDS:
                            param = new Firebase.Analytics.Parameter("type", BoosterType.REBOUNDS.ToString());
                            Firebase.Analytics.FirebaseAnalytics.LogEvent("buy_booster", param);
                            break;
                    }*/
                    break;
            }


        Debug.Log(obj.RawJson);
    }

}
