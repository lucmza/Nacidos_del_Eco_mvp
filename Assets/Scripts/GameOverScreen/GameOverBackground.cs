using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverBackground : MonoBehaviour
{
    [Header("Crashed Screens")]
    [SerializeField] GameObject screen1;
    [SerializeField] GameObject screen2;
    [SerializeField] GameObject screen3;

    [Header("Comportamientos")]
    [SerializeField] float screen2VelRotation;
    [SerializeField] float screen2RangeRotation;
    [SerializeField] float screen3VelRotation;
    [SerializeField] float screen3RangeRotation;
    [SerializeField] float velScale;
    [SerializeField] float maxScale;

    float screen2OriginRot;
    float screen3OriginRot;


    // Start is called before the first frame update
    void Start()
    {
        screen2OriginRot = screen2.GetComponent<RectTransform>().localEulerAngles.z;
        screen3OriginRot = screen3.GetComponent<RectTransform>().localEulerAngles.z;
    }

    // Update is called once per frame
    void Update()
    {
        Screen1Move();
        Screen2Move();
        Screen3Move();
    }

    void Screen1Move()
    {
        screen1.transform.localScale += Vector3.one * Time.deltaTime * velScale /100;

        if (screen1.transform.localScale.y >= maxScale)
        {
            velScale *= -1;
        }
        if (screen1.transform.localScale.y <= 1)
        {
            velScale *= -1;
        }
    }

    void Screen2Move()
    {
        screen2.GetComponent<RectTransform>().Rotate(0, 0, screen2VelRotation * Time.deltaTime);
        if(screen2.GetComponent<RectTransform>().localEulerAngles.z >= screen2OriginRot + screen2RangeRotation)
        {
            screen2VelRotation *= -1;
        }
        if(screen2.GetComponent<RectTransform>().localEulerAngles.z <= screen2OriginRot)
        {
            screen2VelRotation *= -1;
        }
    }

    void Screen3Move()
    {
        screen3.GetComponent<RectTransform>().Rotate(0, 0, screen3VelRotation * Time.deltaTime * -1);
        if (screen3.GetComponent<RectTransform>().localEulerAngles.z <= screen3OriginRot - screen3RangeRotation)
        {
            screen3VelRotation *= -1;
        }
        if (screen3.GetComponent<RectTransform>().localEulerAngles.z >= screen3OriginRot)
        {
            screen3VelRotation *= -1;
        }
    }
}
