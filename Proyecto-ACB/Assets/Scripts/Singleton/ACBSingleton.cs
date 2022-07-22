using System;
using UnityEngine;
using Data;
using Firebase.Extensions;
using Newtonsoft.Json;
using WebAPI;

/// <summary>
/// Singleton principal de juego donde se controlan los valores principales de juego
/// </summary>
public class ACBSingleton : BASESingleton<ACBSingleton>
{
    protected ACBSingleton() { }

    [Header("Scriptable references")]
    [SerializeField] [Tooltip("Objeto que contiene los datos principales de juego")]
    private ScriptableGameData gameData;
    [SerializeField] [Tooltip("Objeto que contiene los datos principales del avatar del jugador")]
    private ScriptableAccount accountData;
    [SerializeField] [Tooltip("Objeto que contiene los datos extra del avatar del jugador")]
    private AvatarExtraData avatarExtraData;

    [Space(5)]
    [Header("Panels references")]
    [SerializeField] [Tooltip("Panel general de alertas del juego")]
    private PanelAlert alertPanel;
    [SerializeField] [Tooltip("Panel de recompensas del juego")]
    private PanelReward rewardPanel;
    [SerializeField] [Tooltip("Panel donde se encuentra el mapa principal de la ciudad")]
    private PanelMainMenu mainMenuPanel;
    [SerializeField] [Tooltip("Panel de autenticación de usuario")]
    private PanelOpener authenticationPanel;
    [SerializeField] [Tooltip("Clase que controla la apertura de nuevos paneles a mostrar")]
    private PanelOpener ViewPanel;
    [SerializeField] [Tooltip("Controlador general de los edificios del juego")]
    private PanelBuildingSelection panelBuildingSelection;
    [SerializeField] [Tooltip("Panel que simula una consola dentro del juego")]
    private PanelDebugLogger panelDebugLogger;
    [SerializeField] [Tooltip("Spinner genérico de carga")]
    private GameObject mainSpinner;

    [Space(5)]
    [Header("Other components")]
    [SerializeField] [Tooltip("Cámara principal de juego")]
    private GameObject mainCamera;
    [SerializeField] [Tooltip("Canvas principal de juego")]
    private Canvas mainCanvas;
    [SerializeField] [Tooltip("Texto de error que se muestra al no poder cargar el juego")]
    private string textError = "No hay conexi�n a internet, conectate a internet e intenta entrar al app nuevamente";
    [SerializeField] [Tooltip("Texto de sesión caducada")]
    private string textToken = " Tu sesión ha cadudado.Identifícate de nuevo y sigue disfrutando de tu experiencia en Distrito acb.";
   
    [Space(5)]
    [Header("Testing Values")]
    [SerializeField] [Tooltip("Determina si se debe mostrar el panel de debug interno del juego")]
    private bool showDebugLogger;
    [SerializeField] [Tooltip("Determina si se puede saltar la verificación de usuario")]
    private bool bypassLogin;

    [HideInInspector] [Tooltip("Misión actual del jugador")]
    public MissionButton actualCachedMission;

    [Tooltip("Acción a ejecutar cuando el usuario ha sido autenticado")]
    public Action onUserAuthenticated;
    [Tooltip("Texto de autenticación")]
    public const string AUTHENTICATION_STRING = "Authenticated";

    private bool scriptablesAreLoaded = false; //Determina si los objetos con los datos principales de juego y del avatar ya han sido cargados
    public GameObject MainSpinner => mainSpinner;
    public Camera MainCamera => mainCamera.GetComponent<Camera>();
    public ScriptableGameData GameData => gameData;
    public ScriptableAccount AccountData => accountData;
    public AvatarExtraData AvatarExtraData => avatarExtraData;
    public PanelAlert AlertPanel => alertPanel;
    public PanelReward RewardPanel => rewardPanel;
    public PanelBuildingSelection PanelBuildingSelection => panelBuildingSelection;
    public PanelDebugLogger PanelDebugLogger => panelDebugLogger;
    public string LostConnectionTextError => textError;
    public bool ScriptablesAreLoaded { get => scriptablesAreLoaded; set => scriptablesAreLoaded = value; }

    #region Unity Methods

