using UnityEngine;

namespace XD.Movement
{
    /// <summary> Класс - интерфейся для управления звуком перемещения </summary>
    public abstract class AbstractMoveSoundManager : MonoBehaviour
    {
        /// <summary> Звук перемещения пазлов, задаваться в Unity! </summary>
        public AudioSource MoveSound;

        /// <summary> Воспроизвести звук передвижения </summary>
        /// <param name="totalDuration">Общая длительность воспроизведения</param>
        /// <param name="fadeInDuration">Время наростания громкости</param>
        /// <param name="fadeOutDuration">Время затухания громкости</param>
        public abstract void PlaySound(float totalDuration, float fadeInDuration = 0f, float fadeOutDuration = 0f);
    }
}