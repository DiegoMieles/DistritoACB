using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WebAPI;
using Data;
using Newtonsoft.Json;

public class Panel_ItemCartelera : MonoBehaviour
{
    [Tooltip("Contiene la informaci�n del video")]
    public BillBoardReturn.BillboardData data;
    [SerializeField]
    [Tooltip("Imagen del video")]
    private Image thumbnailImage;
    [SerializeField]
    [Tooltip("Imagen que muestra que el video es un streaming")]
    private GameObject StreamingFlagImage;
    [SerializeField]
    [Tooltip("Imagen del icono de play")]
    private GameObject playIconImage;
    [SerializeField]
    [Tooltip("Nombre del video")]
    private Text videoTitle;
    [SerializeField]
    [Tooltip("Descripci�n del video")]
    private Text VideoDescription;
    [SerializeField]
    [Tooltip("fecha de publicaci�n del video")]
    private Text videoDate;
    [SerializeField]
    [Tooltip("bot�n de reproducci�n del video")]
    private Button VideoButton;
    /// <summary>
    /// Setea la informaci�n del video en el panel
    /// </summary>
    public void SetupVideoPanel(BillBoardReturn.BillboardData videoData, UnityEngine.Events.UnityAction onPlayButtonPressed = null )
    {
        if (videoData == null) return;
            data = videoData;
        playIconImage.SetActive(false);
        if (onPlayButtonPressed != null)
        {
            VideoButton.onClick.AddListener(onPlayButtonPressed);
            playIconImage.SetActive(true);
        }
        WebProcedure.Instance.GetSprite(videoData.thumbnail, (obj) => { thumbnailImage.sprite = obj; }, (failed) => { Debug.Log("Failed loading back image"); });
        StreamingFlagImage.SetActive(data.type != "video");
        videoTitle.text = data.title;
        VideoDescription.text = data.description;
        videoDate.text = data.created;
    }
}
