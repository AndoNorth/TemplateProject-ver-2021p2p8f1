using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TemplateProject
{
    namespace PlayerController2D_
    {
        namespace PlayerController2D_Platformer
        {
            public class PlayerController2D : MonoBehaviour, IPlayerController2D
            {
                Vector2 _inputVector;
                private Rigidbody2D _rb;
                private CapsuleCollider2D _cCollider;
                // custom collider
                private int _noCollisionPoints;
                private CustomColliderRange _topRays, _bottomRays, _leftRays, _rightRays;
                private bool _topCol, _bottomCol, _leftCol, _rightCol;
                [Header("COLLISION")] [SerializeField] private Bounds _characterBounds;
                [SerializeField] private Vector3 _characterSize;
                [SerializeField] private LayerMask _collidable;
                [SerializeField] private float _detectionRayLength = 0.1f;
                [SerializeField] [Range(0.1f, 0.3f)] private float _rayBuffer = 0.1f; // minor lee-way so that side colliders dont overlap with bottom
                // IPlayerController2D
                public Vector2 Velocity => _rb.velocity;
                public FrameInputs Inputs { get; private set; }
                public bool Grounded => _bottomCol;
                // state machine
                private StateMachine _stateMachine;
                private void Awake()
                {
                    _rb = transform.GetComponent<Rigidbody2D>();
                    _cCollider = transform.GetComponent<CapsuleCollider2D>();
                    _characterBounds = _cCollider.bounds;
                    _characterSize = _characterBounds.size;
                }
                private void Update()
                {

                }
                private void FixedUpdate()
                {
                }
                // state machine example - using a basic one first, transition to this if required
                void SetupStateMachine()
                {
                    // shortcut to add transition
                    void At(IState from, IState to, Func<bool> condition) => _stateMachine.addTransition(from, to, condition);
                    // initialise states
                    /*
                    StartState start = new StartState(this);
                    ResolveRound resolveRound = new ResolveRound(this);
                    AssessBattle assessBattle = new AssessBattle(this);
                    End end = new End(this);
                    ResetState reset = new ResetState(this);
                    */
                    // initialise state machine
                    _stateMachine = new StateMachine();
                    // state transitions
                    /*
                    At(start, assessBattle, IsSetupComplete());
                    At(resolveRound, assessBattle, NoAttackersLeft());
                    At(assessBattle, resolveRound, AttackersLeft());
                    _stateMachine.addAnyTransition(reset, IsResetSet()); // note: reset transition must be set first, as a priority system
                    _stateMachine.addAnyTransition(start, IsSetupNotComplete());
                    _stateMachine.addAnyTransition(end, OneTeamDied());
                    */
                    // func bool methods
                    /*
                    Func<bool> IsSetupComplete() => () => SetupBool;
                    Func<bool> IsSetupNotComplete() => () => !SetupBool;
                    Func<bool> OneTeamDied() => () => !(AnyBotsAlive() && AnyPlayersAive());
                    Func<bool> NoAttackersLeft() => () => _charsCanAttack.Count() <= 0;
                    Func<bool> AttackersLeft() => () => _charsCanAttack.Count() > 0;
                    Func<bool> IsResetSet() => () => ResetBool;
                    */
                }
                private void AddMomentum(Vector2 force)
                {
                    _rb.AddForce(force);
                }
                private void SetMomentum(Vector2 force)
                {
                    _rb.velocity = force;
                }
                public void SetInputs(FrameInputs inputs)
                {
                    Inputs = inputs;
                }
                private void RunCollisionChecks()
                {
                    CalculateCustomColliderRanges();

                    _bottomCol = RunDetection(_bottomRays); // TODO: add coyote time
                    _topCol = RunDetection(_topRays);
                    _leftCol = RunDetection(_leftRays);
                    _rightCol = RunDetection(_rightRays);

                    bool RunDetection(CustomColliderRange range)
                    {
                        return EvaluateRayPositions(range).Any(point => Physics2D.Raycast(point, range.RayDirection, _detectionRayLength, _collidable));
                    }
                }
                private IEnumerable<Vector2> EvaluateRayPositions(CustomColliderRange range)
                {
                    for (var i = 0; i < _noCollisionPoints; i++)
                    {
                        var t = (float)i / (_noCollisionPoints - 1);
                        yield return Vector2.Lerp(range.Start, range.End, t);
                    }
                }
                private void CalculateCustomColliderRanges()
                {
                    // if not using collider, need to calculate new colliders
                    // _characterBounds = new Bounds(transform.position + _characterBounds.center, _characterBounds.size); // not sure if i understand this

                    _topRays    = new CustomColliderRange(_characterBounds.min.x + _rayBuffer, _characterBounds.max.y, _characterBounds.max.x - _rayBuffer, _characterBounds.max.y, Vector2.up);
                    _bottomRays = new CustomColliderRange(_characterBounds.min.x + _rayBuffer, _characterBounds.min.y, _characterBounds.max.x - _rayBuffer, _characterBounds.min.y, Vector2.down);
                    _leftRays   = new CustomColliderRange(_characterBounds.min.x, _characterBounds.min.y + _rayBuffer, _characterBounds.min.x, _characterBounds.max.y - _rayBuffer, Vector2.left);
                    _rightRays  = new CustomColliderRange(_characterBounds.max.x, _characterBounds.min.y + _rayBuffer, _characterBounds.max.x, _characterBounds.max.y - _rayBuffer, Vector2.right);
                }
                private void OnDrawGizmos()
                {
                    DrawGizmoCharacterBounds();
                    if (!Application.isPlaying)
                    {
                        DrawGizmoInputVector();
                        DrawGizmoCustomColliderRays();
                    }
                }
                private void DrawGizmoCharacterBounds()
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawWireCube(transform.position + _characterBounds.center, _characterBounds.size);
                }
                private void DrawGizmoCustomColliderRays()
                {
                    CalculateCustomColliderRanges();
                    Gizmos.color = Color.blue;
                    foreach (var range in new List<CustomColliderRange> { _topRays, _bottomRays, _leftRays, _rightRays })
                    {
                        foreach (var point in EvaluateRayPositions(range))
                        {
                            Gizmos.DrawRay(point, range.RayDirection * _detectionRayLength);
                        }
                    }
                }
                // debug directional input vector
                private void DrawGizmoInputVector()
                {
                    float gapFromPlayer = 0.1f;
                    Vector3 boxSize = new Vector3(0.3f, 0.3f);
                    Vector3 xDiff = new Vector3(_cCollider.bounds.extents.x + gapFromPlayer + boxSize.x, 0, 0);
                    Vector3 yDiff = new Vector3(0, _cCollider.bounds.extents.y + gapFromPlayer + boxSize.y, 0);
                    Gizmos.color = Color.blue;
                    if (_inputVector.x > 0 && _inputVector.y > 0)
                    {
                        // up left
                        Gizmos.DrawWireCube(_cCollider.bounds.center - xDiff + yDiff, new Vector3(boxSize.y, boxSize.x, 0));
                    }
                    else if (_inputVector.x < 0 && _inputVector.y > 0)
                    {
                        // up right
                        Gizmos.DrawWireCube(_cCollider.bounds.center + xDiff + yDiff, new Vector3(boxSize.y, boxSize.x, 0));
                    }
                    else if (_inputVector.x > 0 && _inputVector.y < 0)
                    {
                        // down left
                        Gizmos.DrawWireCube(_cCollider.bounds.center - xDiff - yDiff, new Vector3(boxSize.y, boxSize.x, 0));
                    }
                    else if (_inputVector.x < 0 && _inputVector.y < 0)
                    {
                        // down right
                        Gizmos.DrawWireCube(_cCollider.bounds.center + xDiff - yDiff, new Vector3(boxSize.y, boxSize.x, 0));
                    }
                    else if (_inputVector.y > 0)
                    {
                        // up
                        Gizmos.DrawWireCube(_cCollider.bounds.center + yDiff, new Vector3(boxSize.y, boxSize.x, 0));
                    }
                    else if (_inputVector.y < 0)
                    {
                        // down
                        Gizmos.DrawWireCube(_cCollider.bounds.center - yDiff, new Vector3(boxSize.y, boxSize.x, 0));
                    }
                    else if (_inputVector.x > 0)
                    {
                        // left
                        Gizmos.DrawWireCube(_cCollider.bounds.center - xDiff, new Vector3(boxSize.y, boxSize.x, 0));
                    }
                    else if (_inputVector.x < 0)
                    {
                        // right
                        Gizmos.DrawWireCube(_cCollider.bounds.center + xDiff, new Vector3(boxSize.y, boxSize.x, 0));
                    }
                    else
                    {
                        // none
                        Gizmos.DrawWireCube(_cCollider.bounds.center, new Vector3(boxSize.y, boxSize.x, 0));
                    }
                }
            }

            // custom physics variables
            public static class PlayerController2DPhysics
            {
                // gravity
                public static float _mass = 1f;
                public static float _gravity = -9.81f;
                public static float _minFallSpeed = 10f;
                public static float _maxFallSpeed = 20f;
                // horizontal movement
                public static float _maxHorizontalSpeed = 10f;
                public static float _acceleration = 80f;
                public static float _deceleration = 50f;
                // jump
            }
            // custom ray struct - used to draw vector between two Vector2 s
            public struct CustomColliderRange
            {
                public CustomColliderRange(float x1, float y1, float x2, float y2, Vector2 direction)
                {
                    Start = new Vector2(x1, y1);
                    End = new Vector2(x2, y2);
                    RayDirection = direction;
                }
                public readonly Vector2 Start, End, RayDirection;
            }
            public interface IPlayerController2D
            {
                public Vector2 Velocity { get; }
                public FrameInputs Inputs { get; }
                public bool Grounded { get; }
            }
            // inputs
            public struct FrameInputs
            {
                public bool UpDown;
                public bool UpUp;
                public bool DownDown;
                public bool DownUp;
                public bool LeftDown;
                public bool LeftUp;
                public bool RightDown;
                public bool RightUp;
                public bool JumpDown;
                public bool JumpUp;
                public bool LightAttackDown;
                public bool LightAttackUp;
                public bool HeavyAttackDown;
                public bool HeavyAttackUp;
                public bool OtherDown;
                public bool OtherUp;
            }
        }
    }
}
