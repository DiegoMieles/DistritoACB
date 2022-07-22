using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;

namespace WebAPI
{
    public class WebProcedure : SingletonCustom<WebProcedure>
    {
        public Action onTokenFailed;  // evento que se dispara cuando el token de sesion cambia.
        public UserData accessData = new UserData(); // Informacion del usuario autenticado. 
        
        /// <summary>
        /// Urls de los endspoints
        /// </summary>
        private const string GetBankTransactionUrl = "GetBankTransaction?id={0}&accessToken={1}&refreshToken={2}";
        private const string GetLoadGameDataUrl = "GetLoadGameData?country={0}&id={1}&accessToken={2}&refreshToken={3}";
        private const string GetLoadUserDataUrl = "GetLoadUserData?id={0}&accessToken={1}&refreshToken={2}&token_fb={3}";
        private const string PostSaveUserAvatarUrl = "PostSaveUserAvatar?id={0}&accessToken={1}&refreshToken={2}";
        private const string PostSaveMissionCompleteUrl = "PostSaveMissionComplete?id={0}&accessToken={1}&refreshToken={2}";
        private const string PostSaveCollectCoinsUrl = "PostSaveCollectCoins?id={0}&accessToken={1}&refreshToken={2}&token_fb={3}";
        private const string GetACBallListByUserUrl = "GetACBallListByUser?id={0}&accessToken={1}&refreshToken={2}";
        private const string PostOpenACBallUrl = "PostOpenACBall?id={0}&accessToken={1}&refreshToken={2}";
        private const string GetRankingListUrl = "GetRankingList?id={0}&accessToken={1}&refreshToken={2}";
        private const string GetPostMallUrl = "GetPostMall?id={0}&accessToken={1}&refreshToken={2}";
        private const string PostMallBuyUrl = "PostMallBuy?id={0}&accessToken={1}&refreshToken={2}";
        private const string GetBoosterUserUrl = "GetBoosterUser?id={0}&accessToken={1}&refreshToken={2}";
        private const string GetUserCollectionTCUrl = "GetUserCollectionTC?id={0}&accessToken={1}&refreshToken={2}";
        private const string GetUserSubCollectionTCUrl = "GetUserSubCollectionTC?id={0}&accessToken={1}&refreshToken={2}";
        private const string GetUserCardTCUrl = "GetUserCardTC?id={0}&accessToken={1}&refreshToken={2}";
        private const string GetUserCardTokenTCUrl = "GetUserCardTokenTC?id={0}&accessToken={1}&refreshToken={2}";
        private const string GetUserHigthlightTokenUrl = "GetUserHigthlightToken?id={0}&accessToken={1}&refreshToken={2}";
        private const string PostCreateChallengeUrl = "PostCreateChallenge?id={0}&accessToken={1}&refreshToken={2}"; 
        private const string GetChallengesTablonUrl = "GetChallengesTablon?id={0}&accessToken={1}&refreshToken={2}"; 
        private const string GetCurrentTeamCompetitiveUserUrl = "GetCurrentTeamCompetitiveUser?id={0}&accessToken={1}&refreshToken={2}"; 
        private const string PostAddTokenToTeamUrl = "PostAddTokenToTeam?id={0}&accessToken={1}&refreshToken={2}"; 
        private const string DelRemoveTokenOfTeamUrl = "DelRemoveTokenOfTeam?id={0}&accessToken={1}&refreshToken={2}"; 
        private const string GetCollectionCLUrl = "GetCollectionCL?id={0}&accessToken={1}&refreshToken={2}";
        private const string GetSubCollectionCLUrl = "GetSubCollectionCL?id={0}&accessToken={1}&refreshToken={2}";
        private const string GetCardCLUrl = "GetCardCL?id={0}&accessToken={1}&refreshToken={2}";
        private const string GetChallengesCanchaUrl = "GetChallengesCancha?id={0}&accessToken={1}&refreshToken={2}";
        private const string GetUserBoosterByTypeUrl = "GetUserBoosterByType?id={0}&accessToken={1}&refreshToken={2}";
        private const string PostApplyUserBoosterToTokenCardUrl = "PostApplyUserBoosterToTokenCard?id={0}&accessToken={1}&refreshToken={2}";
        private const string PostAcceptChallengeUrl = "PostAcceptChallenge?id={0}&accessToken={1}&refreshToken={2}";
        private const string GetCardTokenCLUrl = "GetCardTokenCL?id={0}&accessToken={1}&refreshToken={2}";
        private const string GetChallengeCanchaByIdUrl = "GetChallengeCanchaById?id={0}&accessToken={1}&refreshToken={2}";
        private const string PostValidateMissionIsNearToARCardPlaceUrl = "PostValidateMissionIsNearToARCardPlace?id={0}&accessToken={1}&refreshToken={2}";
        private const string PostSetShowACBallUrl = "PostSetShowACBall?id={0}&accessToken={1}&refreshToken={2}";
        private const string PostacbiSetCodeUrl = "PostacbiSetCode?code={0}&token_fb={1}";
        private const string PostacbiSignOutUrl = "PostacbiSignOut?id={0}&accessToken={1}&refreshToken={2}";
        private const string GetPostMallElementUrl = "GetPostMallElement?id={0}";
        private const string GetAvatarInfotUrl = "GetAvatarInfo?id={0}";
        private const string GetHigthlightsTokenCLUrl = "GetHigthlightsTokenCL?id={0}";
        private const string GetSkinsToSellUrl = "GetSkinsToSell?id={0}";
        private const string GetBoostersToSellUrl = "GetBoostersToSell?id={0}";
        private const string GetACBallsToSellUrl = "GetACBallsToSell?id={0}";
        private const string GetCollectionsToSellUrl = "MarketplaceCollectionsCL?id={0}";
        private const string GetSubCollectionsToSellUrl = "MarketplaceSubcollectionsCL?id={0}";
        private const string GetTokenCardsToSellUrl = "MarketplaceTokenCards?id={0}";
        private const string GetTokenCardsUserToSellUrl = "MarketplaceTokenCardsUser?id={0}";
        private const string GetHighlightCollectionsToSellUrl = "MarketplaceCollectionsH?id={0}";
        private const string GetHighlightSubCollectionsToSellUrl = "MarketplaceSubcollectionsH?id={0}";
        private const string GetHighlightToSellUrl = "MarketplaceHighlightsToSell?id={0}";
        private const string SellItemUrl = "Sell?id={0}";
        private const string MarketplaceFilterUrl = "MarketplaceFilter?id={0}";
        private const string MarketplaceBuyUrl = "MarketplaceBuy?id={0}";
        private const string MarketplaceRemoveItemUrl = "MarketplaceRemoveItem?id={0}";
        private const string MarketplaceGetItemUrl = "MarketplaceGetItem?item_id={0}";

        public const string CREDENTIALS = "CREDENTIALS";
        
        #region USERS  
        
