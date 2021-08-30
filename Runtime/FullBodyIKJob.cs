using UnityEngine;
#if UNITY_2019_3_OR_NEWER
using UnityEngine.Animations;
#else
using UnityEngine.Experimental.Animations;
#endif
using Unity.Collections;

public struct FullBodyIKJob : IAnimationJob
{
    public struct EffectorHandle
    {
        public TransformSceneHandle effector; //Scene의 Effector오브젝트의 Transform을 읽음 
        public PropertySceneHandle positionWeight;//Effector오브젝트 구성요소중 (발의)위치에 대한 가중치값
        public PropertySceneHandle rotationWeight;//Effector오브젝트 구성요소중 (발의)회전에 대한 가중치값
        public PropertySceneHandle pullWeight; //공(Effector)을 움직임으로써 모델 전체 몸이 움직이는 정도(ex 0이면 IK만 움직이고 1이면 몸 전체가 함께 움직임) 
    }
    //TransformStreamHandle : 씬에서 Transform(위치,회전,크기)를 읽는 핸들
    //PropertyStreamHandle : 씬에서 개체의 Component 속성을 읽는 핸들 
    public struct HintEffectorHandle
    {
        public TransformSceneHandle hint; //Scene의 HintEffector오브젝트의 Transform을 읽음
        public PropertySceneHandle weight; //HintEffector오브젝트 구성요소중 위치에 대한 가중치값
    }

    public struct LookEffectorHandle
    {
        public TransformSceneHandle lookAt;//Scene의 LookAtEffector 오브젝트의 Transform을 읽음
        public PropertySceneHandle eyesWeight; //LookAtEffector 오브젝트의 구성요소중 눈의 가중치값
        public PropertySceneHandle headWeight; //LookAtEffector 오브젝트의 구성요소중 머리의 가중치값
        public PropertySceneHandle bodyWeight; //LookAtEffector 오브젝트의 구성요소중 몸의 가중치값
        public PropertySceneHandle clampWeight; //LookAtEffector 오브젝트의 구성요소중 클램프 가중치값
        //가중치 범위 0.0f ~ 1.0f 
    }

    public struct BodyEffectorHandle
    {
        public TransformSceneHandle body; //Scene의 BodyEffector 오브젝트의 Transform을 읽음 
    }

    //4개의 구조체 : EffectorHandle, HintEffectorHandle, LookEffectorHandle, BodyEffectorHandle
    public EffectorHandle leftFootEffector;
    public EffectorHandle rightFootEffector;
    public EffectorHandle leftHandEffector;
    public EffectorHandle rightHandEffector;
    //반환형태가 EffectorHandle인 왼발,오른발,왼손,오른손 Effector

    public HintEffectorHandle leftKneeHintEffector;
    public HintEffectorHandle rightKneeHintEffector;
    public HintEffectorHandle leftElbowHintEffector;
    public HintEffectorHandle rightElbowHintEffector;
    //반환형태가 HintEffectorHandle인 왼쪽무릎,오른쪽무릎,왼쪽팔꿈치,오른쪽팔꿈치 Effector

    public LookEffectorHandle lookAtEffector;
    //반환형태가 LookEffectorHandle인 시선 Effector

    public BodyEffectorHandle bodyEffector;
    //반환형태가 BodyEffectorHandle인 body중심 Effector

    public Vector3 bodyPosition; 
    //몸의 중심
    
    public struct IKLimbHandle
    {
        public TransformStreamHandle top;
        public TransformStreamHandle middle;
        public TransformStreamHandle end;
        public float maximumExtension;
    } //TransformStreamHandle은 AnimationStream에 있는 개체의 Transform(위치,회전,크기)를 읽는 핸들
    //top,middle,end는 stream을 통해 Transform을 받아올 변수

    public struct HeadHandle
    {
        public TransformStreamHandle head;
        public TransformStreamHandle neck;
    }
    public HeadHandle realHead;

    public IKLimbHandle leftArm;
    public IKLimbHandle rightArm;
    public IKLimbHandle leftLeg;
    public IKLimbHandle rightLeg;
    //반환형태가 IKLimbHandle인 왼팔,오른팔,왼쪽다리,오른쪽다리

