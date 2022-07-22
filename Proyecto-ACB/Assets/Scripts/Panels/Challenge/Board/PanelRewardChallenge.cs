using System.Collections.Generic;
using Data;
using WebAPI;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// Controla el panel de recompensa que se muestra al finalizar un desafio
/// </summary>
public class PanelRewardChallenge : Panel
{
    #region Fields and Properties

    [Header("Panel components")]
    [SerializeField] [Tooltip("Botón que se encarga del cerrado del panel")]
    private Button closeButton;
    [SerializeField] [Tooltip("Texto del título del panel")]
    private Text titleText;
    [SerializeField] [Tooltip("Texto con el número de puntos ganados o perdidos por el jugador después de un desafio")]
    private Text pointsText;
    [SerializeField] [Tooltip("Botón de regresar/ver ACBalls")]
    private Button returnButton;
    [SerializeField] [Tooltip("Texto del botón de regresar")]
    private Text returnButtonText;
    [SerializeField] [Tooltip("Imagen de la ACBall")]
    private Image acballImage;
    [SerializeField] [Tooltip("Punto donde se oculta la ACBall o punto de inicio de transición de la ACBall")]
    private RectTransform acballStartPosition;
    [SerializeField] [Tooltip("Punto final de la animación de transición de la ACBall")]
    private RectTransform acballEndPosition;
    [SerializeField] [Tooltip("Lista de objetos que se deben desactivar, sobretodo usado cuando se muestra la simulación de un desafio previamente aceptado")]
    private List<GameObject> objectsToDeactivate;

    [Space(5)]
    [Header("Panel to open components")]
    [SerializeField] [Tooltip("Clase que controla la apertura de nuevos paneles a mostrar")]
    private PanelOpener panelOpener;
    [SerializeField] [Tooltip("Prefab del panel donde se muestran las ACBall que tiene el jugador")]
    private GameObject acballPanelPrefab;

    #endregion

    #region Public Methods

    /// <summary>
    /// Configura los datos del premio ganado y muestra la transición de animación del mismo
    /// </summary>
    /// <param name="challengeData">Datos del desafio que se aceptó donde se encuentra los datos de la recompensa</param>
    /// <param name="showAcballPanel">Determina si se muestra el panel de la ACBall al seleccionar el botón de regresar</param>
    public void ShowReward(ChallengeAcceptedData challengeData, bool showAcballPanel)
    {
        objectsToDeactivate.ForEach(obj => obj.SetActive(false));
        closeButton.onClick.AddListener(Close);

        bool playerIsTheWinner = challengeData.challengeData.winner == WebProcedure.Instance.accessData.user && showAcballPanel;

        titleText.text = challengeData.challengeData.winner == WebProcedure.Instance.accessData.user ? "Has ganado: " : "Has perdido: ";
        returnButtonText.text = playerIsTheWinner && challengeData.challengeData.acballData.acballItem != null ? "Ir a acballs" : "Volver";
        pointsText.text = challengeData.challengeData.winner == WebProcedure.Instance.accessData.user ? challengeData.challengeData.win_points.ToString() + " pts" : challengeData.challengeData.lost_points.ToString() + " pts";

        if(playerIsTheWinner && challengeData.challengeData.acballData.acballItem != null)
            returnButton.onClick.AddListener(ShowACBallPanel);
        else
            returnButton.onClick.AddListener(Close);

        if (challengeData.challengeData.winner == WebProcedure.Instance.accessData.user)
        {
            if (challengeData.challengeData.acballData.acballItem != null)
            {
                WebProcedure.Instance.GetSprite(challengeData.challengeData.acballData.acballItem.path_img, OnLoadACBallIamge, (error) => { });  
            }
            else
            {
                objectsToDeactivate.ForEach(obj => obj.SetActive(true));
            }
        }
        else
            objectsToDeactivate.ForEach(obj => obj.SetActive(true));

    }

    #endregion

    #region Inner Methods

    /// <summary>
    /// Cierra el panel actual y muestra el panel de ACBalls del jugador
    /// </summary>
    private void ShowACBallPanel()
    {
        panelOpener.popupPrefab = acballPanelPrefab;
        panelOpener.OpenPopup();
        panelOpener.popup.GetComponent<PanelACBall>().LoadACBallsData();
        Close();
    }

    /// <summary>
    /// Método que se llama cuando la imagen de la ACBall ha sido cargada correctamente desde backend
    /// </summary>
    /// <param name="obj">Imagen devuelta por backend</param>
    private void OnLoadACBallIamge(Sprite obj)
    {
        acballImage.sprite = obj;
        acballImage.GetComponent<RectTransform>().anchoredPosition = acballStartPosition.anchoredPosition;
        acballImage.GetComponent<RectTransform>().DOAnchorPos(acballEndPosition.anchoredPosition, 1.5f).OnComplete(() => { objectsToDeactivate.ForEach(obj => obj.SetActive(true)); });
        Firebase.Analytics.FirebaseAnalytics.LogEvent("win_acball");
        Debug.Log("Analytic win_acball logged");
    }

    #endregion
}
