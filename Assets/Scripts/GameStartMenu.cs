using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameStartMenu : MonoBehaviour
{
    [Header("UI Pages")]
    public GameObject mainMenu;
    public GameObject options;
    public GameObject about;

    [Header("Main Menu Buttons")]
    //public Button startButton;
    public Button optionButton;
    public Button aboutButton;
    public Button quitButton;

    public List<Button> returnButtons;

    // Start is called before the first frame update
    void Start()
    {
        EnableMainMenu();

        //Hook events
        optionButton.onClick.AddListener(EnableOption);
        aboutButton.onClick.AddListener(EnableCreadit);
        quitButton.onClick.AddListener(QuitGame);

        foreach (var item in returnButtons)
        {
            item.onClick.AddListener(EnableMainMenu);
        }
    }

    public void QuitGame()
    {
        SoundManager.instance.PlaySE("메뉴 클릭");
        #if UNITY_EDITOR
        EditorApplication.isPlaying = false; // This will stop the play mode in the editor
        #else
        Application.Quit(); // This will quit the application
        #endif
    }

    public void StartGame(int index)
    {
        SoundManager.instance.PlaySE("메뉴 클릭");
        HideAll();
        SceneTransitionManager.singleton.GoToSceneAsync(index);
    }

    public void HideAll()
    {
        mainMenu.SetActive(false);
        options.SetActive(false);
        about.SetActive(false);
    }

    public void EnableMainMenu()
    {
        SoundManager.instance.PlaySE("메뉴 클릭");
        SoundManager.instance.PlaySE("메뉴 클로즈");
        mainMenu.SetActive(true);
        options.SetActive(false);
        about.SetActive(false);
    }
    public void EnableOption()
    {
        SoundManager.instance.PlaySE("메뉴 클릭");
        SoundManager.instance.PlaySE("메뉴 오픈");
        mainMenu.SetActive(false);
        options.SetActive(true);
        about.SetActive(false);
    }
    public void EnableCreadit()
    {
        SoundManager.instance.PlaySE("메뉴 클릭");
        SoundManager.instance.PlaySE("메뉴 오픈");
        mainMenu.SetActive(false);
        options.SetActive(false);
        about.SetActive(true);
    }
}
