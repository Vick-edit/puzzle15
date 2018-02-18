using System;
using JetBrains.Annotations;

namespace XD.Events
{
    /// <summary>
    /// Интерфейс преобразования двух событий в одно финальное
    /// </summary>
    /// <typeparam name="TFirstDelegate">Тип делегата первого события</typeparam>
    /// <typeparam name="TSecondDeleagate">Тип делегата второго события</typeparam>
    /// <typeparam name="TFinalDeleagate">Тип делегата финального события</typeparam>
    public interface ITwoEventAdapter<TFirstDelegate, TSecondDeleagate>
        where TFirstDelegate : class 
        where TSecondDeleagate : class
    {
        /// <summary> Отслеживать завершение первого события </summary>
        /// <param name="firsEventSubscriber">Экшен в который обёрнуто подписывание на событие первого эвента</param>
        /// <param name="firsEventUnsubscriber">Экшен для отписывания от события</param>
        /// <returns>Ссылка на объект класса - адаптера, для Fluent настройки</returns>
        ITwoEventAdapter<TFirstDelegate, TSecondDeleagate> BindFirst(
            [NotNull] Action<TFirstDelegate> firsEventSubscriber, 
            [NotNull] Action<TFirstDelegate> firsEventUnsubscriber);

        /// <summary> Отслеживать завершение второго события </summary>
        /// <param name="secondEventSubscriber">Экшен в который обёрнуто подписывание на событие второго эвента</param>
        /// <param name="secondEventUnsubscriber">Экшен для отписывания от события</param>
        /// <returns>Ссылка на объект класса - адаптера, для Fluent настройки</returns>
        ITwoEventAdapter<TFirstDelegate, TSecondDeleagate> BindSecond(
            [NotNull] Action<TSecondDeleagate> secondEventSubscriber,
            [NotNull] Action<TFirstDelegate> secondEventUnsubscriber);

        /// <summary> Прекратить отслеживание всех событий, вернуть объект в исходное состояние </summary>
        /// <returns>Ссылка на объект класса - адаптера, для Fluent настройки</returns>
        ITwoEventAdapter<TFirstDelegate, TSecondDeleagate> UnbindAll();

        /// <summary> Привязать вызов события к завершению обоих эвентов </summary>
        /// <param name="finalAction">Экшен, который будет вызван только по завершении обоих событий</param>
        void BindFinal([NotNull] Action finalAction);
    }
}