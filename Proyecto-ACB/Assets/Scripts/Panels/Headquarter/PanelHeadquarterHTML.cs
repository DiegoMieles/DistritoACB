using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controla el panel HTML de heaquarters
/// </summary>
public class PanelHeadquarterHTML : Panel
{

    #region Fields and Properties

    [Header("Panel components")]
    [SerializeField] [Tooltip("Botón que se encarga del cerrado del panel")]
    private Button goBackButton;
    [SerializeField] [Tooltip("Plugin de UniWebView")]
    private UniWebView webView;
    [SerializeField] [Tooltip("Texto de error al cargar de forma fallida la página HTML")]
    private string textErrorInfo;

    #endregion

    #region Public Methods

    /// <summary>
    /// Abre el panel de la vista HTML de headquarters
    /// </summary>
    /// <param name="HTMLUrl"></param>
    public void OpenPanel(string HTMLUrl)
    {
        goBackButton.onClick.AddListener(Close);
        if(!string.IsNullOrEmpty(HTMLUrl))
            webView.urlOnStart = HTMLUrl;
        else
            ACBSingleton.Instance.AlertPanel.SetupPanel(textErrorInfo, "", false, Close, null, 0.2f);
    }

    #endregion
}
