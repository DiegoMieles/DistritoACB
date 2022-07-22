using System;
using System.Linq;
using Data;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace UniWebViews
{
    /// <summary>
    /// Controla los eventos de web view relacionado a las misiones
    /// </summary>
    public class UniWebViewEventManager : MonoBehaviour
    {
        [SerializeField] [Tooltip("Panel de misi�n")]
        private PanelMision panelMision;
        [SerializeField] [Tooltip("Plugin de UniWebView")]
        private UniWebView uniWebView;
        [SerializeField] [Tooltip("Bot�n HTML de QR")]
        private UnityEvent onTapHtmlButtonQR;
        [SerializeField] [Tooltip("Bot�n HTML de AR")]
        private UnityEvent onTapHtmlButtonAR;
        [FormerlySerializedAs("onTapHtmlButton")] [FormerlySerializedAs("onCloseQr")] [SerializeField]
        private UnityEvent onTapHtmlButtonCode;

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
        /// Al seleccionar la misi�n desde la p�gina web abre el panel QR o AR seg�n corresponda
        /// </summary>
        /// <param name="webview">Plugin de UniWebView</param>
        /// <param name="message">Estructura con mensaje de WebView</param>
        private void UniWebViewOnOnMessageReceived(UniWebView webview, UniWebViewMessage message)
        {
          
            switch (panelMision.CurrentMission.type)
            {
                case MissionsData.MissionType.AR:
                    panelMision.CheckIfARPanelCanBeOpened();
                    onTapHtmlButtonAR.Invoke();
                    break;
                case MissionsData.MissionType.QR:
                    panelMision.OpenQRPanel();
                    onTapHtmlButtonQR.Invoke();
                    break;
                case MissionsData.MissionType.CODE:
                    Debug.LogError(message.Args.FirstOrDefault().Value);
                    panelMision.OpenCodePanel(message.Args.FirstOrDefault().Value);
                    onTapHtmlButtonCode.Invoke();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

    }
}
