using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PresentData : MonoBehaviour
{
    float att;
    int hp;
    // Start is called before the first frame update
    void Start()
    {
        LoadScene(); 
    }

    void LoadScene()
    {
        att = PlayerPrefs.GetFloat("Attack");
        hp = PlayerPrefs.GetInt("HP");
        Debug.Log("공격 : " + att + ", Hp : " + hp);
    }
}
