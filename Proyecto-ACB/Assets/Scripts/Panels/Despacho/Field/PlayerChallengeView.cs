using Data;
using System;
using System.Linq;
using UnityEngine;
using WebAPI;

/// <summary>
/// Controla la vista del avatar en el desafio
/// </summary>
public class PlayerChallengeView : PlayerView
{
    #region Fields and Properties

    private ChallengesTablon.ChallengesTablonItem challengeFieldData; //Datos del avatar
    private Action onFinishLoadingAvatar; //Acci�n que se ejecuta al finalizar la carga del avatar

    #endregion

    #region Public Methods

    /// <summary>
    /// Asigna los datos del avatar
    /// </summary>
    /// <param name="challengeFieldData">Datos del avatar</param>
    /// <param name="onFinishLoadingAvatar">Acci�n que se ejecuta al finalizar la carga del avatar</param>
    public void SetAvatarView(ChallengesTablon.ChallengesTablonItem challengeFieldData, Action onFinishLoadingAvatar = null)
    {
        this.challengeFieldData = challengeFieldData;
        this.onFinishLoadingAvatar = onFinishLoadingAvatar;
        UpdateView();
    }

    #endregion

    #region Inner Methods

    /// <summary>
    /// Actualiza la vista del avatar de acuerdo a los items que tenga asignados
    /// </summary>
    protected override void UpdateView()
    {
        AvatarExtraData avatarExtraData = ACBSingleton.Instance.AvatarExtraData;

        SetSpriteByValue(challengeFieldData.bodyForm, DEFAULTBODYFORM, bodyForm);

        SetUndershirt(challengeFieldData.bodyForm, avatarExtraData.underShirts);

        SetSpriteByValue(challengeFieldData.faceForm, DEFAULTFACEFORM, faceForm);

        SetSpriteByValue(challengeFieldData.eyes, DEFAULTEYES, eyes);

        SetSpriteByValue(challengeFieldData.ear, DEFAULTEAR, ears);

        SetSpriteByValue(challengeFieldData.nose, DEFAULTNOSE, nose);

        SetSpriteByValue(challengeFieldData.mouth, DEFAULTMOUTH, mouth);

        SetSpriteByValue(challengeFieldData.eyeBrow, DEFAULTEYEBROW, eyebrow);

        SetSpriteByValue(challengeFieldData.hairStyleFront, DEFAULTHAIRSTYLE, hair);

        if(challengeFieldData.avatarItems != null && challengeFieldData.avatarItems.Count > 0)
        {
            ItemData backgroundItem = challengeFieldData.avatarItems.FirstOrDefault(itemImage => itemImage.id == challengeFieldData.backGround);

            if (backgroundItem != null)
                WebProcedure.Instance.GetSprite(backgroundItem.path_img, (onSuccess) => { backGround.sprite = onSuccess; backGround.gameObject.SetActive(true); }, (onFailed) => { backGround.gameObject.SetActive(false); });
            else
                backGround.gameObject.SetActive(false);

            ItemData bodyAccessoryItem = challengeFieldData.avatarItems.FirstOrDefault(itemImage => itemImage.id == challengeFieldData.bodyAccessory);

            if (bodyAccessoryItem != null)
                WebProcedure.Instance.GetSprite(bodyAccessoryItem.path_img, (onSuccess) => { bodyAccesory.sprite = onSuccess; bodyAccesory.gameObject.SetActive(true); }, (onFailed) => { bodyAccesory.gameObject.SetActive(false); });
            else
                bodyAccesory.gameObject.SetActive(false);

            ItemData armAccessoryItem = challengeFieldData.avatarItems.FirstOrDefault(itemImage => itemImage.id == challengeFieldData.armAccessory);

            if (armAccessoryItem != null)
                WebProcedure.Instance.GetSprite(armAccessoryItem.path_img, (onSuccess) => { armAccesory.sprite = onSuccess; armAccesory.gameObject.SetActive(true); }, (onFailed) => { armAccesory.gameObject.SetActive(false); });
            else
                armAccesory.gameObject.SetActive(false);

            ItemData eyeAccessoryItem = challengeFieldData.avatarItems.FirstOrDefault(itemImage => itemImage.id == challengeFieldData.eyesAccessory);

            if (eyeAccessoryItem != null)
                WebProcedure.Instance.GetSprite(eyeAccessoryItem.path_img, (onSuccess) => { eyesAccesory.sprite = onSuccess; eyesAccesory.gameObject.SetActive(true); }, (onFailed) => { eyesAccesory.gameObject.SetActive(false); });
            else
                eyesAccesory.gameObject.SetActive(false);

            ItemData headAccessoryItem = challengeFieldData.avatarItems.FirstOrDefault(itemImage => itemImage.id == challengeFieldData.headAccessory);

            if (headAccessoryItem != null)
                WebProcedure.Instance.GetSprite(headAccessoryItem.path_img, (onSuccess) => { headAccesory.sprite = onSuccess; headAccesory.gameObject.SetActive(true); }, (onFailed) => { headAccesory.gameObject.SetActive(false); });
            else
                headAccesory.gameObject.SetActive(false);

        }
            
        onFinishLoadingAvatar?.Invoke();

        Sprite hairStyleBackSprite = Resources.Load<Sprite>("SpriteItem/" + challengeFieldData.hairStyleBack.ToString());
        hairBack.sprite = hairStyleBackSprite;
        hairBack.gameObject.SetActive(challengeFieldData.hairStyleBack > 15);

        if (avatarExtraData.hairColors.Count > 0)
        {
            ItemReferencesColor hairColorData = avatarExtraData.hairColors.FirstOrDefault(k => k.id == challengeFieldData.hairColor);

            if (hairColorData == null)
                hairColorData = new ItemReferencesColor(DEFAULTHAIRCOLOR, ItemType.HAIRCOLOR, avatarExtraData.hairColors[0].icon);

            SetColorToAvatar(TypeColor.hair, hairColorData.icon);
        }

        if (avatarExtraData.skinColor.Count > 0)
        {
            ItemReferencesColor skinColorData = avatarExtraData.skinColor.FirstOrDefault(k => k.id == challengeFieldData.skinColor);

            if (skinColorData == null)
                skinColorData = new ItemReferencesColor(DEFAULTSKINCOLOR, ItemType.SKINCOLOR, avatarExtraData.skinColor[0].icon);

            SetColorToAvatar(TypeColor.Body, skinColorData.icon);
        }
    }

    #endregion
}
