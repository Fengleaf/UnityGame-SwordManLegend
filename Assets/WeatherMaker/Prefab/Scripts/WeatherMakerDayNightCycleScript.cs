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
    public class WeatherMakerDayNightCycleScript : MonoBehaviour
    {
        [Tooltip("The sun light to use for the day night cycle.")]
        public Light Sun;

        [Tooltip("The object that is displaying the sun.")]
        public GameObject SunDisplay;

        [Tooltip("The axis to rotate the sun around.")]
        public Vector3 SunDisplayAxis = Vector3.right;

        [Range(0.5f, 0.95f)]
        [Tooltip("Position the sun out by the sky sphere radius multiplied by this value.")]
        public float SunDisplayRadiusMultiplier = 0.85f;

        [Range(0.01f, 1.0f)]
        [Tooltip("The scale of the sun will be this value multiplied by the sky sphere radius.")]
        public float SunDisplayScaleMultiplier = 0.25f;

        [Range(-100000, 100000.0f)]
        [Tooltip("The speed of the cycle. At a speed of 1, the cycle is in real-time (24 hours). A speed of 100 is 0.24 hours for a full cycle. " +
            "Negative numbers run the cycle backwards.")]
        public float Speed = 100.0f;

        [Tooltip("Script to assist in positioning the sun.")]
        public WeatherMakerScript WeatherMakerScript;

        public const float SecondsForFullRotation = 86400.0f;
        public const float SecondsForOneDegree = SecondsForFullRotation / 360.0f;

        //private void DoDayNightCycle()
        //{
        //    if (Sun != null)
        //    {
        //        float rotationDelta = (Time.deltaTime / SecondsForOneDegree) * Speed;
        //        Sun.transform.Rotate(SunDisplayAxis, rotationDelta);

        //        if (WeatherMakerScript != null && SunDisplay != null)
        //        {
        //            Quaternion q = Sun.transform.rotation;
        //            //float radius = WeatherMakerScript.Camera.farClipPlane * WeatherMakerScript.SkySphereScript.FarClipPlaneMultiplier * SunDisplayRadiusMultiplier;
        //            Vector3 rotated = -(q * Vector3.forward * radius);
        //            rotated += WeatherMakerScript.Camera.transform.position;
        //            SunDisplay.transform.position = rotated;
        //            SunDisplay.transform.localScale = new Vector3(radius * SunDisplayScaleMultiplier, radius * SunDisplayScaleMultiplier, 1.0f);
        //            SunDisplay.SetActive(true);
        //        }
        //    }
        //}

        private void Start()
        {
            //DoDayNightCycle();
        }

        private void Update()
        {
            //DoDayNightCycle();
        }
    }
}