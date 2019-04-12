using System;
using Foundation;
using UIKit;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace XamEffects.iOS.GestureRecognizers {
    public class TouchGestureRecognizer : UIGestureRecognizer {
        Point _startPoint;
        TaskCompletionSource<bool> _awaiter;
        bool _ended;

        public enum TouchState {
            Started,
            Ended,
            Cancelled
        }

        public TouchGestureRecognizer() {
            CancelsTouchesInView = false;
        }

        public event EventHandler<TouchArgs> OnTouch;

        public override async void TouchesBegan(NSSet touches, UIEvent evt) {
            base.TouchesBegan(touches, evt);
            _ended = false;
            _startPoint = PointInWindow(View);
            _awaiter?.TrySetResult(false);
            _awaiter = new TaskCompletionSource<bool>();

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Task.Delay(125).ContinueWith(task => {
                _awaiter.TrySetResult(true);
            });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            var start = await _awaiter.Task;
            if (start && !_ended) {
                OnTouch?.Invoke(View, new TouchArgs(TouchState.Started));
                State = UIGestureRecognizerState.Began;
                return;
            }
            State = UIGestureRecognizerState.Cancelled;
        }

        public override void TouchesMoved(NSSet touches, UIEvent evt) {
            var current = PointInWindow(View);
            if (Math.Abs(current.X - _startPoint.X) > 1 || Math.Abs(current.Y - _startPoint.Y) > 1) {
                _awaiter?.TrySetResult(false);
                if (State == UIGestureRecognizerState.Began || State == UIGestureRecognizerState.Changed) {
                    OnTouch?.Invoke(View, new TouchArgs(TouchState.Ended));
                    State = UIGestureRecognizerState.Ended;
                }
            }

            State = UIGestureRecognizerState.Changed;

            base.TouchesMoved(touches, evt);
        }

        public override async void TouchesEnded(NSSet touches, UIEvent evt) {
            base.TouchesEnded(touches, evt);

            var end = await _awaiter.Task;
            if (end) {
                _ended = true;
                OnTouch?.Invoke(View, new TouchArgs(TouchState.Ended));
                State = UIGestureRecognizerState.Ended;
                return;
            }
            State = UIGestureRecognizerState.Cancelled;
        }

        public override void TouchesCancelled(NSSet touches, UIEvent evt) {
            base.TouchesCancelled(touches, evt);
            OnTouch?.Invoke(View, new TouchArgs(TouchState.Cancelled));
            State = UIGestureRecognizerState.Cancelled;
        }

        Point PointInWindow(UIView view) {
            var relativePositionView = UIApplication.SharedApplication.KeyWindow;
            var relativeFrame = view.Superview.ConvertRectToView(view.Frame, relativePositionView);

            return new Point(relativeFrame.X, relativeFrame.Y);
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