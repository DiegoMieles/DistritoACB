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
public class PanelPotenciadores : Panel
{
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
    [SerializeField] [Tooltip("Texto de título")]
    private Text  textNoPotenciadores;
    [SerializeField,TextArea] [Tooltip("Texto que se muestra cuando la traida de datos es fallida")]
    private string textFail;

    [SerializeField] [Tooltip("Contenedor de los potenciadores")]
    private BoosterContainer boosterDataContainer = new BoosterContainer();
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

    private bool isCardInTeam; //Determina si una carta se encuentra en el equipo competitivo

    /// <summary>
    /// Llama a los datos de los potenciadores al mostrarse por primera vez el panel
    /// </summary>
    private void OnEnable()
    {
        if (!isCardPotenciador)
        {
            CallInfo();
        }
    }

    /// <summary>
    /// Trae los datos de los potenciadores de backend
    /// </summary>
    /// <param name="boostertype">Tipo de potenciador</param>
    /// <param name="playerCard">Potenciador de la carta</param>
    /// <param name="isCardInTeam">Determina si la carta se encuentra en el equipo competitivo</param>
    public void CallInfo(BoosterType boostertype, PlayerCard playerCard, bool isCardInTeam)
    {
        var json = JsonConvert.SerializeObject(new BoosterBody()
        {
            type = boostertype
        });

        WebProcedure.Instance.GetUserBoosterByType(json ,snapshot =>
        {
            boosterDataContainer?.boosterData?.boosterItems?.Clear();
            applyBoosterResponse?.boosterData?.boosterItems?.Clear();
            JsonConvert.PopulateObject(snapshot.RawJson, boosterDataContainer);
            Debug.Log(snapshot.RawJson);

            this.isCardInTeam = isCardInTeam;

            CheckPotenciadores();
            if (boosterDataContainer.boosterData.boosterItems != null)
            {
                foreach (var boosterdata in boosterDataContainer.boosterData.boosterItems)
                {
                    
                    switch (boosterdata.type)
                    {
                        case BoosterType.TRIPLES:
                            var prefab1= Instantiate(panelPotenciador, gridLayoutTriples.transform);
                            prefab1.ShowInfo(boosterdata, () =>
                            {
                                ShowDialogConfirmation(boosterdata, playerCard);
                            });
                            textTriples.gameObject.SetActive(true);
                            break;
                        case BoosterType.FREESHOTS:
                            var prefab2= Instantiate(panelPotenciador, gridLayoutTirosLibres.transform);
                            prefab2.ShowInfo(boosterdata,() =>
                            {
                                ShowDialogConfirmation(boosterdata, playerCard);
                            });
                            textLibres.gameObject.SetActive(true);
                            break;
                        case BoosterType.REBOUNDS:
                            var prefab3= Instantiate(panelPotenciador, gridLayoutRebotes.transform);
                            prefab3.ShowInfo(boosterdata,() =>
                            {
                                ShowDialogConfirmation(boosterdata, playerCard);
                            });
                            textRebotes.gameObject.SetActive(true);
                            break;
                        case BoosterType.ASSISTS:
                            var prefab4= Instantiate(panelPotenciador, gridLayoutAsistencia.transform);
                            prefab4.ShowInfo(boosterdata,() =>
                            {
                                ShowDialogConfirmation(boosterdata, playerCard);
                            });
                            textAsistencia.gameObject.SetActive(true);
                            break;
                        case BoosterType.POINTS:
                            var prefab5= Instantiate(panelPotenciador, gridLayoutPuntos.transform);
                            prefab5.ShowInfo(boosterdata,() =>
                            {
                                ShowDialogConfirmation(boosterdata, playerCard);
                            });
                            textPuntos.gameObject.SetActive(true);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            ClosedSpinner();
        }, error =>
        {
            onFailed.Invoke();
        });
    }

    /// <summary>
    /// Muestra el panel de confirmación de aplicar potenciador
    /// </summary>
    /// <param name="boosterdata">Datos del potenciado</param>
    /// <param name="playerCard">Carta a la que se le aplica el potenciador</param>
    public void ShowDialogConfirmation(BoosterData.BoosterItemData boosterdata, PlayerCard playerCard)
    {
        panelOpener.popupPrefab = alertBoostPanelPrefab;
        panelOpener.OpenPopup();
        panelOpener.popup.GetComponent<PanelAlertPotenciador>().ShowAlert(boosterdata, playerCard, isCardInTeam, ApplyBooster);
    }
    
    /// <summary>
    /// Trae los datos de los potenciadores que tiene el jugador
    /// </summary>
    private void CallInfo()
    {
        WebProcedure.Instance.GetGetBoosterUser(snapshot =>
        {
            boosterDataContainer?.boosterData?.boosterItems?.Clear();
            applyBoosterResponse?.boosterData?.boosterItems?.Clear();
            Debug.Log(snapshot.RawJson);
            JsonConvert.PopulateObject(snapshot.RawJson, boosterDataContainer);
            CheckPotenciadores();
            if (boosterDataContainer.boosterData.boosterItems != null)
            {
                foreach (var boosterdata in boosterDataContainer.boosterData.boosterItems)
                {
                    switch (boosterdata.type)
                    {
                        case BoosterType.TRIPLES:
                            var prefab1= Instantiate(panelPotenciador, gridLayoutTriples.transform);
                            prefab1.ShowInfo(boosterdata);
                            textTriples.gameObject.SetActive(true);
                            break;
                        case BoosterType.FREESHOTS:
                            var prefab2= Instantiate(panelPotenciador, gridLayoutTirosLibres.transform);
                            prefab2.ShowInfo(boosterdata);
                            textLibres.gameObject.SetActive(true);
                            break;
                        case BoosterType.REBOUNDS:
                            var prefab3= Instantiate(panelPotenciador, gridLayoutRebotes.transform);
                            prefab3.ShowInfo(boosterdata);
                            textRebotes.gameObject.SetActive(true);
                            break;
                        case BoosterType.ASSISTS:
                            var prefab4= Instantiate(panelPotenciador, gridLayoutAsistencia.transform);
                            prefab4.ShowInfo(boosterdata);
                            textAsistencia.gameObject.SetActive(true);
                            break;
                        case BoosterType.POINTS:
                            var prefab5= Instantiate(panelPotenciador, gridLayoutPuntos.transform);
                            prefab5.ShowInfo(boosterdata);
                            textPuntos.gameObject.SetActive(true);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                } 
            }
            
            ClosedSpinner();

        }, error =>
        {
            onFailed.Invoke();
            ClosedSpinner();
        });
    }

    /// <summary>
    /// Aplica el potenciador a la carta
    /// </summary>
    /// <param name="uuidBooster">id del potenciador</param>
    /// <param name="playerCard">Carta del jugador</param>
    /// <param name="isInTeam">Determina si la carta está en el equipo competitivo</param>
    public void ApplyBooster(string uuidBooster, PlayerCard playerCard, bool isInTeam)
    {
        Debug.Log(uuidBooster);
        var json = JsonConvert.SerializeObject(new ApplyBoosterBody()
        {
            uuidBooster = uuidBooster,
            uuidTokenCard = string.IsNullOrEmpty(playerCard.TokenData.tokencard_token) ? playerCard.TokenData.token : playerCard.TokenData.tokencard_token,
        });
        
        WebProcedure.Instance.PostApplyUserBoosterToTokenCard(json ,snapshot =>
        {
            boosterDataContainer.boosterData.boosterItems.Clear();
            applyBoosterResponse.boosterData.boosterItems.Clear();
            JsonConvert.PopulateObject(snapshot.RawJson, applyBoosterResponse);
            Debug.Log(snapshot.RawJson);
          
            ResetBoosters();
            CheckPotenciadores();
            
            if (applyBoosterResponse.boosterData.boosterItems != null)
            {
                foreach (var boosterdata in applyBoosterResponse.boosterData.boosterItems)
                {
                    switch (boosterdata.type)
                    {
                        case BoosterType.TRIPLES:
                            var prefab1= Instantiate(panelPotenciador, gridLayoutTriples.transform);
                            prefab1.ShowInfo(boosterdata,() =>
                            {
                                ShowDialogConfirmation(boosterdata, playerCard);
                            });
                            textTriples.gameObject.SetActive(true);
                            break;
                        case BoosterType.FREESHOTS:
                            var prefab2= Instantiate(panelPotenciador, gridLayoutTirosLibres.transform);
                            prefab2.ShowInfo(boosterdata,() =>
                            {
                                ShowDialogConfirmation(boosterdata, playerCard);
                            });
                            textLibres.gameObject.SetActive(true);
                            break;
                        case BoosterType.REBOUNDS:
                            var prefab3= Instantiate(panelPotenciador, gridLayoutRebotes.transform);
                            prefab3.ShowInfo(boosterdata,() =>
                            {
                                ShowDialogConfirmation(boosterdata, playerCard);
                            });
                            textRebotes.gameObject.SetActive(true);
                            break;
                        case BoosterType.ASSISTS:
                            var prefab4= Instantiate(panelPotenciador, gridLayoutAsistencia.transform);
                            prefab4.ShowInfo(boosterdata,() =>
                            {
                                ShowDialogConfirmation(boosterdata, playerCard);
                            });
                            textAsistencia.gameObject.SetActive(true);
                            break;
                        case BoosterType.POINTS:
                            var prefab5= Instantiate(panelPotenciador, gridLayoutPuntos.transform);
                            prefab5.ShowInfo(boosterdata,() =>
                            {
                                ShowDialogConfirmation(boosterdata, playerCard);
                            });
                            textPuntos.gameObject.SetActive(true);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                } 
            }

            ClosedSpinner();
            playerCard.OnBoosterAdded?.Invoke();

            if (isInTeam)
                playerCard.SetupCardData(applyBoosterResponse.tokenData.tokenItems.FirstOrDefault(), null, true,true,false,false);
            else
                playerCard.SetupCardData(applyBoosterResponse.tokenData.tokenItems.FirstOrDefault(), null, true,false,false,false);

            panelOpener.popup.GetComponent<Panel>().Close();
            Close();
        }, error =>
        {
            onFailed.Invoke();
            ClosedSpinner();
        });
    }

    /// <summary>
    /// Resetea datos y elementos visuales de los potenciadores
    /// </summary>
    private void ResetBoosters()
    {
        textTriples.gameObject.SetActive(false);
        textLibres.gameObject.SetActive(false);
        textRebotes.gameObject.SetActive(false);
        textAsistencia.gameObject.SetActive(false);
        textPuntos.gameObject.SetActive(false);
            
        foreach(Transform b in gridLayoutTriples.transform)
        {
            Destroy(b.gameObject);
        }
            
        foreach(Transform b in gridLayoutTirosLibres.transform)
        {
            Destroy(b.gameObject);
        }
            
        foreach(Transform b in gridLayoutRebotes.transform)
        {
            Destroy(b.gameObject);
        }
        
        foreach(Transform b in gridLayoutAsistencia.transform)
        {
            Destroy(b.gameObject);
        }
            
        foreach(Transform b in gridLayoutPuntos.transform)
        {
            Destroy(b.gameObject);
        }
    }

    /// <summary>
    /// Realiza verificación de los potenciadores
    /// </summary>
    private void CheckPotenciadores()
    {
        if (boosterDataContainer?.boosterData?.boosterItems?.Count == 0 && applyBoosterResponse?.boosterData?.boosterItems?.Count == 0 )
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
    /// Activa corrutina que retrasa la desactivación del spinner
    /// </summary>
    public void SpinnerClosing()
    {
        StartCoroutine(CloseWait());
    }

    /// <summary>
    /// Corrutina que retrasa la desactivación del spinner
    /// </summary>
    /// <returns></returns>
    IEnumerator CloseWait()
    {
        yield return new WaitForSeconds(3.0f);
        ClosedSpinner();
    }

}
