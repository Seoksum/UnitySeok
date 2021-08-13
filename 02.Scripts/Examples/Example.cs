using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Example : MonoBehaviour
{
    float attVal = 100.34f;
    int hpVal = 500;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            LoadNextScene();
    }
    public void LoadNextScene()
    {
        saveData();
        StartCoroutine(LoadScene());
    }
    IEnumerator LoadScene()
    {
        AsyncOperation aLoad = SceneManager.LoadSceneAsync("Scene02");
        while (!aLoad.isDone)
            yield return null;
    }

    void saveData()
    {
        PlayerPrefs.SetFloat("Attack", attVal);
        PlayerPrefs.SetInt("HP", hpVal);
    }
}
