using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAni : MonoBehaviour
{
    public const string IDLE = "idle";
    public const string WALK = "walk";
    public const string ATTACK = "attack1";
    public const string ANI_DIE = "death1";

    Animation anim;

    void Start()
    {
        anim = GetComponent<Animation>();
    }
    public void ChangeAni(string aniName)
    { anim.CrossFade(aniName); }

    // Update is called once per frame
    void Update()
    {
        
    }
}
