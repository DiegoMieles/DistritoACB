using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WebAPI;
using Data;
using Newtonsoft.Json;

public class PanelControlDeAcceso : Panel
{
    [SerializeField]
    [Tooltip("layout donde est�n los asientos del auditorio")]
    private GameObject layoutSits;
    [SerializeField]
    [Tooltip("layout del asiento en el auditorio")]
    private GameObject layoutBackSit;
    [SerializeField]
    [Tooltip("layout de visualizaci�n del video")]
    private GameObject videoPlayerLayout;
    [SerializeField]
    [Tooltip("Prefab del panel del video")]
    private GameObject videoPanelPrefab;
    [SerializeField]
    [Tooltip("Layout que sostiene los videos")]
    private RectTransform videosLayout;

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
}