    public float stiffness; //아바타 모델 몸 전체가 Effector 오브젝트에 얼만큼 붙어있으려는지 정도
    public int maxPullIteration; //팔이나 다리를 뻗을 수 있는 최대값
                                 //ex) 왼손Effector를 움직일때 해당 값이 작으면 팔을 구부린채로 몸이 따라가고, 값이 크면 팔만 쭉 뻗음 

    private EffectorHandle GetEffectorHandle(AvatarIKGoal goal) //AvatarIKGoal로 받아와 EffectorHandle 구조체로 반환
    {
        switch (goal)
        {
            default:
            case AvatarIKGoal.LeftFoot: return leftFootEffector;//아바타의 왼쪽 발일 경우 leftFootEffector(구조체)를 반환 
            case AvatarIKGoal.RightFoot: return rightFootEffector;
            case AvatarIKGoal.LeftHand: return leftHandEffector;
            case AvatarIKGoal.RightHand: return rightHandEffector;
        }
    }

    private IKLimbHandle GetIKLimbHandle(AvatarIKGoal goal) //AvatarIKGoal을 받아와 IKLimbHandle 구조체로 반환
    {
        switch (goal)
        {
            default:
            case AvatarIKGoal.LeftFoot: return leftLeg; //아바타의 왼쪽 발일 경우 leftLeg(구조체)를 반환 
            case AvatarIKGoal.RightFoot: return rightLeg;
            case AvatarIKGoal.LeftHand: return leftArm;
            case AvatarIKGoal.RightHand: return rightArm;
        }
    }
    //Arm > Hand , Leg > Foot


    //AnimationStream -> 한 Playable에서 다른 Playable으로 전달된 애니메이션 데이터 스트림
    //AnimationHumanStream -> 한 Playable에서 다른 Playable로 전달된 애니메이션 데이터의 휴머노이드 스트림
    //Playables API는 PlayableGraph에서 데이터 소스를 구성하고 평가하여 도구, 효과 또는 기타 게임 플레이 메커니즘을 만드는 방법을 제공한다.

   
    private void SetEffector(AnimationStream stream, AvatarIKGoal goal, ref EffectorHandle handle)  //FullBodyIK.SetupEffector() 참고
    {//ref -> 힙 메모리 영역에 주소가 들어있어서 그 주소가 복사되면서 같은 객체를 가리킴

        if (handle.effector.IsValid(stream) && handle.positionWeight.IsValid(stream) && handle.rotationWeight.IsValid(stream)) //handle(구조체)의 멤버변수들이 stream으로 연결되어있는가?
        {
            AnimationHumanStream humanStream = stream.AsHuman();                        //아바타 IK설정을 위해 AnimationStream을 AnimationHumanStream으로 바꿔줌  
            humanStream.SetGoalPosition(goal, handle.effector.GetPosition(stream)); //stream을 통해 world space에서 받아온 handle.effector의 위치값을 IK goal의 위치로 설정한다
            humanStream.SetGoalRotation(goal, handle.effector.GetRotation(stream)); //stream을 통해 world space에서 받아온 handle.effector의 회전값을 IK goal의 회전을 설정한다
            //GetPosition과 GetRotation은 TransformSceneHandle에서 제공하는 함수로 Vector3로 반환함
            humanStream.SetGoalWeightPosition(goal, handle.positionWeight.GetFloat(stream)); //stream을 통해 world space에서 받아온 handle.positionWeight값을 goal의 위치 가중치값으로 설정
            humanStream.SetGoalWeightRotation(goal, handle.rotationWeight.GetFloat(stream)); //stream을 통해 world space에서 받아온 handle.rotationWeight값을 goal의 회전 가중치값으로 설정 
            //가중치는 0.0f ~ 1.0f 로 설정하므로 GetFloat로 받아옴 
        }
    }//stream을 이용해서 handle의 Transform 정보와 컴포넌트 값을 얻어와 IK goal의 위치,회전,가중치값로 설정해주는 함수 

