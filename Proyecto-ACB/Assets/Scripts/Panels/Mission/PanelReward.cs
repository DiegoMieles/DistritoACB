using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Data;
using WebAPI;

/// <summary>
/// Panel básico de recompensa por completar una misión o al abrir una ACBall
/// </summary>
public class PanelReward : Panel
{
    [Header("Panel components")]
    [SerializeField] [Tooltip("Botón de aceptar la recompensa obtenida")]
    private Button acceptButton;
    [SerializeField] [Tooltip("Texto de título que muestra el titulo con el tipo de recompensa obtenida")]
    private Text titleText;
    [SerializeField] [Tooltip("Texto que muestra el nombre de la recompensa obtenida")]
    private Text rewardTextName;
    [SerializeField] [Tooltip("Imagen de la recompensa obtenida")]
    private Image rewardACBallImage;
    [SerializeField] [Tooltip("Ïmagen de la recompensa obtenida dentro de un ACBall")]
    private Image innerRewardIamge;
    [SerializeField] [Tooltip("Datos de carta/highlight de recompensa")]
    private MissionCardReward cardReward;
    [SerializeField] [Tooltip("Animador de la imagen de ACBall")]
    private Animator acballImageAnimator;

    [Space(5)]
    [Header("Default components")]
    [SerializeField] [Tooltip("Imagen por defecto que se muestra cuando la recompensa tiene datos nulos")]
    private Sprite defaultInnerRewardSprite;
    [SerializeField] [Tooltip("Imagen de la ACBall abierta")]
    private Sprite openAcballImage;

    private AcbBallContainer.AcbBallReward reward; //Datos de la recompensa obtenida
    private Action onShownReward; //Acción que se ejecuta una vez en la animación de mostrar recompensa ya sea visible la imagen de la recompensa obtenida
    private Action onFinishedOpeningACBall; //Acción que se ejecuta al aceptar los datos de la recompensa obtenida

    private Firebase.Analytics.Parameter param;

    #region Public Methods

