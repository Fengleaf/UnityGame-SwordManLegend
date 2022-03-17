//
// Weather Maker for Unity
// (c) 2016 Digital Ruby, LLC
// Source code may be used for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 

using UnityEngine;
using System.Collections;
using System;

namespace DigitalRuby.WeatherMaker
{
    public class WeatherMakerScript : MonoBehaviour
    {
        [Header("Precipitation Objects")]
        [Tooltip("Camera the weather should hover over. Defaults to main camera.")]
        public Camera Camera;

        [Tooltip("Rain object")]
        public GameObject Rain;

        //[Tooltip("Snow object")]
        //public GameObject Snow;

        //[Tooltip("Hail object")]
        //public GameObject Hail;

        //[Tooltip("Sleet object")]
        //public GameObject Sleet;

        //[Tooltip("Clouds object")]
        //public GameObject Clouds;

        //[Tooltip("Sky sphere object")]
        //public GameObject SkySphere;

        [Tooltip("Wind object")]
        public GameObject Wind;

        [Tooltip("The sun")]
        public Light Sun;

        [Tooltip("Whether the precipitation collides with the world")]
        public bool CollisionEnabled = true;

        [Tooltip("Whether to enable wind")]
        public bool WindEnabled;

        //[Tooltip("Configuration script. Should be deactivated for release builds.")]
        //public WeatherMakerConfigurationScript ConfigurationScript;

        [Tooltip("Lightning script")]
        public WeatherMakerThunderAndLightningScript LightningScript;

        //[Tooltip("Sky sphere script or null if no sky sphere")]
        //public WeatherMakerSkySphereScript SkySphereScript;

        //[Tooltip("Day night script or null if no day night script")]
        //public WeatherMakerDayNightCycleScript DayNightScript;

        [Tooltip("Intensity of precipitation (0-1)")]
        [Range(0.0f, 1.0f)]
        public float PrecipitationIntensity;

        [Tooltip("How long in seconds to fully change from one precipitation type to another")]
        [Range(0.0f, 300.0f)]
        public float PrecipitationChangeDuration = 4.0f;

        [Tooltip("The threshold change in intensity that will cause a cross-fade between precipitation changes. Intensity changes smaller than this value happen quickly.")]
        [Range(0.0f, 0.2f)]
        public float PrecipitationChangeThreshold = 0.1f;

        [NonSerialized]
        private float lastPrecipitationIntensityChange = -1.0f;

        private void TweenScript(WeatherMakerFallingParticleScript script, float end)
        {
            if (Mathf.Abs(script.Intensity - end) < PrecipitationChangeThreshold)
            {
                script.Intensity = end;
            }
            else
            {
                TweenFactory.Tween("WeatherMakerPrecipitationChange_" + script.gameObject.GetInstanceID(), script.Intensity, end, PrecipitationChangeDuration, TweenScaleFunctions.Linear, (t) =>
                {
                    // Debug.LogFormat("Tween key: {0}, value: {1}, prog: {2}", t.Key, t.CurrentValue, t.CurrentProgress);
                    script.Intensity = t.CurrentValue;
                }, null);
            }
        }

        private void ChangePrecipitation(WeatherMakerFallingParticleScript newPrecipitation)
        {
            if (newPrecipitation != currentPrecipitation && currentPrecipitation != null)
            {
                TweenScript(currentPrecipitation, 0.0f);
                lastPrecipitationIntensityChange = -1.0f;
            }
            currentPrecipitation = newPrecipitation;
        }

        private void UpdateCollision()
        {
            RainScript.CollisionEnabled = CollisionEnabled;
            //SnowScript.CollisionEnabled = CollisionEnabled;
            //HailScript.CollisionEnabled = CollisionEnabled;
            //SleetScript.CollisionEnabled = CollisionEnabled;
        }

