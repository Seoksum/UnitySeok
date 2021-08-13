using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using FNI;
namespace FNI
{
	public class IS_RectResizeable : MonoBehaviour
	{
		public enum Dir
		{
			UpperLeft,  
			UpperCenter,
			UpperRight, 
			MiddleLeft, 
			MiddleCenter,
			MiddleRight,
			LowerLeft,  
			LowerCenter,
			LowerRight
		
		}
		private RectTransform Rect
		{
			get
			{
				if (resizeTarget != null)
					m_rect = resizeTarget.GetComponent<RectTransform>();
				else
					m_rect = GetComponent<RectTransform>();

				return m_rect;
			}
		}
		private IS_ButtonDrag DragButton
		{ 
			get
			{
				if (m_dragButton == null)
				{
					Transform find = Rect.Find("Resizeable/Drag");
					if(find)
						m_dragButton = find.GetComponent<IS_ButtonDrag>();
				}
				return m_dragButton; 
			}
		}
		private IS_ButtonDrag U_Button
		{
			get
			{
				if (m_U_Button == null)
				{
					Transform find = Rect.Find("Resizeable/U");
					if (find)
						m_U_Button = find.GetComponent<IS_ButtonDrag>();
				}
				return m_U_Button;
			}
		}
		private IS_ButtonDrag D_Button
		{
			get
			{
				if (m_D_Button == null)
				{
					Transform find = Rect.Find("Resizeable/D");
					if (find)
						m_D_Button = find.GetComponent<IS_ButtonDrag>();
				}
				return m_D_Button;
			}
		}
		private IS_ButtonDrag L_Button
		{
			get
			{
				if (m_L_Button == null)
				{
					Transform find = Rect.Find("Resizeable/L");
					if (find)
						m_L_Button = find.GetComponent<IS_ButtonDrag>();
				}
				return m_L_Button;
			}
		}
		private IS_ButtonDrag R_Button
		{
			get
			{
				if (m_R_Button == null)
				{
					Transform find = Rect.Find("Resizeable/R");
					if (find)
						m_R_Button = find.GetComponent<IS_ButtonDrag>();
				}
				return m_R_Button;
			}
		}
		private IS_ButtonDrag UL_Button
		{
			get
			{
				if (m_UL_Button == null)
				{
					Transform find = Rect.Find("Resizeable/UL");
					if (find)
						m_UL_Button = find.GetComponent<IS_ButtonDrag>();
				}
				return m_UL_Button;
			}
		}
		private IS_ButtonDrag UR_Button
		{
			get
			{
				if (m_UR_Button == null)
				{
					Transform find = Rect.Find("Resizeable/UR");
					if (find)
						m_UR_Button = find.GetComponent<IS_ButtonDrag>();
				}
				return m_UR_Button;
			}
		}
		private IS_ButtonDrag DL_Button
		{
			get
			{
				if (m_DL_Button == null)
				{
					Transform find = Rect.Find("Resizeable/DL");
					if (find)
						m_DL_Button = find.GetComponent<IS_ButtonDrag>();
				}
				return m_DL_Button;
			}
		}
		private IS_ButtonDrag DR_Button
		{
			get
			{
				if (m_DR_Button == null)
				{
					Transform find = Rect.Find("Resizeable/DR");
					if (find)
						m_DR_Button = find.GetComponent<IS_ButtonDrag>();
				}
				return m_DR_Button;
			}
		}

		public Dir anchor;
		public Dir Pivot;

		public bool startCenter;
		public bool activeMinSize;
		public bool fullResize;
		public float areaSize = 20;
		public Transform resizeTarget;
		public Vector2 minimumSize = new Vector2(450, 400);
		public Vector2 maximumSize = new Vector2(0, 0);

		public List<RectTransform> withAnchorList = new List<RectTransform>();

		private RectTransform m_rect;

		private IS_ButtonDrag m_dragButton;
		private IS_ButtonDrag m_U_Button;
		private IS_ButtonDrag m_D_Button;
		private IS_ButtonDrag m_L_Button;
		private IS_ButtonDrag m_R_Button;
		private IS_ButtonDrag m_UL_Button;
		private IS_ButtonDrag m_UR_Button;
		private IS_ButtonDrag m_DL_Button;
		private IS_ButtonDrag m_DR_Button;

		private Vector2 oldSize = new Vector2(0, 0);

