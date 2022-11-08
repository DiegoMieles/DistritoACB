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

    public void ShowRankingSeasonView(List<HeadquarterContainerData.RankingUserData> playersRanking, string titleQuarter)
    {
        if (playersRanking == null) return;
        if(!string.IsNullOrEmpty(titleQuarter)) SeasonNameText.text = titleQuarter;
        for (int i = 0; i < playersRanking.Count; i++)
        {
            switch (playersRanking[i].position)
            {
                case 1:
                    playerRankingFirst.gameObject.SetActive(true);
                    playerRankingFirst.ShowRankingView(playersRanking[i]);
                    break;
                case 2:
                    playerRankingSecond.gameObject.SetActive(true);
                    playerRankingSecond.ShowRankingView(playersRanking[i]);
                    break;
                case 3:
                    playerRankingThird.gameObject.SetActive(true);
                    playerRankingThird.ShowRankingView(playersRanking[i]);
                    break;
            }
        }

    }
}
