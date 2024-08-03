using System;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameCameraContext {

    public Camera gameCamera;
    public GameCameraStateMachine cameraStateMachine;
    public AntStateMachine target;
    public InputActionAsset inputActions;
    public GameObject cameraTarget;

    //public Dictionary<String, CinemachineVirtualCamera> VirtualCameras;

    // Settings like move speed, rotate etc
    [Header("Camera Settings")]
    public float topViewMoveSpeed = 5f;

    public GameCameraContext(GameCameraStateMachine stateManager, Camera GameCamera, InputActionAsset inputActions) {
        gameCamera = GameCamera;
        cameraStateMachine = stateManager;
        this.inputActions = inputActions; 
        
        //VirtualCameras.Add("TopDownView", cameraStateMachine.gameObject.transform.Find("Top Down Follow Camera").GetComponent<CinemachineVirtualCamera>());
        //VirtualCameras.Add("TargetView", cameraStateMachine.gameObject.transform.Find("Active Orbit Camera").GetComponent<CinemachineVirtualCamera>());
        //cameraTarget = cameraStateMachine.GetComponentInChildren<GameObject>();
    }

}