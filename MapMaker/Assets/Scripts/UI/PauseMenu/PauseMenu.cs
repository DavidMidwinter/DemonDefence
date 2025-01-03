using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{

    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;
    [SerializeField] private UIDocument _document;
    [SerializeField] private StyleSheet _styleSheet;


    VisualElement root => _document.rootVisualElement;
    VisualElement display => root.Q(className: "main");

    VisualElement pauseMenu;

    VisualElement optionsMenu;

    public void Awake()
    {
        optionsMenu = OptionsMenu.getOptionsMenu(backButton());
        createUI();
    }

    void createUI()
    {
        root.Clear();
        root.styleSheets.Add(_styleSheet);
        root.Add(UITools.Create("container", "main", "menu-container", "pause-menu-container", "page-display"));


        pauseMenu = UITools.Create();
        Label header = UITools.Create<Label>("header-text");
        header.text = "Paused";
        pauseMenu.Add(header);

        Button mainMenuButton = UITools.Create("Main Menu", exitToMenu, "main-menu-button", "exit-main-menu");

        Button optionsMenuButton = UITools.Create("Options", loadOptionsMenu, "main-menu-button", "options-menu");
        Button quitGameButton = UITools.Create("Quit to Desktop", Utils.exit, "main-menu-button", "exit-desktop");
        Button resumeGameButton = UITools.Create("Resume", Resume, "main-menu-button", "resume");

        pauseMenu.Add(resumeGameButton);
        pauseMenu.Add(optionsMenuButton);
        pauseMenu.Add(mainMenuButton);
        pauseMenu.Add(quitGameButton);


        display.style.display = DisplayStyle.None;
        GameIsPaused = false;

    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }


    }
    public void Resume()
    {
        display.style.display = DisplayStyle.None;
        PaintUI.Instance.showUI();
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void Pause()
    {
        PaintUI.Instance.hideUI();
        display.style.display = DisplayStyle.Flex;
        Time.timeScale = 0f;
        GameIsPaused = true;
        load(pauseMenu);

    }

    void exitToMenu()
    {
        Time.timeScale = 1f;
        Utils.returnToMainMenu();

    }

    public Button backButton()
    {
        Button backButton = UITools.Create("Return", back, "back-button");
        return backButton;
    }

    void back() {
        load(pauseMenu);
    }

    void loadOptionsMenu()
    {
        load(optionsMenu);
    }

    void load(VisualElement page)
    {
        Debug.Log($"Load {page}");
        display.Clear();
        display.Add(page);
    }
}