using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GKitForUnity.Unity.EventHandler {
	public delegate void CollisionEventDelegate(Collision collision);
	public delegate void TriggerEventDelegate(Collider collider);

	public class CollisionHandler : MonoBehaviour {
		public event CollisionEventDelegate ColliderEnter;
		public event CollisionEventDelegate ColliderStay;
		public event CollisionEventDelegate ColliderExit;

		public event TriggerEventDelegate TriggerEnter;
		public event TriggerEventDelegate TriggerStay;
		public event TriggerEventDelegate TriggerExit;

		private void OnCollisionEnter(Collision collision) {
			ColliderEnter?.Invoke(collision);
		}
		private void OnCollisionStay(Collision collision) {
			ColliderStay?.Invoke(collision);
		}
		private void OnCollisionExit(Collision collision) {
			ColliderExit?.Invoke(collision);
		}


		private void OnTriggerEnter(Collider collider) {
			TriggerEnter?.Invoke(collider);
		}
		private void OnTriggerStay(Collider collider) {
			TriggerStay?.Invoke(collider);
		}
		private void OnTriggerExit(Collider collider) {
			TriggerExit?.Invoke(collider);
		}
	}
}
