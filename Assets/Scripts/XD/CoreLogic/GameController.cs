using System;
using UnityEngine;

namespace XD.CoreLogic
{
    /// <summary> Основной класс механики игры </summary>
    public class GameController : MonoBehaviour
    {
        /// <summary> Константа - число пазлов на игровом поле, включая пустой </summary>
        private const int                       PUZZLES_COUNT = 16;
        /// <summary> Массив, всех используемых пазлов, нужен только для привязки элементов в Unity</summary>
        public AbstractPuzzleTileFacade[]       AllPuzzleTiles = new AbstractPuzzleTileFacade[PUZZLES_COUNT];
        /// <summary> Константа - размерность виртуального поля </summary>
        private const int                       VIRTUAL_FIELD_SIZE = 4;
        /// <summary> Виртуальное поле пазлов </summary>
        private AbstractPuzzleTileFacade[,]     VirtualFieldOfPuzzles;

        private bool isLocked;

        #region Unitty
        public void OnValidate()
        {
            //Запрещаем изменять размер массива пазлов
            if (AllPuzzleTiles.Length != PUZZLES_COUNT)
            {
                Debug.LogWarning(string.Format("Количество пазлов должно всегда соответствовать заданному числу - {0}", PUZZLES_COUNT));
                Array.Resize(ref AllPuzzleTiles, PUZZLES_COUNT);
            }
        }

        public void Start()
        {
            VirtualFieldOfPuzzles = new AbstractPuzzleTileFacade[VIRTUAL_FIELD_SIZE, VIRTUAL_FIELD_SIZE];
            for (int i = 0; i < VIRTUAL_FIELD_SIZE; i++)
            {
                for (int j = 0; j < VIRTUAL_FIELD_SIZE; j++)
                {
                    int rowNum = i;
                    int columnNum = j;
                    int flatIndex = rowNum * 4 + columnNum;
                    VirtualFieldOfPuzzles[i, j] = AllPuzzleTiles[flatIndex];
                    VirtualFieldOfPuzzles[i, j].StartCoordinates = new PuzzleVirtualCoordinates(rowNum, columnNum);
                    VirtualFieldOfPuzzles[i, j].CurrentCoordinates = new PuzzleVirtualCoordinates(rowNum, columnNum);
                    if (!VirtualFieldOfPuzzles[i, j].IsEpmty)
                    {
                        VirtualFieldOfPuzzles[i, j].ClickСontroller.OnClicked += PuzzleWasClicked;
                    }
                }
            }
        }
        #endregion

        private void PuzzleWasClicked(AbstractPuzzleTileFacade puzzleTile)
        {
            if (isLocked)
            {
                return;
            }

            isLocked = true;

            int rowNum = puzzleTile.CurrentCoordinates.Row;
            int columnNum = puzzleTile.CurrentCoordinates.Column;

            int topNeighborRow = rowNum -1;
            int bottomNeighborRow = rowNum + 1;
            int leftNeighborCol = columnNum - 1;
            int rightNeighborCol = columnNum + 1;

            AbstractPuzzleTileFacade emptyPuzzleTile = null;

            if (topNeighborRow >= 0 && VirtualFieldOfPuzzles[topNeighborRow, columnNum].IsEpmty)
            {
                emptyPuzzleTile = VirtualFieldOfPuzzles[topNeighborRow, columnNum];
            }
            else if (bottomNeighborRow < VIRTUAL_FIELD_SIZE && VirtualFieldOfPuzzles[bottomNeighborRow, columnNum].IsEpmty)
            {
                emptyPuzzleTile = VirtualFieldOfPuzzles[bottomNeighborRow, columnNum];
            }
            else if (leftNeighborCol >=0 && VirtualFieldOfPuzzles[rowNum, leftNeighborCol].IsEpmty)
            {
                emptyPuzzleTile = VirtualFieldOfPuzzles[rowNum, leftNeighborCol];
            }
            else if (rightNeighborCol < VIRTUAL_FIELD_SIZE && VirtualFieldOfPuzzles[rowNum, rightNeighborCol].IsEpmty)
            {
                emptyPuzzleTile = VirtualFieldOfPuzzles[rowNum, rightNeighborCol];
            }

            if (emptyPuzzleTile != null)
            {
                puzzleTile.SwapWithEmptyTile(emptyPuzzleTile);
                puzzleTile.OnSwapEndedEvent += (notEmptyPuzzle, emptyPuzzle) => 
                {
                    VirtualFieldOfPuzzles[notEmptyPuzzle.CurrentCoordinates.Row, notEmptyPuzzle.CurrentCoordinates.Column] = notEmptyPuzzle;
                    VirtualFieldOfPuzzles[emptyPuzzle.CurrentCoordinates.Row, emptyPuzzle.CurrentCoordinates.Column] = emptyPuzzle;
                    isLocked = false;
                };
            }
            else
            {
                isLocked = false;
            }
        }
    }
}
