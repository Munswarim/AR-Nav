using DirectionsNamespace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DirectionsAPI : MonoBehaviour
{
    public string apiKey;
    public string origin;
    public string destination;

    DirectionsNamespace.DirectionsResponse directionsResponse;
    MarkerSpawner mySpawner;

    void Start()
    {
        StartCoroutine(Generate());
    }

    IEnumerator Generate()
    {
        origin = PlayerPrefs.GetString("my origin");
        destination = PlayerPrefs.GetString("my destination");

        Debug.Log($"{origin} -> {destination}");
        // Construct the URL for the Directions API request.
        string baseUrl = "https://maps.googleapis.com/maps/api/directions/json?";
        string url = $"{baseUrl}origin={origin}&destination={destination}&key={apiKey}&language=en";

        // Send the request.
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            Debug.LogError("Error: " + request.error);
        }
        else
        {
            // Parse the JSON response.
            string jsonResponse = request.downloadHandler.text;
            directionsResponse = JsonUtility.FromJson<DirectionsNamespace.DirectionsResponse>(jsonResponse);


            if (directionsResponse != null && directionsResponse.routes.Length > 0)
            {
                // Loop through each step in the directions.
                yield return new WaitForSeconds(3.0f);

                mySpawner = this.GetComponent<MarkerSpawner>();

                double startLat, startLng, endLat, endLng;
                long angleY, anglePlus;

                startLat = startLng = endLat = endLng = angleY = anglePlus = 0;

                foreach (DirectionsNamespace.Step step in directionsResponse.routes[0].legs[0].steps)
                {
                    // Extract latitude and longitude for the start and end of each step.
                    startLat = step.start_location.lat;
                    startLng = step.start_location.lng;
                    endLat = step.end_location.lat;
                    endLng = step.end_location.lng;
                    // double distance = step.distance.value;

                    /*
                    string msg  = $"Step: {step.html_instructions}\n";
                    msg += $"Src : {startLat}, {startLng} \t Dst : {endLat}, {endLng}";
                    Debug.Log(msg);
                    */

                    (angleY, anglePlus) = DirectionAngle(angleY, step.html_instructions);

                    // mySpawner = this.GetComponent<MarkerSpawner>();
                    // mySpawner.Spawn(step.html_instructions, endLat, endLng);
                    StartCoroutine(mySpawner.Spawn(step.html_instructions, startLat, startLng, angleY, anglePlus));

                    /*double step_lat = (endLat - startLat)/(distance / 5);
                    double step_lng = (endLng - startLng)/(distance / 5);

                    for(double i = startLat + step_lat, j = startLng + step_lng; i < endLat || j < endLng; i+= step_lat, j+= step_lat)
                    {
                        this.GetComponent<MarkerSpawner>().Spawn2(i, j);
                    }*/
                }
                StartCoroutine(mySpawner.Spawn("Destination", endLat, endLng, angleY, anglePlus));
            }
        }
    }

    (long, long) DirectionAngle(long angle, string instruction)
    {
        long angleY = angle, anglePlus = 0;

        if (instruction.Contains("Head"))
        {
            if (instruction.Contains("north"))
            {
                angleY = 0;
            }
            else if (instruction.Contains("east"))
            {
                angleY = 90;
            }
            else if (instruction.Contains("south"))
            {
                angleY = 180;
            }
            else if (instruction.Contains("west"))
            {
                angleY = 270;
            }
        }
        else if (instruction.Contains("Turn"))
        {
            if (instruction.Contains("right")) 
            {
                angleY += 90;
            }
            else if (instruction.Contains("left"))
            {
                angleY -= 90;
            }
        }

        if (instruction.Contains("Keep"))
        {
            if (instruction.Contains("right"))
            {
                anglePlus += 45;
            }
            else if (instruction.Contains("left"))
            {
                anglePlus -= 45;
            }
        }

        return (angleY, anglePlus);
    }
}

namespace DirectionsNamespace
{
    [System.Serializable]
    public class DirectionsResponse
    {
        public Route[] routes;
    }

    [System.Serializable]
    public class Route
    {
        public Leg[] legs;
    }

    [System.Serializable]
    public class Leg
    {
        public Step[] steps;
    }

    [System.Serializable]
    public class Step
    {
        public string html_instructions;
        public Location start_location;
        public Location end_location;
        // public Distance distance;
    }

    [System.Serializable]
    public class Location
    {
        public double lat;
        public double lng;
    }
    public class Distance
    {
        public string text;
        public double value;

    }
}
