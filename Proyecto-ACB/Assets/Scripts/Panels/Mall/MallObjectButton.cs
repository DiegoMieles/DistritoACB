using System;
using Data;
using WebAPI;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controla y asigna los datos de un objeto que se puede comprar en la tienda
/// </summary>
public class MallObjectButton : MonoBehaviour
{
    #region Fields and properties

    [Header("Button components")]
    [SerializeField] [Tooltip("Botón del producto")]
    protected Button productButton;
    [SerializeField] [Tooltip("Imagen del producto")]
    protected Image buttonImage;
    [SerializeField] [Tooltip("Texto con el nombre del producto")]
    protected Text productName;
    [SerializeField] [Tooltip("Texto con el costo del producto")]
    protected Text productCost;
    [SerializeField] [Tooltip("Clase que controla la apertura de nuevos paneles a mostrar")]
    protected PanelOpener panelOpener;
    [SerializeField] [Tooltip("Prefab del panel de confirmación de compra")]
    protected GameObject panelBuyPrefab;
    [Tooltip("Datos del objeto de tienda")]
    public MallContainerData.MallData.MallItems itemData;
    [SerializeField] [Tooltip("Sprite del lugar")]
    protected Sprite spritePlace;
    
    protected Action onSuccesfulBuy; //Acción que se ejecuta cuando la compra se ha realizado exitosamente
    protected bool loaded; //Determina si los datos del objeto han sido cargados exitosamente
    protected bool imageloaded; //Determina si la imagen del objeto ha sido cargada
    protected Coroutine _coroutine; //Corrutina de carga de imagen del objeto de la tienda

    #endregion

    #region Public Methods

    /// <summary>
    /// Carga la imagen del objeto
    /// </summary>
    public void LoadImage()
    {
        if (!loaded) 
            return;
        if (imageloaded) 
            return;
        if (buttonImage.sprite != spritePlace) 
            return;
        
        imageloaded = true;
        buttonImage.sprite = spritePlace;
        _coroutine =
            StartCoroutine(WebProcedure.Instance.GetSpriteCoroutine(itemData.path_img, OnSuccessLoadingImage,
                OnFailedLoadingImage));
    }

    /// <summary>
    /// Elimina la imagen del objeto
    /// </summary>
    public void DestroyImage()
    {
        if (!loaded)
            return;
        if (buttonImage.sprite == spritePlace) 
            return;
        DestroyImmediate(buttonImage.sprite);
        buttonImage.sprite = spritePlace;
        imageloaded = false;
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine); 
        }
    }

    /// <summary>
    /// Configura el botón y los datos del objeto de la tienda
    /// </summary>
    /// <param name="itemData">Datos del objeto</param>
    /// <param name="onSuccesfulBuy">Acción que se ejecuta cuando la compra del objeto ha sido exitosa</param>
    public void SetupMallButton(MallContainerData.MallData.MallItems itemData, Action onSuccesfulBuy)
    {
        this.itemData = itemData;
        this.onSuccesfulBuy = onSuccesfulBuy;
        productName.text = itemData.description;
        productCost.text = itemData.price.ToString();
        productButton.onClick.AddListener(OpenBuyPanel);
        WebProcedure.Instance.GetSprite(itemData.path_img, OnSuccessLoadingImage, OnFailedLoadingImage);
        loaded = true;
    }  
    /// <summary>
    /// Configura el botón y los datos del objeto del mercadillo
    /// </summary>
    /// <param name="itemData">Datos del objeto</param>
    /// <param name="onSuccesfulBuy">Acción que se ejecuta cuando la compra del objeto ha sido exitosa</param>
    public void SetupMallButton(JumbleSaleResult.JumbleItems itemData, Action onSuccesfulBuy)
    {
    }

    #endregion

    #region protected Methods

    /// <summary>
    /// Método que se ejecuta cuando backend carga la imagen del objeto de forma exitosa
    /// </summary>
    /// <param name="obj">Imagen del objeto</param>
    protected virtual void OnSuccessLoadingImage(Sprite obj)
    {
        buttonImage.sprite = obj;
        DeactivateSpinner();

    }

    /// <summary>
    /// Método que se ejecuta cuando backend no puede cargar la imagen del objeto
    /// </summary>
    /// <param name="obj">Clase con los datos de error</param>
    protected void OnFailedLoadingImage(WebError obj)
    {
        Debug.Log(obj.Message);
        DeactivateSpinner();
    }

    /// <summary>
    /// Abre el panel donde se confirma la compra del objeto
    /// </summary>
    protected virtual void OpenBuyPanel()
    {
        panelOpener.popupPrefab = panelBuyPrefab;
        panelOpener.OpenPopup();
        panelOpener.popup.GetComponent<MallBuyConfirmation>().SetupProductData(itemData, buttonImage.sprite, onSuccesfulBuy);
    }

    /// <summary>
    /// Desactiva el spinner de carga
    /// </summary>
    protected void DeactivateSpinner()
    {
        GameObject spinner = GameObject.Find("Spinner_mall");

        if (!spinner.activeInHierarchy)
            return;

        for (int i = 0; i < spinner.transform.childCount; i++)
        {
            spinner.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    #endregion
}
