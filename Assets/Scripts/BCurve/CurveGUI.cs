using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BCurve { 
	[CustomEditor(typeof(Curve))]
	public class CurveGUI : Editor {
        private bool _isClosed;
        private Curve _curve;

        public void OnEnable() {
            _curve = (Curve)target;
            _isClosed = _curve.IsClosed;
        }


        public override void OnInspectorGUI() {
            base.OnInspectorGUI(); 
            if (GUILayout.Button("Create node")) {
                _curve.CreateNewPoint(Vector3.zero);
            }
            if (_isClosed != _curve.IsClosed) {
                _curve.RefreshPoints();
            }
        }
    }
}


