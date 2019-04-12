using System;
using System.Linq;
using Foundation;
using UIKit;
using Xamarin.Forms;
using System.Threading.Tasks;
using CoreGraphics;
using XamEffects.iOS.GestureCollectors;
using System.Threading;

namespace XamEffects.iOS.GestureRecognizers {
    public class TouchGestureRecognizer : UIGestureRecognizer {
        CGPoint _startPos;
        TaskCompletionSource<object> _startAwaiter;

        public bool Processing => State == UIGestureRecognizerState.Began || State == UIGestureRecognizerState.Changed;

        public enum TouchState {
            Started,
            Ended,
            Cancelled
        }

        public event EventHandler<TouchArgs> OnTouch;

        public override async void TouchesBegan(NSSet touches, UIEvent evt) {
            base.TouchesBegan(touches, evt);

            if (Processing)
                return;

            _startPos = PointInWindow(View);
            State = UIGestureRecognizerState.Began;
            _startAwaiter?.TrySetCanceled();
            _startAwaiter = new TaskCompletionSource<object>();

            await Task.Delay(125);

            if (Processing) {
                OnTouch?.Invoke(this, new TouchArgs(TouchState.Started, true));
                _startAwaiter.TrySetResult(null);
            }
        }

        public override void TouchesMoved(NSSet touches, UIEvent evt) {
            var inside = View.PointInside(LocationInView(View), evt);
            var currentPos = inside ? PointInWindow(View) : _startPos;

            if (!inside || Math.Abs(currentPos.X - _startPos.X) > 1 || Math.Abs(currentPos.Y - _startPos.Y) > 1) {
                if (Processing)
                    OnTouch?.Invoke(this, new TouchArgs(TouchState.Ended, false));
                State = UIGestureRecognizerState.Ended;
                return;
            }

            State = UIGestureRecognizerState.Changed;
            base.TouchesMoved(touches, evt);
        }

        public override async void TouchesEnded(NSSet touches, UIEvent evt) {
            var inside = View.PointInside(LocationInView(View), evt);

            if (Processing && inside) {
                try {
                    await _startAwaiter.Task;
                    OnTouch?.Invoke(this, new TouchArgs(TouchState.Ended, View.PointInside(LocationInView(View), null)));
                    State = UIGestureRecognizerState.Ended;
                }
                catch (TaskCanceledException) {
                    State = UIGestureRecognizerState.Cancelled;
                }
            }
            State = UIGestureRecognizerState.Ended;

            base.TouchesEnded(touches, evt);
        }

        public override void TouchesCancelled(NSSet touches, UIEvent evt) {
            base.TouchesCancelled(touches, evt);
            OnTouch?.Invoke(this, new TouchArgs(TouchState.Cancelled, false));
            State = UIGestureRecognizerState.Cancelled;
        }

        CGPoint PointInWindow(UIView view) {
            var relativePositionView = UIApplication.SharedApplication.KeyWindow;
            var relativeFrame = view.Superview.ConvertRectToView(view.Frame, relativePositionView);

            return new CGPoint(relativeFrame.X, relativeFrame.Y);
        }

        public class TouchArgs : EventArgs {
            public TouchState State { get; }
            public bool Inside { get; }

            public TouchArgs(TouchState state, bool inside) {
                State = state;
                Inside = inside;
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