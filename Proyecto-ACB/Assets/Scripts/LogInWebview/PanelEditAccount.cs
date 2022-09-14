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
        ACBSingleton.Instance.onUserDeleted += OnDeleted;
    }

    /// <summary>
    /// Se ejecuta cuando se destruye el panel y desuscribe el evento de autenticaci�n al singleton principal de juego
    /// </summary>
    public void OnDestroy()
    {
        ACBSingleton.Instance.onUserDeleted -= OnDeleted;
    }

    /// <summary>
    /// Cierra el panel al autenticar al jugador
    /// </summary>
    private void OnDeleted()
    {
        Close();
    }
}