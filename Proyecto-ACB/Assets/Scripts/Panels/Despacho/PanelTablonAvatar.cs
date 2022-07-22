using Data;
using Newtonsoft.Json;
using System;
using UnityEngine;
using UnityEngine.UI;
using WebAPI;

/// <summary>
/// Controla el panel con los detalles del desafio que un jugador ha seleccionado
/// </summary>
public class PanelTablonAvatar : Panel
{
    #region Fields and properties

    [Header("Panel components")]
    [SerializeField] [Tooltip("Controla la visión de la imagen del avatar del player")]
    private AvatarImageView playerView;
    [SerializeField] [Tooltip("Botón que se encarga del cerrado del panel")]
    private Button goBackButton;
    [SerializeField] [Tooltip("Texto con el nombre del jugador o del rival del jugador")]
    private Text nameText;
    [SerializeField] [Tooltip("Texto con la fecha del día de publicación del desafio")]
    private Text dateText;
    [SerializeField] [Tooltip("Texto que muestra los puntos que se ganan/pierden cuando se acepta un desafio")]
    private Text pointsText;
    [SerializeField] [Tooltip("Botón donde se acepta el desafio seleccionado")]
    private Button acceptChallengeButton;
    [SerializeField] [Tooltip("Texto con la cantidad de monedas que tiene el jugador")]
    private Text coinsAmountText;
    [SerializeField] [Tooltip("Valor de cantidad máxima de monedas del jugador que se puede mostrar en el panel")]
    private float limit;
    [SerializeField] [Tooltip("Texto con la cantidad de monedas que cuesta aceptar el desafio")]
    private Text challengeCost;
    [SerializeField] [Tooltip("Layout donde se muestran los datos de las monedas obtenida por el jugador")]
    private GameObject playerCoinsLayoutElement;
    [SerializeField] [Tooltip("Spinner de carga del panel")]
    private GameObject spinner;

    [Space(5)]
    [Header("Panel to open reference")]
    [SerializeField] [Tooltip("Clase que controla la apertura de nuevos paneles a mostrar")]
    private PanelOpener panelOpener;
    [SerializeField] [Tooltip("Prefab del panel de simulación del desafio")]
    private GameObject simulationPanelPrefab;

    [Space(5)]
    [Header("Alert strings")]
    [SerializeField] [Tooltip("Texto del título de aceptar desafio del panel de alerta")]
    private string buyAlertTitle;
    [SerializeField] [Tooltip("Texto que se muestra al no tener las suficientes ACBCoins para aceptar el desafio en el panel de alerta")]
    private string notEnoughMoneyText;
    [SerializeField] [Tooltip("Texto que se muestra cuando hay un error al aceptar un desafio en el panel de alerta")]
    private string CreationErrorText;

    private ChallengesTablon.ChallengesTablonItem challengeData; //Clase con los datos del desafio
    private Action onFinishedChallenge; //Acción que se ejecuta cuando el desafio ha terminado de ser simmulado
    private bool freeChallenge; //Determina si un desafio es gratis o no
    private string messageFree; //Texto que se muestra en el panel de alerta si el desafio es gratis
    #endregion

    #region Public Methods

    /// <summary>
    /// Pone los datos del desafio cuando ha sido aceptado por tablón
    /// </summary>
    /// <param name="challengeData">Datos del desafio aceptado</param>
    /// <param name="onFinishedChallenge">Acción que se ejecuta cuando el desafio ha sido aceptado</param>
    /// <param name="showLowerLayoutElements">Determina si la información de la parte inferior de la pantalla se debe mostrar</param>
    /// <param name="freeChallenge">Determina si el desafio es gratis o no</param>
    /// <param name="messageFree">Mensaje que se muestra cuando el desafio es gratis</param>
    public void SetAvatarData(ChallengesTablon.ChallengesTablonItem challengeData, Action onFinishedChallenge, bool showLowerLayoutElements, bool freeChallenge, string messageFree)
    {
        spinner.SetActive(false);
        this.challengeData = challengeData;
        this.onFinishedChallenge = onFinishedChallenge;
        this.freeChallenge = freeChallenge;
        this.messageFree = messageFree;
        playerView.UpdateView(challengeData, true);
        goBackButton.onClick.AddListener(Close);
        nameText.text = challengeData.nickName;
        dateText.text = challengeData.created.ToString();
        pointsText.text = challengeData.points.ToString() + " Puntos";
        challengeCost.text = challengeData.challengeCost.ToString();
        acceptChallengeButton.gameObject.SetActive(showLowerLayoutElements);
        acceptChallengeButton.onClick.AddListener(CheckIfCanAcceptChallenge);
        playerCoinsLayoutElement.SetActive(showLowerLayoutElements);
        coinsAmountText.text = Mathf.Clamp(ACBSingleton.Instance.AccountData.statsData.coinsBalance, 0, limit) .ToString();
    }

