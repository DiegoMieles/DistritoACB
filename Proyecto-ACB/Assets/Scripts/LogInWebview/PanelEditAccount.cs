using System;
using UnityEngine;
using UniWebViews;
using WebAPI;
using Data;
using Firebase.Extensions;

/// <summary>
/// Controla el panel de autenticación al leer datos del usuario
/// </summary>
public class PanelEditAccount : Panel
{
    /// <summary>
    /// Se ejecuta cuando se muestra el panel y suscribe el evento de autenticación al singleton principal de juego
    /// </summary>
    public void OnEnable()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Caching.ClearCache();
        Firebase.Messaging.FirebaseMessaging.DeleteTokenAsync().ContinueWithOnMainThread(
            task =>
            {
                Debug.Log("DeleteTokenAsync");
            });
        ACBSingleton.Instance. mainMenuPanel.isFirstTimeLoading = true;
            ACBSingleton.Instance.authenticationPanel.OpenPopup();
        ACBSingleton.Instance.mainMenuPanel.PlacerHolder.gameObject.SetActive(true);
        ACBSingleton.Instance.onUserDeleted += OnDeleted;
    }

    /// <summary>
    /// Se ejecuta cuando se destruye el panel y desuscribe el evento de autenticación al singleton principal de juego
    /// </summary>
    public void OnDestroy()
    {
        ACBSingleton.Instance.onUserDeleted -= OnDeleted;
    }

    /// <summary>
    /// Cierra el panel al eliminar al jugador
    /// </summary>
    private void OnDeleted()
    {
        WebProcedure.Instance.RemoveAccount((DataSnapshot obj) => { }, (WebError error) => { });
        ACBSingleton.Instance.LogOut();
        Close();
    }
}
