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



    #region unityFields
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
    private void OnQRClaimed()
    {
        myTicketsPanel.SetActive(true);
        CloseQRReader();
    }
    private void CloseQRReader()
    {
        if (QRController != null)
        {
            QRController.GetComponent<PanelQr>().CloseQRPanel();
        }
    }
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
    #endregion
}