		void Start()
		{
			SetAnchor();
			SetPivot();

			DragButton?.AddListener(Drag);
			U_Button?.AddListener(Resize_U);
			D_Button?.AddListener(Resize_D);
			L_Button?.AddListener(Resize_L);
			R_Button?.AddListener(Resize_R);
			UL_Button?.AddListener(Resize_UL);
			UR_Button?.AddListener(Resize_UR);
			DL_Button?.AddListener(Resize_DL);
			DR_Button?.AddListener(Resize_DR);

			if (maximumSize.x == 0)
				maximumSize = new Vector2(Screen.width, maximumSize.y);
			if (maximumSize.y == 0)
				maximumSize = new Vector2(maximumSize.x, Screen.height);

			if (startCenter)
			{
				Rect.anchoredPosition = new Vector2((Screen.width / 2) - (Rect.sizeDelta.x / 2),
													(Screen.height / 2) - (Rect.sizeDelta.y / 2));
			}
			oldSize = Rect.sizeDelta;
		}

		public void SetActive(bool isActive)
		{
			if (activeMinSize)
			{
				if (isActive)
					Rect.sizeDelta = new Vector2(minimumSize.x, minimumSize.y);
			}
			Rect.gameObject.SetActive(isActive);
		}

		public void SetAnchor()
		{
			switch (anchor)
			{
				case Dir.UpperLeft:
					Rect.anchorMin = new Vector2(0, 1); 
					Rect.anchorMax = new Vector2(0, 1);
					break;
				case Dir.UpperCenter: 
					Rect.anchorMin = new Vector2(0.5f, 1); 
					Rect.anchorMax = new Vector2(0.5f, 1); 
					break;
				case Dir.UpperRight: 
					Rect.anchorMin = new Vector2(1, 1); 
					Rect.anchorMax = new Vector2(1, 1); 
					break;
				case Dir.MiddleLeft: 
					Rect.anchorMin = new Vector2(0, 0.5f);
					Rect.anchorMax = new Vector2(0, 0.5f);
					break;
				case Dir.MiddleCenter: 
					Rect.anchorMin = new Vector2(0.5f, 0.5f);
					Rect.anchorMax = new Vector2(0.5f, 0.5f);
					break;
				case Dir.MiddleRight: 
					Rect.anchorMin = new Vector2(1, 0.5f);
					Rect.anchorMax = new Vector2(1, 0.5f); 
					break;
				case Dir.LowerLeft: 
					Rect.anchorMin = new Vector2(0, 0); 
					Rect.anchorMax = new Vector2(0, 0); 
					break;
				case Dir.LowerCenter: 
					Rect.anchorMin = new Vector2(0.5f, 0); 
					Rect.anchorMax = new Vector2(0.5f, 0);
					break;
				case Dir.LowerRight:
					Rect.anchorMin = new Vector2(1, 0); 
					Rect.anchorMax = new Vector2(1, 0); 
					break;
			}

			SetActiveTracker();
			WithAnchor();
			Rect.anchoredPosition = new Vector2();
		}
		public void SetPivot()
		{
			switch (Pivot)
			{
				case Dir.UpperLeft: Rect.pivot = new Vector2(0, 1); break;
				case Dir.UpperCenter: Rect.pivot = new Vector2(0.5f, 1); break;
				case Dir.UpperRight: Rect.pivot = new Vector2(1, 1); break;
				case Dir.MiddleLeft: Rect.pivot = new Vector2(0, 0.5f); break;
				case Dir.MiddleCenter: Rect.pivot = new Vector2(0.5f, 0.5f); break;
				case Dir.MiddleRight: Rect.pivot = new Vector2(1, 0.5f); break;
				case Dir.LowerLeft: Rect.pivot = new Vector2(0, 0); break;
				case Dir.LowerCenter: Rect.pivot = new Vector2(0.5f, 0); break;
				case Dir.LowerRight: Rect.pivot = new Vector2(1, 0); break;
			}
			WithPivot();
			Rect.anchoredPosition = new Vector2();
		}
		private void WithAnchor()
		{
			for (int cnt = 0; cnt < withAnchorList.Count; cnt++)
			{
				withAnchorList[cnt].anchorMin = Rect.anchorMin;
				withAnchorList[cnt].anchorMax = Rect.anchorMax;
			} 
		}
		private void WithPivot()
		{
			for (int cnt = 0; cnt < withAnchorList.Count; cnt++)
			{
				withAnchorList[cnt].pivot = Rect.pivot;
			} 
		}
		public void SetActiveTracker()
		{
			if (fullResize == false)
			{
				U_Button?.gameObject.SetActive(GetActive(anchor, DirType.U));
				D_Button?.gameObject.SetActive(GetActive(anchor, DirType.D));
				L_Button?.gameObject.SetActive(GetActive(anchor, DirType.L));
				R_Button?.gameObject.SetActive(GetActive(anchor, DirType.R));
				UL_Button?.gameObject.SetActive(GetActive(anchor, DirType.UL));
				UR_Button?.gameObject.SetActive(GetActive(anchor, DirType.UR));
				DL_Button?.gameObject.SetActive(GetActive(anchor, DirType.DL));
				DR_Button?.gameObject.SetActive(GetActive(anchor, DirType.DR));
			}
			else
			{
				 U_Button?.gameObject.SetActive(true);
				 D_Button?.gameObject.SetActive(true);
				 L_Button?.gameObject.SetActive(true);
				 R_Button?.gameObject.SetActive(true);
				UL_Button?.gameObject.SetActive(true);
				UR_Button?.gameObject.SetActive(true);
				DL_Button?.gameObject.SetActive(true);
				DR_Button?.gameObject.SetActive(true);
			}
		}
		public void ResizeableArea()
		{
			if ( U_Button)  U_Button.Rect.sizeDelta = new Vector2(-areaSize, areaSize);
			if ( D_Button)  D_Button.Rect.sizeDelta = new Vector2(-areaSize, areaSize);
			if ( L_Button)  L_Button.Rect.sizeDelta = new Vector2(areaSize, -areaSize);
			if ( R_Button)  R_Button.Rect.sizeDelta = new Vector2(areaSize, -areaSize);
			if (UL_Button) UL_Button.Rect.sizeDelta = new Vector2(areaSize, areaSize);
			if (UR_Button) UR_Button.Rect.sizeDelta = new Vector2(areaSize, areaSize);
			if (DL_Button) DL_Button.Rect.sizeDelta = new Vector2(areaSize, areaSize);
			if (DR_Button) DR_Button.Rect.sizeDelta = new Vector2(areaSize, areaSize);
		}
		private bool GetActive(Dir dir, DirType type)
		{
			switch (dir)
			{
				case Dir.UpperLeft:
					if (type == DirType.D || 
						type == DirType.R ||
						type == DirType.DR) 
						return true;
					else goto default;
				case Dir.UpperCenter:
					if (type == DirType.D ||
						type == DirType.R ||
						type == DirType.L ||
						type == DirType.DR ||
						type == DirType.DL) 
						return true;
					else goto default;
				case Dir.UpperRight:
					if (type == DirType.D ||
						type == DirType.L ||
						type == DirType.DL) 
						return true;
					else goto default;

				case Dir.MiddleLeft:
					if (type == DirType.U ||
						type == DirType.R ||
						type == DirType.D ||
						type == DirType.UR ||
						type == DirType.DR) 
						return true;
					else goto default;
				case Dir.MiddleCenter:
					if (type == DirType.C)
						goto default; 
					else return true;
				case Dir.MiddleRight:
					if (type == DirType.U ||
						type == DirType.L ||
						type == DirType.D ||
						type == DirType.UL ||
						type == DirType.DL) 
						return true;
					else goto default;

				case Dir.LowerLeft:
					if (type == DirType.U ||
						type == DirType.R ||
						type == DirType.UR) 
						return true;
					else goto default;
				case Dir.LowerCenter:
					if (type == DirType.U ||
						type == DirType.R ||
						type == DirType.L ||
						type == DirType.UR ||
						type == DirType.UL)
						return true;
					else goto default;
				case Dir.LowerRight:
					if (type == DirType.U ||
						type == DirType.L ||
						type == DirType.UL)
						return true;
					else goto default;
				default:
					return false;
			}
		}

