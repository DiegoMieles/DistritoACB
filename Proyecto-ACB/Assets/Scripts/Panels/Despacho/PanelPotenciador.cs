using Data;
using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controla el panel donde se muestran los potenciadores que tiene el jugador
/// </summary>
public class PanelPotenciador : MonoBehaviour
{
    [SerializeField] [Tooltip("Botón de cerrar panel")]
    private Button button;
    [SerializeField] [Tooltip("Muestra estadísticas del potenciador")]
    private Text textStat;
    [SerializeField]
    [Tooltip("Muestra descripción del potenciador")]
    private Text textDescription;
    [SerializeField] [Tooltip("Imagen del potenciador")]
    public Image imageBooter;
    [HideInInspector] [Tooltip("Información del potenciador")]
    public BoosterData.BoosterItemData boostData ;
    
    /// <summary>
    /// Muestra la información del potenciador
    /// </summary>
    /// <param name="boosterdata">Datos del potenciador</param>
    /// <param name="onshowconfirmation">Acción que se ejecuta al mostrar la confirmación</param>
    public void ShowInfo(BoosterData.BoosterItemData boosterdata, Action onshowconfirmation = null)
    {
        if (boosterdata != null) boostData = boosterdata;
        textStat.text = boosterdata.value;
        if (textDescription) textDescription.text = "Potenciador " + boosterdata.name + " " + boosterdata.value;
        if (!string.IsNullOrEmpty(boosterdata.path_img))
        {
            if (imageBooter)
            {
                imageBooter.sprite = boosterdata.GetSprite();
            }
            if (onshowconfirmation != null)
            {
                button.onClick.AddListener(() =>
                {
                    onshowconfirmation?.Invoke();
                });
            }            
        }
    }
}