    /// <summary>
    /// Actualiza los datos de la recompensa de misión, actualiza las analíticas y muestra la recompensa
    /// </summary>
    /// <param name="rewardData">Datos de la recompensa obtenida en misión y retornados desde backend</param>
    /// <param name="onShownReward">Acción que se ejecuta cuando la recompensa es mostrada</param>
    /// <param name="rewardText">Texto que se va a poner en el título</param>
    public void SetMissionRewardToOpen(MissionRewardData rewardData, Action onShownReward, string rewardText = "Has ganado:\n")
    {
        gameObject.SetActive(true);
        transform.SetAsFirstSibling();
        
        this.onShownReward = onShownReward;

        acceptButton.gameObject.SetActive(false);
        acceptButton.onClick.AddListener(() => gameObject.SetActive(false));

        rewardACBallImage.gameObject.SetActive(false);
        innerRewardIamge.gameObject.SetActive(false);
        rewardTextName.gameObject.SetActive(false);
        cardReward.gameObject.SetActive(false);
        ACBSingleton.Instance.ActivateMainSpinner(false);

        switch (rewardData.reward.rewardType)
        {
            case ItemType.ACBALL:
                Firebase.Analytics.FirebaseAnalytics.LogEvent("win_acball");
                Debug.Log("Analytic win_acball logged");

                var acballData = (MissionRewardData.RewardData.RewardItemsACBall)rewardData.reward.ChangeType(ItemType.ACBALL);
                rewardText += acballData.element.name;
                rewardTextName.text = acballData.element.description;
                break;

            case ItemType.COINS:
                var coinsData = (MissionRewardData.RewardData.RewardItemsCoin)rewardData.reward.ChangeType(ItemType.COINS);
                param = new Firebase.Analytics.Parameter("amount", coinsData.element.value);
                if(coinsData.element.value > 1)
                    Firebase.Analytics.FirebaseAnalytics.LogEvent("win_coins", param);
                else
                    Firebase.Analytics.FirebaseAnalytics.LogEvent("win_coin");

                Debug.Log("Analytic win_coin(s) logged");

                rewardText += "";
                rewardTextName.text = coinsData.element.description;
                rewardACBallImage.gameObject.SetActive(true);
                OnSuccessLoadingMissionReward(defaultInnerRewardSprite);
                break;

            case ItemType.SKIN:
                var skinData = (MissionRewardData.RewardData.RewardItemsAvatar)rewardData.reward.ChangeType(ItemType.SKIN);

                SetSkinAnalytics(skinData);

                rewardText += skinData.element.name;
                rewardTextName.text = skinData.element.description;
                rewardACBallImage.gameObject.SetActive(true);

                if (!string.IsNullOrEmpty(rewardData.reward.img))
                    WebProcedure.Instance.GetSprite(skinData.element.img_show, OnSuccessLoadingMissionReward, OnFailedLoadingMissionReward);
                break;

            case ItemType.TOKEN:
                param = new Firebase.Analytics.Parameter("type", ItemType.TOKEN.ToString());
                Firebase.Analytics.FirebaseAnalytics.LogEvent("win_card", param);
                Debug.Log("Analytic win_card logged");

                var tokenData = (MissionRewardData.RewardData.RewardItemsToken)rewardData.reward.ChangeType(ItemType.TOKEN);
                rewardText += tokenData.element.name;
                rewardTextName.text = tokenData.element.description;
                break;

            case ItemType.HIGTHLIGHT:
                param = new Firebase.Analytics.Parameter("type", ItemType.HIGTHLIGHT.ToString());
                Firebase.Analytics.FirebaseAnalytics.LogEvent("win_card", param);
                Debug.Log("Analytic win_card logged");

                var highlightData = (MissionRewardData.RewardData.RewardItemsHighlight)rewardData.reward.ChangeType(ItemType.HIGTHLIGHT);
                rewardText += highlightData.element.name;
                rewardTextName.text = string.Empty;
                break;

            case ItemType.BOOSTER:
                var boosterData = (MissionRewardData.RewardData.RewardItemsBooster)rewardData.reward.ChangeType(ItemType.BOOSTER);
                SetBoosterAnalytics(boosterData);
                rewardText += boosterData.element.name;
                rewardTextName.text = boosterData.element.description;
                break;
        }

        if (rewardData.reward.rewardType == ItemType.TOKEN || rewardData.reward.rewardType == ItemType.HIGTHLIGHT)
        {
            transform.SetAsLastSibling();
            cardReward.gameObject.SetActive(true);
            cardReward.ShowCard(rewardData.reward);
        }
        else if(rewardData.reward.rewardType != ItemType.SKIN)
        {
            rewardACBallImage.gameObject.SetActive(true);
            
            if (!string.IsNullOrEmpty(rewardData.reward.img))
                WebProcedure.Instance.GetSprite(rewardData.reward.img, OnSuccessLoadingMissionReward, OnFailedLoadingMissionReward);
        }

        rewardTextName.gameObject.SetActive(true);
        titleText.text = rewardText;

        ACBSingleton.Instance.UpdateActualMissionButtonView();
    }

