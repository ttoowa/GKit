#if UNITY
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using GKit.Core;

namespace GKit {
	/// <summary>
	/// 입력 레이어를 관리하는 클래스입니다.
	/// </summary>
	public static class InputManager {
		private static List<InputLayer> layerList;

		public delegate void MouseOverlapDelegate(int layer, ref bool handleFlag, ref bool isLastHandle);
		public static event MouseOverlapDelegate OnMouseOverlap;

		static InputManager() {
			layerList = new List<InputLayer>();
		}
		public static void RegistLayer(InputLayer layer) {
			layerList.Add(layer);
		}
		public static void RemoveLayer(InputLayer layer) {
			layerList.Remove(layer);
		}
		internal static void Update() {
			InputMask.Clear();

			int setCount = layerList.Count;
			for (int setIndex=0; setIndex<setCount; ++setIndex) {
				InputLayer layer = layerList[setIndex];

				Camera cam = layer.camera;
				Vector3 originPos = cam.ScreenToWorldPoint(new Vector3(MouseInput.ScreenPos.x, MouseInput.ScreenPos.y, cam.nearClipPlane));
				Vector3 destPos = cam.ScreenToWorldPoint(new Vector3(MouseInput.ScreenPos.x, MouseInput.ScreenPos.y, cam.farClipPlane));

				//레이캐스트
				int hitMask = 1 << layer.layer;

				RaycastHit[] hitInfos =
				Physics.RaycastAll(
					originPos,
					destPos - originPos,
					cam.farClipPlane - cam.nearClipPlane,
					hitMask);
				Array.Sort<RaycastHit>(hitInfos, (x, y) => x.distance.CompareTo(y.distance));

				//포커스 작업
				layer.StartUpdate();

				if (hitInfos.Length > 0) {
					for (int i = 0; i < hitInfos.Length; ++i) {
						RaycastHit hitInfo = hitInfos[i];
						InputHandler inputHandler = hitInfo.collider.GetComponent<InputHandler>();

						if (inputHandler != null) {
							InputMask.Mark(inputHandler.writeMask);
							if (InputMask.Check(inputHandler.readMask)) {
								//마스크 통과
								bool handleFlag = true;
								bool isLastHandle = true;
								OnMouseOverlap?.Invoke(layer.layer, ref handleFlag, ref isLastHandle);

								if (handleFlag) {
									layer.Update(inputHandler);
								}
								if (isLastHandle) {
									break;
								}
							}
						}
					}
				}

				layer.EndUpdate();
			}
			
		}
	}
	
}
#endif