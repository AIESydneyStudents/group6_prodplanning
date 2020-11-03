using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DialougeSequence))]
public class DialougeSequenceEditor : Editor
{
    private DialougeSequence sequence = null;

    private void OnEnable()
    {
        sequence = (DialougeSequence)target;
    }

    public override void OnInspectorGUI()
    {
        if (sequence.textStrings != null) // Make sure the thing exsists
        {
            for (int i = 0; i < sequence.textStrings.Count; i++)
            {
                DialougeLine line = sequence.textStrings[i];

                if (!DrawDialougeLine(ref line)) // Remove from list
                {
                    sequence.textStrings.RemoveAt(i);
                    i--;
                    continue;
                }

                sequence.textStrings[i] = line;
            }
            EditorUtility.SetDirty(target);
        }

        if (GUILayout.Button("Add New"))
        {
            if (sequence.textStrings == null) // Make sure the thing exsists
            {
                sequence.textStrings = new List<DialougeLine>();
            }

            sequence.textStrings.Add(new DialougeLine());
            EditorUtility.SetDirty(target);
        }
    }

    public bool DrawDialougeLine(ref DialougeLine line)
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        {

            EditorGUILayout.BeginHorizontal();
            {
                //Speaker
                line.SpokenLine = EditorGUILayout.ObjectField("", line.SpokenLine, typeof(AudioClip), true) as AudioClip;
                line.EndWaitTime = EditorGUILayout.FloatField(line.EndWaitTime);

                //Remove line
                if (GUILayout.Button("Remove Line"))
                {
                    return false;
                }

                GUILayout.FlexibleSpace();

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.BeginHorizontal(); // Line
            {

                EditorStyles.textField.wordWrap = true;
                line.DialougeText = EditorGUILayout.TextArea(line.DialougeText, GUILayout.MaxHeight(64));

                //GUILayout.FlexibleSpace();

                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }

        return true;

    }
}
