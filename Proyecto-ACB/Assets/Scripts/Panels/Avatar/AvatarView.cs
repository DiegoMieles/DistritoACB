using Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WebAPI;
using Image = UnityEngine.UI.Image;

/// <summary>
/// Controla la vista principal de avatar dentro del editor de avatar
/// </summary>
public class AvatarView : MonoBehaviour
{
    #region Fields and Properties

    [Header("Avatar part references")]
    [SerializeField] [Tooltip("Imagen del fondo del avatar")]
    private Image backGround;
    [SerializeField] [Tooltip("Imagen de cuerpo")]
    private Image bodyForm;
    [SerializeField] [Tooltip("Imagen de camisilla")]
    private Image shirtDefault;
    [SerializeField] [Tooltip("Imagen de la forma de la cara")]
    private Image faceForm;
    [SerializeField] [Tooltip("Imagen de las orejas")]
    private Image ears;
    [SerializeField] [Tooltip("Imagen de los ojos")]
    private Image eyes;
    [SerializeField] [Tooltip("Imagen de la nariz")]
    private Image nose;
    [SerializeField] [Tooltip("Imagen de la boca")]
    private Image mouth;
    [SerializeField] [Tooltip("Imagen de las cejas")]
    private Image eyebrow;
    [SerializeField] [Tooltip("Imagen del cabello frontal")]
    private Image hair;
    [SerializeField] [Tooltip("Imagen del cabello reverso")]
    private Image hairBack;
    [SerializeField] [Tooltip("Imagen del accesorio de ojos")]
    private Image eyesAccesory;
    [SerializeField] [Tooltip("Imagen del accesorio de cabeza")]
    private Image headAccesory;
    [SerializeField] [Tooltip("Imagen del accesorio de cuerpo")]
    private Image bodyAccesory;
    [SerializeField] [Tooltip("Imagen del accesorio de brazos")]
    private Image armAccesory;
    [SerializeField] [Tooltip("Imagen de bigote y barba")]
    private Image facialHair;
    [SerializeField] [Tooltip("Imagen delantera del fondo del avatar")]
    private Image foreground;

    private AvatarData currentData = new AvatarData(); //Datos de los items puestos por el avatar

    private const int DEFAULTHAIRCOLOR = 12; //Color de cabello por defecto
    private const int DEFAULTSKINCOLOR = 0; //Color de piel por defecto
    private const int DEFAULTBODYFORM = 136; //Forma del cuerpo por defecto
    private const int DEFAULTFACEFORM = 42; //Forma de la cara por defecto
    private const int DEFAULTEYES = 72; //Ojos por defecto
    private const int DEFAULTEAR = 130; //Orejas por defecto
    private const int DEFAULTNOSE = 110; //Nariz por defecto
    private const int DEFAULTMOUTH = 22; //Boca por defecto
    private const int DEFAULTEYEBROW = 52; //Cejas por defecto
    private const int DEFAULTHAIRSTYLE = 148; //Estilo de cabello por defecto

    #endregion

    #region Public Methods

