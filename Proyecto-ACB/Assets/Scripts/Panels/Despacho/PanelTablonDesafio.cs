using Data;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using WebAPI;

namespace Panels
{
    /// <summary>
    /// Panel con los datos de los desafios disponibles
    /// </summary>
    public class PanelTablonDesafio : Panel
    {
        #region Fields and properties

        [Header("Panel components")]
        [SerializeField] [Tooltip("Contenedor de los desafios publicados")]
        private RectTransform challengeDataContainer;
        [SerializeField] [Tooltip("Prefab del objeto donde se muestra un desafio publicado")]
        private GameObject PaneltabloDesafioData;
        [SerializeField] [Tooltip("Evento que se ejeuta cuando los desafios no se han podido cargar")]
        private UnityEvent onFailed;
        [SerializeField] [Tooltip("Botón de creación de desafio")]
        private Button createChallengeButton;
        [SerializeField] [Tooltip("Botón de cerrar panel")]
        private Button closePanelButton;
        [SerializeField] [Tooltip("Datos con los desafios publicados")]
        private ChallengeContainer challengeContainer = new ChallengeContainer();
        [SerializeField]
        [Tooltip("botón de la liga actual")]
        private Button actualLeagueButton ;
        [SerializeField]
        [Tooltip("botón de la liga clásica")]
        private Button clasicLeagueButton;
        [Space(5)]
        [Header("Panel opener data")]
        [SerializeField] [Tooltip("Clase que controla la apertura de nuevos paneles a mostrar")]
        private PanelOpener panelOpener;
        [SerializeField] [Tooltip("Panel que muestra una alerta de equipo competitivo incompleto")]
        private GameObject challengeTeamImcompleteAlertPrefab;

        [Space(5)]
        [Header("Creation challenge alert texts")]
        [SerializeField] [Tooltip("Texto de creación de desafio que se muestra en el panel de alerta")]
        private string createChallengeText;
        [SerializeField] [Tooltip("Texto que se muestra en el panel de alerta cuando hay un error al crear un desafio")]
        private string CreationErrorText;

        [SerializeField] [Tooltip("Descripción de costo del desafio que se muestra en el panel de alerta")]
        private string mensaje;

        private List<GameObject> postedChallenges; //Lista de desafios publicados
        public bool isClasicLeague;

        #endregion

        #region Unity Methods

        /// <summary>
        /// Método que se ejecuta cuando se activa el panel, actualiza la información de los desafios
        /// </summary>
        private void OnEnable()
        {
            postedChallenges = new List<GameObject>();
            closePanelButton.onClick.AddListener(() => { ACBSingleton.Instance.PanelBuildingSelection.ResetCachedMapData(); Close(); });
            SwitchLeague(false);
            createChallengeButton.onClick.AddListener(CreateChallenge);
        }

        #endregion

        #region Private Methods
        /// <summary>
        /// Carga la liga seleccionada
        /// </summary>
        public void SwitchLeague(bool isClasic)
        {
            SetSpinnerState(true);
            isClasicLeague = isClasic;
            clasicLeagueButton.image.color = isClasicLeague ? new Color(1f,1f,1f,1f) : new Color(1f, 1f, 1f, 0.5f);
            clasicLeagueButton.interactable = !isClasicLeague;
            actualLeagueButton.image.color = isClasicLeague ? new Color(1f, 1f, 1f, 0.5f):new Color(1f, 1f, 1f, 1f) ;
            actualLeagueButton.interactable = isClasicLeague;
            if (isClasicLeague) CallInfoClasicLeague(); else CallInfoActualLeague();
        }
        /// <summary>
        /// Actualiza la información de los desafios que se van a mostrar en panel
        /// </summary>
        private void CallInfoActualLeague()
        {
            if (postedChallenges.Count > 0)
            {
                postedChallenges.ForEach(challenge => Destroy(challenge));
                postedChallenges.Clear();
            }

            postedChallenges = new List<GameObject>();
            challengeContainer = new ChallengeContainer();

            WebProcedure.Instance.GetChallengesTablon(snapshot =>
            {
                JsonConvert.PopulateObject(snapshot.RawJson, challengeContainer);
                Debug.Log(snapshot.RawJson);
                
                createChallengeButton.gameObject.SetActive(true);
                
                if (challengeContainer.challengeData.challengeItems != null && challengeContainer.challengeData.challengeItems.Count > 0)
                {
                    challengeDataContainer.sizeDelta = new Vector2(challengeDataContainer.sizeDelta.x, PaneltabloDesafioData.GetComponent<LayoutElement>().preferredHeight * challengeContainer.challengeData.challengeItems.Count);

                    foreach (var transactionData in challengeContainer.challengeData.challengeItems)
                    {
                        GameObject prefab = Instantiate(PaneltabloDesafioData, challengeDataContainer.transform);
                        prefab.GetComponent<PanelTabloDesafioData>().ShowInfo(transactionData, challengeContainer.challengeData.TeamComplete, Close, challengeContainer.challengeData.canAcceptChallenge, 
                            challengeContainer.challengeData.challengeFree,challengeContainer.challengeData.challengeFreeMessage,challengeContainer.challengeData.canAcceptChallengeMessage);
                        postedChallenges.Add(prefab);
                    } 
                }
                else
                {
                    SetSpinnerState(false);
                }

            }, error =>
            {
                onFailed.Invoke();
                SetSpinnerState(false);
            },!isClasicLeague);
        }


