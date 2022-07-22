using Data;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controla al contenedor de botones con los items disponibles al avatar
/// </summary>
public class AvatarLayoutItems : MonoBehaviour
{
    
    #region Fields and properties
    
    [Header("Panel components")]
    [SerializeField] [Tooltip("Posición de los contenedores de botones")]
    private List<RectTransform> contents;

    [SerializeField] [Tooltip("Editor de avatar")]
    private AvatarEditor avatarEditor;
    [SerializeField] [Tooltip("Prefab del botón de item")]
    private AvatarItemButton itemButtonPrefab;

    [SerializeField] [Tooltip("Botón de item nulo")]
    private AvatarItemButton itemButtonNull;
    [SerializeField] [Tooltip("Spinner de carga")]
    private GameObject spinner;

    private List<AvatarItemButton> itemButtons =  new List<AvatarItemButton>(); //Lista de botones disponibles

    private int counter; //Contador de items cargados
    private int maxCounter; //Contador con la máxima cantidad de items
    
    #endregion

    /// <summary>
    /// Asigna referencia local al editor de avatar
    /// </summary>
    /// <param name="_avatarEditor">Editor de avatar</param>
    public void Init(AvatarEditor _avatarEditor)
    {
        avatarEditor = _avatarEditor;
    }

    /// <summary>
    /// Activa los botones que tienen los contenedores
    /// </summary>
    /// <param name="items">Lista con los datos de los items</param>
    /// <param name="useNullButton">Determina si es necesario usar botón nulo</param>
    /// <param name="isColorButton">Determina si el botón debe cargar datos de color</param>
    /// <param name="loadFromGameData">Determina si debe cargar los datos del item desde los datos de juego</param>
    /// <param name="itemsCustomNames">Lista de nombres personalizados de los items</param>
    public void ActiveButtons(List<ItemData> items, bool useNullButton, bool isColorButton = false, bool loadFromGameData = false, List<string> itemsCustomNames = null)
    {
        counter = 0;
        maxCounter = items.Count;

        if (!loadFromGameData)
            SetSpinnerActiveState(true);

        if (items.Count < itemButtons.Count)
        {
            DesactiveButtons();
        }
        else
        {
            if (isColorButton)
                return;

            int diff = items.Count - itemButtons.Count;
            for(int i = 0; i < diff; i++)
            {
                AvatarItemButton newItem = Instantiate(itemButtonPrefab, contents[0]);
                newItem.DesactiveButton();
                itemButtons.Add(newItem);
            }
        }

        for(int i = 0; i < items.Count; i++)
        {
            string buttonName = itemsCustomNames != null && itemsCustomNames.Count > 0 ? itemsCustomNames[i] : "";
            itemButtons[i].InitButton(avatarEditor, this, items[i], loadFromGameData, () => { SetSpinnerActiveState(false); }, buttonName);

            var data = ACBSingleton.Instance.AccountData.avatarData.GetDataByType(items[i].itemType);
            if(data == items[i].id)
            {
                itemButtons[i].selectButton.SetActive(true);
            }
        }

        if(items.Count > 0)
        {
            if(useNullButton)
            {
                ItemData dataNull = new ItemData();
                dataNull.id = -1;
                dataNull.itemType = items[0].itemType;
                itemButtonNull.InitButton(avatarEditor, this, dataNull, true, null);
            }
            else
            {
                itemButtonNull.DesactiveButton();
            }
        }
        else
        {
            itemButtonNull.DesactiveButton();
        }
    }

    /// <summary>
    /// Detecta la selección de un botón mediante el id
    /// </summary>
    /// <param name="_idButton">Id del botón</param>
    public void SelectButton(int _idButton)
    {
        itemButtonNull.selectButton.SetActive(false);
        for(int i = 0; i < itemButtons.Count; i++)
        {
            if(itemButtons[i].activeButton)
            {
                if(itemButtons[i].itemData.id != _idButton)
                {
                    itemButtons[i].selectButton.SetActive(false);
                }
            }
        }
    }

    /// <summary>
    /// Determina si el spinner de carga se debe mostrar o no
    /// </summary>
    /// <param name="activateSpinner">Determina si el spinner se debe mostrar</param>
    private void SetSpinnerActiveState(bool activateSpinner)
    {
        counter++;
        
        if (counter < maxCounter && !activateSpinner)
            return;

        spinner.SetActive(activateSpinner);
    }
    
    /// <summary>
    /// Desactiva todos los botones
    /// </summary>
    public void DesactiveButtons()
    {
        for(int i = 0; i < itemButtons.Count; i++)
        {
            itemButtons[i].DesactiveButton();
        }
    }
}