		private void Drag(Vector2 vector)
		{
			Rect.anchoredPosition += vector;
			//min Limit
			if (Rect.anchoredPosition.x < 0)
				Rect.anchoredPosition = new Vector2(0, Rect.anchoredPosition.y);
			if (Rect.anchoredPosition.y < 0)
				Rect.anchoredPosition = new Vector2(Rect.anchoredPosition.x, 0);
			//max Limit
			if ((Screen.width - Rect.sizeDelta.x) < Rect.anchoredPosition.x)
				Rect.anchoredPosition = new Vector2(Screen.width - Rect.sizeDelta.x, Rect.anchoredPosition.y);
			if ((Screen.height - Rect.sizeDelta.y) < Rect.anchoredPosition.y)
				Rect.anchoredPosition = new Vector2(Rect.anchoredPosition.x, Screen.height - Rect.sizeDelta.y);
		}
		private void Resize_U(Vector2 vector)  { Resize(DirType.U, vector); }
		private void Resize_D(Vector2 vector)  { Resize(DirType.D, vector); }
		private void Resize_L(Vector2 vector)  { Resize(DirType.L, vector); }
		private void Resize_R(Vector2 vector)  { Resize(DirType.R, vector); }
		private void Resize_UL(Vector2 vector) { Resize(DirType.UL, vector); }
		private void Resize_UR(Vector2 vector) { Resize(DirType.UR, vector); }
		private void Resize_DL(Vector2 vector) { Resize(DirType.DL, vector); }
		private void Resize_DR(Vector2 vector) { Resize(DirType.DR, vector); }

