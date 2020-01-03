using UnityEngine;

public class maxCamera : MonoBehaviour
{
    [SerializeField]
    private float lookSpeedH = 5f;

    [SerializeField]
    private float lookSpeedV = 5f;

    [SerializeField]
    private float zoomSpeed = 5f;

    [SerializeField]
    private float dragSpeed = 5f;

    private float yaw = 0f;
    private float pitch = 0f;

    private void Start()
    {
        // Initialize the correct initial rotation
        this.yaw = this.transform.eulerAngles.y;
        this.pitch = this.transform.eulerAngles.x;
    }

    private void Update()
    {
        // Only work with the Left Alt pressed

            //Look around with Left Mouse
            if (Input.GetMouseButton(1))
            {
                this.yaw += this.lookSpeedH * Input.GetAxis("Mouse X");
                this.pitch -= this.lookSpeedV * Input.GetAxis("Mouse Y");

                this.transform.eulerAngles = new Vector3(this.pitch, this.yaw, 0f);
            }
            //drag camera around with Middle Mouse
            else if (Input.GetMouseButton(2))
            {
                transform.Translate(-Input.GetAxisRaw("Mouse X") * Time.deltaTime * dragSpeed, -Input.GetAxisRaw("Mouse Y") * Time.deltaTime * dragSpeed, 0);
            }

            //Zoom in and out with Mouse Wheel
            this.transform.Translate(0, 0, Input.GetAxis("Mouse ScrollWheel") * this.zoomSpeed, Space.Self);
        
    }
}