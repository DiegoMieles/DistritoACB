using System;
using System.Collections;
using Data;
using Firebase.Extensions;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using WebAPI;

/// <summary>
/// Clase de botón personalizado de reclamar monedas en la cancha callejera
/// </summary>
public class Claimbutton : MonoBehaviour
{
    [Header("Main components")]
    [SerializeField] [Tooltip("Referencia componente básico de botón")]
    private Button button;
    [SerializeField] [Tooltip("Texto que se encuentra dentro del botón")]
    private Text textGameObject;
    [SerializeField] [Tooltip("Imagen de barra de carga del botón")]
    private Image loadBarImage;
    [SerializeField] [Tooltip("Evento que se ejecuta cuando un botón ha sido clickeado")]
    private UnityEvent onClickedButton;
    [SerializeField] [Tooltip("Animador que controla la animación de las monedas al reclamarlas")]
    private Animator coinsAnimator;

    #region Unity Methods

    /// <summary>
    /// Se ejecuta cuando se muestra el objeto, activando la corrutina de revisión constante del reclamado de monedas
    /// </summary>
    private void OnEnable()
    {
        StartCoroutine(CheckMissionReward());
    }

    /// <summary>
    /// Se ejecuta cuando el botón ha sido iniciado por primera vez en escena, añade el evento de reclamar las monedas al botón de reclamo
    /// </summary>
    private void Start()
    {
        button.onClick.AddListener(ClaimObject);
    }

    #endregion

    #region Inner Methods

    /// <summary>
    /// Reclama las monedas generadas en la cancha callejera y actualiza las analíticas
    /// </summary>
    private void ClaimObject()
    {
        if (!WebProcedure.Instance.IsUserNull())
        {
            onClickedButton?.Invoke();
            button.interactable = false;
            ACBSingleton.Instance.ActivateMainSpinner(true);
        
            Firebase.Messaging.FirebaseMessaging.GetTokenAsync().ContinueWithOnMainThread(
                task => {
                    var  token = task.Result;
                    Debug.Log("Creado TOken: "+token);
                    WebProcedure.Instance.PostSaveCollectCoins(token,
                        snapshot =>
                        {
                            Debug.Log(snapshot.RawJson);
                            JsonConvert.PopulateObject(snapshot.RawJson, ACBSingleton.Instance.AccountData);
                            button.interactable = true;
                            coinsAnimator.SetTrigger("Collect");
                            Firebase.Analytics.FirebaseAnalytics.LogEvent("claim_court_ok");
                            Debug.Log("Analytic claim_court_ok logged");
                            StartCoroutine(CheckMissionReward());
                            ACBSingleton.Instance.ActivateMainSpinner(false);
                        }
                        , error =>
                        {
                            button.interactable = true;
                            ACBSingleton.Instance.AlertPanel.SetupPanel("Aún no tienes acbcoins disponibles, espera a que finalice el periodo de generación", "", false, null);
                            ACBSingleton.Instance.ActivateMainSpinner(false);
                        } );
                }
            );
        }
        else
        {
            ACBSingleton.Instance.NoUserInfo();
        }

    }

    /// <summary>
    /// Corrutina encargada de actualiza el estado de carga del botón para reclamar monedas
    /// </summary>
    /// <returns></returns>
    private IEnumerator CheckMissionReward()
    {
        
        yield return new WaitUntil(() => ACBSingleton.Instance.ScriptablesAreLoaded);

        bool dtsWithinASecOfEachOther = false;

        while(!dtsWithinASecOfEachOther)
        {
            StadiumData stadiumData = ACBSingleton.Instance.AccountData.stadiumData;

            DateTime nextcollecttime = DateTimeHelper.ParseUnixTimestamp(stadiumData.nextCollectTime);
            DateTime oldDate = nextcollecttime;
            DateTime newtimenow = DateTime.UtcNow.AddHours(ACBSingleton.Instance.GameData.hours);
            
            double diffInSeconds = (oldDate - newtimenow).TotalSeconds;
            double totalSeconds =  (oldDate- DateTimeHelper.ParseUnixTimestamp(ACBSingleton.Instance.AccountData.stadiumData.lastCollectTime)).TotalSeconds;

            double normalized = double.Parse(diffInSeconds.ToString("f0")) / double.Parse(totalSeconds.ToString("f0"));
            loadBarImage.fillAmount = (float) (1 - normalized);
            dtsWithinASecOfEachOther = newtimenow > oldDate;
            
            textGameObject.text = dtsWithinASecOfEachOther ? "Recoger": "Generando";
            textGameObject.color = dtsWithinASecOfEachOther ? Color.black : Color.white;
            button.interactable = dtsWithinASecOfEachOther;
            
            yield return new WaitForSeconds(1f);

        }
        
        if (ACBSingleton.Instance.AccountData.stadiumData.collectCoins)
        {
            textGameObject.text = "Recoger";
            loadBarImage.fillAmount = 1;
            textGameObject.color = Color.black;
            button.interactable = true;
        }
        
    }
    
    #endregion
}
