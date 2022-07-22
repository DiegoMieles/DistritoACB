using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controlador del panel que se muestra inmediatamente después de terminada una simulación de desafio
/// </summary>
public class PanelEndGameState : Panel
{
    #region Fields and properties

    [Header("Panel components")]
    [SerializeField][Tooltip("Botón que se encarga del cerrado del panel")]
    private Button closeButton;
    [SerializeField] [Tooltip("Carta del jugador usada en el desafio en forma de token")]
    private PanelTokenItem playerToken;
    [SerializeField] [Tooltip("Título de si el jugador ha sido o no lesionado")]
    private Text injuryTitle;
    [SerializeField] [Tooltip("Texto con el nivel de lesión del jugador")]
    private Text lesionLevel;
    [SerializeField] [Tooltip("Texto con el estado de lesión del jugador")]
    private Text playerInjuryState;
    [SerializeField] [Tooltip("Botón de continuar")]
    private Button continueButton;
    [SerializeField] [Tooltip("Listado de objetos a activar y desactivar del panel")]
    private List<GameObject> objectsToActivate;

    [Space(5)]
    [Header("Panel to open")]
    [SerializeField] [Tooltip("Clase que controla la apertura de nuevos paneles a mostrar")]
    private PanelOpener panelOpener;
    [SerializeField] [Tooltip("Prefab del panel de recompensa obtenida al final del desafio")]
    private GameObject rewardChallengePanelPrefab;

    [Space(5)]
    [Header("Title text strings")]
    [SerializeField] [Tooltip("Texto que se muestra cuando el jugador se lesionó después del desafio")]
    private string titleInjuredString;
    [SerializeField] [Tooltip("Texto que se muestra cuando el jugador no se lesionó después del desafio")]
    private string titleNotInjuredString;

    private ChallengeAcceptedData challengeData; //Datos del desafio terminado
    private bool showAcballPanel; //Determina si al recibir la recompensa se debe abrir el panel de ACBalls

    #endregion

    #region Public methods

    /// <summary>
    /// Muestra el nivel de lesión del jugador
    /// </summary>
    /// <param name="challengeData">Datos del desafio terminado</param>
    /// <param name="showAcballPanel">Determina si al recibir la recompensa se debe abrir el panel de ACBalls</param>
    public void ShowPlayerInjuryState(ChallengeAcceptedData challengeData, bool showAcballPanel)
    {
        this.challengeData = challengeData;
        this.showAcballPanel = showAcballPanel;
        closeButton.onClick.AddListener(Close);
        closeButton.gameObject.SetActive(false);
        playerToken.ShowInfo(challengeData.user.cardData.cardItems);

        injuryTitle.text = challengeData.user.cardData.cardItems.isInjured ? titleInjuredString : titleNotInjuredString;
        lesionLevel.text = challengeData.user.cardData.cardItems.textInjured;
        playerInjuryState.text = challengeData.user.cardData.cardItems.daysOrTextInjured;
        
        continueButton.onClick.AddListener(OpenRewardPanel);
        StartCoroutine(ShowHiddenObjects());
    }

    #endregion

    #region Inner Methods

    /// <summary>
    /// Abre panel de recompensa del desafio
    /// </summary>
    private void OpenRewardPanel()
    {
        panelOpener.popupPrefab = rewardChallengePanelPrefab;
        panelOpener.OpenPopup();
        panelOpener.popup.GetComponent<PanelRewardChallenge>().ShowReward(challengeData, showAcballPanel);
        Close();
    }

    /// <summary>
    /// Oculta y muestra el listado de objetos que se busca mostrar
    /// </summary>
    /// <returns></returns>
    private IEnumerator ShowHiddenObjects()
    {
        objectsToActivate.ForEach(obj => obj.SetActive(false));
        yield return new WaitForSeconds(1.5f);
        objectsToActivate.ForEach(obj => obj.SetActive(true));
    }

    #endregion

}
