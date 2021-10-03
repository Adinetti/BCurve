using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BCurve { 
	public class CurveNode : MonoBehaviour {
        [SerializeField] private float _gizmoSize = 1f;
        [SerializeField] private CurveNode[] _neighbors;
        [SerializeField] private PointTangent[] _tangents;
        [SerializeField] private int _id;
        private Curve _curve;

        public int ID => _id;
        public Vector3 Position => transform.position;

        public void Init(int id, Curve curve) {
            _id = id;
            _curve = curve;
            if (_neighbors == null) {
                _neighbors = new CurveNode[2];
                CreateTangents();
            }
        }

        private void CreateTangents() {
            _tangents = new PointTangent[2];
            for (int i = 0; i < _tangents.Length; i++) {
                var go = new GameObject($"Tangent_{i}", typeof(PointTangent));                
                _tangents[i] = go.GetComponent<PointTangent>();
                _tangents[i].transform.parent = transform;
                var direction = i == 0 ? transform.forward : -transform.forward;
                _tangents[i].transform.position = transform.position + direction * 2 * _gizmoSize;
                _tangents[i].gameObject.SetActive(false);
            }
        }

        public bool RefreshNeighbors() {
            _tangents[0].gameObject.SetActive(false);
            _tangents[1].gameObject.SetActive(false);
            for (int i = 0; i < _curve.NodesCount; i++) {
                if (SetNeighbors(i)) {
                    return true;
                }
            }
            return false;
        }

        private bool SetNeighbors(int orderInCurve) {
            var point = _curve.GetNode(orderInCurve);
            if (point.ID == ID) {
                if (_curve.IsClosed || orderInCurve < _curve.NodesCount - 1) {
                    var nextOrder = (orderInCurve + 1) % _curve.NodesCount;
                    SetNeighbor(nextOrder, 1);
                }
                if (_curve.IsClosed || orderInCurve > 0) {
                    var prevOrder = (orderInCurve + _curve.NodesCount - 1) % _curve.NodesCount;
                    SetNeighbor(prevOrder, 0);
                }
                return true;
            }
            return false;
        }

        private void SetNeighbor(int orderInCurve, int neighborID) {
            _neighbors[neighborID] = _curve.GetNode(orderInCurve);
            _tangents[neighborID].gameObject.SetActive(true);
        }

        public Vector3 GetTangentPositionBy(CurveNode neighbor) {
            for (int i = 0; i < _neighbors.Length; i++) {
                if (_neighbors[i]?.ID == neighbor.ID) {
                    return _tangents[i].transform.position;
                }
            }
            return Vector3.zero;
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _gizmoSize);
            Gizmos.color = Color.blue;
            for (int i = 0; i < _tangents.Length; i++) {
                if (_tangents[i].gameObject.activeSelf) {
                    Gizmos.DrawLine(transform.position, _tangents[i].transform.position);
                }
            }
        }
    }
}


