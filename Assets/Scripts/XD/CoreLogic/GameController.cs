using System;
using System.Linq;
using System.Threading;
using UnityEngine;
using Random = System.Random;

namespace XD.CoreLogic
{
    /// <summary> Основной класс механики игры </summary>
    public class GameController : MonoBehaviour
    {
        /// <summary> Звук победы </summary>
        public AudioSource                      WinSound;
        /// <summary> Фоновая музыка </summary>
        public AudioSource                      BackgroundSound;
        /// <summary> Дефолтное значение громкости фоновой музыки </summary>
        private float                           backgroundSoundVolume;

        /// <summary> Константа - число пазлов на игровом поле, включая пустой </summary>
        private const int                       PUZZLES_COUNT = 16;
        /// <summary> Массив, всех используемых пазлов, нужен только для привязки элементов в Unity</summary>
        public AbstractPuzzleTileFacade[]       AllPuzzleTiles = new AbstractPuzzleTileFacade[PUZZLES_COUNT];
        /// <summary> Константа - размерность виртуального поля </summary>
        private const int                       VIRTUAL_FIELD_SIZE = 4;
        /// <summary> Виртуальное поле пазлов </summary>
        private AbstractPuzzleTileFacade[,]     virtualFieldOfPuzzles;
        /// <summary> Семафор, многопоточного доступа к флагу блокировки </summary>
        private readonly ReaderWriterLockSlim   virtualFieldLock = new ReaderWriterLockSlim();
        /// <summary> флаг, указывающий заблокирована ли обработка новых игровых событий </summary>
        private bool                            isLocked = false;
        /// <summary> флаг, указывающий завершена ли игра </summary>
        private bool                            isWin = false;

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
            backgroundSoundVolume = BackgroundSound.volume;

            FillVirtualField();
            do
            {
                ShuffleVirtualField();
                CheckIsItWin(false);
            } while (isWin);
        }

        #endregion

        /// <summary> Заполнить виртуальное поле пазлов и инициализировать виртуальные координаты пазлов </summary>
        private void FillVirtualField()
        {
            virtualFieldOfPuzzles = new AbstractPuzzleTileFacade[VIRTUAL_FIELD_SIZE, VIRTUAL_FIELD_SIZE];
            for (int i = 0; i < VIRTUAL_FIELD_SIZE; i++)
            {
                for (int j = 0; j < VIRTUAL_FIELD_SIZE; j++)
                {
                    int rowNum = i;
                    int columnNum = j;
                    //Переводим координаты строки и столбца в индекс массива элементов
                    int flatIndex = rowNum*4 + columnNum;
                    //Иницализируем виртуальные координаты
                    AbstractPuzzleTileFacade currentPuzzleTile = AllPuzzleTiles[flatIndex];
                    currentPuzzleTile.StartCoordinates = new PuzzleVirtualCoordinates(rowNum, columnNum);
                    currentPuzzleTile.CurrentCoordinates = currentPuzzleTile.StartCoordinates.Clone();
                    //Сохраняем пазл на виртуальное поле
                    virtualFieldOfPuzzles[rowNum, columnNum] = currentPuzzleTile;
                    //Если текущий пазл не пустой
                    if (!currentPuzzleTile.IsEpmty)
                    {
                        //Привязываем обработку нажатия на пазл
                        currentPuzzleTile.ClickСontroller.OnClicked += PuzzleWasClicked;
                    }
                }
            }
        }

