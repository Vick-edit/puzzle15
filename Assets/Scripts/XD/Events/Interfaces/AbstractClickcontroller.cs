using UnityEngine;

namespace XD.Events
{
    /// <summary> Абстрактный класс, для контроля событий нажатия на элемент </summary>
    /// <typeparam name="TClickedObjectType"> Тип объекта, по которому произошло нажатие </typeparam>
    public abstract class AbstractClickСontroller<TClickedObjectType> : MonoBehaviour where TClickedObjectType : class
    {
        /// <summary> Событие, указывающее, что произошло нажатие по элементу </summary>
        public abstract event ClickEvent<TClickedObjectType> OnClicked;
    }
}