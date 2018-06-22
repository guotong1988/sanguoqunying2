//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UIPlayAnimation))]
public class UIPlayAnimationEditor : Editor
{
	enum ResetOnPlay
	{
		Continue,
		StartFromBeginning,
	}

	enum SelectedObject
	{
		KeepCurrent,
		SetToNothing,
	}

	public override void OnInspectorGUI ()
	{
		NGUIEditorTools.SetLabelWidth(120f);
		UIPlayAnimation pa = target as UIPlayAnimation;
		GUILayout.Space(6f);

		GUI.changed = false;
		Animation anim = (Animation)EditorGUILayout.ObjectField("Target", pa.target, typeof(Animation), true);

		string clipName = EditorGUILayout.TextField("Clip Name", pa.clipName);
		AnimationOrTween.Trigger trigger = (AnimationOrTween.Trigger)EditorGUILayout.EnumPopup("Trigger condition", pa.trigger);
		AnimationOrTween.Direction dir = (AnimationOrTween.Direction)EditorGUILayout.EnumPopup("Play direction", pa.playDirection);
		SelectedObject so = pa.clearSelection ? SelectedObject.SetToNothing : SelectedObject.KeepCurrent;
		bool clear = (SelectedObject)EditorGUILayout.EnumPopup("Selected object", so) == SelectedObject.SetToNothing;
		AnimationOrTween.EnableCondition enab = (AnimationOrTween.EnableCondition)EditorGUILayout.EnumPopup("If disabled on start", pa.ifDisabledOnPlay);
		ResetOnPlay rs = pa.resetOnPlay ? ResetOnPlay.StartFromBeginning : ResetOnPlay.Continue;
		bool reset = (ResetOnPlay)EditorGUILayout.EnumPopup("If already playing", rs) == ResetOnPlay.StartFromBeginning;
		AnimationOrTween.DisableCondition dis = (AnimationOrTween.DisableCondition)EditorGUILayout.EnumPopup("When finished", pa.disableWhenFinished);

		if (GUI.changed)
		{
			NGUIEditorTools.RegisterUndo("PlayAnimation Change", pa);
			pa.target = anim;
			pa.clipName = clipName;
			pa.trigger = trigger;
			pa.playDirection = dir;
			pa.clearSelection = clear;
			pa.ifDisabledOnPlay = enab;
			pa.resetOnPlay = reset;
			pa.disableWhenFinished = dis;
			UnityEditor.EditorUtility.SetDirty(pa);
		}

		NGUIEditorTools.SetLabelWidth(80f);
		NGUIEditorTools.DrawEvents("On Finished", pa, pa.onFinished);
	}
}