        /// <summary> Перемешать пазлы </summary>
        private void ShuffleVirtualField()
        {
            int maxPuzzleIndex = PUZZLES_COUNT;

            //На последнем месте на виртуальном поле всегда должен стоять пустой пазл
            AbstractPuzzleTileFacade lastPuzzleTile = virtualFieldOfPuzzles[VIRTUAL_FIELD_SIZE - 1, VIRTUAL_FIELD_SIZE - 1];
            if (!lastPuzzleTile.IsEpmty)
            {
                AbstractPuzzleTileFacade emptyPuzzleTile = AllPuzzleTiles.First(puzzle => puzzle.IsEpmty);
                SwapToPuzzles(lastPuzzleTile, emptyPuzzleTile);
            }
            //В одномерном списке пазлов пустой так же должен стоять на последнем месте
            lastPuzzleTile = AllPuzzleTiles.Last();
            if (!lastPuzzleTile.IsEpmty)
            {
                for (int i = 0; i < PUZZLES_COUNT; i++)
                {
                    if (AllPuzzleTiles[i].IsEpmty)
                    {
                        AllPuzzleTiles[PUZZLES_COUNT - 1] = AllPuzzleTiles[i];
                        AllPuzzleTiles[i] = lastPuzzleTile;
                        break;
                    }
                }
            }
            //т.к. последний пазл пустой, его мы трогать не будем - уменьшаем максимальный индекс элементов, которые будем перемешивать
            maxPuzzleIndex--;

            Random random = new Random();
            //для всех оставшихся пазлов - 1
            for (int oldIndex = 0; oldIndex < maxPuzzleIndex-1; oldIndex++)
            {
                //определяем новый индекс, больший, чем текущий
                int newIndex = random.Next(oldIndex + 1, maxPuzzleIndex);

                //меняем местами, соответствующие элементы пазла
                SwapToPuzzles(AllPuzzleTiles[oldIndex], AllPuzzleTiles[newIndex]);
            }
        }

        /// <summary> Обмен местами на виртуальной и визуальной доске двух пазлов </summary>
        private void SwapToPuzzles(AbstractPuzzleTileFacade firstPuzzle, AbstractPuzzleTileFacade secondPuzzle)
        {
            Vector3 firstPuzzleVector = firstPuzzle.transform.position;
            PuzzleVirtualCoordinates firstPuzzleCoordinates = firstPuzzle.CurrentCoordinates;

            firstPuzzle.transform.position = secondPuzzle.transform.position;
            firstPuzzle.CurrentCoordinates = secondPuzzle.CurrentCoordinates;

            secondPuzzle.transform.position = firstPuzzleVector;
            secondPuzzle.CurrentCoordinates = firstPuzzleCoordinates;

            virtualFieldOfPuzzles[GetPuzzleRow(firstPuzzle), GetPuzzleCol(firstPuzzle)] = firstPuzzle;
            virtualFieldOfPuzzles[GetPuzzleRow(secondPuzzle), GetPuzzleCol(secondPuzzle)] = secondPuzzle;
        }

        /// <summary> Обработка нажатия на пазл </summary>
        /// <param name="puzzleTile">Пазл, по которому произошло нажатие</param>
        private void PuzzleWasClicked(AbstractPuzzleTileFacade puzzleTile)
        {
            virtualFieldLock.EnterReadLock();
            //Если состояние игры - заблокирована
            if (isLocked)
            {
                //То не обрабатываем нажатия на пазлы
                virtualFieldLock.ExitReadLock();
                return;
            }
            virtualFieldLock.ExitReadLock();

            var emptyPuzzleTile = GetEmptyNeighbor(puzzleTile);
            //Если среди соседних пазлов нет пустого
            if (emptyPuzzleTile == null)
            {
                //Завершаем обработку нажатия
                return;
            }

            //Иначе входим в цикл обмена местами нажатого пазла и соседнего пустого
            virtualFieldLock.EnterWriteLock();
            isLocked = true;
            virtualFieldLock.ExitWriteLock();

            //Привязываем обработчик завершения обмена местами пазлов
            puzzleTile.OnSwapEndedEvent += PuzzlesSwaped;
            //Начинаем обмен местами
            puzzleTile.SwapWithEmptyTile(emptyPuzzleTile);
        }