    private void SetHintEffector(AnimationStream stream, AvatarIKHint goal, ref HintEffectorHandle handle) //FullBodyIK.SetupHintEffector() 참고
    {
        if (handle.hint.IsValid(stream) && handle.weight.IsValid(stream))
        {
            AnimationHumanStream humanStream = stream.AsHuman();
            humanStream.SetHintPosition(goal, handle.hint.GetPosition(stream)); //world space에서 IK goal 힌트의 위치를 설정한다.
            humanStream.SetHintWeightPosition(goal, handle.weight.GetFloat(stream)); 
        }
    }//stream을 이용해서 handle의 Transform 정보와 컴포넌트 값을 얻어와 IK Hint의 위치,가중치값로 설정해주는 함수 

    private void SetLookAtEffector(AnimationStream stream, ref LookEffectorHandle handle) //FullBodyIK.SetupLookAtEffector() 참고
    {       
        if (handle.lookAt.IsValid(stream)) //시선처리에 대한 IK는 없고 그냥 시선이 오브젝트를 따라가게하는 정도
        {
            AnimationHumanStream humanStream = stream.AsHuman();
            humanStream.SetLookAtPosition(handle.lookAt.GetPosition(stream)); //stream을 통해 handle.lookAt의 위치를 받아와 humanStream의 바라보는 위치로 설정 
            humanStream.SetLookAtEyesWeight(handle.eyesWeight.GetFloat(stream));// 눈이 LookAt에 얼마나 관련되어 있는지 결정. 전방 벡터가 LookAt 위치를 가리키도록 눈을 회전함.
            humanStream.SetLookAtHeadWeight(handle.headWeight.GetFloat(stream));// 머리가 LookAt에 얼마나 관련되어 있는지 결정. 전방 벡터가 LookAt 위치를 가리키도록 머리를 회전함.
            humanStream.SetLookAtBodyWeight(handle.bodyWeight.GetFloat(stream)); // 몸이 LookAt에 얼마나 관련되어 있는지 결정. 전방 벡터가 LookAt 위치를 가리키도록 몸을 회전함.
            humanStream.SetLookAtClampWeight(handle.clampWeight.GetFloat(stream)); //LookAt의 클램프 무게를 설정 
            // 0.0은 캐릭터의 움직임이 완전히 제한되지 않았음을 의미하고, 1.0은 캐릭터가 완전히 고정되었음을 의미,
            // 0.5는 캐릭터가 가능한 범위의 절반(180도)으로 이동할 수 있음을 의미한다.
        }
    }

    private void SetBodyEffector(AnimationStream stream, ref BodyEffectorHandle handle) //FullBodyIK.SetupBodyEffector() 참고
    {
        if (handle.body.IsValid(stream))
        {
            AnimationHumanStream humanStream = stream.AsHuman();
            humanStream.bodyRotation = handle.body.GetRotation(stream);
        }
    }  //humanStream.bodyPosition(몸의 중심)에 stream으로 부터 받은 handle.body값만 할당해줌

    
    private void SetMaximumExtension(AnimationStream stream, ref IKLimbHandle handle) // IKLimbHandle -> top,middle,end(TransformStreamHandle) & maximumExtension(float)
                                                                                      // FullBodyIK.SetupIKLimbHandle() 참고 
    {
        if (handle.maximumExtension == 0)
        {
            Vector3 top = handle.top.GetPosition(stream); //stream을 통해 FullBodyIK.cs에서 구한handle.top 위치값 가져와서 FullBodyIKJob.cs의 top에 넣어줌 (어깨,골반) 
            Vector3 middle = handle.middle.GetPosition(stream);//stream을 통해 handle.middle 위치값을 얻어와 middle에 넣어줌 (팔꿈치,무릎) 
            Vector3 end = handle.end.GetPosition(stream);//stream을 통해 handle.end 위치값을 얻어와 end에 넣어줌 (손,발) 
            //Debug.Log("top : " + top + ", middle : " + middle + ", end : " + end);

            Vector3 localMiddle = middle - top; 
            Vector3 localEnd = end - middle;

            handle.maximumExtension = localMiddle.magnitude + localEnd.magnitude; //magnitude : 벡터의 크기를 알려줌 , 즉 팔,다리 전체 길이 
            
        }
    }//top,middle,end TransformStreamHandle(Animation Stream의 Transform 관리하는 핸들)이니깐 AnimationStream를 통해 FullBodyIK에서 전달해준 위치를 받아와서
     //벡터3 top,middle,end에 넣어줌. 다리로 친다면 localMiddle은 골반~무릎, localEnd는 무릎~발
    private void SetHead(AnimationStream stream, ref HeadHandle handle) 
    {
        Vector3 head = handle.head.GetPosition(stream);
        Vector3 neck = handle.neck.GetPosition(stream);
                 
    }
    struct LimbPart
    {
        public Vector3 localPosition;    // 신체와 관련된 팔,다리의 local 위치 
        public Vector3 goalPosition;    // IK goal 위치 벡터
        public float   goalWeight;      // IK goal 가중치값 ? 
        public float   goalPullWeight;  // IK goal 당기는 가중치값 ?
        public float   maximumExtension; // 당기는 사람이 몸체를 잡아당기기 시작할 때를 정의하는 팔다리의 최대 확장가능한 실수값 (spring rest lenght)
        public float   stiffness;        // Effector와 IK가 붙어있으려는 정도
    }
    
