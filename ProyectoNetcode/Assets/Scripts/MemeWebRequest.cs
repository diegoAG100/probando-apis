using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MemeWebRequest : MonoBehaviour
{

    Meme meme1;
    Meme meme2;

    public Image imageMeme1;
    public Image imageMeme2;

    void Start()
    {
        // https://imgflip.com/api
        StartCoroutine(GetRequest("https://meme-api.com/gimme",meme1,imageMeme1));

        StartCoroutine(GetRequest("https://meme-api.com/gimme",meme2,imageMeme2));
    }

    IEnumerator GetRequest(string uri,Meme meme,Image imageMeme)
    {
        
        yield return new WaitForSeconds(UnityEngine.Random.Range(0f,1f));
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

                    meme = JsonToResult(webRequest.downloadHandler.text);

                    StartCoroutine(GetTexture(meme,imageMeme));

                    break;
            }
        }
    }

    private Meme JsonToResult(string json){
        
        return JsonUtility.FromJson<Meme>(json);
    }

    IEnumerator GetTexture(Meme meme,Image imageMeme)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(meme.url);

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

            imageMeme.material = material;
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