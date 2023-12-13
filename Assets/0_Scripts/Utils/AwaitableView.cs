using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class AwaitableView : MonoBehaviour
{
    protected Awaiter currentAwaiter;
    public class Awaiter : INotifyCompletion
    {
        private bool _isCompleted;
        private Action _continuation;
        private bool _success;
        public bool IsCompleted => _isCompleted;
        public void OnCompleted(Action continuation)
        {
            if (IsCompleted) continuation?.Invoke();
            else _continuation = continuation;
        }
        public bool GetResult() => _success;
        public void Finish(bool attack = true)
        {
            _success = attack;
            _isCompleted = true;
            _continuation?.Invoke();
            _continuation = null;
        }
    }
    public Awaiter GetAwaiter()
    {
        currentAwaiter = new Awaiter();
        OnStartAwait();
        return currentAwaiter;
    }
    protected virtual void OnStartAwait() { }
}
