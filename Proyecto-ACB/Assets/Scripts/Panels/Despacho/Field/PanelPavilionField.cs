using Data;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using WebAPI;

/// <summary>
/// Controla el panel de desafio en la cancha del pabell�n
/// </summary>
public class PanelPavilionField : Panel
{
    #region Fields and properties

    [Header("Panel components")]
    [SerializeField] [Tooltip("Bot�n que se encarga del cerrado del panel")]
    private Button goBackButton;
    [SerializeField] [Tooltip("Contenedor de los desafios de la cancha")]
    private RectTransform fieldDataContainer;
    [SerializeField] [Tooltip("Prefab del bot�n donde se muestran los desafios completados por el jugador")]
    private GameObject playerFieldPrefab;
    [SerializeField] [Tooltip("Acci�n que se ejecuta al no poder traer los datos de desafio desde backend")]
    private UnityEvent onFailed;
    [SerializeField] [Tooltip("Datos de los desafios")]
    private ChallengesField challengesFields = new ChallengesField();
    [SerializeField] [Tooltip("Texto que se muestra cuando el jugador no ha realizado desafios")]
    private Text  textNoChallenges;
    [SerializeField,TextArea] [Tooltip("Texto que se muestra cuando el jugador no ha realizado desafios")]
    private string textFail;
    [SerializeField]
    [Tooltip("bot�n de la liga actual")]
    private Button actualLeagueButton;
    [SerializeField]
    [Tooltip("bot�n de la liga cl�sica")]
    private Button clasicLeagueButton;
    [Tooltip("se est� mostrando la liga cl�sica? ")]
    private bool isclasicLeague;
    #endregion

    #region Unity Methods

    /// <summary>
    /// Al crear el panel dentro del juego carga los datos de los desafios
    /// </summary>
    private void OnEnable()
    {
        SwitchLEague(false);
        goBackButton.onClick.AddListener(() => { ACBSingleton.Instance.PanelBuildingSelection.ResetCachedMapData(); Close(); });

    }

    #endregion

    #region Inner Methods
    /// <summary>
    /// Cambia la liga que se va a mostraar
    /// </summary>
    public void SwitchLEague(bool isClasic)
    {
        challengesFields.challengeData.challengeItems.Clear();
        foreach (Transform child in fieldDataContainer)
        {
            Destroy(child.gameObject);
        }
        isclasicLeague = isClasic;
        if (isclasicLeague) UpdatePavilionViewClasicLeague(); else UpdatePavilionViewActualLeague();
        clasicLeagueButton.image.color = isclasicLeague ? new Color(1f, 1f, 1f, 1f) : new Color(1f, 1f, 1f, 0.5f);
        actualLeagueButton.image.color = !isclasicLeague ? new Color(1f, 1f, 1f, 1f) : new Color(1f, 1f, 1f, 0.5f);
    }

    /// <summary>
    /// Carga los datos de desafios llamando a backend de la liga actual
    /// </summary>
    private void UpdatePavilionViewActualLeague()
    {
     
        WebProcedure.Instance.GetChallengesCancha(OnSuccess, OnFailed);
    }

    /// <summary>
    /// Carga los datos de desafios llamando a backend de la liga cl�sica
    /// </summary>
    private void UpdatePavilionViewClasicLeague()
    {
        WebProcedure.Instance.GetChallengesCancha(OnSuccess, OnFailed);
    }
    /// <summary>
    /// M�todo que se ejecuta cuando los desafios han sido correctamente cargados desde backend
    /// </summary>
    /// <param name="obj">Datos de los desafios</param>
    private void OnSuccess(DataSnapshot obj)
    {
        Debug.Log(obj.RawJson);
        JsonConvert.PopulateObject(obj.RawJson, challengesFields);
        CheckChallenges();
        if (challengesFields.challengeData.challengeItems != null && challengesFields.challengeData.challengeItems.Count > 0)
        {
            fieldDataContainer.sizeDelta = new Vector2(fieldDataContainer.sizeDelta.x, playerFieldPrefab.GetComponent<LayoutElement>().preferredHeight * challengesFields.challengeData.challengeItems.Count);
            foreach (var challengeData in challengesFields.challengeData.challengeItems)
            {
                GameObject prefab = Instantiate(playerFieldPrefab, fieldDataContainer.transform);
                prefab.GetComponent<ChallengeFieldButton>().SetupChallengeButton(challengeData);
            }
        }
        else
        {
            CloseSpinner();
        }
    }

    /// <summary>
    /// Desactiva el spinner de carga
    /// </summary>
    private void CloseSpinner()
    {
        GameObject spinner = GameObject.Find("Spinner_cancha");
        for(int i=0; i<spinner.transform.childCount; i++)
        {
            spinner.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// M�todo que se ejecuta cuando los desafios no han sido correctamente cargados desde backend
    /// </summary>
    /// <param name="obj">Clase con los datos del error</param>
    private void OnFailed(WebError obj)
    {
        onFailed?.Invoke();
        CloseSpinner();
    }

    /// <summary>
    /// Verifica que el jugador tenga desafios hechos previamente
    /// </summary>
    private void CheckChallenges()
    {
        if (challengesFields?.challengeData?.challengeItems?.Count == 0 )
        {
            textNoChallenges.text = textFail;
            CloseSpinner();
        }
        else
        {
            textNoChallenges.text = string.Empty;   
        }
    }

    
    #endregion

}
