/// 작성자: 백인성
/// 작성일: 2021-04-19
/// 수정일: 
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 수정이력
/// 

using FNI.NET.Utility;

using System;
using System.Net;
using System.Text;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

namespace FNI.XRST
{
    #region Global Struct
    [Serializable]
    public struct PlayerBaseInfo
    {
        /// <summary>
        /// 사용자 역할
        /// </summary>
        public enum RoleType
        {
            None,
            /// <summary>
            /// 팀 리더
            /// </summary>
            TeamLeader,
            /// <summary>
            /// 팀원
            /// </summary>
            TeamMember,
            /// <summary>
            /// 싱글모드
            /// </summary>
            Single
        }

        public RoleType role;
        public string name;
        public string id;

        public PlayerBaseInfo(RoleType role, string name, string id)
        {
            this.role = role;
            this.name = name;
            this.id = id;
        }
        public bool IsEmpty()
        {
            return role == RoleType.None && name == "" && id == "";
        }

        public override string ToString()
        {
            return $"{id}-{name}";
        }
        public byte[] ToByte()
        {
            List<byte> datas = new List<byte>
            {
                (byte)role
            };

            datas.AddRange(name.ToByte());
            datas.AddRange(id.ToByte());

            return datas.ToArray();
        }
        public void ByteTo(byte[] datas)
        {
            role = (RoleType)datas[0];

            int start = 1;

            name = datas.ByteToString(out int count, start);
            id = datas.ByteToString(out _, start + count);
        }

        public static bool operator ==(PlayerBaseInfo a, PlayerBaseInfo b)
        {
            return a.role == b.role && a.name == b.name && a.id == b.id;
        }
        public static bool operator !=(PlayerBaseInfo a, PlayerBaseInfo b)
        {
            return a.role != b.role || a.name != b.name || a.id != b.id;
        }
    }
    [Serializable]
    public struct XRSTConst
    {
        /// <summary>
        /// 연결이 없음 메시지
        /// </summary>
        public const string kNoConnect = "No Connect";
        /// <summary>
        /// 연결이 없음 IP
        /// </summary>
        public const string kNoIP = "000.000.000.000";
    }
    [Serializable]
    public class XRST_Item
    {
        public ItemCode Prop
        {
            get => (ItemCode)num;
            set
            {
                if ((int)ItemCode.Length < (int)value)
                    num = 0;
                else
                    num = (int)value;
            }
        }
        public BodyCode Body
        {
            get => (BodyCode)num;
            set
            {
                if ((int)BodyCode.Length < (int)value)
                    num = 0;
                else
                    num = (int)value;
            }
        }
        public ClothCode Cloth
        {
            get => (ClothCode)num;
            set
            {
                if ((int)ClothCode.Length < (int)value)
                    num = 0;
                else
                    num = (int)value;
            }
        }
        public AreaCode Area
        {
            get => (AreaCode)num;
            set
            {
                if ((int)AreaCode.Length < (int)value)
                    num = 0;
                else
                    num = (int)value;
            }
        }
        public UICode UI
        {
            get => (UICode)num;
            set
            {
                if ((int)UICode.Length < (int)value)
                    num = 0;
                else
                    num = (int)value;
            }
        }

        //Item
        public ItemType type;
        public int num;
        public int value;

        public byte[] Tobyte()
        {
            return new byte[] { (byte)type, (byte)num };
        }

        public override string ToString()
        {
            switch (type)
            {
                case ItemType.Prop: return ((ItemCode)num).ToString();
                case ItemType.Body: return ((BodyCode)num).ToString();
                case ItemType.Cloth: return ((ClothCode)num).ToString();
                case ItemType.Area: return ((AreaCode)num).ToString();
                case ItemType.UI: return ((UICode)num).ToString();
                default: return "No Type";
            }
        }
        public string ToStringKR()
        {
            switch (type)
            {
                case ItemType.Prop: return ((Kr_ItemCode)num).ToString();
                case ItemType.Body: return ((Kr_BodyCode)num).ToString();
                case ItemType.Cloth: return ((Kr_ClothCode)num).ToString();
                case ItemType.Area: return ((Kr_AreaCode)num).ToString();
                case ItemType.UI: return ((Kr_UICode)num).ToString();
                default: return "No Type";
            }
        }
    }
    /// <summary>
    /// 통신에 사용되는 데이터를 담아두는 곳입니다.
    /// </summary>
    [Serializable]
    public class NetData
    {
        public byte this[int index] { get => data[index]; }
        /// <summary>
        /// 데이터의 타입입니다.
        /// </summary>
        public NetworkDataType Type { get => (NetworkDataType)data[0]; }
        /// <summary>
        /// <see cref="NetworkDataType"/>을 제외한 나머지를 String으로 변환 합니다.
        /// </summary>
        public string GetString => Encoding.UTF8.GetString(RealData);
        /// <summary>
        /// <see cref="NetworkDataType"/>를제외한 실제 데이터입니다.
        /// </summary>
        public byte[] RealData
        {
            get
            {
                return GetByteArea(1);
            }
        }

