using System;
using XD.CoreLogic;

namespace XD.Events
{
    /// <summary> Класс - обертка, позволяющий отслеживать нажатия на пазл </summary>
    public class PazzleTileClickСontroller : AbstractPazzleTileClickСontroller
    {
        /// <summary> От имени какого объекта будут порождаться события о нажатии </summary>
        public AbstractPuzzleTileFacade PuzzleTile;

        #region AbstractClickСontroller
        /// <summary> Событие, указывающее, что произошло нажатие по элементу </summary>
        public override event ClickEvent<AbstractPuzzleTileFacade> OnClicked; 
        #endregion

        public void OnMouseDown()
        {
            if (PuzzleTile == null)
            {
                throw new Exception("Необходимо обязательно задать объект AbstractPuzzleTileFacade");
            }

            if (OnClicked != null)
            {
                OnClicked.Invoke(PuzzleTile);
            }
        }
    }
}