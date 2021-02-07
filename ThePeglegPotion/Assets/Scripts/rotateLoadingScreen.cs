using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotateLoadingScreen : MonoBehaviour
{
    [SerializeField]
    private float speed;
    [SerializeField]
    private bool spinMode;
    [SerializeField]
    private Vector2 limits;
    [SerializeField]
    private bool clockwise;
    public float actualAngle;

    // Start is called before the first frame update
    void Start()
    {
        if (limits[0] > limits[1])
        {
            float temp = limits[1];
            limits[1] = limits[0];
            limits[0] = temp;
        }
    }

    // Update is called once per frame
    void Update()
    {
        float elapsed = Time.deltaTime;
        float angle = this.transform.localEulerAngles.z;

        float newAngle = angle + ((elapsed + speed) * (clockwise ? -1 : 1));
        if (!spinMode)
        {
            if (newAngle - 360 < limits[0] && clockwise)
            {
                newAngle = limits[0] + 360;
                clockwise = false;
            }
            if (newAngle > limits[1] && !clockwise)
            {
                newAngle = limits[1];
                clockwise = true;
            }
        }
        actualAngle = newAngle;
        this.transform.rotation = Quaternion.Euler(new Vector3( 0, 0, newAngle));
    }
}
