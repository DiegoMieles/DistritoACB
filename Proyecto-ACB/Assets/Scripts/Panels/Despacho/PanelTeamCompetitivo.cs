using Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
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
    [SerializeField]
    [Tooltip("borde de las cartas de la liga clásica")]
    private Sprite classicborderCard;
    [SerializeField]
    [Tooltip("borde de las cartas de la liga actual")]
    private Sprite actualborderCard;
    [SerializeField]
    [Tooltip("botón de la liga actual")]
    private Button actualLeagueButton;
    [SerializeField]
    [Tooltip("botón de la liga clásica")]
    private Button classicLeagueButton;
    [Tooltip("está mostrando la liga actual?")]
    private bool isActualLeague = true;

    public static Action<TokenContainer> OnDeleteOrAdd; //Acción que se encarga de añadir o eliminar una carta según corresponda
    public static Action OnClose; //Acción que se ejecuta al cerrar el panel

    /// <summary>
    /// Suscribe acción de cerrar panel del equipo competitivo y trae los datos del equipo competitivo
    /// </summary>
    private void OnEnable()
    {
        SwitchLeague(isActualLeague);
         OnDeleteOrAdd += (TokenContainer obj) => { if (isActualLeague) CallInfoActualLeague(obj); else CallInfoClassicLeague(obj); } ;
    }

    /// <summary>
    /// Desuscribe el evento de cerrar panel al destruirse el objeto
    /// </summary>
    private void OnDestroy()
    {
        OnDeleteOrAdd -= (TokenContainer obj) => { if (isActualLeague) CallInfoActualLeague(obj); else CallInfoClassicLeague(obj); };
    }
    //cambia la liga que se va a mostrar , clasica o actual
    public void SwitchLeague(bool searchActualLeague)
    {
        isActualLeague = searchActualLeague;
        if (isActualLeague) CallInfoActualLeague(); else CallInfoClassicLeague();
        actualLeagueButton.image.color = isActualLeague ? new Color(1,1,1,1): new Color(1, 1, 1,0.5f);
        classicLeagueButton.image.color = !isActualLeague ? new Color(1, 1, 1, 1) : new Color(1, 1, 1, 0.5f);
    }

    /// <summary>
    /// Trae la información del panel del equipo competitivo de la liga actual
    /// </summary>
    private void CallInfoActualLeague()
    {
        panelTokenItemTop.ForEach(t=>t.ResetToken());
        panelTokenItemButtom.ForEach(t=>t.ResetToken());
        panelTokenItemTop.ForEach(t=> t.booster.transform.parent.GetComponent<Image>().sprite = actualborderCard) ;
        panelTokenItemButtom.ForEach(t => t.booster.transform.parent.GetComponent<Image>().sprite = actualborderCard);
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
                    top?.ShowInfo(data, CallInfoActualLeague,spinner);
                    down?.ShowInfo(data, CallInfoActualLeague,spinner);
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
    /// Trae la información del panel del equipo competitivo de la liga actual
    /// </summary>
    /// <param name="team">Contenedor de cartas</param>
    private void CallInfoActualLeague(TokenContainer team)
    {
        panelTokenItemTop.ForEach(t=>t.ResetToken());
        panelTokenItemButtom.ForEach(t=>t.ResetToken());
        tokenContainer = team;
        panelTokenItemTop.ForEach(t => t.booster.transform.parent.GetComponent<Image>().sprite = actualborderCard);
        panelTokenItemButtom.ForEach(t => t.booster.transform.parent.GetComponent<Image>().sprite = actualborderCard);
        if (tokenContainer.teamData.teamItems != null && tokenContainer.teamData.teamItems.Count > 0)
        {
            for (var i = 0; i < tokenContainer.teamData.teamItems.Count; i++)
            {
                var top = panelTokenItemTop.ElementAtOrDefault(i);
                var down = panelTokenItemButtom.ElementAtOrDefault(i);
                var data =  tokenContainer.teamData.teamItems.ElementAtOrDefault(i);
                top?.ShowInfo(data, CallInfoActualLeague,spinner);
                down?.ShowInfo(data, CallInfoActualLeague,spinner);
            }
        }
        else
        {
            spinner.SetActive(false);
        }
    }

    /// <summary>
    /// Trae la información del panel del equipo competitivo de la liga clásica
    /// </summary>
    private void CallInfoClassicLeague()
    {
        panelTokenItemTop.ForEach(t => t.ResetToken());
        panelTokenItemButtom.ForEach(t => t.ResetToken());
        panelTokenItemTop.ForEach(t => t.booster.transform.parent.GetComponent<Image>().sprite = classicborderCard);
        panelTokenItemButtom.ForEach(t => t.booster.transform.parent.GetComponent<Image>().sprite = classicborderCard);
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
                    var data = tokenContainer.teamData.teamItems.ElementAtOrDefault(i);
                    top?.ShowInfo(data, CallInfoClassicLeague, spinner);
                    down?.ShowInfo(data, CallInfoClassicLeague, spinner);
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
    /// Trae la información del panel del equipo competitivo de la liga clásica
    /// </summary>
    /// <param name="team">Contenedor de cartas</param>
    private void CallInfoClassicLeague(TokenContainer team)
    {
        panelTokenItemTop.ForEach(t => t.ResetToken());
        panelTokenItemButtom.ForEach(t => t.ResetToken());
        tokenContainer = team;
        panelTokenItemTop.ForEach(t => t.booster.transform.parent.GetComponent<Image>().sprite = classicborderCard);
        panelTokenItemButtom.ForEach(t => t.booster.transform.parent.GetComponent<Image>().sprite = classicborderCard);
        if (tokenContainer.teamData.teamItems != null && tokenContainer.teamData.teamItems.Count > 0)
        {
            for (var i = 0; i < tokenContainer.teamData.teamItems.Count; i++)
            {
                var top = panelTokenItemTop.ElementAtOrDefault(i);
                var down = panelTokenItemButtom.ElementAtOrDefault(i);
                var data = tokenContainer.teamData.teamItems.ElementAtOrDefault(i);
                top?.ShowInfo(data, CallInfoClassicLeague, spinner);
                down?.ShowInfo(data, CallInfoClassicLeague, spinner);
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
