using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebAPI;
using Data;
using Newtonsoft.Json;
public class PanelCajero : Panel
{
    [Header("Panel Components")]
    [SerializeField]
    [Tooltip("Controlador del lector QR")]
    private QRCodeDecodeController QRController;
    [SerializeField]
    [Tooltip("Panel de los tickets obtenidos")]
    private GameObject myTicketsPanel;
    [SerializeField]
    [Tooltip("panel del lector QR")]
    private GameObject QRReaderPanel;
    [SerializeField]
    [Tooltip("layout del lector QR")]
    private GameObject QRLayoutPanel;
    [SerializeField]
    [Tooltip("prefab de los pases VIP")]
    private GameObject ticketsPrefabPanel;
    [SerializeField]
    [Tooltip("contenedor de los pases VIP")]
    private Transform ticketsContainerLayout;

    #region unityFields
    /// <summary>
    /// desvincula los eventos al destruir el panel
    /// </summary>
    private void OnDestroy()
    {
        if (QRController != null)
            QRController.onQRScanFinished.RemoveAllListeners();
    }
    #endregion
    #region methods
    /// <summary>
    /// Detecta cuando un QR ha sido escaneado y lo comprueba
    /// </summary>
    /// <param name="code"></param>
    public void OnQRDecoded(string code)
    {
        RequestQRTicket request = new RequestQRTicket() { code = code, user_id = WebProcedure.Instance.accessData.user };
        WebProcedure.Instance.DigitalizePass(JsonConvert.SerializeObject(request), (DataSnapshot obj) => {
            try
            {
                MissionAlreadyComplete error = new MissionAlreadyComplete();
                JsonConvert.PopulateObject(obj.RawJson, error);
                if(error.code == 400)
                {
                    ACBSingleton.Instance.AlertPanel.SetupPanel(error.message,"", false, () => { CloseQRReader(); });
                    return;
                }
                else
                {
                    OnQRClaimed();
                }
            }
            catch {
                OnQRClaimed();
            }
        }, (WebError obj) => { 
            Debug.LogError(obj); });
    }
    /// <summary>
    /// El QR ha sido reconocido y ahora mostrará los pases obtenidos
    /// </summary>
    private void OnQRClaimed()
    {
        OpenVIPTicketsPanel();
        CloseQRReader();
    }
    /// <summary>
    /// destruye el panel de lector de QR
    /// </summary>
    private void CloseQRReader()
    {
        if (QRController != null)
        {
            QRController.GetComponent<PanelQr>().CloseQRPanel();
        }
    }
    /// <summary>
    /// Abre el panel lector de QR
    /// </summary>
    public void OpenQRPanel()
    {
        if (QRReaderPanel != null)
        {
            QRController = Instantiate(QRReaderPanel, QRLayoutPanel.transform).GetComponent<QRCodeDecodeController>();
            if(QRController != null)
            {
                QRController.onQRScanFinished.RemoveAllListeners();
                QRController.onQRScanFinished.AddListener(OnQRDecoded);
            }
        }
    }
    /// <summary>
    /// Abre el panel de los pases VIP obtenidos
    /// </summary>
    public void OpenVIPTicketsPanel()
    {
        myTicketsPanel.SetActive(true);
        WebProcedure.Instance.ViewPlayerPasses( (DataSnapshot obj) => {
            try
            {
                MissionAlreadyComplete error = new MissionAlreadyComplete();
                JsonConvert.PopulateObject(obj.RawJson, error);
                if (error.code == 400)
                {
                    ACBSingleton.Instance.AlertPanel.SetupPanel(error.message, "", false,null);
                    return;
                }
                else
                {
                    VIPPassesReturn passesReturn = new VIPPassesReturn();
                    JsonConvert.PopulateObject(obj.RawJson, passesReturn);
                    foreach(VIPPassesReturn.VIPPass ticket in passesReturn.data)
                    {
                        PanelTicketAuditory panelTicket = Instantiate(ticketsPrefabPanel, ticketsContainerLayout).GetComponent<PanelTicketAuditory>();
                        if (panelTicket != null) ;
                        panelTicket.SetupTicketPanel(ticket);
                    }
                }
            }
            catch
            {
                Debug.LogError(obj);
            }
        }, (WebError obj) => {
            Debug.LogError(obj);
        });

    }
    #endregion
}
