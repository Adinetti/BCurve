using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BCurve {
    [ExecuteAlways]
    public class Tangent : MonoBehaviour {
        [SerializeField] private float _gizmoSize = 0.5f;
        [SerializeField] private Tangent _neighbor;
        private Vector3 _neighborPosition;
        private CurveNode _parentNode;

        public Vector3 Position => transform.position;

        private void Awake() {
            _parentNode = GetComponentInParent<CurveNode>();
        }

        public void Init(CurveNode parent, Tangent neighbor) {
            _parentNode = parent;
            _neighbor = neighbor;
            _neighborPosition = neighbor.Position;
        }

        private void Update() {
            UpdatePosition();
        }

        private void UpdatePosition() {
            if (_neighbor != null && _neighbor.gameObject.activeSelf) {
                if (_parentNode.WithOneTangent && _neighborPosition != _neighbor.Position) {
                    _neighborPosition = _neighbor.Position;
                    var direction = _parentNode.Position - _neighborPosition;
                    transform.position = _parentNode.Position + direction;
                }
            }
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(transform.position, _gizmoSize);
        }
    }
}