        /// <summary>
        /// 수신된 데이터
        /// </summary>
        public byte[] data;
        /// <summary>
        /// 데이터를 보낸 곳
        /// </summary>
        public IPEndPoint ip;

        public byte[] GetByteArea(int start)
        {
            byte[] _data = new byte[data.Length - start];
            Array.Copy(data, start, _data, 0, _data.Length);

            return _data;
        }
    }
    /// <summary>
    /// 연결이 완료된 장비 입니다.
    /// </summary>
    [Serializable]
    public class Connected
    {
        /// <summary>
        /// 연결한 장비명
        /// </summary>
        public string deviceName;
        /// <summary>
        /// 연결한 장비 종류
        /// </summary>
        public FNI_Device.DeviceType deviceType;
        /// <summary>
        /// 연결된 IP
        /// </summary>
        public IPEndPoint ip;
    }
    public class XRSTExeption : Exception
    {
        public XRSTExeption(string scripts, string func, string message) : base($"[{scripts}][{func}] =>{message}") { }
    }

    [Serializable]
    public class VitalSign
    {
        public bool isUse = true;
        /// <summary>
        /// 수축기 혈압, High Blood Pressure, 120
        /// </summary>
        public int HighBP = 120;
        /// <summary>
        /// 이완기 혈압, Low Blood Pressure, 80
        /// </summary>
        public int LowBP = 80;
        /// <summary>
        /// 맥박, Pulse Rate, 80회/분
        /// </summary>
        public int PR = 80;
        /// <summary>
        /// 호흡수, Respiratory Rate, 분당 12~18회
        /// </summary>
        public int RR = 15;
        /// <summary>
        /// 체온, Body Temperature, 36~37도
        /// </summary>
        public float BT = 36.5f;
        /// <summary>
        /// 산소포화도, Saturation Of Partial Pressure Oxygen, 95%이상
        /// </summary>
        public int SPO2 = 97;

        public virtual byte[] Get()
        {
            List<byte> data = new List<byte>
            {
                (byte)(isUse ? 1 : 0)
            };

            if (isUse)
            {
                data.AddRange(HighBP.ToByte());
                data.AddRange(LowBP.ToByte());
                data.AddRange(PR.ToByte());
                data.AddRange(RR.ToByte());
                data.AddRange(BT.ToByte());
                data.AddRange(SPO2.ToByte());
            }
            return data.ToArray();
        }
        public virtual int Set(byte[] data, int _start = 1)
        {
            int start = _start;
            isUse = data[start] == 1;

            int count = 1;
            if (isUse)
            {
                start += count;
                HighBP = data.ByteToInt(out count, start);
                start += count;
                LowBP = data.ByteToInt(out count, start);
                start += count;
                PR = data.ByteToInt(out count, start);
                start += count;
                RR = data.ByteToInt(out count, start);
                start += count;
                BT = data.ByteToFloat(out count, start);
                start += count;
                SPO2 = data.ByteToInt(out count, start);
            }

            return start + count - _start;
        }
#if SERVER
        public VitalSignData ToWRESTData()
        {
            return new VitalSignData()
            {
                highBP = HighBP,
                lowBP = LowBP,
                PR = PR,
                RR = RR,
                BT = BT,
                SPO2 = SPO2
            };
        }
#endif
        public override string ToString()
        {
            return $"BP: {HighBP}/{LowBP}\n" +
                   $"PR: {PR}\n" +
                   $"RR: {RR}\n" +
                   $"BT: {BT}\n" +
                   $"SPO2: {SPO2}\n";
        }
    }
    [Serializable]
    public class VitalSignFull : VitalSign
    {
        public ChestSoundType chestSound_LU = ChestSoundType.Normal;
        public ChestSoundType chestSound_LD = ChestSoundType.Normal;
        public ChestSoundType chestSound_RU = ChestSoundType.Normal;
        public ChestSoundType chestSound_RD = ChestSoundType.Normal;

