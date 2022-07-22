using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Data;
using System;
/// <summary>
/// Panel que contiene un skin obtenido por el jugador
/// </summary>
public class SkinPanelToggle : MonoBehaviour
{
    [Header("Datos del item del botón")]
    public ItemData itemData;
    [SerializeField]
    [Header("Texto con nombre del item")]
    private Text itemName;
    [SerializeField]
    [Header("Botón del item")]
    private Button buttonItem;
    /// <summary>
    /// inicializa el botón otorgándole la información y seteando los campos.
    /// </summary>
    /// <param name="itemReference">Información del item </param>
    /// <param name="itemCustomName">si se desea mostrar un nombre en el campo de texto del item</param>
    public void InitButton(  ItemData itemReference,   string itemCustomName = "")
    {

        itemData = itemReference;
        itemName.text = itemCustomName;

        buttonItem.onClick.RemoveAllListeners();
        buttonItem.onClick.AddListener(delegate {
            OnClickInItem();
        });

       
           // WebProcedure.Instance.GetSprite(itemReference.img_show, OnSuccessLoadingSprite, OnFailedLoadingSprite);
        
    }
    public void OnClickInItem()
    {

    }
}
