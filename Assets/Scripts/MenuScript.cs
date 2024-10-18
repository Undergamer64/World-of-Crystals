using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    [SerializeField]
    private GameObject MainMenu;

    [SerializeField]
    private GameObject CreditsMenu;

    [SerializeField]
    private GameObject LevelSelectorMenu;

    public void CloseMenus()
    {
        MainMenu.SetActive(false);
        CreditsMenu.SetActive(false);
        LevelSelectorMenu.SetActive(false);
    }

    public void OpenMainMenu()
    {
        CloseMenus();
        MainMenu.SetActive(true);
    }

    public void OpenCreditsMenu()
    {
        CloseMenus();
        CreditsMenu.SetActive(true);
    }

    public void OpenLevelSelectorMenu()
    {
        CloseMenus();
        LevelSelectorMenu.SetActive(true);
    }

    public void SelectLevel(int level)
    {
        if (level > 0 && level <= 3)
        {
            SceneManager.LoadScene(level);
        }
    }

    public void Quit()
    {
        Application.Quit();
    }

}
