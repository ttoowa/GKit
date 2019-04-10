#if UNITY
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UVector2 = UnityEngine.Vector2;

namespace GKit {
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Rigidbody2D))]
	public class B2Body : MonoBehaviour {
		private static B2Manager Manager {
			get {
				return B2Manager.Instance;
			}
		}
		
		public Rigidbody2D Rigidbody {
			get; private set;
		}

		//PhysicsProperty
		public bool Enable {
			get {
				return Rigidbody.simulated;
			}
			set {
				Rigidbody.simulated = value;
			}
		}
		public UVector2 Pos {
			get {
				return Rigidbody.position;
			}
			set {
				Rigidbody.position = value;
			}
		}
		public float Rot {
			get {
				return Rigidbody.rotation;
			}
			set {
				Rigidbody.rotation = value;
			}
		}
		public UVector2 Speed {
			get {
				return Rigidbody.velocity * Time.deltaTime;
			} set {
				Rigidbody.velocity = value / Time.deltaTime;
			}
		}
		public UVector2 Speed_Velocity {
			get {
				return Rigidbody.velocity;
			} set {
				Rigidbody.velocity = value;
			}
		}
		public float airDragScale = 1f;

		//Info
		public bool IsSleeping {
			get {
				return Rigidbody.IsSleeping();
			}
		}

		//Setting
		public bool UseAutoMass {
			get {
				return Rigidbody.useAutoMass;
			}
			set {
				Rigidbody.useAutoMass = value;
			}
		}
		public bool UseFullKineticContacts {
			get {
				return Rigidbody.useFullKinematicContacts;
			}
			set {
				Rigidbody.useFullKinematicContacts = value;
			}
		}
		public float Mass {
			get {
				return Rigidbody.mass;
			}
			set {
				Rigidbody.mass = value;
			}
		}
		public float Friction {
			get {
				return Rigidbody.drag;
			}
			set {
				Rigidbody.drag = value;
			}
		}
		public RigidbodyType2D BodyType {
			get {
				return Rigidbody.bodyType;
			}
			set {
				Rigidbody.bodyType = value;
			}
		}
		public RigidbodyConstraints2D Constraint {
			get {
				return Rigidbody.constraints;
			}
			set {
				Rigidbody.constraints = value;
			}
		}
		public RigidbodyInterpolation2D Interpolate {
			get {
				return Rigidbody.interpolation;
			}
			set {
				Rigidbody.interpolation = value;
			}
		}
		public RigidbodySleepMode2D SleepMode {
			get {
				return Rigidbody.sleepMode;
			}
			set {
				Rigidbody.sleepMode = value;
			}
		}

		//Event
		public event SingleDelegate<Collision2D> OnCollisionEnter;
		public event SingleDelegate<Collision2D> OnCollisionStay;
		public event SingleDelegate<Collision2D> OnCollisionExit;
		public event SingleDelegate<Collider2D> OnTriggerEnter;
		public event SingleDelegate<Collider2D> OnTriggerStay;
		public event SingleDelegate<Collider2D> OnTriggerExit;

		private void Awake() {
			Rigidbody = GetComponent<Rigidbody2D>();

			Rigidbody.gravityScale = 0f;
		}
		private void Start() {
			if (Manager != null) {
				Manager.AddBody(this);
			} else {
				"B2Manager 컴포넌트를 생성해 주세요.".Log();
			}
		}
		private void OnDestroy() {
			if (Manager != null) {
				Manager.RemoveBody(this);
			}
		}
		//private void UpdateFrame() {
		//	if (Speed.magnitude < 0.003f)
		//		return;

		//	Rigidbody.position += Speed;
		//}

		private void OnCollisionEnter2D(Collision2D collision) {
			OnCollisionEnter?.Invoke(collision);
		}
		private void OnCollisionStay2D(Collision2D collision) {
			OnCollisionStay?.Invoke(collision);
		}
		private void OnCollisionExit2D(Collision2D collision) {
			OnCollisionExit?.Invoke(collision);
		}
		private void OnTriggerEnter2D(Collider2D collider) {
			OnTriggerEnter?.Invoke(collider);
		}
		private void OnTriggerStay2D(Collider2D collider) {
			OnTriggerStay?.Invoke(collider);
		}
		private void OnTriggerExit2D(Collider2D collider) {
			OnTriggerExit?.Invoke(collider);
		}
	}
}
#endif