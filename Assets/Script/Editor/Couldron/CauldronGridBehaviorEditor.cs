using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CauldronGridBehavior))]
public class CauldronGridBehaviorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        CauldronGridBehavior grid = (CauldronGridBehavior)target;

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Cauldron Grid Preview", EditorStyles.boldLabel);

        if (grid.width == 0 || grid.height == 0 || grid.debugGrid == null || grid.debugGrid.Count == 0)
        {
            EditorGUILayout.HelpBox("Grid belum diinisialisasi atau kosong.", MessageType.Info);
            return;
        }

        GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
        boxStyle.alignment = TextAnchor.MiddleCenter;
        boxStyle.fixedWidth = 25;
        boxStyle.margin = new RectOffset(1, 1, 1, 1);

        for (int y = grid.height - 1; y >= 0; y--) // tampil dari atas ke bawah
        {
            EditorGUILayout.BeginHorizontal();

            for (int x = 0; x < grid.width; x++)
            {
                int index = y * grid.width + x;
                int value = grid.debugGrid[index];
                string label = value.ToString();

                GUI.color = value == -1 ? Color.red : value == 1 ? Color.green : Color.white;

                GUILayout.Box(label, boxStyle);

                GUI.color = Color.white;
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}
