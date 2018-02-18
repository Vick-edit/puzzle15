using UnityEngine;
using XD.Events;
using XD.Movement;

namespace XD.CoreLogic
{
    /// <summary> Класс - интерфейс логики поведения кусочка пазла </summary>
    public abstract class AbstractPuzzleTileFacade : MonoBehaviour
    {
        /// <summary> Флаг, указывающий является ли кусочек пазла пустым </summary>
        public bool IsEpmty;

        /// <summary> Класс, позволяющий управлять положением данного пазла, должен использоваться только классами пазла и задаваться в Unity! </summary>
        public AbstractMovable MoveController;

        /// <summary> Класс, обрабатывающий нажатия на элемент и порождающий соответствующие события, должен задаваться в Unity! </summary>
        public AbstractPazzleTileClickСontroller ClickСontroller;

        /// <summary> Событие срабатывает единожды, когда завершается следующий обмен мест ттекущего пазла с пустым </summary>
        public abstract event SwapEndedEvent OnSwapEndedEvent;

        /// <summary> Координаты первоначального расположения пазла </summary>
        public PuzzleVirtualCoordinates StartCoordinates { get; set; }

        /// <summary> Текущие координаты  пазла </summary>
        public PuzzleVirtualCoordinates CurrentCoordinates { get; set; }

        /// <summary> Обменяться местами с пустым пазлом </summary>
        /// <param name="otherPuzzleTile">Пустой пазл, с которым необходимо обменяться местами</param>
        public abstract void SwapWithEmptyTile(AbstractPuzzleTileFacade otherPuzzleTile);
    }
}
