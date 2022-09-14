using Data;
using Newtonsoft.Json;
using System;
using UnityEngine;
using UnityEngine.UI;
using WebAPI;
using System.Collections.Generic;
/// <summary>
/// Controla el panel de carta del equipo competitivo
/// </summary>
public class PanelCardCompetitiveTeam : Panel
{
    #region Fields and properties

    [Header("Panel components")]
    [SerializeField] [Tooltip("Botón que se encarga del cerrado del panel")]
    private Button closeButton;
    [SerializeField] [Tooltip("Panel de carta")]
    private PlayerCard card;
    [SerializeField] [Tooltip("Objeto arrastrable de la carta")]
    private TokenItemData tokendata;
    [SerializeField] [Tooltip("Se ejecuta cuando la traida de datos de liga es fallida")]
    private Action onFailed;
    [SerializeField] [Tooltip("Botón de eliminar carta del equipo competitivo")]
    private Button removeCardButton;
    [SerializeField]
    [Tooltip("icono de la liga clásica")]
    private Sprite clasicLeagueIcon;
    [SerializeField]
    [Tooltip("icono de la liga actual")]
    private Sprite actualLeagueIcon;
    [SerializeField]
    [Tooltip("botón de la liga actual")]
    private List<Image> cardBorders;
    [Space(5)]
    [Header("Panel texts")]
    [SerializeField] [Tooltip("Texto de confirmación de alerta")]
    private string confirmationAlertText = "¿Quieres añadir el token a tu Equipo Competitivo?";
    [SerializeField] [Tooltip("Spinner de carga")]
    private GameObject Spinner;
    
    private Action onGoBack;

    #endregion

    #region Unity Methods

    /// <summary>
    /// Suscribe acción de cerrar panel del equipo competitivo
    /// </summary>
    private void OnEnable()
    {
        PanelTeamCompetitivo.OnClose += Close;
    }

    /// <summary>
    /// Desuscribe el evento de cerrar panel al destruirse el objeto
    /// </summary>
    private void OnDestroy()
    {
        PanelTeamCompetitivo.OnClose -= Close;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Asigna los datos generales de la carta
    /// </summary>
    /// <param name="tokendata">Datos de miniatura de la carta</param>
    /// <param name="onGoBack"><Acción que se ejecuta cuando se cierra el panel/param>
    /// <param name="canDelete">Determina si la carta puede ser retirada del equipo competitivo</param>
    /// <param name="onBoosterAdded">Acción que se ejcuta cuando se aplica un potenciador a una carta</param>
    public void SetCardData(TokenItemData tokendata, Action onGoBack, bool canDelete = true, Action onBoosterAdded = null)
    {
        this.tokendata = tokendata;
        this.onGoBack = onGoBack;

        Action onCardBackButtonClicked;

        if (canDelete)
            onCardBackButtonClicked = DeleteCardFromTeam;
        else
            onCardBackButtonClicked = ShowConfirmationPanel;

        card.SetupCardData(tokendata, onCardBackButtonClicked, true, canDelete, false, true, onBoosterAdded);
        closeButton.onClick.AddListener(() => { Close(); this.onGoBack?.Invoke(); });
        removeCardButton.gameObject.SetActive(canDelete);
        removeCardButton.onClick.AddListener(DeleteCardFromTeam);
    }

    /// <summary>
    /// Asigna los datos de la carta
    /// </summary>
    /// <param name="cardData">Datos de la carta</param>
    public void SetCardData(ChallengeAcceptedData.CardChallengeData.CardItems cardData)
    {
        card.SetupCardData(cardData);
        closeButton.onClick.AddListener(Close);
        removeCardButton.gameObject.SetActive(false);
    }
    
    #endregion

    #region Inner Methods

    /// <summary>
    /// Elimina carta del equipo competitivo
    /// </summary>
    private void DeleteCardFromTeam()
    {
        var cardbody = JsonConvert.SerializeObject(new BodyTokenCard() { tokenCard = tokendata.tokencard_token });
        
        WebProcedure.Instance.DelRemoveTokenOfTeam(cardbody, snapshot =>
        {
            ACBSingleton.Instance.AlertPanel.SetupPanel(snapshot.MessageCustom, string.Empty, false, () =>
            {
                if(GameObject.FindObjectOfType<PanelTeamCompetitivo>() != null)
                {
                    GameObject.FindObjectOfType<PanelTeamCompetitivo>().CallInfoActualLeague();
                    PanelTeamCompetitivo.OnClose?.Invoke();
                }
                else
                    Close();

            },null,0,"Aceptar","Cancelar", FindObjectOfType<PanelTeamCompetitivo>(true).isActualLeague ?actualLeagueIcon: clasicLeagueIcon);
     
        }, error =>
        {
            ClosedSpinner();
        }, tokendata.tokencard_token);
    }

    /// <summary>
    /// Muestra panel de confirmación
    /// </summary>
    public void ShowConfirmationPanel() => ACBSingleton.Instance.AlertPanel.SetupPanel(confirmationAlertText, string.Empty, true, AddCardToTeam);

    /// <summary>
    /// Añade carta al equipo competitivo
    /// </summary>
    private void AddCardToTeam()
    {
        var cardbody = JsonConvert.SerializeObject(new BodyTokenCard() { tokenCard = tokendata.token });
        WebProcedure.Instance.PostAddTokenToTeam(cardbody, snapshot =>
        {
            ACBSingleton.Instance.AlertPanel.SetupPanel(snapshot.MessageCustom, string.Empty, false, () =>
            {
                if(GameObject.FindObjectOfType<PanelTeamCompetitivo>() != null)
                {
                    GameObject.FindObjectOfType<PanelTeamCompetitivo>().CallInfoActualLeague();
                    PanelTeamCompetitivo.OnClose?.Invoke();
                }

                Close();
            });
        }, error =>
        {
            onFailed.Invoke();
            ClosedSpinner();
        });
    }

    /// <summary>
    /// Desactiva el spinner de carga
    /// </summary>
    private void ClosedSpinner()
    {
        Spinner.gameObject.SetActive(false);
    }

    #endregion
}
