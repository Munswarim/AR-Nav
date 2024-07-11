using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;

public class DynamicMapAPI : MonoBehaviour
{
    public string apiKey;
    public RawImage mapImage;
    public TMP_Text originLatLng;
    public TMP_Text destinationLatLng;

    private string origin;
    private string destination;
    private DynamicMapNamespace.DirectionsResponse directionsResponse;


    public void Show()
    {
        origin = originLatLng.text;
        destination = destinationLatLng.text;
        StartCoroutine(GetDirections());
    }

    IEnumerator GetDirections()
    {
        // Construct the URL for the Directions API request.
        string baseUrl = "https://maps.googleapis.com/maps/api/directions/json?";
        string url = $"{baseUrl}origin={origin}&destination={destination}&key={apiKey}";

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
            directionsResponse = JsonUtility.FromJson<DynamicMapNamespace.DirectionsResponse>(jsonResponse);

            if (directionsResponse != null && directionsResponse.routes.Length > 0)
            {
                // Extract the polyline points from the response.
                string polyline = directionsResponse.routes[0].overview_polyline.points;

                // Request a static map image for the route.
                string staticMapUrl = $"https://maps.googleapis.com/maps/api/staticmap?" +
                                      $"size=800x600" +
                                      $"&path=enc:{polyline}" +
                                      $"&key={apiKey}";

                // Load and display the static map image.
                UnityWebRequest mapRequest = UnityWebRequestTexture.GetTexture(staticMapUrl);
                yield return mapRequest.SendWebRequest();

                if (!mapRequest.isNetworkError && !mapRequest.isHttpError)
                {
                    Texture2D mapTexture = ((DownloadHandlerTexture)mapRequest.downloadHandler).texture;
                    mapImage.texture = mapTexture;
                }
                else
                {
                    Debug.LogError("Error loading map image: " + mapRequest.error);
                }
            }
            else
            {
                Debug.LogError("No route found.");
            }
        }
    }
}

namespace DynamicMapNamespace
{
    [System.Serializable]
    public class DirectionsResponse
    {
        public Route[] routes;
    }

    [System.Serializable]
    public class Route
    {
        public OverviewPolyline overview_polyline;
    }

    [System.Serializable]
    public class OverviewPolyline
    {
        public string points;
    }
}