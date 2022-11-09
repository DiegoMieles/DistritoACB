using Data;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controla vista del objeto de rankeo de un jugador
/// </summary>
public class PlayerRankingView : MonoBehaviour
{
    #region Fields and properties

    [Header("Ranking view components")]
    [SerializeField] [Tooltip("Texto con el nombre del jugador")]
    private Text nameText;
    [SerializeField] [Tooltip("Texto con los puntos obtenidos por el jugador")]
    private Text pointsText;
    [SerializeField] [Tooltip("Texto con la posición en ranking del jugador")]
    private Text positionText;
    [SerializeField]
    [Tooltip("Color del primer lugar")]
    private Color firstPlaceColor;
    [SerializeField]
    [Tooltip("Color del segundo lugar")]
    private Color secondPlaceColor;
    [SerializeField]
    [Tooltip("Color del tercer lugar")]
    private Color thirdPlaceColor;
    [SerializeField]
    [Tooltip("anillo alrededor de la imagen del jugador")]
    private Image profileCircle;

    [Space(5)]
    [Header("Avatar part references")]
    [SerializeField] [Tooltip("Vista del avatar del jugador")]
    private AvatarImageView avatarView;

    #endregion

    #region Public Methods

    /// <summary>
    /// Configura los datos de la vista del jugador en el ranking
    /// </summary>
    /// <param name="userData"></param>
    public void ShowRankingView(HeadquarterContainerData.RankingUserData userData,PanelHeadquarter.RankingSections rankingSection = PanelHeadquarter.RankingSections.ClassicLeague)
    {
        string sfx;
        int points;
        switch (rankingSection)
        {
            case PanelHeadquarter.RankingSections.Missions:
                sfx = " misiones";
                points = userData.missions_number;
                break;
            case PanelHeadquarter.RankingSections.Cards:
                sfx = " tokens";
                points = userData.total_tokens;
                break;
        
            default:
                sfx = " pts";
                points = userData.points;
                break;
        }
        if (userData == null) return;
        avatarView.UpdateView(userData);
        pointsText.text = points.ToString() + sfx;
        Color colorToUse = Color.black;
        switch (userData.position)
        {
            case 1:
                colorToUse = firstPlaceColor;
                break;
            case 2:
                colorToUse = secondPlaceColor;
                break;
            case 3:
                colorToUse = thirdPlaceColor;
                break;

        }
        if(nameText != null)
        {
            nameText.text = userData.nickName;
           if(colorToUse != Color.black) nameText.color = colorToUse;
        }
        if (positionText != null)
        {
            positionText.text = userData.position.ToString() + "º";
            if (colorToUse != Color.black) positionText.color = colorToUse;
        }
        if (profileCircle != null && colorToUse != Color.black) profileCircle.color = colorToUse;
        
    }

    #endregion
}
