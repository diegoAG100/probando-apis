using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable] public class Question{
    public string type;
    public string difficulty;
    public string category;
    public string question;
    public string correct_answer;
    public string incorrect_answers; 
}