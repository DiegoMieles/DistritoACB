using Data;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enumerador con los tipos de distribuciones disponibles para la organización de los botones
/// </summary>
public enum TypeLayoutCategory
{
    Vertical,
    Horizontal,
    HorizontalDouble
}

/// <summary>
/// Tipo de item que se va a asignar al jugador
/// </summary>
public enum TypeCategory
{
    Look,
    Clothes,
    BackGround
}

/// <summary>
/// Controla la selección de la categoría de items a asignar a un jugador
/// </summary>
public class CategorySelection : MonoBehaviour
{
    [SerializeField] [Header("Editor de avatar")]
    private AvatarEditor avatarEditor;

    [Space(5)]
    [Header("items group references")]
    [SerializeField] [Tooltip("Lista de botones de selección de categoría")]
    private List<ButtonCategory> buttons;
    [SerializeField] [Tooltip("Controlador de los tipos de distribución de los botones de los items")]
    private List<TypeLayoutController> typeLayouts; 

    [Space(5)]
    [Header("Color buttons groups references")]
    [SerializeField] [Tooltip("Lista de botones de colores de piel")]
    private List<AvatarColorButton> colorButtons;
    [SerializeField] [Tooltip("Lista de botones de colores de cabello")]
    private List<AvatarColorButton> hairColorButtons;

    private int lookCategories; //Índice de categoría de items disponibles para asignar al jugador

    public AvatarEditor AvatarEditor => avatarEditor;
    public int LookCategories => lookCategories;

    /// <summary>
    /// Configura la categoría seleccionada
    /// </summary>
    /// <param name="typeCategory">Categoría de elementos a mostrar</param>
    public void InitCategory(TypeCategory typeCategory)
    {
        bool first = true;
        lookCategories = 0;

        for(int i = 0; i < buttons.Count; i++)
        {
            if(buttons[i].Category == typeCategory)
            {
                buttons[i].Init(this);
                if(first)
                {
                    first = false;
                    bool categoryLoadFromGameData = typeCategory == TypeCategory.Look ? true : false;
                    SelectButton(buttons[i].ItemType, buttons[i].LayoutCategory, buttons[i].UseNullButton, categoryLoadFromGameData);
                }
            }
            else
            {
                buttons[i].DesactiveButton();
            }
        }
    }
    
    /// <summary>
    /// Configura los botones de colores de piel
    /// </summary>
    /// <param name="color">Lista de colores a asignar a cada botón</param>
    public void InitColors(List<ItemReferencesColor> color)
    {
        for(int i = 0; i < color.Count; i++)
        {
            colorButtons[i].Init(this, color[i], () => { avatarEditor.AvatarHasChanges = true; });
        }
    }

    /// <summary>
    /// Configura los botones de colores de cabello
    /// </summary>
    /// <param name="color">Lista de colores a asignar a cada botón</param>
    public void InitHairColors(List<ItemReferencesColor> color)
    {
        for(int i = 0; i < color.Count; i++)
        {
            hairColorButtons[i].Init(this, color[i], () => { avatarEditor.AvatarHasChanges = true; });
        }
    }

    /// <summary>
    /// Asigna el color elegido a alguna parte del cuerpo del avatar
    /// </summary>
    /// <param name="dataColor">Referencia del color asignado</param>
    /// <param name="typeColor">Determina en que parte del cuerpo se aplica el color</param>
    public void OnClickColorButton(ItemReferencesColor dataColor, TypeColor typeColor)
    {
        if(typeColor == TypeColor.Body)
        {
            for(int i = 0; i < colorButtons.Count; i++)
            {
                if(colorButtons[i].Data.id == dataColor.id)
                {
                    avatarEditor.AvatarView.SetColorToAvatar(typeColor, dataColor.icon, dataColor.id);
                }
                else
                {
                    colorButtons[i].DesactiveColor();
                }
            }
        }

        if(typeColor == TypeColor.hair)
        {
            for(int i = 0; i < hairColorButtons.Count; i++)
            {
                if(hairColorButtons[i].Data.id == dataColor.id)
                {
                    avatarEditor.AvatarView.SetColorToAvatar(typeColor, dataColor.icon, dataColor.id);
                }
                else
                {
                    hairColorButtons[i].DesactiveColor();
                }
            }
        }
    }

    /// <summary>
    /// Asigna el atributo seleccionado al atributo específico del avatar
    /// </summary>
    /// <param name="typeButton">Tipo de item del botón</param>
    /// <param name="layoutCategory">Categoría de distribución del botón seleccionado</param>
    /// <param name="_useNullButton">Determina si el botón oprimido es nulo</param>
    /// <param name="loadFromGameData">Determina si carga el atributo desde los datos internos de juego</param>
    public void SelectButton(ItemType typeButton, TypeLayoutCategory layoutCategory, bool _useNullButton, bool loadFromGameData)
    {
        for(int i = 0; i < buttons.Count; i++)
        {
            if(buttons[i].ItemType == typeButton)
            {
                buttons[i].EnableButton();
            }
            else
            {
                buttons[i].DisableButton();
            }
        }

        AvatarLayoutItems layoutItems = null;
        for(int i = 0; i < typeLayouts.Count; i++)
        {
            if(typeLayouts[i].TypeLayout == layoutCategory)
            {
                typeLayouts[i].EnableLayout();
                layoutItems = typeLayouts[i].LayoutItems;
            }
            else
            {
                typeLayouts[i].DisableLayout();
            }
        }
        avatarEditor.OnSelectItems(typeButton, layoutItems, _useNullButton, loadFromGameData);
    }
}