    /// <summary>
    /// Actualiza la vista del avatar de acuerdo a los items que tenga asignados
    /// </summary>
    public void UpdateView()
    {
        ScriptableAccount accountData = ACBSingleton.Instance.AccountData;
        AvatarExtraData avatarExtraData = ACBSingleton.Instance.AvatarExtraData;
        //currentData = ACBSingleton.Instance.AccountData.avatarData;

        currentData.nickName = string.IsNullOrEmpty(accountData.avatarData.nickName)
            ? "DefaultName"
            : accountData.avatarData.nickName;
        currentData.isFirstTime = accountData.avatarData.isFirstTime;

        SetSpriteByValue(accountData.avatarData.bodyForm, DEFAULTBODYFORM, bodyForm, out currentData.bodyForm);

        SetUndershirt(currentData.bodyForm, avatarExtraData.underShirts);

        SetSpriteByValue(accountData.avatarData.faceForm, DEFAULTFACEFORM, faceForm, out currentData.faceForm);

        SetSpriteByValue(accountData.avatarData.eyes, DEFAULTEYES, eyes, out currentData.eyes);

        SetSpriteByValue(accountData.avatarData.ear, DEFAULTEAR, ears, out currentData.ear);

        SetSpriteByValue(accountData.avatarData.nose, DEFAULTNOSE, nose, out currentData.nose);

        SetSpriteByValue(accountData.avatarData.mouth, DEFAULTMOUTH, mouth, out currentData.mouth);

        SetSpriteByValue(accountData.avatarData.eyeBrow, DEFAULTEYEBROW, eyebrow, out currentData.eyeBrow);

        SetSpriteByValue(accountData.avatarData.hairStyleFront, DEFAULTHAIRSTYLE, hair, out currentData.hairStyleFront);

        ItemData backgroundItem =
            accountData.avatarData.avatarItems?.FirstOrDefault(itemImage =>
                itemImage.id == accountData.avatarData.backGround);

        if (backgroundItem != null)
            WebProcedure.Instance.GetSprite(backgroundItem.path_img, (onSuccess) =>
            {
                backGround.sprite = onSuccess;
                backGround.gameObject.SetActive(true);
                currentData.backGround = backgroundItem.id;
            }, (onFailed) =>
            {
                backGround.gameObject.SetActive(false);
                currentData.backGround = -1;
            });
        else
            backGround.gameObject.SetActive(false);

        ItemData bodyAccessoryItem =
            accountData.avatarData.avatarItems?.FirstOrDefault(itemImage =>
                itemImage.id == accountData.avatarData.bodyAccessory);
        if (bodyAccessoryItem != null)
            WebProcedure.Instance.GetSprite(bodyAccessoryItem.path_img, (onSuccess) =>
            {
                bodyAccesory.sprite = onSuccess;
                bodyAccesory.gameObject.SetActive(true);
                currentData.bodyAccessory = bodyAccessoryItem.id;
            }, (onFailed) =>
            {
                bodyAccesory.gameObject.SetActive(false);
                currentData.bodyAccessory = -1;
            });
        else
            bodyAccesory.gameObject.SetActive(false);

        ItemData armAccessoryItem =
            accountData.avatarData.avatarItems?.FirstOrDefault(itemImage =>
                itemImage.id == accountData.avatarData.armAccessory);
        if (armAccessoryItem != null)
            WebProcedure.Instance.GetSprite(armAccessoryItem.path_img, (onSuccess) =>
            {
                armAccesory.sprite = onSuccess;
                armAccesory.gameObject.SetActive(true);
                currentData.armAccessory = armAccessoryItem.id;
            }, (onFailed) =>
            {
                armAccesory.gameObject.SetActive(false);
                currentData.armAccessory = -1;
            });
        else
            armAccesory.gameObject.SetActive(false);

        ItemData eyeAccessoryItem =
            accountData.avatarData.avatarItems?.FirstOrDefault(itemImage =>
                itemImage.id == accountData.avatarData.eyesAccessory);

        if (eyeAccessoryItem != null)
            WebProcedure.Instance.GetSprite(eyeAccessoryItem.path_img, (onSuccess) =>
            {
                eyesAccesory.sprite = onSuccess;
                eyesAccesory.gameObject.SetActive(true);
                currentData.eyesAccessory = eyeAccessoryItem.id;
            }, (onFailed) =>
            {
                eyesAccesory.gameObject.SetActive(false);
                currentData.eyesAccessory = -1;
            });
        else
            eyesAccesory.gameObject.SetActive(false);

        ItemData headAccessoryItem =
            accountData.avatarData.avatarItems?.FirstOrDefault(itemImage =>
                itemImage.id == accountData.avatarData.headAccessory);
        if (headAccessoryItem != null)
            WebProcedure.Instance.GetSprite(headAccessoryItem.path_img, (onSuccess) =>
            {
                headAccesory.sprite = onSuccess;
                headAccesory.gameObject.SetActive(true);
                currentData.headAccessory = headAccessoryItem.id;
            }, (onFailed) =>
            {
                headAccesory.gameObject.SetActive(false);
                currentData.headAccessory = -1;
            });
        else
            headAccesory.gameObject.SetActive(false);

        ItemData foregroundItem =
            accountData.avatarData.avatarItems?.FirstOrDefault(itemImage =>
                itemImage.id == accountData.avatarData.foreGround);
        if (foregroundItem != null)
            WebProcedure.Instance.GetSprite(foregroundItem.path_img, (onSuccess) =>
            {
                foreground.sprite = onSuccess;
                foreground.gameObject.SetActive(true);
                currentData.foreGround = foregroundItem.id;
            }, (onFailed) =>
            {
                foreground.gameObject.SetActive(false);
                currentData.foreGround = -1;
            });
        else
            foreground.gameObject.SetActive(false);

        Sprite hairStyleBackSprite =
            Resources.Load<Sprite>("SpriteItem/" + accountData.avatarData.hairStyleBack.ToString());
        hairBack.sprite = hairStyleBackSprite;
        currentData.hairStyleBack = accountData.avatarData.hairStyleBack;
        hairBack.gameObject.SetActive(currentData.hairStyleBack > 15);

        Sprite facialHairSprite = Resources.Load<Sprite>("SpriteItem/" + accountData.avatarData.facialHair.ToString());
        facialHair.sprite = facialHairSprite;
        currentData.facialHair = accountData.avatarData.facialHair;
        facialHair.gameObject.SetActive(currentData.facialHair > 15);

        if (avatarExtraData.hairColors.Count > 0)
        {
            ItemReferencesColor hairColorData =
                avatarExtraData.hairColors.FirstOrDefault(k => k.id == accountData.avatarData.hairColor);

            if (hairColorData == null)
                hairColorData = new ItemReferencesColor(DEFAULTHAIRCOLOR, ItemType.HAIRCOLOR,
                    avatarExtraData.hairColors[0].icon);

            SetColorToAvatar(TypeColor.hair, hairColorData.icon, hairColorData.id);
            currentData.hairColor = hairColorData.id;
        }

        if (avatarExtraData.skinColor.Count > 0)
        {
            ItemReferencesColor skinColorData =
                avatarExtraData.skinColor.FirstOrDefault(k => k.id == accountData.avatarData.skinColor);

            if (skinColorData == null)
                skinColorData = new ItemReferencesColor(DEFAULTSKINCOLOR, ItemType.SKINCOLOR,
                    avatarExtraData.skinColor[0].icon);

            SetColorToAvatar(TypeColor.Body, skinColorData.icon, skinColorData.id);
            currentData.skinColor = skinColorData.id;
        }
    }