        /// <summary>
        /// Actualiza la información de los desafios que se van a mostrar en panel
        /// </summary>
        private void CallInfoClasicLeague()
        {
            if (postedChallenges.Count > 0)
            {
                postedChallenges.ForEach(challenge => Destroy(challenge));
                postedChallenges.Clear();
            }

            postedChallenges = new List<GameObject>();
            challengeContainer = new ChallengeContainer();

            WebProcedure.Instance.GetChallengesTablon(snapshot =>
            {
                JsonConvert.PopulateObject(snapshot.RawJson, challengeContainer);
                Debug.Log(snapshot.RawJson);

                createChallengeButton.gameObject.SetActive(true);

                if (challengeContainer.challengeData.challengeItems != null && challengeContainer.challengeData.challengeItems.Count > 0)
                {
                    challengeDataContainer.sizeDelta = new Vector2(challengeDataContainer.sizeDelta.x, PaneltabloDesafioData.GetComponent<LayoutElement>().preferredHeight * challengeContainer.challengeData.challengeItems.Count);

                    foreach (var transactionData in challengeContainer.challengeData.challengeItems)
                    {
                        GameObject prefab = Instantiate(PaneltabloDesafioData, challengeDataContainer.transform);
                        prefab.GetComponent<PanelTabloDesafioData>().ShowInfo(transactionData, challengeContainer.challengeData.TeamComplete, Close, challengeContainer.challengeData.canAcceptChallenge,
                            challengeContainer.challengeData.challengeFree, challengeContainer.challengeData.challengeFreeMessage, challengeContainer.challengeData.canAcceptChallengeMessage);
                        postedChallenges.Add(prefab);
                    }
                }
                else
                {
                    SetSpinnerState(false);
                }

            }, error =>
            {
                onFailed.Invoke();
                SetSpinnerState(false);
            }, !isClasicLeague);
        }
        /// <summary>
        /// Llama a la alerta de confirmación de creación de desafio
        /// </summary>
        private void CreateChallenge()
        {
            
            if (challengeContainer.challengeData.TeamComplete)
            {
                if (challengeContainer.challengeData.canPostChallenge)
                {
                    if (challengeContainer.challengeData.challengeFree)
                    {
                        byte[] tempBytes;
                        tempBytes = System.Text.Encoding.Default.GetBytes(mensaje);
                        string message = System.Text.Encoding.UTF8.GetString(tempBytes);
                        ACBSingleton.Instance.AlertPanel.SetupPanel(createChallengeText, challengeContainer.challengeData.challengeFreeMessage, true, CreateAlertChallenge);
                    }
                    else
                    {
                        byte[] tempBytes;
                        tempBytes = System.Text.Encoding.Default.GetBytes(mensaje);
                        string message= System.Text.Encoding.UTF8.GetString(tempBytes);
                        ACBSingleton.Instance.AlertPanel.SetupPanel(createChallengeText, message + ACBSingleton.Instance.GameData.costChallenge + " acbCoins", true, CreateAlertChallenge);
                    }
                  
                }
                else
                {
                    panelOpener.popupPrefab = challengeTeamImcompleteAlertPrefab;
                    panelOpener.OpenPopup();
                    panelOpener.popup.GetComponent<ChallengeIncompleteTeamPanel>().OpenAlertNoCanPostChallenge(challengeContainer.challengeData.canPostChallengeMessage);
                }
            }
            else
            {
                panelOpener.popupPrefab = challengeTeamImcompleteAlertPrefab;
                panelOpener.OpenPopup();
                panelOpener.popup.GetComponent<ChallengeIncompleteTeamPanel>().OpenAlert();
            }
        }

        /// <summary>
        /// Verifica los datos del desafio a crear
        /// </summary>
        private void CreateAlertChallenge()
        {
            Debug.Log("Creating Challenge");
            createChallengeButton.gameObject.SetActive(false);
            SetSpinnerState(true);
            WebProcedure.Instance.PostCreateChallenge((obj) =>
            {
                OnSuccess(obj);
                Debug.Log("Challenge created");
                
            }, (error) =>
                ACBSingleton.Instance.AlertPanel.SetupPanel(CreationErrorText, "", false, null), !isClasicLeague); 

        }

        /// <summary>
        /// Método que se ejecuta cuando el desafio ha sido correctamente publicado
        /// </summary>
        /// <param name="obj">Datos del desafio publicado devueltos desde backend</param>
        private void OnSuccess(DataSnapshot obj)
        {
            Debug.Log(obj.RawJson);
            if (obj.Code == 0 || obj.Code == 200)
            {
                Firebase.Analytics.FirebaseAnalytics.LogEvent("deal_publish_ok");
                Debug.Log("Analytic deal_publish_ok logged");
                Debug.Log("Desafio");
                ACBSingleton.Instance.AlertPanel.SetupPanel(obj.MessageCustom, "", false, UpdateLeague, null, 0, "Volver");
            }
            else
                ACBSingleton.Instance.AlertPanel.SetupPanel(obj.MessageCustom, "", false, null, null, 0, "Volver");
                SetSpinnerState(false);
        }

        /// <summary>
        /// carga la liga seleccionada
        /// </summary>
        private void UpdateLeague()
        {
            if (isClasicLeague) CallInfoClasicLeague(); else CallInfoActualLeague();
        }
        /// <summary>
        /// Activa o desactiva el spinner de carga
        /// </summary>
        /// <param name="state">Estado de activación del spinner</param>
        private void SetSpinnerState(bool state)
        {
            GameObject spinner = GameObject.Find("Spinner_TablonDesafio");
            for (int i = 0; i < spinner.transform.childCount; i++)
            {
                spinner.transform.GetChild(i).gameObject.SetActive(state);
            }
        }

        #endregion
    }
}

