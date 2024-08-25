using System.Collections;
using UI.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuView : MonoBehaviour
{
    [SerializeField]
    UIDocument _document;

    public static MainMenuView Instance;

    void Awake() => Instance = this;

    void Start() => MainMenuData.Document = _document;

    public void Load() => StartCoroutine(LoadScene());

    IEnumerator LoadScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(1);

        while (!asyncLoad.isDone)
            yield return null;
    }
}
