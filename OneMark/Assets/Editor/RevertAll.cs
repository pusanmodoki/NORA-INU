using UnityEditor;
using UnityEditor.SceneManagement;

public class RevertAll
{
	[MenuItem("Toools/Revert Selection Prefab %&Z")]
	private static void RevertSelectionPrefab()
	{
		var gameObjects = Selection.gameObjects;

		if (gameObjects == null || gameObjects.Length <= 0) return;

		foreach (var e in gameObjects)
			PrefabUtility.RevertPrefabInstance(e);

		EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
	}
}
