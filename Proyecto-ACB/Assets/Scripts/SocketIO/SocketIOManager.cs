using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using SocketIOClient;
using System;

public class SocketIOManager : MonoBehaviour
{
    [SerializeField]
    [Tooltip("url a la que se conectará el socket")]
    private string url = "http://distritoacb.seguimientooperativo.site:3000";
   public PanelControlDeAcceso PanelControlAcceso;
    public bool receivedMessage = false;
    public bool sended;
    /// <summary>
    /// socket creado
    /// </summary>
    public SocketIOUnity actualSocket { get; private set; }
    public delegate void VoidDelegate();
    public VoidDelegate OnResponse;
    /// <summary>
    /// liga los eventos de onConnected y on disconnected del socket
    /// </summary>
    /// 
    public void SetupAuditorySocket(int AuditorySitID)
    {
        InitSocket();
        actualSocket.OnConnected += (object sender, EventArgs e) => { SetupEvents(AuditorySitID); };

        actualSocket.OnError += ActualSocket_OnError;
    }
    /// <summary>
    /// error en la ejecución del socket
    /// </summary>
    private void ActualSocket_OnError(object sender, string e)
    {
        throw new NotImplementedException();
    }
    /// <summary>
    /// inicialización del socket
    /// </summary>
    private void InitSocket()
    {
        var uri = new Uri(url);
        actualSocket = new SocketIOUnity(uri);
        actualSocket.Connect();
    }
    /// <summary>
    /// Eventos que se emiten y se reciben del socket
    /// </summary>
    private void SetupEvents(int AuditorySitID)
    {
        actualSocket.Emit("check_seat", AuditorySitID);
        actualSocket.On("check_seat", (response) =>
       {
           FinishSocket();
       });
    }
    /// <summary>
    /// Desconecta el socket 
    /// </summary>
    public void FinishSocket()
    {
        print("finishSocket");
        actualSocket?.Disconnect();
        receivedMessage = true;
    }

}
