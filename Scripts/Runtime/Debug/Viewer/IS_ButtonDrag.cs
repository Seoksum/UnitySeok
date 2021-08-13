/// 작성자: 백인성 
/// 작성일: 2018-08-23
/// 수정일: 2018-08-23
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 수정이력 
/// (2018-07-25) 백인성 
///		1. 함수 구조 및 네이밍 규칙 적용
/// (2018-10-08) 백인성
///		1. 토글 기능 수정

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

using FNI;
namespace FNI
{
	/// <summary>
	/// 토글의 기본형입니다.
	/// </summary>
	public class IS_ButtonDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
	{
		public RectTransform Rect
		{
			get
			{
				if (m_rect == null)
					m_rect = GetComponent<RectTransform>();

				return m_rect;
			}
		}
		private RectTransform m_rect;
		protected UnityAction<Vector2> action_Drag;

		public virtual void OnBeginDrag(PointerEventData ped)
		{
			if (action_Drag != null)
			{
				action_Drag(ped.delta);
			}
		}
		public virtual void OnDrag(PointerEventData ped)
		{
			if (action_Drag != null)
			{
				action_Drag(ped.delta);
			}
		}
		public void OnEndDrag(PointerEventData ped)
		{
			if (action_Drag != null)
			{
				action_Drag(ped.delta);
			}
		}

		/// <summary>
		/// 지정된 이벤트를 추가합니다.
		/// </summary>
		/// <param name="dragAction">추가할 이벤트</param>
		public void AddListener(UnityAction<Vector2> dragAction)
		{
			action_Drag += dragAction;
		}
		/// <summary>
		/// 지정된 이벤트를 제거합니다.
		/// </summary>
		/// <param name="action">제거할 이벤트</param>
		public void RemoveListener(UnityAction<Vector2> dragAction)
		{
			action_Drag -= dragAction;
		}

		/// <summary>
		/// 모든 이벤트를 제거합니다.
		/// </summary>
		public void RemoveAllListeners()
		{
			action_Drag = null;
		}
	}
}