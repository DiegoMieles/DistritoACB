using Data;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using WebAPI;
/// <summary>
/// Controla el panel donde se muestran los potenciadores que tiene el jugador
/// </summary>
public class PanelSkin : MonoBehaviour
{
    public ItemData skinData { get; private set; } // Información de la Skin
    [SerializeField] [Tooltip("B1otón de cerrar panel")]
    private Button button;
    [SerializeField] [Tooltip("Muestra estadísticas del potenciador")]
    private Text textStat;
    [SerializeField] [Tooltip("Imagen del potenciador")]
    public Image imageIcon;
    [SerializeField]
    [Tooltip("Imagen de bloqueo de interacción")]
    private GameObject disabledPanel;

    /// <summary>
    /// Muestra la información del potenciador
    /// </summary>
    /// <param name="boosterdata">Datos del potenciador</param>
    /// <param name="onshowconfirmation">Acción que se ejecuta al mostrar la confirmación</param>
    public void ShowInfo(ItemData itemData, Action onshowconfirmation = null, UnityAction<bool> onToggleClicked = null)
    {
        skinData = itemData;
        textStat.text = itemData.description;
        if (!string.IsNullOrEmpty(itemData.img_show))
        {
            if (imageIcon)
            {
                string imagePath = "https://donext-dev.s3.eu-west-3.amazonaws.com/resources/" + itemData.img_show;
                WebProcedure.Instance.GetSprite(imagePath, OnSuccess, (WebError error) => { Debug.LogError(error); });
            }
        }
        if (onshowconfirmation != null && button)
        {
            button.onClick.AddListener(() =>
            {
                onshowconfirmation?.Invoke();
            });
        }
        if (onToggleClicked != null)
        {
            GetComponent<Toggle>().onValueChanged.AddListener(onToggleClicked);
        }
    }
    /// <summary>
    /// si se encuentra el sprite, setea la imagen del item
    /// </summary>
    /// <param name="obj">sprite encontrado </param>
    private void OnSuccess(Sprite obj)
    {
        imageIcon.sprite = obj;
    }

}
