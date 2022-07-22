using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Video;

/// <summary>
/// Controla el panel de video que se activa desde la carta
/// </summary>
public class PanelVideo : Panel
{
    [SerializeField] [Tooltip("Reproductor de video del panel")]
    private VideoPlayer videoPlayer;
    [SerializeField] [Tooltip("Spinner de carga")]
    private GameObject spinner;

    /// <summary>
    /// Reproduce el video
    /// </summary>
    /// <param name="url">url del video</param>
    public void PlayVideo(string url)
    {
        StartCoroutine(downloadAndPlayVideo(url, "myvideo.mp4", true));
    }
    
    /// <summary>
    /// Corrutina de descarga y reproducción del video
    /// </summary>
    /// <param name="videoUrl">Url del video</param>
    /// <param name="saveFileName">Nombre del archivo</param>
    /// <param name="overwriteVideo">Determina si el video se puede sobreescribir</param>
    /// <returns></returns>
    IEnumerator downloadAndPlayVideo(string videoUrl, string saveFileName, bool overwriteVideo)
    {
        Debug.Log(videoUrl);
        //Where to Save the Video
        string saveDir = Path.Combine(Application.persistentDataPath, saveFileName);

        //Play back Directory
        string playbackDir = saveDir;
    #if UNITY_IPHONE
            playbackDir = "file://" + saveDir;
    #endif

        bool downloadSuccess = false;
        byte[] vidData = null;

        string[] persistantData = Directory.GetFiles(Application.persistentDataPath);
        if (((IList) persistantData).Contains(playbackDir) && !overwriteVideo)
        {
            Debug.Log("Video already exist. Playing it now");
            //Play Video
            playVideo(playbackDir);
            //EXIT
            yield break;
        }
        else if (((IList) persistantData).Contains(playbackDir) && overwriteVideo)
        {
            Debug.Log("Video already exist [but] we are [Re-downloading] it");
            yield return downloadData(videoUrl, (status, dowloadData) =>
            {
                downloadSuccess = status;
                vidData = dowloadData;
            });
        }
        else
        {
            Debug.Log("Video Does not exist. Downloading video");
            yield return downloadData(videoUrl, (status, dowloadData) =>
            {
                downloadSuccess = status;
                vidData = dowloadData;
            });
        }

        //Save then Play if there was no download error
        if (downloadSuccess)
        {
            //Save Video
            saveVideoFile(saveDir, vidData);

            //Play Video
            playVideo(playbackDir);
        }
    }

    /// <summary>
    /// Corrutina de descarga del video
    /// </summary>
    /// <param name="videoUrl">url del video</param>
    /// <param name="result">Resultado de la descarga del video en bytes</param>
    /// <returns></returns>
    IEnumerator downloadData(string videoUrl, Action<bool, byte[]> result)
    {
        //Download Video
        UnityWebRequest webRequest = UnityWebRequest.Get(videoUrl);
        webRequest.Send();

        //Wait until download is done
        while (!webRequest.isDone)
        {
            Debug.Log("Downloading: " + webRequest.downloadProgress);
            yield return null;
        }

        //Exit if we encountered error
        if (webRequest.isNetworkError)
        {
            Debug.Log("Error while downloading Video: " + webRequest.error);
            yield break; //EXIT
        }

        Debug.Log("Video Downloaded");
        //Retrieve downloaded Data
        result(!webRequest.isNetworkError, webRequest.downloadHandler.data);
    }

    /// <summary>
    /// Guarda el video en un archivo
    /// </summary>
    /// <param name="saveDir">Directorio donde se guarda el video</param>
    /// <param name="vidData">Datos del video en bytes</param>
    /// <returns></returns>
    bool saveVideoFile(string saveDir, byte[] vidData)
    {
        try
        {
            FileStream stream = new FileStream(saveDir, FileMode.Create);
            stream.Write(vidData, 0, vidData.Length);
            stream.Close();
            Debug.Log("Video Downloaded to: " + saveDir.Replace("/", "\\"));
            return true;
        }
        catch (Exception e)
        {
            Debug.Log("Error while saving Video File: " + e.Message);
        }
        return false;
    }

    /// <summary>
    /// Reproduce un video local
    /// </summary>
    /// <param name="path">Ubicación del local</param>
    void playVideo(string path)
    {
        videoPlayer.url = path;
        videoPlayer.Play();
        spinner.SetActive(false);
    }
}
