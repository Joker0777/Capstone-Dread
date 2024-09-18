using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    public CinemachineVirtualCamera VirtualCamera { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        VirtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        if (VirtualCamera == null)
        {
            Debug.LogError("CinemachineVirtualCamera not found in the scene.");
        }
    }
}
