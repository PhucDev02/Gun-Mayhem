using System;
using Unity.Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] CinemachineCamera cam;
    [SerializeField] float maxZoomSize, minZoomSize;
    public static float meanDistancePlayers;
    private void Update()
    {
        cam.Lens.OrthographicSize = Mathf.Lerp(cam.Lens.OrthographicSize, GetComfortableSize(), 2.5f * Time.deltaTime);
    }

    private float GetComfortableSize()
    {
        return Mathf.Clamp(meanDistancePlayers, minZoomSize, maxZoomSize);
    }
}
