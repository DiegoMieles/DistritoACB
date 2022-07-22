using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Controla la vista HTML del panel de headquarters
/// </summary>
public class HeadquarterHTMLView : MonoBehaviour
{
    #region Fields and properties

    [Header("WebView components")]
    [SerializeField] [Tooltip("Plugin de UniWebView")]
    private UniWebView uniWebView;
    [SerializeField] [Tooltip("Evento que se ejecuta al presionar el bot�n dentro del HTML")]
    private UnityEvent onTapHtmlButton;

    #endregion

    #region Unity Methods

    /// <summary>
    /// Se ejecuta cuando se activa el controlador y suscribe eventos de carga de p�gina web
    /// </summary>
    private void OnEnable()
    {
        uniWebView.OnMessageReceived += UniWebViewOnOnMessageReceived;
    }

    /// <summary>
    /// Se ejecuta cuando se desactiva el controlador y suscribe eventos de carga de p�gina web
    /// </summary>
    private void OnDisable()
    {
        uniWebView.OnMessageReceived -= UniWebViewOnOnMessageReceived;
    }

    #endregion

    #region Inner Methods

    /// <summary>
    /// Ejecuta la acci�n cuando se selecciona el bot�n HTML
    /// </summary>
    /// <param name="webview">Plugin de UniWebView</param>
    /// <param name="message">Estructura con mensaje de WebView</param>
    private void UniWebViewOnOnMessageReceived(UniWebView webview, UniWebViewMessage message)
    {
        onTapHtmlButton?.Invoke();
    }

    #endregion

}
