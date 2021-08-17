using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEditor;
using UnityEngine.Animations;

#if UNITY_2019_3_OR_NEWER

#else
using UnityEngine.Experimental.Animations;
#endif

public class FullBodyIKRotate : MonoBehaviour
{
    public bool syncGoal = true;
    float speed = 0.6f;
   

    //[Range(0.0f, 1.5f)]
    float stiffness = 0.0f;
    [Range(1, 50)]
    public int maxPullIteration = 5;

    //해당 변수들을 지정해준 범위 만큼 스크롤바로 조절할 수 있도록 Inspector에 나타남 
    public float defaultEffectorPositionWeight = 1.0f;
    public float defaultEffectorRotationWeight = 1.0f;
    public float defaultEffectorPullWeight = 1.0f;
    public float defaultHintWeight = 0.5f;
    //Weight값 초기화해주기 위한 변수

    private GameObject m_LeftFootEffector;
    private GameObject m_RightFootEffector;
    private GameObject m_LeftHandEffector;
    private GameObject m_RightHandEffector;
    //IK와 연결할 Effector오브젝트 

    private GameObject m_LeftKneeHintEffector;
    private GameObject m_RightKneeHintEffector;
    private GameObject m_LeftElbowHintEffector;
    private GameObject m_RightElbowHintEffector;

    //Hint IK와 연결할 Effector오브젝트 

    private GameObject m_LookAtEffector;
    //시선 처리를 해줄 오브젝트 

    private GameObject m_BodyRotationEffector;
    //bodyRotation BodyEffector오브젝트

    private GameObject totalEffector;


    private Animator m_Animator;
    private Animation m_Animation;
    private PlayableGraph m_Graph;
    private AnimationScriptPlayable m_IKPlayable; //AnimationScriptPlayable은 커스텀/멀티 스레드/animation job을 실행할 수 있는 Playable이다. job data를 가져올수도, 설정해줄수도 있음

    Vector3 leftHandpos;
    private static GameObject CreateEffector(string name)
    {
        var go = SampleUtility.CreateEffector(name, Vector3.zero, Quaternion.identity); //Quaternion.identity : 회전없음
        return go;
    }//Effector 만들어주는 함수 

    private static GameObject CreateBodyEffector(string name)
    {
        var go = SampleUtility.CreateBodyEffector(name, Vector3.zero, Quaternion.identity);
        return go;
    }//BodyEffector 만들어주는 함수 

    private GameObject SetupEffector(ref FullBodyIKJob.EffectorHandle handle, string name)
    {
        var go = CreateEffector(name); //Effector 오브젝트를 만들어 go에 넣어줌
        if (go != null)
        {
            go.AddComponent<Effector>();//go(Effector 오브젝트)에 Effector.cs 추가 
            handle.effector = m_Animator.BindSceneTransform(go.transform); //오브젝트의 transform과 m_Animator(아바타모델에 추가해준 animator)간의 새로운 바인딩을 해줘서 handle.effector에 넣음 
            handle.positionWeight = m_Animator.BindSceneProperty(go.transform, typeof(Effector), "positionWeight");
            handle.rotationWeight = m_Animator.BindSceneProperty(go.transform, typeof(Effector), "rotationWeight"); //PropertySceneHandle rotationWeight
            handle.pullWeight = m_Animator.BindSceneProperty(go.transform, typeof(Effector), "pullWeight"); //PropertySceneHandle pullWeight;
                                                                                                            //m_Animator와 바인딩되어있는 go.transform에서 타입이 Effector인 Component에 3번째 인자를 바인딩해줌
                                                                                                            //이때 Effector.cs에서 선언된 변수이름이랑 3번째 인자랑 같은 이름이어야함. 
        }
        return go; //effector 오브젝트 반환
    }

    //BindSceneTransform : Animator와 Scene의 Transform 간의 새로운 바인딩을 나타내는 TransformSceneHandle을 반환한다.
    //BindSceneProperty :씬에서 Transform의 Component 속성에 대한 새로운 바인딩을 나타내는 PropertySceneHandle을 반환.

