using System;
using System.Collections.Generic;
using UIKit;

namespace XamEffects.iOS.GestureCollectors
{
    public static class TapGestureCollector
    {
        private static Dictionary<UIView, GestureActionsContainer> Collection { get; } = new Dictionary<UIView, GestureActionsContainer>();

        public static void Add(UIView view, Action action)
        {
            if (Collection.ContainsKey(view))
            {
                Collection[view].Actions.Add(action);
            }
            else
            {
                var gest = new UITapGestureRecognizer(ActionActivator);
                Collection.Add(view, new GestureActionsContainer
                {
                    Recognizer = gest,
                    Actions = new List<Action>
                    {
                        action
                    }
                });
                view.AddGestureRecognizer(gest);
            }
        }

        public static void Delete(UIView view, Action action)
        {
            if (Collection.ContainsKey(view))
            {
                var ci = Collection[view];
                ci.Actions.Remove(action);
                if (ci.Actions.Count == 0)
                {
                    view.RemoveGestureRecognizer(ci.Recognizer);
                    ci.Recognizer.Dispose();
                    ci.Recognizer = null;
                    ci.Actions = null;
                    Collection.Remove(view);
                }
            }
        }

        private static void ActionActivator(UITapGestureRecognizer uiTapGestureRecognizer)
        {
            if (Collection.ContainsKey(uiTapGestureRecognizer.View))
            { 
                foreach (var valueAction in Collection[uiTapGestureRecognizer.View].Actions)
                {
                    valueAction?.Invoke();
                }
            }
        }

        private class GestureActionsContainer
        {
            public UIGestureRecognizer Recognizer { get; set; }

            public List<Action> Actions { get; set; }
        }

        public static void Init()
        {

        }
    }
}