        /// <summary>
        /// Obtiene lista de desafios de la cancha.
        /// </summary>
        public  void  GetChallengesCancha (Action< DataSnapshot> onSuccess, Action<WebError> onFailed)
        {
            try
            {
                var form = new WWWForm();
                var url = String.Format(GetChallengesCanchaUrl, accessData.user,accessData.accessToken,accessData.refreshToken);
                StartCoroutine(RequestCoroutine(url, form, onSuccess, onFailed, null, UnityWebRequest.kHttpVerbGET));
            }
            catch (WebException webEx)
            {
                onFailed?.Invoke(WebError.Create(webEx));
            }
            catch (Exception ex)
            {
                onFailed?.Invoke(new WebError(ex.Message));
            }
        }
        /// <summary>
        /// Obtiene las traccciones del banco del usuario autenticado.
        /// </summary>
        public  void  GetBankTransaction (Action< DataSnapshot> onSuccess, Action<WebError> onFailed)
        {
            try
            {
                var form = new WWWForm();
                var url = String.Format(GetBankTransactionUrl, accessData.user,accessData.accessToken,accessData.refreshToken);
                 StartCoroutine(RequestCoroutine(url, form, onSuccess, onFailed, null, UnityWebRequest.kHttpVerbGET));
            }
            catch (WebException webEx)
            {
                onFailed?.Invoke(WebError.Create(webEx));
            }
            catch (Exception ex)
            {
              
                onFailed?.Invoke(new WebError(ex.Message));
            }
        }
        /// <summary>
        /// Obtiene la data del juego.
        /// </summary>
        public  void  GetLoadGameData (Action< DataSnapshot> onSuccess, Action<WebError> onFailed)
        {
            try
            {
                var form = new WWWForm();
                var url = String.Format(GetLoadGameDataUrl, 
                    "ES", 
                    accessData.user,
                    accessData.accessToken,
                    accessData.refreshToken);
                StartCoroutine(RequestCoroutine(url, form, onSuccess, onFailed, null, UnityWebRequest.kHttpVerbGET));
            }
            catch (WebException webEx)
            {
                onFailed?.Invoke(WebError.Create(webEx));
            }
            catch (Exception ex)
            {
                onFailed?.Invoke(new WebError(ex.Message));
            }
        }
        /// <summary>
        /// Obtiene la data del usuario autenticado.
        /// </summary>.
        public  void  GetLoadUserData (string token,Action< DataSnapshot> onSuccess, Action<WebError> onFailed)
        {
            try
            {
                var form = new WWWForm();
                var url = String.Format(GetLoadUserDataUrl, 
                    accessData.user,
                    accessData.accessToken,
                    accessData.refreshToken, 
                    token);
                StartCoroutine(RequestCoroutine(url, form, onSuccess, onFailed, null, UnityWebRequest.kHttpVerbGET));
            }
            catch (WebException webEx)
            {
                onFailed?.Invoke(WebError.Create(webEx));
            }
            catch (Exception ex)
            {
                onFailed?.Invoke(new WebError(ex.Message));
            }
        }
        /// <summary>
        /// guarda la informacion del avatar .
        /// </summary>.
        public void PostSaveUserAvatar(string avatarsettings, Action< DataSnapshot> onSuccess, Action<WebError> onFailed)
        {
            try
            {
                var url = String.Format(PostSaveUserAvatarUrl, accessData.user,accessData.accessToken,accessData.refreshToken);
                StartCoroutine(RequestCoroutine(url, avatarsettings, onSuccess, onFailed,null, UnityWebRequest.kHttpVerbPOST));
            }
            catch (WebException webEx)
            {
                onFailed?.Invoke(WebError.Create(webEx));
            }
            catch (Exception ex)
            {
              
                onFailed?.Invoke(new WebError(ex.Message));
            }
        }
        /// <summary>
        /// envia la informacion de la mission a completar.
        /// </summary>.
        public void PostSaveMissionComplete(string json, Action< DataSnapshot> onSuccess, Action<WebError> onFailed)
        {
            try
            {
                var url = String.Format(PostSaveMissionCompleteUrl, accessData.user,accessData.accessToken,accessData.refreshToken);
                StartCoroutine(RequestCoroutine(url,  json, onSuccess, onFailed,null, UnityWebRequest.kHttpVerbPOST));
            }
            catch (WebException webEx)
            {
                onFailed?.Invoke(WebError.Create(webEx));
            }
            catch (Exception ex)
            {
                onFailed?.Invoke(new WebError(ex.Message));
            }
        }
        /// <summary>
        /// obtiene acbcoins.
        /// </summary>.
        
        public void  PostSaveCollectCoins (string token,Action< DataSnapshot> onSuccess, Action<WebError> onFailed)
        {
            try
            {
                var form = new WWWForm();
                var url = String.Format(PostSaveCollectCoinsUrl, accessData.user,accessData.accessToken,accessData.refreshToken,token);
                StartCoroutine(RequestCoroutine(url, form, onSuccess, onFailed, null, UnityWebRequest.kHttpVerbPOST));
            }
            catch (WebException webEx)
            {
                onFailed?.Invoke(WebError.Create(webEx));
            }
            catch (Exception ex)
            {
                onFailed?.Invoke(new WebError(ex.Message));
            }
        }
        /// <summary>
        /// Obtiene la lista acbball del usuario autenticado.
        /// </summary>.
        public void GetACBallList (string json,Action< DataSnapshot> onSuccess, Action<WebError> onFailed)
        {
            try
            {
                var form = new WWWForm();
                var url = String.Format(GetACBallListByUserUrl, accessData.user,accessData.accessToken,accessData.refreshToken);
                StartCoroutine(RequestCoroutine(url, json, onSuccess, onFailed, null, UnityWebRequest.kHttpVerbPUT));
            }
            catch (WebException webEx)
            {
                onFailed?.Invoke(WebError.Create(webEx));
            }
            catch (Exception ex)
            {
                onFailed?.Invoke(new WebError(ex.Message));
            }
        }
        /// <summary>
        /// abre una acball seleccionada por el usuario.
        /// </summary>.
        public void PostOpenACBall(string json, Action< DataSnapshot> onSuccess, Action<WebError> onFailed)
        {
            try
            {
                var url = String.Format(PostOpenACBallUrl, accessData.user,accessData.accessToken,accessData.refreshToken);
                StartCoroutine(RequestCoroutine(url,  json, onSuccess, onFailed,null, UnityWebRequest.kHttpVerbPOST));
            }
            catch (WebException webEx)
            {
                onFailed?.Invoke(WebError.Create(webEx));
            }
            catch (Exception ex)
            {
                onFailed?.Invoke(new WebError(ex.Message));
            }
        }
      