        public override byte[] Get()
        {
            List<byte> data = new List<byte>();

            data.AddRange(base.Get());

            if (isUse)
            {
                data.Add((byte)chestSound_LU);
                data.Add((byte)chestSound_LD);
                data.Add((byte)chestSound_RU);
                data.Add((byte)chestSound_RD);
            }
            return data.ToArray();
        }
        public override int Set(byte[] data, int _start = 1)
        {
            int length = base.Set(data, _start);
            int number = length + _start;
            if (isUse)
            {
                chestSound_LU = (ChestSoundType)data[number];
                number++;
                chestSound_LD = (ChestSoundType)data[number];
                number++;
                chestSound_RU = (ChestSoundType)data[number];
                number++;
                chestSound_RD = (ChestSoundType)data[number];
            }

            return length + 4;
        }
        public override string ToString()
        {
            return base.ToString() +
                   $"LU: {chestSound_LU}\n" +
                   $"LD: {chestSound_LD}\n" +
                   $"RU: {chestSound_RU}\n" +
                   $"RD: {chestSound_RD}\n";
        }
    }
#if SERVER
    [Serializable]
    public class Player
    {
        public IPAddress IPAddress { get => connected.ip.Address; }
        public string Info { get => info.ToString(); }
        public bool IsSelected { get => info.IsEmpty() == false; }

        public PlayerBaseInfo info;
        public Connected connected;
    }
#endif
    [Serializable]
    public abstract class AlwaysUpdate : MonoBehaviour
    {
        public abstract void AlwaysUpdating();
    }
    [Serializable]
    public class XRST_Step
    {
        public UnityAction onInit;
        public UnityAction onStart;
        public UnityAction onUpdate;
        public UnityAction onEnded;

        public StepState step;

        public XRST_Step(UnityAction onInit = null)
        {
            this.onInit = onInit;
            step = StepState.None;
        }

        public bool State()
        {
            switch (step)
            {
                case StepState.None: onInit?.Invoke(); goto default;
                case StepState.Start: onStart?.Invoke(); goto default;
                case StepState.Update: onUpdate?.Invoke(); goto default;
                default: return false;

                case StepState.Ended:
                    Reset();
                    onEnded?.Invoke();
                    return true;
            }
        }
        public void Reset()
        {
            step = StepState.None;
        }
        public void Next()
        {
            step++;
        }
        public void Ended()
        {
            step = StepState.Ended;
        }
    }
    #endregion

    #region Interface
    #endregion

    #region Event
    /// <summary>
    /// Inspector창에 표시되는 Event입니다. 인자로 <see cref="string"/>을 사용하고 있습니다.
    /// </summary>
    [Serializable]
    public class StringEventHandler : UnityEvent<string> { }
    /// <summary>
    /// Inspector창에 표시되는 Event입니다. 인자로 <see cref="float"/>을 사용하고 있습니다.
    /// </summary>
    [Serializable]
    public class FloatEventHandler : UnityEvent<float> { }
    /// <summary>
    /// Inspector창에 표시되는 Event입니다. 인자로 <see cref="NetData"/>을 사용하고 있습니다.
    /// </summary>
    [Serializable]
    public class NetDataEventHandler : UnityEvent<NetData> { }
#if SERVER
    /// <summary>
    /// Inspector창에 표시되는 Event입니다. 인자로 <see cref="Connected"/>을 <see cref="List{T}"/>배열로 사용하고 있습니다.
    /// </summary>
    [Serializable]
    public class ConnectedEventHandler : UnityEvent<List<Connected>> { }
#endif
    #endregion

