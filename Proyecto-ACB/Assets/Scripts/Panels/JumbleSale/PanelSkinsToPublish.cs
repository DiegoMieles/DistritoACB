using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Data;
using Newtonsoft.Json;
using WebAPI;
/// <summary>
/// Controla el Panel de las Skins a publicar
/// </summary>
public class PanelSkinsToPublish : Panel
{
    //Evento que se dispara al completar la publicación en el mercadillo
    public event PanelConfirmPublish.VoidDelegate OnConfirmedPublish;
    [SerializeField]
    [Tooltip("Contenedor de los objetos arrastrables")]
    private ToggleGroup toggleGroup;
    [SerializeField]
    [Tooltip("Grupo que almacena todos los demás contenedores")]
    private GameObject gridLayoutSkinsContainer;
    [SerializeField]
    [Tooltip("Grupo que almacena las camisetas y abrigos")]
    private GridLayoutGroup gridLayoutCamisetas;
    [SerializeField]
    [Tooltip("Grupo de  complementos de fondos")]
    private GridLayoutGroup gridLayoutForeground;
    [SerializeField]
    [Tooltip("Grupo que almacena los accesorios de cabeza")]
    private GridLayoutGroup gridLayoutAccesoriosCabeza;
    [SerializeField]
    [Tooltip("Grupo que almacena las cintas de brazo")]
    private GridLayoutGroup gridLayoutCintaBrazo;
    [SerializeField]
    [Tooltip("Grupo que almacena las gafas de sol")]
    private GridLayoutGroup gridLayoutGafasSol;
    [SerializeField]
    [Tooltip("Grupo que almacena los fondos")]
    private GridLayoutGroup gridLayoutFondo;
    [SerializeField]
    [Tooltip("Prefab de skin")]
    private PanelSkin panelSkin;
    [SerializeField]
    [Tooltip("Texto de tiros dobles (antes tiros de tres)")]
    private Text textCamisetas;
    [SerializeField]
    [Tooltip("Texto de complementos del fondo")]
    private Text textComplementosFondo;
    [SerializeField]
    [Tooltip("Texto de rebotes")]
    private Text textAccesoriosCabeza;
    [SerializeField]
    [Tooltip("Texto de tiros libres")]
    private Text textCintaBrazo;
    [SerializeField]
    [Tooltip("Texto de puntos")]
    private Text textGafasSol;
    [SerializeField]
    [Tooltip("Texto de puntos")]
    private Text textFondos;
    [SerializeField, TextArea]
    [Tooltip("Texto que se muestra cuando la traida de datos es fallida")]
    private string textFail;
    [SerializeField]
    [Tooltip("Texto de no skins")]
    private Text textNoSkins;
    [SerializeField]
    [Tooltip("se encarga de abrir otros páneles")]
    private PanelOpener panelOpener;
    [SerializeField]
    [Tooltip("prefab del panel de confirmación ")]
    private GameObject confirmPanelPrefab;
    [SerializeField]
    [Tooltip("botón para publicar la skin seleccionada")]
    private Button publishButton;
    [SerializeField]
    [Tooltip("Spinner de carga")]
    private GameObject spinner;
    private PageBody page; //Clase que contiene los datos principales de una página de ACBalls
    private SkinsToSell skinData = new SkinsToSell(); // Skins del jugador para vender