    /// <summary>
    /// Pone los datos del avatar de jugador seleccionado
    /// </summary>
    /// <param name="challengeAceptedData">Datos de desafio que se han aceptado</param>
    /// <param name="isRival">Determina si el jugador seleccionado es el rival</param>
    public void SetAvatarData(ChallengeAcceptedData challengeAceptedData, bool isRival)
    {
        spinner.SetActive(false);

        if(isRival)
            playerView.UpdateView(challengeAceptedData.rival.avatarData, true);
        else
            playerView.UpdateView(challengeAceptedData.user.avatarData, true);

        goBackButton.onClick.AddListener(Close);
        nameText.text = isRival ? challengeAceptedData.rival.avatarData.nickName : challengeAceptedData.user.avatarData.nickName;
        dateText.text = "";
        pointsText.text = isRival ? challengeAceptedData.rival.avatarData.points.ToString() + " Puntos" : challengeAceptedData.user.avatarData.points.ToString() + " Puntos";
        acceptChallengeButton.gameObject.SetActive(false);
        playerCoinsLayoutElement.SetActive(false);
    }

    #endregion

    #region Inner Methods

    /// <summary>
    /// Revisa que el jugador tenga la suficiente cantidad de monedas para aceptar el desafio en caso de no se un desafio gratis
    /// </summary>
    private void CheckIfCanAcceptChallenge()
    {
        if (!freeChallenge)
        {
            if(ACBSingleton.Instance.AccountData.statsData.coinsBalance >= challengeData.challengeCost)
                ACBSingleton.Instance.AlertPanel.SetupPanel(buyAlertTitle, "Esta operación te costará " + challengeData.challengeCost + " acbCoins", true, () => { AcceptChallenge(); spinner.SetActive(true); }, Close);
            else
                ACBSingleton.Instance.AlertPanel.SetupPanel(notEnoughMoneyText, "", false, Close);
        }
        else
        {
            ACBSingleton.Instance.AlertPanel.SetupPanel(buyAlertTitle, messageFree, true, () => { AcceptChallenge(); spinner.SetActive(true); }, Close);
        }
    
    }

    /// <summary>
    /// Acepta el desafio el jugador y hace que backend compruebe si el desafio seleccionado puede ser realmente aceptado
    /// </summary>
    private void AcceptChallenge()
    {
        
        ChallengeAccept challengeAcceptedData = new ChallengeAccept() { challenge_id = challengeData.challenge_id };
        WebProcedure.Instance.PostAcceptChallenge(JsonConvert.SerializeObject(challengeAcceptedData), AcceptedChallenge, (error) => ACBSingleton.Instance.AlertPanel.SetupPanel(CreationErrorText, "", false, null));
    }

    /// <summary>
    /// Se ejecuta cuando el desafio aceptado ha sido correctamente validado por backend, actualiza los datos del desafio y abre el panel
    /// de simulación del desafio
    /// </summary>
    /// <param name="obj">Clase con los datos del desafio aceptado</param>
    private void AcceptedChallenge(DataSnapshot obj)
    {
        Debug.Log(obj.RawJson);
        Debug.Log(obj.Code);
        ChallengeAcceptedData challengeData = new ChallengeAcceptedData();
        JsonConvert.PopulateObject(obj.RawJson, challengeData);
        JsonConvert.PopulateObject(obj.RawJson, ACBSingleton.Instance.AccountData.statsData);
        JsonConvert.PopulateObject(obj.RawJson, ACBSingleton.Instance.AccountData);
        if (obj.Code == 200)
        {
            OpenStartChallengePanel(challengeData);
            Firebase.Analytics.FirebaseAnalytics.LogEvent("deal_ok");
            Debug.Log("Analytic deal_ok logged");
        }
        else
        {
            ACBSingleton.Instance.AlertPanel.SetupPanel(obj.MessageCustom, "", false, Close);
        }
     
    }

    /// <summary>
    /// Muestra el panel de simulación del desafio
    /// </summary>
    /// <param name="challengeData">Clase con los datos del desafio aceptado</param>
    private void OpenStartChallengePanel(ChallengeAcceptedData challengeData)
    {
        panelOpener.popupPrefab = simulationPanelPrefab;
        panelOpener.OpenPopup();
        panelOpener.popup.GetComponent<PanelPlayChallenge>().StartChallenge(challengeData, onFinishedChallenge, true);
        Close();
    }

    #endregion
}
