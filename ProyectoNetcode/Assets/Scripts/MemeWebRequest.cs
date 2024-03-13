using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MemeWebRequest : MonoBehaviour
{

    private Memes memes;

    public List<Image> imageMemeArray;

    void Start()
    {
        ApiCall();
    }

    public void ApiCall(){
        StartCoroutine(GetRequest("https://meme-api.com/gimme/2"));
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

                    memes = JsonToResult(webRequest.downloadHandler.text);

                    Debug.Log(webRequest.downloadHandler.text);

                    StartCoroutine(GetTextures());

                    break;
            }
            webRequest.Dispose();
        }
    }

    private Memes JsonToResult(string json){
        
        return JsonUtility.FromJson<Memes>(json);
    }

    IEnumerator GetTextures()
    {
        
        foreach(Meme meme in memes.memes){
            UnityWebRequest www;
            if(meme.url.Reverse().Take(3)=="gif"){
            string imageUrl = meme.url.Replace("gif","jpg");
            www = UnityWebRequestTexture.GetTexture(meme.url);
            }
            else{
            www = UnityWebRequestTexture.GetTexture(meme.url);
            }

            yield return www.SendWebRequest();


            if (www.isNetworkError || www.isHttpError)
            {
               Debug.Log(www.error);
            }
            else
            {
                Debug.Log(meme.url+" "+memes.memes.Count+" "+ imageMemeArray.Count);
                Texture2D myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            
             //Sprite.Create(myTexture,new Rect(0,0,myTexture.width,myTexture.height),Vector2.zero)

            var material = new Material(Shader.Find("UI/Default"));
            material.mainTexture = myTexture;

            imageMemeArray[memes.memes.IndexOf(meme)].material = material;
            www.Dispose();
            }

        }
    }

}

[Serializable] public class Memes{
    public string count;
    public List<Meme> memes;
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