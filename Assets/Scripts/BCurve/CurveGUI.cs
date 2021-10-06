using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BCurve {
    [CustomEditor(typeof(BezierCurve))]
    public class CurveGUI : Editor {
        private BezierCurve _curve;

        public void OnEnable() {
            _curve = (BezierCurve)target;
            _curve.RefreshPoints();
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            if (GUILayout.Button("Create node")) {
                _curve.CreateNewNode(Vector3.zero);
            }
        }
    }
}


