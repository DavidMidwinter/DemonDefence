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
    VisualElement ui => root.Q(className: "main");

    public void Awake()
    {
        createUI();
    }

    void createUI()
    {
        root.Clear();
        root.styleSheets.Add(_styleSheet);
        VisualElement ui = UITools.Create("container", "main", "page-display");
        Label header = UITools.Create<Label>("header-text");
        header.text = "Paused";
        ui.Add(header);

        Button mainMenuButton = UITools.Create("Main Menu", exitToMenu, "main-menu-button", "exit-main-menu");
        Button quitGameButton = UITools.Create("Quit to Desktop", Utils.exit, "main-menu-button", "exit-desktop");

        ui.Add(mainMenuButton);
        ui.Add(quitGameButton);


        ui.style.display = DisplayStyle.None;
        GameIsPaused = false;
        root.Add(ui);
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
        ui.style.display = DisplayStyle.None;
        TacticalUI.Instance.showUI();
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void Pause()
    {
        TacticalUI.Instance.hideUI();
        ui.style.display = DisplayStyle.Flex;
        Time.timeScale = 0f;
        GameIsPaused = true;

    }

    void exitToMenu()
    {
        Time.timeScale = 1f;
        Utils.returnToMainMenu();

    }
    
}
