using System.Collections;
using UnityEngine;

namespace App.Services.Runners
{
    public interface ICoroutineRunner
    {
        Coroutine Run(IEnumerator coroutine);
        void Stop(Coroutine unmuteChecker);
    }
}
