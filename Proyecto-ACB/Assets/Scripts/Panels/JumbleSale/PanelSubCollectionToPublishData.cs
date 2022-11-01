using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelSubCollectionToPublishData : Panels.PanelSubcollecionData
{
    [SerializeField]
    [Tooltip("Panel de a?adir colecci?n")]
    private GameObject panelSubCollectionHighlights;
    /// <summary>
    /// Abre panel de a?adir carta al equipo competitivo
    /// </summary>
    protected override void OpenAddTeamPanel()
    {
        panelOpener.popupPrefab = currentSubcollection.type == Data.ItemType.H ? panelSubCollectionHighlights : panelAddTeamPrefab;
        panelOpener.OpenPopup();
        panelOpener.popup.GetComponent<PanelCardsToPublish>().CallInfo(currentSubcollection.type == Data.ItemType.H ?currentSubcollection.collection_id :currentSubcollection.id, currentSubcollection.name);
        panelOpener.popup.GetComponent<PanelCardsToPublish>().OnConfirmedPublish += () => { OnConfirmedPublishEvent(); };
        onClickedButton?.Invoke();
    }
    /// <summary>
    /// Abre panel de a?adir carta al equipo competitivo
    /// </summary>
    protected override void OpenAddTeamHightLightPanel()
    {
        panelOpener.popupPrefab = currentSubcollection.type == Data.ItemType.H ? panelSubCollectionHighlights : panelAddTeamPrefab;
        panelOpener.OpenPopup();
        panelOpener.popup.GetComponent<PanelCardsToPublish>().CallInfoCHighLightSub(currentSubcollection.type == Data.ItemType.H ? currentSubcollection.collection_id : currentSubcollection.id, currentSubcollection.name);
        panelOpener.popup.GetComponent<PanelCardsToPublish>().OnConfirmedPublish += () => { OnConfirmedPublishEvent(); };
        onClickedButton?.Invoke();
    }
}