        /// <summary>
        /// Obtiene la lista de elementos de la tienda.
        /// </summary>.
        public  void  GetPostMall (string json, Action< DataSnapshot> onSuccess, Action<WebError> onFailed)
        {
            try
            {
                var form = new WWWForm();
                var url = String.Format(GetPostMallUrl, accessData.user,accessData.accessToken,accessData.refreshToken);
                StartCoroutine(RequestCoroutine(url, json, onSuccess, onFailed, null, UnityWebRequest.kHttpVerbPUT));
            }
            catch (WebException webEx)
            {
                onFailed?.Invoke(WebError.Create(webEx));
            }
            catch (Exception ex)
            {
                onFailed?.Invoke(new WebError(ex.Message));
            }
        }
        /// <summary>
        /// Compra un item seleccionado en la tienda.
        /// </summary>.
        public  void PostMallBuy (string json,Action< DataSnapshot> onSuccess, Action<WebError> onFailed)
        {
            try
            {
                var form = new WWWForm();
                var url = String.Format(PostMallBuyUrl, accessData.user,accessData.accessToken,accessData.refreshToken);
                StartCoroutine(RequestCoroutine(url,  json, onSuccess, onFailed,null, UnityWebRequest.kHttpVerbPOST));
            }
            catch (WebException webEx)
            {
                onFailed?.Invoke(WebError.Create(webEx));
            }
            catch (Exception ex)
            {
                onFailed?.Invoke(new WebError(ex.Message));
            }
        }
        /// <summary>
        /// Obtiene la lista de usuario del ranking.
        /// </summary>.
        public  void  GetRankingList (string json,Action< DataSnapshot> onSuccess, Action<WebError> onFailed)
        {
            try
            {
                var form = new WWWForm();
                var url = String.Format(GetRankingListUrl, accessData.user,accessData.accessToken,accessData.refreshToken);
                StartCoroutine(RequestCoroutine(url,  json, onSuccess, onFailed,null, UnityWebRequest.kHttpVerbPOST));
            }
            catch (WebException webEx)
            {
                onFailed?.Invoke(WebError.Create(webEx));
            }
            catch (Exception ex)
            {
                onFailed?.Invoke(new WebError(ex.Message));
            }
        }
        /// <summary>
        /// Obtiene la lista de potenciadores del usuario autenticado.
        /// </summary>.
        public  void  GetGetBoosterUser (Action< DataSnapshot> onSuccess, Action<WebError> onFailed)
        {
            try
            {
                var form = new WWWForm();
                var url = String.Format(GetBoosterUserUrl, accessData.user,accessData.accessToken,accessData.refreshToken);
                StartCoroutine(RequestCoroutine(url, form, onSuccess, onFailed, null, UnityWebRequest.kHttpVerbGET));
            }
            catch (WebException webEx)
            {
                onFailed?.Invoke(WebError.Create(webEx));
            }
            catch (Exception ex)
            {
                onFailed?.Invoke(new WebError(ex.Message));
            }
        }
        /// <summary>
        /// Obtiene la lista filtrada de potenciadores por tipo.
        /// </summary>.
        public void GetUserBoosterByType(string json, Action< DataSnapshot> onSuccess, Action<WebError> onFailed)
        {
            try
            {
                var url = String.Format(GetUserBoosterByTypeUrl, accessData.user,accessData.accessToken,accessData.refreshToken);
                StartCoroutine(RequestCoroutine(url,  json, onSuccess, onFailed,null, UnityWebRequest.kHttpVerbPUT));
            }
            catch (WebException webEx)
            {
                onFailed?.Invoke(WebError.Create(webEx));
            }
            catch (Exception ex)
            {
                onFailed?.Invoke(new WebError(ex.Message));
            }
        }
      
        /// <summary>
        /// aplica un potenciador a una carta seleccionada.
        /// </summary>.
        public void PostApplyUserBoosterToTokenCard(string json, Action< DataSnapshot> onSuccess, Action<WebError> onFailed)
        {
            try
            {
                var url = String.Format(PostApplyUserBoosterToTokenCardUrl, accessData.user,accessData.accessToken,accessData.refreshToken);
                StartCoroutine(RequestCoroutine(url,  json, onSuccess, onFailed,null, UnityWebRequest.kHttpVerbPOST));
            }
            catch (WebException webEx)
            {
                onFailed?.Invoke(WebError.Create(webEx));
            }
            catch (Exception ex)
            {
                onFailed?.Invoke(new WebError(ex.Message));
            }
        }
        /// <summary>
        /// Obtiene la lista de colecciones del team competitivo.
        /// </summary>.
        public  void  GetUserCollectionTC (Action< DataSnapshot> onSuccess, Action<WebError> onFailed)
        {
            try
            {
                var form = new WWWForm();
                var url = String.Format(GetUserCollectionTCUrl, accessData.user,accessData.accessToken,accessData.refreshToken);
                StartCoroutine(RequestCoroutine(url, form, onSuccess, onFailed, null, UnityWebRequest.kHttpVerbGET));
            }
            catch (WebException webEx)
            {
                onFailed?.Invoke(WebError.Create(webEx));
            }
            catch (Exception ex)
            {
                onFailed?.Invoke(new WebError(ex.Message));
            }
        }
      
        /// <summary>
        /// Obtiene la lista de colecciones de las colecciones.
        /// </summary>.
        public  void  GetCollectionCL (Action< DataSnapshot> onSuccess, Action<WebError> onFailed)
        {
            try
            {
                var form = new WWWForm();
                var url = String.Format(GetCollectionCLUrl, accessData.user,accessData.accessToken,accessData.refreshToken);
                StartCoroutine(RequestCoroutine(url, form, onSuccess, onFailed, null, UnityWebRequest.kHttpVerbGET));
            }
            catch (WebException webEx)
            {
                onFailed?.Invoke(WebError.Create(webEx));
            }
            catch (Exception ex)
            {
                onFailed?.Invoke(new WebError(ex.Message));
            }
        }
      
        /// <summary>
        /// Obtiene la lista de subcolecciones del team competitivo.
        /// </summary>.
        public void GetUserSubCollectionTC(string json, Action< DataSnapshot> onSuccess, Action<WebError> onFailed)
        {
            try
            {
                var url = String.Format(GetUserSubCollectionTCUrl, accessData.user,accessData.accessToken,accessData.refreshToken);
                StartCoroutine(RequestCoroutine(url,  json, onSuccess, onFailed,null, UnityWebRequest.kHttpVerbPUT));
            }
            catch (WebException webEx)
            {
                onFailed?.Invoke(WebError.Create(webEx));
            }
            catch (Exception ex)
            {
                onFailed?.Invoke(new WebError(ex.Message));
            }
        }
      
        /// <summary>
        /// Obtiene la lista de subcolecciones de las colecciones.
        /// </summary>.
        public void GetSubCollectionCL(string json, Action< DataSnapshot> onSuccess, Action<WebError> onFailed)
        {
            try
            {
                var url = String.Format(GetSubCollectionCLUrl, accessData.user,accessData.accessToken,accessData.refreshToken);
                StartCoroutine(RequestCoroutine(url,  json, onSuccess, onFailed,null, UnityWebRequest.kHttpVerbPUT));
            }
            catch (WebException webEx)
            {
                onFailed?.Invoke(WebError.Create(webEx));
            }
            catch (Exception ex)
            {
                onFailed?.Invoke(new WebError(ex.Message));
            }
        }
      
        /// <summary>
        /// Obtiene la lista de cards del team competitivo.
        /// </summary>.
        public void GetUserCardTC(string json, Action< DataSnapshot> onSuccess, Action<WebError> onFailed)
        {
            try
            {
                var url = String.Format(GetUserCardTCUrl, accessData.user,accessData.accessToken,accessData.refreshToken);
                StartCoroutine(RequestCoroutine(url,  json, onSuccess, onFailed,null, UnityWebRequest.kHttpVerbPUT));
            }
            catch (WebException webEx)
            {
                onFailed?.Invoke(WebError.Create(webEx));
            }
            catch (Exception ex)
            {
                onFailed?.Invoke(new WebError(ex.Message));
            }
        }
      
