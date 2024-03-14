using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class MemeWebRequest : MonoBehaviour
{

    private Memes memes;

    public List<UnityEngine.UI.Image> imageMemeArray;
    public Dropdown dropdown;
    private string subReddit;

    void Start()
    {
        ApiCall();
    }

    public void ApiCall(){
        StopAllCoroutines();
        StartCoroutine(GetRequest("https://meme-api.com/gimme"+subReddit+"/2"));
    }

    private void CheckSubredit(){
        string valueDropdown = dropdown.options[dropdown.value].text;
        if(valueDropdown!="aleatorio"){
            subReddit = "/"+valueDropdown; 
        }
        else{
            subReddit = "";
        }

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

                    //Comprobamos is tiene un gif, si lo tiene volvemos a llamar a la api
                    foreach(Meme meme in memes.memes){
                        if(meme.url.Contains(".gif")){
                            ApiCall();
                            break;
                        }
                    }
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
            www = UnityWebRequestTexture.GetTexture(meme.url);

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

                Material material = new Material(Shader.Find("UI/Default"));
                material.mainTexture = myTexture;
                
                int index = memes.memes.IndexOf(meme);
                
                if(index<0){
                    index*=-1;
                }

                imageMemeArray[index].material = material;
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