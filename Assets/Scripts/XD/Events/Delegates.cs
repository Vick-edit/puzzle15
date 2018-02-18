using XD.CoreLogic;
using XD.Movement;

namespace XD.Events
{
    /// <summary>
    /// Делегат события нажатия на игровой объект
    /// </summary>
    /// <typeparam name="T">Тип игрового объекта, который породил событие</typeparam>
    /// <param name="sender">Объект, по которому произошло нажатие</param>
    public delegate void ClicledEvent<T>(T sender) where T : class;

    /// <summary>
    /// Делегат события завершения перемещения объекта типа <see cref="AbstractMovable"/>
    /// </summary>
    public delegate void MovementEndedEvent();

    /// <summary>
    /// Делегат события завершения обмена мест <see cref="AbstractPuzzleTileFacade"/> c пустым <see cref="AbstractPuzzleTileFacade"/>
    /// </summary>
    /// <param name="puzzleTile">Не пустой <see cref="AbstractPuzzleTileFacade"/>, который участвовал в обмене</param>
    /// <param name="emptyPuzzleTile">Пустой пазл <see cref="AbstractPuzzleTileFacade"/>, который участвовал в обмене</param>
    public delegate void SwapEndedEvent(AbstractPuzzleTileFacade puzzleTile, AbstractPuzzleTileFacade emptyPuzzleTile);

    /// <summary> Делегат события нажатия по элементу </summary>
    /// <typeparam name="TClickedObjectType"> Тип объекта, по которому произошло нажатие </typeparam>
    /// <param name="clickedObject">Объект по коорому произошло нажатие</param>
    public delegate void ClickEvent<TClickedObjectType>(TClickedObjectType clickedObject) where TClickedObjectType : class;
}