		private void Resize(DirType dir, Vector2 vector)
		{
			//Debug.Log("dir : " + dir);

			if (CheckDir(dir, DirType.L))
			{
				Rect.sizeDelta = new Vector2(Rect.sizeDelta.x - vector.x, Rect.sizeDelta.y);
			}
			if (CheckDir(dir, DirType.R))
			{
				Rect.sizeDelta = new Vector2(Rect.sizeDelta.x + vector.x, Rect.sizeDelta.y);
			}
			if (CheckDir(dir, DirType.U))
			{
				Rect.sizeDelta = new Vector2(Rect.sizeDelta.x, Rect.sizeDelta.y + vector.y);
			}
			if (CheckDir(dir, DirType.D))
			{
				Rect.sizeDelta = new Vector2(Rect.sizeDelta.x, Rect.sizeDelta.y - vector.y);
			}

			if (Rect.sizeDelta.x < minimumSize.x)
				Rect.sizeDelta = new Vector2(minimumSize.x, Rect.sizeDelta.y);
			if (Rect.sizeDelta.y < minimumSize.y)
				Rect.sizeDelta = new Vector2(Rect.sizeDelta.x, minimumSize.y);

			if (maximumSize.x < Rect.sizeDelta.x)
				Rect.sizeDelta = new Vector2(maximumSize.x , Rect.sizeDelta.y);
			if (maximumSize.y < Rect.sizeDelta.y)
				Rect.sizeDelta = new Vector2(Rect.sizeDelta.x, maximumSize.y);

			Vector2 delta = oldSize - Rect.sizeDelta;
			oldSize = Rect.sizeDelta;

			if (Rect.pivot.x < 0.5F)
			{
				if (CheckDir(dir, DirType.L) && 0 < Mathf.Abs(delta.x))
				{
					Rect.anchoredPosition = new Vector2(Rect.anchoredPosition.x + delta.x, Rect.anchoredPosition.y);
				}
			}
			if (Rect.pivot.y < 0.5F)
			{
				if (CheckDir(dir, DirType.D) && 0 < Mathf.Abs(delta.y))
				{
					Rect.anchoredPosition = new Vector2(Rect.anchoredPosition.x, Rect.anchoredPosition.y + delta.y);
				}
			}
			Drag(new Vector2());
		}
		private bool CheckDir(DirType origin, DirType check)
		{
			return (origin & check) == check;
		}
	}
	public enum DirType
	{
		C = 0x0,
		U = 0x1,
		D = 0x2,
		L = 0x4,
		R = 0x8,
		UL = U | L,
		UR = U | R,
		DL = D | L,
		DR = D | R,
	}
}

#if UNITY_EDITOR

[CustomEditor(typeof(IS_RectResizeable))]
public class IS_RectResizeableEditor : Editor
{
	private IS_RectResizeable m_target;

	void OnEnable()
	{
		m_target = base.target as IS_RectResizeable;
	}

