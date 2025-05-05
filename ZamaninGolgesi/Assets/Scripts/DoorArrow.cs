using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorArrow : MonoBehaviour
{
    public Transform target;
    public RectTransform arrowUI;
    public Camera cam;
    public float edgePadding = 30f;

    void Start()
    {
        
    }
    void Update()
    {
        Vector3 viewportPos = cam.WorldToViewportPoint(target.position);
        bool isOffScreen = viewportPos.x < 0 || viewportPos.x > 1 || 
                           viewportPos.y < 0 || viewportPos.y > 1;
        arrowUI.gameObject.SetActive(isOffScreen);
        if (isOffScreen)
        {
            Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
            Vector3 screenPos = cam.WorldToViewportPoint(target.position);
            Vector3 dir = (screenPos - screenCenter).normalized;
            
            Vector3 arrowPos = screenCenter + dir * ((Screen.height / 2f) - edgePadding);
            arrowUI.position = ClampToScreen(arrowPos);
            
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            arrowUI.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    private Vector3 ClampToScreen(Vector3 pos)
    {
        pos.x = Mathf.Clamp(pos.x, edgePadding, Screen.width - edgePadding);
        pos.y = Mathf.Clamp(pos.y, edgePadding, Screen.height - edgePadding);
        return pos;
    }
}
