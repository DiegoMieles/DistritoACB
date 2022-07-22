using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controla el panel que se muestra al intentar entrar a un desafio sin el equipo competitivo completo
/// </summary>
public class ChallengeIncompleteTeamPanel : Panel
{
    #region Fields and Properties

    [Header("Panel components")]
    [SerializeField] [Tooltip("Botón que se encarga del cerrado del panel")]
    private Button closeButton;
    [SerializeField] [Tooltip("Texto de alerta superior")]
    private Text upperAlertText;
    [SerializeField] [Tooltip("Texto de alerta inferior")]
    private Text lowerAlertText;

    [Space(5)]
    [Header("Alert strings values")]
    [SerializeField] [Tooltip("Cadena de texto de alerta superior")]
    private string upperAlertString;
    [SerializeField] [Tooltip("Cadena de texto de alerta inferior")]
    private string lowerAlertString;
    [SerializeField] [Tooltip("Cadena de texto de alerta superior cuando no está el equipo competitivo completo")]
    private string upperAlertNotTeamString;
    [SerializeField] [Tooltip("Cadena de texto de alerta inferior cuando no está el equipo competitivo completo")]
    private string lowerAlertNotTeamString;
    #endregion

    #region Public Methods

    /// <summary>
    /// Abre el panel
    /// </summary>
    public void OpenAlert()
    {
        closeButton.onClick.AddListener(Close);
        upperAlertText.text = upperAlertString;
        lowerAlertText.text = lowerAlertString;
    }
    
    /// <summary>
    /// Abre el panel indicando que el equipo competitivo está incompleto
    /// </summary>
    public void OpenAlertNotTeam()
    {
        closeButton.onClick.AddListener(Close);
        upperAlertText.text = upperAlertNotTeamString;
        lowerAlertText.text = lowerAlertNotTeamString;
    }

    /// <summary>
    /// Abre el panel con la alerta de que no puede el jugador publicar desafios
    /// </summary>
    /// <param name="message">Mensaje de la alerta</param>
    public void OpenAlertNoCanPostChallenge(string message)
    {
        closeButton.onClick.AddListener(Close);
        upperAlertText.text = "No puedes publicar desafío";
        lowerAlertText.text = message;
    }

    /// <summary>
    /// Abre el panel con la alerta de que no puede el jugador aceptar el desafio
    /// </summary>
    /// <param name="message">Mensaje de la alerta</param>
    public void OpenAlertNoCanAcceptChallenge(string message)
    {
        closeButton.onClick.AddListener(Close);
        upperAlertText.text = "No puedes aceptar desafío";
        lowerAlertText.text = message;
    }

    #endregion

}
