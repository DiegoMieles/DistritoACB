using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controla el panel que simula una consola dentro del juego
/// </summary>
public class PanelDebugLogger : MonoBehaviour
{
    #region Fields and properties

    [Header("Panel component")]
    [SerializeField] [Tooltip("Componente canvas group que controla la opacidad de los elementos UI")]
    private CanvasGroup logCanvasGroup;
    [SerializeField] [Tooltip("Texto de la consola")]
    private Text logText;
    [SerializeField] [Tooltip("Botón de limpieza de consola")]
    private Button cleanButton;
    [SerializeField] [Tooltip("Slider que controla la opacidad de la consola")]
    private Slider alphaSlider;

    private bool panelIsSetup = false; //Determina si el panel ha sido previamente configurado

    #endregion

    #region Public Methods

    /// <summary>
    /// Configura la consola y la limpia
    /// </summary>
    public void SetupPanel()
    {
        logCanvasGroup.alpha = 1;
        alphaSlider.onValueChanged.AddListener(SetLoggerAlpha);
        cleanButton.onClick.AddListener(CleanText);
        gameObject.SetActive(true);
        panelIsSetup = true;
        CleanText();
    }

    /// <summary>
    /// Pone un texto en consola
    /// </summary>
    /// <param name="logStringText">Texto a poner en la consola</param>
    public void SetText(string logStringText)
    {
        if (!panelIsSetup)
            return;

        logText.text += "\n" + logStringText;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Cambia la opacidad de la consola
    /// </summary>
    /// <param name="newAlphaValue">Valor de opacidad</param>
    private void SetLoggerAlpha(float newAlphaValue)
    {
        if (!panelIsSetup)
            return;

        logCanvasGroup.alpha = newAlphaValue;
    }

    /// <summary>
    /// Limpia la consola
    /// </summary>
    private void CleanText() => logText.text = "";

    #endregion

}
