using UnityEngine;

namespace XD.Movement
{
    /// <summary> Интерфейс для объектов, которые можно передвинуть из текущей позиции в точку Б </summary>
    public abstract class AbstractMovable : MonoBehaviour
    {
        /// <summary>
        /// Перемещение объекта до заданной позиции
        /// </summary>
        /// <param name="destenation">Конечное положение</param>
        public abstract void MoveToPoint(Vector3 destenation);
    }
}