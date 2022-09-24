using Firebase.Extensions;
using UnityEngine;
using Newtonsoft.Json;
using WebAPI;

namespace UniWebViews
{
    /// <summary>
    /// Controlador de eventos de autenticaci�n
    /// </summary>
    public class UniWebViewEventEditAccount : MonoBehaviour
    {
        #region Fields and Properties

        [SerializeField] [Tooltip("Plugin de UniWebView")]
        private UniWebView uniWebView;

        #endregion

        #region Unity Methods

        /// <summary>
        /// Se ejecuta cuando se activa el controlador, suscribiendo los eventos cada vez que se realiza una autenticaci�n
        /// </summary>
        private void OnEnable()
        {
            uniWebView.OnMessageReceived += UniWebViewOnOnMessageReceived;
            uniWebView.OnPageFinished+= UniWebViewOnOnPageFinished;
            
            Firebase.Messaging.FirebaseMessaging.GetTokenAsync().ContinueWithOnMainThread(
                task => {
                    var  token = task.Result;
                    Debug.Log("Creado TOken: "+token);
                }
            );
        }

        /// <summary>
        /// Asigna el protocolo https para acceder al sitio web donde se hace login y autenticaci�n
        /// </summary>
        /// <param name="webview"></param>
        /// <param name="statuscode"></param>
        /// <param name="url"></param>
        private void UniWebViewOnOnPageFinished(UniWebView webview, int statuscode, string url)
        {
            webview.AddUrlScheme("https");

        }

        /// <summary>
        /// Cuando se desactiva el controlador, desactiva los eventos suscritos de autenticaci�n
        /// </summary>
        private void OnDisable()
        {
            uniWebView.OnMessageReceived -= UniWebViewOnOnMessageReceived;
            uniWebView.OnPageFinished -= UniWebViewOnOnPageFinished;
        }
        
        #endregion

        #region Inner Methods

        /// <summary>
        /// Abre la p�gina principal de login
        /// </summary>
        /// <param name="webview">Plugin de UniWebView</param>
        /// <param name="message">Estructura con mensaje de WebView</param>
        private void UniWebViewOnOnMessageReceived(UniWebView webview, UniWebViewMessage message)
        {
            ACBSingleton.Instance.onUserDeleted?.Invoke();
            Debug.LogError(message);
            if (message.Path.Equals("www.acb.com"))
            {
               
                if (message.Args.ContainsKey("delete_status"))
                {
                    Debug.LogError("delete_status: " + message.Args["delete_status"]);
                    
                    Firebase.Messaging.FirebaseMessaging.GetTokenAsync().ContinueWithOnMainThread(
                        task => {
                            var  token = task.Result;
                            print(message.Args["delete_status"]);
                            
                        }
                    );
                   
                }
                else
                {
                    Debug.LogError("No devolvi� mensaje: "+webview.Url);
                }
            }
        }
        
        #endregion
    }
}
