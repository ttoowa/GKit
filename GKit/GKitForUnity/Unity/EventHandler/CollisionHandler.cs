using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GKitForUnity.Unity.EventHandler {
	public delegate void CollisionEventDelegate(GameObject sender, Collision collision);
	public delegate void TriggerEventDelegate(GameObject sender, Collider collider);

	public class CollisionHandler : MonoBehaviour {
		public event CollisionEventDelegate CollisionEnter;
		public event CollisionEventDelegate CollisionStay;
		public event CollisionEventDelegate CollisionExit;

		public event TriggerEventDelegate TriggerEnter;
		public event TriggerEventDelegate TriggerStay;
		public event TriggerEventDelegate TriggerExit;

		private void OnCollisionEnter(Collision collision) {
			CollisionEnter?.Invoke(gameObject, collision);
		}
		private void OnCollisionStay(Collision collision) {
			CollisionStay?.Invoke(gameObject, collision);
		}
		private void OnCollisionExit(Collision collision) {
			CollisionExit?.Invoke(gameObject, collision);
		}


		private void OnTriggerEnter(Collider collider) {
			TriggerEnter?.Invoke(gameObject, collider);
		}
		private void OnTriggerStay(Collider collider) {
			TriggerStay?.Invoke(gameObject, collider);
		}
		private void OnTriggerExit(Collider collider) {
			TriggerExit?.Invoke(gameObject, collider);
		}
	}
}
