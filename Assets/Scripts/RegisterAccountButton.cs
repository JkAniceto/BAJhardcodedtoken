using UnityEngine;
using UnityEngine.SceneManagement;

public class RegisterAccountButton : MonoBehaviour
{
    public void LoadWorkerRegistrationScene()
    {
        SceneManager.LoadScene("RegistrationPage");
    }
    //BOOKER TABLE
    public void ButtonOnlineWorker()
    {
        SceneManager.LoadScene("BookerTable");
    }
    //REGISTRATION PAGE
    public void BackButtonLogin()
    {
        SceneManager.LoadScene("LoginPage");
    }
    //BOOKER TABLE
    public void BackButtonBookerHomePage()
    {
        SceneManager.LoadScene("BookerHomePage");
    }
}
