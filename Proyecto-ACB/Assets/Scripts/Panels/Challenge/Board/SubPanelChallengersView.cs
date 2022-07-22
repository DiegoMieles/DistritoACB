using Data;
using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controla el subpanel donde se muestran los jugadores que van a competir y sus tokens
/// </summary>
public class SubPanelChallengersView : MonoBehaviour
{
    #region Fields and properties

    [Header("Subpanel components")]
    [SerializeField] [Tooltip("Botón de vista del rival")]
    private Button rivalButtonView;
    [SerializeField] [Tooltip("Botón de vista del jugador")]
    private Button userButtonView;
    [SerializeField] [Tooltip("Vista del token del jugador")]
    private PanelTokenItem playerTokenView;
    [SerializeField] [Tooltip("Texto con el nombre del jugador")]
    private Text playerNameText;
    [SerializeField] [Tooltip("Vista del token del rival")]
    private PanelTokenItem rivalTokenView;
    [SerializeField] [Tooltip("Texto con el nombre del rival")]
    private Text rivalNameText;
    [SerializeField] [Tooltip("Botón de iniciar desafio")]
    private Button continueButton;
    [SerializeField] [Tooltip("Vista del avatar del rival")]
    private AvatarImageView avatarRivalView;
    [SerializeField] [Tooltip("Vista del avatar del jugador")]
    private AvatarImageView avatarPlayerView;

    [Space(5)]
    [Header("Panels to open")]
    [SerializeField] [Tooltip("Clase que controla la apertura de nuevos paneles a mostrar")]
    private PanelOpener panelOpener;
    [SerializeField] [Tooltip("Prefab del panel de avatar del rival")]
    private GameObject rivalViewPanelPrefab;

    private ChallengeAcceptedData challengeData; //Datos del desafio aceptado

    #endregion

    #region Public Methods

    /// <summary>
    /// Muestra la vista de los jugadores y sus tokens para iniciar la simulación
    /// </summary>
    /// <param name="challengeData">Datos del desafio a iniciar</param>
    /// <param name="onContinueChallenge">Acción que se ejecuta al iniciar la simulación del desafio</param>
    public void ShowChallengeView(ChallengeAcceptedData challengeData, Action onContinueChallenge)
    {
        this.challengeData = challengeData;
        avatarRivalView.UpdateView(challengeData.rival.avatarData, false);
        avatarPlayerView.UpdateView(challengeData.user.avatarData, false);
        continueButton.onClick.AddListener(() => { onContinueChallenge?.Invoke(); });
        rivalButtonView.onClick.AddListener(() => { OpenCustomPanel(true); });
        userButtonView.onClick.AddListener(() => { OpenCustomPanel(false); });
        playerTokenView.ShowInfo(challengeData.user.cardData.cardItems);
        rivalTokenView.ShowInfo(challengeData.rival.cardData.cardItems);

        playerNameText.text = challengeData.user.avatarData.nickName;
        rivalNameText.text = challengeData.rival.avatarData.nickName;
    }

    #endregion

    #region Inner Methods

    /// <summary>
    /// Abre el panel de vista de avatar de alguno de los jugadores
    /// </summary>
    /// <param name="isRival">Determina si el avatar a cargar es el del rival</param>
    private void OpenCustomPanel(bool isRival)
    {
        panelOpener.popupPrefab = rivalViewPanelPrefab;
        panelOpener.OpenPopup();
        panelOpener.popup.GetComponent<PanelTablonAvatar>().SetAvatarData(challengeData, isRival);
    }

    #endregion

}
