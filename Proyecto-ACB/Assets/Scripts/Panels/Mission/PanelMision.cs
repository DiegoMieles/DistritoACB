using ARSupportCheck;
using UnityEngine;
using UnityEngine.UI;
using Data;
using WebAPI;
using Newtonsoft.Json;

/// <summary>
/// Controla el panel principal de la misi�n
/// </summary>
public class PanelMision : Panel
{
    [Header("Scriptable backend components")]
    [SerializeField] [Tooltip("Texto con la informaci�n de error cuando falla la misi�n")]
    private string textErrorInfo;
    [SerializeField] [Tooltip("Plugin de UniWebView")]
    private UniWebView webView;

    [Header("Panel Components")]
    [SerializeField] [Tooltip("Clase que controla la apertura de nuevos paneles a mostrar")]
    private PanelOpener missionPanelOpener;
    [SerializeField] [Tooltip("Prefab del panel de QR")]
    private GameObject QRPanelPrefab;
    [SerializeField] [Tooltip("Prefab del panel de c�digo")]
    private GameObject CodePanelPrefab;
    [SerializeField] [Tooltip("Prefab del panel de AR")]
    private GameObject ARPanelPrefab;
    [SerializeField] [Tooltip("Bot�n que se encarga del cerrado del panel")]
    private Button closeButton;

    private MissionsData.MissionItemData currentMission; //Datos de la misi�n a realizar
    public MissionsData.MissionItemData CurrentMission => currentMission;

    #region Unity Methods

    /// <summary>
    /// Se ejecuta cuando el panel ha sido iniciado por primera vez en escena y configura el bot�n de cerrado
    /// </summary>
    private void Start()
    {
        closeButton.onClick.AddListener(() => { Close(); ACBSingleton.Instance.UpdateGameData(); });
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Abre el panel de QR
    /// </summary>
    public void OpenQRPanel()
    {
        missionPanelOpener.popupPrefab = QRPanelPrefab;
        missionPanelOpener.canParentCanvas = true;
        missionPanelOpener.OpenPopup();
        missionPanelOpener.popup.GetComponent<PanelQr>().currentMissionData = currentMission;
        Close();
    }

    /// <summary>
    /// Abre el panel de AR
    /// </summary>
    public void OpenARPanel()
    {
        missionPanelOpener.popupPrefab = ARPanelPrefab;
        missionPanelOpener.canParentCanvas = false;
        ACBSingleton.Instance.SetActiveCamera(false);
        missionPanelOpener.OpenPopup();
        missionPanelOpener.popup.GetComponent<PanelAr>().SetAndShowReward(currentMission, ARSupportChecker.IsSupported());
        Close(); 
    }

    /// <summary>
    /// Abre el panel de c�digo
    /// </summary>
    public void OpenCodePanel(string code)
    {
        missionPanelOpener.popupPrefab = CodePanelPrefab;
        missionPanelOpener.canParentCanvas = true;
        missionPanelOpener.OpenPopup();
        var codepop = missionPanelOpener.popup.GetComponent<PanelCode>();
        codepop.currentMissionData = currentMission;
        codepop.CodeKeyFinished(code);
        Close();
    }

    /// <summary>
    /// Verifica si el panel de AR puede ser abierto de acuerdo a la posici�n del jugador
    /// </summary>
    public void CheckIfARPanelCanBeOpened()
    {
        if (currentMission.valGPS)
        {
            float currentLatitude = Input.location.lastData.latitude;
            float currentLongitude = Input.location.lastData.longitude;

            GPSConfirmBody missionBodyData = new GPSConfirmBody()
            {
                lat = Input.location.lastData.latitude,
                lon = Input.location.lastData.longitude,
                mission_id = currentMission.id,
                uuid_r = currentMission.uuid_r
            };

            WebProcedure.Instance.PostValidateMissionIsNearToARCardPlace(JsonConvert.SerializeObject(missionBodyData), OnValidNearMission, OnInvalidNearMission);
        }
        else
            OpenARPanel();
    }

    /// <summary>
    /// Muestra la infomaci�n de la misi�n
    /// </summary>
    /// <param name="newMissionData">Datos de la misi�n</param>
    public void ShowInfo(MissionsData.MissionItemData newMissionData)
    {
        currentMission = newMissionData;
        if (!string.IsNullOrEmpty(currentMission.url))
        {
            webView.urlOnStart = newMissionData.url;
        }
        else
        {
            byte[] tempBytes;
            tempBytes = System.Text.Encoding.Default.GetBytes(textErrorInfo);
            string message = System.Text.Encoding.UTF8.GetString(tempBytes);
            ACBSingleton.Instance.AlertPanel.SetupPanel(message, "", false, Close, null, 0.2f);
        }
    }

    #endregion

    #region Inner Methods

    /// <summary>
    /// M�todo que se ejecuta cuando la misi�n es v�lida por cercan�a
    /// </summary>
    /// <param name="obj">Clase con los datos devueltos por backend de forma exitosa</param>
    private void OnValidNearMission(DataSnapshot obj)
    {
        if(obj.Code == 200)
            OpenARPanel();
        else
        {
            ACBSingleton.Instance.AlertPanel.SetupPanel(string.Empty, obj.MessageCustom, false, null);
            Close();
        }
    }

    /// <summary>
    /// M�todo que se ejecuta cuando la misi�n falla por distancia
    /// </summary>
    /// <param name="obj">Clase con los datos de error devueltos por backend</param>
    private void OnInvalidNearMission(WebError obj)
    {
        ACBSingleton.Instance.AlertPanel.SetupPanel(string.Empty, obj.Message, false, null);
        Close();
    }
    

    #endregion
}