    private GameObject SetupHintEffector(ref FullBodyIKJob.HintEffectorHandle handle, string name)
    {
        var go = CreateEffector(name);  //name을 가진 effector 오브젝트 만들어 go에 넣어줌 
        if (go != null)
        {
            go.AddComponent<HintEffector>(); //go에 HintEffector.cs를 추가해줌 
            handle.hint = m_Animator.BindSceneTransform(go.transform); //m_Animator와 go.transform을 바인딩해주어 TransformSceneHandle로 반환해준다. 
            //이때 TransformSceneHandle은 씬에서 개체의 Transform을 읽을 수 있는 핸들
            handle.weight = m_Animator.BindSceneProperty(go.transform, typeof(HintEffector), "weight");
            //m_Animator와 바인딩 되어있는 go를 타입이 HintEffector인 Componenet에 3번째 인자를 바인딩해서 PropertySceneHandle로 반환해줌 
            //PropertySceneHandle는 씬에서 개체의 Component속성을 읽는 핸들 

        }
        return go;
    }
    //씬에서 애니메이터와 오브젝트.transform과 연결을 원하면 -> BindSceneTransform
    //씬에서 애니메이터와 오브젝트의 컴포넌트(type)중 해당변수(?) 를 원하면 ->BindSceneProperty


    private GameObject SetupLookAtEffector(ref FullBodyIKJob.LookEffectorHandle handle, string name)
    {
        var go = CreateEffector(name); //effector 오브젝트 만들어 go에 넣음 

        if (go != null)
        {
            go.AddComponent<LookAtEffector>(); //go에 LookAtEffector.cs를 추가함
            handle.lookAt = m_Animator.BindSceneTransform(go.transform);
            handle.eyesWeight = m_Animator.BindSceneProperty(go.transform, typeof(LookAtEffector), "eyesWeight");
            handle.headWeight = m_Animator.BindSceneProperty(go.transform, typeof(LookAtEffector), "headWeight");
            handle.bodyWeight = m_Animator.BindSceneProperty(go.transform, typeof(LookAtEffector), "bodyWeight");
            handle.clampWeight = m_Animator.BindSceneProperty(go.transform, typeof(LookAtEffector), "clampWeight");
            //위와 동일 
        }
        return go;
    }

    private GameObject SetupBodyEffector(ref FullBodyIKJob.BodyEffectorHandle handle, string name)
    {
        var go = CreateBodyEffector(name); //BodyEffector 오브젝트 만들어 go에 넣음 
        if (go != null)
        {
            handle.body = m_Animator.BindSceneTransform(go.transform); //m_Animator와 go.transform을 바인딩해줘서 씬에서 개체의 Transform을 읽는 핸들로 반환해줌 
        }
        return go;
    }

    private void SetupIKLimbHandle(ref FullBodyIKJob.IKLimbHandle handle, HumanBodyBones top, HumanBodyBones middle, HumanBodyBones end) //HumanBodyBones는 유니티에서 인체 뼈들을 열거해둔 enum
    {
        handle.top = m_Animator.BindStreamTransform(m_Animator.GetBoneTransform(top)); //인체 뼈 top에 해당되는 Transform을 가져와 m_Animator와 바인딩해줘서 TransformStreamHandle로 반환
        handle.middle = m_Animator.BindStreamTransform(m_Animator.GetBoneTransform(middle));
        handle.end = m_Animator.BindStreamTransform(m_Animator.GetBoneTransform(end));
        //TransformStreamHandle은 AnimationStream에 있는 개체의 Transform(위치,회전,크기)을 읽는 핸들
    }//HumanBodyBones와 m_Animator를 스트림으로 연결해줘서 구조체 IKLimbHandle의 변수에게 값을 넣어준다.  
    //위의 Setup 함수들로 FullBodyIKJob.cs에서 선언한 구조체의 변수들에 값을 설정해줄 수 있음 

