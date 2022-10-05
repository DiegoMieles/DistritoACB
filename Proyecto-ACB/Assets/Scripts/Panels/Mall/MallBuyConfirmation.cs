using System;
using Data;
using WebAPI;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;

/// <summary>
/// Panel que controla la información del producto a comprar en la tienda
/// </summary>
public class MallBuyConfirmation : Panel
{
    #region Fields and Properties

    [Header("Panel components")]
    [SerializeField] [Tooltip("Botón que se encarga del cerrado del panel")]
    protected Button goBackButton;
    [SerializeField] [Tooltip("Botón que se encarga de la compra del producto de la tienda")]
    protected Button buyButton;
    [SerializeField] [Tooltip("Texto que muestra el precio del producto que se quiere comprar")]
    protected Text productPrice;
    [SerializeField] [Tooltip("Texto que contiene los datos de la cantidad de ACBCoins actuales que tiene el jugador")]
    protected Text currentCoins;
    [SerializeField] [Tooltip("Valor máximo a mostrar de monedas que tiene el jugador a nivel gráfico del panel")]
    protected float limit;

    [Space(5)]
    [Header("Mini panels")]
    [SerializeField] [Tooltip("Minipanel que muestra un producto con su información básica, por ejemplo, una skin")]
    protected BuyMiniPanel generalBuyMinipanel;
    [SerializeField] [Tooltip("Minipanel que muestra un producto de tipo carta o highlight con su información")]
    protected BuyMiniPanelCard cardBuyMinipanel;
    [SerializeField] [Tooltip("Minipanel que muestra la información de la acball junto con lo que esta puede traer dentro")]
    protected BuyMiniPanelACBall acballMinipanel;

    [Space(5)]
    [Header("Alert panel titles")]
    [SerializeField] [Tooltip("Texto a mostrar en el panel de alerta cuando el jugador tiene monedas suficientes para comprar el producto")]
    protected string enoughCurrencyAmountText;
    [SerializeField] [Tooltip("Texto a mostrar en el panel de alerta cuando el jugador no tiene monedas suficientes para comprar el producto")]
    protected string notEnoughCurrencyAmountText;
    [SerializeField] [Tooltip("Texto a mostrar en el panel de alerta cuando la compra de un producto ha sido exitosa")]
    protected string onSuccessCode;
    [SerializeField] [Tooltip("Texto a mostrar en el panel de alerta cuando la compra de un producto ha fallado")]
    protected string onFailedCode;
    [SerializeField] [Tooltip("Spinner de carga")]
    protected GameObject spinner;

    protected Action onSuccessfulbuy; //Action que se ejecuta al hacerse una compra exitosa de un producto
    protected MallContainerData.MallData.MallItems itemData; //Clase con los datos del item que se está comprando

    #endregion

    #region Public Methods

    /// <summary>
    /// Configura los datos a mostrar del producto a comprar
    /// </summary>
    /// <param name="itemData">Clase con los datos del producto seleccionado para comprar</param>
    /// <param name="productSprite">Imagen del producto a comprar</param>
    /// <param name="onSuccessfulbuy">Acción que se ejecuta cuando un objeto se ha comprado satisfactoriamente</param>
    public void SetupProductData(MallContainerData.MallData.MallItems itemData, Sprite productSprite, Action onSuccessfulbuy)
    {
        this.itemData = itemData;
        goBackButton.onClick.AddListener(Close);
        buyButton.onClick.AddListener(() => {  CheckIfCanBuyItem(itemData); } );
        productPrice.text = itemData.price.ToString();
        currentCoins.text = Mathf.Clamp(ACBSingleton.Instance.AccountData.statsData.coinsBalance, 0, limit).ToString();
        generalBuyMinipanel.gameObject.SetActive(false);
        cardBuyMinipanel.gameObject.SetActive(false);

        this.onSuccessfulbuy = onSuccessfulbuy;

        BuyMiniPanel minipanelToOpen = null;

        switch(itemData.elementType)
        {
            case ItemType.ACBALL:
                if(!itemData.random)
                    minipanelToOpen = acballMinipanel;
                else
                    minipanelToOpen = generalBuyMinipanel;
                break;

            case ItemType.TOKEN:
                if(!itemData.random)
                    minipanelToOpen = cardBuyMinipanel;
                else
                    minipanelToOpen = generalBuyMinipanel;

                break;

            case ItemType.HIGTHLIGHT:
                if(!itemData.random)
                    minipanelToOpen = cardBuyMinipanel;
                else
                    minipanelToOpen = generalBuyMinipanel;
                break;

            default:
                minipanelToOpen = generalBuyMinipanel;
                break;
        }

        minipanelToOpen.gameObject.SetActive(true);
        minipanelToOpen.ShowMiniPanel(productSprite, itemData, itemData.description, Close);
    }

