using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;


[RequireComponent(typeof(Animator))]
public class MixAnimationSample : MonoBehaviour
{
    public AnimationClip clip0;
    public AnimationClip clip1;
    public float weight;
    PlayableGraph pg;
    AnimationMixerPlayable mixerPlayable;

    // Start is called before the first frame update
    void Start()
    {
        pg = PlayableGraph.Create();
        var playalbeOutput = AnimationPlayableOutput.Create(pg, "Animation", GetComponent<Animator>());
        mixerPlayable = AnimationMixerPlayable.Create(pg, 2);
        playalbeOutput.SetSourcePlayable(mixerPlayable);

        var cp0 = AnimationClipPlayable.Create(pg, clip0);
        var cp1 = AnimationClipPlayable.Create(pg, clip1);

        pg.Connect(cp0, 0, mixerPlayable, 0);
        pg.Connect(cp1, 0, mixerPlayable, 1);
        pg.Play();
        
    }

    // Update is called once per frame
    void Update()
    {
        weight = Mathf.Clamp01(weight);
        mixerPlayable.SetInputWeight(0, 1.0f - weight);
        mixerPlayable.SetInputWeight(1, weight);
    }
}
