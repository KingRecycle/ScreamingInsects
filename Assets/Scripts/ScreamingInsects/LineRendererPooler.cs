using UnityEngine;
using UnityEngine.Pool;

namespace CharlieMadeAThing.ScreamingInsects.ScreamingInsects {
    public class LineRendererPooler : MonoBehaviour {
        
        IObjectPool<LineRenderer> _lineRendererPool;
        [SerializeField] LineRenderer lineRendererPrefab;

        void Awake() {
            _lineRendererPool = new ObjectPool<LineRenderer>( CreateLineRenderer, OnTakeLineRendererFromPool, OnReturnLineRendererToPool, collectionCheck: false, defaultCapacity: 500 );
            
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