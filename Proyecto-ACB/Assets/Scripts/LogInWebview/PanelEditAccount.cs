using System;
using UnityEngine;
using UniWebViews;
using WebAPI;
using Data;
using Firebase.Extensions;
using Newtonsoft.Json;

/// <summary>
/// Controla el panel de autenticaci?n al leer datos del usuario
/// </summary>
public class PanelEditAccount : Panel
{
    /// <summary>
    /// Se ejecuta cuando se muestra el panel y suscribe el evento de autenticaci?n al singleton principal de juego
    /// </summary>
    public void OnEnable()
    {
        ACBSingleton.Instance.onUserDeleted += OnDeleted;
    }

    /// <summary>
    /// Se ejecuta cuando se destruye el panel y desuscribe el evento de autenticaci?n al singleton principal de juego
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
        RequestDeleteAccount request = new RequestDeleteAccount() { user_id = WebProcedure.Instance.accessData.user, Authorization = "Bearer " + WebProcedure.Instance.accessData.accessToken };

        WebProcedure.Instance.RemoveAccount( Success, OnFail);
        ACBSingleton.Instance.LogOut();
        Close();
    }
    private void Success(DataSnapshot obj)
    {
        print(obj.RawValue);
    }
    private void OnFail(WebError error)
    {
        Debug.Log(error);
    }
}
