namespace XD.Events
{
    /// <summary> Фабрика для создания объектов <see cref="ITwoEventAdapter{TFirstDelegate,TSecondDeleagate}"/> </summary>
    public class TwoEventAdapterFactory : ITwoEventAdapterFactory
    {
        /// <summary> Получить адаптер для двух событий перемещения </summary>
        public ITwoEventAdapter<MovementEndedEvent, MovementEndedEvent> GetTwoMoveToSwipeAdapter()
        {
            return new TwoMoveToSwipeAdapter();
        }
    }
}