        /// <summary>
        /// Obtiene la lista cards de las colecciones.
        /// </summary>.
        public void GetCardCL(string json, Action< DataSnapshot> onSuccess, Action<WebError> onFailed)
        {
            try
            {
                var url = String.Format(GetCardCLUrl, accessData.user,accessData.accessToken,accessData.refreshToken);
                StartCoroutine(RequestCoroutine(url,  json, onSuccess, onFailed,null, UnityWebRequest.kHttpVerbPUT));
            }
            catch (WebException webEx)
            {
                onFailed?.Invoke(WebError.Create(webEx));
            }
            catch (Exception ex)
            {
                onFailed?.Invoke(new WebError(ex.Message));
            }
        }

        /// <summary>
        /// Obtiene la lista token card del team competitivo.
        /// </summary>.
        /// 
        public void GetUserCardTokenTC(string json, Action< DataSnapshot> onSuccess, Action<WebError> onFailed)
        {
            try
            {
                var url = String.Format(GetUserCardTokenTCUrl, accessData.user,accessData.accessToken,accessData.refreshToken);
                StartCoroutine(RequestCoroutine(url,  json, onSuccess, onFailed,null, UnityWebRequest.kHttpVerbPUT));
            }
            catch (WebException webEx)
            {
                onFailed?.Invoke(WebError.Create(webEx));
            }
            catch (Exception ex)
            {
                onFailed?.Invoke(new WebError(ex.Message));
            }
        }
      
        /// <summary>
        /// Obtiene la lista token card de las colecciones.
        /// </summary>.
        ///
        public void GetCardTokenCL(string json, Action< DataSnapshot> onSuccess, Action<WebError> onFailed)
        {
            try
            {
                var url = String.Format(GetCardTokenCLUrl, accessData.user,accessData.accessToken,accessData.refreshToken);
                StartCoroutine(RequestCoroutine(url,  json, onSuccess, onFailed,null, UnityWebRequest.kHttpVerbPUT));
            }
            catch (WebException webEx)
            {
                onFailed?.Invoke(WebError.Create(webEx));
            }
            catch (Exception ex)
            {
                onFailed?.Invoke(new WebError(ex.Message));
            }
        }
      
        /// <summary>
        /// Obtiene la lista hightlight del usuario.
        /// </summary>.

        public void GetUserHigthlightToken(string json, Action< DataSnapshot> onSuccess, Action<WebError> onFailed)
        {
            try
            {
                var url = String.Format(GetUserHigthlightTokenUrl, accessData.user,accessData.accessToken,accessData.refreshToken);
                StartCoroutine(RequestCoroutine(url,  json, onSuccess, onFailed,null, UnityWebRequest.kHttpVerbGET));
            }
            catch (WebException webEx)
            {
                onFailed?.Invoke(WebError.Create(webEx));
            }
            catch (Exception ex)
            {
                onFailed?.Invoke(new WebError(ex.Message));
            }
        }
      
        /// <summary>
        /// Crea un desafio en el tablon.
        /// </summary>.
        public void PostCreateChallenge(Action< DataSnapshot> onSuccess, Action<WebError> onFailed)
        {
            try
            {
                var form = new WWWForm();
                var url = String.Format(PostCreateChallengeUrl, accessData.user,accessData.accessToken,accessData.refreshToken);
                StartCoroutine(RequestCoroutine(url,  form, onSuccess, onFailed,null, UnityWebRequest.kHttpVerbPOST));
            }
            catch (WebException webEx)
            {
                onFailed?.Invoke(WebError.Create(webEx));
            }
            catch (Exception ex)
            {
                onFailed?.Invoke(new WebError(ex.Message));
            }
        }
      
        /// <summary>
        /// Acepta un desfaio en el tablon.
        /// </summary>.
        /// 
        public void PostAcceptChallenge(string json , Action< DataSnapshot> onSuccess, Action<WebError> onFailed)
        {
            try
            {
                var url = String.Format(PostAcceptChallengeUrl, accessData.user,accessData.accessToken,accessData.refreshToken);
                StartCoroutine(RequestCoroutine(url,  json, onSuccess, onFailed,null, UnityWebRequest.kHttpVerbPOST));
            }
            catch (WebException webEx)
            {
                onFailed?.Invoke(WebError.Create(webEx));
            }
            catch (Exception ex)
            {
                onFailed?.Invoke(new WebError(ex.Message));
            }
        }
        /// <summary>
        /// Obtiene la lista de desafios en el tablon.
        /// </summary>.
        /// 
        public  void  GetChallengesTablon (Action< DataSnapshot> onSuccess, Action<WebError> onFailed)
        {
            try
            {
                var form = new WWWForm();
                var url = String.Format(GetChallengesTablonUrl, accessData.user,accessData.accessToken,accessData.refreshToken);
                StartCoroutine(RequestCoroutine(url, form, onSuccess, onFailed, null, UnityWebRequest.kHttpVerbGET));
            }
            catch (WebException webEx)
            {
                onFailed?.Invoke(WebError.Create(webEx));
            }
            catch (Exception ex)
            {
                onFailed?.Invoke(new WebError(ex.Message));
            }
        }
        /// <summary>
        /// Obtiene la data del team competitivo del usuario autenticado.
        /// </summary>.
        /// 
        public void GetCurrentTeamCompetitiveUser (Action< DataSnapshot> onSuccess, Action<WebError> onFailed)
        {
            try
            {
                var form = new WWWForm();
                var url = String.Format(GetCurrentTeamCompetitiveUserUrl, accessData.user,accessData.accessToken,accessData.refreshToken);
                StartCoroutine(RequestCoroutine(url, form, onSuccess, onFailed, null, UnityWebRequest.kHttpVerbGET));
            }
            catch (WebException webEx)
            {
                onFailed?.Invoke(WebError.Create(webEx));
            }
            catch (Exception ex)
            {
                onFailed?.Invoke(new WebError(ex.Message));
            }
        }
        /// <summary>
        /// agrega una carta al team competitivo.
        /// </summary>.
        /// 
        public void PostAddTokenToTeam(string json, Action< DataSnapshot> onSuccess, Action<WebError> onFailed)
        {
            try
            {
                var url = String.Format(PostAddTokenToTeamUrl, accessData.user,accessData.accessToken,accessData.refreshToken);
                StartCoroutine(RequestCoroutine(url,  json, onSuccess, onFailed,null, UnityWebRequest.kHttpVerbPOST));
            }
            catch (WebException webEx)
            {
                onFailed?.Invoke(WebError.Create(webEx));
            }
            catch (Exception ex)
            {
                onFailed?.Invoke(new WebError(ex.Message));
            }
        }
        /// <summary>
        /// obtiene informacion de un desafio.
        /// </summary>.
        /// 
        public void GetChallengeCanchaById(string json, Action< DataSnapshot> onSuccess, Action<WebError> onFailed)
        {
            try
            {
                var url = String.Format(GetChallengeCanchaByIdUrl, accessData.user,accessData.accessToken,accessData.refreshToken);
                StartCoroutine(RequestCoroutine(url,  json, onSuccess, onFailed,null, UnityWebRequest.kHttpVerbPUT));
            }
            catch (WebException webEx)
            {
                onFailed?.Invoke(WebError.Create(webEx));
            }
            catch (Exception ex)
            {
                onFailed?.Invoke(new WebError(ex.Message));
            }
        }
        /// <summary>
        /// elimina una carta del team competitivo.
        /// </summary>.
        /// 
        public void DelRemoveTokenOfTeam(string json, Action< DataSnapshot> onSuccess, Action<WebError> onFailed)
        {
            try
            {
                var url = String.Format(DelRemoveTokenOfTeamUrl, accessData.user,accessData.accessToken,accessData.refreshToken);
                StartCoroutine(RequestCoroutine(url,  json, onSuccess, onFailed,null, UnityWebRequest.kHttpVerbDELETE));
            }
            catch (WebException webEx)
            {
                onFailed?.Invoke(WebError.Create(webEx));
            }
            catch (Exception ex)
            {
                onFailed?.Invoke(new WebError(ex.Message));
            }
        }
        /// <summary>
        /// Obtiene una imagen desde el servidor
        /// </summary>.
        /// 
        public void GetSprite(string url, Action< Sprite> onSuccess, Action<WebError> onFailed)
        {
            try
            {
                StartCoroutine(RequestCoroutine(url, onSuccess, onFailed));
            }
            catch (WebException webEx)
            {
                onFailed?.Invoke(WebError.Create(webEx));
            }
            catch (Exception ex)
            {
                onFailed?.Invoke(new WebError(ex.Message));
            }
        }
        /// <summary>
        /// Valida la ubicacion de un ar dependiendo la geolocalizacion.
        /// </summary>.
        /// 
        public void PostValidateMissionIsNearToARCardPlace(string json, Action< DataSnapshot> onSuccess, Action<WebError> onFailed)
        {
            try
            {
                var url = String.Format(PostValidateMissionIsNearToARCardPlaceUrl, accessData.user,accessData.accessToken,accessData.refreshToken);
                StartCoroutine(RequestCoroutine(url,  json, onSuccess, onFailed,null, UnityWebRequest.kHttpVerbPOST));
            }
            catch (WebException webEx)
            {
                onFailed?.Invoke(WebError.Create(webEx));
            }
            catch (Exception ex)
            {
                onFailed?.Invoke(new WebError(ex.Message));
            }
        }
        /// <summary>
        /// valida se una acball a sido abierta para ver su informacion.
        /// </summary>.
        /// 
        public void PostSetShowACBall(string json, Action< DataSnapshot> onSuccess, Action<WebError> onFailed)
        {
            try
            {
                var url = String.Format(PostSetShowACBallUrl, accessData.user,accessData.accessToken,accessData.refreshToken);
                StartCoroutine(RequestCoroutine(url,  json, onSuccess, onFailed,null, UnityWebRequest.kHttpVerbPOST));
            }
            catch (WebException webEx)
            {
                onFailed?.Invoke(WebError.Create(webEx));
            }
            catch (Exception ex)
            {
                onFailed?.Invoke(new WebError(ex.Message));
            }
        }
        /// <summary>
        /// valida se una acball a sido abierta para ver su informacion.
        /// </summary>.
        ///
        
