using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentLatLngFinder : MonoBehaviour
{
    void Start()
    {
        // Check if location services are enabled
        if (Input.location.isEnabledByUser)
        {
            // Start location service with desired accuracy and update distance
            Input.location.Start(1f, 1f);
        }
        else
        {
            Debug.LogError("Location services are not enabled by the user.");
        }
    }

    public (double,double) Track()
    {
        // Check if location data is available
        if (Input.location.status == LocationServiceStatus.Running)
        {
            // Access latitude and longitude
            float lat = Input.location.lastData.latitude;
            float lng = Input.location.lastData.longitude;

            // Do something with latitude and longitude
            Debug.Log($"Current Latitude: {lat}, Longitude: {lng}");
            return (lat, lng);
        }
        else
        {
            return Track();
        }
    }

    void OnDestroy()
    {
        // Stop location service when the script is destroyed
        Input.location.Stop();
    }
}