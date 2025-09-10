using System.Collections;
using UnityEngine;

namespace App.Services.Runners
{
    public class CoroutineRunner : MonoBehaviour, ICoroutineRunner
    {
        public Coroutine Run(IEnumerator coroutine)
        {
            return StartCoroutine(coroutine);
        }

        public void Stop(Coroutine unmuteChecker)
        {
            StopCoroutine(unmuteChecker);
        }
    }
}
