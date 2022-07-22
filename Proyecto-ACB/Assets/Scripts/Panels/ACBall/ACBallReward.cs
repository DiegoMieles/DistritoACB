using UnityEngine;
using UnityEngine.UI;
using WebAPI;

/// <summary>
/// Clase que muestra y controla la recompensa que pueda tener una ACBall
/// </summary>
public class ACBallReward : MonoBehaviour
{
    [Header("Reward UI components")]
    [SerializeField] [Tooltip("Imagen de la recompensa dentro de la ACBall")]
    public Image rewardImage;

    #region Public Methods

    /// <summary>
    /// Llama a backend para el cargado de la imagen del premio dentro de la ACBall
    /// </summary>
    /// <param name="imageUrl">Url donde se encuentra la imagen</param>
    public void SetupRewardImage(string imageUrl)
    {
        WebProcedure.Instance.GetSprite(imageUrl, OnLoadRewardSprite, OnLoadRewardFailed);
    }

    #endregion

    #region Inner Methods

    /// <summary>
    /// Método que se ejecuta cuando se carga la imagen de forma exitosa desde backend
    /// </summary>
    /// <param name="obj">Imagen del premio de la ACBall</param>
    private void OnLoadRewardSprite(Sprite obj)
    {
        if(rewardImage)
            rewardImage.sprite = obj;
    }

    /// <summary>
    /// Método que se ejecuta cuando la imagen no pudo ser cargada desde backend
    /// </summary>
    /// <param name="obj">Clase con los datos de error al cargar la imagen</param>
    private void OnLoadRewardFailed(WebError obj)
    {
        Destroy(gameObject);
    }

    #endregion
}
