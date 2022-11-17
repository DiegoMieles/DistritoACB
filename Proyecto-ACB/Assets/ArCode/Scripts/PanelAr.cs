using Data;
using WebAPI;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.Collections;

/// <summary>
/// Controla lo que se muestra en el panel de AR
/// </summary>
public class PanelAr : Panel
{
    [Header("Panel AR Components")]
    [SerializeField] [Tooltip("C?mara donde se renderizan los elementos de AR y de lo mostrado por la c?mara")]
    private Camera ARCamera;
    [SerializeField] [Tooltip("Elemento AR en forma de carta que muestra el tipo de recompensa que se va a obtener")]
    private GameObject arCard;
    [SerializeField] [Tooltip("Imagen AR de recompensa ACBall")]
    private GameObject acballModel;
    [SerializeField] [Tooltip("Imagen AR de recompensa monedas")]
    private GameObject coinModel;
    [SerializeField] [Tooltip("Imagen AR de recompensa skin")]
    private GameObject skinModel;
    [SerializeField] [Tooltip("Imagen AR de recompensa carta de jugador")]
    private GameObject tokenModel;
    [SerializeField] [Tooltip("Imagen AR de recompensa carta highlight")]
    private GameObject highlightModel;
    [SerializeField] [Tooltip("Imagen AR de recompensa potenciador")]
    private GameObject boosterModel;
    [SerializeField] [Tooltip("Bot?n que se encarga del cerrado del panel")]
    private Button closeButton;
    [SerializeField] [Tooltip("Spinner de carga del panel")]
    private GameObject spinner;
    private bool isARFake;
    [SerializeField]
    [Tooltip("texto del bot?n de fake ar")]
    private Text buttonFakeARText;

    private MissionsData.MissionItemData missionData; //Datos de la misi?n
    private bool checkRaycast; //Determina si se debe hacer raycasting a los objetos que se renderizan en c?mara

    #region Unity Methods

    /// <summary>
    /// Se ejecuta cuando el panel ha sido iniciado por primera vez en escena, desactivando el spinner de carga y 
    /// configurando el bot?n de cerrado del panel
    /// </summary>
    private void Start()
    {
        closeButton.onClick.AddListener(() => { ACBSingleton.Instance.SetActiveCamera(true); Close(); });
        spinner.SetActive(false);
    }

    /// <summary>
    /// Se ejecuta todo el tiempo (frame by frame), verifica si el jugador puede hacer raycasting a un objeto y
    /// si el jugador ha seleccionado el premio
    /// </summary>
    private void Update()
    {
        if (checkRaycast)
            return;

        
        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
        {
            Debug.LogError("Touch");
            Ray ray = ARCamera.ScreenPointToRay(Input.touches[0].position);
            RaycastHit hit = default;

            Debug.LogError("fuera 1: " + hit.collider?.name);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                Debug.DrawRay(ray.origin, ray.direction * 20, Color.green);
                Debug.LogError("fuera 2: " + hit.collider.name);
                if (hit.collider != null)
                {
                    Debug.LogError("fuera 3: " + hit.collider.name);
                    Destroy(hit.collider.gameObject);
                    MissionBody scan = new MissionBody()
                    {
                        mission_id = missionData.id,
                        uuid_r = missionData.uuid_r,
                    };
                    var json = JsonConvert.SerializeObject(scan);
                    spinner.SetActive(true);
                    WebProcedure.Instance.PostSaveMissionComplete(json, OnSuccessMissionCompleting, OnFailedMissionCompleting);
                    checkRaycast = true;
                }
            }
        }
    
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Determina el premio que se debe mostrar en el panel para que el jugador pueda recolectarlo
    /// </summary>
    /// <param name="missionData">Datos de la misi?n AR traidos desde backend previamente</param>
    /// <param name="isARSupported">Determina si el dispositivo donde se est? jugando soporta tecnolog?a AR</param>
    public void SetAndShowReward(MissionsData.MissionItemData missionData, bool isARSupported)
    {
        checkRaycast = false;
        this.missionData = missionData;

        if (!isARSupported)
        {
            arCard.transform.SetParent(gameObject.transform);
        }
        

        skinModel.SetActive(false);
        tokenModel.SetActive(false);
        highlightModel.SetActive(false);
        boosterModel.SetActive(false);
        coinModel.SetActive(false);
        acballModel.SetActive(false);

        GameObject objectToShow = new GameObject();

        switch (missionData.rewardType)
        {
            case ItemType.SKIN:
                objectToShow = skinModel;
                break;

            case ItemType.ACBALL:
                objectToShow = acballModel;
                break;

            case ItemType.BOOSTER:
                objectToShow = boosterModel;
                break;

            case ItemType.COINS:
                objectToShow = coinModel;
                break;

            case ItemType.TOKEN:
                objectToShow = tokenModel;
                break;

            case ItemType.HIGTHLIGHT:
                objectToShow = highlightModel;
                break;
        }

        
        StartCoroutine(WaitForTurnOnCoin(objectToShow));
        if(!isARSupported)
            objectToShow.GetComponent<MeshRenderer>().enabled = true;

        ARCamera.gameObject.SetActive(true);
    }
    
    #endregion

    IEnumerator WaitForTurnOnCoin(GameObject objectToShow)
    {
        yield return new WaitForSeconds(1.5f);
        objectToShow.SetActive(true);
    }
    #region Inner Methods

    /// <summary>
    /// M?todo que se ejecuta cuando backend no puede determinar si la misi?n ha sido terminada satisfactoriamente
    /// </summary>
    /// <param name="obj">Clase con los datos de error de la misi?n completada</param>
    private void OnFailedMissionCompleting(WebError obj)
    {
        ARCamera.gameObject.SetActive(false);
        ACBSingleton.Instance.AlertPanel.SetupPanel("Hubo un error, por favor intenta nuevamente", "", false, Close);
    }

    /// <summary>
    /// M?todo que se ejecuta cuando backend determina si una misi?n ha sido completada satisfactoriamente
    /// y manda al jugador al panel de recompensa de la misi?n
    /// </summary>
    /// <param name="obj"></param>
    private void OnSuccessMissionCompleting(DataSnapshot obj)
    {
        var cached = JsonConvert.DeserializeObject<MissionRewardData>(obj.RawJson);
        ARCamera.gameObject.SetActive(false);
        Debug.Log(obj.RawJson);
        
        if (cached.code == 200)
        {
            Firebase.Analytics.FirebaseAnalytics.LogEvent("mission_ok");
            Debug.Log("Analytic mission_ok logged");

            JsonConvert.PopulateObject(obj.RawJson, ACBSingleton.Instance.AccountData);
            ACBSingleton.Instance.RewardPanel.SetMissionRewardToOpen(cached, null);
            ACBSingleton.Instance.SetActiveCamera(true);
            Close();
        }
        else
        {
            ACBSingleton.Instance.SetActiveCamera(true);
            Close();
            ACBSingleton.Instance.AlertPanel.SetupPanel(cached.message, "", false, null);
        }
    }
    public void EnableDisableFakeAR()
    {
        isARFake = !isARFake;
        SetAndShowReward(missionData, !isARFake);
        buttonFakeARText.text = isARFake ? "Deshabilitar Fake AR" : "Habilitar Fake AR";
    }

    #endregion

}
