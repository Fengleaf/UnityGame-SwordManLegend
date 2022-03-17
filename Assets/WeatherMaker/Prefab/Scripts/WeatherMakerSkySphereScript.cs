//
// Weather Maker for Unity
// (c) 2016 Digital Ruby, LLC
// Source code may be used for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 

using UnityEngine;
using System.Collections;

namespace DigitalRuby.WeatherMaker
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class WeatherMakerSkySphereScript : MonoBehaviour
    {
        [Tooltip("The camera the sky sphere should follow")]
        public Camera Camera;

        [Header("Sun")]
        [Tooltip("Sun")]
        public Light Sun;

        [Header("Positioning")]
        [Range(-0.5f, 0.5f)]
        [Tooltip("Offset the sky this amount from the camera y. This value is multiplied by the height of the sky sphere.")]
        public float YOffsetMultiplier = 0.0f;

        [Range(0.1f, 0.99f)]
        [Tooltip("Place the sky sphere at this amount of the far clip plane")]
        public float FarClipPlaneMultiplier = 0.8f;

        [Header("Day / night transition")]
        [Tooltip("The daytime texture")]
        public Texture2D DayTexture;

        [Tooltip("The night time texture")]
        public Texture2D NightTexture;

        [Range(0.0f, 180.0f)]
        [Tooltip("It is fully day (i.e. no more night) when the sun x is at least this degrees above 0. 180 is high noon.")]
        public float DayDegrees = 95.0f;

        [Range(0.0f, 90.0f)]
        [Tooltip("The number of degrees that it fades from day to night before becoming fully night")]
        public float NightFadeDegrees = 30.0f;

        [Range(0.0f, 1.0f)]
        [Tooltip("Night pixels must have an R, G or B value greater than or equal to this to be visible. Raise this value if you want to hide dimmer elements " +
            "of your night texture or there is a lot of light pollution.")]
        public float NightVisibilityThreshold = 0.0f;

        [Header("Rotation / movement")]
        [Range(-1.0f, 1.0f)]
        [Tooltip("Rotation speed in degrees per second")]
        public float RotationSpeed;

        [Tooltip("The axis to rotate around")]
        public Vector3 RotationAxis = Vector3.up;

#if UNITY_EDITOR

        [Header("Generation of Sphere")]
        [Range(2, 6)]
        [Tooltip("Resolution of sphere. The higher the more triangles.")]
        public int Resolution = 4;

        [UnityEngine.HideInInspector]
        [UnityEngine.SerializeField]
        private int lastResolution = -1;

        [Tooltip("UV mode for sphere generation")]
        public UVMode UVMode = UVMode.PanoramaMirrorDown;

        [UnityEngine.HideInInspector]
        [UnityEngine.SerializeField]
        private UVMode lastUVMode = (UVMode)int.MaxValue;

#endif

        private MeshRenderer meshRenderer;
        private MeshFilter meshFilter;

        private void DestroyMesh()
        {
            if (meshFilter.sharedMesh != null)
            {
                GameObject.DestroyImmediate(meshFilter.sharedMesh);
                meshFilter.sharedMesh = null;
            }
        }

        private void UpdateSkySphere()
        {

#if UNITY_EDITOR

            if (meshRenderer.sharedMaterial == null)
            {
                return;
            }

#endif

            Camera c = (Camera == null ? (Camera.main == null ? Camera.current : Camera.main) : Camera);
            float farPlane = FarClipPlaneMultiplier * c.farClipPlane;
            float yOffset = farPlane * YOffsetMultiplier;
            gameObject.transform.position = c.transform.position + new Vector3(0.0f, yOffset, 0.0f);
            float scale = farPlane * ((c.farClipPlane - Mathf.Abs(yOffset)) / c.farClipPlane);
            gameObject.transform.localScale = new Vector3(scale, scale, scale);
            meshRenderer.sharedMaterial.mainTexture = DayTexture;
            meshRenderer.sharedMaterial.SetTexture("_NightTex", NightTexture);

            // always fully day between these two values
            float dayMaximum = 360.0f - DayDegrees;

            float dayMultiplier;
            float sunRotation = (Sun == null ? 90.0f : Sun.transform.eulerAngles.x) + 90.0f;
            if (sunRotation > 360.0f)
            {
                sunRotation -= 360.0f;
            }

            if (sunRotation >= DayDegrees && sunRotation <= dayMaximum)
            {
                dayMultiplier = 1.0f;
            }
            else if (sunRotation < (DayDegrees - NightFadeDegrees) || sunRotation > (dayMaximum + NightFadeDegrees))
            {
                dayMultiplier = 0.0f;
            }
            else if (sunRotation < DayDegrees)
            {
                dayMultiplier = Mathf.Lerp(1.0f, 0.0f, 1.0f - ((sunRotation - (DayDegrees - NightFadeDegrees)) / NightFadeDegrees));
            }
            else
            {
                dayMultiplier = Mathf.Lerp(1.0f, 0.0f, 1.0f - ((sunRotation - (360.0f - NightFadeDegrees)) / NightFadeDegrees));
            }
            meshRenderer.sharedMaterial.SetFloat("_DayMultiplier", dayMultiplier);
            meshRenderer.sharedMaterial.SetFloat("_NightMultiplier", 1.0f - dayMultiplier);
            meshRenderer.sharedMaterial.SetFloat("_NightVisibilityThreshold", NightVisibilityThreshold);

#if UNITY_EDITOR

            if (Resolution != lastResolution)
            {
                lastResolution = Resolution;
                DestroyMesh();
            }

            if (UVMode != lastUVMode)
            {
                lastUVMode = UVMode;
                DestroyMesh();
            }

            if (Application.isPlaying)
            {

#endif

                gameObject.transform.Rotate(RotationAxis, RotationSpeed * Time.deltaTime);

#if UNITY_EDITOR

            }

#endif

#if UNITY_EDITOR

            Mesh mesh = meshFilter.sharedMesh;
            if (mesh == null)
            {
                meshFilter.sharedMesh = SphereCreator.Create(Resolution, UVMode);
            }

#endif

        }

        private void Start()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            meshFilter = GetComponent<MeshFilter>();
            UpdateSkySphere();
        }

        private void Update()
        {
            UpdateSkySphere();
        }
    }
}