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
    public void ShowRankingView(HeadquarterContainerData.RankingUserData userData)
    {
        avatarView.UpdateView(userData);
        nameText.text = userData.nickName;
        pointsText.text = userData.points.ToString() + " pts";
        positionText.text = userData.position.ToString()+"º";
    }

    #endregion
}
