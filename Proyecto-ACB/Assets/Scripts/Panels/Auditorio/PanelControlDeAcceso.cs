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
    [SerializeField]
    [Tooltip("asientos dentro del auditorio")]
    private List<Transform> sits;
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
        ticketsContainerLayout.GetComponent<RectTransform>().sizeDelta = new Vector2(ticketsContainerLayout.GetComponent<RectTransform>().sizeDelta.x, 0);
       ACBSingleton.Instance.PanelBuildingSelection.transform.SetSiblingIndex(ACBSingleton.Instance.PanelBuildingSelection.transform.parent.childCount - 2);
        SetSpinnerState(true);
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
                    foreach(Transform child in ticketsContainerLayout)
                    {
                        Destroy(child.gameObject);
                    }
                    foreach (VIPPassesReturn.VIPPass ticket in passesReturn.data)
                    {
                        PanelTicketAuditory panelTicket = Instantiate(ticketsPrefabPanel, ticketsContainerLayout).GetComponent<PanelTicketAuditory>();
                        if (panelTicket != null) ;
                        panelTicket.SetupTicketPanel(ticket,true,GetComponent<ToggleGroup>(), UpdatePassButton);
                        ticketsContainerLayout.GetComponent<RectTransform>().sizeDelta = new Vector2(ticketsContainerLayout.GetComponent<RectTransform>().sizeDelta.x, (ticketsContainerLayout.GetComponent<RectTransform>().sizeDelta.y + panelTicket.GetComponent<LayoutElement>().preferredHeight + ticketsContainerLayout.GetComponent<VerticalLayoutGroup>().spacing + ticketsContainerLayout.GetComponent<VerticalLayoutGroup>().padding.top));
                    }
                    ACBSingleton.Instance.PanelBuildingSelection.innerBuildingName.text = ACBSingleton.Instance.PanelBuildingSelection.cachedBuildingsStack.Peek().previousBuildingData.infoTitle;
                    ACBSingleton.Instance.PanelBuildingSelection.innerBuildingIcon.sprite = ACBSingleton.Instance.PanelBuildingSelection.cachedBuildingsStack.Peek().previousBuildingData.buildingIcon;
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
    /// Selecciona el toggle marcado , y con su id se intenta usar el pase
    /// </summary>
    public void UseVIPPass()
    {
        SetSpinnerState(true);
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
                SetSpinnerState(false);
            }
            catch
            {
                OpenAuditoryRoom();
                SetSpinnerState(false);
            }
        }, (WebError obj) => {
            Debug.LogError(obj);
            SetSpinnerState(false);
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
        SetSpinnerState(false);
        StartCoroutine(KickOutPlayer());
        isInAuditory = true;
        ACBSingleton.Instance.PanelBuildingSelection.transform.SetAsLastSibling();
        SetSpinnerState(true);
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
                    foreach (Transform sit in sits)
                    {
                        sit.gameObject.SetActive(true);
                    }
                    int rnd = Random.Range(1, sits.Count - 1);
                    for (int i = 0; i < rnd; i++)
                    {
                        List<Transform> availableSits = sits.FindAll(x => x.gameObject.activeInHierarchy);
                        int rndNum = Random.Range(0, availableSits.Count);
                        availableSits[rndNum].gameObject.SetActive(false);
                    }
                    SetSpinnerState(false);
                }
            }
            catch
            {
                SetSpinnerState(false);
            }
        }, (WebError obj) => {
            Debug.LogError(obj);
            SetSpinnerState(false);
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
}
