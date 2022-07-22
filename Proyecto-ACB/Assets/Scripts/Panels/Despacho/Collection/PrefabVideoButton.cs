using Data;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using WebAPI;

/// <summary>
/// Controla el prefab del botón de video
/// </summary>
public class PrefabVideoButton : MonoBehaviour
{
    #region Fields and properties

    [Header("Prefab components")]
    [SerializeField] [Tooltip("Prefab del panel de video")]
    private Button openVideoViewPanel;
    [SerializeField] [Tooltip("Texto de descripción del video")]
    private Text videoDescription;
    [SerializeField] [Tooltip("Miniatura del video")]
    private Image imageVideo;
    [Space(5)]
    [Header("Panel to open references")]
    [SerializeField] [Tooltip("Clase que controla la apertura de nuevos paneles a mostrar")]
    private PanelOpener panelOpener;
    [SerializeField] [Tooltip("Prefab del panel del video")]
    private GameObject videoPanelPrefab;

    private CardVideoData videoData; //Datos del video

    #endregion

    #region Public Methods

    /// <summary>
    /// Configura los datos de video
    /// </summary>
    /// <param name="videoData">Datos de video</param>
    public void SetupVideo(CardVideoData videoData)
    {
        this.videoData = videoData;
        videoDescription.text = videoData.title;
        openVideoViewPanel.onClick.AddListener(OpenVideo);
        if (!string.IsNullOrEmpty(videoData.fotograma))
        {
            WebProcedure.Instance.GetSprite(videoData.fotograma, sprite =>
            {
                imageVideo.sprite = sprite;
            }, error =>
            {
               Debug.LogError("FailVideo");
            });
        }

        StartCoroutine(spinner());
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Abre panel del video y lo reproduce
    /// </summary>
    private void OpenVideo()
    {
        panelOpener.popupPrefab = videoPanelPrefab;
        panelOpener.OpenPopup();
        panelOpener.popup.GetComponent<PanelVideo>().PlayVideo(videoData.url_video);
    }

    /// <summary>
    /// Corrutina que desactiva el spinner de carga
    /// </summary>
    /// <returns></returns>
    IEnumerator spinner()
    {
        yield return new WaitForSeconds(2.0f);
        GameObject spinner = GameObject.Find("Spinner_ListVideo");
        for(int i=0; i<spinner.transform.childCount; i++)
        {
            spinner.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    #endregion
}
