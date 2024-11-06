using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesMove : MonoBehaviour
{
    // 각 씬에 대한 로드 함수를 배열로 선언
    private System.Action[] loadSceneActions;

    private void Start()
    {
        // 배열에 씬 로딩 함수를 할당
        loadSceneActions = new System.Action[]
        {
            LoadMainMenuScene,
            LoadGameScene
        };

        // 현재 씬의 빌드 인덱스를 가져옴
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // 배열에서 현재 씬 인덱스에 맞는 씬 전환 함수 호출
        if (currentSceneIndex < loadSceneActions.Length)
        {
            loadSceneActions[currentSceneIndex]();
        }
    }

    // 각 씬에 대한 로딩 함수들
    private void LoadMainMenuScene()
    {
        SceneManager.LoadScene("SeoHyun");
    }

    private void LoadGameScene()
    {
        SceneManager.LoadScene("Main");
    }
}
