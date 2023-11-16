using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using System.Collections;
using System.Collections.Generic;



[System.Serializable]
public class ApiResponse
{
    public WorkCategory[] data;
}

[System.Serializable]
public class WorkCategory
{
    public int id;
    public string work_category;
}

[System.Serializable]
public class ApiResponseContainer
{
    public ApiResponse data;
}

[System.Serializable]
public class UpdatedApiResponse
{
    public List<WorkCategory> data;
}

public class BookerInputField : MonoBehaviour
{
    public TMP_InputField inputField;
    public TMP_Text SuggestionText;

    string url = "http://127.0.0.1:8000/api/getallwork";
    string bearerToken;

    private void Start()
    {
        inputField.onValueChanged.AddListener(OnInputValueChanged);

        // Retrieve the bearer token on Start
        string savedToken = PlayerPrefs.GetString("AccessToken");
        if (!string.IsNullOrEmpty(savedToken))
        {
            bearerToken = savedToken;
        }
    }

    void OnInputValueChanged(string input)
    {
        StartCoroutine(GetWorkSuggestions(input));
    }

    IEnumerator GetWorkSuggestions(string input)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);

        // Use the retrieved bearer token in the request headers
        request.SetRequestHeader("Authorization", bearerToken);
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to fetch data: " + request.error);
        }
        else
        {
            string jsonResponse = request.downloadHandler.text;
            ProcessWorkSuggestions(jsonResponse, input);
        }
    }

    void ProcessWorkSuggestions(string jsonResponse, string input)
    {
        Debug.Log("Received JSON Response: " + jsonResponse);

        ApiResponseContainer responseContainer = JsonUtility.FromJson<ApiResponseContainer>(jsonResponse);

        if (responseContainer == null || responseContainer.data == null)
        {
            Debug.LogError("Response or Data object within response is null.");
            return;
        }

        List<string> suggestions = new List<string>();
        foreach (WorkCategory category in responseContainer.data.data)
        {
            if (category != null && category.work_category.ToLower().Contains(input.ToLower()))
            {
                suggestions.Add(category.work_category);
            }
        }

        if (suggestions.Count == 0)
        {
            Debug.Log("No suggestions found for the input: " + input);
            SuggestionText.text = "No suggestions found.";
        }
        else
        {
            string suggestionsText = string.Join("\n", suggestions);
            SuggestionText.text = suggestionsText;
        }
    }
}

