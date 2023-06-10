using System.Collections;
using UnityEngine;

namespace CharlieMadeAThing.ScreamingInsects.ScreamingInsects {
    public class Insect : MonoBehaviour {
        [Header("Insect Stats")]
        [SerializeField] float screamRadius;
        [SerializeField] float speed;
        [SerializeField] InsectTarget target;
        [SerializeField] bool shouldShowScream;

        [Header("Insect Knowledge")]
        [SerializeField] float distanceToNest;
        [SerializeField] float distanceToFood;
        [SerializeField] Vector3 directionOfMovement;

        [Header("Insect References")]
        [SerializeField] SpriteRenderer spriteRenderer;
        [SerializeField] Rigidbody2D rBody2D;
        [SerializeField] LineRendererPooler lineRendererPooler;
        Collider2D[] _screamHits = new Collider2D[50];
    
        public float ScreamRadius => screamRadius;
    
        public void Init( float insectSpeed, float insectScreamRadius, LineRendererPooler linePooler ) {
            Physics2D.autoSyncTransforms = false;
            //Random initial direction, target, and distances to nest and food
            var startTarget = UnityEngine.Random.Range( 0, 2 ) == 0 ? InsectTarget.Nest : InsectTarget.Food;
            ChangeTarget( startTarget );
            directionOfMovement = new Vector3( UnityEngine.Random.Range( -1f, 1f ), UnityEngine.Random.Range( -1f, 1f ), 0 ).normalized;
            distanceToNest = UnityEngine.Random.Range( 0, 500 );
            distanceToFood = UnityEngine.Random.Range( 0, 500 );
            lineRendererPooler = linePooler;
            speed = insectSpeed;
            screamRadius = insectScreamRadius;
        
        }

    
        void Update() {
            var dt = Time.deltaTime;
            MoveInDirection( dt );
            distanceToNest += speed * dt;
            distanceToFood += speed * dt;
            Physics2D.SyncTransforms();
        }

        void DoScream( InsectTarget screamTarget ) {
            var hitCount = Physics2D.OverlapCircleNonAlloc( transform.position, screamRadius, _screamHits );
            if ( hitCount == 0 ) return;

            for ( var i = 0; i < hitCount; i++ ) {
                var hit = _screamHits[i];
                if ( !hit || !hit.CompareTag( "Insect" ) ) continue;

                var insect = hit.GetComponent<Insect>();
                var screamTargetDistance = screamTarget == InsectTarget.Nest ? distanceToNest : distanceToFood;
                var screamColor = screamTarget == InsectTarget.Nest ? Color.red : Color.green;
                var didUpdate =
                    insect.ListenToScream( transform.position, screamTarget, screamTargetDistance + screamRadius );

                if ( didUpdate && shouldShowScream ) {
                    if ( !lineRendererPooler ) return;
                    StartCoroutine( DrawLine( insect.transform.position, screamColor ) );
                }
            }
        }

        IEnumerator DrawLine( Vector3 targetPosition, Color screamColor ) {
            var lineRenderer = lineRendererPooler.GetLineRenderer();
            lineRenderer.SetPosition( 0, transform.position );
            lineRenderer.SetPosition( 1, targetPosition );
            lineRenderer.startColor = screamColor;
            lineRenderer.endColor = lineRenderer.startColor;
            yield return new WaitForSeconds( 0.05f );
            lineRendererPooler.ReleaseLineRenderer( lineRenderer );
        }

        bool ListenToScream( Vector3 transformPosition, InsectTarget screamTarget, float screamTargetDistance ) {
            switch ( screamTarget ) {
                case InsectTarget.Nest when screamTargetDistance < distanceToNest: {
                    UpdateDistanceToNestAndScream( screamTargetDistance, screamTarget );
                    if ( screamTarget == target ) {
                        directionOfMovement = (transformPosition - transform.position).normalized;
                    }

                    return true;
                }
                case InsectTarget.Food when screamTargetDistance < distanceToFood: {
                    UpdateDistanceToFoodAndScream( screamTargetDistance, screamTarget );
                    if ( screamTarget == target ) {
                        directionOfMovement = (transformPosition - transform.position).normalized;
                    }

                    return true;
                }
                default:
                    return false;
            }
        }
    
    

        void MoveInDirection( float dt ) {
            rBody2D.MovePosition( transform.position + directionOfMovement * (speed * dt) );
        }

        void ChangeTarget( InsectTarget nest ) {
            target = nest;
            spriteRenderer.color = target == InsectTarget.Nest ? Color.red : Color.green;
        }

        void UpdateDistanceToFoodAndScream( float distance, InsectTarget screamTarget ) {
            distanceToFood = distance;
            DoScream( screamTarget );
        }
    
        void UpdateDistanceToNestAndScream( float distance, InsectTarget screamTarget  ) {
            distanceToNest = distance;
            DoScream( screamTarget );
        }

        void OnTriggerEnter2D( Collider2D other ) {
            if ( other.CompareTag( "Nest" ) ) {
                UpdateDistanceToNestAndScream( 0, InsectTarget.Nest );
                if ( target == InsectTarget.Nest ) {
                    ChangeTarget( InsectTarget.Food );
                    //flip direction of movement
                    directionOfMovement *= -1;
                }
            }
            else if ( other.CompareTag( "FoodSource" ) ) {
                UpdateDistanceToFoodAndScream( 0, InsectTarget.Food );
                if ( target == InsectTarget.Food ) {
                    ChangeTarget( InsectTarget.Nest );
                    //flip direction of movement
                    directionOfMovement *= -1;
                }
            }

            if ( other.CompareTag( "Edge" ) ) {
                //Reflect move direction
                directionOfMovement = Vector3.Reflect( directionOfMovement, other.transform.up );
            }
        
        }
    }

    public enum InsectTarget {
        Nest,
        Food,
    }
}