using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MemeWebRequest : MonoBehaviour
{

    Meme meme1;
    Meme meme2;

    public Image _EventTexture;

    void Start()
    {
        // https://imgflip.com/api
        StartCoroutine(GetRequest("https://meme-api.com/gimme"));

    }

    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log(webRequest.downloadHandler.text);

                    JsonToResult(webRequest.downloadHandler.text);

                    StartCoroutine(GetTexture());

                    break;
            }
        }
    }

    private void JsonToResult(string json){
        meme1 = JsonUtility.FromJson<Meme>(json);
        Debug.Log(meme1.url);
    }

    IEnumerator GetTexture()
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(meme1.url);

        yield return www.SendWebRequest();


        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Texture2D myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            
            //Sprite.Create(myTexture,new Rect(0,0,myTexture.width,myTexture.height),Vector2.zero)

            var material = new Material(Shader.Find("UI/Default"));
            material.mainTexture = myTexture;

            _EventTexture.material = material;
        }
    }

}

[Serializable] public class Meme{
    public string postLink;
    public string subreddit;
    public string title;
    public string url;
    public string nsfw;
    public string spoiler; 
    public string author;
    public string ups;
    public string[] preview;
}