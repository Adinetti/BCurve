using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BCurve {
    public class Curve : MonoBehaviour {
        [SerializeField] private CurveNode[] _nodes;
        [SerializeField] [Range(0, 1)] private float _timer;
        [SerializeField] private bool _isClosed;
        private int _nextPointID;

        public int NodesCount => _nodes.Length;
        public bool IsClosed => _isClosed;

        public void CreateNewPoint(Vector3 position) {
            RefreshPoints();
            var point = CreatePointIn(position);
            _nodes = GetComponentsInChildren<CurveNode>();
            _nextPointID += 1;
            RefreshPoints();
        }

        public void RefreshPoints() {
            _nodes = GetComponentsInChildren<CurveNode>();
            foreach (var point in _nodes) {
                if (point.ID >= _nextPointID) {
                    _nextPointID = point.ID + 1;
                }
                point.RefreshNeighbors();
            }
        }

        public CurveNode GetNode(int order) {
            return _nodes[order];
        }

        public Vector3 GetPoint(float time) {
            time = Mathf.Clamp01(time);
            if (_nodes == null) {
                return transform.position;
            }
            if (_nodes.Length == 1) {
                return _nodes[0].transform.position;
            }
            var endOrder = IsClosed ? _nodes.Length : _nodes.Length - 1;
            time = time * (endOrder);
            var dt  = (time) % 1;
            var segmentID  = (int)time;
            if (IsClosed || segmentID < _nodes.Length - 1) {
                return GetPoint(segmentID, dt);
            }
            return _nodes[NodesCount - 1].Position;
        }

        private Vector3 GetPoint(int segmentID, float dt) {
            var startNode = _nodes[segmentID % NodesCount];
            var endNode = _nodes[(segmentID + 1) % NodesCount];
            var startTangent = startNode.GetTangentPositionBy(endNode);
            var endTangent = endNode.GetTangentPositionBy(startNode);
            return GetPoint(startNode.Position, endNode.Position, startTangent, endTangent, dt);
        }

        private Vector3 GetPoint(Vector3 start, Vector3 end, Vector3 tangentA, Vector3 tangentB, float time) {
            var a = Vector3.Lerp(start, tangentA, time);
            var b = Vector3.Lerp(tangentA, tangentB, time);
            var c = Vector3.Lerp(tangentB, end, time);
            var d = Vector3.Lerp(a, b, time);
            var e = Vector3.Lerp(b, c, time);
            return Vector3.Lerp(d, e, time);
        }

        private CurveNode CreatePointIn(Vector3 position) {
            var go = new GameObject($"Point_{_nextPointID}", typeof(CurveNode));
            var point = go.GetComponent<CurveNode>();
            point.transform.position = position;
            point.transform.parent = transform;
            point.Init(_nextPointID, this);
            return point;
        }

        private void OnDrawGizmos() {
            foreach (var point in _nodes) {
                if (point == null) {
                    RefreshPoints();
                }
            }
            if (_nodes?.Length > 1) {
                DrawBezierCurve();
                var pointer = GetPoint(_timer);
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(pointer, 1);
            }
        }

        private void DrawBezierCurve() {
#if UNITY_EDITOR
            var endOrder = _isClosed ? _nodes.Length : _nodes.Length - 1;
            for (int i = 0; i < endOrder; i++) {
                var startCurve = _nodes[i];
                var endCurve = _nodes[(i + 1) % _nodes.Length];
                if (startCurve != null && endCurve != null) {
                    var startTangent = startCurve.GetTangentPositionBy(endCurve);
                    var endTangent = endCurve.GetTangentPositionBy(startCurve);
                    Handles.DrawBezier(startCurve.Position, endCurve.Position,
                                        startTangent, endTangent, Color.white, Texture2D.whiteTexture, 1f);
                }
            }
#endif
        }
    }
}


