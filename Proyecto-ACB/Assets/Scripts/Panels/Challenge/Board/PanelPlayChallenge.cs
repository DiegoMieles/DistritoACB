using Data;
using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controlador del panel principal de simulación del desafio
/// </summary>
public class PanelPlayChallenge : Panel
{
    #region Fields and Properties

    [Header("Subpanels")]
    [SerializeField] [Tooltip("Subpanel con la animación de desafio iniciado")]
    private SubPanelStartChallenge subpanelStartChallenge;
    [SerializeField] [Tooltip("Subpanel con la vista de los jugadores que se van a enfrentar en la simulación")]
    private SubPanelChallengersView subpanelChallengersView;
    [SerializeField] [Tooltip("Subpanel con la vista de la simulación")]
    private SubPanelChallengePlay subpanelChallengePlay;

    [Space(5)]
    [Header("Finish panel components")]
    [SerializeField] [Tooltip("Clase que controla la apertura de nuevos paneles a mostrar")]
    private PanelOpener panelOpener;
    [SerializeField] [Tooltip("Prefab del panel de desafio finalizado")]
    private GameObject endChallengePrefab;

    [Space(5)]
    [Header("Other components")]
    [SerializeField] [Tooltip("Botón que se encarga del cerrado del panel")]
    private Button closeButton;

    private ChallengeAcceptedData challengeData; //Datos del desafio aceptado
    private Action onFinishedChallenge; //Acción que se ejecuta al finalizar el desafio
    private bool showAcballPanel; //Determina si se debe mostrar el panel de ACBalls una vez recibida la recompensa del desafio
    private bool canClose; //Determina si la simulación del desafio puede ser cerrada (Aplica para desafios hechos con anterioridad)

    #endregion

    #region Public Methods

    /// <summary>
    /// Inicia el desafio mostrando el subpanel de inicio de desafio
    /// </summary>
    /// <param name="challengeData">Datos del desafio traidos desde backend</param>
    /// <param name="onFinishedChallenge">Acción que se ejecuta cuando un desafio ha sido finalizado</param>
    /// <param name="showAcballPanel">Determina si se debe mostrar el panel de ACBall después de aceptar la recompensa del desafio</param>
    /// <param name="canClose">Determina si el jugador puede salir del desafio simulado (Sólo cuando el desafio ya se ha hecho previamente)</param>
    public void StartChallenge(ChallengeAcceptedData challengeData, Action onFinishedChallenge = null, bool showAcballPanel = false, bool canClose = false)
    {
        this.onFinishedChallenge = onFinishedChallenge;
        this.showAcballPanel = showAcballPanel;
        this.canClose = canClose;
        ShowSubpanels(true, false, false);
        this.challengeData = challengeData;
        subpanelStartChallenge.AnimateStartChallenge(ShowChallengersView);

        closeButton.onClick.AddListener(Close);
        closeButton.gameObject.SetActive(false);
    }

    #endregion

    #region Inner Methods

    /// <summary>
    /// Muestra un subpanel específico
    /// </summary>
    /// <param name="showStartView">Determina si se debe mostrar el subpanel de inicio del desafio</param>
    /// <param name="showChallengers">Determina si se debe mostrar el subpanel donde se ven los jugadores del desafio</param>
    /// <param name="showChallengePlay">Determina si se debe mostrar el subpanel de simulación del desafio</param>
    private void ShowSubpanels(bool showStartView, bool showChallengers, bool showChallengePlay)
    {
        subpanelStartChallenge.gameObject.SetActive(showStartView);
        subpanelChallengersView.gameObject.SetActive(showChallengers);
        subpanelChallengePlay.gameObject.SetActive(showChallengePlay);
    }

    /// <summary>
    /// Muestra el subpanel de los jugadores que están desafiandose
    /// </summary>
    private void ShowChallengersView()
    {
        ShowSubpanels(false, true, false);

        if(canClose)
            closeButton.gameObject.SetActive(true);

        subpanelChallengersView.ShowChallengeView(challengeData, StartChallengePlay);
    }

    /// <summary>
    /// Inicia el subpanel donde sucede la simulación del desafio
    /// </summary>
    private void StartChallengePlay()
    {
        ShowSubpanels(false, false, true);
        closeButton.gameObject.SetActive(false);
        subpanelChallengePlay.ShowChallenge(challengeData, OpenEndSimulationPanel);
    }

    /// <summary>
    /// Termina la simulación y abre el panel de finalización del desafio
    /// </summary>
    private void OpenEndSimulationPanel()
    {
        panelOpener.popupPrefab = endChallengePrefab;
        panelOpener.OpenPopup();
        onFinishedChallenge?.Invoke();
        panelOpener.popup.GetComponent<PanelEndGameState>().ShowPlayerInjuryState(challengeData, showAcballPanel);
        Close();
    }

    #endregion
}
