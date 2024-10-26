using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using System.Threading.Tasks;

namespace OneHit.Utility
{
    public class EditorUtility
    {
        [MenuItem("OneHit/Utility/Save And Play From Loading")]
        public static async void SaveAndPlayFromLoading()
        {
            EditorSceneManager.SaveOpenScenes();
            await Task.Delay(200);
            EditorSceneManager.OpenScene("Assets\\Source\\Scenes\\Loading.unity"); 
            EditorApplication.isPlaying = true;
        }
        [MenuItem("OneHit/Utility/Next Scene %TAB")]
        public static void LoadNextScene()
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            int nextSceneIndex = (currentSceneIndex + 1) % SceneManager.sceneCountInBuildSettings;

            EditorSceneManager.SaveOpenScenes(); // Lưu các cảnh đang mở nếu cần thiết
            EditorSceneManager.OpenScene(SceneUtility.GetScenePathByBuildIndex(nextSceneIndex));
        }
    }
}