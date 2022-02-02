/*
 * Copyright � 2021+ Oneclick
 * Created in December 2021
 * by �ngel
 */

using UnityEngine;

public class GameInputHandler : MonoBehaviour
{

    private enum PointerPhase
    {
        AWAY, STARTED, STILL, MOVED, ENDED, ENDED_AND_RESTARTED
    }

    private struct PointerStatus
    {
        public bool         pressed;
        public PointerPhase phase;
        public Vector3      position;
        public Vector3      previousPosition;
    }

    private const float   rotationScale     = -250.00f;     // Escala la rotaci�n en radianes (en sentido contrario) cuando se convierte de desplazamiento unitario a �ngulo
    private const float   displacementScale =   10.00f;
    private const float   zoomScale         =   50.00f;

    private const float   minPositionY      =   14.00f;
    private const float   maxPositionY      =   28.20f;
    private const float   minFieldOfView    =   25.00f;
    private const float   maxFieldOfView    =   75.00f;

    private const float   dragThreshold     =    0.01f;

    private PointerStatus pointer;
    private Vector3       pointerStartPosition;

    [SerializeField] GameObject target;                     // Referencia al GameObject del esqueleto que ser� movido/rotado
    
    private UIHandler      uiHandler;
    private GameController gameController;

    void Start ()
    {
        pointer.pressed      = false;
        pointer.phase        = PointerPhase.AWAY;
        pointer.position     = pointer.previousPosition = CurrentPointerPosition ();
        pointerStartPosition = pointer.position;
        uiHandler            = FindObjectOfType<UIHandler> ();
        gameController       = FindObjectOfType<GameController> ();
    }

    void Update ()
    {
        if (uiHandler.status == UIHandler.Status.PLAYING)
        { 
            HandlePointer ();
            HandleZoom    ();
        }
    }

    private void HandlePointer()
    {
        UpdatePointerStatus ();

        switch (pointer.phase)
        {
            case PointerPhase.STARTED:
            {
                pointerStartPosition = pointer.position;
                break;
            }

            case PointerPhase.MOVED:
            {
                ApplyDragAction ();
                break;
            }

            case PointerPhase.ENDED:
            case PointerPhase.ENDED_AND_RESTARTED:
            {
                if ((pointer.position - pointerStartPosition).magnitude < dragThreshold)
                {
                    gameController.Raycast (RayFromPointerPosition ());
                }

                break;
            }
        }
    }

    private void HandleZoom ()
    {
        float zoomMagnitude = GetZoomMagnitude ();
        
        if (zoomMagnitude != 0)
        {
            float newFieldOfView = Camera.main.fieldOfView + zoomMagnitude * zoomScale;
            
            if (newFieldOfView < minFieldOfView) newFieldOfView = minFieldOfView; else
            if (newFieldOfView > maxFieldOfView) newFieldOfView = maxFieldOfView;
             
            Camera.main.fieldOfView = newFieldOfView;
        }
    }

    private void UpdatePointerStatus ()
    {
        pointer.previousPosition = pointer.position;            // No es posible detectar un cambio de posici�n
        pointer.position = CurrentPointerPosition ();

        if (pointer.phase == PointerPhase.ENDED_AND_RESTARTED)
        {
            pointer.phase =  PointerPhase.STARTED;
        }

        var pointerWentDown = PointerWentDown ();
        var pointerWentUp   = PointerWentUp   ();

        if (pointerWentDown)
        {
            if (pointerWentUp)
            {
                // Si se llega aqu� ocurrieron los eventos up y down simult�neamente pero, �en qu� orden?:
                // (es una situaci�n extra�a que no s� si podr�a llegar a ocurrir)

                if (pointer.pressed)    // Estaba presionado: se solt� y se volvi� a presionar r�pidamente
                {
                    pointer.phase    = PointerPhase.ENDED_AND_RESTARTED;    // En el siguiente update se deber�a cambiar a STARTED
                    pointer.pressed  = true;
                }
                else                    // No estaba presionado: se apret� y se volvi� a soltar (single click muy r�pido)
                {
                    pointer.phase            = PointerPhase.ENDED;
                    pointer.pressed          = false;
                    pointer.previousPosition = pointer.position;            // No es posible detectar un cambio de posici�n
                }
            }
            else
            {
                pointer.phase   = PointerPhase.STARTED;
                pointer.pressed = true;

                pointerStartPosition = pointer.position;
            }
        }
        else
        {
            if (pointer.pressed)
            {
                if (pointerWentUp)
                {
                    pointer.phase   = PointerPhase.ENDED;
                    pointer.pressed = false;
                }
                else
                { 
                    // Si el puntero estaba presionado y no se solt�, quiz�s se movi�:

                    if ((pointer.position - pointer.previousPosition).sqrMagnitude > 0)
                    {
                        pointer.phase = PointerPhase.MOVED;
                    }
                    else
                    {
                        pointer.phase = PointerPhase.STILL;
                    }
                }
            }
            else
                pointer.phase = PointerPhase.AWAY;      // No est� presionado ni hubo alg�n up/down
        }
    }

    private bool PointerWentDown ()
    {
        return Input.GetMouseButtonDown (0);
    }

    private bool PointerWentUp ()
    {
        return Input.GetMouseButtonUp (0);
    }

    private Vector3 CurrentPointerPosition ()
    {
        var position = Camera.main.ScreenToViewportPoint (Input.mousePosition);     // Consigue el desplazamiento normalizado entre 0 y 1

        position.x  /= Camera.main.aspect;                  // Consigue que el desplazamiento normalizado en X sea proporcional al de Y

        return position;
    }

    private Ray RayFromPointerPosition ()
    {
        return Camera.main.ScreenPointToRay (Input.mousePosition);
    }

    // Actualmente (usando solo la rueda del rat�n) retorna -0.1, 0.0 � +0.1.
    // Si se implementa el pinch con dos punteros podr�a retornar otros valores proporcionales a la separaci�n
    // entre ellos, pero habr�a que escalarlos para que fuesen equiparables a los valores de la rueda.
    private float GetZoomMagnitude ()
    {
        return Input.GetAxis ("Mouse ScrollWheel");
    }

    private void ApplyDragAction ()
    {
        if (target == null)
        {
            Debug.LogWarning ("Can't move the target because it is null.");
        }
        else
        {
            Vector3 previousPosition =  target.transform.position;
            float   rotation         = (pointer.position.x - pointer.previousPosition.x) * (float)System.Math.PI * rotationScale;
            float   displacement     = (pointer.position.y - pointer.previousPosition.y) * displacementScale;

            target.transform.Rotate    (0,     rotation, 0);
            target.transform.Translate (0, displacement, 0);

            if (target.transform.position.y < minPositionY || target.transform.position.y > maxPositionY)
            {
                target.transform.position = previousPosition;
            }
        }
    }

}