    private void SetupHeadHandle(ref FullBodyIKJob.HeadHandle handle, HumanBodyBones head, HumanBodyBones neck) //HumanBodyBones는 유니티에서 인체 뼈들을 열거해둔 enum
    {
        handle.head = m_Animator.BindStreamTransform(m_Animator.GetBoneTransform(head)); //인체 뼈 top에 해당되는 Transform을 가져와 m_Animator와 바인딩해줘서 TransformStreamHandle로 반환
        handle.neck = m_Animator.BindStreamTransform(m_Animator.GetBoneTransform(neck));
    }


    private void ResetIKWeight()
    {
        m_LeftFootEffector.GetComponent<Effector>().positionWeight = defaultEffectorPositionWeight; //0~1범위 , 현재 1f
        m_LeftFootEffector.GetComponent<Effector>().rotationWeight = defaultEffectorRotationWeight;
        m_LeftFootEffector.GetComponent<Effector>().pullWeight = defaultEffectorPullWeight;
        m_RightFootEffector.GetComponent<Effector>().positionWeight = defaultEffectorPositionWeight;
        m_RightFootEffector.GetComponent<Effector>().rotationWeight = defaultEffectorRotationWeight;
        m_RightFootEffector.GetComponent<Effector>().pullWeight = defaultEffectorPullWeight;
        m_LeftHandEffector.GetComponent<Effector>().positionWeight = defaultEffectorPositionWeight;
        m_LeftHandEffector.GetComponent<Effector>().rotationWeight = defaultEffectorRotationWeight;
        m_LeftHandEffector.GetComponent<Effector>().pullWeight = defaultEffectorPullWeight;
        m_RightHandEffector.GetComponent<Effector>().positionWeight = defaultEffectorPositionWeight;
        m_RightHandEffector.GetComponent<Effector>().rotationWeight = defaultEffectorRotationWeight;
        m_RightHandEffector.GetComponent<Effector>().pullWeight = defaultEffectorPullWeight;
        //위 Effector오브젝트들에 Effector.cs를 추가하고 각 오브젝트의 변수를 default값으로 초기화해줌 

        m_LeftKneeHintEffector.GetComponent<HintEffector>().weight = 0.5f;
        m_RightKneeHintEffector.GetComponent<HintEffector>().weight = 0.5f;
        m_LeftElbowHintEffector.GetComponent<HintEffector>().weight = 0.5f;
        m_RightElbowHintEffector.GetComponent<HintEffector>().weight = 0.5f;
        //위 (Hint)Effector 오브젝트들에 HintEffector.cs를 추가하고 변수를 0.5f로 초기화 해줌.
    }

