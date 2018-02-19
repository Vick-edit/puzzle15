using System.Collections;
using UnityEngine;
using XD.Events;

namespace XD.Movement
{
    /// <summary> Класс, реализующий передвижение из текущей точки в точку Б с ускорением и замедлением за отведённое время </summary>
    public class MoveObject : AbstractMovable
    {
        [Header("время за которое будет перемещён объект")]
        private const float     MomentDurationInSeconds = 0.25f;

        /// <summary> Находится ли сейчас объект в движении </summary>
        private bool            isInMovement = false;

        #region AbstractMovable

        /// <summary> Событие срабатывает единожды, когда завершается следующее перемещение, после этого все подписчики сбрасываются </summary>
        public override event MovementEndedEvent OnNextMovementFinished;

        /// <summary>
        /// Перемещение объекта до заданной позиции
        /// </summary>
        /// <param name="destenation">Конечное положение</param>
        public override void MoveToPoint(Vector3 destenation)
        {
            if (!isInMovement)
            {
                isInMovement = true;
                //начинаем воспроизведение звука перемещения
                if (MovmentSoundManager != null)
                {
                    MovmentSoundManager.PlaySound(MomentDurationInSeconds, MomentDurationInSeconds/3, MomentDurationInSeconds/3);
                }
                //начинаем само перемещение
                StartCoroutine(PerformMovement(destenation));
            }
        }

        #endregion

        /// <summary>
        /// Покадровое перемещение объекта согласно алгоритму в Coroutine
        /// </summary>
        /// <param name="destenation">Конечная точка перемещения</param>
        /// <returns>WaitForEndOfFrame, для покадровой отрисовки</returns>
        private IEnumerator PerformMovement(Vector3 destenation)
        {
            try
            {
                //Расчет основной скорости движения и модуля ускорения, для следующего типа движения
                //Ускорения от нуля за 1/3 времени движения, 1/3 времени - перемещения на постоянной скорости и сброс скорости до нуля на оставшейся 1/3
                float totalTimeInMove       = 0;
                float totalDistance         = Vector3.Distance(transform.position, destenation);
                float avarageSpeed          = 3*totalDistance/(2*MomentDurationInSeconds);
                float accelerationModule    = avarageSpeed/(MomentDurationInSeconds/3);

                //Пока время перемещения меньше заданного порогового значения
                while (MomentDurationInSeconds - totalTimeInMove > 0.001f)
                {
                    yield return new WaitForEndOfFrame();
                    totalTimeInMove += Time.deltaTime;
                    float curretSpeed;

                    //Если время перемещения меньше трети заданного времени перемещения
                    if (totalTimeInMove < MomentDurationInSeconds/3)
                    {
                        //то набираем скорость
                        curretSpeed = accelerationModule*totalTimeInMove;
                    }
                    //Если время перемещения меньше двух третей заданного времени перемещения
                    else if (totalTimeInMove < MomentDurationInSeconds*(2f/3f))
                    {
                        //То движемся равномерно с заданной скоростью
                        curretSpeed = avarageSpeed;
                    }
                    //Иначе
                    else
                    {
                        //Начинаем замедляться
                        curretSpeed = avarageSpeed - accelerationModule * (totalTimeInMove - MomentDurationInSeconds*(2f/3f));
                    }

                    transform.position = Vector3.MoveTowards(transform.position, destenation, curretSpeed * Time.deltaTime);
                }

                //Завершаем перемещение точной установкой координат
                transform.position = destenation;
            }
            finally
            {
                PutDownMovementFlag();
            }
            yield return null;
        }

        private void PutDownMovementFlag()
        {
            isInMovement = false;
            if (OnNextMovementFinished != null)
            {
                OnNextMovementFinished.Invoke();
                OnNextMovementFinished = null;
            }
        }
    }
}
