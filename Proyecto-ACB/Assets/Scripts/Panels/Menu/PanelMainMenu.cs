using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using WebAPI;
using Newtonsoft.Json;
using Data;
using Firebase.Extensions;
using UnityEngine.UI;

#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif

/// <summary>
/// Controla el mapa prinicipal del juego y sus datos
/// </summary>
public class PanelMainMenu : Panel
{
    [Header("Gameplay components")]
    [SerializeField] [Tooltip("Texto de versi�n del juego")]
    private Text textVersion;
    [SerializeField] [Tooltip("Objeto de pantalla de bienvenida del juego")]
    private GameObject placerHolder;
    [SerializeField] [Tooltip("Mapa principal del juego (La ciudad)")]
    private GameObject mainMap;
    [SerializeField] [Tooltip("Controlador de la vista del mundo y del zoom")]
    private PinchableScrollRect worldViewController;
    [SerializeField] [Tooltip("Controlador de los edificios disponibles en la ciudad")]
    private BuildingsManager worldBuildingsManager;
    [SerializeField] [Tooltip("Controlador de los edificios disponibles en el pabell�n")]
    private BuildingsManager pavilionBuildingsManager;
    [SerializeField] [Tooltip("Controlador de los edificios disponibles en el Auditorio")]
    private BuildingsManager auditoryBuildingsManager;
    [SerializeField] [Tooltip("Controlador general de las luces del juego")]
    private LightsCircadianCycle lightsController;
    [Header("EVENTS")]
    [SerializeField] [Tooltip("Evento que se ejecuta cuando backend no puede devolver los datos principales de juego")]
    private UnityEvent onFailedGameDataLoad;
    [SerializeField] [Tooltip("Evento que se ejecuta cuando backend no puede devolver los datos principales del jugador")]
    private UnityEvent onFailedAccountDataLoad;
     
    [SerializeField] [Tooltip("Lista de misiones disponibles en el juego")]
    private List<MissionButton> missionBtn;

    public bool isFirstTimeLoading = true; //Determina si es la primera vez que el jugador abre el juego
    public GameObject PlacerHolder => placerHolder;

    #region Unity Methods

    /// <summary>
    /// Se ejecuta cuando se muestra el mapa principal, obteniendo la versi�n de la aplicaci�n y las coordenadas del jugador
    /// </summary>
    private void OnEnable()
    {
        textVersion.text = "v" + Application.version;
        StartCoroutine(GetCoordinates());
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Carga los datos de la cuenta del jugador
    /// </summary>
    public void LoadPlayerAccountData()
    {
         placerHolder.gameObject.SetActive(true);
#if PLATFORM_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Permission.RequestUserPermission(Permission.Camera);
        }
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
        }
        if (!Permission.HasUserAuthorizedPermission(Permission.CoarseLocation))
        {
            Permission.RequestUserPermission(Permission.CoarseLocation);
        }
#endif
        
        ACBSingleton.Instance.PanelBuildingSelection.gameObject.SetActive(false);

        if(!isFirstTimeLoading)
            ACBSingleton.Instance.ActivateMainSpinner(true);
              