    /// <summary>
    /// Se ejecuta cuando se inicia el singleton
    /// </summary>
    private void OnEnable()
    {
        WebProcedure.Instance.onTokenFailed += OnTokenFailed;
        onUserAuthenticated = AuthenticateUser;
        scriptablesAreLoaded = false;

        if (bypassLogin)
            mainMenuPanel.LoadPlayerAccountData();
        else
            CheckAuthentication();

        if (showDebugLogger)
            panelDebugLogger.SetupPanel();
        else
            panelDebugLogger.gameObject.SetActive(false);
    }


    #endregion

#region Public Methods
    /// <summary>
    /// Resetea los datos de misión actual almacenado
    /// </summary>
    public void UpdateActualMissionButtonView()
    {
        actualCachedMission.ResetMission();
        actualCachedMission = null;
    }

    /// <summary>
    /// Actualiza los datos de juego
    /// </summary>
    public void UpdateGameData() => mainMenuPanel.LoadPlayerAccountData();

    /// <summary>
    /// Determina el estado de activación de la cámara principal de juego
    /// </summary>
    /// <param name="state">Estado de activación</param>
    public void SetActiveCamera(bool state) => mainCamera.SetActive(state);

    /// <summary>
    /// Determina el estado de activación del spinner de carga genérico del juego
    /// </summary>
    /// <param name="state">Estado de activación</param>
    public void ActivateMainSpinner(bool state)
    {
        mainSpinner.SetActive(state);
        mainSpinner.transform.SetAsLastSibling();
    }

    /// <summary>
    /// Determina si la cámara está en modo de renderización
    /// </summary>
    /// <param name="state">Estado de renderización actual</param>
    public void SetCanvasRenderModeAsCamera(bool state)
    {
        RenderMode newRenderMode = state ? RenderMode.ScreenSpaceCamera : RenderMode.WorldSpace;
        mainCanvas.renderMode = newRenderMode;
    }

#endregion

#region Private Methods

    /// <summary>
    /// Cuando el token falla pone datos genéricos
    /// </summary>
    private void OnTokenFailed()
    {
      WebProcedure.Instance.accessData = new UserData();
      AlertPanel.SetupPanel(textToken, "", false, ()=> LogOut());
    }
    
    /// <summary>
    /// Verifica si el usuario ya ha sido autenticado e inicia los datos de juego
    /// </summary>
    private void CheckAuthentication()
    {
        if (PlayerPrefs.GetInt(AUTHENTICATION_STRING, 0) != 1)
        {
            authenticationPanel.OpenPopup();
        }
        var credentials=  PlayerPrefs.GetString(WebProcedure.CREDENTIALS,string.Empty);
        
        if(PlayerPrefs.GetInt(AUTHENTICATION_STRING) == 1)
        {
            JsonConvert.PopulateObject(credentials , WebProcedure.Instance.accessData);  
            mainMenuPanel.LoadPlayerAccountData();
        }
    }

    /// <summary>
    /// Autentica al usuario
    /// </summary>
    private void AuthenticateUser()
    {
        PlayerPrefs.SetInt(AUTHENTICATION_STRING, 1);
        authenticationPanel.ClosePopup();
        mainMenuPanel.LoadPlayerAccountData();
    }

    /// <summary>
    /// Se salta la verificación de usuario cargando los datos princiaples del juego
    /// </summary>
    public void PassWithOutUser()
    {
        mainMenuPanel.LoadPlayerAccountData();
    }

    /// <summary>
    /// Cierra sesión de juego del jugador
    /// </summary>
    /// <param name="view"></param>
    public void LogOut(bool view = false)
    {
        WebProcedure.Instance.LogOut();
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Caching.ClearCache();
        Firebase.Messaging.FirebaseMessaging.DeleteTokenAsync().ContinueWithOnMainThread(
            task =>
            {
                Debug.Log( "DeleteTokenAsync");
            });
        mainMenuPanel.isFirstTimeLoading = true;
        
        if (!view)
        {
            authenticationPanel.OpenPopup(); 
        }
        else
        {
            ViewPanel.OpenPopup();
        }

        mainMenuPanel.PlacerHolder.gameObject.SetActive(true);
    }
    
    /// <summary>
    /// Alerta que se muestra cuando no se tiene información del usuario
    /// </summary>
    public void NoUserInfo()
    {
        AlertPanel.SetupPanel("Lo sentimos. Estás tratando de acceder a una funcionalidad reservada para usuarios registrados. Regístrate y disfruta de una experiencia plena en Distrito acb."
            , "", true,
            ()=>{Instance.LogOut(true);}, null, 0,"Ir al Registro","No, gracias");
    }
#endregion
}