    #region Enum
    public enum SequenceCode
    {
        None,
        /// <summary>
        /// 개방성 복합골절
        /// </summary>
        SQ1,
        /// <summary>
        /// 복수/흉부 폭발 및 관통상
        /// </summary>
        SQ2,
        SQ3,
        SQ4
    }
    /// <summary>
    /// 환부 특성
    /// </summary>
    public enum InjuredSpotCode
    {
        /// <summary>
        /// 팔아래
        /// </summary>
        P1,
        /// <summary>
        /// 다리 위
        /// </summary>
        P2,
        /// <summary>
        /// 다리아래
        /// </summary>
        P3,
        /// <summary>
        /// 복부
        /// </summary>
        P4,
        /// <summary>
        /// 흉부
        /// </summary>
        P5
    }
    /// <summary>
    /// 사고특성
    /// </summary>
    public enum AccidentCode
    {
        /// <summary>
        /// 교통사고(차량 전차)
        /// </summary>
        A1,
        /// <summary>
        /// 추락/충격
        /// </summary>
        A2,
        A3,
        A4
    }
    public enum Operation2Code
    {
        MR_Team,
        MR_Single,
        VR_Single
    }
    /// <summary>
    /// 시나리오, 환경 특성
    /// </summary>
    public enum ScenarioCode
    {
        /// <summary>
        /// 사고현장
        /// </summary>
        AC,
        /// <summary>
        /// 앰뷸러스
        /// </summary>
        AM,
        /// <summary>
        /// 닥터헬기
        /// </summary>
        DH,
        /// <summary>
        /// 응급실
        /// </summary>
        ER,
        /// <summary>
        /// 위급상황 - 의식저하
        /// </summary>
        E1,
        /// <summary>
        /// 위급상황 - 호흡곤란
        /// </summary>
        E2,
        /// <summary>
        /// 위급상황 - 혈압저하
        /// </summary>
        E3
    }
    /// <summary>
    /// 운영특성
    /// </summary>
    [Flags]
    public enum MissionMainCategory
    {
        None = 0x00,
        준비 = 0x01,
        사정 = 0x02,
        처치 = 0x04,
        재 = 0x08,
    }
    public enum MissionSubCategory
    {
        /// <summary>
        /// 준비
        /// </summary>
        None,
        A,
        B,
        C,
        D,
        E,
        ETC,
        The_C
    }
    [Flags]
    public enum CharactorType
    {
        None = 0x0000,
        Done = 0x0001,

        Male = 0x0002,
        Female = 0x0004,
        Soldier = 0x0008,

