using System;
using UnityEngine;
using UnityEngine.UI;
using WebAPI;
using Newtonsoft.Json;
using Data;

/// <summary>
/// Panel del cambio de nombre del jugador
/// </summary>
public class PanelChangeName : Panel
{
    [Tooltip("Datos del jugador")]
    public ScriptableAccount account;
    [Tooltip("Campo de escritura de texto donde se puede cambiar el nombre del jugador")]
    public InputField inputFieldNickname;

    private Action onFinishNicknameChange; //Acción que se ejecuta cuando el nombre del jugador es cambiado con éxtio

    /// <summary>
    /// Abre el panel
    /// </summary>
    /// <param name="onFinishNicknameChange">Acción que se ejecuta cuando el nombre del jugador es cambiado con éxtio</param>
    public void OpenChangeNameView(Action onFinishNicknameChange)
    {
        inputFieldNickname.text = account.avatarData.nickName;
        this.onFinishNicknameChange = onFinishNicknameChange;
    }

    /// <summary>
    /// Se ejecuta al aceptar el cambio de nombre del jugador
    /// </summary>
    public void OnAccept()
    {
        if(string.IsNullOrEmpty(inputFieldNickname.text))
            return;

        account.avatarData.nickName = inputFieldNickname.text;
        WebProcedure.Instance.PostSaveUserAvatar(JsonConvert.SerializeObject(account.avatarData), OnSuccess, OnFailed);
    }

    /// <summary>
    /// Este método se llama cuando el cambio de nombre no se ha podido efectuar en backend
    /// </summary>
    /// <param name="obj">Clase con los datos de error</param>
    private void OnFailed(WebError obj)
    {
        Debug.Log(obj.Message);
        ACBSingleton.Instance.AlertPanel.SetupPanel("Ha ocurrido un error por favor intenta nuevamente", "", false, Close);
    }

    /// <summary>
    /// Este método se llama cuando backend aprueba el cambio de nombre del jugador
    /// </summary>
    /// <param name="obj">Datos del cambio de nombre satisfactorio</param>
    private void OnSuccess(DataSnapshot obj)
    {
        Debug.Log(obj.RawJson);
        onFinishNicknameChange?.Invoke();
        Debug.Log(this.onFinishNicknameChange == null);
        Close();
    }
}
