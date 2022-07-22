using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelSubCollectionToPublishData : Panels.PanelSubcollecionData
{
    /// <summary>
    /// Abre panel de añadir carta al equipo competitivo
    /// </summary>
    protected override void OpenAddTeamPanel()
    {
        panelOpener.popupPrefab = panelAddTeamPrefab;
        panelOpener.OpenPopup();
        panelOpener.popup.GetComponent<PanelCardsToPublish>().CallInfo(currentSubcollection.id, currentSubcollection.name);
        panelOpener.popup.GetComponent<PanelCardsToPublish>().OnConfirmedPublish += () => { OnConfirmedPublishEvent(); };
        onClickedButton?.Invoke();
    }
}
