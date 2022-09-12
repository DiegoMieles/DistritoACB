using System;
using UnityEngine;
using UniWebViews;

/// <summary>
/// Controla el panel de autenticaci�n al leer datos del usuario
/// </summary>
public class PanelEditAccount : Panel
{
    /// <summary>
    /// Se ejecuta cuando se muestra el panel y suscribe el evento de autenticaci�n al singleton principal de juego
    /// </summary>
    public void OnEnable()
    {
        ACBSingleton.Instance.onUserAuthenticated += OnAuthenticate;
    }

    /// <summary>
    /// Se ejecuta cuando se destruye el panel y desuscribe el evento de autenticaci�n al singleton principal de juego
    /// </summary>
    public void OnDestroy()
    {
        ACBSingleton.Instance.onUserAuthenticated -= OnAuthenticate;
    }

    /// <summary>
    /// M�todo con fines de prueba para pasar sin autenticar al jugador
    /// </summary>
    public void PassWithOutUser()
    {
        ACBSingleton.Instance.PassWithOutUser();
        OnAuthenticate();
    }

    /// <summary>
    /// Cierra el panel al autenticar al jugador
    /// </summary>
    private void OnAuthenticate()
    {
        Close();
    }
}