        /// <summary>
        /// obtiene el code del usuario autenticado.
        /// </summary>.
        public void PostacbiSetCode(string code,string token,Action< DataSnapshot> onSuccess, Action<WebError> onFailed)
        {
            try
            {
                var form = new WWWForm();
                var url = String.Format(PostacbiSetCodeUrl, code,token);
                StartCoroutine(RequestCoroutine(url,  form, onSuccess, onFailed, null , UnityWebRequest.kHttpVerbPOST));
            }
            catch (WebException webEx)
            {
                onFailed?.Invoke(WebError.Create(webEx));
            }
            catch (Exception ex)
            {
                onFailed?.Invoke(new WebError(ex.Message));
            }
        }
        /// <summary>
        /// cierra sesion del usuario autenticado.
        /// </summary>.
        public void PostacbiSignOut(Action< DataSnapshot> onSuccess, Action<WebError> onFailed)
        {
            try
            {
                var form = new WWWForm();
                var url = String.Format(PostacbiSignOutUrl, accessData.user,accessData.accessToken,accessData.refreshToken);
                StartCoroutine(RequestCoroutine(url,  form, onSuccess, onFailed, null, UnityWebRequest.kHttpVerbPOST));
            }
            catch (WebException webEx)
            {
                onFailed?.Invoke(WebError.Create(webEx));
            }
            catch (Exception ex)
            {
                onFailed?.Invoke(new WebError(ex.Message));
            }
        }
        
        /// <summary>
        /// obtiene la informacion de un item de la tienda
        /// </summary>.

        public void GetPostMallElement(string id, Action< DataSnapshot> onSuccess, Action<WebError> onFailed)
        {
            try
            {
                var form = new WWWForm();
                var url = String.Format(GetPostMallElementUrl, id);
                StartCoroutine(RequestCoroutine(url,  form, onSuccess, onFailed, null, UnityWebRequest.kHttpVerbGET));
            }
            catch (WebException webEx)
            {
                onFailed?.Invoke(WebError.Create(webEx));
            }
            catch (Exception ex)
            {
                onFailed?.Invoke(new WebError(ex.Message));
            }
        }
        /// <summary>
        /// obtiene la informacion de la configuracion actual del avatar.
        /// </summary>.
        public void GetAvatarInfo(Action< DataSnapshot> onSuccess, Action<WebError> onFailed)
        {
            try
            {
                var form = new WWWForm();
                var url = String.Format(GetAvatarInfotUrl,  accessData.user);
                StartCoroutine(RequestCoroutine(url,  form, onSuccess, onFailed, null, UnityWebRequest.kHttpVerbGET));
            }
            catch (WebException webEx)
            {
                onFailed?.Invoke(WebError.Create(webEx));
            }
            catch (Exception ex)
            {
                onFailed?.Invoke(new WebError(ex.Message));
            }
        }
        
        /// <summary>
        /// obtiene la lista de highlight de las colecciones.
        /// </summary>.

