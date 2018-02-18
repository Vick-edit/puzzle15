using System;

namespace XD.CoreLogic
{
    /// <summary> Виртуальные X,Y координаты пазла </summary>
    public class PuzzleVirtualCoordinates
    {
        /// <summary> Строка пазла, отсчет от 0 </summary>
        public int Row { get; set; }
        /// <summary> Колонка пазла, отсчет от 0 </summary>
        public int Column { get; set; }


        public PuzzleVirtualCoordinates(int row, int column)
        {
            Row = row;
            Column = column;
        }


        /// <summary> Получить клон текущих координат </summary>
        public PuzzleVirtualCoordinates Clone()
        {
            return new PuzzleVirtualCoordinates(Row, Column);
        }

        public override bool Equals(object otherObject)
        {
            PuzzleVirtualCoordinates otherCoordinates = otherObject as PuzzleVirtualCoordinates;
            if (otherCoordinates == null)
            {
                return false;
            }

            return otherCoordinates.Row == Row && otherCoordinates.Column == Column;
        }

        public override int GetHashCode()
        {
            return Row.GetHashCode() ^ Column.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("Строка {0}, Колонка {1}", Row, Column);
        }
    }
}