        SoldierMale = Soldier | Male,
        SoldierFemale = Soldier | Female
    }
    public enum ItemType
    {
        Prop,
        Body,
        Cloth,
        Area,
        UI,
        Fail
    }
    public enum ItemCode
    {
        None = 0,
        //Tools
        Table_1,
        Table_2,
        /// <summary>
        /// 비 재호흡 마스크
        /// </summary>
        ValveMask,
        /// <summary>
        /// 도뇨관
        /// </summary>
        FoleyCatheter,
        /// <summary>
        /// 수액 라인
        /// </summary>
        LVLine,
        /// <summary>
        /// 신축성 압박붕대
        /// </summary>
        ElasticBandage,
        /// <summary>
        /// 지혈대
        /// </summary>
        Tourniquet,
        /// <summary>
        /// 혈압 측정용 밴드
        /// </summary>
        BP_Cuff,
        /// <summary>
        /// 약액 주입 및 센서 삽입등의 통로 확보용 보조 도구
        /// </summary>
        Catheter,
        /// <summary>
        /// 수액
        /// </summary>
        SalineVinyl,
        /// <summary>
        /// 팬라이트
        /// </summary>
        PenLight,
        /// <summary>
        /// 적외선 온도계
        /// </summary>
        InfraredThemometer,
        /// <summary>
        /// 수술용 테이프
        /// </summary>
        Tape,
        /// <summary>
        /// 트라마돌, 오피오이드 진통제
        /// </summary>
        Tramadol,
        /// <summary>
        /// 흡입관
        /// </summary>
        Suction,
        /// <summary>
        /// 청진기
        /// </summary>
        Stethoscope,
        /// <summary>
        /// 4(inc) by 4(inc) 사이즈 거즈
        /// </summary>
        Gauze_4x4,
        /// <summary>
        /// 손 소독제
        /// </summary>
        HandSanitizer,
        /// <summary>
        /// 산소포화도 측정기
        /// </summary>
        Pulse_OX_imetry,
        /// <summary>
        /// 가위
        /// </summary>
        Scissors,
        /// <summary>
        /// 수용성 윤활류, 카테터용
        /// </summary>
        Gel,
        /// <summary>
        /// 수술용 마스크
        /// </summary>
        Mask,
        /// <summary>
        /// 수술용 장갑, PPE
        /// </summary>
        SurgicalGlove,
        /// <summary>
        /// 알콜 솜
        /// </summary>
        AlcoholSwab,
        /// <summary>
        /// 경추 보호대
        /// </summary>
        NeckCollar,
        /// <summary>
        /// 모포
        /// </summary>
        Blanket,
        /// <summary>
        /// 식염수
        /// </summary>
        SalinePlastic,
        /// <summary>
        /// 링거 대
        /// </summary>
        RingerPole,
        /// <summary>
        /// 산소탱크
        /// </summary>
        OxygenTank,
        /// <summary>
        /// 기관내 튜브
        /// </summary>
        ET_Tube,
        /// <summary>
        /// 후두 마스크
        /// </summary>
        LMA_Tube,
        /// <summary>
        /// 비 인두기도, 비강 인두기도 삽입관
        /// </summary>
        NPA,
        /// <summary>
        /// 구강 인두기도
        /// </summary>
        OPA,
        /// <summary>
        /// 부목
        /// </summary>
        Splint,
        /// <summary>
        /// 재세동기
        /// </summary>
        Defibrillator,
        /// <summary>
        /// 수액세트
        /// </summary>
        SalineSet,
        /// <summary>
        /// 수액 양 조절
        /// </summary>
        SalineValve,
        /// <summary>
        /// 비 재호흡마스크
        /// </summary>
        ReverseMask,
        /// <summary>
        /// 메달린 수액백
        /// </summary>
        HangingSlineVinyl,
        Length
    }
    public enum BodyCode
    {
        /// <summary>
        /// 환자
        /// </summary>
        Body = 0,
        /// <summary>
        /// 머리, 두부
        /// </summary>
        Head,
        /// <summary>
        /// 구강
        /// </summary>
        Mouse,
        /// <summary>
        /// 경부
        /// </summary>
        Neck,
        /// <summary>
        /// 환자 귀
        /// </summary>
        Ear,
        /// <summary>
        /// 어깨
        /// </summary>
        Shoulder,
        /// <summary>
        /// 흉부
        /// </summary>
        Chest,
        /// <summary>
        /// 손목
        /// </summary>
        Wrist,
        /// <summary>
        /// 사타구니
        /// </summary>
        Groin,
        /// <summary>
        /// 발가락
        /// </summary>
        Toes,
        /// <summary>
        /// 복부 시진용
        /// </summary>
        Abdominal,
        /// <summary>
        /// 복부 촉진, 좌상
        /// </summary>
        Abdominal_LU,
        /// <summary>
        /// 복부 촉진, 좌하
        /// </summary>
        Abdominal_LD,
        /// <summary>
        /// 복부 촉진, 우상
        /// </summary>
        Abdominal_RU,
        /// <summary>
        /// 복부 촉진, 우하
        /// </summary>
        Abdominal_RD,
        /// <summary>
        /// 감각체크, 왼팔
        /// </summary>
        Arm_L,
        /// <summary>
        /// 감각체크, 오른팔
        /// </summary>
        Arm_R,
        /// <summary>
        /// 감각체크, 왼쪽 허벅지
        /// </summary>
        Leg_Up_L,
        /// <summary>
        /// 감각체크, 왼쪽 종아리 또는 발
        /// </summary>
        Leg_Dw_L,
        /// <summary>
        /// 감각체크, 왼쪽 허벅지
        /// </summary>
        Leg_Up_R,
        /// <summary>
        /// 감각체크, 오른쪽 종아리 또는 발
        /// </summary>
        Leg_Dw_R,
        /// <summary>
        /// 입안
        /// </summary>
        InMouse,
        /// <summary>
        /// 얼굴
        /// </summary>
        Face,
        /// <summary>
        /// 겨드랑이
        /// </summary>
        Armpit,
        /// <summary>
        /// 허벅지
        /// </summary>
        Thigh,

        /// <summary>
        /// 착용한 토니켓
        /// </summary>
        On_Tourniquet = 40,
        /// <summary>
        /// 착용한 산소포화도 측정기
        /// </summary>
        On_Pulse_OX_imetry,

        /// <summary>
        /// 환부, 상처부위, 출혈부위
        /// </summary>
        TheDiseasedPart = 50,