    #endregion

    #region Inner Methods

    /// <summary>
    /// Método que controla si el jugador puede comprar un item o no de acuerdo a la cantidad de ACBCoins que tenga
    /// </summary>
    /// <param name="itemData">Clase con los datos del producto que se quiere comprar</param>
    protected void CheckIfCanBuyItem(MallContainerData.MallData.MallItems itemData)
    {
        spinner.gameObject.SetActive(true);
        if (itemData.price > ACBSingleton.Instance.AccountData.statsData.coinsBalance)
            ACBSingleton.Instance.AlertPanel.SetupPanel(notEnoughCurrencyAmountText, "", false, () => { spinner.gameObject.SetActive(false); });
        else
            ACBSingleton.Instance.AlertPanel.SetupPanel(enoughCurrencyAmountText, "Esta operación te costará " + itemData.price + " acbcoins", true, () => { BuyItem(itemData); }, () => { spinner.gameObject.SetActive(false); });
    }

    /// <summary>
    /// Método que se llama al presionar el botón de comprar producto, llamando a backend para verificar que la compra se pueda hacer
    /// </summary>
    /// <param name="itemData">Clase con los datos del producto que se quiere comprar</param>
    protected void BuyItem(MallContainerData.MallData.MallItems itemData)
    {
        MallContainerData.MallBody body = new MallContainerData.MallBody() { idPost = itemData.id };
        WebProcedure.Instance.PostMallBuy(JsonConvert.SerializeObject(body), OnSuccessBuying, OnFailedBuying);
    }

