using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderTest : MonoBehaviour
{
    AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Smoke")
        {
            Debug.Log("충돌감지");
            StartCoroutine(delay());
            audioSource.Play();

        }
    }
    IEnumerator delay()
    {
        yield return new WaitForSeconds(3.0f);
        Destroy(this.gameObject);
        NumberChange.objectQueue.Dequeue();
        //ScorePlus.score += 5;
    }
   
}

//if (col.gameObject.name=="Smoke")
