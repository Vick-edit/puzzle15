using UnityEngine;
using UnityEngine.SceneManagement;

namespace XD.CoreLogic
{
    /// <summary> Класс логики кнопок </summary>
    public class ButtonsLogic : MonoBehaviour
    {
        /// <summary> Перезапустить игру </summary>
        public void RestartGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        /// <summary> Закрыть приложение </summary>
        public void ExitGame()
        {
            Application.Quit();
        }
    }
}