using UnityEngine;

public class GravityOnMove : MonoBehaviour
{
    public float distanceFromCamera = 1f;
    
    private Rigidbody rb;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private bool gravityEnabled = false;

    private void Start()
    {
        // Get the Rigidbody component attached to this GameObject
        rb = GetComponent<Rigidbody>();

        // Find the OVRCameraRig in the scene
        GameObject ovrCameraRig = GameObject.Find("OVRCameraRig");
        if (ovrCameraRig != null)
        {
            // Calculate the position in front of the OVRCameraRig
            Vector3 cameraPosition = ovrCameraRig.transform.position;
            Vector3 cameraForward = ovrCameraRig.transform.forward;
            Vector3 targetPosition = cameraPosition + cameraForward * distanceFromCamera;

            // Set the position of the GameObject
            transform.position = targetPosition;
        }
        else
        {
            Debug.LogError("OVRCameraRig not found in the scene!");
        }

        // Store the original position of the GameObject
        originalPosition = transform.position;
        originalRotation = transform.rotation;

        // Disable gravity initially
        rb.useGravity = false;
    }

    private void Update()
    {
        // Check if the GameObject has moved from its original position
        if (!gravityEnabled && transform.position != originalPosition)
        {
            // Enable gravity on the Rigidbody
            rb.useGravity = true;
            gravityEnabled = true;
        }

        if(OVRInput.GetDown(OVRInput.Button.Start))
        {
            transform.position = originalPosition;
            transform.rotation = originalRotation;

            rb.useGravity = false;
            gravityEnabled = false;
        }
    }
}