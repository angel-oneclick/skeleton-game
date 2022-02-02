/*
 * Copyright © 2021+ Oneclick
 * Created in December 2021
 * by Ángel
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHandler : MonoBehaviour
{

    public enum Status
    {
        SHOWING_INTRO,
        PLAYING,
        SHOWING_OUTRO,
    }

    private const float    outroDelay    = 1.75f;
    private const float    resetSpeedIn  = 0.01f;
    private const float    resetSpeedOut = 1.00f - resetSpeedIn;
    private const float    resetEpsilon  = 0.01f;

    private float          outroElapsed;

    private Vector3        skeletonInitialPosition;
    private Vector3        skeletonInitialRotation;
    private float          cameraInitialFov;

    private GameObject     hud;
    private GameObject     startPanel;
    private GameObject     endPanel;
    private GameObject     skeleton;

    public Status status
    {
        get;
        private set;
    }

    private void Start ()
    {
        status = Status.SHOWING_INTRO;

            startPanel = GameObject.Find ("StartPanel");
              endPanel = GameObject.Find ("EndPanel"  );
                   hud = GameObject.Find ("[hud]"     );
              skeleton = GameObject.Find ("Skeleton"  );

        startPanel.SetActive (true );
          endPanel.SetActive (false);
               hud.SetActive (false);

        skeletonInitialPosition = skeleton.transform.position;
        skeletonInitialRotation = skeleton.transform.rotation.eulerAngles;
        cameraInitialFov        = Camera.main.fieldOfView;
    }

    private void Update()
    {
        if (status == Status.SHOWING_OUTRO)
        {
            // Se espera un momento desde que termina la partida hasta que se muestra el panel final:

            outroElapsed += Time.deltaTime;

            if (outroElapsed > outroDelay)
            {
                endPanel.SetActive (true );
                     hud.SetActive (false);
            }

            // Se coloca al esqueleto en su posición y orientación iniciales:

            skeleton.transform.SetPositionAndRotation
            (
                resetStep (skeleton.transform.position, skeletonInitialPosition), 
                Quaternion.Euler (resetStep (skeleton.transform.rotation.eulerAngles, skeletonInitialRotation))
            );

            // Se ajusta el zoom de la cámara a su valor original:

            Camera.main.fieldOfView = resetStep (Camera.main.fieldOfView, cameraInitialFov);
        }
    }

    public void StartGame ()
    {
        status = Status.PLAYING;

        startPanel.SetActive (false);

        FindObjectOfType<GameController> ().StartGame ();

        hud.SetActive (true);
    }

    public void EndGame ()
    {
        status = Status.SHOWING_OUTRO;

        outroElapsed = 0;
    }

    public void CloseEndPanel ()
    {
        status = Status.SHOWING_INTRO;

        startPanel.SetActive (true );
          endPanel.SetActive (false);
    }

    private float resetStep (float a, float b)
    {
        return System.Math.Abs (a - b) < resetEpsilon ? b : resetSpeedOut * a + resetSpeedIn * b;
    }

    private Vector3 resetStep (Vector3 a, Vector3 b)
    {
        return (a - b).magnitude < resetEpsilon ? b : resetSpeedOut * a + resetSpeedIn * b;
    }

}