    /// <summary>
    /// Asigna el atributo seleccionado al avatar
    /// </summary>
    /// <param name="itemData">Dato del item</param>
    public void SetIconInAvatar(ItemData itemData)
    {
        switch (itemData.itemType)
        {
            case ItemType.BODYACCESORY:

                if (itemData.id > -1)
                    WebProcedure.Instance.GetSprite(itemData.path_img, (obj) =>
                    {
                        bodyAccesory.sprite = obj;
                        bodyAccesory.gameObject.SetActive(true);
                    }, (failed) => { bodyAccesory.gameObject.SetActive(false); });
                else
                    bodyAccesory.gameObject.SetActive(false);
                currentData.bodyAccessory = itemData.id;
                break;

            case ItemType.HEADACCESORY:

                if (itemData.id > -1)
                    WebProcedure.Instance.GetSprite(itemData.path_img, (obj) =>
                    {
                        headAccesory.sprite = obj;
                        headAccesory.gameObject.SetActive(true);
                    }, (failed) => { headAccesory.gameObject.SetActive(false); });
                else
                    headAccesory.gameObject.SetActive(false);

                currentData.headAccessory = itemData.id;
                break;

            case ItemType.BACKGROUNDACCESORY:

                if (itemData.id > -1)
                    WebProcedure.Instance.GetSprite(itemData.path_img, (obj) =>
                    {
                        backGround.sprite = obj;
                        backGround.gameObject.SetActive(true);
                    }, (failed) => { backGround.gameObject.SetActive(false); });
                else
                    backGround.gameObject.SetActive(false);

                currentData.backGround = itemData.id;
                break;

            case ItemType.EYES:
                eyes.sprite = itemData.GetSprite();
                currentData.eyes = itemData.id;
                break;

            case ItemType.FACEFORM:
                faceForm.sprite = itemData.GetSprite();
                currentData.faceForm = itemData.id;
                break;

            case ItemType.EAR:
                ears.sprite = itemData.GetSprite();
                currentData.ear = itemData.id;
                break;

            case ItemType.NOSE:
                nose.sprite = itemData.GetSprite();
                currentData.nose = itemData.id;
                break;

            case ItemType.MOUTH:
                mouth.sprite = itemData.GetSprite();
                currentData.mouth = itemData.id;
                break;

            case ItemType.EYEBROW:
                eyebrow.sprite = itemData.GetSprite();
                currentData.eyeBrow = itemData.id;
                break;

            case ItemType.HAIRSTYLE:
                hair.sprite = itemData.GetSprite();
                currentData.hairStyleFront = itemData.id;
                break;

            case ItemType.FACIALHAIR:
                facialHair.sprite = itemData.GetSprite();
                currentData.facialHair = itemData.id;
                facialHair.gameObject.SetActive(itemData.id > 15);
                break;

            case ItemType.BODYFORM:
                bodyForm.sprite = itemData.GetSprite();
                currentData.bodyForm = itemData.id;
                SetUndershirt(itemData.id, ACBSingleton.Instance.AvatarExtraData.underShirts);
                break;

            case ItemType.SHIRTDEFAULT:
                shirtDefault.sprite = itemData.GetSprite();
                break;

            case ItemType.EYEACCESORY:

                if (itemData.id > -1)
                    WebProcedure.Instance.GetSprite(itemData.path_img, (obj) =>
                    {
                        eyesAccesory.sprite = obj;
                        eyesAccesory.gameObject.SetActive(true);
                    }, (failed) => { eyesAccesory.gameObject.SetActive(false); });
                else
                    eyesAccesory.gameObject.SetActive(false);

                currentData.eyesAccessory = itemData.id;
                break;

            case ItemType.ARMACCESORY:

                if (itemData.id > -1)
                    WebProcedure.Instance.GetSprite(itemData.path_img, (obj) =>
                    {
                        armAccesory.sprite = obj;
                        armAccesory.gameObject.SetActive(true);
                    }, (failed) => { armAccesory.gameObject.SetActive(false); });
                else
                    armAccesory.gameObject.SetActive(false);

                currentData.armAccessory = itemData.id;
                break;

            case ItemType.FOREGROUNDACCESORY:
                if (itemData.id > -1)
                    WebProcedure.Instance.GetSprite(itemData.path_img, (obj) =>
                    {
                        foreground.sprite = obj;
                        foreground.gameObject.SetActive(true);
                    }, (failed) => { foreground.gameObject.SetActive(false); });
                else
                    foreground.gameObject.SetActive(false);

                currentData.foreGround = itemData.id;
                break;

            case ItemType.HAIRSTYLEBACK:

                hairBack.sprite = itemData.GetSprite();
                currentData.hairStyleBack = itemData.id;
                hairBack.gameObject.SetActive(itemData.id > 15);
                break;
        }
    }

