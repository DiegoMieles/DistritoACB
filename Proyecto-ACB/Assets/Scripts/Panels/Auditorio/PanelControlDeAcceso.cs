using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WebAPI;
using Data;
using Newtonsoft.Json;
using System.Linq;
public class PanelControlDeAcceso : Panel
{
    /// <summary>
    /// Segundos para sacar al jugador del auditorio
    /// </summary>
    static float SECONDS_TO_KICK_OUT_PLAYER = 60;
    [SerializeField]
    [Tooltip("layout donde están los asientos del auditorio")]
    private GameObject layoutAuditory;
    [SerializeField]
    [Tooltip("layout donde están los asientos del auditorio")]
    private GameObject layoutSits;
    [SerializeField]
    [Tooltip("layout del asiento en el auditorio")]
    private GameObject layoutBackSit;
    [SerializeField]
    [Tooltip("prefab de los pases VIP")]
    private GameObject ticketsPrefabPanel;
    [SerializeField]
    [Tooltip("contenedor de los pases VIP")]
    private Transform ticketsContainerLayout;
    [SerializeField]
    [Tooltip("contenedor de los pases VIP")]
    private Button useTicketVIPButton;
    [SerializeField]
    [Tooltip("encargado de abrir los paneles")]
    private PanelOpener PanelOpener;
    [SerializeField]
    [Tooltip("panel donde se muestran los videos del ticket")]
    private GameObject panelVideosPrefab;
    /// <summary>
    /// iD del asiento ocupado por el jugador 
    /// </summary>
    private int seatID;
    /// <summary>
    /// el jugador está dentro del auditorio? 
    /// </summary>
    private bool isInAuditory;
    private void OnEnable()
    {
        OpenVIPTicketsPanel();
        layoutAuditory.SetActive(false);
    }
   
