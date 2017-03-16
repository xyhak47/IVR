using UnityEngine;


public class IVRCamera : MonoBehaviour
{
    public Camera MainCamera;


#if CHILDREN_VR
    // Config IPD parameters for Children VR
    public float IPD = 61f;
#else
    // Config IPD parameters for Nebula VR
    public float IPD = 64f;
#endif

    private const float EyeHeight = 0.15f;


    private float K0, K1, K2;

    private const float DistK0 = 1.0f;
    private const float DistK1 = 0.22f;
    private const float DistK2 = 0.24f;

    private const float DistK0_ChildrenVR = 1.0f;
    private const float DistK1_ChildrenVR = 0.034f;
    private const float DistK2_ChildrenVR = 0.004f;

    private Vector2 DistortionCenter = new Vector2(0.5f, 0.5f);
    const float ScaleIn = 2.0f;

    const float DistortionFitX = 0.0f;
    const float DistortionFitY = 1.0f;
    const float DistortionFitScale = 1.0f;


    void Start()
    {
        InitIVRCamera();
    }

    private void InitIVRCamera()
    {
        Camera CameraLeft = Instantiate(MainCamera, Vector3.zero, Quaternion.identity) as Camera;
        Camera CameraRight = Instantiate(MainCamera, Vector3.zero, Quaternion.identity) as Camera;

        GameObject Head = new GameObject("Head");
        Head.transform.parent = this.transform;
        Head.transform.localPosition = Vector3.zero;
        Head.transform.localRotation = Quaternion.identity;

        CameraLeft.transform.parent = Head.transform;
        CameraRight.transform.parent = Head.transform;

        MainCamera.transform.localPosition = new Vector3(0, EyeHeight, 0);
        CameraLeft.transform.localPosition = new Vector3(IPD / 2000 * (-1), EyeHeight, 0);
        CameraRight.transform.localPosition = new Vector3(IPD / 2000 * 1, EyeHeight, 0);
        CameraLeft.transform.localRotation = Quaternion.identity;
        CameraRight.transform.localRotation = Quaternion.identity;

        MainCamera.GetComponent<Camera>().rect = new Rect(0.25f, 0, 0.5f, 1);
        CameraLeft.GetComponent<Camera>().rect = new Rect(0, 0, 0.5f, 1);
        CameraRight.GetComponent<Camera>().rect = new Rect(0.5f, 0, 0.5f, 1);

        ConfigureCameraLensCorrection(ref CameraLeft);
        ConfigureCameraLensCorrection(ref CameraRight);
    }

    private void ConfigureCameraLensCorrection(ref Camera camera)
    {
        IVRPostEffect PostEffect = camera.GetComponent<IVRPostEffect>();
        PostEffect.Scale = CalculateDistortionScale();
        PostEffect.ScaleIn = new Vector2(2.0f, 2.0f);
        PostEffect.K0 = K0;
        PostEffect.K1 = K1;
        PostEffect.K2 = K2;
    }

    private Vector2 CalculateDistortionScale()
    {
#if CHILDREN_VR
        k0 = DistK0_ChildrenVR;
        K1 = DistK1_ChildrenVR;
        K2 = DistK2_ChildrenVR;
#else
        K0 = DistK0;
        K1 = DistK1;
        K2 = DistK2;
#endif

        float StereoAspect = 0.5f * Screen.width / Screen.height;

        float DX = (DistortionFitX * DistortionFitScale) * StereoAspect;
        float DY = (DistortionFitY * DistortionFitScale);
        float FitRadius = Mathf.Sqrt(DX * DX + DY * DY);

        // This should match distortion equation used in shader.
        float Srq = FitRadius * FitRadius;
        float Scale = FitRadius * (K0 + K1 * Srq + K2 * Srq * Srq);
        Scale /= FitRadius;
        Scale = 1 / Scale;

        Vector2 DistortionScale = new Vector2();
        DistortionScale.x = Scale * 0.5f;
        DistortionScale.y = Scale * 0.5f; //  *StereoAspect;

        return DistortionScale;
    }
}