    /// <summary>
    /// Asigna color al avatar en un atributo en específico
    /// </summary>
    /// <param name="typeColor">Tipo de color</param>
    /// <param name="color">Color a asignar</param>
    /// <param name="idColor">Id del color</param>
    public void SetColorToAvatar(TypeColor typeColor, Color color, int idColor)
    {
        if (typeColor == TypeColor.Body)
        {
            faceForm.color = color;
            bodyForm.color = color;
            nose.color = color;
            ears.color = color;
            currentData.skinColor = idColor;
        }

        if (typeColor == TypeColor.hair)
        {
            hair.color = color;
            hairBack.color = color;
            eyebrow.color = color;
            facialHair.color = color;
            currentData.hairColor = idColor;
        }
    }

    /// <summary>
    /// Guarda los datos del avatar
    /// </summary>
    /// <param name="onSaveAvatar">Acción que se ejecuta al guardar el avatar</param>
    /// <param name="imageSmallBase64">Imagen pequeña del avatar</param>
    /// <param name="imageBigBase64">Imagen grande del avatar</param>
    public void SaveAvatar(Action onSaveAvatar, string imageSmallBase64, string imageBigBase64)
    {

        currentData.avatarItems = ACBSingleton.Instance.AccountData.avatarData.avatarItems;
        ACBSingleton.Instance.AccountData.avatarData = currentData;
        var json = JsonConvert.SerializeObject(ACBSingleton.Instance.AccountData.avatarData);
        var ava = new AvatarData();

        JsonConvert.PopulateObject(json, ava);

        ava.img_small = imageSmallBase64;
        ava.img_big  = imageBigBase64;
        

        WebProcedure.Instance.PostSaveUserAvatar(JsonConvert.SerializeObject(ava), (obj) =>
        {
            Debug.Log(obj.RawJson);
            JsonConvert.PopulateObject(obj.RawJson, ACBSingleton.Instance.AccountData.avatarData);
            onSaveAvatar?.Invoke();
        }, (WebError err) =>
        {
            onSaveAvatar?.Invoke();
            OnFailedSavingAvatar(err);
        });
    }

