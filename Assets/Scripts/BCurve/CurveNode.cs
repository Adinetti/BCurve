using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BCurve {
    [ExecuteAlways]
    public class CurveNode : MonoBehaviour {
        [SerializeField] private float _gizmoSize = 1f;
        [SerializeField] private CurveNode[] _neighbors;
        [SerializeField] private bool _withOneTangent = true;
        private BezierCurve _parentCurve;
        private Tangent[] _tangents;

        public Vector3 Position => transform.position;
        public bool WithOneTangent => _withOneTangent;

        private void Awake() {
            _parentCurve = GetComponentInParent<BezierCurve>();
            _tangents = GetComponentsInChildren<Tangent>();
        }

        public void Init(BezierCurve parent) {
            _parentCurve = parent;
            _neighbors = new CurveNode[2];
            CreateTangents();
        }

        private void CreateTangents() {
            _tangents = new Tangent[2];
            for (int i = 0; i < _tangents.Length; i++) {
                var go = new GameObject($"Tangent_{i}", typeof(Tangent));
                _tangents[i] = go.GetComponent<Tangent>();
                _tangents[i].transform.parent = transform;
                var direction = i == 0 ? transform.forward : -transform.forward;
                _tangents[i].transform.position = transform.position + direction * 5 * _gizmoSize;
            }
            _tangents[0].Init(this, _tangents[1]);
            _tangents[1].Init(this, _tangents[0]);
        }

        public bool RefreshNeighbors() {
            for (int i = 0; i < _parentCurve?.NodesCount; i++) {
                if (SetNeighbors(i)) {
                    return true;
                }
            }
            return false;
        }

        private bool SetNeighbors(int orderInCurve) {
            var point = _parentCurve?.GetNode(orderInCurve);
            if (point == this) {
                if (_parentCurve.IsClosed || orderInCurve > 0) {
                    var prevOrder = (orderInCurve + _parentCurve.NodesCount - 1) % _parentCurve.NodesCount;
                    _neighbors[0] = _parentCurve.GetNode(prevOrder);
                }
                if (_parentCurve.IsClosed || orderInCurve < _parentCurve.NodesCount - 1) {
                    var nextOrder = (orderInCurve + 1) % _parentCurve.NodesCount;
                    _neighbors[1] = _parentCurve.GetNode(nextOrder);
                }
                return true;
            }
            return false;
        }

        public Vector3 GetTangentPositionBy(CurveNode neighbor) {
            for (int i = 0; i < _neighbors.Length; i++) {
                if (_neighbors[i] == neighbor) {
                    return _tangents[i].transform.position;
                }
            }
            return Vector3.zero;
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _gizmoSize);
            Gizmos.color = Color.blue;
            for (int i = 0; i < _tangents?.Length; i++) {
                if (_tangents[i].gameObject.activeSelf) {
                    Gizmos.DrawLine(transform.position, _tangents[i].transform.position);
                }
            }
        }
    }
}


