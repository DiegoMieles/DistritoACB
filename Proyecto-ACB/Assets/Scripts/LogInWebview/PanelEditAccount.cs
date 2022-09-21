using System;
using UnityEngine;
using UniWebViews;
using WebAPI;
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
        WebProcedure.Instance.RemoveAccount((DataSnapshot obj) => { }, (WebError error) => { Debug.LogError(error); });
        Close();
    }
}
