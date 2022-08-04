using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controlador del panel general de alertas del juego
/// </summary>
public class PanelAlert : MonoBehaviour
{
    [Header("Panel Components")]
    [SerializeField] [Tooltip("Texto de título de la alerta")]
    private Text alertText;
    [SerializeField] [Tooltip("Texto de descripción de la alerta")]
    private Text descriptionText;
    [SerializeField] [Tooltip("Botón de aceptar alerta")]
    private Button acceptButton;
    [SerializeField] [Tooltip("Botón de cancelar alerta")]
    private Button cancelButton;
    [SerializeField] [Tooltip("Texto del boton de aceptar")]
    private Text acceptButtonText;
    [SerializeField] [Tooltip("Texto del botón de cancelar")]
    private Text cancelButtonText;

    private Action onSelectAcceptButton; //Acción que se ejecuta cuando se acepta la alerta
    private Action onSelectCancelButton; //Acción que se ejecuta cuando se cancela la alerta

    #region Unity Methods

    /// <summary>
    /// e ejecuta cuando el panel ha sido iniciado por primera vez en escena, añadiendo los eventos básicos de los botones disponibles en el panel
    /// </summary>
    private void Start()
    {
        acceptButton.onClick.AddListener(() => { onSelectAcceptButton?.Invoke(); gameObject.SetActive(false); descriptionText.gameObject.SetActive(false); });
        cancelButton.onClick.AddListener(() => { onSelectCancelButton?.Invoke(); gameObject.SetActive(false); descriptionText.gameObject.SetActive(false); });
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Configura el panel junto con los textos a mostrar y las acciones a realizar de acuerdo a la opción
    /// </summary>
    /// <param name="newText">Título del panel</param>
    /// <param name="description">Descripción de la alerta</param>
    /// <param name="needCancelButton">Determina si se necesita el botón de cancelar para la alerta</param>
    /// <param name="onSelectAcceptButton">Acción personalizada que se ejecutará al dar click en el botón de aceptar</param>
    /// <param name="onSelectCancelButton">Acción personalizada que se ejecutará al dar click en el botón de cancelar</param>
    /// <param name="delay">Valor en segundos para retrasar el momento de mostrar el panel</param>
    /// <param name="customAcceptButtonText">Texto personalizado al botón de aceptar</param>
    /// <param name="customCancelButtonText">Texto personalizado al botón de cancelar</param>
    public void SetupPanel(string newText, string description, bool needCancelButton, Action onSelectAcceptButton, Action onSelectCancelButton = null, float delay = 0,
        string customAcceptButtonText = "Aceptar", string customCancelButtonText = "Cancelar")
    {
        descriptionText.gameObject.SetActive(true);
        gameObject.SetActive(true);
        transform.SetAsFirstSibling();
        acceptButtonText.text = customAcceptButtonText;
        cancelButtonText.text = customCancelButtonText;
        descriptionText.text = description;
        if (string.IsNullOrEmpty(description))
        {
            descriptionText.gameObject.SetActive(false);
        }
        StartCoroutine(ShowPanel(newText,needCancelButton,onSelectAcceptButton,onSelectCancelButton,delay));
    }

    /// <summary>
    /// Corrutina encargada de mostrar el panel una vez cumplido el tiempo de retraso para mostrarse
    /// </summary>
    /// <param name="newText">Título del panel</param>
    /// <param name="needCancelButton">Determina si se necesita el botón de cancelar para la alerta</param>
    /// <param name="onSelectAcceptButton">Acción personalizada que se ejecutará al dar click en el botón de aceptar</param>
    /// <param name="onSelectCancelButton">Acción personalizada que se ejecutará al dar click en el botón de cancelar</param>
    /// <param name="delay">Valor en segundos para retrasar el momento de mostrar el panel</param>
    /// <returns></returns>
    private IEnumerator ShowPanel(string newText,  bool needCancelButton, Action onSelectAcceptButton, Action onSelectCancelButton = null, float delay = 0)
    {
        yield return new WaitForSeconds(delay);
        descriptionText.gameObject.SetActive(false);
        alertText.text = newText;
        this.onSelectAcceptButton = onSelectAcceptButton;
        this.onSelectCancelButton = onSelectCancelButton;

        descriptionText.gameObject.SetActive(!string.IsNullOrEmpty(descriptionText.text));
        cancelButton.gameObject.SetActive(needCancelButton);
        transform.SetAsLastSibling();
    }

    #endregion
}