    /// <summary>
    /// Muestra la ACBall con los datos de la recompensa, actualiza las analíticas y muestra la descripción de la recompensa
    /// </summary>
    /// <param name="acballItemData">Datos de la ACBall traidos previamente por backend</param>
    /// <param name="reward">Datos de la recompensa</param>
    /// <param name="onShownReward">Acción que se ejecuta cuando la recompensa ha sido mostrada</param>
    /// <param name="onFinishedOpeningACBall">Acción que se ejecuta una vez abierta la ACBall</param>
    /// <param name="rewardText">Texto del título de ACBall abierta</param>
    public void SetACBallOpen(AcbBallContainer.AcbBallsData.AcBallsItems acballItemData, AcbBallContainer.AcbBallReward reward, Action onShownReward, Action onFinishedOpeningACBall, string rewardText = "¡acball abierto!")
    {
        Firebase.Analytics.FirebaseAnalytics.LogEvent("open_acball");
        Debug.Log("Analytic open_acball logged");

        this.reward = reward;
        rewardTextName.gameObject.SetActive(false);
        cardReward.gameObject.SetActive(false);
        this.onShownReward = onShownReward;
        
        gameObject.SetActive(true);
        rewardACBallImage.gameObject.SetActive(true);
        transform.SetAsFirstSibling();
        //
        this.onShownReward = onShownReward;
        titleText.text = rewardText;

        switch (reward.openACBall.elemenType)
        {
            case ItemType.COINS:
                var coins = (AcbBallContainer.ACBallItemsCoin)reward.openACBall.ChangeType(reward.openACBall.elemenType);
                rewardTextName.text = coins.element.description;

                param = new Firebase.Analytics.Parameter("amount", coins.element.value);
                if (coins.element.value > 1)
                    Firebase.Analytics.FirebaseAnalytics.LogEvent("win_coins", param);
                else
                    Firebase.Analytics.FirebaseAnalytics.LogEvent("win_coin");

                Debug.Log("Analytic win_coin(s) logged");

                break;

            case ItemType.TOKEN:
                var token = (AcbBallContainer.ACBallItemsToken)reward.openACBall.ChangeType(reward.openACBall.elemenType);

                param = new Firebase.Analytics.Parameter("type", ItemType.TOKEN.ToString());
                Firebase.Analytics.FirebaseAnalytics.LogEvent("win_card", param);
                Debug.Log("Analytic win_card logged");

                rewardTextName.text = token.element.description;
                break;

            case ItemType.HIGTHLIGHT:
                var highlight = (AcbBallContainer.ACBallItemsHighlight)reward.openACBall.ChangeType(reward.openACBall.elemenType);

                param = new Firebase.Analytics.Parameter("type", ItemType.HIGTHLIGHT.ToString());
                Firebase.Analytics.FirebaseAnalytics.LogEvent("win_card", param);
                Debug.Log("Analytic win_card logged");

                rewardTextName.text = highlight.element.name;
                break;

            case ItemType.SKIN:
                var skin = (AcbBallContainer.ACBallItemsAvatar)reward.openACBall.ChangeType(reward.openACBall.elemenType);

                SetSkinAnalytics(skin);

                rewardTextName.text = skin.element.description;
                break;

            case ItemType.BOOSTER:
                var booster = (AcbBallContainer.ACBallItemsBooster)reward.openACBall.ChangeType(reward.openACBall.elemenType);

                SetBoosterAnalytics(booster);

                rewardTextName.text = booster.element.description;
                break;
        }

        this.onFinishedOpeningACBall = onFinishedOpeningACBall;
        WebProcedure.Instance.GetSprite(acballItemData.path_img, OnSucessLoadingAcball, (error) => { });
    }

    #endregion

    #region Inner Methods

    /// <summary>
    /// Método que se ejecuta cuando la carga de la imagen de la recompensa de ACBall ha sido satisfactoria
    /// </summary>
    /// <param name="obj">Imagen de la recompensa dentro de ACBall</param>
    private void OnSucessLoadingAcball(Sprite obj)
    {
        rewardACBallImage.sprite = obj;

        acceptButton.onClick.AddListener(() => gameObject.SetActive(false));

        if (reward.openACBall.elemenType == ItemType.SKIN)
        {
            
            var skin = (AcbBallContainer.ACBallItemsAvatar)reward.openACBall.ChangeType(reward.openACBall.elemenType);
            if (!string.IsNullOrEmpty(skin.element.img_show))
            {
                WebProcedure.Instance.GetSprite(skin.element.img_show, OnSuccessLoadingInnerRewardImage, OnFailedLoadingMissionReward);
            }
            else
            {
                OnSuccessLoadingInnerRewardImage(defaultInnerRewardSprite);

            }
        }
        else if (reward.openACBall.elemenType == ItemType.COINS)
        {
            OnSuccessLoadingInnerRewardImage(defaultInnerRewardSprite); 
        }
        else
        {
            if (!string.IsNullOrEmpty(reward.openACBall.img))
            {
                WebProcedure.Instance.GetSprite(reward.openACBall.img, OnSuccessLoadingInnerRewardImage, OnFailedLoadingMissionReward);
            }
            else
            {
                OnSuccessLoadingInnerRewardImage(defaultInnerRewardSprite);

            }
        }
   
    }

    /// <summary>
    /// Método que se ejecuta cuando una recompensa (distinta a las que se encuentran dentro de las ACBalls) es cargado satisfactoriamente
    /// </summary>
    /// <param name="obj">Imagen de recompensa de missión</param>
    private void OnSuccessLoadingMissionReward(Sprite obj)
    {
        rewardACBallImage.sprite = obj;

        transform.SetAsLastSibling();
        onShownReward?.Invoke();
        acceptButton.gameObject.SetActive(true);
        rewardTextName.gameObject.SetActive(true);
        ACBSingleton.Instance.UpdateGameData();
    }

