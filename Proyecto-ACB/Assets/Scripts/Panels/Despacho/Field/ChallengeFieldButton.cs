using Data;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using WebAPI;

/// <summary>
/// Controla el bot�n de desafio en la cancha
/// </summary>
public class ChallengeFieldButton : MonoBehaviour
{
    #region Fields and properties

    [Header("Main components")]
    [SerializeField] [Tooltip("B�ton de entrar a la simulaci�n del desafio")]
    private Button enterChallengeButton;
    [SerializeField] [Tooltip("Texto con el nombre del jugador")]
    private Text playerName;
    [SerializeField] [Tooltip("Texto con la fecha de desafio")]
    private Text challengeDate;
    [SerializeField] [Tooltip("Imagen de desafio ganado")]
    private Image winStateImage;
    [SerializeField] [Tooltip("Texto con el estado de ganacia del desafio")]
    private Text winState;
    [SerializeField] [Tooltip("Imagen del token del jugador")]
    private Image playerToken;
    [SerializeField] [Tooltip("Vista del avatar")]
    private AvatarImageView playerView;

    [Space(5)]
    [Header("Panel to open components")]
    [SerializeField] [Tooltip("Clase que controla la apertura de nuevos paneles a mostrar")]
    private PanelOpener panelOpener;
    [SerializeField] [Tooltip("Prefab del panel de simulaci�n del desafio")]
    private GameObject panelPrefab;

    [Space(5)]
    [Header("Win State images")]
    [SerializeField] [Tooltip("Imagen del check")]
    private Sprite checkImage;
    [SerializeField] [Tooltip("Imagen de la cruz")]
    private Sprite loseImage;

    private ChallengesField.ChallengeFieldData.ChallengesFieldItem challengeFieldData; //Datos del desafio

    #endregion

    #region Public Methods

    /// <summary>
    /// Configura el bot�n con los datos del desafio
    /// </summary>
    /// <param name="challengeFieldData">Datos de desafio</param>
    public void SetupChallengeButton(ChallengesField.ChallengeFieldData.ChallengesFieldItem challengeFieldData)
    {
        this.challengeFieldData = challengeFieldData;
        playerView.UpdateView(challengeFieldData);
        playerName.text = challengeFieldData.nickName;
        challengeDate.text = challengeFieldData.jugado.ToString();

        bool playerHasWon = challengeFieldData.win_status == "GANASTE" || challengeFieldData.win_status == "Ganaste";

        winStateImage.sprite = playerHasWon ? checkImage : loseImage;
        winState.text = playerHasWon ? "GANASTE" : "PERDISTE";
        WebProcedure.Instance.GetSprite(challengeFieldData.pathThumbnail, (obj) => { playerToken.sprite = obj; }, (error) => { });
        enterChallengeButton.onClick.AddListener(LoadBackendData);
        
       SetActiveSpinner(false);
    }

    #endregion

    #region Inner Methods

    /// <summary>
    /// Carga los datos del desafio desde backend
    /// </summary>
    private void LoadBackendData()
    {
        SetActiveSpinner(true);
        ChallengeAccept challenge = new ChallengeAccept() { challenge_id = challengeFieldData.challenge_id };
        WebProcedure.Instance.GetChallengeCanchaById(JsonConvert.SerializeObject(challenge), OpenPrefabPanel, OnFailedGettingChallenge);
    }

    /// <summary>
    /// Abre panel de simulaci�n del desafio
    /// </summary>
    /// <param name="obj">Datos del desafio</param>
    private void OpenPrefabPanel(DataSnapshot obj)
    {
        Debug.Log(obj.RawJson);
        ChallengeAcceptedData challengeAceptedData = new ChallengeAcceptedData();
        JsonConvert.PopulateObject(obj.RawJson, challengeAceptedData);
        panelOpener.popupPrefab = panelPrefab;
        SetActiveSpinner(false);
        panelOpener.OpenPopup();
        panelOpener.popup.GetComponent<PanelPlayChallenge>().StartChallenge(challengeAceptedData, null, false, true);
    }

    /// <summary>
    /// Este m�todo se activa cuando no se ha podido traer los datos de desafio desde backend
    /// </summary>
    /// <param name="obj">Clase con los datos de error</param>
    private void OnFailedGettingChallenge(WebError obj)
    {
        SetActiveSpinner(false);
        ACBSingleton.Instance.AlertPanel.SetupPanel("No se pudo ver este desafio intenta m�s tarde", string.Empty, false, null);
    }
    
    /// <summary>
    /// Activa o desactiva el spinner de carga de acuerdo al estado
    /// </summary>
    /// <param name="state">Estado de activaci�n del spinner de carga</param>
    private void SetActiveSpinner(bool state)
    {
        GameObject spinner = GameObject.Find("Spinner_cancha");
        for(int i=0; i<spinner.transform.childCount; i++)
        {
            spinner.transform.GetChild(i).gameObject.SetActive(state);
        }
    }

    #endregion
}
