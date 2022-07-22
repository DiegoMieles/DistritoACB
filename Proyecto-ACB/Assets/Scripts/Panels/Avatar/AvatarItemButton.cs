using Data;
using System;
using UnityEngine;
using UnityEngine.UI;
using WebAPI;

/// <summary>
/// Controla el botón que muestra uno de los items disponibles en avatar
/// </summary>
public class AvatarItemButton : MonoBehaviour
{
    [Header("Determina si el botón debe iniciar activo")]
    public bool activeButton;
    [Header("Items de avatar disponibles")]
    public AvatarLayoutItems layoutItems;
    [Header("Datos del item del botón")]
    public ItemData itemData;
    [Header("Referencia al editor de avatar")]
    public AvatarEditor avatarEditor;

    [Space(5)]
    [Tooltip("Objeto del botón")]
    public GameObject selectButton;

    [Header("Button components")]
    [SerializeField] [Tooltip("Objeto del botón")]
    private Button buttonItem;
    
    [SerializeField] [Tooltip("Imagen de cuerpo")]
    private Image bodyImage;
    [SerializeField] [Tooltip("Imagen de la forma de la cara")]
    private Image faceFormImage;
    [SerializeField] [Tooltip("Imagen de los ojos")]
    private Image eyesImage;
    [SerializeField] [Tooltip("Imagen de la forma de las cejas")]
    private Image eyebrowsImage;
    [SerializeField] [Tooltip("Imagen de la forma de la boca")]
    private Image mouthImage;
    [SerializeField] [Tooltip("Imagen de la forma del cabello delantero")]
    private Image hairBackImage;
    [SerializeField] [Tooltip("Imagen de la forma del cabello reverso")]
    private Image hairFrontImage;
    [SerializeField] [Tooltip("Imagen de la camisa")]
    private Image shirtImage;
    [SerializeField] [Tooltip("Imagen de la forma de las orejas")]
    private Image earsImage;
    [SerializeField] [Tooltip("Imagen de la forma de la nariz")]
    private Image noseImage;
    [SerializeField] [Tooltip("Imagen de accesorio para el jugador")]
    private Image accesoryImage;
    [SerializeField] [Tooltip("Imagen de la barba y bigote")]
    private Image facialHairImage;
    [SerializeField] [Tooltip("Imagen cuando no se encuentra algún accesorio en específico")]
    private Image NullImage;

    [Space(5)]
    [Header("Extra components")]
    [SerializeField] [Tooltip("Texto con el nombre del item")]
    private Text itemName;

    private Action onLoadedImage; //Acción que se ejecuta al finalizar la carga de la imagen

    /// <summary>
    /// Configura el funcionamiento del botón asignando el item que va a asignar al jugador
    /// </summary>
    /// <param name="avatarEditor">Editor de avatar</param>
    /// <param name="layoutItems">Contenedor de los botones de item</param>
    /// <param name="itemReference">Referencia a nivel de datos del item</param>
    /// <param name="loadFromGameData">Determina si el item se debe cargar desde los datos internos de juego</param>
    /// <param name="onLoadedImage">Acción que se ejecuta cuando la imagen ha sido cargada</param>
    /// <param name="itemCustomName">Nombre personalizado para mostrar del objeto</param>
    public void InitButton(AvatarEditor avatarEditor, AvatarLayoutItems layoutItems, ItemData itemReference, bool loadFromGameData, Action onLoadedImage, string itemCustomName = "")
    {
        this.avatarEditor = avatarEditor;
        itemData = itemReference;
        this.layoutItems = layoutItems;
        itemName.text = itemCustomName;
        this.onLoadedImage = onLoadedImage;
        buttonItem.onClick.RemoveAllListeners();
        buttonItem.onClick.AddListener(delegate {
            OnClickInItem();
        });

        if(loadFromGameData)
        {
            if(itemReference.id <= -1)
            {
                DesactiveAllIcons();
                NullImage.gameObject.SetActive(true);
            }
            else
            {
                SetIcon(itemReference.itemType, itemReference.GetSprite());

                if(itemReference.itemType == ItemType.BODYFORM)
                    SetUndershirt(itemReference.id);
            }

            activeButton = true;
            gameObject.SetActive(true);
        }
        else
        {
            WebProcedure.Instance.GetSprite(itemReference.img_show, OnSuccessLoadingSprite, OnFailedLoadingSprite);
        }
    }

    /// <summary>
    /// Método que se ejecuta una vez ha sido cargada la imagen del item satisfactoriamente
    /// </summary>
    /// <param name="obj">Imagen del item para mostrar en el botón</param>
    private void OnSuccessLoadingSprite(Sprite obj)
    {
        SetIcon(itemData.itemType, obj);
        activeButton = true;
        gameObject.SetActive(true);
        onLoadedImage?.Invoke();
    }