    private void SyncIKFromPose() //Effector 오브젝트들을 알맞은 IK에 연결되게 해주는 함수 , 본체 위치랑 Effector 동기화 
    {
        var selectedTransform = Selection.transforms;
        var stream = new AnimationStream();

        //this.gameObject.transform.Rotate(new Vector3(-90f, 0, 0));
        //this.gameObject.transform.parent.rotation = Quaternion.Euler(-90f, 0, 0);
        
        this.gameObject.transform.Rotate(new Vector3(-45f, 0, 0));

        if (m_Animator.OpenAnimationStream(ref stream))
        {
            AnimationHumanStream humanStream = stream.AsHuman();

            m_LeftFootEffector.transform.SetParent(this.gameObject.transform);
            m_RightFootEffector.transform.SetParent(this.gameObject.transform);
            m_LeftHandEffector.transform.SetParent(this.gameObject.transform);
            m_RightHandEffector.transform.SetParent(this.gameObject.transform);

            m_LeftKneeHintEffector.transform.SetParent(this.gameObject.transform);
            m_RightKneeHintEffector.transform.SetParent(this.gameObject.transform);
            m_LeftElbowHintEffector.transform.SetParent(this.gameObject.transform);
            m_RightElbowHintEffector.transform.SetParent(this.gameObject.transform);
            m_LookAtEffector.transform.SetParent(this.gameObject.transform);
            m_BodyRotationEffector.transform.SetParent(this.gameObject.transform);

            if (!Array.Exists(selectedTransform, tr => tr == m_LeftFootEffector.transform)) //selectedtransform과  m_LeftFootEffector.transform이 같을때가 존재하지않는다면 
            // m_LeftFootEffector.transform이 현재 선택되어 있지 않다면 
            {
                m_LeftFootEffector.transform.position = humanStream.GetGoalPositionFromPose(AvatarIKGoal.LeftFoot);
                m_LeftFootEffector.transform.rotation = humanStream.GetGoalRotationFromPose(AvatarIKGoal.LeftFoot);
                //월드 공간에서 계산된 Stream(m_Animator와 연결된 humanstream)의 현재 pose로부터 IK goal의 위치값 회전값을 m_LeftFootEffector.transform에 반환해줌
                //Pose:3차원 공간에서의 위치 표현과 회전)             
            }

            if (!Array.Exists(selectedTransform, tr => tr == m_RightFootEffector.transform))
            {
                m_RightFootEffector.transform.position = humanStream.GetGoalPositionFromPose(AvatarIKGoal.RightFoot);
                m_RightFootEffector.transform.rotation = humanStream.GetGoalRotationFromPose(AvatarIKGoal.RightFoot);
            }

            if (!Array.Exists(selectedTransform, tr => tr == m_LeftHandEffector.transform))
            {
                m_LeftHandEffector.transform.position = humanStream.GetGoalPositionFromPose(AvatarIKGoal.LeftHand);
                m_LeftHandEffector.transform.rotation = humanStream.GetGoalRotationFromPose(AvatarIKGoal.LeftHand);

            }

            if (!Array.Exists(selectedTransform, tr => tr == m_RightHandEffector.transform))
            {
                m_RightHandEffector.transform.position = humanStream.GetGoalPositionFromPose(AvatarIKGoal.RightHand);
                m_RightHandEffector.transform.rotation = humanStream.GetGoalRotationFromPose(AvatarIKGoal.RightHand);
            }

            if (!Array.Exists(selectedTransform, tr => tr == m_LeftKneeHintEffector.transform))
            {
                m_LeftKneeHintEffector.transform.position = humanStream.GetHintPosition(AvatarIKHint.LeftKnee);
            }

            if (!Array.Exists(selectedTransform, tr => tr == m_RightKneeHintEffector.transform))
            {
                m_RightKneeHintEffector.transform.position = humanStream.GetHintPosition(AvatarIKHint.RightKnee);
            }

            if (!Array.Exists(selectedTransform, tr => tr == m_LeftElbowHintEffector.transform))
            {
                m_LeftElbowHintEffector.transform.position = humanStream.GetHintPosition(AvatarIKHint.LeftElbow);
            }

            if (!Array.Exists(selectedTransform, tr => tr == m_RightElbowHintEffector.transform))
            {
                m_RightElbowHintEffector.transform.position = humanStream.GetHintPosition(AvatarIKHint.RightElbow);
            }

            if (!Array.Exists(selectedTransform, tr => tr == m_BodyRotationEffector.transform))
            {
                m_BodyRotationEffector.transform.position = humanStream.bodyPosition;
                m_BodyRotationEffector.transform.rotation = humanStream.bodyRotation;
            }
            m_LookAtEffector.transform.position = new Vector3(humanStream.bodyPosition.x, humanStream.bodyPosition.y + 0.5f, humanStream.bodyPosition.z - 0.6f);
           

            //모두 해당 Effector가 선택되어있지 않을때, Effector오브젝트에 IK goal의 transform 정보들을 반환해주는데 
            //씬 시작할때부터 Effector오브젝트가 나와있지 않아서 굳이 조건문 안 줘도 될듯.?..
            m_Animator.CloseAnimationStream(ref stream);  //m_Animator에서 스트림 연결 닫아주기
        }
    }
    