    /// <summary>
    /// Elimina los datos almacenados de forma temporal del jugador
    /// </summary>
    public void ResetCachedData()
    {
        currentData = ACBSingleton.Instance.AccountData.avatarData;
    }

    #endregion

    #region Inner Methods

    /// <summary>
    /// Evento que se ejecuta cuando el avatar no se pudo guardar correctamente
    /// </summary>
    /// <param name="obj">Clase con los datos de error</param>
    private void OnFailedSavingAvatar(WebError obj)
    {
        Debug.Log(obj.Message);
        ACBSingleton.Instance.AlertPanel.SetupPanel("Ha sucedido un error por favor intenta nuevamente", "", false,
            null);
    }

    /// <summary>
    /// Asigna sprite por valor y carga internamente la vista del jugador
    /// </summary>
    /// <param name="loadedBackendData">id cargado de backend</param>
    /// <param name="defaultData">Dato por defecto</param>
    /// <param name="imageSpriteReference">Imagen del item a asignar</param>
    /// <param name="currentDataValue">Valor de salida del item</param>
    private void SetSpriteByValue(int loadedBackendData, int defaultData, Image imageSpriteReference,
        out int currentDataValue)
    {
        int ResourceValue = loadedBackendData <= 15 ? defaultData : loadedBackendData;
        Sprite testSprite = Resources.Load<Sprite>("SpriteItem/" + ResourceValue.ToString());
        imageSpriteReference.sprite = testSprite;
        currentDataValue = ResourceValue;
    }

    /// <summary>
    /// Ajusta la camisilla de acuerdo al tamaño del cuerpo
    /// </summary>
    /// <param name="bodyFormId">Id de la forma del cuerpo</param>
    /// <param name="underShirtReference">Lista de camisillas disponibles</param>
    private void SetUndershirt(int bodyFormId, List<UnderShirtReferenceData> underShirtReference)
    {
        for (int i = 0; i < underShirtReference.Count; i++)
        {
            UnderShirtReferenceData actualUnderShirtData = underShirtReference[i];
            for (int j = 0; j < actualUnderShirtData.bodyReferences.Count; j++)
            {
                if (actualUnderShirtData.bodyReferences[j] == bodyFormId)
                {
                    shirtDefault.sprite = actualUnderShirtData.underShirtSprite;
                    return;
                }
            }
        }
    }
}

#endregion