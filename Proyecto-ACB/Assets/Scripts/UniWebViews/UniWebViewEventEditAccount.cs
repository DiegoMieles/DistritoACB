using Firebase.Extensions;
using UnityEngine;
using Newtonsoft.Json;
using WebAPI;

namespace UniWebViews
{
    /// <summary>
    /// Controlador de eventos de autenticación
    /// </summary>
    public class UniWebViewEventEditAccount : MonoBehaviour
    {
        #region Fields and Properties

        [SerializeField] [Tooltip("Plugin de UniWebView")]
        private UniWebView uniWebView;

        #endregion

        #region Unity Methods

        /// <summary>
        /// Se ejecuta cuando se activa el controlador, suscribiendo los eventos cada vez que se realiza una autenticación
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
        /// Asigna el protocolo https para acceder al sitio web donde se hace login y autenticación
        /// </summary>
        /// <param name="webview"></param>
        /// <param name="statuscode"></param>
        /// <param name="url"></param>
        private void UniWebViewOnOnPageFinished(UniWebView webview, int statuscode, string url)
        {
            webview.AddUrlScheme("https");
        }

        /// <summary>
        /// Cuando se desactiva el controlador, desactiva los eventos suscritos de autenticación
        /// </summary>
        private void OnDisable()
        {
            uniWebView.OnMessageReceived -= UniWebViewOnOnMessageReceived;
            uniWebView.OnPageFinished -= UniWebViewOnOnPageFinished;
        }
        
        #endregion

        #region Inner Methods

        /// <summary>
        /// Abre la página principal de login
        /// </summary>
        /// <param name="webview">Plugin de UniWebView</param>
        /// <param name="message">Estructura con mensaje de WebView</param>
        private void UniWebViewOnOnMessageReceived(UniWebView webview, UniWebViewMessage message)
        {
            Debug.LogError(message);
            if (message.Path.Equals("www.acb.com"))
            {
                if(message.Args.ContainsKey("code"))
                {
                    Debug.LogError("code: "+message.Args["code"]);
                    
                    Firebase.Messaging.FirebaseMessaging.GetTokenAsync().ContinueWithOnMainThread(
                        task => {
                            var  token = task.Result;
                            WebProcedure.Instance.PostacbiSetCode(message.Args["code"], token, ONSuccess, ONFailed );
                        }
                    );
                }
                else
                {
                    Debug.LogError("No trabajo el code: "+webview.Url);
                }
            }
        }
        

        /// <summary>
        /// Se ejecuta al fallar la autenticación del usuario en la página
        /// </summary>
        /// <param name="obj">Datos de backend del error</param>
        private void ONFailed(WebError obj)
        {
            Debug.Log("Fallo: " +obj.Message);
        }

        /// <summary>
        /// Completa la autenticación del usuario
        /// </summary>
        /// <param name="obj"></param>
        private void ONSuccess(DataSnapshot obj)
        {
            if (obj.Code == 200)
            {
                Debug.Log("Todo Bien: " +obj.RawJson);

                JsonConvert.PopulateObject(obj.RawJson,  WebProcedure.Instance.accessData);
                
                Debug.LogError("  CurrentUser.accessToken: "+   WebProcedure.Instance.accessData.accessToken);
                Debug.LogError("  CurrentUser.refreshToken: "+   WebProcedure.Instance.accessData.refreshToken);
            
                PlayerPrefs.SetString(WebProcedure.CREDENTIALS, obj.RawJson);
                ACBSingleton.Instance.onUserAuthenticated?.Invoke();
                
            }
            else
            {
                Debug.LogError("Fallo algo: "+obj.RawJson);
            }
        }

        #endregion
    }
}