    void OnEnable() //활성화 될 때마다 호출
    {
        m_Animator = GetComponent<Animator>(); //모델의 Animator 컴포넌트를 반환       
        m_Animation = GetComponent<Animation>();

        // Setting to Always animate because on the first frame the renderer can be not visible which break syncGoal on start up
        //첫 번째 프레임에서 시작 시 syncGoal을 깨는 렌더러를 볼 수 없기 때문에 항상 애니메이션으로 설정
        m_Animator.cullingMode = AnimatorCullingMode.AlwaysAnimate; // 캐릭터 전체를 항상 애니메이션시킨다

        if (!m_Animator.avatar.isHuman)
            throw new InvalidOperationException("Avatar must be a humanoid."); //메서드 호출이 개체의 현재 상태에 대해 유효하지 않을 때 InvalidOperationException 예외 발생
                                                                               //throw new Exception : 오류 발생 여부와 관계없이 강제로 임의의 오류를 발생시키는 구문 

        m_Graph = PlayableGraph.Create("FyllBodyIK"); //PlayabkeGraph를 만든다. 
        m_Graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime); //재생할 때 시간이 증가하는 방식을 변경한다.GameTime은 업데이트시 Time.time을 기반으로 합니다

        var output = AnimationPlayableOutput.Create(m_Graph, "output", m_Animator); //PlayableGraph에 AnimationPlayableOutput을 만든다. 
        var femaleClip = SampleUtility.LoadAnimationClipFromFbx("Character/Soldier Female/chr_f_soldier", "idle");
        var Maleclip = SampleUtility.LoadAnimationClipFromFbx("Character/Soldier Male", "idle");
        
        


        var femaleClipPlayable = AnimationClipPlayable.Create(m_Graph, femaleClip); //playable그래프에 idle clip으로 AnimationClipPlayable을 만들기 
        var maleClipPlayable = AnimationClipPlayable.Create(m_Graph, Maleclip); //playable그래프에 idle clip으로 AnimationClipPlayable을 만들기 


        femaleClipPlayable.SetApplyFootIK(false); //FootIK 플래그의 값을 설정, Foot IK : 체크하면 애니메이션이 발 미끄러짐이 감소하거나 제거 된다.
        femaleClipPlayable.SetApplyPlayableIK(false); // OnAnimatorIK 호출을 설정

        maleClipPlayable.SetApplyFootIK(false); //FootIK 플래그의 값을 설정, Foot IK : 체크하면 애니메이션이 발 미끄러짐이 감소하거나 제거 된다.
        maleClipPlayable.SetApplyPlayableIK(false);

        var job = new FullBodyIKJob();
        job.stiffness = stiffness; //빳빳함 정도 
        job.maxPullIteration = maxPullIteration; //IK를 움직일때 몸이 따라올 수 있는 최대 확장 정도 



        // SetupIKLimbHandle(ref FullBodyIKJob.IKLimbHandle handle, HumanBodyBones top, HumanBodyBones middle, HumanBodyBones end)
        SetupIKLimbHandle(ref job.leftArm, HumanBodyBones.LeftUpperArm, HumanBodyBones.LeftLowerArm, HumanBodyBones.LeftHand);
        SetupIKLimbHandle(ref job.rightArm, HumanBodyBones.RightUpperArm, HumanBodyBones.RightLowerArm, HumanBodyBones.RightHand);
        SetupIKLimbHandle(ref job.leftLeg, HumanBodyBones.LeftUpperLeg, HumanBodyBones.LeftLowerLeg, HumanBodyBones.LeftFoot);
        SetupIKLimbHandle(ref job.rightLeg, HumanBodyBones.RightUpperLeg, HumanBodyBones.RightLowerLeg, HumanBodyBones.RightFoot);
        //아바타의 해당 오브젝트(관절)들을 top,middle,end에 담아 job의 IKLimbHandle 구조체 멤버변수로 초기화해줌 

