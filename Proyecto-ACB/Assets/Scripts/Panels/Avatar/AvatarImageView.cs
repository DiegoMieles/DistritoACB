using Data;
using WebAPI;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controla la imagen de avatar
/// </summary>
public class AvatarImageView : MonoBehaviour
{
    [Header("Image components")]
    [SerializeField] [Header("Imagen del jugador cargada de backend")]
    private Image backendLoadedImage;

    [Space(5)]
    [Header("Image values")]
    [SerializeField] [Tooltip("Determina si se debe cargar la imagen grande")]
    private bool loadBigImage;

    public Image BackendLoadedImage => backendLoadedImage;
    /// <summary>
    /// Actualiza la vista del jugador (Normalmente cuando el avatar ha sido modificado)
    /// </summary>
    public void UpdateView()
    {
        if (!WebProcedure.Instance.IsUserNull())
        {
            string imageToLoadUrl = loadBigImage ? ACBSingleton.Instance.AccountData.avatarData.img_big : ACBSingleton.Instance.AccountData.avatarData.img_small;
            WebProcedure.Instance.GetSprite(imageToLoadUrl, 
                (obj) => { backendLoadedImage.sprite = obj; }, 
                (error) => { });
        }
        else
        {
            backendLoadedImage.sprite = Resources.Load<Sprite>("defaultAvatar");
        }

    }

    /// <summary>
    /// Actualiza la vista del jugador (Normalmente cuando el avatar ha sido modificado)
    /// </summary>
    /// <param name="userData">Dato de ranking del usuario</param>
    public void UpdateView(HeadquarterContainerData.RankingUserData userData)
    {
        WebProcedure.Instance.GetSprite(userData.img_small,
            (obj) => { backendLoadedImage.sprite = obj; },
            (error) => { });
    }

    /// <summary>
    /// Actualiza la vista del jugador (Normalmente cuando el avatar ha sido modificado)
    /// </summary>
    /// <param name="challengeData">Datos del tablón de desafio</param>
    /// <param name="isBigImage">Determina si la imagen a cargar es grande</param>
    public void UpdateView(ChallengesTablon.ChallengesTablonItem challengeData, bool isBigImage)
    {
        string imageUrl = isBigImage ? challengeData.img_big : challengeData.img_small;
        WebProcedure.Instance.GetSprite(imageUrl,
            (obj) => { if(backendLoadedImage) backendLoadedImage.sprite = obj; },
            (error) => { });
    }

    /// <summary>
    /// Actualiza la vista del jugador (Normalmente cuando el avatar ha sido modificado)
    /// </summary>
    /// <param name="challengeFieldData">Datos de desafio aceptado por el jugador</param>
    /// <param name="isBigImage">Determina si la imagen a cargar es grande</param>
    public void UpdateView(ChallengeAcceptedData.AvatarChallengeData challengeFieldData, bool isBigImage)
    {
        string imageUrl = isBigImage ? challengeFieldData.img_big : challengeFieldData.img_small;
        WebProcedure.Instance.GetSprite(imageUrl,
            (obj) => { backendLoadedImage.sprite = obj; },
            (error) => { });
    }

    /// <summary>
    /// Actualiza la vista del jugador (Normalmente cuando el avatar ha sido modificado)
    /// </summary>
    /// <param name="challengeFieldData">Datos de desafio aceptado por el jugador</param>
    public void UpdateView(ChallengesField.ChallengeFieldData.ChallengesFieldItem challengeFieldData)
    {
        string imageUrl = challengeFieldData.img_small;
        WebProcedure.Instance.GetSprite(imageUrl,
            (obj) => { if(backendLoadedImage)backendLoadedImage.sprite = obj; },
            (error) => { });
    }
}
