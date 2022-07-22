using Data;
using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controla el panel de alerta para activar el potenciador de una carta
/// </summary>
public class PanelAlertPotenciador : Panel
{
    #region Fields and properties

    [Header("Panel components")]
    [SerializeField] [Tooltip("Imagen del potenciador")]
    private Image boosterImage;
    [SerializeField] [Tooltip("Texto del potenciador")]
    private Text boosterValue;
    [SerializeField] [Tooltip("Botón de activar potenciador")]
    private Button acceptButton;
    [SerializeField] [Tooltip("Botón de cancelar potenciador")]
    private Button cancelButton;
    [SerializeField] [Tooltip("Texto de nombre de potenciador")]
    private Text boosterTypeText;

    #endregion

    #region Unity Methods

    /// <summary>
    /// Muestra la alerta de activar potenciador
    /// </summary>
    /// <param name="boosterdata">Datos del potenciador</param>
    /// <param name="playerCard">Carta del jugador</param>
    /// <param name="isInTeam">Determina si la carta del jugador está en el equipo competitivo</param>
    /// <param name="onAccept">Acción que se ejecuta al aplicar el potenciador a la carta</param>
    public void ShowAlert(BoosterData.BoosterItemData boosterdata, PlayerCard playerCard, bool isInTeam, Action<string, PlayerCard, bool> onAccept)
    {
        SetBoosterTypeText(boosterdata);
        boosterImage.sprite = boosterdata.GetSprite();
        boosterValue.text = boosterdata.value;
        acceptButton.onClick.AddListener(() => { onAccept?.Invoke(boosterdata.token, playerCard, isInTeam); });
        cancelButton.onClick.AddListener(Close);
    }

    #endregion

    #region Inner Methods

    /// <summary>
    /// Pone el título de la alerta de acuerdo al potenciador
    /// </summary>
    /// <param name="boosterdata">Datos del potenciador</param>
    private void SetBoosterTypeText(BoosterData.BoosterItemData boosterdata)
    {
        string typeString = "";
        switch (boosterdata.type)
        {
            case BoosterType.TRIPLES:
                typeString = "Tiros de 2";
                break;

            case BoosterType.ASSISTS:
                typeString = "Asistencias";
                break;

            case BoosterType.FREESHOTS:
                typeString = "Tiros libres";
                break;

            case BoosterType.POINTS:
                typeString = "Puntos";
                break;

            case BoosterType.REBOUNDS:
                typeString = "Rebotes";
                break;
        }

        boosterTypeText.text = typeString;
    }

    #endregion
}
