using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace XD.Movement
{
    /// <summary> Класс - обертка вокруг источника звука, для управления им во время перемещения пазла </summary>
    public class MoveSoundManager : AbstractMoveSoundManager
    {
        /// <summary> Уровень звука файла по умолчанию, -1 - значит не инициализированно </summary>
        private float defaultSondLevel = -1f;

        /// <summary> Воспроизвести звук передвижения </summary>
        /// <param name="totalDuration">Общая длительность воспроизведения</param>
        /// <param name="fadeInDuration">Время наростания громкости</param>
        /// <param name="fadeOutDuration">Время затухания громкости</param>
        public override void PlaySound(float totalDuration, float fadeInDuration = 0, float fadeOutDuration = 0)
        {
            if (defaultSondLevel < 0)
            {
                defaultSondLevel = MoveSound.volume;
            }

            this.StopAllCoroutines();
            StartCoroutine(PlayMovementSound(totalDuration, fadeInDuration, fadeOutDuration));
        }

        /// <summary> Непосредственный алгоритм управления звуком </summary>
        /// <param name="totalDuration">Общая длительность воспроизведения</param>
        /// <param name="fadeInDuration">Время наростания громкости</param>
        /// <param name="fadeOutDuration">Время затухания громкости</param>
        /// <returns>WaitForEndOfFrame, для покадровой отрисовки</returns>
        private IEnumerator PlayMovementSound(float totalDuration, float fadeInDuration = 0, float fadeOutDuration = 0)
        {
            //Отключаем звук и начинаем его вопроизведение
            this.MoveSound.volume = 0;
            this.MoveSound.pitch = Random.Range(0.5f, 1f);
            this.MoveSound.Play();
            try
            {
                bool isOnDefaultVolume = false; //воспроизводится ли звук сейчас с нормальной громкостью?

                //В зависимости от прошедшего времени управляем громкостью звука
                float durationOfCoroutine = 0;
                while (totalDuration - durationOfCoroutine > 0.001f)
                {
                    yield return new WaitForEndOfFrame();
                    durationOfCoroutine += Time.deltaTime;

                    //Если прошедшее время меньше времени нарастания звука
                    if (durationOfCoroutine < fadeInDuration && fadeInDuration > 0)
                    {
                        //то набираем громкость
                        MoveSound.volume = defaultSondLevel*durationOfCoroutine/fadeInDuration;
                    }
                    //Если прошедшее время меньше общее время минус время затухания
                    else if (durationOfCoroutine < totalDuration - fadeOutDuration)
                    {
                        //То воспроизводим со стандартной громкостью
                        if (!isOnDefaultVolume)
                        {
                            MoveSound.volume = defaultSondLevel;
                            isOnDefaultVolume = true;
                        }
                    }
                    //Иначе
                    else if (fadeOutDuration > 0)
                    {
                        //Начинаем уменьшаем громкость
                        MoveSound.volume = defaultSondLevel*(1 - durationOfCoroutine/fadeOutDuration);
                    }
                }
            }
            finally
            {
                this.MoveSound.Stop();
                this.MoveSound.volume = defaultSondLevel;
                this.MoveSound.pitch = 1;
            }

            yield return null;
        }
    }
}