    /// <summary>
    /// Método que se muestra cuando la imagen de la recompensa de una mission no ha sido traida por backend satisfactoriamente
    /// </summary>
    /// <param name="obj">Clase con los datos de error al intentar traer la imagen de recompensa</param>
    private void OnFailedLoadingMissionReward(WebError obj)
    {
        Debug.Log(obj.Message);
        transform.SetAsLastSibling();
        onShownReward?.Invoke();
        acceptButton.gameObject.SetActive(true);
        rewardTextName.gameObject.SetActive(true);
    }

    /// <summary>
    /// Se ejecuta una vez la recompensa dentro de la ACBall cargado desde backend es válido, cargando la recompensa y activando la animación de la ACBall al ser abierta
    /// </summary>
    /// <param name="obj">Imagen de la recompensa dentro de la ACBall</param>
    private void OnSuccessLoadingInnerRewardImage(Sprite obj)
    {
        transform.SetAsLastSibling();
        onShownReward?.Invoke();
        innerRewardIamge.sprite = obj;
        acballImageAnimator.SetTrigger("isACBall");
        StartCoroutine(SetOpenAcballImage());
        onFinishedOpeningACBall?.Invoke();
    }
    
    /// <summary>
    /// Activa la imagen de la recompensa
    /// </summary>
    /// <returns></returns>
    private IEnumerator SetOpenAcballImage()
    {
        yield return new WaitForSeconds(2.4f);
        rewardTextName.gameObject.SetActive(true);
        innerRewardIamge.gameObject.SetActive(true);
        rewardACBallImage.sprite = openAcballImage;
    }

    /// <summary>
    /// Actualiza las analíticas de acuerdo a los datos de skin traidos previamente
    /// </summary>
    /// <param name="skinData">Clase con el dato de la skin ganada</param>
    private void SetSkinAnalytics(MissionRewardData.RewardData.RewardItemsAvatar skinData)
    {
        Debug.Log("Analytic win_skin logged");

        switch (skinData.element.type)
        {
            case ItemType.ARMACCESORY:
                param = new Firebase.Analytics.Parameter("type", ItemType.ARMACCESORY.ToString());
                Firebase.Analytics.FirebaseAnalytics.LogEvent("win_skin", param);
                break;

            case ItemType.BACKGROUNDACCESORY:
                param = new Firebase.Analytics.Parameter("type", ItemType.BACKGROUNDACCESORY.ToString());
                Firebase.Analytics.FirebaseAnalytics.LogEvent("win_skin", param);
                break;

            case ItemType.BODYACCESORY:
                param = new Firebase.Analytics.Parameter("type", ItemType.BODYACCESORY.ToString());
                Firebase.Analytics.FirebaseAnalytics.LogEvent("win_skin", param);
                break;

            case ItemType.EYEACCESORY:
                param = new Firebase.Analytics.Parameter("type", ItemType.EYEACCESORY.ToString());
                Firebase.Analytics.FirebaseAnalytics.LogEvent("win_skin", param);
                break;

            case ItemType.FOREGROUNDACCESORY:
                param = new Firebase.Analytics.Parameter("type", ItemType.FOREGROUNDACCESORY.ToString());
                Firebase.Analytics.FirebaseAnalytics.LogEvent("win_skin", param);
                break;

            case ItemType.HEADACCESORY:
                param = new Firebase.Analytics.Parameter("type", ItemType.HEADACCESORY.ToString());
                Firebase.Analytics.FirebaseAnalytics.LogEvent("win_skin", param);
                break;
        }
    }