    private void PrepareSolvePull(AnimationStream stream, NativeArray<LimbPart> limbParts) //IK 당겼을 때 아바타 모델의 몸 전체에 대한 가중치를 설정하는?.. 
    {// Unity C# job 시스템과 함께 NativeContainer사용하면 job이 복사본으로 작업하는 대신 기본 스레드와 공유되는 데이터에 액세스할 수 있다.
        AnimationHumanStream humanStream = stream.AsHuman();

        Vector3 bodyPosition = humanStream.bodyPosition; //몸의 중심값 

        for (int goalIter = 0; goalIter < 4; goalIter++) //leftFoot=0,rightFoot=1,leftHand=2,rightHand=3
        {
            var effector = GetEffectorHandle((AvatarIKGoal)goalIter); //EffectorHandle 구조체 반환 
            var limbHandle = GetIKLimbHandle((AvatarIKGoal)goalIter);//IKLimbHandle 구조체 반환 , LeftFoot이면 leftLeg반환 
            Vector3 top = limbHandle.top.GetPosition(stream); //그때 leftLeg의 top의 위치 반환(왼쪽 골반)
                                                              //Debug.Log("top : " + top); //=> (-2.2 ~ -1.8 ,0.9~1.4, 0)
                                                              //Debug.Log("bodyPosition : " + bodyPosition); => bodyposition(-2,1,0)

            limbParts[goalIter] = new LimbPart {
                localPosition = top - bodyPosition, //leftLeg의 top과 몸의 중심 위치 사이 벡터.
                goalPosition = humanStream.GetGoalPosition((AvatarIKGoal)goalIter), //LeftFoot의 IK위치 반환 
                goalWeight = humanStream.GetGoalWeightPosition((AvatarIKGoal)goalIter), //LeftFoot의 IK 가중치값 반환
                goalPullWeight = effector.pullWeight.GetFloat(stream), //Effector가 몸 전체 움직임에 미치는 정도, stream을 통해 FullBodyIK.cs에서 받아온 pullWeight값 
                maximumExtension = limbHandle.maximumExtension, //당기는 사람이 몸체를 잡아당기기 시작할 때를 정의하는 팔다리의 최대 확장가능한 실수값
                stiffness = stiffness
            };
        }
    } // LimbPart 구조체의 멤버 변수들을 초기화해주는 함수

