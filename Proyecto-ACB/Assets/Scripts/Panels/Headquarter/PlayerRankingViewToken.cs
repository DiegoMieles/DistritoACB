using Data;
using UnityEngine;
using UnityEngine.UI;

public class PlayerRankingViewToken : MonoBehaviour
{
    #region Fields and properties

    [Header("Ranking view components")]
    [SerializeField]
    [Tooltip("Texto con el nombre del jugador")]
    private Text nameText;
    [SerializeField]
    [Tooltip("Texto con los puntos obtenidos por el jugador")]
    private Text pointsText;
    [SerializeField]
    [Tooltip("Texto con la posición en ranking del jugador")]
    private Text positionText;
    [SerializeField]
    [Tooltip("Imagen miniatura de la carta")]
    private PanelTokenItemToggle TokenToggle;

    [Space(5)]
    [Header("Avatar part references")]
    [SerializeField]
    [Tooltip("Vista del avatar del jugador")]
    private AvatarImageView avatarView;

    #endregion

    #region Public Methods
    /// <summary>
    /// Configura los datos de la vista del jugador en el ranking
    /// </summary>
    /// <param name="userData"></param>
    public void ShowRankingView(TokenDataRanking userData)
    { 
        if (userData == null) return;
        HeadquarterContainerData.RankingUserData user = new HeadquarterContainerData.RankingUserData() { img_small = userData.img_small};
        TokenItemData tokenItemData = new TokenItemData()
        { name = userData.player_name,
            assists = userData.assists,
            assits = userData.assists,
            isBooster = userData.isBooster,
            isInjured = userData.isInjured,
            isTeam = userData.isTeam,
            id = userData.id,
            points = userData.points,
            freeshots = userData.freeshots,
            pt_freeshots = userData.pt_freeshots,
            pt_assists = userData.pt_assists,
            description = userData.description,
            daysOrTextInjured = userData.daysOrTextInjured,
            pathImgBack = userData.path_img_back,
            pathImgCol = userData.pathImgCol,
            pathImgFront = userData.path_img_front,
            pt_points = userData.pt_points,
            pt_rebounds = userData.pt_rebounds,
            st_points = userData.st_points,
            st_assists = userData.st_assists,
            rarity = userData.rarity,
         pt_triples = userData.pt_triples,
         rebounds = userData.rebounds,
         st_freeshots = userData.st_freeshots,
         st_rebounds = userData.st_rebounds,
         st_triples = userData.st_triples,
         triples = userData.triples,
         textInjured = userData.textInjured,
         pathThumbnail = userData.pathThumbnail,
         victories = userData.victories,
         user_id = userData.user_id,
         token = userData.token,
         

        };
        TokenToggle.ShowInfo(tokenItemData);
        avatarView.UpdateView(user);
        nameText.text = userData.nickName;
        pointsText.text = userData.victories + " Victorias";
       
        if (positionText != null) positionText.text = userData.position.ToString() + "º";
    }

    #endregion
}
