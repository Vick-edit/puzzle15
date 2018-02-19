using UnityEngine;
using XD.Events;

namespace XD.Movement
{
    /// <summary> Интерфейс для объектов, которые можно передвинуть из текущей позиции в точку Б </summary>
    public abstract class AbstractMovable : MonoBehaviour
    {
        /// <summary> Опциональный элемент, позволяет воспроизводить звук перемещения элемента </summary>
        public AbstractMoveSoundManager MovmentSoundManager;

        /// <summary> Событие срабатывает единожды, когда завершается следующее перемещение, после этого все подписчики сбрасываются </summary>
        public abstract event MovementEndedEvent OnNextMovementFinished;

        /// <summary>
        /// Перемещение объекта до заданной позиции
        /// </summary>
        /// <param name="destenation">Конечное положение</param>
        public abstract void MoveToPoint(Vector3 destenation);
    }
}