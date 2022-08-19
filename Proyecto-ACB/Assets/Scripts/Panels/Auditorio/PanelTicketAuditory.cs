using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using WebAPI;
using Data;
using Newtonsoft.Json;

public class PanelTicketAuditory : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Contiene la información del pase")]
    public VIPPassesReturn.VIPPass data;
    [SerializeField]
    [Tooltip("Imagen del borde del ticket")]
    private Image ticketBorder;
    [SerializeField]
    [Tooltip("Sprite del borde del ticket General")]
    private Sprite ticketBorderGeneralSprite;
    [SerializeField]
    [Tooltip("Sprite del borde del ticket específico")]
    private Sprite ticketBorderSpecificSprite;
    [SerializeField]
    [Tooltip("Nombre del pase")]
    private Text title;
    [SerializeField]
    [Tooltip("Descripción del pase")]
    private Text description;
    [SerializeField]
    [Tooltip("fecha de caducidad del pase")]
    private Text date;
    [SerializeField]
    [Tooltip("Toggle del ticket")]
    private Toggle ticketToggle;

    /// <summary>
    /// Setea la info del pase VIP para el auditorio
    /// </summary>
    public void SetupTicketPanel(VIPPassesReturn.VIPPass ticketData,bool useToggle = false,ToggleGroup toggleGroup = null, Action OnToggleUpdate = null )
    {
        if (ticketData == null) return;
        data = ticketData;
        if (toggleGroup != null) ticketToggle.group = toggleGroup;
        if (OnToggleUpdate != null) ticketToggle.onValueChanged.AddListener((bool toggle)=> { OnToggleUpdate(); });
        ticketBorder.sprite = data.vip_pass.type == "GENERAL" ? ticketBorderGeneralSprite : ticketBorderSpecificSprite;
        title.text = data.vip_pass.title;
        description.text = data.vip_pass.description;
        ticketToggle.graphic.transform.parent.gameObject.SetActive(useToggle);
    }
}
