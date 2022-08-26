using UnityEngine;

public class Portal : MonoBehaviour
{
    [Header("Main Settings")]
    public Portal linkedPortal;

    [SerializeField] private GameObject _screen;
    private Camera portalСam;

    [Header("Advanced Settings")]
    public float nearClipOffset = 0.05f;
    public float nearClipLimit = 0.2f;

    // Private variables
    private Camera _mainCamera;
    private Renderer _rendererScreen;
   

    private void Awake()
    {
        portalСam = GetComponentInChildren<Camera>();
        
    }

    private void Start()
    {
        _mainCamera = Camera.main;
        linkedPortal.portalСam.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
        _rendererScreen = _screen.GetComponent<MeshRenderer>();
        _screen.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = linkedPortal.portalСam.targetTexture;
    }

    private void Update()
    {
        // Skip rendering the view from this portal if player is not looking at the linked portal
        if (!VisibleFromCamera(linkedPortal._rendererScreen, _mainCamera))
        {
            return;
        }

        // Position
        Vector3 lookerPosition = linkedPortal.transform.worldToLocalMatrix.MultiplyPoint3x4(Camera.main.transform.position);
        lookerPosition = new Vector3(-lookerPosition.x, lookerPosition.y, -lookerPosition.z);
        portalСam.transform.localPosition = lookerPosition;

        // Rotation
        Quaternion difference = transform.rotation * Quaternion.Inverse(linkedPortal.transform.rotation * Quaternion.Euler(0, 180, 0));
        portalСam.transform.rotation = difference * Camera.main.transform.rotation;

        // Clipping
        portalСam.nearClipPlane = lookerPosition.magnitude;
        //SetNearClipPlane();
    }


    //additional functions
    private bool VisibleFromCamera(Renderer renderer, Camera camera)
    {
        Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(camera);
        return GeometryUtility.TestPlanesAABB(frustumPlanes, renderer.bounds);
    }

    // Use custom projection matrix to align portal camera's near clip plane with the surface of the portal
    // Note that this affects precision of the depth buffer, which can cause issues with effects like screenspace AO
    void SetNearClipPlane()
    {
        // Learning resource:
        // http://www.terathon.com/lengyel/Lengyel-Oblique.pdf
        Transform clipPlane = transform;
        int dot = System.Math.Sign(Vector3.Dot(clipPlane.forward, transform.position - portalСam.transform.position));

        Vector3 camSpacePos = portalСam.worldToCameraMatrix.MultiplyPoint(clipPlane.position);
        Vector3 camSpaceNormal = portalСam.worldToCameraMatrix.MultiplyVector(clipPlane.forward) * dot;
        float camSpaceDst = -Vector3.Dot(camSpacePos, camSpaceNormal) + nearClipOffset;

        // Don't use oblique clip plane if very close to portal as it seems this can cause some visual artifacts
        if (Mathf.Abs(camSpaceDst) > nearClipLimit)
        {
            Vector4 clipPlaneCameraSpace = new Vector4(camSpaceNormal.x, camSpaceNormal.y, camSpaceNormal.z, camSpaceDst);

            // Update projection based on new clip plane
            // Calculate matrix with player cam so that player camera settings (fov, etc) are used
            portalСam.projectionMatrix = _mainCamera.CalculateObliqueMatrix(clipPlaneCameraSpace);
        }
        else
        {
            portalСam.projectionMatrix = _mainCamera.projectionMatrix;
        }
    }
    void OnValidate()
    {
        if (linkedPortal != null)
        {
            linkedPortal.linkedPortal = this;
        }
    }

    ////teleporting
    //void OnTriggerEnter(Collider other)
    //{
    //    //var traveller = other.GetComponent<PortalTraveller>();
    //    //if (traveller)
    //    //{
    //        other.gameObject.layer = 9;
    //       // Debug.Log("enter");
    //   // }
    //}


    //private void OnTriggerStay(Collider other)
    //{
    //    //var traveller = other.GetComponent<PortalTraveller>();
    //    //if (traveller)
    //    //{
    //        float zPos = transform.worldToLocalMatrix.MultiplyPoint3x4(other.transform.position).z;

    //        if (zPos < 0) 
    //            Teleport(other.transform);
    //   // }
    //}

    //private void Teleport(Transform obj)
    //{
    //    Debug.Log("teleported");
    //    // Position
    //    Vector3 localPos = transform.worldToLocalMatrix.MultiplyPoint3x4(obj.position);
    //    localPos = new Vector3(-localPos.x, localPos.y, -localPos.z);
    //    obj.position = Vector3.up; //linkedPortal.transform.localToWorldMatrix.MultiplyPoint3x4(localPos);

    //    // Rotation
    //    Quaternion difference = linkedPortal.transform.rotation * Quaternion.Inverse(transform.rotation * Quaternion.Euler(0, 180, 0));
    //    obj.rotation = difference * obj.rotation;
    //}


    //private void OnTriggerExit(Collider other)
    //{
    //    //var traveller = other.GetComponent<PortalTraveller>();
    //    //if (traveller)
    //    //{
    //        other.gameObject.layer = 8;
    //     //   Debug.Log("exit");
    //    //}
    //}
}