	public override void OnInspectorGUI()
	{
		EditorGUI.BeginChangeCheck();
		float labelWidth = (EditorGUIUtility.currentViewWidth - 70) * 0.5f;

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Resize Target", GUILayout.Width(labelWidth));
		m_target.resizeTarget = (Transform)EditorGUILayout.ObjectField(m_target.resizeTarget, typeof(Transform), true);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.Space();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Anchor", GUILayout.Width(labelWidth));
		IS_RectResizeable.Dir select = (IS_RectResizeable.Dir)EditorGUILayout.EnumPopup(m_target.anchor);
		if (m_target.anchor != select)
		{
			m_target.anchor = select;
			m_target.SetAnchor();
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Pivot", GUILayout.Width(labelWidth));
		select = (IS_RectResizeable.Dir)EditorGUILayout.EnumPopup(m_target.Pivot);
		if (m_target.Pivot != select)
		{
			m_target.Pivot = select;
			m_target.SetPivot();
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.Space();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Min", GUILayout.Width(labelWidth));
		m_target.minimumSize = EditorGUILayout.Vector2Field("", m_target.minimumSize);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Max", GUILayout.Width(labelWidth));
		m_target.maximumSize = EditorGUILayout.Vector2Field("", m_target.maximumSize);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.Space();

		EditorGUILayout.LabelField("Option", GUILayout.Width(labelWidth));

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Start Center", GUILayout.Width(labelWidth));
		m_target.startCenter = EditorGUILayout.Toggle(m_target.startCenter);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Active Min Size", GUILayout.Width(labelWidth));
		m_target.activeMinSize = EditorGUILayout.Toggle(m_target.activeMinSize);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("full Resize", GUILayout.Width(labelWidth));
		bool checker = EditorGUILayout.Toggle(m_target.fullResize);
		if (m_target.fullResize != checker)
		{
			m_target.fullResize = checker;
			m_target.SetActiveTracker();
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Resizeable Area Size", GUILayout.Width(labelWidth));
		float size = EditorGUILayout.Slider(m_target.areaSize, 10, 100);
		if (m_target.areaSize != size)
		{
			m_target.areaSize = size;
			m_target.ResizeableArea();
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginVertical();
		{
			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.LabelField("With Move Anchors", GUILayout.Width(labelWidth));
				m_target.withAnchorList = ListController(m_target.withAnchorList);
			}
			EditorGUILayout.EndHorizontal();
			for (int cnt = 0; cnt < m_target.withAnchorList.Count; cnt++)
			{
				if (m_target.withAnchorList.Count < cnt) break;
				m_target.withAnchorList[cnt] = (RectTransform)EditorGUILayout.ObjectField(m_target.withAnchorList[cnt], typeof(RectTransform), true);
			}
		}
		EditorGUILayout.EndVertical();

		//여기까지 검사해서 필드에 변화가 있으면
		if (EditorGUI.EndChangeCheck())
		{
			Undo.RecordObjects(targets, "Changed Update Mode");
			//변경이 있을 시 적용된다. 이 코드가 없으면 인스펙터 창에서 변화는 있지만 적용은 되지 않는다.
			EditorUtility.SetDirty(m_target);
		}
		serializedObject.ApplyModifiedProperties();
		serializedObject.Update();
	}
	private List<T> ListController<T>(List<T> list, bool isRight = true)
	{
		if (list == null)
			list = new List<T>();

		if (isRight)
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.Space();
		}
		else
			EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(120));
		{
			EditorGUILayout.LabelField($"Total[{list.Count}]", GUILayout.MaxWidth(60));


			if (GUILayout.Button("R", GUILayout.Width(20)))
			{
				if (list.Count != 0)
				{
					if (EditorUtility.DisplayDialog("경고", "초기화 할거임?\n복구 못해", "응", "아니"))
						list = new List<T>();
				}
				else
					EditorUtility.DisplayDialog("이런", "초기화 할게 없져", "응");
			}
			if (GUILayout.Button("+", GUILayout.Width(20)))
			{
				list.Add(default);
			}
			if (GUILayout.Button("-", GUILayout.Width(20)))
			{
				if (EditorUtility.DisplayDialog("경고", "갯수 줄일거임?\n줄이면 값 넣은거 사라져", "응", "아니"))
				{
					if (list.Count != 0)
						list.RemoveAt(list.Count - 1);
				}
			}
		}
		EditorGUILayout.EndHorizontal();

		return list;
	}
}
#endif