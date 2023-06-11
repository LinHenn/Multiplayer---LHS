using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;



public class scoreCmd : MonoBehaviour
{
    public static scoreCmd instance;

    public List<pRegistry> pScores;

    private void Awake()
    {
        instance = this;
    }

    public void addPlayer(string nameP)
    {
        pScores[0].pScore.gameObject.SetActive(true);
        pScores[0].pScore.text = $"Score: {pScores[0].score}";
    }


    public void setPoint(string PlayerName)
    {
        pScores[0].score += 10;
        pScores[0].pScore.text = $"Score: {pScores[0].score}";
    }
}
