namespace XD.Events
{
    /// <summary> Интерфейс фабрика для создания объектов <see cref="ITwoEventAdapter{TFirstDelegate,TSecondDeleagate}"/> </summary>
    public interface ITwoEventAdapterFactory
    {
        /// <summary> Получить адаптер для двух событий перемещения </summary>
        ITwoEventAdapter<MovementEndedEvent, MovementEndedEvent> GetTwoMoveToSwipeAdapter();
    }
}