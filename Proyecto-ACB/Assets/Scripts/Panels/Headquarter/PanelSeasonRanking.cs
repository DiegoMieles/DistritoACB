using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Data;
public class PanelSeasonRanking : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Texto con el nombre de la temporada")]
    private TextMeshProUGUI SeasonNameText;
    [SerializeField]
    [Tooltip("visualizador del primer jugador")]
    private PlayerRankingView playerRankingFirst;
    [SerializeField]
    [Tooltip("visualizador del segundo jugador")]
    private PlayerRankingView playerRankingSecond;
    [SerializeField]
    [Tooltip("visualizador del tercer jugador")]
    private PlayerRankingView playerRankingThird;
    [SerializeField]
    [Tooltip("visualizador del primer jugador")]
    private PlayerRankingViewToken playerHistoryRankingFirst;
    [SerializeField]
    [Tooltip("visualizador del segundo jugador")]
    private PlayerRankingViewToken playerHistoryRankingSecond;
    [SerializeField]
    [Tooltip("visualizador del tercer jugador")]
    private PlayerRankingViewToken playerHistoryRankingThird;

    public void ShowRankingSeasonView(List<HeadquarterContainerData.RankingUserData> playersRanking, string titleQuarter, PanelHeadquarter.RankingSections rankingSection = PanelHeadquarter.RankingSections.ClassicLeague)
    {
        if (playersRanking == null) return;
        if(!string.IsNullOrEmpty(titleQuarter)) SeasonNameText.text = titleQuarter;
        for (int i = 0; i < playersRanking.Count; i++)
        {
            switch (playersRanking[i].position)
            {
                case 1:
                    playerRankingFirst.gameObject.SetActive(true);
                    playerRankingFirst.ShowRankingView(playersRanking[i], rankingSection);
                    break;
                case 2:
                    playerRankingSecond.gameObject.SetActive(true);
                    playerRankingSecond.ShowRankingView(playersRanking[i], rankingSection);
                    break;
                case 3:
                    playerRankingThird.gameObject.SetActive(true);
                    playerRankingThird.ShowRankingView(playersRanking[i], rankingSection);
                    break;
            }
        }

    }
    public void ShowRankingSeasonView(List<TokenDataRanking> playersRanking, string titleQuarter)
    {
        if (playersRanking == null) return;
        if (!string.IsNullOrEmpty(titleQuarter)) SeasonNameText.text = titleQuarter;
        for (int i = 0; i < playersRanking.Count; i++)
        {
            switch (playersRanking[i].position)
            {
                case 1:
                    playerHistoryRankingFirst.gameObject.SetActive(true);
                    playerHistoryRankingFirst.ShowRankingView(playersRanking[i]);
                    break;
                case 2:
                    playerHistoryRankingSecond.gameObject.SetActive(true);
                    playerHistoryRankingSecond.ShowRankingView(playersRanking[i]);
                    break;
                case 3:
                    playerHistoryRankingThird.gameObject.SetActive(true);
                    playerHistoryRankingThird.ShowRankingView(playersRanking[i]);
                    break;
            }
        }

    }
}
