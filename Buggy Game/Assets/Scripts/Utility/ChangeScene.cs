using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public void GoToScene(string scene) => SceneManager.LoadScene(scene);
}
