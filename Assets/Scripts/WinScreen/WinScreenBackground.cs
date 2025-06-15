using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinScreenBackground : MonoBehaviour
{
    [Header("Background objects")]
    [SerializeField] GameObject destello;
    [SerializeField] GameObject fragmento1;
    [SerializeField] GameObject fragmento2;

    [Header("Destello")]
    [SerializeField] float scaleVelocity;
    [SerializeField] float scaleMax;

    [Header("Fragmentos")]
    [SerializeField] float moveVelocity;
    [SerializeField] float moveRange;
    float fragmento1InitialPosy;
    float fragmento2InitialPosy;

    private void Start()
    {
        fragmento1InitialPosy = fragmento1.GetComponent<RectTransform>().anchoredPosition.y;
        fragmento2InitialPosy = fragmento2.GetComponent<RectTransform>().anchoredPosition.y;
    }

    // Update is called once per frame
    void Update()
    {
        destello.transform.localScale += Vector3.one * Time.deltaTime * scaleVelocity /100;

        if (destello.transform.localScale.y >= scaleMax)
        {
            scaleVelocity *= -1;
        }
        if (destello.transform.localScale.y <= 1)
        {
            scaleVelocity *= -1;
        }

        fragmento1.GetComponent<RectTransform>().anchoredPosition += new Vector2(0,1) * moveVelocity * Time.deltaTime;
        if (fragmento1.GetComponent<RectTransform>().anchoredPosition.y >= fragmento1InitialPosy + moveRange)
        {
            moveVelocity *= -1;
        }
        if (fragmento1.GetComponent<RectTransform>().anchoredPosition.y < fragmento1InitialPosy)
        {
            moveVelocity *= -1;
        }

        fragmento2.GetComponent<RectTransform>().anchoredPosition += new Vector2(0, 1) * moveVelocity * Time.deltaTime * -1;
    }
}
