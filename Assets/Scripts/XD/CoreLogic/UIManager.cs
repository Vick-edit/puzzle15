using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

namespace XD.CoreLogic
{
    /// <summary> Класс управляет логикой поведения UI </summary>
    public class UIManager : MonoBehaviour
    {
        /// <summary> Счетчик шагов пользователя </summary>
        public Text                 StepCounter;
        /// <summary> Элемент на панели, для отображения финального результата </summary>
        public Text                 StepResults;
        /// <summary> Счетчик игрового времени </summary>
        public Text                 TimeCounter;
        /// <summary> Элемент на панели, для отображения финального времени </summary>
        public Text                 TimeResult;
        /// <summary> Панель с финальными результатами </summary>
        public GameObject           FinalPanel;
        /// <summary> Счетчик шагов </summary>
        public int                  StepsCount { get; set; }
        /// <summary> Счетчик времени </summary>
        private Stopwatch           timer;
        /// <summary> Инициализирвоан ли менеджер </summary>
        private bool                isSetuped;
        /// <summary> Завершена ли игра </summary>
        private bool                isWin;

        /// <summary> Начать отсчет времени </summary>
        public void StartUIControl()
        {
            isSetuped = true;
            timer = new Stopwatch();
            timer.Start();
        }

        public void Update()
        {
            if (isSetuped && !isWin)
            {
                var time = timer.Elapsed;
                StepCounter.text = string.Format("Количество ходов: {0}", StepsCount);
                TimeCounter.text = string.Format("Время: {0:D2}:{1:D2}:{2:D2}", time.Minutes, time.Seconds, time.Milliseconds / 10);
            }
        }

        /// <summary> Объявить побед </summary>
        public void SetWin()
        {
            isWin = true;
            timer.Stop();

            StepCounter.enabled = TimeCounter.enabled = false;

            var time = timer.Elapsed;
            StepResults.text = string.Format("Количество ходов: {0}", StepsCount);
            TimeResult.text = string.Format("Время игры: {0:D2}:{1:D2}:{2:D2}", time.Minutes, time.Seconds, time.Milliseconds / 10);

            FinalPanel.SetActive(true);
        }
    }
}