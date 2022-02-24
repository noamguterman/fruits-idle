using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
	public class MultipleCoroutine : CustomYieldInstruction, IEnumerable, IDisposable
	{
		public bool IsRunning { get; set; }

		public override bool keepWaiting
		{
			get
			{
				return this.IsRunning;
			}
		}

		public MultipleCoroutine(MonoBehaviour monoBehaviour = null)
		{
			this.monoBehaviour = (monoBehaviour ? monoBehaviour : CoroutineUtils.CoroutineHandler);
			this.currentActiveRoutines = new List<Coroutine>();
			this.ienimeratorFunc = new List<Func<IEnumerator>>();
		}

		public void Add(Func<IEnumerator> newIEnumerator)
		{
			this.ienimeratorFunc.Add(newIEnumerator);
		}

		public void StartCoroutines(bool loop = false)
		{
			this.IsRunning = true;
			List<Coroutine> list = this.currentActiveRoutines;
			MonoBehaviour monoBehaviour = this.monoBehaviour;
			this.infiniteLoop = loop;
			list.Add(monoBehaviour.StartCoroutine(loop ? this.LoopExecute(this.ienimeratorFunc) : this.ChainExecute(this.ienimeratorFunc)));
		}

		private void StopAllCoroutines()
		{
			this.currentActiveRoutines.ForEach(delegate(Coroutine c)
			{
				this.monoBehaviour.StopCoroutineIfRunning(c);
			});
		}

		private IEnumerator LoopExecute(List<Func<IEnumerator>> iEnumeratorsToExec)
		{
			for (;;)
			{
				Coroutine routine = this.monoBehaviour.StartCoroutine(this.ChainExecute(iEnumeratorsToExec));
				this.currentActiveRoutines.Add(routine);
				yield return routine;
				this.currentActiveRoutines.Remove(routine);
				routine = null;
			}
			yield break;
		}

		private IEnumerator ChainExecute(List<Func<IEnumerator>> iEnumeratorsToExec)
		{
			int index = 0;
			while (index < iEnumeratorsToExec.Count)
			{
				int num = index;
				index = num + 1;
				IEnumerator enumerator = iEnumeratorsToExec[num]();
				if (enumerator != null)
				{
					Coroutine routine = this.monoBehaviour.StartCoroutine(enumerator);
					this.currentActiveRoutines.Add(routine);
					yield return routine;
					this.currentActiveRoutines.Remove(routine);
					routine = null;
				}
			}
			if (!this.infiniteLoop)
			{
				this.Dispose();
			}
			yield break;
		}

		public void Dispose()
		{
			this.IsRunning = false;
			this.StopAllCoroutines();
		}

		public IEnumerator GetEnumerator()
		{
			return this.ienimeratorFunc.GetEnumerator();
		}

		private readonly MonoBehaviour monoBehaviour;

		private readonly List<Coroutine> currentActiveRoutines;

		private readonly List<Func<IEnumerator>> ienimeratorFunc;

		private bool infiniteLoop;
	}
}
