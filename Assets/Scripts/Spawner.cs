using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharlieMadeAThing.ScreamingInsects
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] Insect insectPrefab;
        [SerializeField] BoxCollider2D spawnArea;
        [SerializeField] LineRendererPooler lineRendererPooler;
        [SerializeField] int insectCount;
        [SerializeField] float insectSpeed;
        [SerializeField] float insectScreamRadius;
        
        List<Insect> _insects = new List<Insect>();

        void Start() {
            for ( var i = 0; i < insectCount; i++ ) {
                var insect = Instantiate( insectPrefab );
                insect.transform.position = GetRandomPositionInSpawnArea();
                insect.Init( insectSpeed, insectScreamRadius, lineRendererPooler );
                _insects.Add( insect );
            }
        }

        Vector3 GetRandomPositionInSpawnArea() {
            var x = UnityEngine.Random.Range( spawnArea.bounds.min.x, spawnArea.bounds.max.x );
            var y = UnityEngine.Random.Range( spawnArea.bounds.min.y, spawnArea.bounds.max.y );
            return new Vector3( x, y, 0 );
        }
        
    }
}