        /// <summary> Обработка завершения обмена местами пазлов по нажатию на один из них </summary>
        private void PuzzlesSwaped(AbstractPuzzleTileFacade notEmptyPuzzle, AbstractPuzzleTileFacade emptyPuzzle)
        {
            try
            {
                //обновляем координаты на виртуальном поле
                virtualFieldOfPuzzles[GetPuzzleRow(notEmptyPuzzle), GetPuzzleCol(notEmptyPuzzle)] = notEmptyPuzzle;
                virtualFieldOfPuzzles[GetPuzzleRow(emptyPuzzle), GetPuzzleCol(emptyPuzzle)] = emptyPuzzle;
                CheckIsItWin();
            }
            finally
            {
                //высвобождаем флаг
                virtualFieldLock.EnterWriteLock();
                isLocked = isWin; //isLocked = false, только если не была завершена игра этим ходом
                virtualFieldLock.ExitWriteLock();
            }
        }

        /// <summary> Найти пустой пазл среди соседних к текущему пазлу </summary>
        /// <param name="puzzleTile">Пазл для которого осуществляем поиск</param>
        /// <returns>Пустой соседний пазл или null</returns>
        private AbstractPuzzleTileFacade GetEmptyNeighbor(AbstractPuzzleTileFacade puzzleTile)
        {
            int currentRow = puzzleTile.CurrentCoordinates.Row;
            int currentCol = puzzleTile.CurrentCoordinates.Column;

            int topNeighborRow = currentRow - 1;
            int bottomNeighborRow = currentRow + 1;
            int leftNeighborCol = currentCol - 1;
            int rightNeighborCol = currentCol + 1;

            AbstractPuzzleTileFacade emptyPuzzleTile = null;

            if (topNeighborRow >= 0 && virtualFieldOfPuzzles[topNeighborRow, currentCol].IsEpmty)
            {
                emptyPuzzleTile = virtualFieldOfPuzzles[topNeighborRow, currentCol];
            }
            else if (bottomNeighborRow < VIRTUAL_FIELD_SIZE && virtualFieldOfPuzzles[bottomNeighborRow, currentCol].IsEpmty)
            {
                emptyPuzzleTile = virtualFieldOfPuzzles[bottomNeighborRow, currentCol];
            }
            else if (leftNeighborCol >= 0 && virtualFieldOfPuzzles[currentRow, leftNeighborCol].IsEpmty)
            {
                emptyPuzzleTile = virtualFieldOfPuzzles[currentRow, leftNeighborCol];
            }
            else if (rightNeighborCol < VIRTUAL_FIELD_SIZE && virtualFieldOfPuzzles[currentRow, rightNeighborCol].IsEpmty)
            {
                emptyPuzzleTile = virtualFieldOfPuzzles[currentRow, rightNeighborCol];
            }

            return emptyPuzzleTile;
        }

        /// <summary> Проверить завершена ли игра </summary>
        /// <param name="isItUserAction"> Вызвана ли эта проверка в связи с действями игрока </param>
        private void CheckIsItWin(bool isItUserAction = true)
        {
            isWin = true;
            foreach (var puzzle in AllPuzzleTiles)
            {
                if (!puzzle.CurrentCoordinates.Equals(puzzle.StartCoordinates))
                {
                    isWin = false;
                    break;
                }
            }
            if (isWin && isItUserAction)
            {
                BackgroundSound.volume = backgroundSoundVolume/2;
                WinSound.Play();
            }
        }

        /// <summary> Номер строки из текущей координаты пазла </summary>
        private int GetPuzzleRow(AbstractPuzzleTileFacade puzzleTile)
        {
            return puzzleTile.CurrentCoordinates.Row;
        }

        /// <summary> Номер столбца из текущей координаты пазла </summary>
        private int GetPuzzleCol(AbstractPuzzleTileFacade puzzleTile)
        {
            return puzzleTile.CurrentCoordinates.Column;
        }
    }
}
