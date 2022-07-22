using Firebase.Extensions;
using UnityEngine;

public class NotificationManager : MonoBehaviour
{
    Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;

    protected virtual void Start() 
    {
        Firebase.Messaging.FirebaseMessaging.TokenRegistrationOnInitEnabled = true;
        
        Firebase.Messaging.FirebaseMessaging.GetTokenAsync().ContinueWithOnMainThread(
            task => {
                var  token = task.Result;
                Debug.Log("Creado TOken: "+token);

            }
        );
        
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available) {
                InitializeFirebase();
            } else {
                Debug.LogError(
                    "Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }
    
    // End our messaging session when the program exits.
    private void OnDestroy() {
        Firebase.Messaging.FirebaseMessaging.MessageReceived -= OnMessageReceived;
        Firebase.Messaging.FirebaseMessaging.TokenReceived -= OnTokenReceived;
    }
    
    private void InitializeFirebase() {
        Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
        Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
        
        // This will display the prompt to request permission to receive
        // notifications if the prompt has not already been displayed before. (If
        // the user already responded to the prompt, thier decision is cached by
        // the OS and can be changed in the OS settings).
        Firebase.Messaging.FirebaseMessaging.RequestPermissionAsync().ContinueWithOnMainThread(
            task => {
                Debug.Log("RequestPermissionAsync");
            }
        );
    }
    
    private void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e) 
    {
        Debug.Log("Received a new message");
        var notification = e.Message.Notification;
        if (notification != null) {
            Debug.Log("title: " + notification.Title);
            Debug.Log("body: " + notification.Body);
            var android = notification.Android;
            if (android != null) {
                Debug.Log("android channel_id: " + android.ChannelId);
            }
        }
        if (e.Message.From.Length > 0)
            Debug.Log("from: " + e.Message.From);
        if (e.Message.Link != null) {
            Debug.Log("link: " + e.Message.Link.ToString());
        }
        if (e.Message.Data.Count > 0) {
            Debug.Log("data:");
            foreach (System.Collections.Generic.KeyValuePair<string, string> iter in
                e.Message.Data) {
                Debug.Log("  " + iter.Key + ": " + iter.Value);
            }
        }
    }

    private void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token) {
        Debug.Log("Received Registration Token: " + token.Token);
    }
}
