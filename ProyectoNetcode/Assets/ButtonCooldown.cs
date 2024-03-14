using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonCooldown : MonoBehaviour
{
    public Button[] buttons; 

    public void StartCooldown(){
        foreach(Button button in buttons){
            button.enabled = false;
        }
        StartCoroutine(Cooldown());
    }

    IEnumerator Cooldown(){
        yield return new WaitForSeconds(1);
        foreach(Button button in buttons){
            button.enabled = true;
        }
    }
}