        SetupHeadHandle(ref job.realHead, HumanBodyBones.Head, HumanBodyBones.Neck);



        //GameObject SetupEffector(ref FullBodyIKJob.EffectorHandle handle, string name)
        m_LeftFootEffector = SetupEffector(ref job.leftFootEffector, "LeftFootEffector");
        m_RightFootEffector = SetupEffector(ref job.rightFootEffector, "RightFootEffector");
        m_LeftHandEffector = SetupEffector(ref job.leftHandEffector, "LeftHandEffector");
        m_RightHandEffector = SetupEffector(ref job.rightHandEffector, "RightHandEffector");
        //job에서 EffectorHandle 구조체 멤버변수들에 대해 setup해줌. 그리고 name을 가진 effector오브젝트를 만들어서 반환

        m_LeftKneeHintEffector = SetupHintEffector(ref job.leftKneeHintEffector, "LeftKneeHintEffector");
        m_RightKneeHintEffector = SetupHintEffector(ref job.rightKneeHintEffector, "RightKneeHintEffector");
        m_LeftElbowHintEffector = SetupHintEffector(ref job.leftElbowHintEffector, "LeftElbowHintEffector");
        m_RightElbowHintEffector = SetupHintEffector(ref job.rightElbowHintEffector, "RightElbowHintEffector");
        //job에서 HintEffectorHandle 구조체 멤버변수들에 대해 setup해줌. 그리고 name을 가진 effector오브젝트를 만들어서 반환

        m_LookAtEffector = SetupLookAtEffector(ref job.lookAtEffector, "LookAtEffector");
        //job에서 LookEffectorHandle 구조체 멤버변수들에 대해 setup해줌. 그리고 name을 가진 effector오브젝트를 만들어서 반환

        m_BodyRotationEffector = SetupBodyEffector(ref job.bodyEffector, "BodyEffector");
        //job에서 BodyEffectorHandle 구조체 멤버변수들에 대해 setup해줌. 그리고 name을 가진 BodyEffector오브젝트를 만들어서 반환


        m_IKPlayable = AnimationScriptPlayable.Create(m_Graph, job, 1); //m_Graph에서 animation job을 실행할 수 있는 Playable을 만들어준다.
        m_IKPlayable.ConnectInput(0, femaleClipPlayable, 0, 1.0f);
        //m_IKPlayable.ConnectInput(0, maleClipPlayable, 0, 1.0f);
        //ConnectInput(입력포트, 연결할 Playable, 출력포트 , 입력포트의 가중치)

        output.SetSourcePlayable(m_IKPlayable);
        //이걸 해야 FullBodyIKJob.cs랑 연결할 수 있다..
        //AnimationPlayableOutput와 AnimationScript를 연결시켜줌

