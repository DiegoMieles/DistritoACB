using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BestHTTP.SocketIO3;
using BestHTTP.SocketIO3.Events;
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
 public SocketManager actualSocket { get; private set; }
    public delegate void VoidDelegate();
    public VoidDelegate OnResponse;
    /// <summary>
    /// liga los eventos de onConnected y on disconnected del socket
    /// </summary>
    /// 
    public void SetupAuditorySocket(int AuditorySitID)
    {
        InitSocket();
        actualSocket.Socket.On("connect", () => SetupEvents(AuditorySitID));
        actualSocket.Socket.On<ConnectResponse>(SocketIOEventTypes.Connect, (ConnectResponse callback)=> { print("conetau"); });
        actualSocket.Open();
        /*   actualSocket.Socket.On(SocketIOEventTypes.Connect, (s, p, a) =>
          {
              print("connectado");
          });
          actualSocket.Socket.On(SocketIOEventTypes.Disconnect, (s, p, a) =>
          {
              print("desconnectado");
          });
          actualSocket.Socket.On("check_seat", (s, p, a) => { print("seat"); });*/

       
       
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
        actualSocket = new SocketManager(uri);
    }
    /// <summary>
    /// Eventos que se emiten y se reciben del socket
    /// </summary>
    private void SetupEvents(int AuditorySitID)
    {
       actualSocket.Socket.Emit("check_seat", AuditorySitID);
        actualSocket.Socket.On<string>("check_seat", (response) =>
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
      //  actualSocket?.Disconnect();
        receivedMessage = true;
    }

}
