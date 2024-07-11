using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public class PlaceFinderAPI : MonoBehaviour
{
    public string apiKey;
    public string placeName;

    private string formatted_address;
    private double lat;
    private double lng;
    private PlaceCoordinatesNamespace.PlaceResponse placeResponse;

    public void Fetch(string keyword, Action<string, double, double> callback)
    {
        placeName = keyword;
        StartCoroutine(GetPlaceCoordinates(callback));
    }

    IEnumerator GetPlaceCoordinates(Action<string, double, double> callback)
    {
        // Construct the URL for the Places API request.
        string baseUrl = "https://maps.googleapis.com/maps/api/place/findplacefromtext/json?";
        string inputType = "textquery";  // Indicates that we are using a text query.
        string url = $"{baseUrl}input={placeName}&inputtype={inputType}&fields=geometry,formatted_address&key={apiKey}&language=en";

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
            placeResponse = JsonUtility.FromJson<PlaceCoordinatesNamespace.PlaceResponse>(jsonResponse);

            if (placeResponse != null && placeResponse.candidates.Length > 0)
            {
                // Extract latitude and longitude.
                formatted_address = placeResponse.candidates[0].formatted_address;
                lat = placeResponse.candidates[0].geometry.location.lat;
                lng = placeResponse.candidates[0].geometry.location.lng;

                Debug.Log($"{formatted_address} \nCoordinates: {lat}, {lng}");
            }
            else
            {
                formatted_address = "Place not found.";
                Debug.LogError(formatted_address);
            }

            callback?.Invoke(formatted_address, lat, lng);
        }
    }
}

namespace PlaceCoordinatesNamespace
{
    [System.Serializable]
    public class PlaceResponse
    {
        public PlaceCandidate[] candidates;
    }

    [System.Serializable]
    public class PlaceCandidate
    {
        public string formatted_address;
        public PlaceGeometry geometry;
    }

    [System.Serializable]
    public class PlaceGeometry
    {
        public Location location;
    }

    [System.Serializable]
    public class Location
    {
        public double lat;
        public double lng;
    }
}