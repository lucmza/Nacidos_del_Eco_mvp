using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using static UnityEditor.PlayerSettings;

public class MiniMapEdgePoint : MonoBehaviour
{
   
    public Camera minimapCam;
    public RectTransform minimapRect;
    public RectTransform markerIcon;
    public Transform target;

    public float edgeOffset = 15f;

    void Update()
    {
        {
            if (minimapCam == null || minimapRect == null ||
                markerIcon == null || target == null) return;

            
            Vector3 vp3 = minimapCam.WorldToViewportPoint(target.position);

            if (vp3.z < 0f)
            {
                vp3.x = 1f - vp3.x;
                vp3.y = 1f - vp3.y;
            }

            Vector2 vpCenter = new Vector2(0.5f, 0.5f);
            Vector2 dir = new Vector2(vp3.x - vpCenter.x, vp3.y - vpCenter.y);
            dir.Normalize();
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            float halfSize = minimapRect.sizeDelta.x * 0.5f;
            float radius = halfSize - edgeOffset;

            Vector2 anchoredPos;
            if (vp3.x >= 0f && vp3.x <= 1f && vp3.y >= 0f && vp3.y <= 1f)
            {
             
                float xPos = (vp3.x - 0.5f) * minimapRect.sizeDelta.x;
                float yPos = (vp3.y - 0.5f) * minimapRect.sizeDelta.y;
                anchoredPos = new Vector2(xPos, yPos);
            }
            else
            {
              
                anchoredPos = dir * radius;
            }


            markerIcon.anchoredPosition = anchoredPos;
        }
    }
}