    /// <summary>
    /// Abre el panel de los pases VIP obtenidos
    /// </summary>
    public void OpenVIPTicketsPanel()
    {
        ACBSingleton.Instance.PanelBuildingSelection.transform.SetSiblingIndex(ACBSingleton.Instance.PanelBuildingSelection.transform.parent.childCount - 2);
        WebProcedure.Instance.ViewPlayerPasses((DataSnapshot obj) => {
            try
            {
                MissionAlreadyComplete error = new MissionAlreadyComplete();
                JsonConvert.PopulateObject(obj.RawJson, error);
                if (error.code == 400)
                {
                    ACBSingleton.Instance.AlertPanel.SetupPanel(error.message, "", false, null);
                    return;
                }
                else
                {
                    VIPPassesReturn passesReturn = new VIPPassesReturn();
                    JsonConvert.PopulateObject(obj.RawJson, passesReturn);
                    foreach (VIPPassesReturn.VIPPass ticket in passesReturn.data)
                    {
                        PanelTicketAuditory panelTicket = Instantiate(ticketsPrefabPanel, ticketsContainerLayout).GetComponent<PanelTicketAuditory>();
                        if (panelTicket != null) ;
                        panelTicket.SetupTicketPanel(ticket,true,GetComponent<ToggleGroup>(), UpdatePassButton);
                    }
                    ACBSingleton.Instance.PanelBuildingSelection.innerBuildingName.text = ACBSingleton.Instance.PanelBuildingSelection.cachedBuildingsStack.Peek().previousBuildingData.infoTitle;
                    ACBSingleton.Instance.PanelBuildingSelection.innerBuildingIcon.sprite = ACBSingleton.Instance.PanelBuildingSelection.cachedBuildingsStack.Peek().previousBuildingData.buildingIcon;
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
    /// <summary>
    /// Selecciona el toggle marcado , y con su id se intenta usar el pase
    /// </summary>
    public void UseVIPPass()
    {
        RequestUseVIPPass request = new RequestUseVIPPass() { userpass_id = GetSelectedToggle().data.id, user_id = WebProcedure.Instance.accessData.user };
        WebProcedure.Instance.UseVIPPass(JsonConvert.SerializeObject(request), (DataSnapshot obj) => {
            try
            {
                MissionAlreadyComplete error = new MissionAlreadyComplete();
                JsonConvert.PopulateObject(obj.RawJson, error);
                if (error.code == 400)
                {
                    ACBSingleton.Instance.AlertPanel.SetupPanel(error.message, "", false, () => { });
                    return;
                }
                else
                {
                    OpenAuditoryRoom();
                }
            }
            catch
            {
                OpenAuditoryRoom();
            }
        }, (WebError obj) => {
            Debug.LogError(obj);
        });
    }
    /// <summary>
    /// Obtiene el Toggle que ha sido marcado
    /// </summary>
    /// <returns></returns>
    private PanelTicketAuditory GetSelectedToggle()
    {
        var toggles = ticketsContainerLayout.GetComponentsInChildren<Toggle>();
        PanelTicketAuditory ticket = toggles.FirstOrDefault(t => t.isOn).GetComponent<PanelTicketAuditory>();
        return ticket;
    }
    public void OpenAuditoryRoom()
    {
        StartCoroutine(KickOutPlayer());
        isInAuditory = true;
        ACBSingleton.Instance.PanelBuildingSelection.transform.SetAsLastSibling();
        WebProcedure.Instance.GetPlayerSeatInfo((DataSnapshot obj) => {
            try
            {
                MissionAlreadyComplete error = new MissionAlreadyComplete();
                JsonConvert.PopulateObject(obj.RawJson, error);
                if (error.code == 400)
                {
                    ACBSingleton.Instance.AlertPanel.SetupPanel(error.message, "", false, () => { });
                    return;
                }
                else
                {
                    ReturnSeatInfoAuditory seatInfo = new ReturnSeatInfoAuditory();
                    JsonConvert.PopulateObject(obj.RawJson, seatInfo);
                    if (seatInfo.id != 0)
                    {
                        seatID = seatInfo.id;
                    }
                    layoutAuditory.SetActive(true);
                    layoutSits.SetActive(true);
                }
            }
            catch
            {

            }
        }, (WebError obj) => {
            Debug.LogError(obj);
        });

    }
    private void UpdatePassButton()
    {
        useTicketVIPButton.interactable = GetSelectedToggle() != null;
    }
    public void OpenBackSitPanel()
    {
        PanelOpener.popupPrefab = layoutBackSit;
        PanelOpener.OpenPopup();
        ACBSingleton.Instance.PanelBuildingSelection.transform.SetAsLastSibling();
    }
    private void OnApplicationQuit()
    {
        Close();
    }
    public void ExitButtonPressed()
    {
        ACBSingleton.Instance.PanelBuildingSelection.goBackButton.onClick.Invoke();
    }
    public override void Close()
    {
        if (seatID != 0)
        {
            RequestLeaveAuditory request = new RequestLeaveAuditory() { seat_id = seatID, user_id = WebProcedure.Instance.accessData.user };
            WebProcedure.Instance.LeaveProjectionRoom(JsonConvert.SerializeObject(request), (DataSnapshot obj) =>
            {
                try
                {
                    MissionAlreadyComplete error = new MissionAlreadyComplete();
                    JsonConvert.PopulateObject(obj.RawJson, error);
                    if (error.code == 400)
                    {
                        ACBSingleton.Instance.AlertPanel.SetupPanel(error.message, "", false, () => { });
                        return;
                    }
                    else
                    {
                        isInAuditory = false;
                        StopAllCoroutines();
                        base.Close();
                    }
                }
                catch
                {
                    isInAuditory = false;
                    StopAllCoroutines();
                    base.Close();
                }
            }, (WebError obj) =>
            {
                Debug.LogError(obj);
            });
        }
        else
        {
            base.Close();
        }
        
    }
    public void OpenVideoPanel()
    {
        if(PanelOpener != null)
        {
            PanelOpener.popupPrefab = panelVideosPrefab;
            PanelOpener.OpenPopup();
            PanelOpener.popup.GetComponent<PanelVideoRoomAuditory>().Populate(seatID);
        }
    }
    public IEnumerator KickOutPlayer()
    {
        yield return new WaitForSeconds(SECONDS_TO_KICK_OUT_PLAYER);
        if(PanelOpener != null && PanelOpener.popup != null)
        {
            PanelOpener.popup.GetComponent<Panel>().Close();
        }
        ExitButtonPressed();
    }
}
