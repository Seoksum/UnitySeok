using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Button btn;
    public GameObject obj;
    public int sceneNum = 1;
    // Start is called before the first frame update
    void Start()
    {
        btn.onClick.AddListener(btnListener);
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            SceneManager.LoadScene("Scene03");
        }
    }
    // Update is called once per frame
    public void btnListener()
    {
        SceneManager.LoadScene("Scene03", LoadSceneMode.Additive);
        GameManager.instance.MoveToOtherScene(obj, sceneNum);
    }
}
