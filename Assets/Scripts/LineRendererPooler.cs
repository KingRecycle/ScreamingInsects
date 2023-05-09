using System;
using UnityEngine;
using UnityEngine.Pool;

namespace CharlieMadeAThing.ScreamingInsects {
    public class LineRendererPooler : MonoBehaviour {
        
        ObjectPool<LineRenderer> _lineRendererPool;
        [SerializeField] LineRenderer lineRendererPrefab;

        void Awake() {
            _lineRendererPool = new ObjectPool<LineRenderer>( CreateLineRenderer, OnTakeLineRendererFromPool, OnReturnLineRendererToPool );
        }

        LineRenderer CreateLineRenderer() {
            var lineRenderer = Instantiate( lineRendererPrefab );
            
            return lineRenderer;
        }

        void OnReturnLineRendererToPool( LineRenderer obj ) {
            obj.gameObject.SetActive( false );
        }

        void OnTakeLineRendererFromPool( LineRenderer obj ) {
            obj.gameObject.SetActive( true );
        }
        
        public LineRenderer GetLineRenderer() {
            return _lineRendererPool.Get();
        }
        
        public void ReleaseLineRenderer( LineRenderer lineRenderer ) {
            _lineRendererPool.Release( lineRenderer );
        }
    }
}