        Firebase.Messaging.FirebaseMessaging.GetTokenAsync().ContinueWithOnMainThread(
            task => {
                var  token = task.Result;
                Debug.Log("Creado TOken: "+token);
                WebProcedure.Instance.GetLoadUserData(token,snapshot =>
                {
                    ACBSingleton.Instance.AccountData.Clear();
                    Debug.Log(snapshot.RawJson);
                    JsonConvert.PopulateObject(snapshot.RawJson, ACBSingleton.Instance.AccountData);
                    LoadGameData();
                     placerHolder.gameObject.SetActive(false);
                }, error =>
                {
                    onFailedAccountDataLoad?.Invoke();
                    placerHolder.gameObject.SetActive(true);
                    ACBSingleton.Instance.ActivateMainSpinner(false);
                    ACBSingleton.Instance.AlertPanel.SetupPanel(ACBSingleton.Instance.LostConnectionTextError, "", false, LoadPlayerAccountData);
                });

            }
        );
        
 
    }

    /// <summary>
    /// Carga los datos principales del juego a nivel general
    /// </summary>
    public void LoadGameData()
    {
         placerHolder.gameObject.SetActive(true);
        if (!isFirstTimeLoading)
            ACBSingleton.Instance.ActivateMainSpinner(true);
        
        WebProcedure.Instance.GetLoadGameData(snapshot =>
        {
            ACBSingleton.Instance.GameData.Clear();

            JsonConvert.PopulateObject(snapshot.RawJson, ACBSingleton.Instance.GameData);

            string v1 = Application.version;

#if UNITY_ANDROID
            string v2 =ACBSingleton.Instance.GameData.app.vers_android;
#else
      string v2 = ACBSingleton.Instance.GameData.app.vers_ios;
#endif
            var version1 = new Version(v1);
            var version2 = new Version(v2);

            var result = version1.CompareTo(version2);
            if (result > 0)
            {
                Console.WriteLine("version1 is greater"); 
            }
            else if (result < 0)
            {
                ACBSingleton.Instance.AlertPanel.SetupPanel(ACBSingleton.Instance.GameData.app.text, "", false, () =>
                {
                    Application.Quit();
#if UNITY_ANDROID
                    Application.OpenURL(ACBSingleton.Instance.GameData.app.url_android);
#else
                    Application.OpenURL(ACBSingleton.Instance.GameData.app.url_ios);
#endif
                });
                Console.WriteLine("version2 is greater");
                return;
            }
     
            else
            {
                Console.WriteLine("versions are equal");
            }
          
            
            if (isFirstTimeLoading)
                worldViewController.SetupWorldView(ACBSingleton.Instance.GameData.mainMenuData.transitionTime, lightsController);
            
            worldBuildingsManager.SetupBuildings(ACBSingleton.Instance.GameData.mainMenuData.transitionTime == TransitionTime.NIGHT, ACBSingleton.Instance.GameData.mainMenuData.buildInfoData);
            pavilionBuildingsManager.SetupBuildings(false, ACBSingleton.Instance.GameData.mainMenuData.buildInfoData);
            auditoryBuildingsManager.SetupBuildings(false, ACBSingleton.Instance.GameData.mainMenuData.buildInfoData);

            ACBSingleton.Instance.PanelBuildingSelection.OpenFirstTimeAvatarPanel(ACBSingleton.Instance.AccountData.avatarData.isFirstTime);

            isFirstTimeLoading = false;

            CheckMission();
            
            ACBSingleton.Instance.ScriptablesAreLoaded = true;
            
            mainMap.SetActive(true);
            placerHolder.gameObject.SetActive(false);
            
            ACBSingleton.Instance.ActivateMainSpinner(false);
        }, error =>
        {
            onFailedGameDataLoad?.Invoke();
            placerHolder.gameObject.SetActive(true);
            ACBSingleton.Instance.ActivateMainSpinner(false);
            ACBSingleton.Instance.AlertPanel.SetupPanel(ACBSingleton.Instance.LostConnectionTextError, "", false,LoadGameData);
        });
    }
    
    #endregion

    #region Inner Methods

    /// <summary>
    /// Revisa que misiones se encuentran disponibles para el jugador y asigna los datos de misi�n a una marquesina
    /// </summary>
    private void CheckMission()
    {
        missionBtn.ForEach(mission=> mission.gameObject.SetActive(false));
        
        missionBtn.ForEach(b =>
        {
            MissionsData.MissionItemData missionItemData = ACBSingleton.Instance.AccountData.missionsData?.missionItems?.Find(bb => bb.position == b.Position);

            if (missionItemData != null)
            {
                b.Setup(missionItemData);
            }
        });
    }

    /// <summary>
    /// Corrutina que carga las coordenadas del jugador
    /// </summary>
    /// <returns></returns>
    private IEnumerator GetCoordinates()
    {
        // Check if the user has location service enabled.
        if (!Input.location.isEnabledByUser)
            yield break;

        // Starts the location service.
        Input.location.Start();

        // Waits until the location service initializes
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // If the service didn't initialize in 20 seconds this cancels location service use.
        if (maxWait < 1)
        {
            print("Timed out");
            yield break;
        }

        // If the connection failed this cancels location service use.
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            print("Unable to determine device location");
            yield break;
        }
        else
        {
            // If the connection succeeded, this retrieves the device's current location and displays it in the Console window.
            print("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
        }
    }

    #endregion

    #region Offline Debug

    /// <summary>
    /// Carga datos por defecto de los objetos donde se tiene la informaci�n de los datos de juego general
    /// y los datos del jugador
    /// </summary>
    public void LoadOfflineData()
    {
        if(isFirstTimeLoading)
            worldViewController.SetupWorldView(ACBSingleton.Instance.GameData.mainMenuData.transitionTime, lightsController);
        
        worldBuildingsManager.SetupBuildings(ACBSingleton.Instance.GameData.mainMenuData.transitionTime == TransitionTime.NIGHT, ACBSingleton.Instance.GameData.mainMenuData.buildInfoData);
        pavilionBuildingsManager.SetupBuildings(false, ACBSingleton.Instance.GameData.mainMenuData.buildInfoData);
        auditoryBuildingsManager.SetupBuildings(false, ACBSingleton.Instance.GameData.mainMenuData.buildInfoData);
        ACBSingleton.Instance.PanelBuildingSelection.OpenFirstTimeAvatarPanel(ACBSingleton.Instance.AccountData.avatarData.isFirstTime);
        mainMap.SetActive(true);
        CheckMission();
        ACBSingleton.Instance.ScriptablesAreLoaded = true;
    }

    #endregion
}
