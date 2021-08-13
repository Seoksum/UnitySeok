/// 작성자: 김윤빈
/// 작성일: 2021-04-15
/// 수정일: 
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 수정이력
/// (2021-07-15) 백인성
/// 1. 기능 정리 함
///    => 기존 중구난방이던 데이터통신을 이벤트로 정리
///    => 수신된 데이터를 이벤트에의해 전달 되도록 수정함
///    => 반대로 서버로 데이터를 보낼 때 해당 클래스를 이용하도록 함.
/// 2. Vital Sign 추가
/// 3. 명령어 수신 이벤트
///    => Inspector나 해당 스크립트의 각 이벤트용 변수에 등록하여 사용바랍니다.
/// 4. 명령어 처리 순서
///    => 초기화(DeviceCommandType.Controll/DeviceControllType.Initialization)
///       1) 수신된 명령은 onDeviceInit 이벤트를 통해 전달 된다.
///       2) 장치의 초기화를 진행한다.(소프트웨어 및 하드웨어)
///       3) 초기화가 완료되면 SendControlFeedback(DeviceControllType.Initialization);를 호출해준다.
///    => 캐릭터 변경(DeviceCommandType.ChangeCharacter)
///       1) 수신된 명령은 onCharacter 이벤트를 통해 전달 된다.
///       2) 캐릭터를 변경한다.(소프트웨어 및 하드웨어)
///       3) 변경이 완료 되면 CharacterChangeComplete(CharactorType.(성별));을 호출해준다.
///       4) 올바른 성별로 처리하게 되면 onCharacter_Done을 호출한다.
///       4) 잘못된 성별로 처리하게 되면 onCharacter_MissMatch를 호출한다.
///			 4-1) 3)을 다시 실행하여 올바른 캐릭터를 선택한다.

using FNI.XRST;
using FNI.NET;
using FNI.NET.Utility;
using FNI.Common.Utils;

