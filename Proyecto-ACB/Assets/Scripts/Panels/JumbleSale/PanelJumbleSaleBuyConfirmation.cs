using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Data;
using WebAPI;
using System;
using Newtonsoft.Json;
/// <summary>
/// panel de confirmación de compra del mercadillo
/// </summary>
public class PanelJumbleSaleBuyConfirmation : MallBuyConfirmation
{
    protected Action OnDeletePublish; //Action que se ejecuta al eliminarse un producto ;
    protected JumbleSaleResult.JumbleItems itemData; //Clase con los datos del item que se está comprando
    [SerializeField]
    [Tooltip("botón de eliminar publicación")]
    protected Button deleteButton;
    [SerializeField]
    [Tooltip("objeto que contiene la información de los costos de la publicación")]
    protected GameObject costInfoLayout;
    [SerializeField]
    [Tooltip("Primer título de alerta al eliminar un item")]
    private string alertDelete = "¿Estás seguro?";
    [SerializeField]
    [Tooltip("Descripción de alerta al eliminar un item")]
    private string alertDeleteDescription = "Esta oferta se eliminará automáticamente";
    /// <summary>
    /// Configura los datos a mostrar del producto a comprar
    /// </summary>
    /// <param name="itemData">Clase con los datos del producto seleccionado para comprar</param>
    /// <param name="productSprite">Imagen del producto a comprar</param>
    /// <param name="onSuccessfulbuy">Acción que se ejecuta cuando un objeto se ha comprado satisfactoriamente</param>
    public void SetupProductData(JumbleSaleResult.JumbleItems itemData, Sprite productSprite, Action onSuccessfulbuy, Action OnDeletePublish)
    {
        if (itemData.seller_user_id == WebProcedure.Instance.accessData.user && deleteButton != null)
        {
            costInfoLayout.SetActive(false);
            deleteButton.gameObject.SetActive(true);
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
                 if (itemData.seller_user_id == WebProcedure.Instance.accessData.user)
                {
                    minipanelToOpen = generalBuyMinipanel;
                }
                else
                {
                    minipanelToOpen = acballMinipanel;
                }
             
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
    /// Método que controla si el jugador puede comprar un item o no de acuerdo a la cantidad de ACBCoins que tenga
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
            ACBSingleton.Instance.AlertPanel.SetupPanel(enoughCurrencyAmountText, "Esta operación te costará " + itemData.price + " acbcoins", true, () => { BuyItem(itemData); }, () => { spinner.gameObject.SetActive(false); });
    }
    /// <summary>
    /// Método que se llama al presionar el botón de comprar producto, llamando a backend para verificar que la compra se pueda hacer
    /// </summary>
    /// <param name="itemData">Clase con los datos del producto que se quiere comprar</param>
    protected void BuyItem(JumbleSaleResult.JumbleItems itemData)
    {
        JumbleSaleResult.JumbleBuyRequest body = new JumbleSaleResult.JumbleBuyRequest() { item_id = itemData.id, user_id = WebProcedure.Instance.accessData.user };
        WebProcedure.Instance.BuyJumbleSaleItem(JsonConvert.SerializeObject(body), OnSuccessBuying, OnFailedBuying);
    }
    /// <summary>
    /// destruye la publicación creada en el mercadillo
    /// </summary>
    protected void DeletePublication(JumbleSaleResult.JumbleItems itemData)
    {
         ACBSingleton.Instance.AlertPanel.SetupPanel(alertDelete, alertDeleteDescription, true,()=> {
             JumbleSaleResult.JumbleDeleteItemRequest body = new JumbleSaleResult.JumbleDeleteItemRequest() { item_id = itemData.id, user_id = WebProcedure.Instance.accessData.user };
             WebProcedure.Instance.DeleteJumbleSaleItem(JsonConvert.SerializeObject(body), (DataSnapshot obj) => { OnDeletePublish?.Invoke(); Close(); }, (WebError obj) => { Debug.LogError(obj); });
         });
       
       
    }
    /// <summary>
    /// Método que se ejecuta cuando la compra del producto ha sido exitosa en backend y actualiza las analíticas de firebase
    /// </summary>
    /// <param name="obj">Clase con los datos de la compra exitosa</param>
    protected override void OnSuccessBuying(DataSnapshot obj)
    {
    
            ACBSingleton.Instance.AlertPanel.SetupPanel(onSuccessCode, "", false, () => { JsonConvert.PopulateObject(obj.RawJson, ACBSingleton.Instance.AccountData); Close(); onSuccessfulbuy?.Invoke(); });
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