        /// <summary>
        /// 맥박체크 위치, 상처부위의 맥
        /// </summary>
        Vein,
        /// <summary>
        /// 맥박체크 위치, 상처가 없는 부위의 맥
        /// </summary>
        VeinClean,
        /// <summary>
        /// 중앙 부정맥, 수액 용
        /// </summary>
        MedianCubitalVein,
        Length
    }
    public enum ClothCode
    {
        /// <summary>
        /// 수액 맞기 용 팔 옷, 팔 걷기
        /// </summary>
        Arm = 0,
        /// <summary>
        /// 환부 옷
        /// </summary>
        TheDiseasedPart,
        Length
    }
    public enum AreaCode
    {
        /// <summary>
        /// BPCuff 착용 부위
        /// </summary>
        BP_Cuff = 0,
        /// <summary>
        /// 산소포화도 측정기 착용 부위
        /// </summary>
        PulseOx,
        /// <summary>
        /// 토니켓 적용 부위
        /// </summary>
        Torniquet,
        /// <summary>
        /// 제새동기 적용부위
        /// </summary>
        Defibrillator,
        /// <summary>
        /// 부목 적용 부위
        /// </summary>
        Splint,
        /// <summary>
        /// 주사 부위, 수액 부위
        /// </summary>
        InjectionSite,
        /// <summary>
        /// 펜라이트 적용 부위
        /// </summary>
        PenLight,
        Length
    }
    public enum UICode
    {
        /// <summary>
        /// 산소탱크용 UI
        /// </summary>
        OxygenValue,
        /// <summary>
        /// 카테터 선택
        /// </summary>
        CatheterChoose,
        /// <summary>
        /// 수액 속도 설정
        /// </summary>
        SalineSpeed,
        Length
    }

    public enum Kr_ItemCode
    {
        //Tools
        없음,
        테이블_1,
        테이블_2,
        벨브마스크,
        도뇨관,
        수액_라인,
        신축성압박붕대,
        지혈대,
        혈압측정용밴드,
        카테터,
        수액,
        팬라이트,
        적외선온도계,
        수술용테이프,
        트라마돌,
        흡입관,
        청진기,
        거즈,
        손소독제,
        산소포화도측정기,
        가위,
        카테터용_수용성_윤활류,
        수술용_마스크,
        PPE,
        알콜_솜,
        경추보호대,
        모포,
        식염수,
        링거거치대,
        산소탱크,
        기관내튜브,
        후두마스크,
        비인두기도,
        구강인두기도,
        부목,
        재세동기,
        수액세트,
        수액양조절,
        비재호흡마스크,
        메달린수액백,
    }
    public enum Kr_BodyCode
    {
        //Person
        몸 = 0,
        머리,
        구강,
        경부,
        귀,
        어깨,
        흉부,
        손목,
        사타구니,
        발가락,
        복부_시진,
        좌상복부,
        좌하복부,
        우상복부,
        우하복부,
        좌완,
        우완,
        좌허벅지,
        좌종아리,
        우허벅지,
        우종아리,
        입안,
        얼굴,
        겨드랑이,
        허벅지,

        착용중인토니켓 = 40,
        착용중인산소포화도,

        환부 = 50,

        맥박체크위치_환부,
        맥박체크위치_비환부,
        중앙부정맥,
    }
    public enum Kr_ClothCode
    {
        옷소매 = 0,
        환부옷,
    }
    public enum Kr_AreaCode
    {
        BPCuff = 0,
        산소포화도측정기,
        토니켓,
        재세동기,
        부목,
        주사,
        펜라이트
    }
    public enum Kr_UICode
    {
        산소탱크 = 0,
        주사바늘선택,
        수액속도설정
    }

