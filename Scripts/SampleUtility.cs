using System.Linq;
using UnityEngine;

public static class SampleUtility
{
    public static AnimationClip LoadAnimationClipFromFbx(string fbxName, string clipName)
    {
        var clips = Resources.LoadAll<AnimationClip>(fbxName); //fbxName의 경로에 AnimationClip를 다 Load함 
        return clips.FirstOrDefault(clip => clip.name == clipName); //시퀀스의 특정 조건에 맞는 첫 번째 요소를 반환하거나 기본값을 반환
    }

    public static GameObject CreateEffector(string name, Vector3 position, Quaternion rotation)
    {
        var effector = Resources.Load("Effector/Effector", typeof(GameObject)) as GameObject; //리소스 폴더의 경로에 저장된 에셋을 로드한다. 
        return CreateEffectorFromGO(name, effector, position, rotation);
    }

    public static GameObject CreateBodyEffector(string name, Vector3 position, Quaternion rotation)
    {
        var prefab = Resources.Load("Effector/BodyEffector", typeof(GameObject)) as GameObject;
        var effector = CreateEffectorFromGO(name, prefab, position, rotation);
        effector.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
        return effector;

   
    }

    public static GameObject CreateEffectorFromGO(string name, GameObject prefab, Vector3 position, Quaternion rotation)
    {
        
        var effector = Object.Instantiate(prefab);
        effector.name = name;
        effector.transform.position = position;
        effector.transform.rotation = rotation;
        effector.transform.localScale = Vector3.one * 0.15f;
        var meshRenderer = effector.GetComponent<MeshRenderer>();
        meshRenderer.material.color = Color.magenta;
        return effector;
    }

    static public Color FadeEffectorColorByWeight(Color original, float weight)
    {
        Color color = original * (0.2f + 0.8f * weight); 
        color.a = (0.2f + 0.5f * weight); //RGBA중 a는 투명도를 나타냄 
        return color;
    }
}