        public void GetHigthlightsTokenCL(string json, Action< DataSnapshot> onSuccess, Action<WebError> onFailed)
        {
            try
            {
                var url = String.Format(GetHigthlightsTokenCLUrl, accessData.user);
                StartCoroutine(RequestCoroutine(url,  json, onSuccess, onFailed,null, UnityWebRequest.kHttpVerbPUT));
            }
            catch (WebException webEx)
            {
                onFailed?.Invoke(WebError.Create(webEx));
            }
            catch (Exception ex)
            {
                onFailed?.Invoke(new WebError(ex.Message));
            }
        }
        /// <summary>
        /// Obtiene las skins para publicar en el mercadillo
        /// </summary>
        /// <param name="onSuccess">Acción que se dispara al obtener los skins</param>
        /// <param name="onFailed">se dispara cuando ocurrió un error</param>
        public void GetSkinsToSell(Action<DataSnapshot> onSuccess, Action<WebError> onFailed)
        {
            var form = new WWWForm();
            var url = String.Format(GetSkinsToSellUrl, accessData.user);
            StartCoroutine(RequestCoroutine(url, form, onSuccess, onFailed, null, UnityWebRequest.kHttpVerbGET));
        }
        /// <summary>
        /// Obtiene los potenciadores para publicar en el mercadillo
        /// </summary>
        /// <param name="onSuccess">Acción que se dispara al obtener los potenciadores</param>
        /// <param name="onFailed">se dispara cuando ocurrió un error</param>
        public void GetBoostersToSell(Action<DataSnapshot> onSuccess, Action<WebError> onFailed)
        {
            var form = new WWWForm();
            var url = String.Format(GetBoostersToSellUrl, accessData.user);
            StartCoroutine(RequestCoroutine(url, form, onSuccess, onFailed, null, UnityWebRequest.kHttpVerbGET));
        }
        /// <summary>
        /// Obtiene los Acballs para publicar en el mercadillo 
        /// </summary>
        /// <param name="json">información de la paginación </param>
        /// <param name="onSuccess">Acción que se dispara al obtener los ACBalls</param>
        /// <param name="onFailed">se dispara cuando ocurrió un error</param>
        public void GetACBallsToSell(string json, Action<DataSnapshot> onSuccess, Action<WebError> onFailed)
        {
            try
            {
                var url = String.Format(GetACBallsToSellUrl, accessData.user);
                StartCoroutine(RequestCoroutine(url, json, onSuccess, onFailed, null, UnityWebRequest.kHttpVerbPUT));
            }
            catch (WebException webEx)
            {
                onFailed?.Invoke(WebError.Create(webEx));
            }
            catch (Exception ex)
            {
                onFailed?.Invoke(new WebError(ex.Message));
            }
        }
        /// <summary>
        /// Obtiene las colecciones disponibles para publicar en el mercadillo
        /// </summary>
        public void GetCollectionsToSell(Action<DataSnapshot> onSuccess, Action<WebError> onFailed)
        {
            var form = new WWWForm();
            var url = String.Format(GetCollectionsToSellUrl, accessData.user);
            StartCoroutine(RequestCoroutine(url, form, onSuccess, onFailed, null, UnityWebRequest.kHttpVerbGET));
        }
        /// <summary>
        /// Obtiene las sub colecciones para publicar en el mercadillo
        /// </summary>
        public void GetSubCollectionsToSell(string json, Action<DataSnapshot> onSuccess, Action<WebError> onFailed)
        {
            try
            {
                var url = String.Format(GetSubCollectionsToSellUrl, accessData.user);
                StartCoroutine(RequestCoroutine(url, json, onSuccess, onFailed, null, UnityWebRequest.kHttpVerbPUT));
            }
            catch (WebException webEx)
            {
                onFailed?.Invoke(WebError.Create(webEx));
            }
            catch (Exception ex)
            {
                onFailed?.Invoke(new WebError(ex.Message));
            }
        }
        /// <summary>
        /// Obtiene las cartas de personajes para publicar en el mercadillo
        /// </summary>
        public void GetCardsToSell(string json, Action<DataSnapshot> onSuccess, Action<WebError> onFailed)
        {
            try
            {
                var url = String.Format(GetTokenCardsToSellUrl, accessData.user);
                StartCoroutine(RequestCoroutine(url, json, onSuccess, onFailed, null, UnityWebRequest.kHttpVerbPUT));
            }
            catch (WebException webEx)
            {
                onFailed?.Invoke(WebError.Create(webEx));
            }
            catch (Exception ex)
            {
                onFailed?.Invoke(new WebError(ex.Message));
            }
        }
        /// <summary>
        /// Obtiene las cartas únicas del personaje para publicar en el mercadillo
        /// </summary>
        public void GetCardsUserToSell(string json, Action<DataSnapshot> onSuccess, Action<WebError> onFailed)
        {
            try
            {
                var url = String.Format(GetTokenCardsUserToSellUrl, accessData.user);
                StartCoroutine(RequestCoroutine(url, json, onSuccess, onFailed, null, UnityWebRequest.kHttpVerbPUT));
            }
            catch (WebException webEx)
            {
                onFailed?.Invoke(WebError.Create(webEx));
            }
            catch (Exception ex)
            {
                onFailed?.Invoke(new WebError(ex.Message));
            }
        }
        /// <summary>
        /// Obtiene las Subcolecciones de highlights  para publicar en el mercadillo
        /// </summary>
        public void GetSubCollectionHighlightsToSell(string json, Action<DataSnapshot> onSuccess, Action<WebError> onFailed)
        {
            try
            {
                var url = String.Format(GetHighlightSubCollectionsToSellUrl, accessData.user);
                StartCoroutine(RequestCoroutine(url, json, onSuccess, onFailed, null, UnityWebRequest.kHttpVerbPUT));
            }
            catch (WebException webEx)
            {
                onFailed?.Invoke(WebError.Create(webEx));
            }
            catch (Exception ex)
            {
                onFailed?.Invoke(new WebError(ex.Message));
            }
        }
        /// <summary>
        /// Obtiene los highlights únicas del personaje para publicar en el mercadillo
        /// </summary>
        public void GetHighlightsToSell(string json, Action<DataSnapshot> onSuccess, Action<WebError> onFailed)
        {
            try
            {
                var url = String.Format(GetHighlightToSellUrl, accessData.user);
                StartCoroutine(RequestCoroutine(url, json, onSuccess, onFailed, null, UnityWebRequest.kHttpVerbPUT));
            }
            catch (WebException webEx)
            {
                onFailed?.Invoke(WebError.Create(webEx));
            }
            catch (Exception ex)
            {
                onFailed?.Invoke(new WebError(ex.Message));
            }
        }
        /// <summary>
        /// Obtiene las colecciones de highlights disponibles para publicar en el mercadillo
        /// </summary>
        public void GetHighlightCollectionsToSell(Action<DataSnapshot> onSuccess, Action<WebError> onFailed)
        {
            var form = new WWWForm();
            var url = String.Format(GetHighlightCollectionsToSellUrl, accessData.user);
            StartCoroutine(RequestCoroutine(url, form, onSuccess, onFailed, null, UnityWebRequest.kHttpVerbGET));
        }
        /// <summary>
        /// publica un item en el mercadillo.
        /// </summary>.
        public void SellItem(string json, Action<DataSnapshot> onSuccess, Action<WebError> onFailed)
        {
            try
            {
                var url = String.Format(SellItemUrl,accessData.user);
                StartCoroutine(RequestCoroutine(url, json, onSuccess, onFailed, null, UnityWebRequest.kHttpVerbPOST));
            }
            catch (WebException webEx)
            {
                onFailed?.Invoke(WebError.Create(webEx));
            }
            catch (Exception ex)
            {
                onFailed?.Invoke(new WebError(ex.Message));
            }
        }
        /// <summary>
        /// Obtiene todos los items del jumble sale en arreglos de cada tipo :    "BOOSTER","SKIN","ACBALL", "TOKENCARD",  "TOKENHIGTHLIGHT" .
        /// </summary>.
        public void GetJumbleSaleItems(string json, Action<DataSnapshot> onSuccess, Action<WebError> onFailed)
        {
            try
            {
                var url = String.Format(MarketplaceFilterUrl, accessData.user, accessData.accessToken, accessData.refreshToken);
                StartCoroutine(RequestCoroutine(url, json, onSuccess, onFailed, null, UnityWebRequest.kHttpVerbPOST));
            }
            catch (WebException webEx)
            {
                onFailed?.Invoke(WebError.Create(webEx));
            }
            catch (Exception ex)
            {
                onFailed?.Invoke(new WebError(ex.Message));
            }
        }
        /// <summary>
        /// Intenta comprar un item del mercadillo
        /// </summary>
        /// <param name="json"></param>
        /// <param name="onSuccess"></param>
        /// <param name="onFailed"></param>
        public void BuyJumbleSaleItem(string json, Action<DataSnapshot> onSuccess, Action<WebError> onFailed)
        {
            try
            {
                var url = String.Format(MarketplaceBuyUrl, accessData.user, accessData.accessToken, accessData.refreshToken);
                StartCoroutine(RequestCoroutine(url, json, onSuccess, onFailed, null, UnityWebRequest.kHttpVerbPOST));
            }
            catch (WebException webEx)
            {
                onFailed?.Invoke(WebError.Create(webEx));
            }
            catch (Exception ex)
            {
                onFailed?.Invoke(new WebError(ex.Message));
            }
        }
        public void DeleteJumbleSaleItem(string json, Action<DataSnapshot> onSuccess, Action<WebError> onFailed)
        {
            try
            {
                var url = String.Format(MarketplaceRemoveItemUrl, accessData.user, accessData.accessToken, accessData.refreshToken);
                StartCoroutine(RequestCoroutine(url, json, onSuccess, onFailed, null, UnityWebRequest.kHttpVerbPOST));
            }
            catch (WebException webEx)
            {
                onFailed?.Invoke(WebError.Create(webEx));
            }
            catch (Exception ex)
            {
                onFailed?.Invoke(new WebError(ex.Message));
            }
        }
        /// <summary>
        /// Obtiene información del objeto específico del mercadillo
        /// </summary>
        public void GetJumbleSaleInfoItem(string item_id,Action<DataSnapshot> onSuccess, Action<WebError> onFailed)
        {
            var form = new WWWForm();
            var url = String.Format(MarketplaceGetItemUrl, item_id);
            StartCoroutine(RequestCoroutine(url, form, onSuccess, onFailed, null, UnityWebRequest.kHttpVerbGET));
        }
        #endregion

