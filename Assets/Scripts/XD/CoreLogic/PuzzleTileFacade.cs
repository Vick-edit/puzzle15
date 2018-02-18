using System;
using UnityEngine;
using XD.Events;
using XD.Movement;

namespace XD.CoreLogic
{
    /// <summary> Класс - фасад для работы с частичкой пазла </summary>
    public class PuzzleTileFacade : AbstractPuzzleTileFacade
    {
        /// <summary> Переменная - содержит фабрику для адапетров событий, должна использоваться только внутри свойства! </summary>
        private ITwoEventAdapterFactory twoEventAdapterForProperty;

        /// <summary> Фабрика для для адапетров событий </summary>
        private ITwoEventAdapterFactory TwoEventAdapter
        {
            get { return twoEventAdapterForProperty ?? (twoEventAdapterForProperty = new TwoEventAdapterFactory()); }
        }


        #region AbstractPuzzleTileFacade
        /// <summary> Событие срабатывает единожды, когда завершается следующий обмен мест ттекущего пазла с пустым </summary>
        public override event SwapEndedEvent OnSwapEndedEvent;

        /// <summary> Обменяться местами с пустым пазлом </summary>
        /// <param name="otherPuzzleTile">Пустой пазл, с которым необходимо обменяться местами</param>
        public override void SwapWithEmptyTile(AbstractPuzzleTileFacade otherPuzzleTile)
        {
            if (!otherPuzzleTile.IsEpmty)
            {
                throw new Exception("Невозможно обменяться местами с непустой ячейкой");
            }

            SetupEventHandler(otherPuzzleTile);

            Vector3 emptyPuzzleDestination = this.transform.position;
            Vector3 thisPuzzleDestination = otherPuzzleTile.transform.position;

            this.MoveController.MoveToPoint(thisPuzzleDestination);
            otherPuzzleTile.MoveController.MoveToPoint(emptyPuzzleDestination);
        } 
        #endregion


        /// <summary> Настроить обработку события обмена мест с пустым пазлом </summary>
        /// <param name="otherPuzzleTile">Пустой пазл, с которым меняемся местами</param>
        private void SetupEventHandler(AbstractPuzzleTileFacade otherPuzzleTile)
        {
            //Экшены - обертки для подписки и одписки на события перемещения пустой ячейки
            Action<MovementEndedEvent> subscribeForFirstMoveEnded = (sunscriber) => otherPuzzleTile.MoveController.OnNextMovementFinished += sunscriber;
            Action<MovementEndedEvent> unsubscribeFromFirstMoveEnded = (sunscriber) => otherPuzzleTile.MoveController.OnNextMovementFinished -= sunscriber;
            //Экшены - обертки для подписки и одписки на события перемещения текущей ячейки
            Action<MovementEndedEvent> subscribeForSecondMoveEnded = (sunscriber) => this.MoveController.OnNextMovementFinished += sunscriber;
            Action<MovementEndedEvent> unsubscribeFromSecondMoveEnded = (sunscriber) => this.MoveController.OnNextMovementFinished -= sunscriber;
            //Создание хэндлера этих двух перемещений
            TwoEventAdapter.GetTwoMoveToSwipeAdapter()
                .BindFirst(subscribeForFirstMoveEnded, unsubscribeFromFirstMoveEnded)
                .BindSecond(subscribeForSecondMoveEnded, unsubscribeFromSecondMoveEnded)
                .BindFinal(RaiseOnSwapEnded(otherPuzzleTile));
        }

        /// <summary> Вызвать евент об окончании обмена мест с пустым пазлом </summary>
        private Action RaiseOnSwapEnded(AbstractPuzzleTileFacade otherPuzzleTile)
        {
            Action newOnSwaped = () =>
            {
                //После завершения физического перемещения, необходимо обновить координаты
                PuzzleVirtualCoordinates emptyPuzzleCoordinates = otherPuzzleTile.CurrentCoordinates;
                otherPuzzleTile.CurrentCoordinates = this.CurrentCoordinates;
                this.CurrentCoordinates = emptyPuzzleCoordinates;

                //Вызываем соответствующее событие
                if (OnSwapEndedEvent != null)
                {
                    OnSwapEndedEvent.Invoke(this, otherPuzzleTile);
                    OnSwapEndedEvent = null;
                }
            };
            return newOnSwaped;
        }
    }
}