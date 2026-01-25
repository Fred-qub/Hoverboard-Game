using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerScript : MonoBehaviour
{
    //this all controls the main menu UI and uses it to switch scenes
    public GameObject titlePanel;
    public GameObject controlGuidePanel;
    public GameObject scoreBoardPanel;
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void backToTitle()
    {
        titlePanel.SetActive(true);
        controlGuidePanel.SetActive(false);
        scoreBoardPanel.SetActive(false);
    }

    public void openControlGuide()
    {
        titlePanel.SetActive(false);
        controlGuidePanel.SetActive(true);
        scoreBoardPanel.SetActive(false);
    }

    public void openScoreboard()
    {
        titlePanel.SetActive(false);
        controlGuidePanel.SetActive(false);
        scoreBoardPanel.SetActive(true);
    }
}
