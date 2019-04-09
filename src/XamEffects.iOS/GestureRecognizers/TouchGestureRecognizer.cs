using System;
using Foundation;
using UIKit;

namespace XamEffects.iOS.GestureRecognizers {
    public class TouchGestureRecognizer : UIGestureRecognizer {
        public enum TouchState {
            Started,
            Ended
        }

        public event EventHandler<TouchArgs> OnTouch;

        public override void TouchesBegan(NSSet touches, UIEvent evt) {
            base.TouchesBegan(touches, evt);
            OnTouch?.Invoke(View, new TouchArgs(TouchState.Started));
            State = UIGestureRecognizerState.Began;
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt) {
            base.TouchesEnded(touches, evt);
            OnTouch?.Invoke(View, new TouchArgs(TouchState.Ended));
            State = UIGestureRecognizerState.Ended;
        }

        public override void TouchesCancelled(NSSet touches, UIEvent evt) {
            base.TouchesCancelled(touches, evt);
            OnTouch?.Invoke(View, new TouchArgs(TouchState.Ended));
            State = UIGestureRecognizerState.Cancelled;
        }

        public class TouchArgs : EventArgs {
            public TouchState State { get; }

            public TouchArgs(TouchState state) {
                State = state;
            }
        }
    }

    public class TouchGestureRecognizerDelegate : UIGestureRecognizerDelegate {
        readonly UIView _view;

        public TouchGestureRecognizerDelegate(UIView view) {
            _view = view;
        }

        public override bool ShouldRecognizeSimultaneously(UIGestureRecognizer gestureRecognizer,
            UIGestureRecognizer otherGestureRecognizer) {
            return true;
        }

        public override bool ShouldReceiveTouch(UIGestureRecognizer recognizer, UITouch touch) {
            return touch.View == _view;
        }
    }
}