    private void Start()
    {
        GetInfo();
    }
    /// <summary>
    /// actualiza las skins a vender en la ventana
    /// </summary>
    void UpdateData()
    {
        if (skinData != null)
        {
            foreach (ItemData itemData in skinData.HEADACCESORY)
            {
                var prefab = Instantiate(panelSkin, gridLayoutAccesoriosCabeza.transform);
                prefab.ShowInfo(itemData, null, OnToggleValueChanged );
                textAccesoriosCabeza.gameObject.SetActive(true);
                if (toggleGroup)
                {
                    prefab.GetComponent<Toggle>().group = toggleGroup;
                }
            }
            foreach (ItemData itemData in skinData.ARMACCESORY)
            {
                var prefab = Instantiate(panelSkin, gridLayoutCintaBrazo.transform);
                prefab.ShowInfo(itemData, null, OnToggleValueChanged);
                textCintaBrazo.gameObject.SetActive(true);
                if (toggleGroup)
                {
                    prefab.GetComponent<Toggle>().group = toggleGroup;
                }
            }
            foreach (ItemData itemData in skinData.BACKGROUNDACCESORY)
            {
                var prefab = Instantiate(panelSkin, gridLayoutFondo.transform);
                prefab.ShowInfo(itemData,null, OnToggleValueChanged);
                textFondos.gameObject.SetActive(true);
                if (toggleGroup)
                {
                    prefab.GetComponent<Toggle>().group = toggleGroup;
                }
            }
            foreach (ItemData itemData in skinData.EYEACCESORY)
            {
                var prefab = Instantiate(panelSkin, gridLayoutGafasSol.transform);
                prefab.ShowInfo(itemData, null, OnToggleValueChanged);
                textGafasSol.gameObject.SetActive(true);
                if (toggleGroup)
                {
                    prefab.GetComponent<Toggle>().group = toggleGroup;
                }
            }
            foreach (ItemData itemData in skinData.BODYACCESORY)
            {
                var prefab = Instantiate(panelSkin, gridLayoutCamisetas.transform);
                prefab.ShowInfo(itemData, null, OnToggleValueChanged);
                textCamisetas.gameObject.SetActive(true);
                if (toggleGroup)
                {
                    prefab.GetComponent<Toggle>().group = toggleGroup;
                }
            }
            foreach (ItemData itemData in skinData.FOREGROUNDACCESORY)
            {
                var prefab = Instantiate(panelSkin, gridLayoutForeground.transform);
                prefab.ShowInfo(itemData, null, OnToggleValueChanged);
                textComplementosFondo.gameObject.SetActive(true);
                if (toggleGroup)
                {
                    prefab.GetComponent<Toggle>().group = toggleGroup;
                }
            }
        }
    }
    /// <summary>
    /// Obtiene la información de las skins que tiene el jugador para vender
    /// </summary>
    private void GetInfo()
    {
        skinData?.BODYACCESORY?.Clear();
        skinData?.ARMACCESORY?.Clear();
        skinData?.BACKGROUNDACCESORY?.Clear();
        skinData?.EYEACCESORY?.Clear();
        skinData?.HEADACCESORY?.Clear();
        WebProcedure.Instance.GetSkinsToSell( OnSuccess, OnFailed);
    }
    /// <summary>
    /// Se encontró información de las skins a vender
    /// </summary>
    /// <param name="obj">Información de skins a vender se decodifica en la clase SkinsToSell</param>
    private void OnSuccess(DataSnapshot obj)
    {
            if (obj.RawJson != "[]")
            {
                JsonConvert.PopulateObject(obj.RawJson, skinData);
            }
        if (skinData != null && obj.RawJson != "[]"  && (skinData.BODYACCESORY.Count > 0 || skinData.ARMACCESORY.Count > 0 || skinData.BACKGROUNDACCESORY.Count > 0 || skinData.EYEACCESORY.Count > 0 || skinData.HEADACCESORY.Count > 0 || skinData.FOREGROUNDACCESORY.Count > 0))
        {
            textNoSkins.text = "";
        }
        else
        {
            textNoSkins.text = textFail;
        }
   
        publishButton.onClick.AddListener(PublishClick);
        UpdateData();
        ClosedSpinner();
    }
    /// <summary>
    /// No encontró información de las skins
    /// </summary>
    /// <param name="obj">causas de error</param>
    private void OnFailed(WebError obj)
    {
        ClosedSpinner();
        textNoSkins.text = textFail;
        Debug.LogError(obj);
    }
    /// <summary>
    /// Muestra el panel de confirmación para publicar el item en el mercadillo
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
    /// Intenta Abrir el panel de confirmación para publicar
    /// </summary>
    public void PublishClick()
    {
        if (GetSelectedToggle())
        {
            ShowDialogConfirmation(GetSelectedToggle().GetComponent<PanelSkin>().skinData);
        }
    }
    /// <summary>
    /// Obtiene el objeto arrastrable seleccionado
    /// </summary>
    /// <returns>Referencia del objeto arrastrable</returns>
    private Toggle GetSelectedToggle()
    {
        var toggles = gridLayoutSkinsContainer.transform.GetComponentsInChildren<Toggle>();
        return toggles.FirstOrDefault(t => t.isOn);
    }
    /// <summary>
    /// called when a toggle item change his value
    /// </summary>
    private void OnToggleValueChanged(bool status)
    {
        publishButton.interactable = GetSelectedToggle();
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
}
