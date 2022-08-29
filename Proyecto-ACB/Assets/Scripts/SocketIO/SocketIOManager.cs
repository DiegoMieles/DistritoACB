using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using SocketIOClient;
using System;

public class SocketIOManager : MonoBehaviour
{
    public SocketIOUnity actualSocket;
    public void InitSocket()
    {
        var uri = new Uri("https://www.example.com");
        actualSocket = new SocketIOUnity(uri, new SocketIOOptions
        {
            Query = new Dictionary<string, string>
        {
            {"token", "UNITY" }
        }
        ,
            Transport = SocketIOClient.Transport.TransportProtocol.WebSocket
        });
    }
}