    /// <summary>
    /// Método que se ejecuta una vez ha fallado la carga de la imagen del item
    /// </summary>
    /// <param name="obj">Clase con los datos de error</param>
    private void OnFailedLoadingSprite(WebError obj)
    {
        SetIcon(itemData.itemType, itemData.GetSprite());
        activeButton = true;
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Deshabilita el botón
    /// </summary>
    public void DesactiveButton()
    {
        activeButton = false;
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Asigna al avatar el atributo seleccionado en el botón
    /// </summary>
    private void OnClickInItem()
    {
        avatarEditor.AvatarHasChanges = true;
        layoutItems.SelectButton(itemData.id);
        avatarEditor.AvatarView.SetIconInAvatar(itemData);
        selectButton.SetActive(true);
    }

    /// <summary>
    /// Asigna ícono del item de acuerdo a su tipo
    /// </summary>
    /// <param name="itemType">Tipo de item</param>
    /// <param name="icon">Imagen del item</param>
    /// <param name="alphaValue">Valor de opacidad</param>
    private void SetIcon(ItemType itemType, Sprite icon, float alphaValue = 1f)
    {
        DesactiveAllIcons();
        switch(itemType)
        {
            case ItemType.BODYACCESORY:
                accesoryImage.sprite = icon;
                accesoryImage.gameObject.SetActive(true);
                accesoryImage.color = new Color(1, 1, 1, alphaValue);
                break;
            case ItemType.HEADACCESORY:
                accesoryImage.sprite = icon;
                accesoryImage.gameObject.SetActive(true);
                accesoryImage.color = new Color(1, 1, 1, alphaValue);
                break;
            case ItemType.ARMACCESORY:
                break;
            case ItemType.EYEACCESORY:
                accesoryImage.sprite = icon;
                accesoryImage.gameObject.SetActive(true);
                accesoryImage.color = new Color(1, 1, 1, alphaValue);
                break;
            case ItemType.BACKGROUNDACCESORY:
                accesoryImage.sprite = icon;
                accesoryImage.gameObject.SetActive(true);
                accesoryImage.color = new Color(1, 1, 1, alphaValue);
                break;
            case ItemType.SKINCOLOR:
                break;
            case ItemType.FACEFORM:
                faceFormImage.sprite = icon;
                faceFormImage.gameObject.SetActive(true);
                faceFormImage.color = new Color(1, 1, 1, alphaValue);
                break;
            case ItemType.EYES:
                eyesImage.sprite = icon;
                eyesImage.gameObject.SetActive(true);
                eyesImage.color = new Color(1, 1, 1, alphaValue);
                break;
            case ItemType.EAR:
                earsImage.sprite = icon;
                earsImage.gameObject.SetActive(true);
                earsImage.color = new Color(1, 1, 1, alphaValue);
                break;
            case ItemType.NOSE:
                noseImage.sprite = icon;
                noseImage.gameObject.SetActive(true);
                noseImage.color = new Color(1, 1, 1, alphaValue);
                break;
            case ItemType.MOUTH:
                mouthImage.sprite = icon;
                mouthImage.gameObject.SetActive(true);
                mouthImage.color = new Color(1, 1, 1, alphaValue);
                break;
            case ItemType.EYEBROW:
                eyebrowsImage.sprite = icon;
                eyebrowsImage.gameObject.SetActive(true);
                eyebrowsImage.color = new Color(1, 1, 1, alphaValue);
                break;
            case ItemType.HAIRSTYLE:
                hairFrontImage.sprite = icon;
                hairFrontImage.gameObject.SetActive(true);
                hairFrontImage.color = new Color(1, 1, 1, alphaValue);
                break;
            case ItemType.FACIALHAIR:
                facialHairImage.sprite = icon;
                facialHairImage.gameObject.SetActive(true);
                facialHairImage.color = new Color(1, 1, 1, alphaValue);
                break;
            case ItemType.BODYFORM:
                bodyImage.sprite = icon;
                bodyImage.gameObject.SetActive(true);
                bodyImage.color = new Color(1, 1, 1, alphaValue);
                break;
            case ItemType.HAIRSTYLEBACK:
                hairFrontImage.sprite = icon;
                hairFrontImage.gameObject.SetActive(true);
                hairFrontImage.color = new Color(1, 1, 1, alphaValue);
                break;

            case ItemType.FOREGROUNDACCESORY:
                accesoryImage.sprite = icon;
                accesoryImage.gameObject.SetActive(true);
                accesoryImage.color = new Color(1, 1, 1, alphaValue);
                break;
        }
    }

    /// <summary>
    /// Desactiva visualmente todos los íconos
    /// </summary>
    private void DesactiveAllIcons()
    {
        selectButton.SetActive(false);
        bodyImage.gameObject.SetActive(false);
        faceFormImage.gameObject.SetActive(false);
        eyesImage.gameObject.SetActive(false);
        eyebrowsImage.gameObject.SetActive(false);
        mouthImage.gameObject.SetActive(false);
        hairBackImage.gameObject.SetActive(false);
        hairFrontImage.gameObject.SetActive(false);
        shirtImage.gameObject.SetActive(false);
        earsImage.gameObject.SetActive(false);
        noseImage.gameObject.SetActive(false);
        accesoryImage.gameObject.SetActive(false);
        facialHairImage.gameObject.SetActive(false);
        NullImage.gameObject.SetActive(false);
    }

    /// <summary>
    /// Ajusta la camisilla de acuerdo al tamaño del cuerpo mostrado en el botón
    /// </summary>
    /// <param name="bodyFormId">Id de la forma del cuerpo</param>
    private void SetUndershirt(int bodyFormId)
    {
        for (int i = 0; i < ACBSingleton.Instance.AvatarExtraData.underShirts.Count; i++)
        {
            UnderShirtReferenceData actualUnderShirtData = ACBSingleton.Instance.AvatarExtraData.underShirts[i];
            for (int j = 0; j < actualUnderShirtData.bodyReferences.Count; j++)
            {
                if (actualUnderShirtData.bodyReferences[j] == bodyFormId)
                {
                    shirtImage.gameObject.SetActive(true);
                    shirtImage.sprite = actualUnderShirtData.underShirtSprite;
                    return;
                }
            }
        }
    }
}