        private void CalculateTimeOfDay()
        {
            Quaternion q = Sun.transform.rotation;
            float rotationDegrees = q.eulerAngles.x;
            if (q.eulerAngles.y != 0.0f || q.eulerAngles.z != 0.0f)
            {
                rotationDegrees = 180.0f + (360.0f - rotationDegrees);
            }
            rotationDegrees += 90.0f + (Camera.orthographic ? 90.0f : 0.0f);
            if (rotationDegrees > 360.0f)
            {
                rotationDegrees -= 360.0f;
            }
            float secondsElapsedThisDay = Mathf.Lerp(0.0f, WeatherMakerDayNightCycleScript.SecondsForFullRotation, rotationDegrees / 360.0f);
            TimeOfDay = System.TimeSpan.FromSeconds(secondsElapsedThisDay);

#if UNITY_EDITOR

            //if (ConfigurationScript != null && ConfigurationScript.TimeOfDayText != null && ConfigurationScript.TimeOfDayText.IsActive())
            //{
            //    ConfigurationScript.TimeOfDayText.text = string.Format("{0:00}:{1:00}:{2:00}", TimeOfDay.Hours, TimeOfDay.Minutes, TimeOfDay.Seconds);
            //}

#endif

        }

        private void Awake()
        {
            if (Camera == null)
            {
                Camera = Camera.main;
                if (Camera == null)
                {
                    Camera = Camera.current;
                }
            }
            CalculateTimeOfDay();
        }

        private void Start()
        {
            RainScript = Rain.GetComponent<WeatherMakerFallingParticleScript>();
            RainScript.Camera = Camera;
            //SnowScript = Snow.GetComponent<WeatherMakerFallingParticleScript>();
            //SnowScript.Camera = Camera;
            //HailScript = Hail.GetComponent<WeatherMakerFallingParticleScript>();
            //HailScript.Camera = Camera;
            //SleetScript = Sleet.GetComponent<WeatherMakerFallingParticleScript>();
            //SleetScript.Camera = Camera;
            //CloudScript = Clouds.GetComponent<WeatherMakerCloudScript>();
            //CloudScript.Camera = Camera;
            //SkySphereScript = null; //  SkySphere.GetComponent<WeatherMakerSkySphereScript>();
            //if (SkySphereScript != null)
            //{
            //    SkySphereScript.Camera = Camera;
            //}
            UpdateCollision();
        }

        private void Update()
        {
            if (currentPrecipitation != null && PrecipitationIntensity != lastPrecipitationIntensityChange)
            {
                lastPrecipitationIntensityChange = PrecipitationIntensity;
                TweenScript(currentPrecipitation, PrecipitationIntensity);
            }
            UpdateCollision();
            if (Wind != null)
            {
                Wind.SetActive(WindEnabled);
            }
            if (Sun != null && !Camera.orthographic)
            {
                float sunX = (Sun.transform.eulerAngles.x + 90.0f);
                sunX = (sunX >= 360.0f ? sunX - 360.0f : sunX);
                Sun.gameObject.SetActive(sunX > 45.0f && sunX < 295.0f);
            }
            //CalculateTimeOfDay();
        }

        public WeatherMakerFallingParticleScript CurrentPrecipitation
        {
            get { return currentPrecipitation; }
            set
            {
                if (value != currentPrecipitation)
                {
                    ChangePrecipitation(value);
                }
            }
        }

        private WeatherMakerFallingParticleScript currentPrecipitation;
        public WeatherMakerFallingParticleScript RainScript { get; private set; }
        //public WeatherMakerFallingParticleScript SnowScript { get; private set; }
        //public WeatherMakerFallingParticleScript HailScript { get; private set; }
        //public WeatherMakerFallingParticleScript SleetScript { get; private set; }
        //public WeatherMakerCloudScript CloudScript { get; private set; }

        /// <summary>
        /// Gets the current time of day in hours, minutes and seconds (0-23:59:59). 0 is midnight. Sun object must be set.
        /// </summary>
        public System.TimeSpan TimeOfDay { get; private set; }
    }
}