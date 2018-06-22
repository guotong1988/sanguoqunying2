//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UITweener))]
public class UITweenerEditor : Editor
{
	public override void OnInspectorGUI ()
	{
		GUILayout.Space(6f);
		NGUIEditorTools.SetLabelWidth(120f);
		base.OnInspectorGUI();
		DrawCommonProperties();
	}

	protected void DrawCommonProperties ()
	{
		UITweener tw = target as UITweener;

		if (NGUIEditorTools.DrawHeader("Tweener"))
		{
			NGUIEditorTools.BeginContents();

			GUI.changed = false;
			AnimationCurve curve = EditorGUILayout.CurveField("Animation Curve", tw.animationCurve);
			UITweener.Style style = (UITweener.Style)EditorGUILayout.EnumPopup("Play Style", tw.style);
			//UITweener.Method method = (UITweener.Method)EditorGUILayout.EnumPopup("Play Method", tw.method);

			GUILayout.BeginHorizontal();
			float dur = EditorGUILayout.FloatField("Duration", tw.duration, GUILayout.Width(160f));
			GUILayout.Label("seconds");
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			float del = EditorGUILayout.FloatField("Start Delay", tw.delay, GUILayout.Width(160f));
			GUILayout.Label("seconds");
			GUILayout.EndHorizontal();

			int tg = EditorGUILayout.IntField("Tween Group", tw.tweenGroup, GUILayout.Width(160f));
			bool ts = EditorGUILayout.Toggle("Ignore TimeScale", tw.ignoreTimeScale);

			if (GUI.changed)
			{
				NGUIEditorTools.RegisterUndo("Tween Change", tw);
				tw.animationCurve = curve;
				//tw.method = method;
				tw.style = style;
				tw.ignoreTimeScale = ts;
				tw.tweenGroup = tg;
				tw.duration = dur;
				tw.delay = del;
				UnityEditor.EditorUtility.SetDirty(tw);
			}
			NGUIEditorTools.EndContents();
		}

		NGUIEditorTools.SetLabelWidth(80f);
		NGUIEditorTools.DrawEvents("On Finished", tw, tw.onFinished);
	}
}
