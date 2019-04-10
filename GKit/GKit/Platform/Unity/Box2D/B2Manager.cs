#if UNITY
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GKit {
	public class B2Manager : MonoBehaviour {
		public static B2Manager Instance {
			get; private set;
		}

		public B2Environment env;
		public GLoopCore core;
		private List<B2Body> bodyList = new List<B2Body>();

		private void Awake() {
			Instance = this;

			core.AddLoopAction(UpdateFrame);
		}
		private void Update() {
			Time.fixedDeltaTime = core.FPSInvert;
		}
		private void UpdateFrame() {
			Physics2D.gravity = env.gravity * (60f / Time.deltaTime); //Need Fix

			int count = bodyList.Count;
			for(int i=0; i<count; ++i) {
				B2Body body = bodyList[i];
				if (!body.Enable || !body.gameObject.activeInHierarchy)
					continue;

				Vector2 airDragFactor = new Vector2(
					1f - env.airDrag.x * body.airDragScale,
					1f - env.airDrag.y * body.airDragScale);

				body.Speed = new Vector2(
					(body.Speed.x + env.gravity.x) * airDragFactor.x, 
					(body.Speed.y + env.gravity.y) * airDragFactor.y);
				body.Speed = body.Speed.Clamp(-env.maxSpeed, env.maxSpeed);
			}
		}

		internal void AddBody(B2Body body) {
			bodyList.Add(body);
		}
		internal void RemoveBody(B2Body body) {
			bodyList.Remove(body);
		}
	}
}
#endif