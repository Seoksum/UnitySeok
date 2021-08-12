using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }
    public void MoveToOtherScene(GameObject obj , int sceneNum)
    {
        Scene scene = SceneManager.GetSceneByBuildIndex(sceneNum);
        SceneManager.MoveGameObjectToScene(obj, scene);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
