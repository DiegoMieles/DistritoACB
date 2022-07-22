using Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using WebAPI;

/// <summary>
/// Controla el panel que contiene los datos del equipo competitivo
/// </summary>
public class PanelTeamCompetitivo : Panel
{
    [SerializeField] [Tooltip("Lista de cartas en el equipo en la vista superior")]
    private List<PanelTokenItem> panelTokenItemTop;
    [SerializeField] [Tooltip("Lista de cartas en el equipo en la vista inferior")]
    private List<PanelTokenItem> panelTokenItemButtom;
    [SerializeField] [Tooltip("Se ejecuta cuando la traida de datos es fallida")]
    private UnityEvent onFailed;
    [SerializeField] [Tooltip("Contenedor de las cartas")]
    private TokenContainer tokenContainer = new TokenContainer();
    [SerializeField] [Tooltip("Spinner de carga")]
    private GameObject spinner;

    public static Action<TokenContainer> OnDeleteOrAdd; //Acción que se encarga de añadir o eliminar una carta según corresponda
    public static Action OnClose; //Acción que se ejecuta al cerrar el panel

    /// <summary>
    /// Suscribe acción de cerrar panel del equipo competitivo y trae los datos del equipo competitivo
    /// </summary>
    private void OnEnable()
    {
        CallInfo();
        OnDeleteOrAdd += CallInfo;
    }

    /// <summary>
    /// Desuscribe el evento de cerrar panel al destruirse el objeto
    /// </summary>
    private void OnDestroy()
    {
        OnDeleteOrAdd -= CallInfo;
    }

    /// <summary>
    /// Trae la información del panel del equipo competitivo
    /// </summary>
    private void CallInfo()
    {
        panelTokenItemTop.ForEach(t=>t.ResetToken());
        panelTokenItemButtom.ForEach(t=>t.ResetToken());
        
        WebProcedure.Instance.GetCurrentTeamCompetitiveUser(snapshot =>
        {
            Debug.Log("Cards states" + snapshot.RawJson);
            tokenContainer = new TokenContainer();
            JsonConvert.PopulateObject(snapshot.RawJson, tokenContainer);

            if (tokenContainer.teamData.teamItems != null && tokenContainer.teamData.teamItems.Count > 0)
            {
                for (var i = 0; i < tokenContainer.teamData.teamItems.Count; i++)
                {
                    var top = panelTokenItemTop.ElementAtOrDefault(i);
                    var down = panelTokenItemButtom.ElementAtOrDefault(i);
                    var data =  tokenContainer.teamData.teamItems.ElementAtOrDefault(i);
                    top?.ShowInfo(data, CallInfo,spinner);
                    down?.ShowInfo(data, CallInfo,spinner);
                }
            }
            else
            {
                spinner.SetActive(false);
            }
            
        }, error =>
        {
            onFailed?.Invoke();
            spinner.SetActive(false);
        });
    }

    /// <summary>
    /// Trae la información del panel del equipo competitivo
    /// </summary>
    /// <param name="team">Contenedor de cartas</param>
    private void CallInfo(TokenContainer team)
    {
        panelTokenItemTop.ForEach(t=>t.ResetToken());
        panelTokenItemButtom.ForEach(t=>t.ResetToken());
        tokenContainer = team;
        
        if (tokenContainer.teamData.teamItems != null && tokenContainer.teamData.teamItems.Count > 0)
        {
            for (var i = 0; i < tokenContainer.teamData.teamItems.Count; i++)
            {
                var top = panelTokenItemTop.ElementAtOrDefault(i);
                var down = panelTokenItemButtom.ElementAtOrDefault(i);
                var data =  tokenContainer.teamData.teamItems.ElementAtOrDefault(i);
                top?.ShowInfo(data, CallInfo,spinner);
                down?.ShowInfo(data, CallInfo,spinner);
            }
        }
        else
        {
            spinner.SetActive(false);
        }
    }

    /// <summary>
    /// Muestra las carta en el centro de la pantalla
    /// </summary>
    /// <param name="index">Índice de la carta</param>
    public void ShowPivot(int index)
    {
        panelTokenItemButtom.ForEach(i=> i.ShowPivot(false));
        panelTokenItemButtom[index].ShowPivot(true);
    }
    

}
