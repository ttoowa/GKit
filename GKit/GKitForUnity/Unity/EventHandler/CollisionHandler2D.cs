using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GKitForUnity.Unity.EventHandler {
	public delegate void Collision2DEventDelegate(GameObject sender, Collision2D collision);
	public delegate void Trigger2DEventDelegate(GameObject sender, Collider2D collider);

	public class CollisionHandler2D : MonoBehaviour {
		public event Collision2DEventDelegate CollisionEnter2D;
		public event Collision2DEventDelegate CollisionStay2D;
		public event Collision2DEventDelegate CollisionExit2D;

		public event Trigger2DEventDelegate TriggerEnter2D;
		public event Trigger2DEventDelegate TriggerStay2D;
		public event Trigger2DEventDelegate TriggerExit2D;

		private void OnCollisionEnter2D(Collision2D collision) {
			CollisionEnter2D?.Invoke(gameObject, collision);
		}
		private void OnCollisionStay2D(Collision2D collision) {
			CollisionStay2D?.Invoke(gameObject, collision);
		}
		private void OnCollisionExit2D(Collision2D collision) {
			CollisionExit2D?.Invoke(gameObject, collision);
		}


		private void OnTriggerEnter2D(Collider2D collider) {
			TriggerEnter2D?.Invoke(gameObject, collider);
		}
		private void OnTriggerStay2D(Collider2D collider) {
			TriggerStay2D?.Invoke(gameObject, collider);
		}
		private void OnTriggerExit2D(Collider2D collider) {
			TriggerExit2D?.Invoke(gameObject, collider);
		}
	}
}