using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FNI.Data
{
	[System.Serializable]
	public class CharacterEvent : UnityEvent<CharactorType> { }
	[System.Serializable]
	public class VitalSignEvent : UnityEvent<VitalSignFull> {  }

    public class FNI_DataManager : FNI_NetBase
    {
		#region Singleton
		private static FNI_DataManager _instance;
		public static FNI_DataManager Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = FindObjectOfType<FNI_DataManager>();
					if (_instance == null)
						Debug.LogError("DataManager를 찾을 수 없습니다. ");
				}
				return _instance;
			}
		}
		#endregion

		[Header("[Receive Event]")]
		// Controll
		public UnityEvent onDeviceInit;
		public UnityEvent onSyncStart;
		public UnityEvent onSyncStop;
		public UnityEvent onSyncPause;
		public UnityEvent onSyncResume;
		// Charactor
		public CharacterEvent onCharacter;
		// VitalSign
		public VitalSignEvent onVitalSign;

		[Header("[Charactor Select Event]")]
		/// <summary>
		/// 정상 선택되었을 때 호출됩니다.
		/// </summary>
		public CharacterEvent onCharacter_Done;
		/// <summary>
		/// 비정상 선택되었을 때 호출됩니다.
		/// </summary>
		public UnityEvent onCharacter_MissMatch;

		/// <summary>
		/// 어떤 케릭터로 바꿔야할지 서버에서 명령이 들어오면 적용
		/// </summary>
		private CharactorType receiveCharactor;

		protected override void Start()
        {
			dataType = NetworkDataType.T_DeviceFeedback;
			base.Start();
		}

		/// <summary>
		/// 서버로 현재 연결된 장비의 피드백을 보내줍니다.
		/// </summary>
		/// <param name="tool"></param>
		public void SendToolFeedback(PacomToolType tool, BodyPosition position = BodyPosition.None)
		{
            List<byte> dataList = new List<byte>
            {
                (byte)dataType,
                (byte)DeviceCommandType.Data,
                (byte)tool
            };

            if (tool == PacomToolType.Stethoscope)
			{
				dataList.Add((byte)position);
				Debug.Log($"{tool}/{position} => [{FNI_Device.IP.ServerAddress}]");
			}
			else
				Debug.Log($"{tool} => [{FNI_Device.IP.ServerAddress}]");

			Send(dataList.ToArray());
		}
		/// <summary>
		/// 서버로 캐릭터 변경 피드백을 보내줍니다.
		/// </summary>
		/// <param name="commandType"></param>
		public void SendCharactorFeedback(CharactorType charactor)
		{
            List<byte> dataList = new List<byte>
            {
                (byte)dataType,
                (byte)DeviceCommandType.ChangeCharacter,
                (byte)charactor
            };

			Debug.Log($"Charactor Feedback [{charactor}] => [{FNI_Device.IP.ServerAddress}]");

			Send(dataList.ToArray());
		}
		/// <summary>
		/// 서버로 Initialize 등의 Control 피드백을 보내줍니다.
		/// </summary>
		/// <param name="control"></param>
		public void SendControlFeedback(DeviceControllType control)
		{
            List<byte> dataList = new List<byte>
            {
                (byte)dataType,
                (byte)DeviceCommandType.Controll,
                (byte)control
            };

			Debug.Log($"Control Feedback [{control}] => [{FNI_Device.IP.ServerAddress}]");

			Send(dataList.ToArray());
		}
		/// <summary>
		/// 데이터 수신
		/// </summary>
		/// <param name="_data"></param>
        protected override void Receive(NetData _data)
        {
			_ = _data.data[0]; // 데이터 타입

			DeviceCommandType commandType = (DeviceCommandType)_data.data[1];

			switch (commandType)
			{
				case DeviceCommandType.ChangeCharacter:
					ReceiveCharacter(_data.data);
					break;
				case DeviceCommandType.Controll:
					ReceiveController(_data.data);
					break;
				case DeviceCommandType.Data:
					ReceiveVitalSign(_data.data);
					break;
				default:
					break;
			}
		}
		private void ReceiveCharacter(byte[] _data)
		{
			receiveCharactor = (CharactorType)_data[2];

			onCharacter.Invoke(receiveCharactor);

			Debug.Log($"Receive Character [ {receiveCharactor} ]");
		}
		private void ReceiveController(byte[] _data)
		{
			DeviceControllType controlType = (DeviceControllType)_data[2];

			switch (controlType)
			{
				case DeviceControllType.Initialization: onDeviceInit?.Invoke(); break;
				case DeviceControllType.Start:			onSyncStart?.Invoke();  break;
				case DeviceControllType.Stop:			onSyncStop?.Invoke();   break;
				case DeviceControllType.Pause:			onSyncPause?.Invoke();  break;
				case DeviceControllType.Resume:			onSyncResume?.Invoke(); break;
			}

			Debug.Log($"Receive Controller: {controlType}");
		}
		private void ReceiveVitalSign(byte[] _data)
		{
			VitalSignFull vitalSign = new VitalSignFull();
			vitalSign.Set(_data, 2);

			onVitalSign?.Invoke(vitalSign);

			Debug.Log($"Receive VitalSign:\n {vitalSign}");
		}

		/// <summary>
		/// 모델 변경이 완료될 때 케릭터 타입을 넣어서 호출해주시면 됩니다.
		/// </summary>
		/// <param name="characterType">케릭터 타입 파라미터입니다.</param>
		public virtual bool CharacterChangeComplete(CharactorType charactorType)
        {
			// receiveCharacter(서버 -> 클라)로 받은 명령 캐릭터 타입과 characterType(실제 연결한 캐릭터 타입)이 일치하는지 판별

			// 일치한다면 케릭터 생성해줌
			if ((receiveCharactor & charactorType) == charactorType)
			{
				SendCharactorFeedback(charactorType | CharactorType.Done);
				onCharacter_Done?.Invoke(charactorType);
				return true;
			}
			else
			{
				onCharacter_MissMatch?.Invoke();
				return false;
			}
		}
















		#region Test UI 입력용, 삭제예정
		[Obsolete("삭제예정이므로 사용하지 말 것")]
		[SerializeField]
		private BodyPosition position = BodyPosition.LU;

		[Obsolete("삭제예정이므로 사용하지 말 것")]
		public void SendInitDone()
        {
			SendControlFeedback(DeviceControllType.Initialization);
		}
		[Obsolete("삭제예정이므로 사용하지 말 것")]
		public void SendChangeComplete_Male()
		{
			CharacterChangeComplete(CharactorType.Male);
		}
		[Obsolete("삭제예정이므로 사용하지 말 것")]
		public void SendChangeComplete_Female()
		{
			CharacterChangeComplete(CharactorType.Female);
		}

		[Obsolete("삭제예정이므로 사용하지 말 것")]
		public void ShoulderTool()
		{
			SendToolFeedback(PacomToolType.Shoulder);
		}
		[Obsolete("삭제예정이므로 사용하지 말 것")]
		public void PulseBendTool()
		{
			SendToolFeedback(PacomToolType.PulseBend);
		}
		[Obsolete("삭제예정이므로 사용하지 말 것")]
		public void StethoscopeTool(BodyPosition position)
		{
			SendToolFeedback(PacomToolType.Stethoscope, position);
		}
		[Obsolete("삭제예정이므로 사용하지 말 것")]
		public void SetBodyPosition(int num)
		{
			switch (num)
			{
				case 0: position = BodyPosition.LU; break;
				case 1: position = BodyPosition.LD; break;
				case 2: position = BodyPosition.RU; break;
				case 3: position = BodyPosition.RD; break;
			}
		}
		[Obsolete("삭제예정이므로 사용하지 말 것")]
		public void StethoscopeTool()
		{
			SendToolFeedback(PacomToolType.Stethoscope, position);
		}
		#endregion
	}
}