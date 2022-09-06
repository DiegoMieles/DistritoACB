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
    [SerializeField]
    [Tooltip("Lista de cartas en el equipo actual")]
    private Transform actualTokensLayout;
    [SerializeField]
    [Tooltip("Lista de cartas en el equipo cl?sico")]
    private Transform classicTokensLayout;
    [SerializeField] [Tooltip("Lista de cartas en el equipo en la vista superior")]
    private List<PanelTokenItem> panelTokenItemTop;
    [SerializeField] [Tooltip("Lista de cartas en el equipo en la vista inferior")]
    private List<PanelTokenItem> panelTokenItemButtom;
    [SerializeField]
    [Tooltip("Lista de cartas en el equipo en la vista superior de la liga cl?sica")]
    private List<PanelTokenItem> panelTokenclassicItemTop;
    [SerializeField]
    [Tooltip("Lista de cartas en el equipo en la vista inferior de la liga cl?sica")]
    private List<PanelTokenItem> panelTokenclassicItemButtom;
    [SerializeField] [Tooltip("Se ejecuta cuando la traida de datos es fallida")]
    private UnityEvent onFailed;
    [SerializeField] [Tooltip("Contenedor de las cartas")]
    private AllTokensContainer tokenContainer = new AllTokensContainer();
    [SerializeField] [Tooltip("Spinner de carga")]
    private GameObject spinner;
    [SerializeField]
    [Tooltip("borde de las cartas de la liga cl?sica")]
    private Sprite classicborderCard;
    [SerializeField]
    [Tooltip("borde de las cartas de la liga actual")]
    private Sprite actualborderCard;
    [SerializeField]
    [Tooltip("bot?n de la liga actual")]
    private Button actualLeagueButton;
    [SerializeField]
    [Tooltip("bot?n de la liga cl?sica")]
    private Button classicLeagueButton;
    [SerializeField]
    [Tooltip("Lista de cartas en el equipo actual")]
    private PanelOpener panelOpener;
    [Tooltip("est? mostrando la liga actual?")]
    public bool isActualLeague = true;
    [SerializeField]
    [Tooltip("Nombre del jugador en la liga actual")]
    private Text actualLeaguePlayerName;
    [SerializeField]
    [Tooltip("Nombre del jugador en la liga clasica")]
    private Text classicLeaguePlayerName;

    public static Action<AllTokensContainer> OnDeleteOrAdd; //Acci?n que se encarga de a?adir o eliminar una carta seg?n corresponda
    public static Action OnClose; //Acci?n que se ejecuta al cerrar el panel

    /// <summary>
    /// Suscribe acci?n de cerrar panel del equipo competitivo y trae los datos del equipo competitivo
    /// </summary>
    private void OnEnable()
    {
        SwitchLeague(isActualLeague);
         OnDeleteOrAdd += (AllTokensContainer obj) => {CallInfoActualLeague(obj); } ;
        CallInfoActualLeague();
    }

    /// <summary>
    /// Desuscribe el evento de cerrar panel al destruirse el objeto
    /// </summary>
    private void OnDestroy()
    {
        OnDeleteOrAdd -= (AllTokensContainer obj) => {CallInfoActualLeague(obj); };
    }
    //cambia la liga que se va a mostrar , clasica o actual
    public void SwitchLeague(bool searchActualLeague)
    {
        isActualLeague = searchActualLeague;
        classicTokensLayout.gameObject.SetActive(!isActualLeague);
        actualTokensLayout.gameObject.SetActive(isActualLeague);
        actualLeagueButton.image.color = isActualLeague ? new Color(1,1,1,1): new Color(1, 1, 1,0.5f);
        classicLeagueButton.image.color = !isActualLeague ? new Color(1, 1, 1, 1) : new Color(1, 1, 1, 0.5f);
    }

    /// <summary>
    /// Trae la informaci?n del panel del equipo competitivo de la liga actual
    /// </summary>
    private void CallInfoActualLeague()
    {
        spinner.SetActive(true);
        panelTokenItemTop.ForEach(t=>t.ResetToken());
        panelTokenItemButtom.ForEach(t=>t.ResetToken());
        panelTokenItemTop.ForEach(t=> t.booster.transform.parent.GetComponent<Image>().sprite = actualborderCard) ;
        panelTokenItemButtom.ForEach(t => t.booster.transform.parent.GetComponent<Image>().sprite = actualborderCard);        
        panelTokenclassicItemTop.ForEach(t=>t.ResetToken());
        panelTokenclassicItemButtom.ForEach(t=>t.ResetToken());
        panelTokenclassicItemTop.ForEach(t=> t.booster.transform.parent.GetComponent<Image>().sprite = classicborderCard) ;
        panelTokenclassicItemButtom.ForEach(t => t.booster.transform.parent.GetComponent<Image>().sprite = classicborderCard);
        WebProcedure.Instance.GetCurrentTeamCompetitiveUser(snapshot =>
        {
            Debug.Log("Cards states" + snapshot.RawJson);
            tokenContainer = new AllTokensContainer();
            JsonConvert.PopulateObject(snapshot.RawJson, tokenContainer);

                if (tokenContainer.current != null && tokenContainer.current.Count > 0)
                {
                    for (var i = 0; i < tokenContainer.current.Count; i++)
                    {
                        var top = panelTokenItemTop.ElementAtOrDefault(i);
                        var down = panelTokenItemButtom.ElementAtOrDefault(i);
                        var data = tokenContainer.current.ElementAtOrDefault(i);
                        top?.ShowInfo(data, CallInfoActualLeague, spinner);
                        down?.ShowInfo(data, CallInfoActualLeague, spinner);
                    }
                }            
                if (tokenContainer.classical != null && tokenContainer.classical.Count > 0)
                {
                    for (var i = 0; i < tokenContainer.classical.Count; i++)
                    {
                        var top = panelTokenclassicItemTop.ElementAtOrDefault(i);
                        var down = panelTokenclassicItemButtom.ElementAtOrDefault(i);
                        var data = tokenContainer.classical.ElementAtOrDefault(i);
                        top?.ShowInfo(data, CallInfoActualLeague, spinner);
                        down?.ShowInfo(data, CallInfoActualLeague, spinner);
                    }
                }
                else
                {
                    spinner.SetActive(false);
                }
            ShowActualPlayerName(0);
            ShowClassicPlayerName(0);
        }, error =>
        {
            onFailed?.Invoke();
            spinner.SetActive(false);
        });
    }

    /// <summary>
    /// Trae la informaci?n del panel del equipo competitivo de la liga actual
    /// </summary>
    /// <param name="team">Contenedor de cartas</param>
    private void CallInfoActualLeague(AllTokensContainer team)
    {
        tokenContainer = team;
        panelTokenItemTop.ForEach(t => t.ResetToken());
        panelTokenItemButtom.ForEach(t => t.ResetToken());
        panelTokenclassicItemTop.ForEach(t => t.ResetToken());
        panelTokenclassicItemButtom.ForEach(t => t.ResetToken());
        if (tokenContainer.current != null && tokenContainer.current.Count > 0)
        {
            for (var i = 0; i < tokenContainer.current.Count; i++)
            {
                var top = panelTokenItemTop.ElementAtOrDefault(i);
                var down = panelTokenItemButtom.ElementAtOrDefault(i);
                var data = tokenContainer.current.ElementAtOrDefault(i);
                top?.ShowInfo(data, CallInfoActualLeague, spinner);
                down?.ShowInfo(data, CallInfoActualLeague, spinner);
            }
        }
        if (tokenContainer.classical != null && tokenContainer.classical.Count > 0)
        {
            for (var i = 0; i < tokenContainer.classical.Count; i++)
            {
                var top = panelTokenclassicItemTop.ElementAtOrDefault(i);
                var down = panelTokenclassicItemButtom.ElementAtOrDefault(i);
                var data = tokenContainer.classical.ElementAtOrDefault(i);
                top?.ShowInfo(data, CallInfoActualLeague, spinner);
                down?.ShowInfo(data, CallInfoActualLeague, spinner);
            }
        }
        else
        {
            spinner.SetActive(false);
        }
        ShowActualPlayerName(0);
        ShowClassicPlayerName(0);
    }

    /// <summary>
    /// Muestra las carta en el centro de la pantalla
    /// </summary>
    /// <param name="index">?ndice de la carta</param>
    public void ShowPivotActualLeague(int index)
    {
        panelTokenItemButtom.ForEach(i=> i.ShowPivot(false));
        panelTokenItemButtom[index].ShowPivot(true);
    }   /// <summary>
    /// Muestra las carta en el centro de la pantalla
    /// </summary>
    /// <param name="index">?ndice de la carta</param>
    public void ShowPivotClassicLeague(int index)
    {
        panelTokenclassicItemButtom.ForEach(i=> i.ShowPivot(false));
        panelTokenclassicItemButtom[index].ShowPivot(true);
    }
    /// <summary>
    /// obtiene el nombre del jugador en la liga actual
    /// </summary>
    /// <param name="index"></param>
    public void ShowActualPlayerName(int index)
    {
        actualLeaguePlayerName.text = tokenContainer.current[index].name;
    }
    /// <summary>
    /// Obiene el nombre del jugador en la liga clasica
    /// </summary>
    /// <param name="index"></param>
    public void ShowClassicPlayerName(int index)
    {
        classicLeaguePlayerName.text = tokenContainer.classical[index].name;
    }
    public void SearchForPlayer(bool isActualLeague)
    {
        if(panelOpener)
        {
            panelOpener.OpenPopup();
            panelOpener.popup.GetComponent<Panels.PanelAñadirEquipoCol>().isActualLeague = isActualLeague;
        }
    }
}
