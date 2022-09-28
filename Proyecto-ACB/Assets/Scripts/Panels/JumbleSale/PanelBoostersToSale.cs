using Data;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using WebAPI;

/// <summary>
/// Controla el panel de potenciadores
/// </summary>
public class PanelBoostersToSale : Panel
{
    //Evento que se dispara al completar la publicaci?n en el mercadillo
    public event PanelConfirmPublish.VoidDelegate OnConfirmedPublish;
    [SerializeField] [Tooltip("Determina si el potenciador es de una carta")]
    private bool isCardPotenciador;
    [SerializeField] [Tooltip("Grupo que almacena los potenciadores de tiros dobles (antes tiros de tres)")]
    private GridLayoutGroup gridLayoutTriples;
    [SerializeField] [Tooltip("Grupo que almacena los potenciadores de rebotes")]
    private GridLayoutGroup gridLayoutRebotes;
    [SerializeField] [Tooltip("Grupo que almacena los potenciadores de tiros libres")]
    private GridLayoutGroup gridLayoutTirosLibres;
    [SerializeField] [Tooltip("Grupo que almacena los potenciadores de puntos")]
    private GridLayoutGroup gridLayoutPuntos;
    [SerializeField] [Tooltip("Grupo que almacena los potenciadores de asistencias")]
    private GridLayoutGroup gridLayoutAsistencia;
    [SerializeField] [Tooltip("Prefab de potenciador")]
    private PanelPotenciador panelPotenciador;

    [SerializeField]
    [Tooltip("Toggle group que contiene todos los potenciadores")]
    private ToggleGroup toggleGroup;
    [SerializeField] [Tooltip("Texto de tiros dobles (antes tiros de tres)")]
    private Text textTriples;
    [SerializeField] [Tooltip("Texto de rebotes")]
    private Text textRebotes;
    [SerializeField] [Tooltip("Texto de tiros libres")]
    private Text  textLibres;
    [SerializeField] [Tooltip("Texto de puntos")]
    private Text  textPuntos;
    [SerializeField] [Tooltip("Texto de asistencias")]
    private Text  textAsistencia;
    [SerializeField] [Tooltip("Texto de t?tulo")]
    private Text  textNoPotenciadores;
    [SerializeField,TextArea] [Tooltip("Texto que se muestra cuando la traida de datos es fallida")]
    private string textFail;

    [SerializeField] [Tooltip("Contenedor de los potenciadores")]
    private BoosterContainerJumbleSale boosterDataContainer = new BoosterContainerJumbleSale();
    [SerializeField] [Tooltip("Datos de los potenciadores")]
    private ApplyBoosterResponse applyBoosterResponse = new ApplyBoosterResponse();

    [Header("Panel to open components")]
    [SerializeField] [Tooltip("Clase que controla la apertura de nuevos paneles a mostrar")]
    private PanelOpener panelOpener;
    [SerializeField] [Tooltip("Prefab de aplicar poptenciador a la carta")]
    private GameObject alertBoostPanelPrefab;

    [SerializeField] [Tooltip("Se ejecuta cuando la traida de datos de potenciadores es fallida")]
    private UnityEvent onFailed;
    [SerializeField] [Tooltip("Spinner de carga")]
    private GameObject spinner;
    [SerializeField]
    [Tooltip("prefab del panel de confirmaci?n ")]
    private GameObject confirmPanelPrefab;
    [SerializeField]
    [Tooltip("bot?n para publicar la skin seleccionada")]
    private Button publishButton;

    private bool isCardInTeam; //Determina si una carta se encuentra en el equipo competitivo

    /// <summary>
    /// Llama a los datos de los potenciadores al mostrarse por primera vez el panel
    /// </summary>
    private void OnEnable()
    {
        publishButton.onClick.AddListener(PublishClick);
        if (!isCardPotenciador)
        {
            CallInfo();
            OnToggleValueChanged();
        }
    }
    /// <summary>
    /// Muestra el panel de confirmaci?n de aplicar potenciador
    /// </summary>
    /// <param name="boosterdata">Datos del potenciado</param>
    /// <param name="playerCard">Carta a la que se le aplica el potenciador</param>
    public void ShowDialogConfirmation(BoosterData.BoosterItemData boosterdata)
    {
        panelOpener.popupPrefab = confirmPanelPrefab;
        panelOpener.OpenPopup();
        panelOpener.popup.GetComponent<PanelConfirmPublish>().Populate(boosterdata);
        panelOpener.popup.GetComponent<PanelConfirmPublish>().OnConfirmedPublish += () => { OnConfirmedPublish?.Invoke(); Close(); };
    }
    
