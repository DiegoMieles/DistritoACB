using UnityEngine;

public class SimulatedARCamera : MonoBehaviour
{
    public GameObject PlaneObject;

    void Start()
    {
        //this is checking for device!
        if (Application.isMobilePlatform)
        {
            GameObject cameraParent = new GameObject("camParent");
            cameraParent.transform.position = this.transform.position;
            this.transform.parent = cameraParent.transform;
            cameraParent.transform.Rotate(Vector3.right, 90); //This is for rotation of camera.
        }


        Input.gyro.enabled = true; // enabling the gyro sensor of your device make sure that your device has a gyro sensor!

        //In this part we are place camera texture on the plane object that we create!

        WebCamTexture webCameraTexture = new WebCamTexture();
        PlaneObject.GetComponent<MeshRenderer>().material.mainTexture = webCameraTexture;
        webCameraTexture.Play();
    }

    // Update is called once per frame

    void Update()
    {
        //It is for camera rotation of gyro sensor!
        Quaternion cameraRotation = new Quaternion(Input.gyro.attitude.x, Input.gyro.attitude.y,
            -Input.gyro.attitude.z, -Input.gyro.attitude.w);
        this.transform.localRotation = cameraRotation;
    }
}
