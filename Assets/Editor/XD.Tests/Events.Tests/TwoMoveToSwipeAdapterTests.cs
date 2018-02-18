using System;
using System.Collections.Generic;
using NUnit.Framework;
using XD.Events;

namespace Assets.Editor.XD.Tests.Events.Tests
{
    [TestFixture]
    public class TwoMoveToSwipeAdapterTests
    {
        [Test]
        public void BindFirst_SeveralCallInRow_ShouldBindToLastOnly()
        {
            //arrange
            List<MovementEndedEvent> allEvents = new List<MovementEndedEvent>();
            Action<MovementEndedEvent> eventSubscriber = (sunscriber) => allEvents.Add(sunscriber);
            Action<MovementEndedEvent> eventUnsubscriber = (sunscriber) => allEvents.Remove(sunscriber);

            TwoMoveToSwipeAdapter eventAdapter = new TwoMoveToSwipeAdapter();


            //act
            eventAdapter.BindFirst(eventSubscriber, eventUnsubscriber).
                BindFirst(eventSubscriber, eventUnsubscriber).
                BindFirst(eventSubscriber, eventUnsubscriber).
                BindFirst(eventSubscriber, eventUnsubscriber);

            //assert
            Assert.That(allEvents.Count, Is.EqualTo(1));
        }

        [Test]
        public void BindSecond_SeveralCallInRow_ShouldBindToLastOnly()
        {
            //arrange
            List<MovementEndedEvent> allEvents = new List<MovementEndedEvent>();
            Action<MovementEndedEvent> eventSubscriber = (sunscriber) => allEvents.Add(sunscriber);
            Action<MovementEndedEvent> eventUnsubscriber = (sunscriber) => allEvents.Remove(sunscriber);

            TwoMoveToSwipeAdapter eventAdapter = new TwoMoveToSwipeAdapter();


            //act
            eventAdapter.BindSecond(eventSubscriber, eventUnsubscriber).
                BindSecond(eventSubscriber, eventUnsubscriber).
                BindSecond(eventSubscriber, eventUnsubscriber).
                BindSecond(eventSubscriber, eventUnsubscriber);

            //assert
            Assert.That(allEvents.Count, Is.EqualTo(1));
        }

        [Test]
        public void BindFinal_BindTwoCorrectMovementEvent_ShouldUnsunscribeAfterEvents()
        {
            //arrange
            List<MovementEndedEvent> allEvents = new List<MovementEndedEvent>();
            Action<MovementEndedEvent> eventSubscriber = (sunscriber) => allEvents.Add(sunscriber);
            Action<MovementEndedEvent> eventUnsubscriber = (sunscriber) => allEvents.Remove(sunscriber);
            Action finalAction = () => {};

            TwoMoveToSwipeAdapter eventAdapter = new TwoMoveToSwipeAdapter();
            eventAdapter.BindFirst(eventSubscriber, eventUnsubscriber)
                .BindSecond(eventSubscriber, eventUnsubscriber)
                .BindFinal(finalAction);

            //act
            for (int i = 0; i < allEvents.Count; i++)
            {
                MovementEndedEvent endedEvent = allEvents[i];
                endedEvent.Invoke();
            }

            //assert
            Assert.That(allEvents, Is.Empty);
        }

        [Test]
        public void BindFinal_BindTwoCorrectMovementEvent_ShouldCallFinalEventOnce()
        {
            //arrange
            List<MovementEndedEvent> allEvents= new List<MovementEndedEvent>();
            Action<MovementEndedEvent> eventSubscriber = (sunscriber) => allEvents.Add(sunscriber);
            Action<MovementEndedEvent> eventUnsubscriber = (sunscriber) => allEvents.Remove(sunscriber);

            int actionCallcounter = 0;
            Action finalAction = () => actionCallcounter++;

            TwoMoveToSwipeAdapter eventAdapter = new TwoMoveToSwipeAdapter();
            eventAdapter.BindFirst(eventSubscriber, eventUnsubscriber)
                .BindSecond(eventSubscriber, eventUnsubscriber)
                .BindFinal(finalAction);

            //act
            for (int i = 0; i < allEvents.Count; i++)
            {
                MovementEndedEvent endedEvent = allEvents[i];
                endedEvent.Invoke();
            }
            for (int i = 0; i < allEvents.Count; i++)
            {
                MovementEndedEvent endedEvent = allEvents[i];
                endedEvent.Invoke();
            }
            for (int i = 0; i < allEvents.Count; i++)
            {
                MovementEndedEvent endedEvent = allEvents[i];
                endedEvent.Invoke();
            }

            //assert
            Assert.That(actionCallcounter, Is.EqualTo(1));
        }
    }
}