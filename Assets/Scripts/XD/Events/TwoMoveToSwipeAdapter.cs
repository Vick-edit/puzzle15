using System;
using System.Threading;

namespace XD.Events
{
    /// <summary> Класс, позволяет РАЗОВО обернуть исполнение двух событий <see cref="MovementEndedEvent"/> в вызов другого события </summary>
    public class TwoMoveToSwipeAdapter : ITwoEventAdapter<MovementEndedEvent, MovementEndedEvent>
    {
        /// <summary> Семафор для потокобезопасного управления флагами и подписками объекта </summary>
        private readonly ReaderWriterLockSlim           stateLock;
        private bool                                    isFirstEventFinished;
        private bool                                    isSecondEventFinished;

        /// <summary> Экшен, который нужно вызвать по завершении обоих эвентов </summary>
        private Action                                  resultEvent;
        /// <summary> Экшен, который нужно вызвать, чтобы отписаться от первого эвента </summary>
        private Action                                  unsubscribeFromFirstEvent;
        /// <summary> Экшен, который нужно вызвать, чтобы отписаться от второго эвента  </summary>
        private Action                                  unsubscribeFromSecondEvent;


        public TwoMoveToSwipeAdapter()
        {
            stateLock = new ReaderWriterLockSlim();
            isFirstEventFinished = false;
            isSecondEventFinished = false;
        }


        #region ITwoEventAdapter

        /// <summary> Отслеживать завершение первого события </summary>
        /// <param name="firsEventSubscriber">Экшен в который обёрнуто подписывание на событие первого эвента</param>
        /// <param name="firsEventUnsubscriber">Экшен для отписывания от события</param>
        /// <returns>Ссылка на объект класса - адаптера, для Fluent настройки</returns>
        public ITwoEventAdapter<MovementEndedEvent, MovementEndedEvent> BindFirst(Action<MovementEndedEvent> firsEventSubscriber, Action<MovementEndedEvent> firsEventUnsubscriber)
        {
            if (firsEventSubscriber == null)
            {
                throw new ArgumentException("firsEventSubscriber - обязательный параметр!");
            }
            if (firsEventUnsubscriber == null)
            {
                throw new ArgumentException("firsEventUnsubscriber - обязательный параметр!");
            }

            stateLock.EnterWriteLock();
            try
            {
                //Если раньше были подписаны на какой-то эвент
                if (unsubscribeFromFirstEvent != null)
                {
                    //Сначала отпишемся от него
                    unsubscribeFromFirstEvent.Invoke();
                }

                isFirstEventFinished = false;
                Action riseFinishedFlag = () => isFirstEventFinished = true;                            //Экшен, который поднимает флаг окончания первого события
                MovementEndedEvent onFirstMovementFinished = () => EventFinished(riseFinishedFlag);     //Делегат для подпики на событие

                firsEventSubscriber.Invoke(onFirstMovementFinished);                                    //Подписка делегата на событие
                unsubscribeFromFirstEvent = () => firsEventUnsubscriber.Invoke(onFirstMovementFinished);//Экшен, позволяющий отписать делегат от события

                return this;
            }
            finally
            {
                stateLock.ExitWriteLock();
            }
        }

        /// <summary> Отслеживать завершение второго события </summary>
        /// <param name="secondEventSubscriber">Экшен в который обёрнуто подписывание на событие второго эвента</param>
        /// <param name="secondEventUnsubscriber">Экшен для отписывания от события</param>
        /// <returns>Ссылка на объект класса - адаптера, для Fluent настройки</returns>
        public ITwoEventAdapter<MovementEndedEvent, MovementEndedEvent> BindSecond(Action<MovementEndedEvent> secondEventSubscriber, Action<MovementEndedEvent> secondEventUnsubscriber)
        {
            if (secondEventSubscriber == null)
            {
                throw new ArgumentException("firsEventSubscriber - обязательный параметр!");
            }
            if (secondEventUnsubscriber == null)
            {
                throw new ArgumentException("firsEventUnsubscriber - обязательный параметр!");
            }

            stateLock.EnterWriteLock();
            try
            {
                //Если раньше были подписаны на какой-то эвент
                if (unsubscribeFromSecondEvent != null)
                {
                    //Сначала отпишемся от него
                    unsubscribeFromSecondEvent.Invoke();
                }

                isSecondEventFinished = false;
                Action riseFinishedFlag = () => isSecondEventFinished = true;                               //Экшен, который поднимает флаг окончания второго события
                MovementEndedEvent onSecondMovementFinished = () => EventFinished(riseFinishedFlag);        //Делегат для подпики на событие

                secondEventSubscriber.Invoke(onSecondMovementFinished);                                     //Подписка делегата на событие
                unsubscribeFromSecondEvent = () => secondEventUnsubscriber.Invoke(onSecondMovementFinished); //Экшен, позволяющий отписать делегат от события

                return this;
            }
            finally
            {
                stateLock.ExitWriteLock();
            }
        }

        /// <summary> Прекратить отслеживание всех событий, вернуть объект в исходное состояние </summary>
        /// <returns>Ссылка на объект класса - адаптера, для Fluent настройки</returns>
        public ITwoEventAdapter<MovementEndedEvent, MovementEndedEvent> UnbindAll()
        {
            stateLock.EnterWriteLock();
            try
            {
                isFirstEventFinished = isSecondEventFinished = false;

                if (unsubscribeFromFirstEvent != null)
                {
                    unsubscribeFromFirstEvent.Invoke();
                }
                if (unsubscribeFromSecondEvent != null)
                {
                    unsubscribeFromSecondEvent.Invoke();
                }

                resultEvent = unsubscribeFromFirstEvent = unsubscribeFromSecondEvent = null;

                return this;
            }
            finally 
            {
                stateLock.ExitWriteLock();
            }
        }

        /// <summary> Привязать вызов события к завершению обоих эвентов </summary>
        /// <param name="finalAction">Экшен, который будет вызван только по завершении обоих событий</param>
        public void BindFinal(Action finalAction)
        {
            //Если не заданы подписки (считай и отписки) хотя бы на одно событие
            if (unsubscribeFromFirstEvent == null || unsubscribeFromSecondEvent == null)
            {
                //То использование класса пошло не задуманному сценарию - бросаем эксепшен
                throw new NotSupportedException("Для корректной работы класса необходимо сперва привязать два исходных события");
            }

            resultEvent = finalAction;
        }
        #endregion

        /// <summary> Функция, отслеживающая завершение событий </summary>
        /// <param name="writeEventFlag">Экшен, который изменяет флаг, соответствующий эвенту, вызвавшему эту функцию</param>
        private void EventFinished(Action writeEventFlag)
        {
            //потокобезопасно поднимаем флаг завершившегося события
            stateLock.EnterWriteLock();
            writeEventFlag.Invoke();
            stateLock.ExitWriteLock();

            //заходим в потокобезопасное чтение состояния объекта
            stateLock.EnterUpgradeableReadLock();
            try
            {
                //если оба флага подняты (оба события завершились)
                if (isFirstEventFinished && isSecondEventFinished)
                {
                    //то вызываем результирующий экшен, в который должно быть обёрнуто финальное событие
                    if (resultEvent != null)
                    {
                        resultEvent.Invoke();
                    }
                    //отписываемся от всех событий (подписка разовая)
                    UnbindAll();
                }
            }
            finally
            {
                stateLock.ExitUpgradeableReadLock();
            }
        }
    }
}