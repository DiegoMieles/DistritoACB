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
    [Tooltip("layout de visualización del video")]
    private GameObject videoPlayerLayout;
    [SerializeField]
    [Tooltip("Prefab del panel del video")]
    private GameObject videoPanelPrefab;
    [SerializeField]
    [Tooltip("Layout que sostiene los videos")]
    private RectTransform videosLayout;
    [SerializeField]
    [Tooltip("prefab de los pases VIP")]
    private GameObject ticketsPrefabPanel;
    [SerializeField]
    [Tooltip("contenedor de los pases VIP")]
    private Transform ticketsContainerLayout;
    [SerializeField]
    [Tooltip("contenedor de los pases VIP")]
    private Button useTicketVIPButton;

    private void OnEnable()
    {
        OpenVIPTicketsPanel();
    }
    public void ShowVideos()
    {
        WebProcedure.Instance.GetBillboardAuditory(OnSucces, (WebError error) => { Debug.LogError(error); });
    }
    private void OnSucces(DataSnapshot obj)
    {
        BillBoardReturn boardReturn = new BillBoardReturn();
        Debug.Log(obj.RawJson);
        JsonConvert.PopulateObject(obj.RawJson, boardReturn);
        videosLayout.sizeDelta = new Vector2(videosLayout.sizeDelta.x, (videoPanelPrefab.GetComponent<RectTransform>().rect.height + videosLayout.GetComponent<VerticalLayoutGroup>().spacing + videosLayout.GetComponent<VerticalLayoutGroup>().padding.top) * boardReturn.data.Length);
        foreach (BillBoardReturn.BillboardData data in boardReturn.data)
        {
            Panel_ItemCartelera panel = Instantiate(videoPanelPrefab, videosLayout.transform).GetComponent<Panel_ItemCartelera>();
            if (panel != null) panel.SetupVideoPanel(data);
        }
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
        layoutSits.SetActive(true);
        ACBSingleton.Instance.PanelBuildingSelection.transform.SetAsLastSibling();
    }
    private void UpdatePassButton()
    {
        useTicketVIPButton.interactable = GetSelectedToggle() != null;
    }
}
