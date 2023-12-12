using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class AwaitableView : MonoBehaviour
{
    protected ClickAwaiter clickAwaiter;
    public class ClickAwaiter : INotifyCompletion
    {
        private bool _isCompleted;
        private Action _continuation;
        private bool _attack;
        public bool IsCompleted => _isCompleted;
        public void OnCompleted(Action continuation)
        {
            if (IsCompleted) continuation?.Invoke();
            else _continuation = continuation;
        }
        public bool GetResult() => _attack;
        public void Finish(bool attack = true)
        {
            _attack = attack;
            _isCompleted = true;
            _continuation?.Invoke();
            _continuation = null;
        }
    }
    public ClickAwaiter GetAwaiter()
    {
        clickAwaiter = new ClickAwaiter();
        OnStartAwait();
        return clickAwaiter;
    }
    protected virtual void OnStartAwait() { }
}
