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
    public ItemData skinData { get; private set; } // Informaci?n de la Skin
    [SerializeField] [Tooltip("B1ot?n de cerrar panel")]
    private Button button;
    [SerializeField] [Tooltip("Muestra estad?sticas del potenciador")]
    private Text textStat;
    [SerializeField] [Tooltip("Imagen del potenciador")]
    public Image imageIcon;
    [SerializeField]
    [Tooltip("Imagen de bloqueo de interacci?n")]
    private GameObject disabledPanel;

    /// <summary>
    /// Muestra la informaci?n del potenciador
    /// </summary>
    /// <param name="boosterdata">Datos del potenciador</param>
    /// <param name="onshowconfirmation">Acci?n que se ejecuta al mostrar la confirmaci?n</param>
    public void ShowInfo(ItemData itemData, Action onshowconfirmation = null, UnityAction<bool> onToggleClicked = null)
    {
        skinData = itemData;
        textStat.text = itemData.description;
        if (!string.IsNullOrEmpty(itemData.img_show))
        {
            if (imageIcon)
            {
                //Se ha cambiadola variable que se usa para cargar la imagen TODO
                //string imagePath = "https://donext-dev.s3.eu-west-3.amazonaws.com/resources/" + itemData.img_show;
                string imagePath = itemData.img_show_full;
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