        m_Graph.Play(); //PlayableGraph 시작
        m_Graph.Evaluate(0); //그래프의 모든 PlayableOutputs를 평가하고 그래프에서 연결된 모든 Playables를 업데이트한다. 인자는 Playable 각 항목을 걸리는 시간 

        
        SyncIKFromPose();
        //Effector 오브젝트가 아바타와 동기화됨 
        ResetIKWeight();//IK goal과 IK Hint Effector들의 구성요소들을 기본값으로 초기화해줌

    }

    void OnDisable()
    {
        GameObject.DestroyImmediate(m_LeftFootEffector); //객체를 즉시 파괴함
        GameObject.DestroyImmediate(m_RightFootEffector);
        GameObject.DestroyImmediate(m_LeftHandEffector);
        GameObject.DestroyImmediate(m_RightHandEffector);
        GameObject.DestroyImmediate(m_LeftKneeHintEffector);
        GameObject.DestroyImmediate(m_RightKneeHintEffector);
        GameObject.DestroyImmediate(m_LeftElbowHintEffector);
        GameObject.DestroyImmediate(m_RightElbowHintEffector);
        GameObject.DestroyImmediate(m_LookAtEffector);
        GameObject.DestroyImmediate(m_BodyRotationEffector);

        if (m_Graph.IsValid())
            m_Graph.Destroy();

        //FullBodyIK.cs 비활성화하면 Effector 오브젝트들이랑 PlayableGraph 사라짐 
    }
   void Update()
    {

        var job = m_IKPlayable.GetJobData<FullBodyIKJob>(); //Playable에 포함된 <FullBodyIKJob>구조체 데이터를 가져온다 
        job.stiffness = stiffness;
        job.maxPullIteration = maxPullIteration;
        m_IKPlayable.SetJobData(job);
        //stiffness랑 maxPullIteration값이 바뀔때마다 적용돼야하므로 update문에 써주고 
        //기존 job data에서 stiffness랑 maxPullIteration를 재설정해줘서 다시 새로운 JobData를 Playable에 설정해준다.

        PressKey();

    }
    void PressKey()
    {
        if (Input.GetKey(KeyCode.Alpha1))
            MoveObject(m_RightHandEffector);        
        if (Input.GetKey(KeyCode.Alpha2))
            MoveObject(m_LeftHandEffector);
        if (Input.GetKey(KeyCode.Alpha3))
            MoveObject(m_RightFootEffector);
        if (Input.GetKey(KeyCode.Alpha4))
            MoveObject(m_LeftFootEffector); //IK goal Effector 움직이기

        if (Input.GetKey(KeyCode.Alpha5))
            MoveObject(m_LookAtEffector); //시선 Effector 움직이기

        if (Input.GetKey(KeyCode.Q))
            MoveObject(m_RightElbowHintEffector); 
        if (Input.GetKey(KeyCode.W))
            MoveObject(m_LeftElbowHintEffector);
        if (Input.GetKey(KeyCode.E))
            MoveObject(m_RightKneeHintEffector);
        if (Input.GetKey(KeyCode.R))
            MoveObject(m_LeftKneeHintEffector); //IK Hint Effector 움직이기
    }
    void MoveObject(GameObject obj)
    {
       // var stream = new AnimationStream();
        //AnimationHumanStream humanStream = stream.AsHuman();
            
            if (Input.GetKey(KeyCode.LeftArrow))
        {
            obj.transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.World);
            
            //Debug.Log("LeftHand : " + humanStream.GetGoalPositionFromPose(AvatarIKGoal.LeftHand));
        }
        if (Input.GetKey(KeyCode.RightArrow))
            obj.transform.Translate(Vector3.back * speed * Time.deltaTime, Space.World);
        if (Input.GetKey(KeyCode.UpArrow))
            obj.transform.Translate(Vector3.up * speed * Time.deltaTime, Space.World);
        if (Input.GetKey(KeyCode.DownArrow))
            obj.transform.Translate(Vector3.down * speed * Time.deltaTime, Space.World);

        if (Input.GetKey(KeyCode.O))
            obj.transform.Translate(Vector3.right * speed * Time.deltaTime, Space.World);
        if (Input.GetKey(KeyCode.P))
            obj.transform.Translate(Vector3.left * speed * Time.deltaTime, Space.World);

    }
    void LateUpdate() //모든 Update 함수가 호출된 후, 마지막으로 호출된다. 
    {
        // Synchronize on LateUpdate to sync goal on current frame
        if (syncGoal)
        {
            SyncIKFromPose();
            syncGoal = false;
        }
        //처음 한 번 SyncIKFromPose() 해준다. 
               
        m_BodyRotationEffector.transform.position = m_IKPlayable.GetJobData<FullBodyIKJob>().bodyPosition; //m_BodyRotationEffector의 position은 계속 FullBodyIKJob 구조체에서
                                                                                                           //정의한 bodyPosition을 따라가도록                                                                                               
    }
    
}