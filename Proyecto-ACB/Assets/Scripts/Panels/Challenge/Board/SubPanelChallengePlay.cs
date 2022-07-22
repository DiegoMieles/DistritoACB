using Data;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WebAPI;

/// <summary>
/// Subpanel de simulación del desafio
/// </summary>
public class SubPanelChallengePlay : MonoBehaviour
{
    #region Fields and properties

    [Header("Subpanel components")]
    [SerializeField] [Tooltip("Token del jugador principal")]
    private PanelTokenItem playerTokenView;
    [SerializeField] [Tooltip("Texto con el nombre del jugador")]
    private Text playerNameText;
    [SerializeField] [Tooltip("Token del jugador rival")]
    private PanelTokenItem rivalTokenView;
    [SerializeField] [Tooltip("Texto con el nombre del rival")]
    private Text rivalNameText;
    [SerializeField] [Tooltip("Botón de continuar")]
    private Button continueButton;
    [SerializeField] [Tooltip("Vista del avatar del rival")]
    private AvatarImageView avatarRivalView;
    [SerializeField] [Tooltip("Vista del avatar del jugador")]
    private AvatarImageView avatarPlayerView;
    [SerializeField] [Tooltip("Texto del estado de la simulación del desafio")]
    private Text challengeStateText;
    [SerializeField] [Tooltip("Imagen de la flecha")]
    private RectTransform arrowImage;

    [Space(5)]
    [Header("Challenge State Images")]
    [SerializeField] [Tooltip("Imagen que aparece al ganar en una de las partes de la simulación")]
    private Sprite checkImage;
    [SerializeField] [Tooltip("Imagen que aparece al perder en una de las partes de la simulación")]
    private Sprite failImage;
    [SerializeField] [Tooltip("Imagen de tiros dobles (antes triples)")]
    private Image triplesImages;
    [SerializeField] [Tooltip("Imagen de tiros libres")]
    private Image freeshotsImages;
    [SerializeField] [Tooltip("Imagen de puntos")]
    private Image pointsImages;
    [SerializeField] [Tooltip("Imagen de rebotes")]
    private Image reboundsImages;
    [SerializeField] [Tooltip("Imagen de asistencias")]
    private Image assistImages;

    private ChallengeAcceptedData challengeData; //Datos del desafio aceptado

    private List<Image> testImages; //Imagenes de ganar o perder que aparecen al finalizar una de las partes de la simulación
    private List<string> testNames; //Nombres de ganar o perder que aparecen al finalizar una de las partes de la simulación

    #endregion

    #region Public Methods

    /// <summary>
    /// Inicia la simulación de desafio con los datos respectivos
    /// </summary>
    /// <param name="challengeData">Datos del desafio traidos desde backend</param>
    /// <param name="onFinishedSimulation">Acción que se ejecuta una vez finalizada</param>
    public void ShowChallenge(ChallengeAcceptedData challengeData, Action onFinishedSimulation)
    {
        testImages = new List<Image>();
        testNames = new List<string>();

        this.challengeData = challengeData;
        avatarRivalView.UpdateView(challengeData.rival.avatarData, false);
        avatarPlayerView.UpdateView(challengeData.user.avatarData, false);

        playerNameText.text = challengeData.user.avatarData.nickName;
        rivalNameText.text = challengeData.rival.avatarData.nickName;

        continueButton.onClick.AddListener(() => { onFinishedSimulation?.Invoke(); });
        continueButton.gameObject.SetActive(false);

        if (testImages.Count > 0)
            testImages.Clear();
        
        if(testNames.Count > 0)
            testNames.Clear();

        playerTokenView.ShowInfo(challengeData.user.cardData.cardItems);
        rivalTokenView.ShowInfo(challengeData.rival.cardData.cardItems);

        DeactivateImage(triplesImages, challengeData.challengeData.win_st_triples);
        DeactivateImage(freeshotsImages, challengeData.challengeData.win_st_freeshots);
        DeactivateImage(pointsImages, challengeData.challengeData.win_st_points);
        DeactivateImage(reboundsImages, challengeData.challengeData.win_st_rebounds);
        DeactivateImage(assistImages, challengeData.challengeData.win_st_assists);
        StartCoroutine(StartChallengeSimulationPart());
    }

    #endregion

    #region Inner Methods

    /// <summary>
    /// Desactiva la imagen y el texto de la parte de la simulación recién finalizada
    /// </summary>
    /// <param name="imageToDeactivate">Imagen a desactivar</param>
    /// <param name="imageTestName">Texto a ocultar</param>
    private void DeactivateImage(Image imageToDeactivate, string imageTestName)
    {
        imageToDeactivate.gameObject.SetActive(false);
        testImages.Add(imageToDeactivate);
        testNames.Add(imageTestName);
    }

    /// <summary>
    /// Corrutina que inicia la simulación del desafio
    /// </summary>
    /// <returns></returns>
    private IEnumerator StartChallengeSimulationPart()
    {
        RectTransform arrowTransform = arrowImage.GetComponent<RectTransform>();
        
        for(int i = 0; i < testImages.Count; i++)
        {
            bool canContinue = false;
            challengeStateText.text = "";
            yield return new WaitForSeconds(1f);
            challengeStateText.text = "Calculando";

            if(i > 0)
                arrowImage.DOAnchorPos(new Vector2(arrowTransform.anchoredPosition.x + 67f, arrowTransform.anchoredPosition.y), 2f).OnComplete(() => { canContinue = true; });
            else
            {
                yield return new WaitForSeconds(2f);
                canContinue = true;
            }

            yield return new WaitUntil(() => canContinue);
            bool userWon = testNames[i] == WebProcedure.Instance.accessData.user;
            challengeStateText.text = "";
            testImages[i].gameObject.SetActive(true);
            testImages[i].sprite = userWon ? checkImage : failImage;
            yield return new WaitForSeconds(1f);
        }
        yield return new WaitForSeconds(1f);
        continueButton.gameObject.SetActive(true);
        challengeStateText.text = challengeData.challengeData.winner == WebProcedure.Instance.accessData.user ? "Ganaste" : "Perdiste";
    }

    #endregion
}
