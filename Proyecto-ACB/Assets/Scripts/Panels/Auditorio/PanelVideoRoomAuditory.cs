using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using WebAPI;
using Data;
using Newtonsoft.Json;
using System.Linq;

public class PanelVideoRoomAuditory : Panel
{
    [SerializeField]
    [Tooltip("Prefab del panel del video")]
    private GameObject videoPanelPrefab;
    [SerializeField]
    [Tooltip("panel del reproductor de video")]
    private GameObject videoViewerPanel;
    [SerializeField]
    [Tooltip("Layout que sostiene los videos")]
    private RectTransform videosLayout;
    [SerializeField]
    [Tooltip("Reproductor de videos")]
    private VideoPlayer videoPlayer;
    /// <summary>
    /// id del asiento ocupado por el jugado
    /// </summary>
    private int seatID;
    // Start is called before the first frame update
    void Start()
    {
        ShowVideos();
    }
    public void Populate(int seat_id)
    {
        seatID = seat_id;
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
        foreach (BillBoardReturn.BillboardData data in boardReturn.data)
        {
            Panel_ItemCartelera panel = Instantiate(videoPanelPrefab, videosLayout.transform).GetComponent<Panel_ItemCartelera>();
            if (panel != null) panel.SetupVideoPanel(data,()=> { OpenVideoURL(data); });
            videosLayout.sizeDelta = new Vector2(videosLayout.sizeDelta.x, (videosLayout.sizeDelta.y + panel.GetComponent<LayoutElement>().preferredHeight + videosLayout.GetComponent<VerticalLayoutGroup>().spacing + videosLayout.GetComponent<VerticalLayoutGroup>().padding.top));
        }
    }
    public void OpenVideoURL(BillBoardReturn.BillboardData data)
    {
        WebProcedure.Instance.GetVideoDetails(seatID, data.id,(DataSnapshot obj)=> 
            {
                Debug.Log(obj.RawJson);
                videoViewerPanel.SetActive(true);
                BillBoardReturn.BillboardData boardReturn = new BillBoardReturn.BillboardData();
                JsonConvert.PopulateObject(obj.RawJson, boardReturn);
                videoPlayer.url = boardReturn.media_path;
                if(!string.IsNullOrEmpty( videoPlayer.url))
                videoPlayer.Play();
            },
             (WebError error) => { Debug.LogError(error); }
            );
    }
    public void CloseVideoPlayer()
    {
        videoPlayer.Pause();
        videoViewerPanel.SetActive(false);
    }
}