    /// <summary>
    /// Método que se ejecuta cuando la compra del producto ha sido exitosa en backend y actualiza las analíticas de firebase
    /// </summary>
    /// <param name="obj">Clase con los datos de la compra exitosa</param>
    protected virtual void OnSuccessBuying(DataSnapshot obj)
    {
        if (obj.Code == 200)
        {
            JsonConvert.PopulateObject(obj.RawJson, ACBSingleton.Instance.AccountData);
            MissionRewardData transactionData = new MissionRewardData();
            JsonConvert.PopulateObject(obj.RawJson, transactionData);
      
            ACBSingleton.Instance.AlertPanel.SetupPanel(onSuccessCode, "", false, () => {ACBSingleton.Instance.AccountData.statsData.coinsBalance = transactionData.balance; Close(); onSuccessfulbuy?.Invoke(); });
            Firebase.Analytics.Parameter param;
            switch (itemData.elementType)
            {
                case ItemType.ACBALL:
                    Debug.Log("Analytic buy_acball logged");
                    Firebase.Analytics.FirebaseAnalytics.LogEvent("buy_acball");
                    break;

                case ItemType.SKIN:

                    MallContainerData.MallData.MallItemsAvatar skinData = (MallContainerData.MallData.MallItemsAvatar)itemData.ChangeType(ItemType.SKIN);
                    JsonConvert.PopulateObject(obj.RawJson,skinData);
                    Debug.Log("Analytic buy_skin logged");

                    switch(skinData.element.type)
                    {
                        case ItemType.ARMACCESORY:
                            param = new Firebase.Analytics.Parameter("type", ItemType.ARMACCESORY.ToString());
                            Firebase.Analytics.FirebaseAnalytics.LogEvent("buy_skin", param);
                            Debug.Log("Analytic buy_skin logged"+ItemType.ARMACCESORY.ToString());
                            break;

                        case ItemType.BACKGROUNDACCESORY:
                            param = new Firebase.Analytics.Parameter("type", ItemType.BACKGROUNDACCESORY.ToString());
                            Firebase.Analytics.FirebaseAnalytics.LogEvent("buy_skin", param);
                            Debug.Log("Analytic buy_skin logged"+ItemType.BACKGROUNDACCESORY.ToString());
                            break;

                        case ItemType.BODYACCESORY:
                            param = new Firebase.Analytics.Parameter("type", ItemType.BODYACCESORY.ToString());
                            Firebase.Analytics.FirebaseAnalytics.LogEvent("buy_skin", param);
                            Debug.Log("Analytic buy_skin logged"+ItemType.BODYACCESORY.ToString());
                            break;

                        case ItemType.EYEACCESORY:
                            param = new Firebase.Analytics.Parameter("type", ItemType.EYEACCESORY.ToString());
                            Firebase.Analytics.FirebaseAnalytics.LogEvent("buy_skin", param);
                            Debug.Log("Analytic buy_skin logged"+ItemType.EYEACCESORY.ToString());
                            break;

                        case ItemType.FOREGROUNDACCESORY:
                            param = new Firebase.Analytics.Parameter("type", ItemType.FOREGROUNDACCESORY.ToString());
                            Firebase.Analytics.FirebaseAnalytics.LogEvent("buy_skin", param);
                            Debug.Log("Analytic buy_skin logged"+ItemType.FOREGROUNDACCESORY.ToString());
                            break;

                        case ItemType.HEADACCESORY:
                            param = new Firebase.Analytics.Parameter("type", ItemType.HEADACCESORY.ToString());
                            Firebase.Analytics.FirebaseAnalytics.LogEvent("buy_skin", param);
                            Debug.Log("Analytic buy_skin logged"+ItemType.HEADACCESORY.ToString());
                            break;
                    }
                    break;

                case ItemType.TOKEN:
                    param = new Firebase.Analytics.Parameter("type", ItemType.TOKEN.ToString());
                    Debug.Log("Analytic buy_card logged");
                    Firebase.Analytics.FirebaseAnalytics.LogEvent("buy_card", param);
                    break;

                case ItemType.HIGTHLIGHT:
                    param = new Firebase.Analytics.Parameter("type", ItemType.HIGTHLIGHT.ToString());
                    Debug.Log("Analytic buy_card logged");
                    Firebase.Analytics.FirebaseAnalytics.LogEvent("buy_card", param);
                    break;

                case ItemType.BOOSTER:
                    MallContainerData.MallData.MallItemsBooster boosterData = (MallContainerData.MallData.MallItemsBooster)itemData.ChangeType(ItemType.BOOSTER);
                    Debug.Log("Analytic buy_booster logged");
                    
                    switch(boosterData.element.type)
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
                    }
                    break;
            }
        }
        else if(!string.IsNullOrEmpty(obj.MessageCustom))
            ACBSingleton.Instance.AlertPanel.SetupPanel(obj.MessageCustom, "", false, Close);
        else
            ACBSingleton.Instance.AlertPanel.SetupPanel(ACBSingleton.Instance.LostConnectionTextError, "", false, Close);

        Debug.Log(obj.RawJson);
    }

    /// <summary>
    /// Método que se ejecuta cuando una compra en backend ha sido fallida
    /// </summary>
    /// <param name="obj"></param>
    protected void OnFailedBuying(WebError obj)
    {
        ACBSingleton.Instance.AlertPanel.SetupPanel(onFailedCode, "", false, null);
        ClosingSpinner();
    }

    /// <summary>
    /// Desactiva el spinner de carga
    /// </summary>
    public void ClosingSpinner()
    {
        spinner.gameObject.SetActive(false);
    }

    #endregion
}