    /// <summary>
    /// Actualiza las analíticas de acuerdo a los datos de skin traidos previamente
    /// </summary>
    /// <param name="skinData">Clase con el dato de la skin ganada</param>
    private void SetSkinAnalytics(AcbBallContainer.ACBallItemsAvatar skinData)
    {
        Debug.Log("Analytic win_skin logged");

        switch (skinData.element.type)
        {
            case ItemType.ARMACCESORY:
                param = new Firebase.Analytics.Parameter("type", ItemType.ARMACCESORY.ToString());
                Firebase.Analytics.FirebaseAnalytics.LogEvent("win_skin", param);
                break;

            case ItemType.BACKGROUNDACCESORY:
                param = new Firebase.Analytics.Parameter("type", ItemType.BACKGROUNDACCESORY.ToString());
                Firebase.Analytics.FirebaseAnalytics.LogEvent("win_skin", param);
                break;

            case ItemType.BODYACCESORY:
                param = new Firebase.Analytics.Parameter("type", ItemType.BODYACCESORY.ToString());
                Firebase.Analytics.FirebaseAnalytics.LogEvent("win_skin", param);
                break;

            case ItemType.EYEACCESORY:
                param = new Firebase.Analytics.Parameter("type", ItemType.EYEACCESORY.ToString());
                Firebase.Analytics.FirebaseAnalytics.LogEvent("win_skin", param);
                break;

            case ItemType.FOREGROUNDACCESORY:
                param = new Firebase.Analytics.Parameter("type", ItemType.FOREGROUNDACCESORY.ToString());
                Firebase.Analytics.FirebaseAnalytics.LogEvent("win_skin", param);
                break;

            case ItemType.HEADACCESORY:
                param = new Firebase.Analytics.Parameter("type", ItemType.HEADACCESORY.ToString());
                Firebase.Analytics.FirebaseAnalytics.LogEvent("win_skin", param);
                break;
        }
    }

    /// <summary>
    /// Actualiza las analíticas de acuerdo a los datos de booster traido previamente
    /// </summary>
    /// <param name="boosterData">Clase con el dato del boster ganado</param>
    private void SetBoosterAnalytics(MissionRewardData.RewardData.RewardItemsBooster boosterData)
    {
        Debug.Log("Analytic win_booster logged");

        switch (boosterData.element.type)
        {
            case BoosterType.FREESHOTS:
                param = new Firebase.Analytics.Parameter("type", BoosterType.FREESHOTS.ToString());
                Firebase.Analytics.FirebaseAnalytics.LogEvent("win_booster", param);
                break;

            case BoosterType.TRIPLES:
                param = new Firebase.Analytics.Parameter("type", BoosterType.TRIPLES.ToString());
                Firebase.Analytics.FirebaseAnalytics.LogEvent("win_booster", param);
                break;

            case BoosterType.POINTS:
                param = new Firebase.Analytics.Parameter("type", BoosterType.POINTS.ToString());
                Firebase.Analytics.FirebaseAnalytics.LogEvent("win_booster", param);
                break;

            case BoosterType.ASSISTS:
                param = new Firebase.Analytics.Parameter("type", BoosterType.ASSISTS.ToString());
                Firebase.Analytics.FirebaseAnalytics.LogEvent("win_booster", param);
                break;

            case BoosterType.REBOUNDS:
                param = new Firebase.Analytics.Parameter("type", BoosterType.REBOUNDS.ToString());
                Firebase.Analytics.FirebaseAnalytics.LogEvent("win_booster", param);
                break;
        }
    }

    /// <summary>
    /// Actualiza las analíticas de acuerdo a los datos de booster traido previamente
    /// </summary>
    /// <param name="boosterData">Clase con el dato del boster ganado</param>
    private void SetBoosterAnalytics(AcbBallContainer.ACBallItemsBooster boosterData)
    {
        Debug.Log("Analytic win_booster logged");

        switch (boosterData.element.type)
        {
            case BoosterType.FREESHOTS:
                param = new Firebase.Analytics.Parameter("type", BoosterType.FREESHOTS.ToString());
                Firebase.Analytics.FirebaseAnalytics.LogEvent("win_booster", param);
                break;

            case BoosterType.TRIPLES:
                param = new Firebase.Analytics.Parameter("type", BoosterType.TRIPLES.ToString());
                Firebase.Analytics.FirebaseAnalytics.LogEvent("win_booster", param);
                break;

            case BoosterType.POINTS:
                param = new Firebase.Analytics.Parameter("type", BoosterType.POINTS.ToString());
                Firebase.Analytics.FirebaseAnalytics.LogEvent("win_booster", param);
                break;

            case BoosterType.ASSISTS:
                param = new Firebase.Analytics.Parameter("type", BoosterType.ASSISTS.ToString());
                Firebase.Analytics.FirebaseAnalytics.LogEvent("win_booster", param);
                break;

            case BoosterType.REBOUNDS:
                param = new Firebase.Analytics.Parameter("type", BoosterType.REBOUNDS.ToString());
                Firebase.Analytics.FirebaseAnalytics.LogEvent("win_booster", param);
                break;
        }
    }

    #endregion

}
