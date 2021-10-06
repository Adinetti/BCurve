using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BCurve {
    [ExecuteAlways]
    public class BezierCurve : MonoBehaviour {
        [SerializeField] private CurveNode[] _nodes;
        [SerializeField] [Range(0, 1)] private float _timer;
        [SerializeField] private bool _isClosed;

        public int NodesCount => _nodes.Length;
        public bool IsClosed => _isClosed;

        private void Update() {
            if (NodeIsChanged()) {
                RefreshPoints();
            }
        }

        private bool NodeIsChanged() {
            foreach (var nodes in _nodes) {
                if (nodes == null) {
                    return true;
                }
            }
            return false;
        }

        public void RefreshPoints() {
            _nodes = GetComponentsInChildren<CurveNode>();
            foreach (var point in _nodes) {
                point.RefreshNeighbors();
            }
        }

        public void CreateNewNode(Vector3 position) {
            CreateNodeIn(position);
            RefreshPoints();
        }

        private void CreateNodeIn(Vector3 position) {
            var go = new GameObject($"Point_{_nodes.Length}", typeof(CurveNode));
            var point = go.GetComponent<CurveNode>();
            point.transform.position = position;
            point.transform.parent = transform;
            point.Init(this);
        }

        public CurveNode GetNode(int index) {
            return _nodes[index];
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
            var dt = (time) % 1;
            var segmentID = (int)time;
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

        private Vector3 GetPoint(Vector3 start, Vector3 end, Vector3 startTangent, Vector3 endTangent, float time) {
            var a = Vector3.Lerp(start, startTangent, time);
            var b = Vector3.Lerp(startTangent, endTangent, time);
            var c = Vector3.Lerp(endTangent, end, time);
            var ab = Vector3.Lerp(a, b, time);
            var bc = Vector3.Lerp(b, c, time);
            return Vector3.Lerp(ab, bc, time);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos() {
            if (_nodes?.Length > 1) {
                DrawBezierCurve();
                var pointer = GetPoint(_timer);
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(pointer, 1);
            }
        }

        private void DrawBezierCurve() {
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
        }
#endif
    }
}


