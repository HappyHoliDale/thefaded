using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesMove : MonoBehaviour
{
    // �� ���� ���� �ε� �Լ��� �迭�� ����
    private System.Action[] loadSceneActions;

    private void Start()
    {
        // �迭�� �� �ε� �Լ��� �Ҵ�
        loadSceneActions = new System.Action[]
        {
            LoadMainMenuScene,
            LoadGameScene
        };

        // ���� ���� ���� �ε����� ������
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // �迭���� ���� �� �ε����� �´� �� ��ȯ �Լ� ȣ��
        if (currentSceneIndex < loadSceneActions.Length)
        {
            loadSceneActions[currentSceneIndex]();
        }
    }

    // �� ���� ���� �ε� �Լ���
    private void LoadMainMenuScene()
    {
        SceneManager.LoadScene("SeoHyun");
    }

    private void LoadGameScene()
    {
        SceneManager.LoadScene("Main");
    }
}
