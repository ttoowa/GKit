using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GKitForUnity.Unity.EventHandler {
	public delegate void Collision2DEventDelegate(Collision2D collision);
	public delegate void Trigger2DEventDelegate(Collider2D collider);

	public class CollisionHandler2D : MonoBehaviour {
		public event Collision2DEventDelegate ColliderEnter2D;
		public event Collision2DEventDelegate ColliderStay2D;
		public event Collision2DEventDelegate ColliderExit2D;

		public event Trigger2DEventDelegate TriggerEnter2D;
		public event Trigger2DEventDelegate TriggerStay2D;
		public event Trigger2DEventDelegate TriggerExit2D;

		private void OnCollisionEnter2D(Collision2D collision) {
			ColliderEnter2D?.Invoke(collision);
		}
		private void OnCollisionStay2D(Collision2D collision) {
			ColliderStay2D?.Invoke(collision);
		}
		private void OnCollisionExit2D(Collision2D collision) {
			ColliderExit2D?.Invoke(collision);
		}


		private void OnTriggerEnter2D(Collider2D collider) {
			TriggerEnter2D?.Invoke(collider);
		}
		private void OnTriggerStay2D(Collider2D collider) {
			TriggerStay2D?.Invoke(collider);
		}
		private void OnTriggerExit2D(Collider2D collider) {
			TriggerExit2D?.Invoke(collider);
		}
	}
}