    /// <summary>
    /// 미션 실행 상태
    /// </summary>
    public enum MissionState
    {
        None,
        InProcess,
        Success,
        Fail,
    }
    /// <summary>
    /// 스탭의 진행 순서
    /// </summary>
    public enum StepState
    {
        /// <summary>
        /// 아무것도 하지 않은 상태
        /// </summary>
        None,
        /// <summary>
        /// 초기화 및 활성화
        /// </summary>
        Start,
        /// <summary>
        /// 기능 업데이트
        /// </summary>
        Update,
        /// <summary>
        /// 종료 및 닫기
        /// </summary>
        Ended
    }
    /// <summary>
    /// 장비에 각종 정보를 전달 할 때
    /// </summary>
    public enum DeviceFeedbackState
    {
        /// <summary>
        /// Client에서 상태값을 전달하고자 할 때 적용
        /// </summary>
        State,
        /// <summary>
        /// 정보를 전달 할 때
        /// </summary>
        Data,
        /// <summary>
        /// Client에서 피드백을 발생시킬때
        /// </summary>
        Feedback,
        /// <summary>
        /// QR관련 정보
        /// </summary>
        QR
    }
    public enum MissionContollType
    {
        Popup,
        Mission,
        Timer,
        Info,
        VitalSign
    }
    /// <summary>
    /// 홀로렌즈의 상태 입니다.
    /// </summary>
    public enum HololensState
    {
        Battery,
    }
    /// <summary>
    /// U => UDP,
    /// T => TCP
    /// </summary>
    public enum NetworkDataType
    {
        // UDP 항목
        /// <summary>
        /// [SERVER <=> CLIENT]장치 연결 목적
        /// </summary>
        U_DeviceConnect,
        /// <summary>
        /// [SERVER <=> CLIENT]도구 동기화 목적
        /// </summary>
        U_SyncObject,
        /// <summary>
        /// [SERVER <=> CLIENT] 캐릭터 동기화 목적, Client To Server => Only OptiTrack, Server To Client => Only Hololens
        /// </summary>
        U_SyncCharactor,
        //이곳에 새로운 UDP항목 추가 할 것

        //TCP 항목
        /// <summary>
        /// [CLIENT <=> SERVER] 장치 피드백
        /// </summary>
        T_DeviceFeedback,
        /// <summary>
        /// [SERVER <=> HOLOLENS] 미션정보
        /// </summary>
        T_Mission,
        /// <summary>
        /// [SERVER <=> HOLOLENS] 학생정보
        /// </summary>
        T_Student,
        // 이곳에 새로운 TCP항목 추가 할 것

        /// <summary>   
        /// Enum길이
        /// </summary>
        Length,
    }
    /// <summary>
    /// 장치 컨트롤러 명령
    /// </summary>
    public enum DeviceCommandType
    {
        /// <summary>
        /// [Server -> Client] 캐릭터
        /// </summary>
        ChangeCharacter,
        /// <summary>
        /// [Server -> Client] 장치 제어
        /// </summary>
        Controll,
        /// <summary>
        /// [Server <-> Client] 추가 데이터 전달
        /// </summary>
        Data
    }

    public enum ControllResponeType
    {
        Order,
        Done
    }
    public enum DeviceControllType
    {
        Initialization,
        Start,
        Pause,
        Resume,
        Stop,
        Ended,
        Quit,
    }
    public enum PacomToolType
    {
        Shoulder,
        PulseBend,
        Stethoscope,
        Hololens
    }
    /// <summary>
    /// 흉부 진찰용 사운드타입
    /// </summary>
    public enum ChestSoundType
    {
        /// <summary>
        /// 정상
        /// </summary>
        Normal,
        /// <summary>
        /// 수포음
        /// </summary>
        Crackle,
        /// <summary>
        /// 천명음
        /// </summary>
        Wheezing,
        /// <summary>
        /// 협착음
        /// </summary>
        Stridor,
        /// <summary>
        /// 흉막마찰음
        /// </summary>
        PleuralFrictionRub
    }
    public enum BodyPosition
    {
        None = 0x00,
        C = 0x01,
        L = 0x02,
        R = 0x04,
        U = 0x08,
        D = 0x10,

        LU = L | U,
        LD = L | D,
        RU = R | U,
        RD = R | D,
    }
    #endregion

#if SERVER
    /// <summary>
    /// 연결된 장비의 정보 입니다.
    /// </summary>
    public class XRSTDeviceData
    {
        /// <summary>
        /// 연결 장비의 IP목록
        /// </summary>
        /// <returns></returns>
        public static List<IPEndPoint> GetAllIPList => allIPList;
        /// <summary>
        /// 연결 장비의 IP목록
        /// </summary>
        /// <returns></returns>
        public static List<IPEndPoint> GetHololensList => hololensIPList;
        /// <summary>
        /// 연결된 장비 중 OptiTrack IP
        /// </summary>
        public static IPEndPoint OptiTrackIP => optitrackIP;
        /// <summary>
        /// 연결된 장비 중 Tools IP
        /// </summary>
        public static IPEndPoint ToolsIP => toolIP;
        public static List<Player> PlayerList => playerList;

