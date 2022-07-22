using System;
using Data;
using Panels;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using WebAPI;

/// <summary>
/// Controla el panel de que contiene los items de cartas
/// </summary>
public class PanelCardItem : MonoBehaviour
{
    #region Fields and Properties
    //Evento que se dispara al completar la publicación en el mercadillo
    public event PanelConfirmPublish.VoidDelegate OnConfirmedPublish;
    [Header("Card Components")]
    [SerializeField] [Tooltip("Máscara de la carta")]
    private Mask mask;
    [SerializeField] [Tooltip("Máscara del highlight")]
    private Sprite maskHighLight;
    [SerializeField] [Tooltip("Botón de la carta")]
    private Button button;
    [SerializeField] [Tooltip("Miniatura de la carta")]
    private Image imageThumbnail;
    [SerializeField] [Tooltip("Texto con el nombre de la carta")]
    private Text textName;
    [SerializeField] [Tooltip("Se ejecuta cuando la traida de datos de liga es fallida")]
    private UnityEvent onFailed;
    [SerializeField] [Tooltip("Datos de la carta")]
    private CardData.CardItemData currentToken;
    [SerializeField] [Tooltip("Datos del highlight")]
    private HighLightData.HigthlightItems currentHighLight;
    [Space(5)]
    [Header("Panel opener components")]
    [SerializeField] [Tooltip("Clase que controla la apertura de nuevos paneles a mostrar")]
    private PanelOpener panelOpener;
    [SerializeField] [Tooltip("Prefab de panel de añadir carta al equipo competitivo")]
    private GameObject panelAddPlayerPrefab;
    [SerializeField] [Tooltip("Prefab de añadir highlight")]
    private GameObject panelHighPrefab;

    [SerializeField] [Tooltip("Nombre del spinner")]
    private string spinner_name;
    
    private Action onClickedButton; //Acción que se ejecuta al hacer click en el botón

    #endregion

    #region Public Methods

    /// <summary>
    /// Muestra información de la carta
    /// </summary>
    /// <param name="tokendata">Datos de la carta</param>
    /// <param name="onClickedButton">Acción que se ejecuta al hacer click en el botón</param>
    ///     /// <param name="isJumbleSale">true si este item se mostrará en el mercadillo</param>
    public void ShowInfo(CardData.CardItemData tokendata, Action onClickedButton, bool isJumbleSale = false)
    {
        currentToken = tokendata;
        textName.text = tokendata.name;
        this.onClickedButton = onClickedButton;
        
        if (!string.IsNullOrEmpty(currentToken.pathThumbnail))
        {
            if((isJumbleSale && tokendata.enter ) || !isJumbleSale)
            {
                WebProcedure.Instance.GetSprite(currentToken.pathThumbnail, sprite =>
                {
                    if (imageThumbnail)
                    {
                        imageThumbnail.sprite = sprite;
                        imageThumbnail.gameObject.SetActive(true);
                        CloseSpinner();

                    }

                    if (tokendata.enter)
                    {
                        button.onClick.AddListener(() => OpenCustomPanel(isJumbleSale)); ;
                    }

                }, error =>
                {
                    onFailed.Invoke();
                });
            }
        }
    }

    /// <summary>
    /// Muestra información del highlight
    /// </summary>
    /// <param name="tokendata">Datos del highlight</param>
    /// <param name="onClickedButton">Acción que se ejecuta al hacer click en el botón</param>
    public void ShowInfo(HighLightData.HigthlightItems highlight, Action onClickedButton, bool isJumbleSale = false)
    {
        mask.GetComponent<Image>().sprite = maskHighLight;
        currentHighLight = highlight;
        textName.text = highlight.title;
        this.onClickedButton = onClickedButton;
        
        if (!string.IsNullOrEmpty(highlight.pathImgThumbnail))
        {
            if ((isJumbleSale && highlight.enter) || !isJumbleSale)
            {
                WebProcedure.Instance.GetSprite(highlight.pathImgThumbnail, sprite =>
                {
                    imageThumbnail.sprite = sprite;
                    imageThumbnail.gameObject.SetActive(true);
                    if (highlight.enter)
                    {
                        button.onClick.AddListener(() => { OpenCustomHightLightPanel(isJumbleSale); });
                    }
                    CloseSpinner();
                }, error =>
                {
                    onFailed.Invoke();
                });
            }
        }
    }

    #endregion

    #region Inner Methods

    /// <summary>
    /// Abre el panel de añadir carta al equipo competitivo
    /// </summary>
    private void OpenCustomPanel(bool isJumbleSale)
    {
        panelOpener.popupPrefab = panelAddPlayerPrefab;
        panelOpener.OpenPopup();
        if (isJumbleSale)
        {
            panelOpener.popup.GetComponent<PanelPublishPlayer>().CallInfo(currentToken.id, currentToken.name);
            panelOpener.popup.GetComponent<PanelPublishPlayer>().OnConfirmedPublish += () => { OnConfirmedPublish?.Invoke(); };
        }
        else panelOpener.popup.GetComponent<PanelAñadirPlayer>().CallInfo(currentToken.id, currentToken.name);
        onClickedButton?.Invoke();
    }
    
    /// <summary>
    /// Abre el panel de highlights
    /// </summary>
    private void OpenCustomHightLightPanel(bool isJumbleSale)
    {
        panelOpener.popupPrefab = panelHighPrefab;
        panelOpener.OpenPopup();
        if (isJumbleSale)
        {
            panelOpener.popup.GetComponent<PanelPublishPlayer>().CallInfo(currentHighLight.id, currentHighLight.title, isJumbleSale);
            panelOpener.popup.GetComponent<PanelPublishPlayer>().OnConfirmedPublish += () => { OnConfirmedPublish?.Invoke(); };
        }
        else panelOpener.popup.GetComponent<PanelHighLights>().Setup(currentHighLight);
        onClickedButton?.Invoke();
    }

    /// <summary>
    /// Oculta el spinner de carga
    /// </summary>
    private void CloseSpinner()
    {
        GameObject spinner = GameObject.Find(spinner_name);
        for(int i=0; i<spinner.transform.childCount; i++)
        {
            spinner.transform.GetChild(i).gameObject.SetActive(false);
        }  
    }

    #endregion

}