    private Vector3 SolvePull(AnimationStream stream)
    {
        AnimationHumanStream humanStream = stream.AsHuman();

        Vector3 originalBodyPosition = humanStream.bodyPosition; //처음 몸의 중심위치
        Vector3 bodyPosition = originalBodyPosition;

        NativeArray<LimbPart> limbParts = new NativeArray<LimbPart>(4, Allocator.Temp); //수명이 짧지만 메모리에 빠르게 할당해주는 .Temp 
        PrepareSolvePull(stream, limbParts); //LimpPart 구조체 4개의 멤버변수들 값 초기화
        
        for (int iter = 0; iter < maxPullIteration; iter++) 
        {
            Vector3 deltaPosition = Vector3.zero;
            for (int goalIter = 0; goalIter < 4; goalIter++)
            {
                Vector3 top = bodyPosition + limbParts[goalIter].localPosition; //limbParts[goalIter].localPosition가 top - bodyPosition이니깐 top = top ?
                //Debug.Log("top : " + top);
                Vector3 localForce = limbParts[goalIter].goalPosition - top; //IK goal(왼손,오른손,왼팔,오른팔)와 top사이의 벡터 , 팔/다리 길이
                float restLenght = limbParts[goalIter].maximumExtension; // 당기는 사람이 몸체를 잡아당기기 시작할 때를 정의하는 팔다리의 최대 확장가능한 실수값
                float currentLenght = localForce.magnitude; //팔,다리 길이 

                localForce.Normalize(); //localForce 벡터를 정규화 시켜준다. (단위 벡터)

                var force = Mathf.Max( limbParts[goalIter].stiffness * (currentLenght - restLenght), 0.0f); //아무런 움직임이 없을땐 force=0 
                //Debug.Log((AvatarIKGoal)goalIter+", "+"currentLenght : "+ currentLenght + " , restLengtht : " + restLenght);
                //Debug.Log("force : " + force);

                deltaPosition += (localForce * force * limbParts[goalIter].goalPullWeight * limbParts[goalIter].goalWeight); //Weight값이 클수록 영향을 많이 받으니깐
                                                                                                                             //Position 변화값도 커짐
                //Debug.Log("deltaPosition : " + deltaPosition);
            }

            deltaPosition /= (maxPullIteration - iter); 
            bodyPosition += deltaPosition;
            //?? ㅠㅠ
            //변화값만큼 bodyPosition에 계속해서 더해라..?
        }

        limbParts.Dispose(); //Array 해제 

        return bodyPosition - originalBodyPosition; //움직인 후 몸 중심위치 - 처음 몸 중심위치
    }

    
    private void Solve(AnimationStream stream)
    {
        AnimationHumanStream humanStream = stream.AsHuman();

        bodyPosition = humanStream.bodyPosition;
        Vector3 bodyPositionDelta = SolvePull(stream); //몸의 중심위치 변화값 
        
        bodyPosition += bodyPositionDelta; //변화값들을 더해가며 bodyPosition 위치도 변함

        humanStream.bodyPosition = bodyPosition; //그걸 humanstream.bodyPosition으로 재설정

        humanStream.SolveIK();//IK Solver 실행. Humanoid IK Solver는 현재 AnimationHumanStream에 설정된 IK goal 위치,회전 및 가중치를 사용하여 실행된다. 
    }
    //effector를 움직임으로써 팔,다리가 움직이는데 이때 bodyPosition의 변화값을 구함 
    
    public void ProcessRootMotion(AnimationStream stream) { }

    public void ProcessAnimation(AnimationStream stream)
    {
        SetMaximumExtension(stream, ref leftArm);
        SetMaximumExtension(stream, ref rightArm);
        SetMaximumExtension(stream, ref leftLeg);
        SetMaximumExtension(stream, ref rightLeg);
        //왼팔,오른팔,왼쪽다리,오른쪽다리 길이 설정 

        SetEffector(stream, AvatarIKGoal.LeftFoot, ref leftFootEffector); //
        SetEffector(stream, AvatarIKGoal.RightFoot, ref rightFootEffector);
        SetEffector(stream, AvatarIKGoal.LeftHand, ref leftHandEffector);
        SetEffector(stream, AvatarIKGoal.RightHand, ref rightHandEffector);
        //FullBodyIK.cs로 부터 받아온 EffectorHandle 구조체의 멤버 변수 값들을 AvatarIKGoal의 위치,회전,가중치값으로 설정 

        SetHintEffector(stream, AvatarIKHint.LeftKnee, ref leftKneeHintEffector);
        SetHintEffector(stream, AvatarIKHint.RightKnee, ref rightKneeHintEffector);
        SetHintEffector(stream, AvatarIKHint.LeftElbow, ref leftElbowHintEffector);
        SetHintEffector(stream, AvatarIKHint.RightElbow, ref rightElbowHintEffector);
        //FullBodyIK.cs로 부터 받아온 HintEffectorHandle 구조체의 멤버 변수 값들을 AvatarIKHint의 위치,가중치값으로 설정 

        SetLookAtEffector(stream, ref lookAtEffector);

        SetBodyEffector(stream, ref bodyEffector);

        Solve(stream);
    }
}
