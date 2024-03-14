using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WebRequestExample : MonoBehaviour
{
    private Questions result;

    void Start()
    {
        // A correct website page.
        StartCoroutine(GetRequest("https://opentdb.com/api.php?amount=10"));

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

                    JsonToResult(webRequest.downloadHandler.text);

                    break;
            }
        }
    }

    private void JsonToResult(string json){
        result = JsonUtility.FromJson<Questions>(json);
        Debug.Log("Numero de preguntas: "+result.results.Count+" \n4 parametros: \npregunta: "+result.results[0].question+" \nDificultad: "+result.results[0].difficulty+" \nCategoria: "+result.results[0].category+" \nRespuesta correcta "+result.results[0].correct_answer);
    }
}


