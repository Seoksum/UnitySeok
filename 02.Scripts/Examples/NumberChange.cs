using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NumberChange : MonoBehaviour
{
    //public GameObject [] magicfire ;
    public int magicfirenumber;
    public Text numText; 
    public int changeNum;
    //
    public static Queue objectQueue;
    public GameObject fire;
    public GameObject temp;
    public GameObject alarm;

    public GameObject EndCanvas;

    float min = 1.0f;
    float time = 0f;
    float i = 0,j=0;
    
    public AudioClip soundExplosion;
    AudioSource myAudio;

    public GameObject FireTruck;

    private void Awake()
    {
        numText = GetComponentInChildren<Text>();
        myAudio = this.gameObject.GetComponent<AudioSource>();
        //temp = (GameObject)Instantiate(fire, new Vector3(-1.0f + min, 1.3f, -15f), Quaternion.identity);
        objectQueue = new Queue();
        objectQueue.Enqueue(temp);
        objectQueue.Enqueue(temp);
        objectQueue.Enqueue(temp);
        objectQueue.Enqueue(temp);
        objectQueue.Enqueue(temp);


    }

    private void Update()
    {
        this.time += Time.deltaTime;
        numText.text = changeNum.ToString() + "%";

        while (objectQueue.Count <= 5 && this.time > 6f)
        {
            i+=0.7f;
            if (fire == null)
                break;
            temp = (GameObject)Instantiate(fire, new Vector3(5.75f+i, 1.28f, 4.377f), Quaternion.identity);
            objectQueue.Enqueue(temp);
            
            this.time = 0;

   
        }
        
        while (6 <= objectQueue.Count && objectQueue.Count <= 20 && this.time > 7f)
        {
            j += 0.75f;
            if (fire == null)
                break;
            temp = (GameObject)Instantiate(fire, new Vector3(9.11f , 1.3f, 4.412f - j), Quaternion.identity);
            objectQueue.Enqueue(temp);
            this.time = 0;

        }



        if (objectQueue.Count == 0) { changeNum = 0; EndCanvas.SetActive(true); }
        else if (objectQueue.Count == 1) { changeNum = 5; }
        else if (objectQueue.Count == 2) { changeNum = 10; }
        else if (objectQueue.Count == 3) { changeNum = 15; }
        else if (objectQueue.Count == 4) { changeNum = 20; }
        else if (objectQueue.Count == 5) { changeNum = 25;  }
        else if (objectQueue.Count == 6) { changeNum = 30;  }
        else if (objectQueue.Count == 7) { changeNum = 35;   }
        else if (objectQueue.Count == 8) { changeNum = 40; }
        else if (objectQueue.Count == 9) { changeNum = 45;   }
        else if (objectQueue.Count == 10) { changeNum = 50;  }
        else if (objectQueue.Count == 11) { changeNum = 55;  }
        else if (objectQueue.Count == 12) { changeNum = 60;   }
        else if (objectQueue.Count == 13) { changeNum = 65;   }
        else if (objectQueue.Count == 14) { changeNum = 70; alarm.SetActive(true); }
        else if (objectQueue.Count == 15) { changeNum = 75;   }
        else if (objectQueue.Count == 16) { changeNum = 80;  }
        else if (objectQueue.Count == 17) { changeNum = 85;  }
        else if (objectQueue.Count == 18) { changeNum = 90;  }
        else if (objectQueue.Count == 19) { changeNum = 95;  }
        else if (objectQueue.Count == 20) { SceneManager.LoadScene("6.GameOver"); }
    }

    public void PlaySound()
    {
        myAudio.PlayOneShot(soundExplosion);
        myAudio.volume = 0.03f;
    }
    public void StopSound()
    {
        myAudio.Stop();
    }

}
