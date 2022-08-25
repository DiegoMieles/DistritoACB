using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using WebAPI;
using Data;
using Newtonsoft.Json;
using System.Linq;
using System;

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
        videoPlayer.prepareCompleted += (VideoPlayer source) => { StartCoroutine(closeSpinnerTimer(0.5f)); };
    }
    public void ShowVideos()
    {
        SetSpinnerState(true);
        WebProcedure.Instance.GetBillboardAuditory(OnSucces, (WebError error) => { Debug.LogError(error); SetSpinnerState(false); });
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
        SetSpinnerState(false);
    }
    /// <summary>
    /// reproduce el video encontrado en el url
    /// </summary>
    public void OpenVideoURL(BillBoardReturn.BillboardData data)
    {
        videoPlayer.url = data.media_path;
        if(!string.IsNullOrEmpty( videoPlayer.url))
        {
            SetSpinnerState(true);
            videoViewerPanel.SetActive(true);
            videoPlayer.Play();
        }
    }
    /// <summary>
    /// cierra y pausa el reproductor de video
    /// </summary>
    public void CloseVideoPlayer()
    {
        videoPlayer.Pause();
        videoViewerPanel.SetActive(false);
        SetSpinnerState(false);
    }
    
    private IEnumerator closeSpinnerTimer(float time)
    {
    yield return new WaitForSeconds(time);
        SetSpinnerState(false);
    }
    /// <summary>
    /// Activa o desactiva el spinner de carga
    /// </summary>
    /// <param name="state">Estado de activación del spinner</param>
    private void SetSpinnerState(bool state)
    {
        GameObject spinner = GameObject.Find("Spinner_Video");
        for (int i = 0; i < spinner.transform.childCount; i++)
        {
            spinner.transform.GetChild(i).gameObject.SetActive(state);
        }
    }
    
}