    /// <summary>
    /// Trae los datos de los potenciadores que tiene el jugador
    /// </summary>
    private void CallInfo()
    {
        WebProcedure.Instance.GetBoostersToSell(snapshot =>
        {
            boosterDataContainer?.Asistencias?.Clear();
            boosterDataContainer?.Puntos?.Clear();
            boosterDataContainer?.Rebotes?.Clear();
            boosterDataContainer?.Tiros?.Clear();
            Debug.Log(snapshot.RawJson);
            JsonConvert.PopulateObject(snapshot.RawJson, boosterDataContainer);
            CheckPotenciadores();
            foreach (var boosterdata in boosterDataContainer.Asistencias)
            {
                var prefab = Instantiate(panelPotenciador, gridLayoutAsistencia.transform);
                prefab.ShowInfo(boosterdata);
                prefab.GetComponent<Toggle>().group = toggleGroup;
                prefab.GetComponent<Toggle>().onValueChanged.AddListener(OnToggleValueChanged);
                textTriples.gameObject.SetActive(true);
            }
            foreach (var boosterdata in boosterDataContainer.Puntos)
            {
                var prefab = Instantiate(panelPotenciador, gridLayoutPuntos.transform);
                prefab.ShowInfo(boosterdata);
                prefab.GetComponent<Toggle>().group = toggleGroup;
                prefab.GetComponent<Toggle>().onValueChanged.AddListener(OnToggleValueChanged);
                textTriples.gameObject.SetActive(true);
            }
            foreach (var boosterdata in boosterDataContainer.Triples)
            {
                var prefab = Instantiate(panelPotenciador, gridLayoutTriples.transform);
                prefab.ShowInfo(boosterdata);
                prefab.GetComponent<Toggle>().group = toggleGroup;
                prefab.GetComponent<Toggle>().onValueChanged.AddListener(OnToggleValueChanged);
                textTriples.gameObject.SetActive(true);
            }
            foreach (var boosterdata in boosterDataContainer.Rebotes)
            {
                var prefab = Instantiate(panelPotenciador, gridLayoutRebotes.transform);
                prefab.ShowInfo(boosterdata);
                prefab.GetComponent<Toggle>().group = toggleGroup;
                prefab.GetComponent<Toggle>().onValueChanged.AddListener(OnToggleValueChanged);
                textTriples.gameObject.SetActive(true);
            }
            foreach (var boosterdata in boosterDataContainer.Tiros)
            {
                var prefab = Instantiate(panelPotenciador, gridLayoutTirosLibres.transform);
                prefab.ShowInfo(boosterdata);
                prefab.GetComponent<Toggle>().group = toggleGroup;
                prefab.GetComponent<Toggle>().onValueChanged.AddListener(OnToggleValueChanged);
                textTriples.gameObject.SetActive(true);
            }
            ClosedSpinner();

        }, error =>
        {
            CheckPotenciadores();
            onFailed.Invoke();
            ClosedSpinner();
        });
    }

    /// <summary>
    /// Realiza verificaci?n de los potenciadores
    /// </summary>
    private void CheckPotenciadores()
    {
        if (boosterDataContainer == null || (boosterDataContainer?.Asistencias?.Count == 0 && boosterDataContainer?.Puntos?.Count == 0)&& boosterDataContainer?.Rebotes?.Count == 0 && boosterDataContainer?.Tiros?.Count == 0 && boosterDataContainer?.Triples?.Count == 0)
        {
            textNoPotenciadores.text = textFail;
            ClosedSpinner();
        }
        else
        {
            textNoPotenciadores.text = string.Empty;   
        }
    }

    /// <summary>
    /// Oculta spinner de carga
    /// </summary>
    private void ClosedSpinner()
    {
        for (int i = 0; i < spinner.transform.childCount; i++)
        {
            spinner.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Activa corrutina que retrasa la desactivaci?n del spinner
    /// </summary>
    public void SpinnerClosing()
    {
        StartCoroutine(CloseWait());
    }

    /// <summary>
    /// Corrutina que retrasa la desactivaci?n del spinner
    /// </summary>
    /// <returns></returns>
    IEnumerator CloseWait()
    {
        yield return new WaitForSeconds(3.0f);
        ClosedSpinner();
    }
    /// <summary>
    /// Muestra el panel de confirmaci?n para publicar el item en el mercadillo
    /// </summary>
    /// <param name="itemData">Datos de la skin</param>
    public void ShowDialogConfirmation(ItemData itemData)
    {
        panelOpener.popupPrefab = confirmPanelPrefab;
        panelOpener.OpenPopup();
        panelOpener.popup.GetComponent<PanelConfirmPublish>().Populate(itemData);
        panelOpener.popup.GetComponent<PanelConfirmPublish>().OnConfirmedPublish += () => { OnConfirmedPublish?.Invoke(); Close(); };
    }
    /// <summary>
    /// Intenta Abrir el panel de confirmaci?n para publicar
    /// </summary>
    public void PublishClick()
    {
        if (GetSelectedToggle())
        {
            ShowDialogConfirmation(GetSelectedToggle().GetComponent<PanelPotenciador>().boostData);
        }

    }
    /// <summary>
    /// Obtiene el objeto arrastrable seleccionado
    /// </summary>
    /// <returns>Referencia del objeto arrastrable</returns>
    private Toggle GetSelectedToggle()
    {
        var toggles = toggleGroup.transform.GetComponentsInChildren<Toggle>();
        return toggles.FirstOrDefault(t => t.isOn);
    }
    /// <summary>
    /// called when a toggle item change his value
    /// </summary>
    private void OnToggleValueChanged(bool status = true)
    {
        publishButton.interactable = GetSelectedToggle();
    }
}