        /// <summary>
        /// Procedimiento general para llamar endpoints.
        /// </summary>.
        /// 
        private IEnumerator RequestCoroutine(string url, WWWForm postData,Action< DataSnapshot> onSuccess, Action<WebError> onFailed, Action<DataSnapshot> onData = null, string method = "")
        {
            var setting = WebProcedureSettings.Instance;
            Debug.Log(setting.Ip+url);
            var www = UnityWebRequest.Post(setting.Ip+url, postData);
            www.method = method;

#pragma warning disable 618
            using (www)
#pragma warning restore 618
            {
                yield return www.SendWebRequest();
                if (!string.IsNullOrEmpty(www.error))
                {
                    HttpStatusCode status = 0;
                    var errMessage = "";
                    if (www.GetResponseHeaders() != null)
                    {
                        if (www.GetResponseHeaders().ContainsKey("STATUS"))
                        {
                            var str = www.GetResponseHeaders()["STATUS"] as string;
                            var components = str.Split(' ');
                            if (components.Length >= 3 && int.TryParse(components[1], out var code))
                                status = (HttpStatusCode)code;
                        }
                    }
            
                    if (www.error.Contains("crossdomain.xml") || www.error.Contains("Couldn't resolve"))
                    {
                        errMessage = "No internet connection or crossdomain.xml policy problem";
                    }
                    else 
                    {
                        try
                        {
                            if (!string.IsNullOrEmpty(www.downloadHandler.text))
                            {
                                var json = Json.Deserialize(www.downloadHandler.text);
                                if (json is Dictionary<string, object> obj2 && obj2.ContainsKey("messages") &&
                                    obj2.ContainsKey("error"))
                                {
                                    if (obj2["messages"] is Dictionary<string, object> m)
                                    {
                                        errMessage = Json.Serialize(m["error"]);
                                    }
                                }
                            }
                        }
                        catch
                        {
                            
                        }
                    }

                    if (onFailed != null)
                    {
                        if (string.IsNullOrEmpty(errMessage))
                            errMessage = www.error;

                        if (errMessage.Contains("Failed downloading"))
                        {
                            errMessage = "Request failed with no info of error.";
                        }

                        onFailed(new WebError(status, errMessage));
                    }

#if UNITY_EDITOR
                var settings = WebProcedureSettings.Instance;
                if (settings.ShowDebug)
                {
                    Debug.LogWarning(www.error + " (" + (int)status + ")\nResponse Message: " + errMessage);
                }
#endif
                }
                else
                {
                    var snapshot =  new DataSnapshot(www.downloadHandler.text);
                    var json = Json.Deserialize(www.downloadHandler.text);
                    if (json is Dictionary<string, object> obj2 && obj2.ContainsKey("messages"))
                    {
                        
                    }
                    else
                    {
                        snapshot = new DataSnapshot(www.downloadHandler.text);
                    }

                    onData?.Invoke(snapshot);
                    onSuccess?.Invoke( snapshot);
                    
                    if (snapshot.Code == 401)
                    {
                        onTokenFailed?.Invoke();
                    }
                    
                    www.Dispose();
                }
            }
        }
        /// <summary>
        /// Procedimiento general para llamar endpoints.
        /// </summary>.
        /// 
        private IEnumerator RequestCoroutine(string url, string postData,Action< DataSnapshot> onSuccess, Action<WebError> onFailed, Action<DataSnapshot> onData = null, string method = "")
        {
            var setting = WebProcedureSettings.Instance;
            var www = UnityWebRequest.Put(setting.Ip+url, postData); 
            www.SetRequestHeader("Content-Type", "application/json");
            www.method = method;

#pragma warning disable 618
            using (www)
#pragma warning restore 618
            {
                yield return www.SendWebRequest();
                if (!string.IsNullOrEmpty(www.error))
                {
                    HttpStatusCode status = 0;
                    var errMessage = "";
                    
                    if (www.GetResponseHeaders().ContainsKey("STATUS"))
                    {
                        var str = www.GetResponseHeaders()["STATUS"] as string;
                        var components = str.Split(' ');
                        if (components.Length >= 3 && int.TryParse(components[1], out var code))
                            status = (HttpStatusCode)code;
                    }

                    if (www.error.Contains("crossdomain.xml") || www.error.Contains("Couldn't resolve"))
                    {
                        errMessage = "No internet connection or crossdomain.xml policy problem";
                    }
                    else 
                    {
                        try
                        {
                            if (!string.IsNullOrEmpty(www.downloadHandler.text))
                            {
                                var json = Json.Deserialize(www.downloadHandler.text);
                                if (json is Dictionary<string, object> obj2 && obj2.ContainsKey("messages") &&
                                    obj2.ContainsKey("error"))
                                {
                                    if (obj2["messages"] is Dictionary<string, object> m)
                                    {
                                        errMessage = Json.Serialize(m["error"]);
                                    }
                                }
                            }
                        }
                        catch
                        {

                        }
                    }

                    if (onFailed != null)
                    {
                        if (string.IsNullOrEmpty(errMessage))
                            errMessage = www.error;

                        if (errMessage.Contains("Failed downloading"))
                        {
                            errMessage = "Request failed with no info of error.";
                        }

                        onFailed(new WebError(status, errMessage));
                    }

#if UNITY_EDITOR
                var settings = WebProcedureSettings.Instance;
                if (settings.ShowDebug)
                {
                    Debug.LogWarning(www.error + " (" + (int)status + ")\nResponse Message: " + errMessage);
                }
#endif
                }
                else
                {
                    var snapshot =  new DataSnapshot(www.downloadHandler.text);
                    var json = Json.Deserialize(www.downloadHandler.text);
                    if (json is Dictionary<string, object> obj2 && obj2.ContainsKey("messages"))
                    {
                        
                    }
                    else
                    {
                        snapshot = new DataSnapshot(www.downloadHandler.text);
                    }

                    onData?.Invoke(snapshot);
                    onSuccess?.Invoke( snapshot);
                    if (snapshot.Code == 401)
                    {
                        onTokenFailed?.Invoke();
                    }
                    www.Dispose();;
                }
            
            }
        }
        /// <summary>
        /// Procedimiento general para llamar endpoints.
        /// </summary>.
        /// 
        private IEnumerator  RequestCoroutine(string url,Action<Sprite> onSuccess, Action<WebError> onFailed)
        {
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(url, true);
          
#pragma warning disable 618
            using (www)
#pragma warning restore 618
            {
                yield return www.SendWebRequest();
                if (!string.IsNullOrEmpty(www.error))
                {

                    HttpStatusCode status = 0;
                    var errMessage = "";

                    if (www.GetResponseHeaders() != null)
                    {
                        if (www.GetResponseHeaders().ContainsKey("STATUS"))
                        {
                            var str = www.GetResponseHeaders()["STATUS"] as string;
                            var components = str.Split(' ');
                            if (components.Length >= 3 && int.TryParse(components[1], out var code))
                                status = (HttpStatusCode)code;
                        }
                    }
          
                    if (www.error.Contains("crossdomain.xml") || www.error.Contains("Couldn't resolve"))
                    {
                        errMessage = "No internet connection or crossdomain.xml policy problem";
                    }
                    else 
                    {
                        try
                        {
                            if (!string.IsNullOrEmpty(www.downloadHandler.text))
                            {
                                var json = Json.Deserialize(www.downloadHandler.text);
                                if (json is Dictionary<string, object> obj2 && obj2.ContainsKey("messages") &&
                                    obj2.ContainsKey("error"))
                                {
                                    if (obj2["messages"] is Dictionary<string, object> m)
                                    {
                                        errMessage = Json.Serialize(m["error"]);
                                    }
                                }
                            }
                        }
                        catch
                        {

                        }
                    }

                    
                    if (onFailed != null)
                    {
                        if (string.IsNullOrEmpty(errMessage))
                            errMessage = www.error;

                        if (errMessage.Contains("Failed downloading"))
                        {
                            errMessage = "Request failed with no info of error.";
                        }

                        onFailed(new WebError(status, errMessage));
                    }

#if UNITY_EDITOR
                var settings = WebProcedureSettings.Instance;
                if (settings.ShowDebug)
                {
                    Debug.LogWarning(www.error + " (" + (int)status + ")\nResponse Message: " + errMessage);
                }
#endif
                }
                else
                {
                    var webSprite = SpriteFromTexture2D ( ((DownloadHandlerTexture)www.downloadHandler).texture);
                    www.Dispose();
                    onSuccess?.Invoke(webSprite);
                    System.GC.Collect();
                    Resources.UnloadUnusedAssets();
                }
            };
        }
        /// <summary>
        /// Procedimiento general para llamar endpoints.
        /// </summary>.
        /// 
        public IEnumerator  GetSpriteCoroutine(string url,Action<Sprite> onSuccess, Action<WebError> onFailed)
        {
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(url, true);
          
#pragma warning disable 618
            using (www)
#pragma warning restore 618
            {
                yield return www.SendWebRequest();
                if (!string.IsNullOrEmpty(www.error))
                {

                    HttpStatusCode status = 0;
                    var errMessage = "";

                    if (www.GetResponseHeaders() != null)
                    {
                        if (www.GetResponseHeaders().ContainsKey("STATUS"))
                        {
                            var str = www.GetResponseHeaders()["STATUS"] as string;
                            var components = str.Split(' ');
                            if (components.Length >= 3 && int.TryParse(components[1], out var code))
                                status = (HttpStatusCode)code;
                        }
                    }
          
                    if (www.error.Contains("crossdomain.xml") || www.error.Contains("Couldn't resolve"))
                    {
                        errMessage = "No internet connection or crossdomain.xml policy problem";
                    }
                    else 
                    {
                        try
                        {
                            if (!string.IsNullOrEmpty(www.downloadHandler.text))
                            {
                                var json = Json.Deserialize(www.downloadHandler.text);
                                if (json is Dictionary<string, object> obj2 && obj2.ContainsKey("messages") &&
                                    obj2.ContainsKey("error"))
                                {
                                    if (obj2["messages"] is Dictionary<string, object> m)
                                    {
                                        errMessage = Json.Serialize(m["error"]);
                                    }
                                }
                            }
                        }
                        catch
                        {
                            // ignored
                        }
                    }

                    
                    if (onFailed != null)
                    {
                        if (string.IsNullOrEmpty(errMessage))
                            errMessage = www.error;

                        if (errMessage.Contains("Failed downloading"))
                        {
                            errMessage = "Request failed with no info of error.";
                        }

                        onFailed(new WebError(status, errMessage));
                    }

#if UNITY_EDITOR
                var settings = WebProcedureSettings.Instance;
                if (settings.ShowDebug)
                {
                    Debug.LogWarning(www.error + " (" + (int)status + ")\nResponse Message: " + errMessage);
                }
#endif
                }
                else
                {
                    
                    var webSprite = SpriteFromTexture2D ( ((DownloadHandlerTexture)www.downloadHandler).texture);
                    www.Dispose();
                    onSuccess?.Invoke(webSprite);
                    System.GC.Collect();
                    Resources.UnloadUnusedAssets();
                }
            } 
;
        }
        /// <summary>
        /// Convierte la imagen descargada a formato sprite.
        /// </summary>.
        private Sprite SpriteFromTexture2D(Texture2D texture)
        {
            return Sprite.Create(texture, new Rect(0.0f, 0.0f,texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
        }
        /// <summary>
        /// Fuerza al usuario a cerrar sesion.
        /// </summary>.
        public void LogOut()
        {
            accessData = new UserData()
            {
                accessToken = "null",
                refreshToken = "null",
                user = "null"
            };
        }
        /// <summary>
        /// Valida si existe algun usuario autenticado.
        /// </summary>.
        public bool IsUserNull()
        {
            return accessData.user == "null";
        }
    
    }
}