        private static List<Player> playerList = new List<Player>();

        private static List<Connected> allConnectList = new List<Connected>();
        private static List<Connected> hololensList = new List<Connected>();
        private static Connected optitrack;
        private static Connected tool;

        private static List<IPEndPoint> allIPList = new List<IPEndPoint>();
        private static List<IPEndPoint> hololensIPList = new List<IPEndPoint>();
        private static IPEndPoint optitrackIP;
        private static IPEndPoint toolIP;

        /// <summary>
        /// 연결된 장치를 기관별로 등록합니다.
        /// </summary>
        /// <param name="connectList"></param>
        public static void Set(List<Connected> connectList)
        {
            allConnectList.Clear();

            allConnectList = connectList;

            List<IPEndPoint> hololensList = GetHololensList;

            for (int cnt = 0; cnt < connectList.Count; cnt++)
            {
                if ((connectList[cnt].deviceType & FNI_Device.DeviceType.Hololens) == FNI_Device.DeviceType.Hololens)
                {
                    hololensList.Add(connectList[cnt].ip);
                    playerList.Add(new Player() { connected = connectList[cnt] });
                }
                if ((connectList[cnt].deviceType & FNI_Device.DeviceType.Optitrack) == FNI_Device.DeviceType.Optitrack)
                {
                    optitrackIP = connectList[cnt].ip;
                }
                if ((connectList[cnt].deviceType & FNI_Device.DeviceType.Tools) == FNI_Device.DeviceType.Tools)
                {
                    toolIP = connectList[cnt].ip;
                }

                allIPList.Add(connectList[cnt].ip);
            }
        }
        /// <summary>
        /// 연결된 장비의 정보를 넘겨줍니다.
        /// </summary>
        /// <param name="device">값을 가져올 장치 종류</param>
        /// <returns>IPEndPoint 또는 Connected</returns>
        public static IPEndPoint[] GetIP(FNI_Device.DeviceType device)
        {
            List<IPEndPoint> ips = new List<IPEndPoint>();
            if ((device & FNI_Device.DeviceType.Hololens) == FNI_Device.DeviceType.Hololens)
            {
                ips.AddRange(hololensIPList);
            }
            if ((device & FNI_Device.DeviceType.Optitrack) == FNI_Device.DeviceType.Optitrack)
            {
                ips.Add(optitrackIP);
            }
            if ((device & FNI_Device.DeviceType.Tools) == FNI_Device.DeviceType.Tools)
            {
                ips.Add(toolIP);
            }

            return ips.ToArray();
        }
        /// <summary>
        /// 연결된 장비의 정보를 넘겨줍니다.
        /// </summary>
        /// <param name="device">값을 가져올 장치 종류</param>
        /// <returns>IPEndPoint 또는 Connected</returns>
        public static Connected[] GetConnected(FNI_Device.DeviceType device)
        {
            List<Connected> connectors = new List<Connected>();
            if ((device & FNI_Device.DeviceType.Hololens) == FNI_Device.DeviceType.Hololens)
            {
                connectors.AddRange(hololensList);
            }
            if ((device & FNI_Device.DeviceType.Optitrack) == FNI_Device.DeviceType.Optitrack)
            {
                connectors.Add(optitrack);
            }
            if ((device & FNI_Device.DeviceType.Tools) == FNI_Device.DeviceType.Tools)
            {
                connectors.Add(tool);
            }

            return connectors.ToArray();
        }
        /// <summary>
        /// 연결된 장비의 정보를 넘겨줍니다.
        /// </summary>
        /// <param name="device">값을 가져올 장치 종류</param>
        /// <returns>IPEndPoint 또는 Connected</returns>
        public static Player GetPlayer(string info)
        {
            return playerList.Find(x => x.Info == info);
        }
        public static void Clear()
        {
            playerList.Clear();

            allConnectList.Clear();
            hololensList.Clear();
            optitrack = null;
            tool = null;

            allIPList.Clear();
            hololensIPList.Clear();
            optitrackIP = null;
            toolIP = null;
        }
    }
#endif
}