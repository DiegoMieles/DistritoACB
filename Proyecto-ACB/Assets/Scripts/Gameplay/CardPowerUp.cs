using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controla los potenciadores que se encuentra dentro de las cartas de los jugadores
/// </summary>
public class CardPowerUp : MonoBehaviour
{
    #region Fields and Properties

    [Header("Card components")]
    [SerializeField] [Tooltip("Botón del potenciador")]
    private Button powerUpButton;
    [SerializeField] [Tooltip("Imagen del fondo del potenciador")]
    private Image backgroundImage;
    [SerializeField] [Tooltip("Vista del potenciador activado")]
    private GameObject boostView;
    [SerializeField] [Tooltip("Valor del potenciador")]
    private Text boostValueText;

    [Space(5)]
    [Header("Power up view values")]
    [SerializeField] [Tooltip("Tipo de potenciador")]
    private Data.BoosterType booster;
    [SerializeField] [Tooltip("Color del fondo cuando el portenciador está activado")]
    private Color powerUpOnColor;
    [SerializeField] [Tooltip("Color del fondo cuando el portenciador está desactivado")]
    private Color powerUpOffColor;

    [Space(5)]
    [Header("Panel to Open components")]
    [SerializeField] [Tooltip("Clase que controla la apertura de nuevos paneles a mostrar")]
    private PanelOpener panelOpener;
    [SerializeField] [Tooltip("Prefab del panel donde se muestran todos los potenciadores")]
    private GameObject boosterPanelPrefab;

    private PlayerCard cardReference; //Referencia de la carta a la que se quiere potenciar
    private bool isCardInTeam; //Determina si la carta a potenciar está en el equipo competitivo

    #endregion

    #region Unity Methods

    /// <summary>
    /// Se ejecuta cuando el potenciador ha sido iniciado por primera vez en escena, añadiendo la carga del panel de
    /// potenciadores al botón del potenciador
    /// </summary>
    private void Start()
    {
        powerUpButton.onClick.AddListener(LoadBoosterPanel);
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Configura a nivel visual el potenciador en la carta
    /// </summary>
    /// <param name="powerUpIsOn">Determina si el potenciador está activo</param>
    /// <param name="cardReference">Referencia de la carta a la que se va a potenciar</param>
    /// <param name="isCardInTeam">Determina si la carta se encuentra en el equipo competitivo</param>
    /// <param name="boostValue">Valor del potenciador</param>
    public void SetupPowerUpView(bool powerUpIsOn, PlayerCard cardReference, bool isCardInTeam, string boostValue = "")
    {
        backgroundImage.color = powerUpIsOn ? powerUpOnColor : powerUpOffColor;
        this.cardReference = cardReference;
        boostView.SetActive(powerUpIsOn);
        boostValueText.text = boostValue;

        this.isCardInTeam = isCardInTeam;
    }

    #endregion

    #region Inner Methods

    /// <summary>
    /// Carga el panel de potenciadores
    /// </summary>
    private void LoadBoosterPanel()
    {
        panelOpener.popupPrefab = boosterPanelPrefab;
        panelOpener.OpenPopup();
        Debug.Log(booster);
        panelOpener.popup.GetComponent<PanelPotenciadores>().CallInfo(booster, cardReference, isCardInTeam);
    }

    #endregion

}
