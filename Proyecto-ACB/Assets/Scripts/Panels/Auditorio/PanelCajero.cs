using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WebAPI;
using Data;
using Newtonsoft.Json;
using System.Linq;
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
    private bool newPassReceived;

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
        SetSpinnerState(true);
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
        SetSpinnerState(false);
    }
    /// <summary>
    /// El QR ha sido reconocido y ahora mostrará los pases obtenidos
    /// </summary>
    private void OnQRClaimed()
    {
        SetSpinnerState(false);
        newPassReceived = true;
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
            if (QRController != null)
            {
                UnityEditor.Events.UnityEventTools.RemovePersistentListener(QRController.onQRScanFinished, 0);
                QRController.onQRScanFinished.AddListener(OnQRDecoded);
            }
        }
    }
    /// <summary>
    /// Abre el panel de los pases VIP obtenidos
    /// </summary>
    public void OpenVIPTicketsPanel()
    {
        ticketsContainerLayout.GetComponent<RectTransform>().sizeDelta = new Vector2(ticketsContainerLayout.GetComponent<RectTransform>().sizeDelta.x, 0);
        myTicketsPanel.SetActive(true);
        SetSpinnerState(true);
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
                    foreach (Transform child in ticketsContainerLayout)
                    {
                        Destroy(child.gameObject);
                    }
                    passesReturn.data = passesReturn.data.Reverse().ToArray();
                    foreach (VIPPassesReturn.VIPPass ticket in passesReturn.data)
                    {
                        PanelTicketAuditory panelTicket = Instantiate(ticketsPrefabPanel, ticketsContainerLayout).GetComponent<PanelTicketAuditory>();
                        if (panelTicket != null) ;
                        panelTicket.SetupTicketPanel(ticket);
                        ticketsContainerLayout.GetComponent<RectTransform>().sizeDelta = new Vector2(ticketsContainerLayout.GetComponent<RectTransform>().sizeDelta.x, (ticketsContainerLayout.GetComponent<RectTransform>().sizeDelta.y + panelTicket.GetComponent<LayoutElement>().preferredHeight + ticketsContainerLayout.GetComponent<VerticalLayoutGroup>().spacing + ticketsContainerLayout.GetComponent<VerticalLayoutGroup>().padding.top));
                        panelTicket.ticketBorder.GetComponent<Outline>().enabled = newPassReceived;
                        newPassReceived = false;
                    }
                    SetSpinnerState(false);
                }
            }
            catch
            {
                Debug.LogError(obj);
                SetSpinnerState(false);
            }
        }, (WebError obj) => {
            Debug.LogError(obj);
            SetSpinnerState(false);
        });

    }
    /// <summary>
    /// Activa o desactiva el spinner de carga
    /// </summary>
    /// <param name="state">Estado de activación del spinner</param>
    private void SetSpinnerState(bool state)
    {
        GameObject spinner = GameObject.Find("Spinner_TablonDesafio");
        for (int i = 0; i < spinner.transform.childCount; i++)
        {
            spinner.transform.GetChild(i).gameObject.SetActive(state);
        }
    }
    #endregion
}
