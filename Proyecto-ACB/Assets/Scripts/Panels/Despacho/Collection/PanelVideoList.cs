using System.Collections.Generic;
using Data;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controla el panel donde se muestran los videos de una carta
/// </summary>
public class PanelVideoList : Panel
{
    #region Fields and Properties

    [Header("Panel components")]
    [SerializeField] [Tooltip("Prefab del botón que activa los videos")]
    private GameObject buttonVideoPrefab;
    [SerializeField] [Tooltip("Contenedor de lo botones de video")]
    private Transform buttonsContainer;
    [SerializeField] [Tooltip("Botón que se encarga del cerrado del panel")]
    private Button goBackButton;

    #endregion

    #region Public Methods

    /// <summary>
    /// Configura la lista de videos
    /// </summary>
    /// <param name="videosData">Datos de los videos de la carta</param>
    public void SetupVideos(List<CardVideoData> videosData)
    {
        goBackButton.onClick.AddListener(Close);

        if (videosData == null || videosData.Count < 0)
        {
            ACBSingleton.Instance.AlertPanel.SetupPanel("No hay videos disponibles", "", false, Close);
            return;
        }

        for(int i = 0; i < videosData.Count; i++)
        {
            GameObject prefab = Instantiate(buttonVideoPrefab, buttonsContainer);
            prefab.GetComponent<PrefabVideoButton>().SetupVideo(videosData[i]);
        }
    }

    #endregion

}
