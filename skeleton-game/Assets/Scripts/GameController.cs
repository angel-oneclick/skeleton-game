/*
 * Copyright © 2021+ Oneclick
 * Created in December 2021
 * by Ángel
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{

    private enum Status
    {
        IDLE, PLAYING
    }

    struct Hud
    {
        public GameObject targetText;
        public GameObject timeText;
        public GameObject scoreText;
        public GameObject correctImage;
        public GameObject wrongImage;
    }

    struct EndPanel
    {
        public GameObject scoreText;
    }

    private const float gameDuration = 60.0f;           // Duración de una partida en segundos
    private const float iconDuration =  0.5f;
    
    private Status      status;
    private float       elapsedTime;
    private int         score;

    private int         targetIndex;
    private bool     [] usedBones;

    private Hud         hud;
    private EndPanel    endPanel;

    private bool        iconDisplayed;
    private float       iconElapsed;

    void Start ()
    {
        status = Status.IDLE;

        hud.targetText     = GameObject.Find (    "TargetText");
        hud.timeText       = GameObject.Find (      "TimeText");
        hud.scoreText      = GameObject.Find (     "ScoreText");
        hud.correctImage   = GameObject.Find (  "CorrectImage");
        hud.wrongImage     = GameObject.Find (    "WrongImage");
        endPanel.scoreText = GameObject.Find ("OutroScoreText");
    }

    /// Esta función la llama el UIHandler cuando se pulsa el botón de empezar partida.
    public void StartGame ()
    {
        status      = Status.PLAYING;
        elapsedTime = 0.0f;
        score       = 0;

        usedBones   = new bool[Bone.names.Count];

        SelectNextBone ();
        UpdateScore    ();
        HideIcons      ();
    }

    /// Esta función la llama el propio GameController cuando el tiempo de la partida termina.
    private void EndGame ()
    {
        status = Status.IDLE;

        endPanel.scoreText.GetComponent<Text> ().text = score.ToString ();

        FindObjectOfType<UIHandler> ().EndGame();
    }

    void Update ()
    {
        if (status == Status.PLAYING)
        {
            // Se ajusta la cuenta atrás:

            elapsedTime += Time.deltaTime;

            if (elapsedTime >= gameDuration)
            {
                elapsedTime  = gameDuration;

                EndGame ();
            }

            hud.timeText.GetComponent<Text> ().text = (gameDuration - elapsedTime).ToString ("0.0");

            // Se oculta el icono que pueda estar siendo mostrado pasado un tiempo:

            if (iconDisplayed)
            {
                iconElapsed += Time.deltaTime;

                if (iconElapsed > iconDuration)
                {
                    HideIcons ();
                }
            }
        }
    }

    public void Raycast (Ray ray)
    {
        if (Physics.Raycast (ray, out RaycastHit hit))
        {
            if (hit.collider.gameObject.name == Bone.names[targetIndex])
            {
                UpdateScore (10);

                Show (hud.correctImage, ray);

                SelectNextBone ();
            }
            else
            {
                UpdateScore (-1);
                
                Show (hud.wrongImage, ray);
            }
        }
        else
        { 
            UpdateScore (-1);

            Show (hud.wrongImage, ray);
        }
    }

    private void SelectNextBone ()
    {
        int start = Random.Range (0, Bone.names.Count - 1);
        int index = start;

        while (usedBones[index] == true)
        {
            index++;

            if (index == Bone.names.Count)
            {
                index = 0;
            }

            if (index == start)
            {
                usedBones = new bool[Bone.names.Count];
                break;
            }
        }

        usedBones[targetIndex = index] = true;

        hud.targetText.GetComponent<Text> ().text = "Busca '" + Bone.Name.translations["es"][targetIndex] + "'";
    }

    private void UpdateScore (int increment = 0)
    {
        score += increment;

        if (score < 0) score = 0;

        hud.scoreText.GetComponent<Text> ().text = score.ToString () + " puntos";
    }

    private void HideIcons ()
    {
        hud.correctImage.SetActive (false);
        hud.  wrongImage.SetActive (false);

        iconDisplayed = false;
    }

    private void Show (GameObject image, Ray ray)
    {
        var pos = ray.origin;
        var vec = ray.direction;

        pos.x += 2.0f * vec.x / vec.z;              // 2.0f es la distancia del plano de proyección (origin.z = -4) al
        pos.y += 2.0f * vec.y / vec.z;              // plano de la UI (ui.z = -2), que es donde se encuentran las imágenes
        pos.z += 2.0f;

        image.transform.position = pos;
        
        image.SetActive (true);

        iconDisplayed = true;
        iconElapsed   = 0;
    }

}
